#if HWINPUT

#endif

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using NthDimension.Algebra;
//using NthDimension.Algebra.Geometry;
//using NthDimension.Algebra.Raytracing;
//using SYSCON.Common;
//using SYSCON.Graphics.Buffers;
//using SYSCON.Graphics.Cameras;
//using SYSCON.Graphics.Drawables;
//using SYSCON.Graphics.Geometry;
//using SYSCON.Graphics.Input;
//using SYSCON.Graphics.RenderCommands;
//using SYSCON.Graphics.Scenegraph;

namespace NthDimension.Rasterizer.GL1
{
    //[DataContract(Name = "enuPolygonWinding", Namespace = "SYSCON.Graphics.Geometry"), Serializable]
    public enum enuPolygonWinding
    {
        //[EnumMember(Value = "Clockwise")]
        Clockwise,
        //[EnumMember(Value = "Counterclockwise")]
        Counterclockwise
    }

    //[DataContract(Name = "enuPolygonMode", Namespace = "SYSCON.Graphics.Geometry")]
    public enum enuPolygonMode
    {
        //[EnumMember(Value = "Points")]
        Points = 0,
        //[EnumMember(Value = "Lines")]
        Lines = 1,
        //[EnumMember(Value = "LineLoop")]
        LineLoop = 2,
        //[EnumMember(Value = "LineStrip")]
        LineStrip = 3,
        //[EnumMember(Value = "Triangles")]
        Triangles = 4,
        //[EnumMember(Value = "TriangleStrip")]
        TriangleStrip = 5,
        //[EnumMember(Value = "TriangleFan")]
        TriangleFan = 6,
        //[EnumMember(Value = "Quads")]
        Quads = 7,
        //[EnumMember(Value = "QuadStrip")]
        QuadStrip = 8,
        //[EnumMember(Value = "Polygon")]
        Polygon = 9,
        //[EnumMember(Value = "LinesAdjacencyArb")]
        LinesAdjacencyArb = 10,
        //[EnumMember(Value = "LinesAdjacency")]
        LinesAdjacency = 10,
        //[EnumMember(Value = "LinesAdjacencyExt")]
        LinesAdjacencyExt = 10,
        //[EnumMember(Value = "LineStripAdjacencyArb")]
        LineStripAdjacencyArb = 11,
        //[EnumMember(Value = "LineStripAdjacencyExt")]
        LineStripAdjacencyExt = 11,
        //[EnumMember(Value = "LineStripAdjacency")]
        LineStripAdjacency = 11,
        //[EnumMember(Value = "TrianglesAdjacencyArb")]
        TrianglesAdjacencyArb = 12,
        //[EnumMember(Value = "TrianglesAdjacencyExt")]
        TrianglesAdjacencyExt = 12,
        //[EnumMember(Value = "TrianglesAdjacency")]
        TrianglesAdjacency = 12,
        //[EnumMember(Value = "TriangleStripAdjacencyExt")]
        TriangleStripAdjacencyExt = 13,
        //[EnumMember(Value = "TriangleStripAdjacencyExt")]
        TriangleStripAdjacency = 13,
        //[EnumMember(Value = "TriangleStripAdjacencyArb")]
        TriangleStripAdjacencyArb = 13,
        //[EnumMember(Value = "Patches")]
        Patches = 14,
    }

    public enum PolygonSplitResult
    {
        CompletelyInside,       // Polygon is completely inside half-space defined by plane
        CompletelyOutside,      // Polygon is completely outside half-space defined by plane

        Split,                  // Polygon has been split into two parts by plane

        PlaneAligned,           // Polygon is aligned with cutting plane and the polygons' normal points in the same direction
        PlaneOppositeAligned    // Polygon is aligned with cutting plane and the polygons' normal points in the opposite direction
    }


    #region Renderer enumerators
    // TODO:: Default to VAO
    //public enum enuGraphicsMode
    //{
    //    Default,                    // TODO:: Default to VAO                 
    //    Direct,
    //    Lists,                      // OpenGL 1.0
    //    VAO,                        // OpenGL 4
    //    VBO,                        // OpenGL 1.1
    //}

    #region Renderer (GL) enumerators
    [Flags]
    public enum BlendingFactorSrc
    {
        // Summary:
        //     Original was GL_ZERO = 0
        Zero = 0,
        //
        // Summary:
        //     Original was GL_ONE = 1
        One = 1,
        //
        // Summary:
        //     Original was GL_SRC_COLOR = 0x0300
        SrcColor = 768,
        //
        // Summary:
        //     Original was GL_ONE_MINUS_SRC_COLOR = 0x0301
        OneMinusSrcColor = 769,
        //
        // Summary:
        //     Original was GL_SRC_ALPHA = 0x0302
        SrcAlpha = 770,
        //
        // Summary:
        //     Original was GL_ONE_MINUS_SRC_ALPHA = 0x0303
        OneMinusSrcAlpha = 771,
        //
        // Summary:
        //     Original was GL_DST_ALPHA = 0x0304
        DstAlpha = 772,
        //
        // Summary:
        //     Original was GL_ONE_MINUS_DST_ALPHA = 0x0305
        OneMinusDstAlpha = 773,
        //
        // Summary:
        //     Original was GL_DST_COLOR = 0x0306
        DstColor = 774,
        //
        // Summary:
        //     Original was GL_ONE_MINUS_DST_COLOR = 0x0307
        OneMinusDstColor = 775,
        //
        // Summary:
        //     Original was GL_SRC_ALPHA_SATURATE = 0x0308
        SrcAlphaSaturate = 776,
        //
        // Summary:
        //     Original was GL_CONSTANT_COLOR_EXT = 0x8001
        ConstantColorExt = 32769,
        //
        // Summary:
        //     Original was GL_CONSTANT_COLOR = 0x8001
        ConstantColor = 32769,
        //
        // Summary:
        //     Original was GL_ONE_MINUS_CONSTANT_COLOR = 0x8002
        OneMinusConstantColor = 32770,
        //
        // Summary:
        //     Original was GL_ONE_MINUS_CONSTANT_COLOR_EXT = 0x8002
        OneMinusConstantColorExt = 32770,
        //
        // Summary:
        //     Original was GL_CONSTANT_ALPHA = 0x8003
        ConstantAlpha = 32771,
        //
        // Summary:
        //     Original was GL_CONSTANT_ALPHA_EXT = 0x8003
        ConstantAlphaExt = 32771,
        //
        // Summary:
        //     Original was GL_ONE_MINUS_CONSTANT_ALPHA_EXT = 0x8004
        OneMinusConstantAlphaExt = 32772,
        //
        // Summary:
        //     Original was GL_ONE_MINUS_CONSTANT_ALPHA = 0x8004
        OneMinusConstantAlpha = 32772,
        //
        // Summary:
        //     Original was GL_SRC1_ALPHA = 0x8589
        Src1Alpha = 34185,
        //
        // Summary:
        //     Original was GL_SRC1_COLOR = 0x88F9
        Src1Color = 35065,
        //
        // Summary:
        //     Original was GL_ONE_MINUS_SRC1_COLOR = 0x88FA
        OneMinusSrc1Color = 35066,
        //
        // Summary:
        //     Original was GL_ONE_MINUS_SRC1_ALPHA = 0x88FB
        OneMinusSrc1Alpha = 35067,
    }
    public enum BlendingFactorDest
    {
        // Summary:
        //     Original was GL_ZERO = 0
        Zero = 0,
        //
        // Summary:
        //     Original was GL_ONE = 1
        One = 1,
        //
        // Summary:
        //     Original was GL_SRC_COLOR = 0x0300
        SrcColor = 768,
        //
        // Summary:
        //     Original was GL_ONE_MINUS_SRC_COLOR = 0x0301
        OneMinusSrcColor = 769,
        //
        // Summary:
        //     Original was GL_SRC_ALPHA = 0x0302
        SrcAlpha = 770,
        //
        // Summary:
        //     Original was GL_ONE_MINUS_SRC_ALPHA = 0x0303
        OneMinusSrcAlpha = 771,
        //
        // Summary:
        //     Original was GL_DST_ALPHA = 0x0304
        DstAlpha = 772,
        //
        // Summary:
        //     Original was GL_ONE_MINUS_DST_ALPHA = 0x0305
        OneMinusDstAlpha = 773,
        //
        // Summary:
        //     Original was GL_DST_COLOR = 0x0306
        DstColor = 774,
        //
        // Summary:
        //     Original was GL_ONE_MINUS_DST_COLOR = 0x0307
        OneMinusDstColor = 775,
        //
        // Summary:
        //     Original was GL_SRC_ALPHA_SATURATE = 0x0308
        SrcAlphaSaturate = 776,
        //
        // Summary:
        //     Original was GL_CONSTANT_COLOR_EXT = 0x8001
        ConstantColorExt = 32769,
        //
        // Summary:
        //     Original was GL_CONSTANT_COLOR = 0x8001
        ConstantColor = 32769,
        //
        // Summary:
        //     Original was GL_ONE_MINUS_CONSTANT_COLOR = 0x8002
        OneMinusConstantColor = 32770,
        //
        // Summary:
        //     Original was GL_ONE_MINUS_CONSTANT_COLOR_EXT = 0x8002
        OneMinusConstantColorExt = 32770,
        //
        // Summary:
        //     Original was GL_CONSTANT_ALPHA = 0x8003
        ConstantAlpha = 32771,
        //
        // Summary:
        //     Original was GL_CONSTANT_ALPHA_EXT = 0x8003
        ConstantAlphaExt = 32771,
        //
        // Summary:
        //     Original was GL_ONE_MINUS_CONSTANT_ALPHA_EXT = 0x8004
        OneMinusConstantAlphaExt = 32772,
        //
        // Summary:
        //     Original was GL_ONE_MINUS_CONSTANT_ALPHA = 0x8004
        OneMinusConstantAlpha = 32772,
        //
        // Summary:
        //     Original was GL_SRC1_ALPHA = 0x8589
        Src1Alpha = 34185,
        //
        // Summary:
        //     Original was GL_SRC1_COLOR = 0x88F9
        Src1Color = 35065,
        //
        // Summary:
        //     Original was GL_ONE_MINUS_SRC1_COLOR = 0x88FA
        OneMinusSrc1Color = 35066,
        //
        // Summary:
        //     Original was GL_ONE_MINUS_SRC1_ALPHA = 0x88FB
        OneMinusSrc1Alpha = 35067,
    }
    [Flags]
    public enum ClearBufferMask
    {
        None = 0,
        DepthBufferBit = 256,
        AccumBufferBit = 512,
        StencilBufferBit = 1024,
        ColorBufferBit = 16384,
        CoverageBufferBitNv = 32768,
    }
    [Flags] /* Added 20-July-15 */ 
    public enum enuCullFaceMode
    {
        None            = -1,       // Added Fed-22-2017
        Front           = 1028,
        Back            = 1029,
        FrontAndBack    = 1032,
    }
    [Flags] /* Added 02-July-15 */
    public enum DepthFunction
    {
        Never = 512,
        Less = 513,
        Equal = 514,
        Lequal = 515,
        Greater = 516,
        Notequal = 517,
        Gequal = 518,
        Always = 519,
    }
    [Flags] /* Added 02-July-15 */
    public enum HintMode
    {
        DontCare = 4352,
        Fastest = 4353,
        Nicest = 4354,
    }
    [Flags] /* Added 02-July-15 */
    public enum HintTarget
    {
        PerspectiveCorrectionHint = 3152,
        PointSmoothHint = 3153,
        LineSmoothHint = 3154,
        PolygonSmoothHint = 3155,
        FogHint = 3156,
        PackCmykHintExt = 32782,
        UnpackCmykHintExt = 32783,
        PhongHintWin = 33003,
        ClipVolumeClippingHintExt = 33008,
        TextureMultiBufferHintSgix = 33070,
        GenerateMipmapHintSgis = 33170,
        GenerateMipmapHint = 33170,
        ProgramBinaryRetrievableHint = 33367,
        ConvolutionHintSgix = 33558,
        ScalebiasHintSgix = 33570,
        LineQualityHintSgix = 33627,
        VertexPreclipSgix = 33774,
        VertexPreclipHintSgix = 33775,
        TextureCompressionHintArb = 34031,
        TextureCompressionHint = 34031,
        VertexArrayStorageHintApple = 34079,
        MultisampleFilterHintNv = 34100,
        TransformHintApple = 34225,
        TextureStorageHintApple = 34236,
        FragmentShaderDerivativeHint = 35723,
        FragmentShaderDerivativeHintOes = 35723,
        FragmentShaderDerivativeHintArb = 35723,
        BinningControlHintQcom = 36784,
        PreferDoublebufferHintPgi = 107000,
        ConserveMemoryHintPgi = 107005,
        ReclaimMemoryHintPgi = 107006,
        NativeGraphicsBeginHintPgi = 107011,
        NativeGraphicsEndHintPgi = 107012,
        AlwaysFastHintPgi = 107020,
        AlwaysSoftHintPgi = 107021,
        AllowDrawObjHintPgi = 107022,
        AllowDrawWinHintPgi = 107023,
        AllowDrawFrgHintPgi = 107024,
        AllowDrawMemHintPgi = 107025,
        StrictDepthfuncHintPgi = 107030,
        StrictLightingHintPgi = 107031,
        StrictScissorHintPgi = 107032,
        FullStippleHintPgi = 107033,
        ClipNearHintPgi = 107040,
        ClipFarHintPgi = 107041,
        WideLineHintPgi = 107042,
        BackNormalsHintPgi = 107043,
        VertexDataHintPgi = 107050,
        VertexConsistentHintPgi = 107051,
        MaterialSideHintPgi = 107052,
        MaxVertexHintPgi = 107053,
    }
    [Flags]
    public enum GetPName
    {
        // Summary:
        //     Original was GL_CURRENT_COLOR = 0x0B00
        CurrentColor = 2816,
        //
        // Summary:
        //     Original was GL_CURRENT_INDEX = 0x0B01
        CurrentIndex = 2817,
        //
        // Summary:
        //     Original was GL_CURRENT_NORMAL = 0x0B02
        CurrentNormal = 2818,
        //
        // Summary:
        //     Original was GL_CURRENT_TEXTURE_COORDS = 0x0B03
        CurrentTextureCoords = 2819,
        //
        // Summary:
        //     Original was GL_CURRENT_RASTER_COLOR = 0x0B04
        CurrentRasterColor = 2820,
        //
        // Summary:
        //     Original was GL_CURRENT_RASTER_INDEX = 0x0B05
        CurrentRasterIndex = 2821,
        //
        // Summary:
        //     Original was GL_CURRENT_RASTER_TEXTURE_COORDS = 0x0B06
        CurrentRasterTextureCoords = 2822,
        //
        // Summary:
        //     Original was GL_CURRENT_RASTER_POSITION = 0x0B07
        CurrentRasterPosition = 2823,
        //
        // Summary:
        //     Original was GL_CURRENT_RASTER_POSITION_VALID = 0x0B08
        CurrentRasterPositionValid = 2824,
        //
        // Summary:
        //     Original was GL_CURRENT_RASTER_DISTANCE = 0x0B09
        CurrentRasterDistance = 2825,
        //
        // Summary:
        //     Original was GL_POINT_SMOOTH = 0x0B10
        PointSmooth = 2832,
        //
        // Summary:
        //     Original was GL_POINT_SIZE = 0x0B11
        PointSize = 2833,
        //
        // Summary:
        //     Original was GL_SMOOTH_POINT_SIZE_RANGE = 0x0B12
        SmoothPointSizeRange = 2834,
        //
        // Summary:
        //     Original was GL_POINT_SIZE_RANGE = 0x0B12
        PointSizeRange = 2834,
        //
        // Summary:
        //     Original was GL_POINT_SIZE_GRANULARITY = 0x0B13
        PointSizeGranularity = 2835,
        //
        // Summary:
        //     Original was GL_SMOOTH_POINT_SIZE_GRANULARITY = 0x0B13
        SmoothPointSizeGranularity = 2835,
        //
        // Summary:
        //     Original was GL_LINE_SMOOTH = 0x0B20
        LineSmooth = 2848,
        //
        // Summary:
        //     Original was GL_LINE_WIDTH = 0x0B21
        LineWidth = 2849,
        //
        // Summary:
        //     Original was GL_LINE_WIDTH_RANGE = 0x0B22
        LineWidthRange = 2850,
        //
        // Summary:
        //     Original was GL_SMOOTH_LINE_WIDTH_RANGE = 0x0B22
        SmoothLineWidthRange = 2850,
        //
        // Summary:
        //     Original was GL_SMOOTH_LINE_WIDTH_GRANULARITY = 0x0B23
        SmoothLineWidthGranularity = 2851,
        //
        // Summary:
        //     Original was GL_LINE_WIDTH_GRANULARITY = 0x0B23
        LineWidthGranularity = 2851,
        //
        // Summary:
        //     Original was GL_LINE_STIPPLE = 0x0B24
        LineStipple = 2852,
        //
        // Summary:
        //     Original was GL_LINE_STIPPLE_PATTERN = 0x0B25
        LineStipplePattern = 2853,
        //
        // Summary:
        //     Original was GL_LINE_STIPPLE_REPEAT = 0x0B26
        LineStippleRepeat = 2854,
        //
        // Summary:
        //     Original was GL_LIST_MODE = 0x0B30
        ListMode = 2864,
        //
        // Summary:
        //     Original was GL_MAX_LIST_NESTING = 0x0B31
        MaxListNesting = 2865,
        //
        // Summary:
        //     Original was GL_LIST_BASE = 0x0B32
        ListBase = 2866,
        //
        // Summary:
        //     Original was GL_LIST_INDEX = 0x0B33
        ListIndex = 2867,
        //
        // Summary:
        //     Original was GL_POLYGON_MODE = 0x0B40
        PolygonMode = 2880,
        //
        // Summary:
        //     Original was GL_POLYGON_SMOOTH = 0x0B41
        PolygonSmooth = 2881,
        //
        // Summary:
        //     Original was GL_POLYGON_STIPPLE = 0x0B42
        PolygonStipple = 2882,
        //
        // Summary:
        //     Original was GL_EDGE_FLAG = 0x0B43
        EdgeFlag = 2883,
        //
        // Summary:
        //     Original was GL_CULL_FACE = 0x0B44
        CullFace = 2884,
        //
        // Summary:
        //     Original was GL_CULL_FACE_MODE = 0x0B45
        CullFaceMode = 2885,
        //
        // Summary:
        //     Original was GL_FRONT_FACE = 0x0B46
        FrontFace = 2886,
        //
        // Summary:
        //     Original was GL_LIGHTING = 0x0B50
        Lighting = 2896,
        //
        // Summary:
        //     Original was GL_LIGHT_MODEL_LOCAL_VIEWER = 0x0B51
        LightModelLocalViewer = 2897,
        //
        // Summary:
        //     Original was GL_LIGHT_MODEL_TWO_SIDE = 0x0B52
        LightModelTwoSide = 2898,
        //
        // Summary:
        //     Original was GL_LIGHT_MODEL_AMBIENT = 0x0B53
        LightModelAmbient = 2899,
        //
        // Summary:
        //     Original was GL_SHADE_MODEL = 0x0B54
        ShadeModel = 2900,
        //
        // Summary:
        //     Original was GL_COLOR_MATERIAL_FACE = 0x0B55
        ColorMaterialFace = 2901,
        //
        // Summary:
        //     Original was GL_COLOR_MATERIAL_PARAMETER = 0x0B56
        ColorMaterialParameter = 2902,
        //
        // Summary:
        //     Original was GL_COLOR_MATERIAL = 0x0B57
        ColorMaterial = 2903,
        //
        // Summary:
        //     Original was GL_FOG = 0x0B60
        Fog = 2912,
        //
        // Summary:
        //     Original was GL_FOG_INDEX = 0x0B61
        FogIndex = 2913,
        //
        // Summary:
        //     Original was GL_FOG_DENSITY = 0x0B62
        FogDensity = 2914,
        //
        // Summary:
        //     Original was GL_FOG_START = 0x0B63
        FogStart = 2915,
        //
        // Summary:
        //     Original was GL_FOG_END = 0x0B64
        FogEnd = 2916,
        //
        // Summary:
        //     Original was GL_FOG_MODE = 0x0B65
        FogMode = 2917,
        //
        // Summary:
        //     Original was GL_FOG_COLOR = 0x0B66
        FogColor = 2918,
        //
        // Summary:
        //     Original was GL_DEPTH_RANGE = 0x0B70
        DepthRange = 2928,
        //
        // Summary:
        //     Original was GL_DEPTH_TEST = 0x0B71
        DepthTest = 2929,
        //
        // Summary:
        //     Original was GL_DEPTH_WRITEMASK = 0x0B72
        DepthWritemask = 2930,
        //
        // Summary:
        //     Original was GL_DEPTH_CLEAR_VALUE = 0x0B73
        DepthClearValue = 2931,
        //
        // Summary:
        //     Original was GL_DEPTH_FUNC = 0x0B74
        DepthFunc = 2932,
        //
        // Summary:
        //     Original was GL_ACCUM_CLEAR_VALUE = 0x0B80
        AccumClearValue = 2944,
        //
        // Summary:
        //     Original was GL_STENCIL_TEST = 0x0B90
        StencilTest = 2960,
        //
        // Summary:
        //     Original was GL_STENCIL_CLEAR_VALUE = 0x0B91
        StencilClearValue = 2961,
        //
        // Summary:
        //     Original was GL_STENCIL_FUNC = 0x0B92
        StencilFunc = 2962,
        //
        // Summary:
        //     Original was GL_STENCIL_VALUE_MASK = 0x0B93
        StencilValueMask = 2963,
        //
        // Summary:
        //     Original was GL_STENCIL_FAIL = 0x0B94
        StencilFail = 2964,
        //
        // Summary:
        //     Original was GL_STENCIL_PASS_DEPTH_FAIL = 0x0B95
        StencilPassDepthFail = 2965,
        //
        // Summary:
        //     Original was GL_STENCIL_PASS_DEPTH_PASS = 0x0B96
        StencilPassDepthPass = 2966,
        //
        // Summary:
        //     Original was GL_STENCIL_REF = 0x0B97
        StencilRef = 2967,
        //
        // Summary:
        //     Original was GL_STENCIL_WRITEMASK = 0x0B98
        StencilWritemask = 2968,
        //
        // Summary:
        //     Original was GL_MATRIX_MODE = 0x0BA0
        MatrixMode = 2976,
        //
        // Summary:
        //     Original was GL_NORMALIZE = 0x0BA1
        Normalize = 2977,
        //
        // Summary:
        //     Original was GL_VIEWPORT = 0x0BA2
        Viewport = 2978,
        //
        // Summary:
        //     Original was GL_MODELVIEW0_STACK_DEPTH_EXT = 0x0BA3
        Modelview0StackDepthExt = 2979,
        //
        // Summary:
        //     Original was GL_MODELVIEW_STACK_DEPTH = 0x0BA3
        ModelviewStackDepth = 2979,
        //
        // Summary:
        //     Original was GL_PROJECTION_STACK_DEPTH = 0x0BA4
        ProjectionStackDepth = 2980,
        //
        // Summary:
        //     Original was GL_TEXTURE_STACK_DEPTH = 0x0BA5
        TextureStackDepth = 2981,
        //
        // Summary:
        //     Original was GL_MODELVIEW0_MATRIX_EXT = 0x0BA6
        Modelview0MatrixExt = 2982,
        //
        // Summary:
        //     Original was GL_MODELVIEW_MATRIX = 0x0BA6
        ModelviewMatrix = 2982,
        //
        // Summary:
        //     Original was GL_PROJECTION_MATRIX = 0x0BA7
        ProjectionMatrix = 2983,
        //
        // Summary:
        //     Original was GL_TEXTURE_MATRIX = 0x0BA8
        TextureMatrix = 2984,
        //
        // Summary:
        //     Original was GL_ATTRIB_STACK_DEPTH = 0x0BB0
        AttribStackDepth = 2992,
        //
        // Summary:
        //     Original was GL_CLIENT_ATTRIB_STACK_DEPTH = 0x0BB1
        ClientAttribStackDepth = 2993,
        //
        // Summary:
        //     Original was GL_ALPHA_TEST = 0x0BC0
        AlphaTest = 3008,
        //
        // Summary:
        //     Original was GL_ALPHA_TEST_QCOM = 0x0BC0
        AlphaTestQcom = 3008,
        //
        // Summary:
        //     Original was GL_ALPHA_TEST_FUNC_QCOM = 0x0BC1
        AlphaTestFuncQcom = 3009,
        //
        // Summary:
        //     Original was GL_ALPHA_TEST_FUNC = 0x0BC1
        AlphaTestFunc = 3009,
        //
        // Summary:
        //     Original was GL_ALPHA_TEST_REF = 0x0BC2
        AlphaTestRef = 3010,
        //
        // Summary:
        //     Original was GL_ALPHA_TEST_REF_QCOM = 0x0BC2
        AlphaTestRefQcom = 3010,
        //
        // Summary:
        //     Original was GL_DITHER = 0x0BD0
        Dither = 3024,
        //
        // Summary:
        //     Original was GL_BLEND_DST = 0x0BE0
        BlendDst = 3040,
        //
        // Summary:
        //     Original was GL_BLEND_SRC = 0x0BE1
        BlendSrc = 3041,
        //
        // Summary:
        //     Original was GL_BLEND = 0x0BE2
        Blend = 3042,
        //
        // Summary:
        //     Original was GL_LOGIC_OP_MODE = 0x0BF0
        LogicOpMode = 3056,
        //
        // Summary:
        //     Original was GL_LOGIC_OP = 0x0BF1
        LogicOp = 3057,
        //
        // Summary:
        //     Original was GL_INDEX_LOGIC_OP = 0x0BF1
        IndexLogicOp = 3057,
        //
        // Summary:
        //     Original was GL_COLOR_LOGIC_OP = 0x0BF2
        ColorLogicOp = 3058,
        //
        // Summary:
        //     Original was GL_AUX_BUFFERS = 0x0C00
        AuxBuffers = 3072,
        //
        // Summary:
        //     Original was GL_DRAW_BUFFER = 0x0C01
        DrawBuffer = 3073,
        //
        // Summary:
        //     Original was GL_DRAW_BUFFER_EXT = 0x0C01
        DrawBufferExt = 3073,
        //
        // Summary:
        //     Original was GL_READ_BUFFER_EXT = 0x0C02
        ReadBufferExt = 3074,
        //
        // Summary:
        //     Original was GL_READ_BUFFER = 0x0C02
        ReadBuffer = 3074,
        //
        // Summary:
        //     Original was GL_READ_BUFFER_NV = 0x0C02
        ReadBufferNv = 3074,
        //
        // Summary:
        //     Original was GL_SCISSOR_BOX = 0x0C10
        ScissorBox = 3088,
        //
        // Summary:
        //     Original was GL_SCISSOR_TEST = 0x0C11
        ScissorTest = 3089,
        //
        // Summary:
        //     Original was GL_INDEX_CLEAR_VALUE = 0x0C20
        IndexClearValue = 3104,
        //
        // Summary:
        //     Original was GL_INDEX_WRITEMASK = 0x0C21
        IndexWritemask = 3105,
        //
        // Summary:
        //     Original was GL_COLOR_CLEAR_VALUE = 0x0C22
        ColorClearValue = 3106,
        //
        // Summary:
        //     Original was GL_COLOR_WRITEMASK = 0x0C23
        ColorWritemask = 3107,
        //
        // Summary:
        //     Original was GL_INDEX_MODE = 0x0C30
        IndexMode = 3120,
        //
        // Summary:
        //     Original was GL_RGBA_MODE = 0x0C31
        RgbaMode = 3121,
        //
        // Summary:
        //     Original was GL_DOUBLEBUFFER = 0x0C32
        Doublebuffer = 3122,
        //
        // Summary:
        //     Original was GL_STEREO = 0x0C33
        Stereo = 3123,
        //
        // Summary:
        //     Original was GL_RENDER_MODE = 0x0C40
        RenderMode = 3136,
        //
        // Summary:
        //     Original was GL_PERSPECTIVE_CORRECTION_HINT = 0x0C50
        PerspectiveCorrectionHint = 3152,
        //
        // Summary:
        //     Original was GL_POINT_SMOOTH_HINT = 0x0C51
        PointSmoothHint = 3153,
        //
        // Summary:
        //     Original was GL_LINE_SMOOTH_HINT = 0x0C52
        LineSmoothHint = 3154,
        //
        // Summary:
        //     Original was GL_POLYGON_SMOOTH_HINT = 0x0C53
        PolygonSmoothHint = 3155,
        //
        // Summary:
        //     Original was GL_FOG_HINT = 0x0C54
        FogHint = 3156,
        //
        // Summary:
        //     Original was GL_TEXTURE_GEN_S = 0x0C60
        TextureGenS = 3168,
        //
        // Summary:
        //     Original was GL_TEXTURE_GEN_T = 0x0C61
        TextureGenT = 3169,
        //
        // Summary:
        //     Original was GL_TEXTURE_GEN_R = 0x0C62
        TextureGenR = 3170,
        //
        // Summary:
        //     Original was GL_TEXTURE_GEN_Q = 0x0C63
        TextureGenQ = 3171,
        //
        // Summary:
        //     Original was GL_PIXEL_MAP_I_TO_I_SIZE = 0x0CB0
        PixelMapIToISize = 3248,
        //
        // Summary:
        //     Original was GL_PIXEL_MAP_S_TO_S_SIZE = 0x0CB1
        PixelMapSToSSize = 3249,
        //
        // Summary:
        //     Original was GL_PIXEL_MAP_I_TO_R_SIZE = 0x0CB2
        PixelMapIToRSize = 3250,
        //
        // Summary:
        //     Original was GL_PIXEL_MAP_I_TO_G_SIZE = 0x0CB3
        PixelMapIToGSize = 3251,
        //
        // Summary:
        //     Original was GL_PIXEL_MAP_I_TO_B_SIZE = 0x0CB4
        PixelMapIToBSize = 3252,
        //
        // Summary:
        //     Original was GL_PIXEL_MAP_I_TO_A_SIZE = 0x0CB5
        PixelMapIToASize = 3253,
        //
        // Summary:
        //     Original was GL_PIXEL_MAP_R_TO_R_SIZE = 0x0CB6
        PixelMapRToRSize = 3254,
        //
        // Summary:
        //     Original was GL_PIXEL_MAP_G_TO_G_SIZE = 0x0CB7
        PixelMapGToGSize = 3255,
        //
        // Summary:
        //     Original was GL_PIXEL_MAP_B_TO_B_SIZE = 0x0CB8
        PixelMapBToBSize = 3256,
        //
        // Summary:
        //     Original was GL_PIXEL_MAP_A_TO_A_SIZE = 0x0CB9
        PixelMapAToASize = 3257,
        //
        // Summary:
        //     Original was GL_UNPACK_SWAP_BYTES = 0x0CF0
        UnpackSwapBytes = 3312,
        //
        // Summary:
        //     Original was GL_UNPACK_LSB_FIRST = 0x0CF1
        UnpackLsbFirst = 3313,
        //
        // Summary:
        //     Original was GL_UNPACK_ROW_LENGTH = 0x0CF2
        UnpackRowLength = 3314,
        //
        // Summary:
        //     Original was GL_UNPACK_SKIP_ROWS = 0x0CF3
        UnpackSkipRows = 3315,
        //
        // Summary:
        //     Original was GL_UNPACK_SKIP_PIXELS = 0x0CF4
        UnpackSkipPixels = 3316,
        //
        // Summary:
        //     Original was GL_UNPACK_ALIGNMENT = 0x0CF5
        UnpackAlignment = 3317,
        //
        // Summary:
        //     Original was GL_PACK_SWAP_BYTES = 0x0D00
        PackSwapBytes = 3328,
        //
        // Summary:
        //     Original was GL_PACK_LSB_FIRST = 0x0D01
        PackLsbFirst = 3329,
        //
        // Summary:
        //     Original was GL_PACK_ROW_LENGTH = 0x0D02
        PackRowLength = 3330,
        //
        // Summary:
        //     Original was GL_PACK_SKIP_ROWS = 0x0D03
        PackSkipRows = 3331,
        //
        // Summary:
        //     Original was GL_PACK_SKIP_PIXELS = 0x0D04
        PackSkipPixels = 3332,
        //
        // Summary:
        //     Original was GL_PACK_ALIGNMENT = 0x0D05
        PackAlignment = 3333,
        //
        // Summary:
        //     Original was GL_MAP_COLOR = 0x0D10
        MapColor = 3344,
        //
        // Summary:
        //     Original was GL_MAP_STENCIL = 0x0D11
        MapStencil = 3345,
        //
        // Summary:
        //     Original was GL_INDEX_SHIFT = 0x0D12
        IndexShift = 3346,
        //
        // Summary:
        //     Original was GL_INDEX_OFFSET = 0x0D13
        IndexOffset = 3347,
        //
        // Summary:
        //     Original was GL_RED_SCALE = 0x0D14
        RedScale = 3348,
        //
        // Summary:
        //     Original was GL_RED_BIAS = 0x0D15
        RedBias = 3349,
        //
        // Summary:
        //     Original was GL_ZOOM_X = 0x0D16
        ZoomX = 3350,
        //
        // Summary:
        //     Original was GL_ZOOM_Y = 0x0D17
        ZoomY = 3351,
        //
        // Summary:
        //     Original was GL_GREEN_SCALE = 0x0D18
        GreenScale = 3352,
        //
        // Summary:
        //     Original was GL_GREEN_BIAS = 0x0D19
        GreenBias = 3353,
        //
        // Summary:
        //     Original was GL_BLUE_SCALE = 0x0D1A
        BlueScale = 3354,
        //
        // Summary:
        //     Original was GL_BLUE_BIAS = 0x0D1B
        BlueBias = 3355,
        //
        // Summary:
        //     Original was GL_ALPHA_SCALE = 0x0D1C
        AlphaScale = 3356,
        //
        // Summary:
        //     Original was GL_ALPHA_BIAS = 0x0D1D
        AlphaBias = 3357,
        //
        // Summary:
        //     Original was GL_DEPTH_SCALE = 0x0D1E
        DepthScale = 3358,
        //
        // Summary:
        //     Original was GL_DEPTH_BIAS = 0x0D1F
        DepthBias = 3359,
        //
        // Summary:
        //     Original was GL_MAX_EVAL_ORDER = 0x0D30
        MaxEvalOrder = 3376,
        //
        // Summary:
        //     Original was GL_MAX_LIGHTS = 0x0D31
        MaxLights = 3377,
        //
        // Summary:
        //     Original was GL_MAX_CLIP_DISTANCES = 0x0D32
        MaxClipDistances = 3378,
        //
        // Summary:
        //     Original was GL_MAX_CLIP_PLANES = 0x0D32
        MaxClipPlanes = 3378,
        //
        // Summary:
        //     Original was GL_MAX_TEXTURE_SIZE = 0x0D33
        MaxTextureSize = 3379,
        //
        // Summary:
        //     Original was GL_MAX_PIXEL_MAP_TABLE = 0x0D34
        MaxPixelMapTable = 3380,
        //
        // Summary:
        //     Original was GL_MAX_ATTRIB_STACK_DEPTH = 0x0D35
        MaxAttribStackDepth = 3381,
        //
        // Summary:
        //     Original was GL_MAX_MODELVIEW_STACK_DEPTH = 0x0D36
        MaxModelviewStackDepth = 3382,
        //
        // Summary:
        //     Original was GL_MAX_NAME_STACK_DEPTH = 0x0D37
        MaxNameStackDepth = 3383,
        //
        // Summary:
        //     Original was GL_MAX_PROJECTION_STACK_DEPTH = 0x0D38
        MaxProjectionStackDepth = 3384,
        //
        // Summary:
        //     Original was GL_MAX_TEXTURE_STACK_DEPTH = 0x0D39
        MaxTextureStackDepth = 3385,
        //
        // Summary:
        //     Original was GL_MAX_VIEWPORT_DIMS = 0x0D3A
        MaxViewportDims = 3386,
        //
        // Summary:
        //     Original was GL_MAX_CLIENT_ATTRIB_STACK_DEPTH = 0x0D3B
        MaxClientAttribStackDepth = 3387,
        //
        // Summary:
        //     Original was GL_SUBPIXEL_BITS = 0x0D50
        SubpixelBits = 3408,
        //
        // Summary:
        //     Original was GL_INDEX_BITS = 0x0D51
        IndexBits = 3409,
        //
        // Summary:
        //     Original was GL_RED_BITS = 0x0D52
        RedBits = 3410,
        //
        // Summary:
        //     Original was GL_GREEN_BITS = 0x0D53
        GreenBits = 3411,
        //
        // Summary:
        //     Original was GL_BLUE_BITS = 0x0D54
        BlueBits = 3412,
        //
        // Summary:
        //     Original was GL_ALPHA_BITS = 0x0D55
        AlphaBits = 3413,
        //
        // Summary:
        //     Original was GL_DEPTH_BITS = 0x0D56
        DepthBits = 3414,
        //
        // Summary:
        //     Original was GL_STENCIL_BITS = 0x0D57
        StencilBits = 3415,
        //
        // Summary:
        //     Original was GL_ACCUM_RED_BITS = 0x0D58
        AccumRedBits = 3416,
        //
        // Summary:
        //     Original was GL_ACCUM_GREEN_BITS = 0x0D59
        AccumGreenBits = 3417,
        //
        // Summary:
        //     Original was GL_ACCUM_BLUE_BITS = 0x0D5A
        AccumBlueBits = 3418,
        //
        // Summary:
        //     Original was GL_ACCUM_ALPHA_BITS = 0x0D5B
        AccumAlphaBits = 3419,
        //
        // Summary:
        //     Original was GL_NAME_STACK_DEPTH = 0x0D70
        NameStackDepth = 3440,
        //
        // Summary:
        //     Original was GL_AUTO_NORMAL = 0x0D80
        AutoNormal = 3456,
        //
        // Summary:
        //     Original was GL_MAP1_COLOR_4 = 0x0D90
        Map1Color4 = 3472,
        //
        // Summary:
        //     Original was GL_MAP1_INDEX = 0x0D91
        Map1Index = 3473,
        //
        // Summary:
        //     Original was GL_MAP1_NORMAL = 0x0D92
        Map1Normal = 3474,
        //
        // Summary:
        //     Original was GL_MAP1_TEXTURE_COORD_1 = 0x0D93
        Map1TextureCoord1 = 3475,
        //
        // Summary:
        //     Original was GL_MAP1_TEXTURE_COORD_2 = 0x0D94
        Map1TextureCoord2 = 3476,
        //
        // Summary:
        //     Original was GL_MAP1_TEXTURE_COORD_3 = 0x0D95
        Map1TextureCoord3 = 3477,
        //
        // Summary:
        //     Original was GL_MAP1_TEXTURE_COORD_4 = 0x0D96
        Map1TextureCoord4 = 3478,
        //
        // Summary:
        //     Original was GL_MAP1_VERTEX_3 = 0x0D97
        Map1Vertex3 = 3479,
        //
        // Summary:
        //     Original was GL_MAP1_VERTEX_4 = 0x0D98
        Map1Vertex4 = 3480,
        //
        // Summary:
        //     Original was GL_MAP2_COLOR_4 = 0x0DB0
        Map2Color4 = 3504,
        //
        // Summary:
        //     Original was GL_MAP2_INDEX = 0x0DB1
        Map2Index = 3505,
        //
        // Summary:
        //     Original was GL_MAP2_NORMAL = 0x0DB2
        Map2Normal = 3506,
        //
        // Summary:
        //     Original was GL_MAP2_TEXTURE_COORD_1 = 0x0DB3
        Map2TextureCoord1 = 3507,
        //
        // Summary:
        //     Original was GL_MAP2_TEXTURE_COORD_2 = 0x0DB4
        Map2TextureCoord2 = 3508,
        //
        // Summary:
        //     Original was GL_MAP2_TEXTURE_COORD_3 = 0x0DB5
        Map2TextureCoord3 = 3509,
        //
        // Summary:
        //     Original was GL_MAP2_TEXTURE_COORD_4 = 0x0DB6
        Map2TextureCoord4 = 3510,
        //
        // Summary:
        //     Original was GL_MAP2_VERTEX_3 = 0x0DB7
        Map2Vertex3 = 3511,
        //
        // Summary:
        //     Original was GL_MAP2_VERTEX_4 = 0x0DB8
        Map2Vertex4 = 3512,
        //
        // Summary:
        //     Original was GL_MAP1_GRID_DOMAIN = 0x0DD0
        Map1GridDomain = 3536,
        //
        // Summary:
        //     Original was GL_MAP1_GRID_SEGMENTS = 0x0DD1
        Map1GridSegments = 3537,
        //
        // Summary:
        //     Original was GL_MAP2_GRID_DOMAIN = 0x0DD2
        Map2GridDomain = 3538,
        //
        // Summary:
        //     Original was GL_MAP2_GRID_SEGMENTS = 0x0DD3
        Map2GridSegments = 3539,
        //
        // Summary:
        //     Original was GL_TEXTURE_1D = 0x0DE0
        Texture1D = 3552,
        //
        // Summary:
        //     Original was GL_TEXTURE_2D = 0x0DE1
        Texture2D = 3553,
        //
        // Summary:
        //     Original was GL_FEEDBACK_BUFFER_SIZE = 0x0DF1
        FeedbackBufferSize = 3569,
        //
        // Summary:
        //     Original was GL_FEEDBACK_BUFFER_TYPE = 0x0DF2
        FeedbackBufferType = 3570,
        //
        // Summary:
        //     Original was GL_SELECTION_BUFFER_SIZE = 0x0DF4
        SelectionBufferSize = 3572,
        //
        // Summary:
        //     Original was GL_POLYGON_OFFSET_UNITS = 0x2A00
        PolygonOffsetUnits = 10752,
        //
        // Summary:
        //     Original was GL_POLYGON_OFFSET_POINT = 0x2A01
        PolygonOffsetPoint = 10753,
        //
        // Summary:
        //     Original was GL_POLYGON_OFFSET_LINE = 0x2A02
        PolygonOffsetLine = 10754,
        //
        // Summary:
        //     Original was GL_CLIP_PLANE0 = 0x3000
        ClipPlane0 = 12288,
        //
        // Summary:
        //     Original was GL_CLIP_PLANE1 = 0x3001
        ClipPlane1 = 12289,
        //
        // Summary:
        //     Original was GL_CLIP_PLANE2 = 0x3002
        ClipPlane2 = 12290,
        //
        // Summary:
        //     Original was GL_CLIP_PLANE3 = 0x3003
        ClipPlane3 = 12291,
        //
        // Summary:
        //     Original was GL_CLIP_PLANE4 = 0x3004
        ClipPlane4 = 12292,
        //
        // Summary:
        //     Original was GL_CLIP_PLANE5 = 0x3005
        ClipPlane5 = 12293,
        //
        // Summary:
        //     Original was GL_LIGHT0 = 0x4000
        Light0 = 16384,
        //
        // Summary:
        //     Original was GL_LIGHT1 = 0x4001
        Light1 = 16385,
        //
        // Summary:
        //     Original was GL_LIGHT2 = 0x4002
        Light2 = 16386,
        //
        // Summary:
        //     Original was GL_LIGHT3 = 0x4003
        Light3 = 16387,
        //
        // Summary:
        //     Original was GL_LIGHT4 = 0x4004
        Light4 = 16388,
        //
        // Summary:
        //     Original was GL_LIGHT5 = 0x4005
        Light5 = 16389,
        //
        // Summary:
        //     Original was GL_LIGHT6 = 0x4006
        Light6 = 16390,
        //
        // Summary:
        //     Original was GL_LIGHT7 = 0x4007
        Light7 = 16391,
        //
        // Summary:
        //     Original was GL_BLEND_COLOR_EXT = 0x8005
        BlendColorExt = 32773,
        //
        // Summary:
        //     Original was GL_BLEND_EQUATION_RGB = 0x8009
        BlendEquationRgb = 32777,
        //
        // Summary:
        //     Original was GL_BLEND_EQUATION_EXT = 0x8009
        BlendEquationExt = 32777,
        //
        // Summary:
        //     Original was GL_PACK_CMYK_HINT_EXT = 0x800E
        PackCmykHintExt = 32782,
        //
        // Summary:
        //     Original was GL_UNPACK_CMYK_HINT_EXT = 0x800F
        UnpackCmykHintExt = 32783,
        //
        // Summary:
        //     Original was GL_CONVOLUTION_1D_EXT = 0x8010
        Convolution1DExt = 32784,
        //
        // Summary:
        //     Original was GL_CONVOLUTION_2D_EXT = 0x8011
        Convolution2DExt = 32785,
        //
        // Summary:
        //     Original was GL_SEPARABLE_2D_EXT = 0x8012
        Separable2DExt = 32786,
        //
        // Summary:
        //     Original was GL_POST_CONVOLUTION_RED_SCALE_EXT = 0x801C
        PostConvolutionRedScaleExt = 32796,
        //
        // Summary:
        //     Original was GL_POST_CONVOLUTION_GREEN_SCALE_EXT = 0x801D
        PostConvolutionGreenScaleExt = 32797,
        //
        // Summary:
        //     Original was GL_POST_CONVOLUTION_BLUE_SCALE_EXT = 0x801E
        PostConvolutionBlueScaleExt = 32798,
        //
        // Summary:
        //     Original was GL_POST_CONVOLUTION_ALPHA_SCALE_EXT = 0x801F
        PostConvolutionAlphaScaleExt = 32799,
        //
        // Summary:
        //     Original was GL_POST_CONVOLUTION_RED_BIAS_EXT = 0x8020
        PostConvolutionRedBiasExt = 32800,
        //
        // Summary:
        //     Original was GL_POST_CONVOLUTION_GREEN_BIAS_EXT = 0x8021
        PostConvolutionGreenBiasExt = 32801,
        //
        // Summary:
        //     Original was GL_POST_CONVOLUTION_BLUE_BIAS_EXT = 0x8022
        PostConvolutionBlueBiasExt = 32802,
        //
        // Summary:
        //     Original was GL_POST_CONVOLUTION_ALPHA_BIAS_EXT = 0x8023
        PostConvolutionAlphaBiasExt = 32803,
        //
        // Summary:
        //     Original was GL_HISTOGRAM_EXT = 0x8024
        HistogramExt = 32804,
        //
        // Summary:
        //     Original was GL_MINMAX_EXT = 0x802E
        MinmaxExt = 32814,
        //
        // Summary:
        //     Original was GL_POLYGON_OFFSET_FILL = 0x8037
        PolygonOffsetFill = 32823,
        //
        // Summary:
        //     Original was GL_POLYGON_OFFSET_FACTOR = 0x8038
        PolygonOffsetFactor = 32824,
        //
        // Summary:
        //     Original was GL_POLYGON_OFFSET_BIAS_EXT = 0x8039
        PolygonOffsetBiasExt = 32825,
        //
        // Summary:
        //     Original was GL_RESCALE_NORMAL_EXT = 0x803A
        RescaleNormalExt = 32826,
        //
        // Summary:
        //     Original was GL_TEXTURE_BINDING_1D = 0x8068
        TextureBinding1D = 32872,
        //
        // Summary:
        //     Original was GL_TEXTURE_BINDING_2D = 0x8069
        TextureBinding2D = 32873,
        //
        // Summary:
        //     Original was GL_TEXTURE_3D_BINDING_EXT = 0x806A
        Texture3DBindingExt = 32874,
        //
        // Summary:
        //     Original was GL_TEXTURE_BINDING_3D = 0x806A
        TextureBinding3D = 32874,
        //
        // Summary:
        //     Original was GL_PACK_SKIP_IMAGES_EXT = 0x806B
        PackSkipImagesExt = 32875,
        //
        // Summary:
        //     Original was GL_PACK_IMAGE_HEIGHT_EXT = 0x806C
        PackImageHeightExt = 32876,
        //
        // Summary:
        //     Original was GL_UNPACK_SKIP_IMAGES_EXT = 0x806D
        UnpackSkipImagesExt = 32877,
        //
        // Summary:
        //     Original was GL_UNPACK_IMAGE_HEIGHT_EXT = 0x806E
        UnpackImageHeightExt = 32878,
        //
        // Summary:
        //     Original was GL_TEXTURE_3D_EXT = 0x806F
        Texture3DExt = 32879,
        //
        // Summary:
        //     Original was GL_MAX_3D_TEXTURE_SIZE = 0x8073
        Max3DTextureSize = 32883,
        //
        // Summary:
        //     Original was GL_MAX_3D_TEXTURE_SIZE_EXT = 0x8073
        Max3DTextureSizeExt = 32883,
        //
        // Summary:
        //     Original was GL_VERTEX_ARRAY = 0x8074
        VertexArray = 32884,
        //
        // Summary:
        //     Original was GL_NORMAL_ARRAY = 0x8075
        NormalArray = 32885,
        //
        // Summary:
        //     Original was GL_COLOR_ARRAY = 0x8076
        ColorArray = 32886,
        //
        // Summary:
        //     Original was GL_INDEX_ARRAY = 0x8077
        IndexArray = 32887,
        //
        // Summary:
        //     Original was GL_TEXTURE_COORD_ARRAY = 0x8078
        TextureCoordArray = 32888,
        //
        // Summary:
        //     Original was GL_EDGE_FLAG_ARRAY = 0x8079
        EdgeFlagArray = 32889,
        //
        // Summary:
        //     Original was GL_VERTEX_ARRAY_SIZE = 0x807A
        VertexArraySize = 32890,
        //
        // Summary:
        //     Original was GL_VERTEX_ARRAY_TYPE = 0x807B
        VertexArrayType = 32891,
        //
        // Summary:
        //     Original was GL_VERTEX_ARRAY_STRIDE = 0x807C
        VertexArrayStride = 32892,
        //
        // Summary:
        //     Original was GL_VERTEX_ARRAY_COUNT_EXT = 0x807D
        VertexArrayCountExt = 32893,
        //
        // Summary:
        //     Original was GL_NORMAL_ARRAY_TYPE = 0x807E
        NormalArrayType = 32894,
        //
        // Summary:
        //     Original was GL_NORMAL_ARRAY_STRIDE = 0x807F
        NormalArrayStride = 32895,
        //
        // Summary:
        //     Original was GL_NORMAL_ARRAY_COUNT_EXT = 0x8080
        NormalArrayCountExt = 32896,
        //
        // Summary:
        //     Original was GL_COLOR_ARRAY_SIZE = 0x8081
        ColorArraySize = 32897,
        //
        // Summary:
        //     Original was GL_COLOR_ARRAY_TYPE = 0x8082
        ColorArrayType = 32898,
        //
        // Summary:
        //     Original was GL_COLOR_ARRAY_STRIDE = 0x8083
        ColorArrayStride = 32899,
        //
        // Summary:
        //     Original was GL_COLOR_ARRAY_COUNT_EXT = 0x8084
        ColorArrayCountExt = 32900,
        //
        // Summary:
        //     Original was GL_INDEX_ARRAY_TYPE = 0x8085
        IndexArrayType = 32901,
        //
        // Summary:
        //     Original was GL_INDEX_ARRAY_STRIDE = 0x8086
        IndexArrayStride = 32902,
        //
        // Summary:
        //     Original was GL_INDEX_ARRAY_COUNT_EXT = 0x8087
        IndexArrayCountExt = 32903,
        //
        // Summary:
        //     Original was GL_TEXTURE_COORD_ARRAY_SIZE = 0x8088
        TextureCoordArraySize = 32904,
        //
        // Summary:
        //     Original was GL_TEXTURE_COORD_ARRAY_TYPE = 0x8089
        TextureCoordArrayType = 32905,
        //
        // Summary:
        //     Original was GL_TEXTURE_COORD_ARRAY_STRIDE = 0x808A
        TextureCoordArrayStride = 32906,
        //
        // Summary:
        //     Original was GL_TEXTURE_COORD_ARRAY_COUNT_EXT = 0x808B
        TextureCoordArrayCountExt = 32907,
        //
        // Summary:
        //     Original was GL_EDGE_FLAG_ARRAY_STRIDE = 0x808C
        EdgeFlagArrayStride = 32908,
        //
        // Summary:
        //     Original was GL_EDGE_FLAG_ARRAY_COUNT_EXT = 0x808D
        EdgeFlagArrayCountExt = 32909,
        //
        // Summary:
        //     Original was GL_INTERLACE_SGIX = 0x8094
        InterlaceSgix = 32916,
        //
        // Summary:
        //     Original was GL_DETAIL_TEXTURE_2D_BINDING_SGIS = 0x8096
        DetailTexture2DBindingSgis = 32918,
        //
        // Summary:
        //     Original was GL_MULTISAMPLE = 0x809D
        Multisample = 32925,
        //
        // Summary:
        //     Original was GL_MULTISAMPLE_SGIS = 0x809D
        MultisampleSgis = 32925,
        //
        // Summary:
        //     Original was GL_SAMPLE_ALPHA_TO_COVERAGE = 0x809E
        SampleAlphaToCoverage = 32926,
        //
        // Summary:
        //     Original was GL_SAMPLE_ALPHA_TO_MASK_SGIS = 0x809E
        SampleAlphaToMaskSgis = 32926,
        //
        // Summary:
        //     Original was GL_SAMPLE_ALPHA_TO_ONE_SGIS = 0x809F
        SampleAlphaToOneSgis = 32927,
        //
        // Summary:
        //     Original was GL_SAMPLE_ALPHA_TO_ONE = 0x809F
        SampleAlphaToOne = 32927,
        //
        // Summary:
        //     Original was GL_SAMPLE_COVERAGE = 0x80A0
        SampleCoverage = 32928,
        //
        // Summary:
        //     Original was GL_SAMPLE_MASK_SGIS = 0x80A0
        SampleMaskSgis = 32928,
        //
        // Summary:
        //     Original was GL_SAMPLE_BUFFERS = 0x80A8
        SampleBuffers = 32936,
        //
        // Summary:
        //     Original was GL_SAMPLE_BUFFERS_SGIS = 0x80A8
        SampleBuffersSgis = 32936,
        //
        // Summary:
        //     Original was GL_SAMPLES = 0x80A9
        Samples = 32937,
        //
        // Summary:
        //     Original was GL_SAMPLES_SGIS = 0x80A9
        SamplesSgis = 32937,
        //
        // Summary:
        //     Original was GL_SAMPLE_COVERAGE_VALUE = 0x80AA
        SampleCoverageValue = 32938,
        //
        // Summary:
        //     Original was GL_SAMPLE_MASK_VALUE_SGIS = 0x80AA
        SampleMaskValueSgis = 32938,
        //
        // Summary:
        //     Original was GL_SAMPLE_COVERAGE_INVERT = 0x80AB
        SampleCoverageInvert = 32939,
        //
        // Summary:
        //     Original was GL_SAMPLE_MASK_INVERT_SGIS = 0x80AB
        SampleMaskInvertSgis = 32939,
        //
        // Summary:
        //     Original was GL_SAMPLE_PATTERN_SGIS = 0x80AC
        SamplePatternSgis = 32940,
        //
        // Summary:
        //     Original was GL_COLOR_MATRIX_SGI = 0x80B1
        ColorMatrixSgi = 32945,
        //
        // Summary:
        //     Original was GL_COLOR_MATRIX_STACK_DEPTH_SGI = 0x80B2
        ColorMatrixStackDepthSgi = 32946,
        //
        // Summary:
        //     Original was GL_MAX_COLOR_MATRIX_STACK_DEPTH_SGI = 0x80B3
        MaxColorMatrixStackDepthSgi = 32947,
        //
        // Summary:
        //     Original was GL_POST_COLOR_MATRIX_RED_SCALE_SGI = 0x80B4
        PostColorMatrixRedScaleSgi = 32948,
        //
        // Summary:
        //     Original was GL_POST_COLOR_MATRIX_GREEN_SCALE_SGI = 0x80B5
        PostColorMatrixGreenScaleSgi = 32949,
        //
        // Summary:
        //     Original was GL_POST_COLOR_MATRIX_BLUE_SCALE_SGI = 0x80B6
        PostColorMatrixBlueScaleSgi = 32950,
        //
        // Summary:
        //     Original was GL_POST_COLOR_MATRIX_ALPHA_SCALE_SGI = 0x80B7
        PostColorMatrixAlphaScaleSgi = 32951,
        //
        // Summary:
        //     Original was GL_POST_COLOR_MATRIX_RED_BIAS_SGI = 0x80B8
        PostColorMatrixRedBiasSgi = 32952,
        //
        // Summary:
        //     Original was GL_POST_COLOR_MATRIX_GREEN_BIAS_SGI = 0x80B9
        PostColorMatrixGreenBiasSgi = 32953,
        //
        // Summary:
        //     Original was GL_POST_COLOR_MATRIX_BLUE_BIAS_SGI = 0x80BA
        PostColorMatrixBlueBiasSgi = 32954,
        //
        // Summary:
        //     Original was GL_POST_COLOR_MATRIX_ALPHA_BIAS_SGI = 0x80BB
        PostColorMatrixAlphaBiasSgi = 32955,
        //
        // Summary:
        //     Original was GL_TEXTURE_COLOR_TABLE_SGI = 0x80BC
        TextureColorTableSgi = 32956,
        //
        // Summary:
        //     Original was GL_BLEND_DST_RGB = 0x80C8
        BlendDstRgb = 32968,
        //
        // Summary:
        //     Original was GL_BLEND_SRC_RGB = 0x80C9
        BlendSrcRgb = 32969,
        //
        // Summary:
        //     Original was GL_BLEND_DST_ALPHA = 0x80CA
        BlendDstAlpha = 32970,
        //
        // Summary:
        //     Original was GL_BLEND_SRC_ALPHA = 0x80CB
        BlendSrcAlpha = 32971,
        //
        // Summary:
        //     Original was GL_COLOR_TABLE_SGI = 0x80D0
        ColorTableSgi = 32976,
        //
        // Summary:
        //     Original was GL_POST_CONVOLUTION_COLOR_TABLE_SGI = 0x80D1
        PostConvolutionColorTableSgi = 32977,
        //
        // Summary:
        //     Original was GL_POST_COLOR_MATRIX_COLOR_TABLE_SGI = 0x80D2
        PostColorMatrixColorTableSgi = 32978,
        //
        // Summary:
        //     Original was GL_MAX_ELEMENTS_VERTICES = 0x80E8
        MaxElementsVertices = 33000,
        //
        // Summary:
        //     Original was GL_MAX_ELEMENTS_INDICES = 0x80E9
        MaxElementsIndices = 33001,
        //
        // Summary:
        //     Original was GL_POINT_SIZE_MIN = 0x8126
        PointSizeMin = 33062,
        //
        // Summary:
        //     Original was GL_POINT_SIZE_MIN_SGIS = 0x8126
        PointSizeMinSgis = 33062,
        //
        // Summary:
        //     Original was GL_POINT_SIZE_MAX = 0x8127
        PointSizeMax = 33063,
        //
        // Summary:
        //     Original was GL_POINT_SIZE_MAX_SGIS = 0x8127
        PointSizeMaxSgis = 33063,
        //
        // Summary:
        //     Original was GL_POINT_FADE_THRESHOLD_SIZE = 0x8128
        PointFadeThresholdSize = 33064,
        //
        // Summary:
        //     Original was GL_POINT_FADE_THRESHOLD_SIZE_SGIS = 0x8128
        PointFadeThresholdSizeSgis = 33064,
        //
        // Summary:
        //     Original was GL_POINT_DISTANCE_ATTENUATION = 0x8129
        PointDistanceAttenuation = 33065,
        //
        // Summary:
        //     Original was GL_DISTANCE_ATTENUATION_SGIS = 0x8129
        DistanceAttenuationSgis = 33065,
        //
        // Summary:
        //     Original was GL_FOG_FUNC_POINTS_SGIS = 0x812B
        FogFuncPointsSgis = 33067,
        //
        // Summary:
        //     Original was GL_MAX_FOG_FUNC_POINTS_SGIS = 0x812C
        MaxFogFuncPointsSgis = 33068,
        //
        // Summary:
        //     Original was GL_PACK_SKIP_VOLUMES_SGIS = 0x8130
        PackSkipVolumesSgis = 33072,
        //
        // Summary:
        //     Original was GL_PACK_IMAGE_DEPTH_SGIS = 0x8131
        PackImageDepthSgis = 33073,
        //
        // Summary:
        //     Original was GL_UNPACK_SKIP_VOLUMES_SGIS = 0x8132
        UnpackSkipVolumesSgis = 33074,
        //
        // Summary:
        //     Original was GL_UNPACK_IMAGE_DEPTH_SGIS = 0x8133
        UnpackImageDepthSgis = 33075,
        //
        // Summary:
        //     Original was GL_TEXTURE_4D_SGIS = 0x8134
        Texture4DSgis = 33076,
        //
        // Summary:
        //     Original was GL_MAX_4D_TEXTURE_SIZE_SGIS = 0x8138
        Max4DTextureSizeSgis = 33080,
        //
        // Summary:
        //     Original was GL_PIXEL_TEX_GEN_SGIX = 0x8139
        PixelTexGenSgix = 33081,
        //
        // Summary:
        //     Original was GL_PIXEL_TILE_BEST_ALIGNMENT_SGIX = 0x813E
        PixelTileBestAlignmentSgix = 33086,
        //
        // Summary:
        //     Original was GL_PIXEL_TILE_CACHE_INCREMENT_SGIX = 0x813F
        PixelTileCacheIncrementSgix = 33087,
        //
        // Summary:
        //     Original was GL_PIXEL_TILE_WIDTH_SGIX = 0x8140
        PixelTileWidthSgix = 33088,
        //
        // Summary:
        //     Original was GL_PIXEL_TILE_HEIGHT_SGIX = 0x8141
        PixelTileHeightSgix = 33089,
        //
        // Summary:
        //     Original was GL_PIXEL_TILE_GRID_WIDTH_SGIX = 0x8142
        PixelTileGridWidthSgix = 33090,
        //
        // Summary:
        //     Original was GL_PIXEL_TILE_GRID_HEIGHT_SGIX = 0x8143
        PixelTileGridHeightSgix = 33091,
        //
        // Summary:
        //     Original was GL_PIXEL_TILE_GRID_DEPTH_SGIX = 0x8144
        PixelTileGridDepthSgix = 33092,
        //
        // Summary:
        //     Original was GL_PIXEL_TILE_CACHE_SIZE_SGIX = 0x8145
        PixelTileCacheSizeSgix = 33093,
        //
        // Summary:
        //     Original was GL_SPRITE_SGIX = 0x8148
        SpriteSgix = 33096,
        //
        // Summary:
        //     Original was GL_SPRITE_MODE_SGIX = 0x8149
        SpriteModeSgix = 33097,
        //
        // Summary:
        //     Original was GL_SPRITE_AXIS_SGIX = 0x814A
        SpriteAxisSgix = 33098,
        //
        // Summary:
        //     Original was GL_SPRITE_TRANSLATION_SGIX = 0x814B
        SpriteTranslationSgix = 33099,
        //
        // Summary:
        //     Original was GL_TEXTURE_4D_BINDING_SGIS = 0x814F
        Texture4DBindingSgis = 33103,
        //
        // Summary:
        //     Original was GL_MAX_CLIPMAP_DEPTH_SGIX = 0x8177
        MaxClipmapDepthSgix = 33143,
        //
        // Summary:
        //     Original was GL_MAX_CLIPMAP_VIRTUAL_DEPTH_SGIX = 0x8178
        MaxClipmapVirtualDepthSgix = 33144,
        //
        // Summary:
        //     Original was GL_POST_TEXTURE_FILTER_BIAS_RANGE_SGIX = 0x817B
        PostTextureFilterBiasRangeSgix = 33147,
        //
        // Summary:
        //     Original was GL_POST_TEXTURE_FILTER_SCALE_RANGE_SGIX = 0x817C
        PostTextureFilterScaleRangeSgix = 33148,
        //
        // Summary:
        //     Original was GL_REFERENCE_PLANE_SGIX = 0x817D
        ReferencePlaneSgix = 33149,
        //
        // Summary:
        //     Original was GL_REFERENCE_PLANE_EQUATION_SGIX = 0x817E
        ReferencePlaneEquationSgix = 33150,
        //
        // Summary:
        //     Original was GL_IR_INSTRUMENT1_SGIX = 0x817F
        IrInstrument1Sgix = 33151,
        //
        // Summary:
        //     Original was GL_INSTRUMENT_MEASUREMENTS_SGIX = 0x8181
        InstrumentMeasurementsSgix = 33153,
        //
        // Summary:
        //     Original was GL_CALLIGRAPHIC_FRAGMENT_SGIX = 0x8183
        CalligraphicFragmentSgix = 33155,
        //
        // Summary:
        //     Original was GL_FRAMEZOOM_SGIX = 0x818B
        FramezoomSgix = 33163,
        //
        // Summary:
        //     Original was GL_FRAMEZOOM_FACTOR_SGIX = 0x818C
        FramezoomFactorSgix = 33164,
        //
        // Summary:
        //     Original was GL_MAX_FRAMEZOOM_FACTOR_SGIX = 0x818D
        MaxFramezoomFactorSgix = 33165,
        //
        // Summary:
        //     Original was GL_GENERATE_MIPMAP_HINT_SGIS = 0x8192
        GenerateMipmapHintSgis = 33170,
        //
        // Summary:
        //     Original was GL_GENERATE_MIPMAP_HINT = 0x8192
        GenerateMipmapHint = 33170,
        //
        // Summary:
        //     Original was GL_DEFORMATIONS_MASK_SGIX = 0x8196
        DeformationsMaskSgix = 33174,
        //
        // Summary:
        //     Original was GL_FOG_OFFSET_SGIX = 0x8198
        FogOffsetSgix = 33176,
        //
        // Summary:
        //     Original was GL_FOG_OFFSET_VALUE_SGIX = 0x8199
        FogOffsetValueSgix = 33177,
        //
        // Summary:
        //     Original was GL_LIGHT_MODEL_COLOR_CONTROL = 0x81F8
        LightModelColorControl = 33272,
        //
        // Summary:
        //     Original was GL_SHARED_TEXTURE_PALETTE_EXT = 0x81FB
        SharedTexturePaletteExt = 33275,
        //
        // Summary:
        //     Original was GL_MAJOR_VERSION = 0x821B
        MajorVersion = 33307,
        //
        // Summary:
        //     Original was GL_MINOR_VERSION = 0x821C
        MinorVersion = 33308,
        //
        // Summary:
        //     Original was GL_NUM_EXTENSIONS = 0x821D
        NumExtensions = 33309,
        //
        // Summary:
        //     Original was GL_CONTEXT_FLAGS = 0x821E
        ContextFlags = 33310,
        //
        // Summary:
        //     Original was GL_PROGRAM_PIPELINE_BINDING = 0x825A
        ProgramPipelineBinding = 33370,
        //
        // Summary:
        //     Original was GL_MAX_VIEWPORTS = 0x825B
        MaxViewports = 33371,
        //
        // Summary:
        //     Original was GL_VIEWPORT_SUBPIXEL_BITS = 0x825C
        ViewportSubpixelBits = 33372,
        //
        // Summary:
        //     Original was GL_VIEWPORT_BOUNDS_RANGE = 0x825D
        ViewportBoundsRange = 33373,
        //
        // Summary:
        //     Original was GL_LAYER_PROVOKING_VERTEX = 0x825E
        LayerProvokingVertex = 33374,
        //
        // Summary:
        //     Original was GL_VIEWPORT_INDEX_PROVOKING_VERTEX = 0x825F
        ViewportIndexProvokingVertex = 33375,
        //
        // Summary:
        //     Original was GL_CONVOLUTION_HINT_SGIX = 0x8316
        ConvolutionHintSgix = 33558,
        //
        // Summary:
        //     Original was GL_ASYNC_MARKER_SGIX = 0x8329
        AsyncMarkerSgix = 33577,
        //
        // Summary:
        //     Original was GL_PIXEL_TEX_GEN_MODE_SGIX = 0x832B
        PixelTexGenModeSgix = 33579,
        //
        // Summary:
        //     Original was GL_ASYNC_HISTOGRAM_SGIX = 0x832C
        AsyncHistogramSgix = 33580,
        //
        // Summary:
        //     Original was GL_MAX_ASYNC_HISTOGRAM_SGIX = 0x832D
        MaxAsyncHistogramSgix = 33581,
        //
        // Summary:
        //     Original was GL_PIXEL_TEXTURE_SGIS = 0x8353
        PixelTextureSgis = 33619,
        //
        // Summary:
        //     Original was GL_ASYNC_TEX_IMAGE_SGIX = 0x835C
        AsyncTexImageSgix = 33628,
        //
        // Summary:
        //     Original was GL_ASYNC_DRAW_PIXELS_SGIX = 0x835D
        AsyncDrawPixelsSgix = 33629,
        //
        // Summary:
        //     Original was GL_ASYNC_READ_PIXELS_SGIX = 0x835E
        AsyncReadPixelsSgix = 33630,
        //
        // Summary:
        //     Original was GL_MAX_ASYNC_TEX_IMAGE_SGIX = 0x835F
        MaxAsyncTexImageSgix = 33631,
        //
        // Summary:
        //     Original was GL_MAX_ASYNC_DRAW_PIXELS_SGIX = 0x8360
        MaxAsyncDrawPixelsSgix = 33632,
        //
        // Summary:
        //     Original was GL_MAX_ASYNC_READ_PIXELS_SGIX = 0x8361
        MaxAsyncReadPixelsSgix = 33633,
        //
        // Summary:
        //     Original was GL_VERTEX_PRECLIP_SGIX = 0x83EE
        VertexPreclipSgix = 33774,
        //
        // Summary:
        //     Original was GL_VERTEX_PRECLIP_HINT_SGIX = 0x83EF
        VertexPreclipHintSgix = 33775,
        //
        // Summary:
        //     Original was GL_FRAGMENT_LIGHTING_SGIX = 0x8400
        FragmentLightingSgix = 33792,
        //
        // Summary:
        //     Original was GL_FRAGMENT_COLOR_MATERIAL_SGIX = 0x8401
        FragmentColorMaterialSgix = 33793,
        //
        // Summary:
        //     Original was GL_FRAGMENT_COLOR_MATERIAL_FACE_SGIX = 0x8402
        FragmentColorMaterialFaceSgix = 33794,
        //
        // Summary:
        //     Original was GL_FRAGMENT_COLOR_MATERIAL_PARAMETER_SGIX = 0x8403
        FragmentColorMaterialParameterSgix = 33795,
        //
        // Summary:
        //     Original was GL_MAX_FRAGMENT_LIGHTS_SGIX = 0x8404
        MaxFragmentLightsSgix = 33796,
        //
        // Summary:
        //     Original was GL_MAX_ACTIVE_LIGHTS_SGIX = 0x8405
        MaxActiveLightsSgix = 33797,
        //
        // Summary:
        //     Original was GL_LIGHT_ENV_MODE_SGIX = 0x8407
        LightEnvModeSgix = 33799,
        //
        // Summary:
        //     Original was GL_FRAGMENT_LIGHT_MODEL_LOCAL_VIEWER_SGIX = 0x8408
        FragmentLightModelLocalViewerSgix = 33800,
        //
        // Summary:
        //     Original was GL_FRAGMENT_LIGHT_MODEL_TWO_SIDE_SGIX = 0x8409
        FragmentLightModelTwoSideSgix = 33801,
        //
        // Summary:
        //     Original was GL_FRAGMENT_LIGHT_MODEL_AMBIENT_SGIX = 0x840A
        FragmentLightModelAmbientSgix = 33802,
        //
        // Summary:
        //     Original was GL_FRAGMENT_LIGHT_MODEL_NORMAL_INTERPOLATION_SGIX = 0x840B
        FragmentLightModelNormalInterpolationSgix = 33803,
        //
        // Summary:
        //     Original was GL_FRAGMENT_LIGHT0_SGIX = 0x840C
        FragmentLight0Sgix = 33804,
        //
        // Summary:
        //     Original was GL_PACK_RESAMPLE_SGIX = 0x842C
        PackResampleSgix = 33836,
        //
        // Summary:
        //     Original was GL_UNPACK_RESAMPLE_SGIX = 0x842D
        UnpackResampleSgix = 33837,
        //
        // Summary:
        //     Original was GL_CURRENT_FOG_COORD = 0x8453
        CurrentFogCoord = 33875,
        //
        // Summary:
        //     Original was GL_FOG_COORD_ARRAY_TYPE = 0x8454
        FogCoordArrayType = 33876,
        //
        // Summary:
        //     Original was GL_FOG_COORD_ARRAY_STRIDE = 0x8455
        FogCoordArrayStride = 33877,
        //
        // Summary:
        //     Original was GL_COLOR_SUM = 0x8458
        ColorSum = 33880,
        //
        // Summary:
        //     Original was GL_CURRENT_SECONDARY_COLOR = 0x8459
        CurrentSecondaryColor = 33881,
        //
        // Summary:
        //     Original was GL_SECONDARY_COLOR_ARRAY_SIZE = 0x845A
        SecondaryColorArraySize = 33882,
        //
        // Summary:
        //     Original was GL_SECONDARY_COLOR_ARRAY_TYPE = 0x845B
        SecondaryColorArrayType = 33883,
        //
        // Summary:
        //     Original was GL_SECONDARY_COLOR_ARRAY_STRIDE = 0x845C
        SecondaryColorArrayStride = 33884,
        //
        // Summary:
        //     Original was GL_CURRENT_RASTER_SECONDARY_COLOR = 0x845F
        CurrentRasterSecondaryColor = 33887,
        //
        // Summary:
        //     Original was GL_ALIASED_POINT_SIZE_RANGE = 0x846D
        AliasedPointSizeRange = 33901,
        //
        // Summary:
        //     Original was GL_ALIASED_LINE_WIDTH_RANGE = 0x846E
        AliasedLineWidthRange = 33902,
        //
        // Summary:
        //     Original was GL_ACTIVE_TEXTURE = 0x84E0
        ActiveTexture = 34016,
        //
        // Summary:
        //     Original was GL_CLIENT_ACTIVE_TEXTURE = 0x84E1
        ClientActiveTexture = 34017,
        //
        // Summary:
        //     Original was GL_MAX_TEXTURE_UNITS = 0x84E2
        MaxTextureUnits = 34018,
        //
        // Summary:
        //     Original was GL_TRANSPOSE_MODELVIEW_MATRIX = 0x84E3
        TransposeModelviewMatrix = 34019,
        //
        // Summary:
        //     Original was GL_TRANSPOSE_PROJECTION_MATRIX = 0x84E4
        TransposeProjectionMatrix = 34020,
        //
        // Summary:
        //     Original was GL_TRANSPOSE_TEXTURE_MATRIX = 0x84E5
        TransposeTextureMatrix = 34021,
        //
        // Summary:
        //     Original was GL_TRANSPOSE_COLOR_MATRIX = 0x84E6
        TransposeColorMatrix = 34022,
        //
        // Summary:
        //     Original was GL_MAX_RENDERBUFFER_SIZE_EXT = 0x84E8
        MaxRenderbufferSizeExt = 34024,
        //
        // Summary:
        //     Original was GL_MAX_RENDERBUFFER_SIZE = 0x84E8
        MaxRenderbufferSize = 34024,
        //
        // Summary:
        //     Original was GL_TEXTURE_COMPRESSION_HINT = 0x84EF
        TextureCompressionHint = 34031,
        //
        // Summary:
        //     Original was GL_TEXTURE_BINDING_RECTANGLE = 0x84F6
        TextureBindingRectangle = 34038,
        //
        // Summary:
        //     Original was GL_MAX_RECTANGLE_TEXTURE_SIZE = 0x84F8
        MaxRectangleTextureSize = 34040,
        //
        // Summary:
        //     Original was GL_MAX_TEXTURE_LOD_BIAS = 0x84FD
        MaxTextureLodBias = 34045,
        //
        // Summary:
        //     Original was GL_TEXTURE_CUBE_MAP = 0x8513
        TextureCubeMap = 34067,
        //
        // Summary:
        //     Original was GL_TEXTURE_BINDING_CUBE_MAP = 0x8514
        TextureBindingCubeMap = 34068,
        //
        // Summary:
        //     Original was GL_MAX_CUBE_MAP_TEXTURE_SIZE = 0x851C
        MaxCubeMapTextureSize = 34076,
        //
        // Summary:
        //     Original was GL_PACK_SUBSAMPLE_RATE_SGIX = 0x85A0
        PackSubsampleRateSgix = 34208,
        //
        // Summary:
        //     Original was GL_UNPACK_SUBSAMPLE_RATE_SGIX = 0x85A1
        UnpackSubsampleRateSgix = 34209,
        //
        // Summary:
        //     Original was GL_VERTEX_ARRAY_BINDING = 0x85B5
        VertexArrayBinding = 34229,
        //
        // Summary:
        //     Original was GL_PROGRAM_POINT_SIZE = 0x8642
        ProgramPointSize = 34370,
        //
        // Summary:
        //     Original was GL_DEPTH_CLAMP = 0x864F
        DepthClamp = 34383,
        //
        // Summary:
        //     Original was GL_NUM_COMPRESSED_TEXTURE_FORMATS = 0x86A2
        NumCompressedTextureFormats = 34466,
        //
        // Summary:
        //     Original was GL_COMPRESSED_TEXTURE_FORMATS = 0x86A3
        CompressedTextureFormats = 34467,
        //
        // Summary:
        //     Original was GL_NUM_PROGRAM_BINARY_FORMATS = 0x87FE
        NumProgramBinaryFormats = 34814,
        //
        // Summary:
        //     Original was GL_PROGRAM_BINARY_FORMATS = 0x87FF
        ProgramBinaryFormats = 34815,
        //
        // Summary:
        //     Original was GL_STENCIL_BACK_FUNC = 0x8800
        StencilBackFunc = 34816,
        //
        // Summary:
        //     Original was GL_STENCIL_BACK_FAIL = 0x8801
        StencilBackFail = 34817,
        //
        // Summary:
        //     Original was GL_STENCIL_BACK_PASS_DEPTH_FAIL = 0x8802
        StencilBackPassDepthFail = 34818,
        //
        // Summary:
        //     Original was GL_STENCIL_BACK_PASS_DEPTH_PASS = 0x8803
        StencilBackPassDepthPass = 34819,
        //
        // Summary:
        //     Original was GL_RGBA_FLOAT_MODE = 0x8820
        RgbaFloatMode = 34848,
        //
        // Summary:
        //     Original was GL_MAX_DRAW_BUFFERS = 0x8824
        MaxDrawBuffers = 34852,
        //
        // Summary:
        //     Original was GL_DRAW_BUFFER0 = 0x8825
        DrawBuffer0 = 34853,
        //
        // Summary:
        //     Original was GL_DRAW_BUFFER1 = 0x8826
        DrawBuffer1 = 34854,
        //
        // Summary:
        //     Original was GL_DRAW_BUFFER2 = 0x8827
        DrawBuffer2 = 34855,
        //
        // Summary:
        //     Original was GL_DRAW_BUFFER3 = 0x8828
        DrawBuffer3 = 34856,
        //
        // Summary:
        //     Original was GL_DRAW_BUFFER4 = 0x8829
        DrawBuffer4 = 34857,
        //
        // Summary:
        //     Original was GL_DRAW_BUFFER5 = 0x882A
        DrawBuffer5 = 34858,
        //
        // Summary:
        //     Original was GL_DRAW_BUFFER6 = 0x882B
        DrawBuffer6 = 34859,
        //
        // Summary:
        //     Original was GL_DRAW_BUFFER7 = 0x882C
        DrawBuffer7 = 34860,
        //
        // Summary:
        //     Original was GL_DRAW_BUFFER8 = 0x882D
        DrawBuffer8 = 34861,
        //
        // Summary:
        //     Original was GL_DRAW_BUFFER9 = 0x882E
        DrawBuffer9 = 34862,
        //
        // Summary:
        //     Original was GL_DRAW_BUFFER10 = 0x882F
        DrawBuffer10 = 34863,
        //
        // Summary:
        //     Original was GL_DRAW_BUFFER11 = 0x8830
        DrawBuffer11 = 34864,
        //
        // Summary:
        //     Original was GL_DRAW_BUFFER12 = 0x8831
        DrawBuffer12 = 34865,
        //
        // Summary:
        //     Original was GL_DRAW_BUFFER13 = 0x8832
        DrawBuffer13 = 34866,
        //
        // Summary:
        //     Original was GL_DRAW_BUFFER14 = 0x8833
        DrawBuffer14 = 34867,
        //
        // Summary:
        //     Original was GL_DRAW_BUFFER15 = 0x8834
        DrawBuffer15 = 34868,
        //
        // Summary:
        //     Original was GL_BLEND_EQUATION_ALPHA = 0x883D
        BlendEquationAlpha = 34877,
        //
        // Summary:
        //     Original was GL_TEXTURE_CUBE_MAP_SEAMLESS = 0x884F
        TextureCubeMapSeamless = 34895,
        //
        // Summary:
        //     Original was GL_POINT_SPRITE = 0x8861
        PointSprite = 34913,
        //
        // Summary:
        //     Original was GL_MAX_VERTEX_ATTRIBS = 0x8869
        MaxVertexAttribs = 34921,
        //
        // Summary:
        //     Original was GL_MAX_TESS_CONTROL_INPUT_COMPONENTS = 0x886C
        MaxTessControlInputComponents = 34924,
        //
        // Summary:
        //     Original was GL_MAX_TESS_EVALUATION_INPUT_COMPONENTS = 0x886D
        MaxTessEvaluationInputComponents = 34925,
        //
        // Summary:
        //     Original was GL_MAX_TEXTURE_COORDS = 0x8871
        MaxTextureCoords = 34929,
        //
        // Summary:
        //     Original was GL_MAX_TEXTURE_IMAGE_UNITS = 0x8872
        MaxTextureImageUnits = 34930,
        //
        // Summary:
        //     Original was GL_ARRAY_BUFFER_BINDING = 0x8894
        ArrayBufferBinding = 34964,
        //
        // Summary:
        //     Original was GL_ELEMENT_ARRAY_BUFFER_BINDING = 0x8895
        ElementArrayBufferBinding = 34965,
        //
        // Summary:
        //     Original was GL_VERTEX_ARRAY_BUFFER_BINDING = 0x8896
        VertexArrayBufferBinding = 34966,
        //
        // Summary:
        //     Original was GL_NORMAL_ARRAY_BUFFER_BINDING = 0x8897
        NormalArrayBufferBinding = 34967,
        //
        // Summary:
        //     Original was GL_COLOR_ARRAY_BUFFER_BINDING = 0x8898
        ColorArrayBufferBinding = 34968,
        //
        // Summary:
        //     Original was GL_INDEX_ARRAY_BUFFER_BINDING = 0x8899
        IndexArrayBufferBinding = 34969,
        //
        // Summary:
        //     Original was GL_TEXTURE_COORD_ARRAY_BUFFER_BINDING = 0x889A
        TextureCoordArrayBufferBinding = 34970,
        //
        // Summary:
        //     Original was GL_EDGE_FLAG_ARRAY_BUFFER_BINDING = 0x889B
        EdgeFlagArrayBufferBinding = 34971,
        //
        // Summary:
        //     Original was GL_SECONDARY_COLOR_ARRAY_BUFFER_BINDING = 0x889C
        SecondaryColorArrayBufferBinding = 34972,
        //
        // Summary:
        //     Original was GL_FOG_COORD_ARRAY_BUFFER_BINDING = 0x889D
        FogCoordArrayBufferBinding = 34973,
        //
        // Summary:
        //     Original was GL_WEIGHT_ARRAY_BUFFER_BINDING = 0x889E
        WeightArrayBufferBinding = 34974,
        //
        // Summary:
        //     Original was GL_VERTEX_ATTRIB_ARRAY_BUFFER_BINDING = 0x889F
        VertexAttribArrayBufferBinding = 34975,
        //
        // Summary:
        //     Original was GL_PIXEL_PACK_BUFFER_BINDING = 0x88ED
        PixelPackBufferBinding = 35053,
        //
        // Summary:
        //     Original was GL_PIXEL_UNPACK_BUFFER_BINDING = 0x88EF
        PixelUnpackBufferBinding = 35055,
        //
        // Summary:
        //     Original was GL_MAX_DUAL_SOURCE_DRAW_BUFFERS = 0x88FC
        MaxDualSourceDrawBuffers = 35068,
        //
        // Summary:
        //     Original was GL_MAX_ARRAY_TEXTURE_LAYERS = 0x88FF
        MaxArrayTextureLayers = 35071,
        //
        // Summary:
        //     Original was GL_MIN_PROGRAM_TEXEL_OFFSET = 0x8904
        MinProgramTexelOffset = 35076,
        //
        // Summary:
        //     Original was GL_MAX_PROGRAM_TEXEL_OFFSET = 0x8905
        MaxProgramTexelOffset = 35077,
        //
        // Summary:
        //     Original was GL_SAMPLER_BINDING = 0x8919
        SamplerBinding = 35097,
        //
        // Summary:
        //     Original was GL_CLAMP_VERTEX_COLOR = 0x891A
        ClampVertexColor = 35098,
        //
        // Summary:
        //     Original was GL_CLAMP_FRAGMENT_COLOR = 0x891B
        ClampFragmentColor = 35099,
        //
        // Summary:
        //     Original was GL_CLAMP_READ_COLOR = 0x891C
        ClampReadColor = 35100,
        //
        // Summary:
        //     Original was GL_MAX_VERTEX_UNIFORM_BLOCKS = 0x8A2B
        MaxVertexUniformBlocks = 35371,
        //
        // Summary:
        //     Original was GL_MAX_GEOMETRY_UNIFORM_BLOCKS = 0x8A2C
        MaxGeometryUniformBlocks = 35372,
        //
        // Summary:
        //     Original was GL_MAX_FRAGMENT_UNIFORM_BLOCKS = 0x8A2D
        MaxFragmentUniformBlocks = 35373,
        //
        // Summary:
        //     Original was GL_MAX_COMBINED_UNIFORM_BLOCKS = 0x8A2E
        MaxCombinedUniformBlocks = 35374,
        //
        // Summary:
        //     Original was GL_MAX_UNIFORM_BUFFER_BINDINGS = 0x8A2F
        MaxUniformBufferBindings = 35375,
        //
        // Summary:
        //     Original was GL_MAX_UNIFORM_BLOCK_SIZE = 0x8A30
        MaxUniformBlockSize = 35376,
        //
        // Summary:
        //     Original was GL_MAX_COMBINED_VERTEX_UNIFORM_COMPONENTS = 0x8A31
        MaxCombinedVertexUniformComponents = 35377,
        //
        // Summary:
        //     Original was GL_MAX_COMBINED_GEOMETRY_UNIFORM_COMPONENTS = 0x8A32
        MaxCombinedGeometryUniformComponents = 35378,
        //
        // Summary:
        //     Original was GL_MAX_COMBINED_FRAGMENT_UNIFORM_COMPONENTS = 0x8A33
        MaxCombinedFragmentUniformComponents = 35379,
        //
        // Summary:
        //     Original was GL_UNIFORM_BUFFER_OFFSET_ALIGNMENT = 0x8A34
        UniformBufferOffsetAlignment = 35380,
        //
        // Summary:
        //     Original was GL_MAX_FRAGMENT_UNIFORM_COMPONENTS = 0x8B49
        MaxFragmentUniformComponents = 35657,
        //
        // Summary:
        //     Original was GL_MAX_VERTEX_UNIFORM_COMPONENTS = 0x8B4A
        MaxVertexUniformComponents = 35658,
        //
        // Summary:
        //     Original was GL_MAX_VARYING_COMPONENTS = 0x8B4B
        MaxVaryingComponents = 35659,
        //
        // Summary:
        //     Original was GL_MAX_VARYING_FLOATS = 0x8B4B
        MaxVaryingFloats = 35659,
        //
        // Summary:
        //     Original was GL_MAX_VERTEX_TEXTURE_IMAGE_UNITS = 0x8B4C
        MaxVertexTextureImageUnits = 35660,
        //
        // Summary:
        //     Original was GL_MAX_COMBINED_TEXTURE_IMAGE_UNITS = 0x8B4D
        MaxCombinedTextureImageUnits = 35661,
        //
        // Summary:
        //     Original was GL_FRAGMENT_SHADER_DERIVATIVE_HINT = 0x8B8B
        FragmentShaderDerivativeHint = 35723,
        //
        // Summary:
        //     Original was GL_CURRENT_PROGRAM = 0x8B8D
        CurrentProgram = 35725,
        //
        // Summary:
        //     Original was GL_IMPLEMENTATION_COLOR_READ_TYPE = 0x8B9A
        ImplementationColorReadType = 35738,
        //
        // Summary:
        //     Original was GL_IMPLEMENTATION_COLOR_READ_FORMAT = 0x8B9B
        ImplementationColorReadFormat = 35739,
        //
        // Summary:
        //     Original was GL_TEXTURE_BINDING_1D_ARRAY = 0x8C1C
        TextureBinding1DArray = 35868,
        //
        // Summary:
        //     Original was GL_TEXTURE_BINDING_2D_ARRAY = 0x8C1D
        TextureBinding2DArray = 35869,
        //
        // Summary:
        //     Original was GL_MAX_GEOMETRY_TEXTURE_IMAGE_UNITS = 0x8C29
        MaxGeometryTextureImageUnits = 35881,
        //
        // Summary:
        //     Original was GL_TEXTURE_BUFFER = 0x8C2A
        TextureBuffer = 35882,
        //
        // Summary:
        //     Original was GL_MAX_TEXTURE_BUFFER_SIZE = 0x8C2B
        MaxTextureBufferSize = 35883,
        //
        // Summary:
        //     Original was GL_TEXTURE_BINDING_BUFFER = 0x8C2C
        TextureBindingBuffer = 35884,
        //
        // Summary:
        //     Original was GL_TEXTURE_BUFFER_DATA_STORE_BINDING = 0x8C2D
        TextureBufferDataStoreBinding = 35885,
        //
        // Summary:
        //     Original was GL_SAMPLE_SHADING = 0x8C36
        SampleShading = 35894,
        //
        // Summary:
        //     Original was GL_MIN_SAMPLE_SHADING_VALUE = 0x8C37
        MinSampleShadingValue = 35895,
        //
        // Summary:
        //     Original was GL_MAX_TRANSFORM_FEEDBACK_SEPARATE_COMPONENTS = 0x8C80
        MaxTransformFeedbackSeparateComponents = 35968,
        //
        // Summary:
        //     Original was GL_MAX_TRANSFORM_FEEDBACK_INTERLEAVED_COMPONENTS = 0x8C8A
        MaxTransformFeedbackInterleavedComponents = 35978,
        //
        // Summary:
        //     Original was GL_MAX_TRANSFORM_FEEDBACK_SEPARATE_ATTRIBS = 0x8C8B
        MaxTransformFeedbackSeparateAttribs = 35979,
        //
        // Summary:
        //     Original was GL_STENCIL_BACK_REF = 0x8CA3
        StencilBackRef = 36003,
        //
        // Summary:
        //     Original was GL_STENCIL_BACK_VALUE_MASK = 0x8CA4
        StencilBackValueMask = 36004,
        //
        // Summary:
        //     Original was GL_STENCIL_BACK_WRITEMASK = 0x8CA5
        StencilBackWritemask = 36005,
        //
        // Summary:
        //     Original was GL_DRAW_FRAMEBUFFER_BINDING = 0x8CA6
        DrawFramebufferBinding = 36006,
        //
        // Summary:
        //     Original was GL_FRAMEBUFFER_BINDING = 0x8CA6
        FramebufferBinding = 36006,
        //
        // Summary:
        //     Original was GL_FRAMEBUFFER_BINDING_EXT = 0x8CA6
        FramebufferBindingExt = 36006,
        //
        // Summary:
        //     Original was GL_RENDERBUFFER_BINDING = 0x8CA7
        RenderbufferBinding = 36007,
        //
        // Summary:
        //     Original was GL_RENDERBUFFER_BINDING_EXT = 0x8CA7
        RenderbufferBindingExt = 36007,
        //
        // Summary:
        //     Original was GL_READ_FRAMEBUFFER_BINDING = 0x8CAA
        ReadFramebufferBinding = 36010,
        //
        // Summary:
        //     Original was GL_MAX_COLOR_ATTACHMENTS = 0x8CDF
        MaxColorAttachments = 36063,
        //
        // Summary:
        //     Original was GL_MAX_COLOR_ATTACHMENTS_EXT = 0x8CDF
        MaxColorAttachmentsExt = 36063,
        //
        // Summary:
        //     Original was GL_MAX_SAMPLES = 0x8D57
        MaxSamples = 36183,
        //
        // Summary:
        //     Original was GL_FRAMEBUFFER_SRGB = 0x8DB9
        FramebufferSrgb = 36281,
        //
        // Summary:
        //     Original was GL_MAX_GEOMETRY_VARYING_COMPONENTS = 0x8DDD
        MaxGeometryVaryingComponents = 36317,
        //
        // Summary:
        //     Original was GL_MAX_VERTEX_VARYING_COMPONENTS = 0x8DDE
        MaxVertexVaryingComponents = 36318,
        //
        // Summary:
        //     Original was GL_MAX_GEOMETRY_UNIFORM_COMPONENTS = 0x8DDF
        MaxGeometryUniformComponents = 36319,
        //
        // Summary:
        //     Original was GL_MAX_GEOMETRY_OUTPUT_VERTICES = 0x8DE0
        MaxGeometryOutputVertices = 36320,
        //
        // Summary:
        //     Original was GL_MAX_GEOMETRY_TOTAL_OUTPUT_COMPONENTS = 0x8DE1
        MaxGeometryTotalOutputComponents = 36321,
        //
        // Summary:
        //     Original was GL_MAX_SUBROUTINES = 0x8DE7
        MaxSubroutines = 36327,
        //
        // Summary:
        //     Original was GL_MAX_SUBROUTINE_UNIFORM_LOCATIONS = 0x8DE8
        MaxSubroutineUniformLocations = 36328,
        //
        // Summary:
        //     Original was GL_SHADER_BINARY_FORMATS = 0x8DF8
        ShaderBinaryFormats = 36344,
        //
        // Summary:
        //     Original was GL_NUM_SHADER_BINARY_FORMATS = 0x8DF9
        NumShaderBinaryFormats = 36345,
        //
        // Summary:
        //     Original was GL_SHADER_COMPILER = 0x8DFA
        ShaderCompiler = 36346,
        //
        // Summary:
        //     Original was GL_MAX_VERTEX_UNIFORM_VECTORS = 0x8DFB
        MaxVertexUniformVectors = 36347,
        //
        // Summary:
        //     Original was GL_MAX_VARYING_VECTORS = 0x8DFC
        MaxVaryingVectors = 36348,
        //
        // Summary:
        //     Original was GL_MAX_FRAGMENT_UNIFORM_VECTORS = 0x8DFD
        MaxFragmentUniformVectors = 36349,
        //
        // Summary:
        //     Original was GL_MAX_COMBINED_TESS_CONTROL_UNIFORM_COMPONENTS = 0x8E1E
        MaxCombinedTessControlUniformComponents = 36382,
        //
        // Summary:
        //     Original was GL_MAX_COMBINED_TESS_EVALUATION_UNIFORM_COMPONENTS = 0x8E1F
        MaxCombinedTessEvaluationUniformComponents = 36383,
        //
        // Summary:
        //     Original was GL_TRANSFORM_FEEDBACK_BUFFER_PAUSED = 0x8E23
        TransformFeedbackBufferPaused = 36387,
        //
        // Summary:
        //     Original was GL_TRANSFORM_FEEDBACK_BUFFER_ACTIVE = 0x8E24
        TransformFeedbackBufferActive = 36388,
        //
        // Summary:
        //     Original was GL_TRANSFORM_FEEDBACK_BINDING = 0x8E25
        TransformFeedbackBinding = 36389,
        //
        // Summary:
        //     Original was GL_TIMESTAMP = 0x8E28
        Timestamp = 36392,
        //
        // Summary:
        //     Original was GL_QUADS_FOLLOW_PROVOKING_VERTEX_CONVENTION = 0x8E4C
        QuadsFollowProvokingVertexConvention = 36428,
        //
        // Summary:
        //     Original was GL_PROVOKING_VERTEX = 0x8E4F
        ProvokingVertex = 36431,
        //
        // Summary:
        //     Original was GL_SAMPLE_MASK = 0x8E51
        SampleMask = 36433,
        //
        // Summary:
        //     Original was GL_MAX_SAMPLE_MASK_WORDS = 0x8E59
        MaxSampleMaskWords = 36441,
        //
        // Summary:
        //     Original was GL_MAX_GEOMETRY_SHADER_INVOCATIONS = 0x8E5A
        MaxGeometryShaderInvocations = 36442,
        //
        // Summary:
        //     Original was GL_MIN_FRAGMENT_INTERPOLATION_OFFSET = 0x8E5B
        MinFragmentInterpolationOffset = 36443,
        //
        // Summary:
        //     Original was GL_MAX_FRAGMENT_INTERPOLATION_OFFSET = 0x8E5C
        MaxFragmentInterpolationOffset = 36444,
        //
        // Summary:
        //     Original was GL_FRAGMENT_INTERPOLATION_OFFSET_BITS = 0x8E5D
        FragmentInterpolationOffsetBits = 36445,
        //
        // Summary:
        //     Original was GL_MIN_PROGRAM_TEXTURE_GATHER_OFFSET = 0x8E5E
        MinProgramTextureGatherOffset = 36446,
        //
        // Summary:
        //     Original was GL_MAX_PROGRAM_TEXTURE_GATHER_OFFSET = 0x8E5F
        MaxProgramTextureGatherOffset = 36447,
        //
        // Summary:
        //     Original was GL_MAX_TRANSFORM_FEEDBACK_BUFFERS = 0x8E70
        MaxTransformFeedbackBuffers = 36464,
        //
        // Summary:
        //     Original was GL_MAX_VERTEX_STREAMS = 0x8E71
        MaxVertexStreams = 36465,
        //
        // Summary:
        //     Original was GL_PATCH_VERTICES = 0x8E72
        PatchVertices = 36466,
        //
        // Summary:
        //     Original was GL_PATCH_DEFAULT_INNER_LEVEL = 0x8E73
        PatchDefaultInnerLevel = 36467,
        //
        // Summary:
        //     Original was GL_PATCH_DEFAULT_OUTER_LEVEL = 0x8E74
        PatchDefaultOuterLevel = 36468,
        //
        // Summary:
        //     Original was GL_MAX_TESS_GEN_LEVEL = 0x8E7E
        MaxTessGenLevel = 36478,
        //
        // Summary:
        //     Original was GL_MAX_TESS_CONTROL_UNIFORM_COMPONENTS = 0x8E7F
        MaxTessControlUniformComponents = 36479,
        //
        // Summary:
        //     Original was GL_MAX_TESS_EVALUATION_UNIFORM_COMPONENTS = 0x8E80
        MaxTessEvaluationUniformComponents = 36480,
        //
        // Summary:
        //     Original was GL_MAX_TESS_CONTROL_TEXTURE_IMAGE_UNITS = 0x8E81
        MaxTessControlTextureImageUnits = 36481,
        //
        // Summary:
        //     Original was GL_MAX_TESS_EVALUATION_TEXTURE_IMAGE_UNITS = 0x8E82
        MaxTessEvaluationTextureImageUnits = 36482,
        //
        // Summary:
        //     Original was GL_MAX_TESS_CONTROL_OUTPUT_COMPONENTS = 0x8E83
        MaxTessControlOutputComponents = 36483,
        //
        // Summary:
        //     Original was GL_MAX_TESS_PATCH_COMPONENTS = 0x8E84
        MaxTessPatchComponents = 36484,
        //
        // Summary:
        //     Original was GL_MAX_TESS_CONTROL_TOTAL_OUTPUT_COMPONENTS = 0x8E85
        MaxTessControlTotalOutputComponents = 36485,
        //
        // Summary:
        //     Original was GL_MAX_TESS_EVALUATION_OUTPUT_COMPONENTS = 0x8E86
        MaxTessEvaluationOutputComponents = 36486,
        //
        // Summary:
        //     Original was GL_MAX_TESS_CONTROL_UNIFORM_BLOCKS = 0x8E89
        MaxTessControlUniformBlocks = 36489,
        //
        // Summary:
        //     Original was GL_MAX_TESS_EVALUATION_UNIFORM_BLOCKS = 0x8E8A
        MaxTessEvaluationUniformBlocks = 36490,
        //
        // Summary:
        //     Original was GL_DRAW_INDIRECT_BUFFER_BINDING = 0x8F43
        DrawIndirectBufferBinding = 36675,
        //
        // Summary:
        //     Original was GL_TEXTURE_BINDING_2D_MULTISAMPLE = 0x9104
        TextureBinding2DMultisample = 37124,
        //
        // Summary:
        //     Original was GL_TEXTURE_BINDING_2D_MULTISAMPLE_ARRAY = 0x9105
        TextureBinding2DMultisampleArray = 37125,
        //
        // Summary:
        //     Original was GL_MAX_COLOR_TEXTURE_SAMPLES = 0x910E
        MaxColorTextureSamples = 37134,
        //
        // Summary:
        //     Original was GL_MAX_DEPTH_TEXTURE_SAMPLES = 0x910F
        MaxDepthTextureSamples = 37135,
        //
        // Summary:
        //     Original was GL_MAX_INTEGER_SAMPLES = 0x9110
        MaxIntegerSamples = 37136,
        //
        // Summary:
        //     Original was GL_MAX_VERTEX_OUTPUT_COMPONENTS = 0x9122
        MaxVertexOutputComponents = 37154,
        //
        // Summary:
        //     Original was GL_MAX_GEOMETRY_INPUT_COMPONENTS = 0x9123
        MaxGeometryInputComponents = 37155,
        //
        // Summary:
        //     Original was GL_MAX_GEOMETRY_OUTPUT_COMPONENTS = 0x9124
        MaxGeometryOutputComponents = 37156,
        //
        // Summary:
        //     Original was GL_MAX_FRAGMENT_INPUT_COMPONENTS = 0x9125
        MaxFragmentInputComponents = 37157,
    }
    [Flags]
    public enum MaterialFace
    {
        // Summary:
        //     Original was GL_FRONT = 0x0404
        Front = 1028,
        //
        // Summary:
        //     Original was GL_BACK = 0x0405
        Back = 1029,
        //
        // Summary:
        //     Original was GL_FRONT_AND_BACK = 0x0408
        FrontAndBack = 1032,
    }
    [Flags]
    public enum MatrixMode
    {
        // Summary:
        //     Original was GL_MODELVIEW = 0x1700
        Modelview = 5888,
        //
        // Summary:
        //     Original was GL_MODELVIEW0_EXT = 0x1700
        Modelview0Ext = 5888,
        //
        // Summary:
        //     Original was GL_PROJECTION = 0x1701
        Projection = 5889,
        //
        // Summary:
        //     Original was GL_TEXTURE = 0x1702
        Texture = 5890,
        //
        // Summary:
        //     Original was GL_COLOR = 0x1800
        Color = 6144,
    }
    [Flags]
    public enum PolygonMode
    {
        // Summary:
        //     Original was GL_POINT = 0x1B00
        Point = 6912,
        //
        // Summary:
        //     Original was GL_LINE = 0x1B01
        Line = 6913,
        //
        // Summary:
        //     Original was GL_FILL = 0x1B02
        Fill = 6914,
    }
    [Flags]
    public enum ShadingModel
    {
        // Summary:
        //     Original was GL_FLAT = 0x1D00
        Flat = 7424,
        //
        // Summary:
        //     Original was GL_SMOOTH = 0x1D01
        Smooth = 7425,
    }
    [Flags]
    public enum VertexAttribPointerType
    {
        Byte = 5120,
        UnsignedByte = 5121,
        Short = 5122,
        UnsignedShort = 5123,
        Int = 5124,
        UnsignedInt = 5125,
        Float = 5126,
        Double = 5130,
        HalfFloat = 5131,
        Fixed = 5132,
        UnsignedInt2101010Rev = 33640,
        Int2101010Rev = 36255,
    }  
    #endregion

    #endregion

    [Serializable]  // Required by CodeDom
    public abstract partial class RendererBase : IRenderDevice
    {
        public delegate void LogApplication(string msg);
        public delegate void LogErrorApplication(string msg, ErrorSeverity err);       // TODO:: Globalize ErrorSeverity from NthApplication

        public void RaiseApplicationLog(string msg)
        {
            SendLogToApplication?.Invoke(msg);
        }
        public void RaiseApplicationError(string msg, ErrorSeverity err)
        {
            // TODO:: Globalize ErrorSeverity from NthApplication
            SendErrorLogToApplication?.Invoke(msg, err);
        }

        public event LogApplication SendLogToApplication;
        public event LogErrorApplication SendErrorLogToApplication;

        //public enuGraphicsMode                  GraphicsMode; // Commented for debug, remove next
        //public enuGraphicsMode                  GraphicsMode = enuGraphicsMode.Lists;

        //// To be replaced by camera
        //public Vector3                              worldRotation = new Vector3();
        //public Vector3                              cameraPosition = new Vector3(0, 0, 0);

        //private Vector3                             m_position      = new Vector3();
        //private Vector3                             m_rotation      = new Vector3();
        //private Vector3                             m_scale         = new Vector3(1f, 1f, 1f);

        public double                           Time { get; set; }

        public CameraBase                           Camera;

        protected static bool                       _isRendering = false;

        public RendererBase(/*enuGraphicsMode graphicsMode*/)
        {
            
        }

        //#region Device
        public abstract string                  GraphicsApi { get; }
        public abstract string                  GraphicsApiVersion { get; }

        public abstract IntPtr                  DeviceContextHandle { get; }
        public abstract IntPtr                  RenderContextHandle { get; }

        public abstract Color                   ClearColor { get; set; }

        #region Glu

//public abstract IntPtr CreateQuadric();

#endregion

//[Obsolete("Use RendererBase.Shading instead")]
//public abstract BasicShader             CreateBasicShader();


//public abstract Texture2D               CreateTexture(int Width, int Height);
//public abstract Texture2D               CreateTextureFromBitmap(System.Drawing.Bitmap bitmap);
//public abstract void                      CreateVertexBuffer(SYSCON.Graphics.Modelling.Model model);                     // TODO:: Temporary, remove-replace

#if HWINPUT
        public abstract Keyboard                DefaultKeyboard
        {
            get;
        }
        //public abstract Mouse                   DefaultMouse { get; }
        public abstract Touchpad DefaultTouchpad
        {
            get;
        }
        
#endif
        public abstract bool                    ErrorLevel(string contains);
      

        public abstract void                    Dispose();
        [Obsolete]
        public abstract void StartFrameBuffer(uint fboId, float width, float height);                       // Experimental, never worked
        [Obsolete]
        public abstract void EndFrameBuffer(uint fboId, float screenWidth, float screenHeight);             // Experimental, never worked

        public abstract void                    BeginFixedPipeline2D();
        public abstract void                    EndFixedPipeline2D();

        public abstract void                    BeginFixedPipelineOrtho(Rectangle clientRectangle);
        public abstract void                    EndFixedPipelineOrtho();

        public virtual void                     BeginFixedPipeline3D(enuPolygonMode beginMode)
        {
            //foreach (IRayPicker rayPicker in rayPickerManager)
            //{
            //    RayPickerManager.InfoByRayPicker info = rayPickerManager[rayPicker];
            //    info.Clear();
            //}
        }
        public abstract void                    EndFixedPipeline3D();

        public virtual void BeginAttributes()
        {
        }

        public abstract void EndAttributes();

        public abstract void EnableVertexAttribArray(int attrib);
        public abstract void DisableVertexAttribArray(int attrib);

        public abstract void                    BlendEquation(BlendEquationMode mode);
        public abstract void                    BlendFunc(BlendingFactorSrc src, BlendingFactorDest dest);
        public abstract void                    Clear(ClearBufferMask mask);
        public abstract void                    ClearDepth(double depth);                                                        // Added Junly-02-15
        public abstract void                    CullMode(enuCullFaceMode mode);

        public abstract bool                    Blend { get; set; }
        public abstract bool                    CullFace { get; set; }
        public abstract bool                    DepthTest { get; set; }
        public abstract bool                    Lighting { get; set; }
        public abstract float                   LineWidth { get; set; }
        public abstract bool                    Texture2D { get; set; }

        public abstract void                    Enable(EnableCap cap);
        public abstract void                    Disable(EnableCap cap);

        public abstract void                    Color3(float r, float g, float b);
        public abstract void                    Color4(Color color);
        public abstract void                    Color4(Color4 color);
        public abstract void                    Color4(float r, float g, float b, float a);

        public abstract void                    PointSize(float pointSize = 1.0f);
        //public abstract void                    LineSize(float lineSize = .0f);

        
        public abstract void                    HighQualityLines(bool blending = false, bool depthMask = true);
        public abstract void                    HighQualityModels(bool blending = true);


        public abstract void                    Hint(HintTarget target, HintMode mode);

        public abstract void                    TexCoord2(float s, float t);

        public abstract void                    Vertex3(float x, float y, float z);

        protected void                          DrawRenderCommands()
        {
            lock (RenderCommand.commandlist)
            {
                foreach (RenderCommand command in RenderCommand.commandlist)
                {
                    command.Draw();
                }
                lock (RenderCommand.removeList)
                {
                    foreach (RenderCommand et in RenderCommand.removeList)
                    {
                        RenderCommand.commandlist.Remove(et);
                    }
                }
                RenderCommand.removeList.Clear();
            }
        }

        public abstract void                    Draw(SceneRoot sceneRoot);
        public abstract void                    DrawTrueTypeFont2D(int fontIndex, Vector3 color, string text, float top, float left, float scaleWidth, float scaleHeight, int format);
        //public abstract void                    DrawText(SYSCON.Graphics.UserInterface.Font font, Point position, string text);
        public abstract void                    DrawAABB(BoundingVolume volume);
        public abstract void                    DrawAngle(Vector3 orig, Vector3 vtx, Vector3 vty, float angle);
        public abstract void                    DrawAxis(Vector3 orig, Vector3 axis, Vector3 vtx, Vector3 vty, float fct, float fct2, Vector4 color);
        public abstract void                    DrawCircle(Vector3 orig, float r, float g, float b, Vector3 vtx, Vector3 vty);
        public abstract void                    DrawCircleX(float size, float lineWidth, Color unColor);
        public abstract void                    DrawCircleX(float size, float lineWidth, Color unColor, float posx = 0f, float posy = 0f, float posz = 0f);
        public abstract void                    DrawCircleY(float size, float lineWidth, Color unColor);
        public abstract void                    DrawCircleY(float size, float lineWidth, Color unColor, float posx = 0f, float posy = 0f, float posz = 0f);
        public abstract void                    DrawCircleZ(float size, float lineWidth, Color unColor);
        public abstract void                    DrawCircleZ(float size, float lineWidth, Color unColor, float posx = 0f, float posy = 0f, float posz = 0f);
        public abstract void                    DrawCircleHalf(Vector3 orig, float r, float g, float b, Vector3 vtx, Vector3 vty, ref Vector4 camPlan);
        public abstract void                    DrawConeX(float width, float height, float offset);
        public abstract void                    DrawConeX(float width, float height, float offset, ref BoundingAABB aabb);
        public abstract void                    DrawConeY(float width, float height, float offset);
        public abstract void                    DrawConeY(float width, float height, float offset, ref BoundingAABB aabb);
        public abstract void                    DrawConeZ(float width, float height, float offset);
        public abstract void                    DrawConeZ(float width, float height, float offset, ref BoundingAABB aabb);
        public abstract void                    DrawCube(float size, float posX, float posY, float posZ, Color unColor);
        public abstract void                    DrawFace(uint[] indices, List<Vertex> vertices, Vector3 normal, Color unColor, int poliId, uint obj3dId, bool picking);
        public abstract void                    DrawFace(Dictionary<uint, VertexIndex> indices, List<Vertex> vertices, Vector3 normal, Color unColor, int poliId, uint obj3dId, bool picking);
        public abstract void                    DrawFace(List<uint> indices, List<Vertex> vertices, Vector3 normal, Color unColor, int poliId, uint obj3dId, bool picking);
        public abstract void                    DrawFace(List<VertexIndex> indices, List<Vertex> vertices, Vector3 normal, Color unColor, int poliId, uint obj3dId, bool picking, enuPolygonMode beginMode = enuPolygonMode.Polygon);
        public abstract void                    DrawLine(float v1x, float v1y, float v1z, float v2x, float v2y, float v2z, Color unColor, bool blend = true, float lineSize = 1.0f);
        public abstract void                    DrawLine(float v1x, float v1y, float v1z, float v2x, float v2y, float v2z, Color unColor, int lineId, uint obj3dId, float lineSize = 1.0f);
        public abstract void                    DrawLine(Vertex v1, Vertex v2, Color unColor, int lineId, uint obj3dId, float lineSize = 1.0f);
        /// <summary>
        /// Draws a lined rectangle. Used for keyboard focus overlay.
        /// </summary>
        /// <param name="rect">Target rectangle.</param>
        public virtual void                     DrawLinedRect(Rectangle rect, Color color)
        {
            DrawFilledRect(new Rectangle(rect.X, rect.Y, rect.Width, 1), color);
            DrawFilledRect(new Rectangle(rect.X, rect.Y + rect.Height - 1, rect.Width, 1), color);

            DrawFilledRect(new Rectangle(rect.X, rect.Y, 1, rect.Height), color);
            DrawFilledRect(new Rectangle(rect.X + rect.Width - 1, rect.Y, 1, rect.Height), color);
        }
        public abstract void                    DrawQuad(Vector3 orig, float size, bool bSelected, Vector3 axisU, Vector3 axisV);
        public abstract void                    DrawQuadXZ(float size, float sizeOffset, Color unColor);
        public abstract void                    DrawQuadZY(float size, float sizeOffset, Color unColor);
        public abstract void                    DrawQuadYX(float size, float sizeOffset, Color unColor);
        public abstract void                    DrawRect(Rectangle rect, Color drawColor, float u1 = 0, float v1 = 0, float u2 = 1, float v2 = 1);
        public abstract void                    DrawFilledRect(Rectangle rect, Color rectColor);
        public abstract void                    DrawSelectionSquare(float x1, float y1, float x2, float y2, Color unColor, Rectangle clientRect);
        public abstract void                    DrawSphere(float radius, int lats, int longs, Color unColor);
        public abstract void                    DrawTriangle(Vector3 orig, float size, bool bSelected, Vector3 axisU, Vector3 axisV);
        public abstract void                    DrawVertex(Vertex v, Color color, uint objId, float pointSize = 1f);
        public abstract void                    DrawVertex(Vertex[] v, Color color, enuPolygonMode mode = enuPolygonMode.Points, bool smooth = true, float pointSize = 1f);
        public abstract void                    DrawVertices(Vector3[] v, Color4[] colors, enuPolygonMode mode = enuPolygonMode.LineLoop /*, bool smooth = true, float lineSize = 1f*/);

        public abstract void                    DepthFunc(DepthFunction depth);
        public abstract void                    DepthMask(bool mask);
        public abstract unsafe void             Flush();

        public abstract void                    GetFloat(GetPName pname, float[] data);
        public abstract void                    GetFloat(GetPName pname, out Matrix4 m);
        public abstract int[]                   GetInteger(GetPName pname, int[] data);                 // ref directive
        public abstract void                    GluOrtho2D(double left, double right, double bottom, double top);

        //public abstract void                    InsertVertex(ref float f);
        public abstract void                    InsertVertex(Vector3 vertex);
        public abstract void                    InsertVertex(float x, float y, float z);
        public abstract void                    InsertNormal(float x, float y, float z);

        public abstract void                    LoadIdentity();
        public abstract void                    LoadMatrix(ref Matrix4 matrix);

        public abstract void                    MatrixMode(MatrixMode mode);
        public abstract void                    MultMatrix(float[] matrix);
        public abstract void                    MultMatrix(ref Matrix4 matrix);
        public abstract void                    MultMatrix(ref Matrix4d matrix);

        public abstract void                    PushAttrib(AttribMask mask);
        public abstract void                    PopAttrib();

        public abstract void                    PushMatrix();
        public abstract void                    PopMatrix();

        public abstract void                    PushFaceColor(Face face);
        public abstract void                    PopFaceColor(Face face);

        public abstract Vector4                 Project(Vector4 objPos, Matrix4 projection, Matrix4 view, Size viewport);

        public abstract void                    PolygonOffset(float factor, float units);
        public abstract bool                    PolygonOffsetFill { get; set; }
        public abstract void                    PolygonMode(MaterialFace face, PolygonMode mode);

        public abstract void                    Ortho(double left, double right, double bottom, double top, double zNear, double zFar);
        public abstract void                    Translate(double x, double y, double z);
        public abstract void                    Rotate(float degrees, Vector3 axis);

        public abstract void                    SetRenderTarget(Texture2D texture, Vector3 campos, Vector3 camrot);
        public abstract void                    SetShadingMode(ShadingModel shading);

        public abstract Vector4                 UnProject(Matrix4 projection, Matrix4 view, Size viewport, Vector2 mouse);
        public abstract Vector4                 UnProject(Matrix4d projection, Matrix4d view, Size viewport, Vector2 mouse);
        //public abstract void                    Sfcale(float x, float y, float z);
        public abstract void                    Viewport(int x, int y, int width, int height);

        [Obsolete("Use the Point function instead, is an exact copy of glu.dll")]
        public abstract Vector2                 WorldToScreen0(Vector3 world);
        public abstract Point                   WorldToScreen(Vector3 world);
        public abstract Point WorldToScreen(Vector3 world, Matrix4 modelview);

        public abstract string GetString(StringName sName);
        public abstract ErrorCode GetError(bool raiseUIevent = false);

        public abstract void LightModel(LightModelParameter pname, int param);
        public abstract void ShadeModel(ShadingModel model);

        public abstract void ActiveTexture(TextureUnit textureUnit);
        public abstract void ClientActiveTexture(TextureUnit textureUnit);
        public abstract void BindTexture(TextureTarget textureTarget, int textureId);
        public abstract void GenTextures(int n, out int textures);
        public abstract void DeleteTexture(int textureId);
        //public abstract void BindToTexture(TextureUnit textureUnit, int textureId);

        public abstract void EnableClientState(ArrayCap cap);
        public abstract void DisableClientState(ArrayCap cap);

        public abstract void ColorPointer(int size, ColorPointerType type, int stride, int offset);
        public abstract void NormalPointer(NormalPointerType type, int stride, int offset);
        public abstract void VertexPointer(int size, VertexPointerType type, int stride, int offset);
        public abstract void TexCoordPointer(int size, TexCoordPointerType type, int stride, int offset);

        public abstract void TexEnv(TextureEnvTarget texEnvTarget, TextureEnvParameter texEnvParam, int mode);

        public abstract void loadTextureFromBitmapData(int glTextureId,
                                                       Bitmap TextureBitmap,
                                                       string name = null,
                                                       bool hasAlpha = false,
                                                       bool mipmap = true);
        public abstract int loadTextureFromFile(string filename/*, int filterNear, int filterFar*/);


#region -- Frame Buffers

        public abstract bool FBO_Supported();

        public abstract void FBO_Create(TextureUnit texUnit, int textureWidth, int textureHeight, out int textureId, out int framebufferId, out TextureUnit textureUnit);

        public abstract void FBO_PrepareForRead(TextureUnit textureUnit, int textureId);

        public abstract void FBO_FinishRead(TextureUnit textureUnit);

        public abstract void FBO_PrepareForRender(int framebufferId, int textureId, int textureWidth, int textureHeight);

        public abstract bool FBO_AssertOK(FramebufferTarget target, int textureId, int framebufferId);

        public abstract void FBO_DeleteData(int textureId, int framebufferId);

        public abstract void FBO_Unbind();

#endregion

#region SSM Lighting (Functions supporting code to match SimpleSceneRenderer (April, 11 2015)

        public abstract void Light(LightName name, LightParameter pname, Vector4 lparam);

        public abstract void Light_Disable(int idx);

        public abstract void Light_Setup(ref Matrix4 modelView,
                                        LightName lightName,
                                        LightName firstName,
                                        Vector4 Ambient,
                                        Vector4 Diffuse,
                                        Vector4 Specular);

#endregion

#region -- Business logic

        //public abstract void                    ClearPointsBuffer();

#endregion


        

    }
}
