using System;
using System.Numerics;

namespace Cosmos.OpenGL;

/// <summary>
/// OpenGL Utility Library (GLU) implementation for Cosmos.
/// </summary>
public static class GLU
{
    /// <summary>
    /// Sets up a perspective projection matrix.
    /// </summary>
    public static void gluPerspective(double fovy, double aspect, double zNear, double zFar)
    {
        GL.Perspective(fovy, aspect, zNear, zFar);
    }

    /// <summary>
    /// Defines a viewing transformation (camera).
    /// </summary>
    public static void gluLookAt(
        double eyeX, double eyeY, double eyeZ,
        double centerX, double centerY, double centerZ,
        double upX, double upY, double upZ)
    {
        Vector3 eye = new((float)eyeX, (float)eyeY, (float)eyeZ);
        Vector3 target = new((float)centerX, (float)centerY, (float)centerZ);
        Vector3 up = new((float)upX, (float)upY, (float)upZ);

        // Update Canvas3D hardware camera (which sets the VIEW transform in SVGA3D)
        GL.SetCamera(eye, target, up);
    }

    /// <summary>
    /// Sets up a 2D orthographic projection matrix.
    /// </summary>
    public static void gluOrtho2D(double left, double right, double bottom, double top)
    {
        GL.Ortho(left, right, bottom, top, -1.0, 1.0);
    }

    /// <summary>
    /// Returns a string describing the specified OpenGL error code.
    /// </summary>
    public static string gluErrorString(ErrorCode errorCode)
    {
        return errorCode switch
        {
            ErrorCode.NoError => "no error",
            ErrorCode.InvalidEnum => "invalid enumerant",
            ErrorCode.InvalidValue => "invalid value",
            ErrorCode.InvalidOperation => "invalid operation",
            ErrorCode.StackOverflow => "stack overflow",
            ErrorCode.StackUnderflow => "stack underflow",
            ErrorCode.OutOfMemory => "out of memory",
            _ => "unknown error"
        };
    }

    public static string gluErrorString(int errorCode) => gluErrorString((ErrorCode)errorCode);
}
