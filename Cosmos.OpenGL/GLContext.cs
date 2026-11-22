using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using System.Runtime.InteropServices;
using Cosmos.Kernel.System.Graphics;
using Cosmos.Kernel.System.Graphics.Rendering3D;

namespace Cosmos.OpenGL;

public sealed class GLContext
{
    private static GLContext? _current;
    public static GLContext Current => _current ??= new GLContext();

    public Canvas3D? Canvas { get; private set; }

    // Matrix stacks
    private readonly Stack<Matrix4x4> _modelviewStack = new();
    private readonly Stack<Matrix4x4> _projectionStack = new();
    private readonly Stack<Matrix4x4> _textureStack = new();

    public Matrix4x4 ModelviewMatrix = Matrix4x4.Identity;
    public Matrix4x4 ProjectionMatrix = Matrix4x4.Identity;
    public Matrix4x4 TextureMatrix = Matrix4x4.Identity;

    public MatrixMode CurrentMatrixMode = MatrixMode.Modelview;

    // Viewport
    public int ViewportX { get; set; }
    public int ViewportY { get; set; }
    public int ViewportWidth { get; set; } = 1920;
    public int ViewportHeight { get; set; } = 1080;

    // Camera settings for Canvas3D
    public Vector3 CameraPosition = new(0f, 0f, 0f);
    public Vector3 CameraTarget = new(0f, 0f, -1f);
    public Vector3 CameraUp = Vector3.UnitY;
    public float CameraFovY = 60.0f;
    private bool _cameraCustomized = false;

    // Clear state
    public Color ClearColorValue = Color.Black;
    public float ClearDepthValue = 1.0f;

    // Current vertex attributes
    public uint CurrentColor = 0xFFFFFFFF; // White ARGB
    public Vector3 CurrentNormal = Vector3.UnitZ;
    public Vector2 CurrentTexCoord = Vector2.Zero;

    // Staging data for glBegin / glEnd
    private bool _inBeginEnd = false;
    private PrimitiveType _currentPrimitive = PrimitiveType.Triangles;
    private readonly List<Vector3> _stagedPositions = new(64);
    private readonly List<uint> _stagedColors = new(64);
    private readonly List<Vector2> _stagedUVs = new(64);
    private readonly List<Vector3> _stagedNormals = new(64);

    // State flags
    private readonly HashSet<EnableCap> _enabledCaps = new();

    public CullFaceMode CurrentCullFace = CullFaceMode.Back;
    public FrontFaceDirection CurrentFrontFace = FrontFaceDirection.Ccw;
    public DepthFunction CurrentDepthFunc = DepthFunction.Less;
    public bool DepthMaskEnabled = true;
    public ShadingModel CurrentShadeModel = ShadingModel.Smooth;
    public BlendingFactorSrc BlendSrc = BlendingFactorSrc.SrcAlpha;
    public BlendingFactorDest BlendDest = BlendingFactorDest.OneMinusSrcAlpha;

    // Texture tracking
    private readonly Dictionary<int, Texture> _textures = new();
    private int _boundTextureId = 0;
    private int _nextTextureId = 1;

    // Error code
    public ErrorCode LastError = ErrorCode.NoError;

    public GLContext()
    {
        _enabledCaps.Add(EnableCap.DepthTest);
    }

    public void SetCanvas(Canvas3D canvas)
    {
        ClearMeshCache();
        Canvas = canvas;
        ViewportWidth = canvas.Width;
        ViewportHeight = canvas.Height;
        UpdateCamera();
    }

    public void UpdateCamera()
    {
        if (Canvas != null)
        {
            Canvas.Camera = new Camera3D(CameraPosition, CameraTarget, CameraUp, CameraFovY);
        }
    }

    public void SetCamera(Vector3 position, Vector3 target, Vector3 up, float fovY)
    {
        CameraPosition = position;
        CameraTarget = target;
        CameraUp = up;
        CameraFovY = fovY;
        _cameraCustomized = true;
        UpdateCamera();
    }

    // -------------------------------------------------------------
    // Matrix stack operations
    // -------------------------------------------------------------

    public ref Matrix4x4 CurrentMatrix
    {
        get
        {
            switch (CurrentMatrixMode)
            {
                case MatrixMode.Projection:
                    return ref ProjectionMatrix;
                case MatrixMode.Texture:
                    return ref TextureMatrix;
                default:
                    return ref ModelviewMatrix;
            }
        }
    }

    public void PushMatrix()
    {
        switch (CurrentMatrixMode)
        {
            case MatrixMode.Projection:
                _projectionStack.Push(ProjectionMatrix);
                break;
            case MatrixMode.Texture:
                _textureStack.Push(TextureMatrix);
                break;
            default:
                _modelviewStack.Push(ModelviewMatrix);
                break;
        }
    }

    public void PopMatrix()
    {
        switch (CurrentMatrixMode)
        {
            case MatrixMode.Projection:
                if (_projectionStack.Count > 0) ProjectionMatrix = _projectionStack.Pop();
                else LastError = ErrorCode.StackUnderflow;
                break;
            case MatrixMode.Texture:
                if (_textureStack.Count > 0) TextureMatrix = _textureStack.Pop();
                else LastError = ErrorCode.StackUnderflow;
                break;
            default:
                if (_modelviewStack.Count > 0) ModelviewMatrix = _modelviewStack.Pop();
                else LastError = ErrorCode.StackUnderflow;
                break;
        }
    }

    public void LoadIdentity()
    {
        CurrentMatrix = Matrix4x4.Identity;
    }

    public void LoadMatrix(in Matrix4x4 m)
    {
        CurrentMatrix = m;
    }

    public void MultMatrix(in Matrix4x4 m)
    {
        // In row-major System.Numerics:
        // Multiplying CurrentMatrix * m preserves the OpenGL convention that subsequent transforms affect geometry first.
        CurrentMatrix = m * CurrentMatrix;
    }

    public void Translate(float x, float y, float z)
    {
        MultMatrix(Matrix4x4.CreateTranslation(x, y, z));
    }

    public void Rotate(float angleDeg, float x, float y, float z)
    {
        Vector3 axis = new(x, y, z);
        if (axis.LengthSquared() < 0.0001f) return;
        axis = Vector3.Normalize(axis);
        float radians = angleDeg * (MathF.PI / 180.0f);
        MultMatrix(Matrix4x4.CreateFromAxisAngle(axis, radians));
    }

    public void Scale(float x, float y, float z)
    {
        MultMatrix(Matrix4x4.CreateScale(x, y, z));
    }

    public void Ortho(double left, double right, double bottom, double top, double zNear, double zFar)
    {
        float l = (float)left, r = (float)right, b = (float)bottom, t = (float)top, n = (float)zNear, f = (float)zFar;
        Matrix4x4 ortho = new(
            2f / (r - l), 0f,           0f,           0f,
            0f,           2f / (t - b), 0f,           0f,
            0f,           0f,          -2f / (f - n), 0f,
            -(r + l) / (r - l), -(t + b) / (t - b), -(f + n) / (f - n), 1f
        );
        MultMatrix(ortho);
    }

    private static float FastAtan(float x)
    {
        bool neg = x < 0;
        if (neg) x = -x;
        bool inv = x > 1.0f;
        if (inv) x = 1.0f / x;
        float r = x / (1.0f + 0.28086f * x * x);
        if (inv) r = 1.5707963f - r;
        return neg ? -r : r;
    }

    public void Frustum(double left, double right, double bottom, double top, double zNear, double zFar)
    {
        float l = (float)left, r = (float)right, b = (float)bottom, t = (float)top, n = (float)zNear, f = (float)zFar;
        Matrix4x4 frustum = new(
            (2f * n) / (r - l), 0f,                 0f,                  0f,
            0f,                 (2f * n) / (t - b), 0f,                  0f,
            (r + l) / (r - l),  (t + b) / (t - b),  -(f + n) / (f - n), -1f,
            0f,                 0f,                 -(2f * f * n) / (f - n), 0f
        );
        MultMatrix(frustum);

        // Derive vertical FOV in degrees for Canvas3D camera (using FastAtan to avoid atanf linker dependency in Cosmos NativeAOT)
        float fovY = 2.0f * FastAtan((t - b) / (2.0f * n)) * (180.0f / 3.14159265f);
        CameraFovY = fovY;
        if (!_cameraCustomized)
        {
            UpdateCamera();
        }
    }

    public void Perspective(double fovy, double aspect, double zNear, double zFar)
    {
        CameraFovY = (float)fovy;
        double halfAngle = (fovy * 3.141592653589793) / 360.0;
        double cosVal = Math.Cos(halfAngle);
        double tanVal = cosVal != 0.0 ? Math.Sin(halfAngle) / cosVal : 0.0;
        double ymax = zNear * tanVal;
        double ymin = -ymax;
        double xmin = ymin * aspect;
        double xmax = ymax * aspect;
        Frustum(xmin, xmax, ymin, ymax, zNear, zFar);
        CameraFovY = (float)fovy;
        if (!_cameraCustomized)
        {
            UpdateCamera();
        }
    }

    // -------------------------------------------------------------
    // Enable / Disable States
    // -------------------------------------------------------------

    public void Enable(EnableCap cap) => _enabledCaps.Add(cap);
    public void Disable(EnableCap cap) => _enabledCaps.Remove(cap);
    public bool IsEnabled(EnableCap cap) => _enabledCaps.Contains(cap);

    // -------------------------------------------------------------
    // Staged vertex operations (glBegin / glEnd)
    // -------------------------------------------------------------

    public void Begin(PrimitiveType mode)
    {
        if (_inBeginEnd)
        {
            LastError = ErrorCode.InvalidOperation;
            return;
        }

        _inBeginEnd = true;
        _currentPrimitive = mode;
        _stagedPositions.Clear();
        _stagedColors.Clear();
        _stagedUVs.Clear();
        _stagedNormals.Clear();
    }

    public void Vertex(float x, float y, float z)
    {
        if (!_inBeginEnd)
        {
            LastError = ErrorCode.InvalidOperation;
            return;
        }

        _stagedPositions.Add(new Vector3(x, y, z));
        _stagedColors.Add(CurrentColor);
        _stagedUVs.Add(CurrentTexCoord);
        _stagedNormals.Add(CurrentNormal);
    }

    public void End()
    {
        if (!_inBeginEnd)
        {
            LastError = ErrorCode.InvalidOperation;
            return;
        }

        _inBeginEnd = false;

        if (Canvas == null || _stagedPositions.Count == 0) return;

        // Convert the staged vertices into triangular mesh indices or lines
        AssembleAndDraw();
    }

    private void AssembleAndDraw()
    {
        int count = _stagedPositions.Count;
        if (count == 0) return;

        switch (_currentPrimitive)
        {
            case PrimitiveType.Lines:
                DrawLinesList();
                break;
            case PrimitiveType.LineStrip:
                DrawLineStrip();
                break;
            case PrimitiveType.LineLoop:
                DrawLineLoop();
                break;
            case PrimitiveType.Points:
                DrawPoints();
                break;
            case PrimitiveType.Triangles:
            case PrimitiveType.Quads:
            case PrimitiveType.TriangleStrip:
            case PrimitiveType.TriangleFan:
            case PrimitiveType.Polygon:
            case PrimitiveType.QuadStrip:
                DrawTriangularMesh();
                break;
        }
    }

    private void DrawLinesList()
    {
        int count = _stagedPositions.Count;
        for (int i = 0; i + 1 < count; i += 2)
        {
            Vector3 v0 = Vector3.Transform(_stagedPositions[i], ModelviewMatrix);
            Vector3 v1 = Vector3.Transform(_stagedPositions[i + 1], ModelviewMatrix);
            Color col = Color.FromArgb((int)_stagedColors[i]);
            Canvas!.DrawLine3D(v0, v1, col);
        }
    }

    private void DrawLineStrip()
    {
        int count = _stagedPositions.Count;
        for (int i = 0; i + 1 < count; i++)
        {
            Vector3 v0 = Vector3.Transform(_stagedPositions[i], ModelviewMatrix);
            Vector3 v1 = Vector3.Transform(_stagedPositions[i + 1], ModelviewMatrix);
            Color col = Color.FromArgb((int)_stagedColors[i]);
            Canvas!.DrawLine3D(v0, v1, col);
        }
    }

    private void DrawLineLoop()
    {
        int count = _stagedPositions.Count;
        if (count < 2) return;
        DrawLineStrip();
        Vector3 vLast = Vector3.Transform(_stagedPositions[count - 1], ModelviewMatrix);
        Vector3 vFirst = Vector3.Transform(_stagedPositions[0], ModelviewMatrix);
        Color col = Color.FromArgb((int)_stagedColors[count - 1]);
        Canvas!.DrawLine3D(vLast, vFirst, col);
    }

    private void DrawPoints()
    {
        int count = _stagedPositions.Count;
        Vector3 size = new(0.04f, 0.04f, 0.04f);
        for (int i = 0; i < count; i++)
        {
            Vector3 pos = Vector3.Transform(_stagedPositions[i], ModelviewMatrix);
            Color col = Color.FromArgb((int)_stagedColors[i]);
            Canvas!.DrawCube(pos, size, col);
        }
    }

    private void DrawTriangularMesh()
    {
        List<ushort> indices = new(64);
        int count = _stagedPositions.Count;

        switch (_currentPrimitive)
        {
            case PrimitiveType.Triangles:
                for (ushort i = 0; i + 2 < count; i += 3)
                {
                    indices.Add(i);
                    indices.Add((ushort)(i + 1));
                    indices.Add((ushort)(i + 2));
                }
                break;

            case PrimitiveType.Quads:
                for (ushort i = 0; i + 3 < count; i += 4)
                {
                    indices.Add(i);
                    indices.Add((ushort)(i + 1));
                    indices.Add((ushort)(i + 2));

                    indices.Add(i);
                    indices.Add((ushort)(i + 2));
                    indices.Add((ushort)(i + 3));
                }
                break;

            case PrimitiveType.TriangleStrip:
                for (ushort i = 0; i + 2 < count; i++)
                {
                    if ((i & 1) == 0)
                    {
                        indices.Add(i);
                        indices.Add((ushort)(i + 1));
                        indices.Add((ushort)(i + 2));
                    }
                    else
                    {
                        indices.Add(i);
                        indices.Add((ushort)(i + 2));
                        indices.Add((ushort)(i + 1));
                    }
                }
                break;

            case PrimitiveType.TriangleFan:
            case PrimitiveType.Polygon:
                for (ushort i = 1; i + 1 < count; i++)
                {
                    indices.Add(0);
                    indices.Add(i);
                    indices.Add((ushort)(i + 1));
                }
                break;

            case PrimitiveType.QuadStrip:
                for (ushort i = 0; i + 3 < count; i += 2)
                {
                    indices.Add(i);
                    indices.Add((ushort)(i + 1));
                    indices.Add((ushort)(i + 2));

                    indices.Add((ushort)(i + 1));
                    indices.Add((ushort)(i + 3));
                    indices.Add((ushort)(i + 2));
                }
                break;
        }

        if (indices.Count == 0) return;

        ReadOnlySpan<Vector3> posSpan = CollectionsMarshal.AsSpan(_stagedPositions);
        ReadOnlySpan<ushort> idxSpan = CollectionsMarshal.AsSpan(indices);

        Mesh? mesh = null;
        Texture? tex = null;
        bool useTexture = IsEnabled(EnableCap.Texture2D) && _boundTextureId != 0 && _textures.TryGetValue(_boundTextureId, out tex);

        ReadOnlySpan<Vector2> uvSpan = useTexture ? CollectionsMarshal.AsSpan(_stagedUVs) : default;
        ReadOnlySpan<uint> colSpan = !useTexture ? CollectionsMarshal.AsSpan(_stagedColors) : default;

        ulong key = ComputeMeshKey(posSpan, colSpan, uvSpan, idxSpan, useTexture, _boundTextureId);

        if (!_meshCache.TryGetValue(key, out var cached))
        {
            if (useTexture && tex != null)
            {
                mesh = Canvas!.CreateMesh(posSpan, uvSpan, tex, idxSpan);
            }
            else
            {
                mesh = Canvas!.CreateMesh(posSpan, colSpan, idxSpan);
            }

            if (mesh != null)
            {
                cached = new CachedMesh
                {
                    Mesh = mesh,
                    Key = key,
                    LastFrameUsed = _currentFrame
                };
                _meshCache[key] = cached;
            }
        }
        else
        {
            cached.LastFrameUsed = _currentFrame;
            mesh = cached.Mesh;
        }

        if (mesh != null)
        {
            Matrix4x4 mat = ModelviewMatrix;
            Canvas!.DrawMesh(mesh, in mat);
        }
    }

    // -------------------------------------------------------------
    // Mesh Cache & Lifecycle Management
    // -------------------------------------------------------------

    private sealed class CachedMesh
    {
        public Mesh Mesh = null!;
        public ulong Key;
        public uint LastFrameUsed;
    }

    private readonly Dictionary<ulong, CachedMesh> _meshCache = new(64);
    private uint _currentFrame = 1;

    private static ulong ComputeMeshKey(ReadOnlySpan<Vector3> positions, ReadOnlySpan<uint> colors, ReadOnlySpan<Vector2> uvs, ReadOnlySpan<ushort> indices, bool useTexture, int textureId)
    {
        ulong hash = 14695981039346656037UL;
        hash ^= (ulong)positions.Length;
        hash *= 1099511628211UL;
        hash ^= (ulong)indices.Length;
        hash *= 1099511628211UL;
        hash ^= (ulong)(useTexture ? (textureId + 1) : 0);
        hash *= 1099511628211UL;

        for (int i = 0; i < positions.Length; i++)
        {
            Vector3 p = positions[i];
            hash ^= (ulong)BitConverter.SingleToUInt32Bits(p.X);
            hash *= 1099511628211UL;
            hash ^= (ulong)BitConverter.SingleToUInt32Bits(p.Y);
            hash *= 1099511628211UL;
            hash ^= (ulong)BitConverter.SingleToUInt32Bits(p.Z);
            hash *= 1099511628211UL;
        }

        if (useTexture)
        {
            for (int i = 0; i < uvs.Length; i++)
            {
                Vector2 uv = uvs[i];
                hash ^= (ulong)BitConverter.SingleToUInt32Bits(uv.X);
                hash *= 1099511628211UL;
                hash ^= (ulong)BitConverter.SingleToUInt32Bits(uv.Y);
                hash *= 1099511628211UL;
            }
        }
        else
        {
            for (int i = 0; i < colors.Length; i++)
            {
                hash ^= (ulong)colors[i];
                hash *= 1099511628211UL;
            }
        }

        for (int i = 0; i < indices.Length; i++)
        {
            hash ^= (ulong)indices[i];
            hash *= 1099511628211UL;
        }

        return hash;
    }

    public void OnFramePresented()
    {
        _currentFrame++;

        // Safely evict meshes not reused for 2 or more frames (safe to dispose after GPU has synced)
        List<ulong>? toRemove = null;
        foreach (var kvp in _meshCache)
        {
            if (_currentFrame - kvp.Value.LastFrameUsed >= 2)
            {
                toRemove ??= new();
                toRemove.Add(kvp.Key);
            }
        }

        if (toRemove != null)
        {
            for (int i = 0; i < toRemove.Count; i++)
            {
                if (_meshCache.Remove(toRemove[i], out var evicted))
                {
                    evicted.Mesh.Dispose();
                }
            }
        }
    }

    public void ClearMeshCache()
    {
        foreach (var kvp in _meshCache)
        {
            kvp.Value.Mesh.Dispose();
        }
        _meshCache.Clear();
    }

    // -------------------------------------------------------------
    // Texture Management
    // -------------------------------------------------------------

    public int GenTexture()
    {
        return _nextTextureId++;
    }

    public void BindTexture(TextureTarget target, int id)
    {
        _boundTextureId = id;
    }

    public void RegisterTexture(int id, Texture tex)
    {
        if (_textures.TryGetValue(id, out var oldTex))
        {
            oldTex.Dispose();
        }
        _textures[id] = tex;
    }

    public void DeleteTexture(int id)
    {
        if (_textures.TryGetValue(id, out var tex))
        {
            tex.Dispose();
            _textures.Remove(id);
        }
        if (_boundTextureId == id) _boundTextureId = 0;
    }
}
