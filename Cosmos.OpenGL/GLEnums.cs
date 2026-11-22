using System;

namespace Cosmos.OpenGL;

public enum PrimitiveType
{
    Points = 0x0000,
    Lines = 0x0001,
    LineLoop = 0x0002,
    LineStrip = 0x0003,
    Triangles = 0x0004,
    TriangleStrip = 0x0005,
    TriangleFan = 0x0006,
    Quads = 0x0007,
    QuadStrip = 0x0008,
    Polygon = 0x0009
}

[Flags]
public enum ClearBufferMask
{
    None = 0,
    DepthBufferBit = 0x00000100,
    AccumBufferBit = 0x00000200,
    StencilBufferBit = 0x00000400,
    ColorBufferBit = 0x00004000
}

public enum MatrixMode
{
    Modelview = 0x1700,
    Projection = 0x1701,
    Texture = 0x1702,
    Color = 0x1800
}

public enum EnableCap
{
    PointSmooth = 0x0B10,
    LineSmooth = 0x0B20,
    LineStipple = 0x0B24,
    PolygonSmooth = 0x0B41,
    PolygonStipple = 0x0B42,
    CullFace = 0x0B44,
    Lighting = 0x0B50,
    ColorMaterial = 0x0B57,
    Fog = 0x0B60,
    DepthTest = 0x0B71,
    StencilTest = 0x0B90,
    Normalize = 0x0BA1,
    AlphaTest = 0x0BC0,
    Dither = 0x0BD0,
    Blend = 0x0BE2,
    IndexLogicOp = 0x0BF0,
    ColorLogicOp = 0x0BF1,
    ScissorTest = 0x0C11,
    Texture1D = 0x0DE0,
    Texture2D = 0x0DE1,
    Texture3D = 0x806F,
    Light0 = 0x4000,
    Light1 = 0x4001,
    Light2 = 0x4002,
    Light3 = 0x4003,
    Light4 = 0x4004,
    Light5 = 0x4005,
    Light6 = 0x4006,
    Light7 = 0x4007
}

public enum DepthFunction
{
    Never = 0x0200,
    Less = 0x0201,
    Equal = 0x0202,
    Lequal = 0x0203,
    Greater = 0x0204,
    Notequal = 0x0205,
    Gequal = 0x0206,
    Always = 0x0207
}

public enum CullFaceMode
{
    Front = 0x0404,
    Back = 0x0405,
    FrontAndBack = 0x0408
}

public enum FrontFaceDirection
{
    Cw = 0x0900,
    Ccw = 0x0901
}

public enum BlendingFactorSrc
{
    Zero = 0,
    One = 1,
    DstColor = 0x0306,
    OneMinusDstColor = 0x0307,
    SrcAlpha = 0x0302,
    OneMinusSrcAlpha = 0x0303,
    DstAlpha = 0x0304,
    OneMinusDstAlpha = 0x0305,
    SrcAlphaSaturate = 0x0308
}

public enum BlendingFactorDest
{
    Zero = 0,
    One = 1,
    SrcColor = 0x0300,
    OneMinusSrcColor = 0x0301,
    SrcAlpha = 0x0302,
    OneMinusSrcAlpha = 0x0303,
    DstAlpha = 0x0304,
    OneMinusDstAlpha = 0x0305
}

public enum ShadingModel
{
    Flat = 0x1D00,
    Smooth = 0x1D01
}

public enum PolygonMode
{
    Point = 0x1B00,
    Line = 0x1B01,
    Fill = 0x1B02
}

public enum MaterialFace
{
    Front = 0x0404,
    Back = 0x0405,
    FrontAndBack = 0x0408
}

public enum TextureTarget
{
    Texture1D = 0x0DE0,
    Texture2D = 0x0DE1,
    Texture3D = 0x806F
}

public enum TextureParameterName
{
    TextureMagFilter = 0x2800,
    TextureMinFilter = 0x2801,
    TextureWrapS = 0x2802,
    TextureWrapT = 0x2803
}

public enum TextureMinFilter
{
    Nearest = 0x2600,
    Linear = 0x2601,
    NearestMipmapNearest = 0x2700,
    LinearMipmapNearest = 0x2701,
    NearestMipmapLinear = 0x2702,
    LinearMipmapLinear = 0x2703
}

public enum TextureMagFilter
{
    Nearest = 0x2600,
    Linear = 0x2601
}

public enum TextureWrapMode
{
    Clamp = 0x2900,
    Repeat = 0x2901,
    ClampToEdge = 0x812F
}

public enum PixelFormat
{
    ColorIndex = 0x1900,
    Rgb = 0x1907,
    Rgba = 0x1908,
    Bgr = 0x80E0,
    Bgra = 0x80E1,
    Luminance = 0x1909,
    LuminanceAlpha = 0x190A
}

public enum PixelInternalFormat
{
    Rgb = 0x1907,
    Rgba = 0x1908,
    Rgb8 = 0x8051,
    Rgba8 = 0x8058
}

public enum PixelType
{
    Byte = 0x1400,
    UnsignedByte = 0x1401,
    Short = 0x1402,
    UnsignedShort = 0x1403,
    Int = 0x1404,
    UnsignedInt = 0x1405,
    Float = 0x1406
}

public enum LightName
{
    Light0 = 0x4000,
    Light1 = 0x4001,
    Light2 = 0x4002,
    Light3 = 0x4003,
    Light4 = 0x4004,
    Light5 = 0x4005,
    Light6 = 0x4006,
    Light7 = 0x4007
}

public enum LightParameter
{
    Ambient = 0x1200,
    Diffuse = 0x1201,
    Specular = 0x1202,
    Position = 0x1203,
    SpotDirection = 0x1204,
    SpotExponent = 0x1205,
    SpotCutoff = 0x1206,
    ConstantAttenuation = 0x1207,
    LinearAttenuation = 0x1208,
    QuadraticAttenuation = 0x1209
}

public enum MaterialParameter
{
    Emission = 0x1600,
    Shininess = 0x1601,
    AmbientAndDiffuse = 0x1602,
    ColorIndexes = 0x1603,
    Ambient = 0x1200,
    Diffuse = 0x1201,
    Specular = 0x1202
}

public enum ErrorCode
{
    NoError = 0,
    InvalidEnum = 0x0500,
    InvalidValue = 0x0501,
    InvalidOperation = 0x0502,
    StackOverflow = 0x0503,
    StackUnderflow = 0x0504,
    OutOfMemory = 0x0505
}

public enum StringName
{
    Vendor = 0x1F00,
    Renderer = 0x1F01,
    Version = 0x1F02,
    Extensions = 0x1F03
}
