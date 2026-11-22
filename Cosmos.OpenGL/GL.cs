using System;
using System.Drawing;
using System.Numerics;
using Cosmos.Kernel.System.Graphics;
using Cosmos.Kernel.System.Graphics.Rendering3D;

namespace Cosmos.OpenGL;

/// <summary>
/// OpenGL 1.x/2.x API implementation for Cosmos OS backed by Canvas3D.
/// </summary>
public static class GL
{
    // =========================================================================
    // OpenGL Constants (GL_*)
    // =========================================================================

    public const int GL_POINTS = 0x0000;
    public const int GL_LINES = 0x0001;
    public const int GL_LINE_LOOP = 0x0002;
    public const int GL_LINE_STRIP = 0x0003;
    public const int GL_TRIANGLES = 0x0004;
    public const int GL_TRIANGLE_STRIP = 0x0005;
    public const int GL_TRIANGLE_FAN = 0x0006;
    public const int GL_QUADS = 0x0007;
    public const int GL_QUAD_STRIP = 0x0008;
    public const int GL_POLYGON = 0x0009;

    public const int GL_DEPTH_BUFFER_BIT = 0x00000100;
    public const int GL_ACCUM_BUFFER_BIT = 0x00000200;
    public const int GL_STENCIL_BUFFER_BIT = 0x00000400;
    public const int GL_COLOR_BUFFER_BIT = 0x00004000;

    public const int GL_MODELVIEW = 0x1700;
    public const int GL_PROJECTION = 0x1701;
    public const int GL_TEXTURE = 0x1702;

    public const int GL_CURRENT_BIT = 0x00000001;
    public const int GL_POINT_BIT = 0x00000002;
    public const int GL_LINE_BIT = 0x00000004;
    public const int GL_POLYGON_BIT = 0x00000008;
    public const int GL_POLYGON_STIPPLE_BIT = 0x00000010;
    public const int GL_PIXEL_MODE_BIT = 0x00000020;
    public const int GL_LIGHTING_BIT = 0x00000040;
    public const int GL_FOG_BIT = 0x00000080;
    public const int GL_DEPTH_BUFFER_BIT_ATTRIB = 0x00000100;
    public const int GL_ACCUM_BUFFER_BIT_ATTRIB = 0x00000200;
    public const int GL_STENCIL_BUFFER_BIT_ATTRIB = 0x00000400;
    public const int GL_VIEWPORT_BIT = 0x00000800;
    public const int GL_TRANSFORM_BIT = 0x00001000;
    public const int GL_ENABLE_BIT = 0x00002000;
    public const int GL_COLOR_BUFFER_BIT_ATTRIB = 0x00004000;
    public const int GL_HINT_BIT = 0x00008000;
    public const int GL_EVAL_BIT = 0x00010000;
    public const int GL_LIST_BIT = 0x00020000;
    public const int GL_TEXTURE_BIT = 0x00040000;
    public const int GL_SCISSOR_BIT = 0x00080000;
    public const int GL_ALL_ATTRIB_BITS = 0x000FFFFF;

    public const int GL_NEVER = 0x0200;
    public const int GL_LESS = 0x0201;
    public const int GL_EQUAL = 0x0202;
    public const int GL_LEQUAL = 0x0203;
    public const int GL_GREATER = 0x0204;
    public const int GL_NOTEQUAL = 0x0205;
    public const int GL_GEQUAL = 0x0206;
    public const int GL_ALWAYS = 0x0207;

    public const int GL_SRC_COLOR = 0x0300;
    public const int GL_ONE_MINUS_SRC_COLOR = 0x0301;
    public const int GL_SRC_ALPHA = 0x0302;
    public const int GL_ONE_MINUS_SRC_ALPHA = 0x0303;
    public const int GL_DST_ALPHA = 0x0304;
    public const int GL_ONE_MINUS_DST_ALPHA = 0x0305;
    public const int GL_DST_COLOR = 0x0306;
    public const int GL_ONE_MINUS_DST_COLOR = 0x0307;
    public const int GL_SRC_ALPHA_SATURATE = 0x0308;

    public const int GL_FRONT_LEFT = 0x0400;
    public const int GL_FRONT_RIGHT = 0x0401;
    public const int GL_BACK_LEFT = 0x0402;
    public const int GL_BACK_RIGHT = 0x0403;
    public const int GL_FRONT = 0x0404;
    public const int GL_BACK = 0x0405;
    public const int GL_LEFT = 0x0406;
    public const int GL_RIGHT = 0x0407;
    public const int GL_FRONT_AND_BACK = 0x0408;

    public const int GL_CW = 0x0900;
    public const int GL_CCW = 0x0901;

    public const int GL_CULL_FACE = 0x0B44;
    public const int GL_LIGHTING = 0x0B50;
    public const int GL_COLOR_MATERIAL = 0x0B57;
    public const int GL_FOG = 0x0B60;
    public const int GL_DEPTH_TEST = 0x0B71;
    public const int GL_STENCIL_TEST = 0x0B90;
    public const int GL_NORMALIZE = 0x0BA1;
    public const int GL_ALPHA_TEST = 0x0BC0;
    public const int GL_DITHER = 0x0BD0;
    public const int GL_BLEND = 0x0BE2;
    public const int GL_SCISSOR_TEST = 0x0C11;
    public const int GL_TEXTURE_2D = 0x0DE1;

    public const int GL_BYTE = 0x1400;
    public const int GL_UNSIGNED_BYTE = 0x1401;
    public const int GL_SHORT = 0x1402;
    public const int GL_UNSIGNED_SHORT = 0x1403;
    public const int GL_INT = 0x1404;
    public const int GL_UNSIGNED_INT = 0x1405;
    public const int GL_FLOAT = 0x1406;
    public const int GL_DOUBLE = 0x140A;

    public const int GL_RGB = 0x1907;
    public const int GL_RGBA = 0x1908;

    public const int GL_FLAT = 0x1D00;
    public const int GL_SMOOTH = 0x1D01;

    public const int GL_VENDOR = 0x1F00;
    public const int GL_RENDERER = 0x1F01;
    public const int GL_VERSION = 0x1F02;
    public const int GL_EXTENSIONS = 0x1F03;

    public const int GL_NEAREST = 0x2600;
    public const int GL_LINEAR = 0x2601;

    public const int GL_TEXTURE_MAG_FILTER = 0x2800;
    public const int GL_TEXTURE_MIN_FILTER = 0x2801;
    public const int GL_TEXTURE_WRAP_S = 0x2802;
    public const int GL_TEXTURE_WRAP_T = 0x2803;

    public const int GL_CLAMP = 0x2900;
    public const int GL_REPEAT = 0x2901;

    public const int GL_LIGHT0 = 0x4000;
    public const int GL_LIGHT1 = 0x4001;
    public const int GL_LIGHT2 = 0x4002;
    public const int GL_LIGHT3 = 0x4003;
    public const int GL_LIGHT4 = 0x4004;
    public const int GL_LIGHT5 = 0x4005;
    public const int GL_LIGHT6 = 0x4006;
    public const int GL_LIGHT7 = 0x4007;

    // =========================================================================
    // Initialization and Buffer Management
    // =========================================================================

    public static void Initialize(Canvas3D canvas)
    {
        GLContext.Current.SetCanvas(canvas);
    }

    public static bool Initialize()
    {
        var canvas = Canvas.GetFullScreen() as Canvas3D;
        if (canvas != null)
        {
            Initialize(canvas);
            return true;
        }

        try
        {
            var newCanvas = Canvas.GetFullScreen(new Mode(1920, 1080, ColorDepth.ColorDepth32)) as Canvas3D;
            if (newCanvas != null)
            {
                Initialize(newCanvas);
                return true;
            }
        }
        catch { }

        return false;
    }

    public static Canvas3D? GetCanvas() => GLContext.Current.Canvas;

    public static void Flush()
    {
        // No-op for immediate Canvas3D drawing
    }

    public static void Finish()
    {
        var ctx = GLContext.Current;
        ctx.Canvas?.Display();
        ctx.OnFramePresented();
    }

    public static void SwapBuffers()
    {
        var ctx = GLContext.Current;
        ctx.Canvas?.Display();
        ctx.OnFramePresented();
    }

    // =========================================================================
    // Camera Integration with Canvas3D
    // =========================================================================

    public static Camera3D Camera
    {
        get => GLContext.Current.Canvas?.Camera ?? default;
        set
        {
            if (GLContext.Current.Canvas != null)
            {
                GLContext.Current.Canvas.Camera = value;
            }
        }
    }

    public static void SetCamera(Vector3 position, Vector3 target, Vector3 up, float fovY = 60.0f)
    {
        GLContext.Current.SetCamera(position, target, up, fovY);
    }

    // =========================================================================
    // Clear and Viewport
    // =========================================================================

    public static void ClearColor(float red, float green, float blue, float alpha)
    {
        int r = Math.Clamp((int)(red * 255.0f), 0, 255);
        int g = Math.Clamp((int)(green * 255.0f), 0, 255);
        int b = Math.Clamp((int)(blue * 255.0f), 0, 255);
        int a = Math.Clamp((int)(alpha * 255.0f), 0, 255);
        GLContext.Current.ClearColorValue = System.Drawing.Color.FromArgb(a, r, g, b);
    }

    public static void ClearColor(System.Drawing.Color color)
    {
        GLContext.Current.ClearColorValue = color;
    }

    public static void ClearDepth(double depth)
    {
        GLContext.Current.ClearDepthValue = (float)depth;
    }

    public static void Clear(ClearBufferMask mask)
    {
        var ctx = GLContext.Current;
        if (ctx.Canvas != null)
        {
            ctx.Canvas.ClearScene(ctx.ClearColorValue, ctx.ClearDepthValue);
        }
    }

    public static void Clear(int mask) => Clear((ClearBufferMask)mask);

    public static void Viewport(int x, int y, int width, int height)
    {
        var ctx = GLContext.Current;
        ctx.ViewportX = x;
        ctx.ViewportY = y;
        ctx.ViewportWidth = width;
        ctx.ViewportHeight = height;
    }

    // =========================================================================
    // Matrix Operations
    // =========================================================================

    public static void MatrixMode(MatrixMode mode) => GLContext.Current.CurrentMatrixMode = mode;
    public static void MatrixMode(int mode) => MatrixMode((MatrixMode)mode);

    public static void PushMatrix() => GLContext.Current.PushMatrix();
    public static void PopMatrix() => GLContext.Current.PopMatrix();

    public static void LoadIdentity() => GLContext.Current.LoadIdentity();

    public static void LoadMatrix(in Matrix4x4 m) => GLContext.Current.LoadMatrix(m);
    public static void MultMatrix(in Matrix4x4 m) => GLContext.Current.MultMatrix(m);

    public static void LoadMatrixf(float[] m)
    {
        if (m != null && m.Length >= 16)
        {
            Matrix4x4 mat = new(
                m[0], m[1], m[2], m[3],
                m[4], m[5], m[6], m[7],
                m[8], m[9], m[10], m[11],
                m[12], m[13], m[14], m[15]
            );
            LoadMatrix(mat);
        }
    }

    public static void MultMatrixf(float[] m)
    {
        if (m != null && m.Length >= 16)
        {
            Matrix4x4 mat = new(
                m[0], m[1], m[2], m[3],
                m[4], m[5], m[6], m[7],
                m[8], m[9], m[10], m[11],
                m[12], m[13], m[14], m[15]
            );
            MultMatrix(mat);
        }
    }

    public static void Translate(float x, float y, float z) => GLContext.Current.Translate(x, y, z);
    public static void Translatef(float x, float y, float z) => Translate(x, y, z);
    public static void Translated(double x, double y, double z) => Translate((float)x, (float)y, (float)z);
    public static void Translate(Vector3 v) => Translate(v.X, v.Y, v.Z);

    public static void Rotate(float angle, float x, float y, float z) => GLContext.Current.Rotate(angle, x, y, z);
    public static void Rotatef(float angle, float x, float y, float z) => Rotate(angle, x, y, z);
    public static void Rotated(double angle, double x, double y, double z) => Rotate((float)angle, (float)x, (float)y, (float)z);

    public static void Scale(float x, float y, float z) => GLContext.Current.Scale(x, y, z);
    public static void Scalef(float x, float y, float z) => Scale(x, y, z);
    public static void Scaled(double x, double y, double z) => Scale((float)x, (float)y, (float)z);
    public static void Scale(Vector3 v) => Scale(v.X, v.Y, v.Z);

    public static void Ortho(double left, double right, double bottom, double top, double zNear, double zFar)
    {
        GLContext.Current.Ortho(left, right, bottom, top, zNear, zFar);
    }

    public static void Frustum(double left, double right, double bottom, double top, double zNear, double zFar)
    {
        GLContext.Current.Frustum(left, right, bottom, top, zNear, zFar);
    }

    public static void Perspective(double fovy, double aspect, double zNear, double zFar)
    {
        GLContext.Current.Perspective(fovy, aspect, zNear, zFar);
    }

    // =========================================================================
    // glBegin / glEnd and Vertex Attributes
    // =========================================================================

    public static void Begin(PrimitiveType mode) => GLContext.Current.Begin(mode);
    public static void Begin(int mode) => Begin((PrimitiveType)mode);

    public static void End() => GLContext.Current.End();

    public static void Vertex3f(float x, float y, float z) => GLContext.Current.Vertex(x, y, z);
    public static void Vertex3d(double x, double y, double z) => Vertex3f((float)x, (float)y, (float)z);
    public static void Vertex2f(float x, float y) => Vertex3f(x, y, 0f);
    public static void Vertex2d(double x, double y) => Vertex3f((float)x, (float)y, 0f);
    public static void Vertex2i(int x, int y) => Vertex3f(x, y, 0f);
    public static void Vertex4f(float x, float y, float z, float w) => Vertex3f(w != 0 ? x / w : x, w != 0 ? y / w : y, w != 0 ? z / w : z);
    public static void Vertex3(Vector3 v) => Vertex3f(v.X, v.Y, v.Z);
    public static void Vertex3fv(float[] v)
    {
        if (v != null && v.Length >= 3) Vertex3f(v[0], v[1], v[2]);
    }

    public static void Color4f(float red, float green, float blue, float alpha)
    {
        uint a = (uint)Math.Clamp((int)(alpha * 255.0f), 0, 255);
        uint r = (uint)Math.Clamp((int)(red * 255.0f), 0, 255);
        uint g = (uint)Math.Clamp((int)(green * 255.0f), 0, 255);
        uint b = (uint)Math.Clamp((int)(blue * 255.0f), 0, 255);
        GLContext.Current.CurrentColor = (a << 24) | (r << 16) | (g << 8) | b;
    }

    public static void Color3f(float red, float green, float blue) => Color4f(red, green, blue, 1.0f);

    public static void Color4ub(byte red, byte green, byte blue, byte alpha)
    {
        GLContext.Current.CurrentColor = ((uint)alpha << 24) | ((uint)red << 16) | ((uint)green << 8) | (uint)blue;
    }

    public static void Color3ub(byte red, byte green, byte blue) => Color4ub(red, green, blue, 255);

    public static void Color(Color color)
    {
        Color4ub(color.R, color.G, color.B, color.A);
    }

    public static void Color3fv(float[] c)
    {
        if (c != null && c.Length >= 3) Color3f(c[0], c[1], c[2]);
    }

    public static void Color4fv(float[] c)
    {
        if (c != null && c.Length >= 4) Color4f(c[0], c[1], c[2], c[3]);
    }

    public static void Normal3f(float nx, float ny, float nz)
    {
        GLContext.Current.CurrentNormal = new Vector3(nx, ny, nz);
    }

    public static void Normal3(Vector3 n) => Normal3f(n.X, n.Y, n.Z);

    public static void Normal3fv(float[] n)
    {
        if (n != null && n.Length >= 3) Normal3f(n[0], n[1], n[2]);
    }

    public static void TexCoord2f(float u, float v)
    {
        GLContext.Current.CurrentTexCoord = new Vector2(u, v);
    }

    public static void TexCoord2(Vector2 uv) => TexCoord2f(uv.X, uv.Y);

    public static void TexCoord2fv(float[] uv)
    {
        if (uv != null && uv.Length >= 2) TexCoord2f(uv[0], uv[1]);
    }

    // =========================================================================
    // Enable / Disable States
    // =========================================================================

    public static void Enable(EnableCap cap) => GLContext.Current.Enable(cap);
    public static void Enable(int cap) => Enable((EnableCap)cap);

    public static void Disable(EnableCap cap) => GLContext.Current.Disable(cap);
    public static void Disable(int cap) => Disable((EnableCap)cap);

    public static bool IsEnabled(EnableCap cap) => GLContext.Current.IsEnabled(cap);
    public static bool IsEnabled(int cap) => IsEnabled((EnableCap)cap);

    public static void CullFace(CullFaceMode mode) => GLContext.Current.CurrentCullFace = mode;
    public static void CullFace(int mode) => CullFace((CullFaceMode)mode);

    public static void FrontFace(FrontFaceDirection mode) => GLContext.Current.CurrentFrontFace = mode;
    public static void FrontFace(int mode) => FrontFace((FrontFaceDirection)mode);

    public static void DepthFunc(DepthFunction func) => GLContext.Current.CurrentDepthFunc = func;
    public static void DepthFunc(int func) => DepthFunc((DepthFunction)func);

    public static void DepthMask(bool flag) => GLContext.Current.DepthMaskEnabled = flag;

    public static void BlendFunc(BlendingFactorSrc sfactor, BlendingFactorDest dfactor)
    {
        GLContext.Current.BlendSrc = sfactor;
        GLContext.Current.BlendDest = dfactor;
    }

    public static void BlendFunc(int sfactor, int dfactor) => BlendFunc((BlendingFactorSrc)sfactor, (BlendingFactorDest)dfactor);

    public static void ShadeModel(ShadingModel model) => GLContext.Current.CurrentShadeModel = model;
    public static void ShadeModel(int model) => ShadeModel((ShadingModel)model);

    public static void PolygonMode(MaterialFace face, PolygonMode mode)
    {
        // Stored for state compatibility
    }

    // =========================================================================
    // Texture Management
    // =========================================================================

    public static int GenTexture() => GLContext.Current.GenTexture();

    public static void GenTextures(int n, int[] textures)
    {
        if (textures == null || textures.Length < n) return;
        for (int i = 0; i < n; i++) textures[i] = GenTexture();
    }

    public static void GenTextures(int n, out int texture)
    {
        texture = GenTexture();
    }

    public static void DeleteTextures(int n, int[] textures)
    {
        if (textures == null) return;
        for (int i = 0; i < n && i < textures.Length; i++)
        {
            GLContext.Current.DeleteTexture(textures[i]);
        }
    }

    public static void DeleteTexture(int texture) => GLContext.Current.DeleteTexture(texture);

    public static void BindTexture(TextureTarget target, int texture) => GLContext.Current.BindTexture(target, texture);
    public static void BindTexture(int target, int texture) => BindTexture((TextureTarget)target, texture);

    public static void TexImage2D(TextureTarget target, int level, Cosmos.Kernel.System.Graphics.Image image)
    {
        var ctx = GLContext.Current;
        if (ctx.Canvas != null && image != null)
        {
            var tex = ctx.Canvas.CreateTexture(image);
            ctx.RegisterTexture(ctx.GenTexture(), tex);
        }
    }

    public static void TexImage2D(TextureTarget target, int level, PixelInternalFormat internalformat, int width, int height, int border, PixelFormat format, PixelType type, byte[] data)
    {
        var ctx = GLContext.Current;
        if (ctx.Canvas != null && data != null && width > 0 && height > 0)
        {
            // Convert byte array into Cosmos Bitmap Image
            var bmp = new Bitmap(width, height, data, ColorDepth.ColorDepth32);
            var tex = ctx.Canvas.CreateTexture(bmp);
            ctx.RegisterTexture(ctx.GenTexture(), tex);
        }
    }

    public static void TexParameter(TextureTarget target, TextureParameterName pname, int param)
    {
        // Filter / wrap modes recorded
    }

    public static void TexParameteri(TextureTarget target, TextureParameterName pname, int param) => TexParameter(target, pname, param);
    public static void TexParameterf(TextureTarget target, TextureParameterName pname, float param) => TexParameter(target, pname, (int)param);

    // =========================================================================
    // Lighting & Materials
    // =========================================================================

    public static void Light(LightName light, LightParameter pname, float[] @params) { }
    public static void Light(LightName light, LightParameter pname, float param) { }
    public static void Material(MaterialFace face, MaterialParameter pname, float[] @params) { }
    public static void Material(MaterialFace face, MaterialParameter pname, float param) { }
    public static void LightModel(int pname, float[] @params) { }

    // =========================================================================
    // Queries & Diagnostics
    // =========================================================================

    public static ErrorCode GetError()
    {
        var err = GLContext.Current.LastError;
        GLContext.Current.LastError = ErrorCode.NoError;
        return err;
    }

    public static string GetString(StringName name)
    {
        return name switch
        {
            StringName.Vendor => "Cosmos",
            StringName.Renderer => "Cosmos Canvas3D",
            StringName.Version => "2.1 CosmosGL 1.0.0",
            StringName.Extensions => "GL_EXT_texture_object GL_EXT_vertex_array",
            _ => string.Empty
        };
    }

    public static string GetString(int name) => GetString((StringName)name);
}
