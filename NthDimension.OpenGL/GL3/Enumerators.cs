/* LICENSE
 * Copyright (C) 2008 - 2018 SYSCON Technologies, Hellas - All Rights Reserved
 * Written by Stylianos N. Polychroniadis (info@polytronic.gr) http://www.polytronic.gr
 * 
 * This file is part of nthDimension Platform
 * 
 * WARNING! Commercial Software, All Use Must Be Licensed
 * This software is protected by Hellenic Copyright Law and International Treaties. 
 * Unauthorized use, duplication, reverse engineering, any form of redistribution, or 
 * use in part or in whole other than by prior, express, printed and signed license 
 * for use is subject to civil and criminal prosecution. 
*/

using System;

namespace NthDimension.Rasterizer
{
    #region Buffer
    public enum BufferParameterName
    {
        BufferImmutableStorage = 33311,
        BufferStorageFlags = 33312,
        BufferSize = 34660,
        BufferUsage = 34661,
        BufferAccess = 35003,
        BufferMapped = 35004,
        BufferAccessFlags = 37151,
        BufferMapLength = 37152,
        BufferMapOffset = 37153
    }

    public enum BufferTarget : int
    {
        /// <summary>
        /// Original was GL_ARRAY_BUFFER = 0x8892
        /// </summary>
        ArrayBuffer = ((int)0x8892),
        /// <summary>
        /// Original was GL_ELEMENT_ARRAY_BUFFER = 0x8893
        /// </summary>
        ElementArrayBuffer = ((int)0x8893),
        /// <summary>
        /// Original was GL_PIXEL_PACK_BUFFER = 0x88EB
        /// </summary>
        PixelPackBuffer = ((int)0x88EB),
        /// <summary>
        /// Original was GL_PIXEL_UNPACK_BUFFER = 0x88EC
        /// </summary>
        PixelUnpackBuffer = ((int)0x88EC),
        /// <summary>
        /// Original was GL_UNIFORM_BUFFER = 0x8A11
        /// </summary>
        UniformBuffer = ((int)0x8A11),
        /// <summary>
        /// Original was GL_TEXTURE_BUFFER = 0x8C2A
        /// </summary>
        TextureBuffer = ((int)0x8C2A),
        /// <summary>
        /// Original was GL_TRANSFORM_FEEDBACK_BUFFER = 0x8C8E
        /// </summary>
        TransformFeedbackBuffer = ((int)0x8C8E),
        /// <summary>
        /// Original was GL_COPY_READ_BUFFER = 0x8F36
        /// </summary>
        CopyReadBuffer = ((int)0x8F36),
        /// <summary>
        /// Original was GL_COPY_WRITE_BUFFER = 0x8F37
        /// </summary>
        CopyWriteBuffer = ((int)0x8F37),
        /// <summary>
        /// Original was GL_DRAW_INDIRECT_BUFFER = 0x8F3F
        /// </summary>
        DrawIndirectBuffer = ((int)0x8F3F),
        /// <summary>
        /// Original was GL_SHADER_STORAGE_BUFFER = 0x90D2
        /// </summary>
        ShaderStorageBuffer = ((int)0x90D2),
        /// <summary>
        /// Original was GL_DISPATCH_INDIRECT_BUFFER = 0x90EE
        /// </summary>
        DispatchIndirectBuffer = ((int)0x90EE),
        /// <summary>
        /// Original was GL_QUERY_BUFFER = 0x9192
        /// </summary>
        QueryBuffer = ((int)0x9192),
        /// <summary>
        /// Original was GL_ATOMIC_COUNTER_BUFFER = 0x92C0
        /// </summary>
        AtomicCounterBuffer = ((int)0x92C0),
    }

    public enum BufferUsageHint : int
    {
        /// <summary>
        /// Original was GL_STREAM_DRAW = 0x88E0
        /// </summary>
        StreamDraw = ((int)0x88E0),
        /// <summary>
        /// Original was GL_STREAM_READ = 0x88E1
        /// </summary>
        StreamRead = ((int)0x88E1),
        /// <summary>
        /// Original was GL_STREAM_COPY = 0x88E2
        /// </summary>
        StreamCopy = ((int)0x88E2),
        /// <summary>
        /// Original was GL_STATIC_DRAW = 0x88E4
        /// </summary>
        StaticDraw = ((int)0x88E4),
        /// <summary>
        /// Original was GL_STATIC_READ = 0x88E5
        /// </summary>
        StaticRead = ((int)0x88E5),
        /// <summary>
        /// Original was GL_STATIC_COPY = 0x88E6
        /// </summary>
        StaticCopy = ((int)0x88E6),
        /// <summary>
        /// Original was GL_DYNAMIC_DRAW = 0x88E8
        /// </summary>
        DynamicDraw = ((int)0x88E8),
        /// <summary>
        /// Original was GL_DYNAMIC_READ = 0x88E9
        /// </summary>
        DynamicRead = ((int)0x88E9),
        /// <summary>
        /// Original was GL_DYNAMIC_COPY = 0x88EA
        /// </summary>
        DynamicCopy = ((int)0x88EA),
    }

    public enum VertexAttribPointerType : int
    {
        /// <summary>
        /// Original was GL_BYTE = 0x1400
        /// </summary>
        Byte = ((int)0x1400),
        /// <summary>
        /// Original was GL_UNSIGNED_BYTE = 0x1401
        /// </summary>
        UnsignedByte = ((int)0x1401),
        /// <summary>
        /// Original was GL_SHORT = 0x1402
        /// </summary>
        Short = ((int)0x1402),
        /// <summary>
        /// Original was GL_UNSIGNED_SHORT = 0x1403
        /// </summary>
        UnsignedShort = ((int)0x1403),
        /// <summary>
        /// Original was GL_INT = 0x1404
        /// </summary>
        Int = ((int)0x1404),
        /// <summary>
        /// Original was GL_UNSIGNED_INT = 0x1405
        /// </summary>
        UnsignedInt = ((int)0x1405),
        /// <summary>
        /// Original was GL_FLOAT = 0x1406
        /// </summary>
        Float = ((int)0x1406),
        /// <summary>
        /// Original was GL_DOUBLE = 0x140A
        /// </summary>
        Double = ((int)0x140A),
        /// <summary>
        /// Original was GL_HALF_FLOAT = 0x140B
        /// </summary>
        HalfFloat = ((int)0x140B),
        /// <summary>
        /// Original was GL_FIXED = 0x140C
        /// </summary>
        Fixed = ((int)0x140C),
        /// <summary>
        /// Original was GL_UNSIGNED_INT_2_10_10_10_REV = 0x8368
        /// </summary>
        UnsignedInt2101010Rev = ((int)0x8368),
        /// <summary>
        /// Original was GL_INT_2_10_10_10_REV = 0x8D9F
        /// </summary>
        Int2101010Rev = ((int)0x8D9F),
    }
    #endregion

    #region Element
    public enum BeginMode : int
    {
        /// <summary>
        /// Original was GL_POINTS = 0x0000
        /// </summary>
        Points = ((int)0x0000),
        /// <summary>
        /// Original was GL_LINES = 0x0001
        /// </summary>
        Lines = ((int)0x0001),
        /// <summary>
        /// Original was GL_LINE_LOOP = 0x0002
        /// </summary>
        LineLoop = ((int)0x0002),
        /// <summary>
        /// Original was GL_LINE_STRIP = 0x0003
        /// </summary>
        LineStrip = ((int)0x0003),
        /// <summary>
        /// Original was GL_TRIANGLES = 0x0004
        /// </summary>
        Triangles = ((int)0x0004),
        /// <summary>
        /// Original was GL_TRIANGLE_STRIP = 0x0005
        /// </summary>
        TriangleStrip = ((int)0x0005),
        /// <summary>
        /// Original was GL_TRIANGLE_FAN = 0x0006
        /// </summary>
        TriangleFan = ((int)0x0006),
        /// <summary>
        /// Original was GL_QUADS = 0x0007
        /// </summary>
        Quads = ((int)0x0007),
        /// <summary>
        /// Original was GL_QUAD_STRIP = 0x0008
        /// </summary>
        QuadStrip = ((int)0x0008),
        /// <summary>
        /// Original was GL_POLYGON = 0x0009
        /// </summary>
        Polygon = ((int)0x0009),
        /// <summary>
        /// Original was GL_PATCHES = 0x000E
        /// </summary>
        Patches = ((int)0x000E),
        /// <summary>
        /// Original was GL_LINES_ADJACENCY = 0xA
        /// </summary>
        LinesAdjacency = ((int)0xA),
        /// <summary>
        /// Original was GL_LINE_STRIP_ADJACENCY = 0xB
        /// </summary>
        LineStripAdjacency = ((int)0xB),
        /// <summary>
        /// Original was GL_TRIANGLES_ADJACENCY = 0xC
        /// </summary>
        TrianglesAdjacency = ((int)0xC),
        /// <summary>
        /// Original was GL_TRIANGLE_STRIP_ADJACENCY = 0xD
        /// </summary>
        TriangleStripAdjacency = ((int)0xD),
    }

    public enum DrawElementsType : int
    {
        /// <summary>
        /// Original was GL_UNSIGNED_BYTE = 0x1401
        /// </summary>
        UnsignedByte = ((int)0x1401),
        /// <summary>
        /// Original was GL_UNSIGNED_SHORT = 0x1403
        /// </summary>
        UnsignedShort = ((int)0x1403),
        /// <summary>
        /// Original was GL_UNSIGNED_INT = 0x1405
        /// </summary>
        UnsignedInt = ((int)0x1405),
    }

    public enum PrimitiveType
    {
        Points = 0,
        Lines = 1,
        LineLoop = 2,
        LineStrip = 3,
        Triangles = 4,
        TriangleStrip = 5,
        TriangleFan = 6,
        Quads = 7,
        QuadsExt = 7,
        QuadStrip = 8,
        Polygon = 9,
        LinesAdjacency = 10,
        LinesAdjacencyArb = 10,
        LinesAdjacencyExt = 10,
        LineStripAdjacency = 11,
        LineStripAdjacencyArb = 11,
        LineStripAdjacencyExt = 11,
        TrianglesAdjacency = 12,
        TrianglesAdjacencyArb = 12,
        TrianglesAdjacencyExt = 12,
        TriangleStripAdjacency = 13,
        TriangleStripAdjacencyArb = 13,
        TriangleStripAdjacencyExt = 13,
        Patches = 14,
        PatchesExt = 14
    }
    #endregion

    #region Framebuffer
    public enum FramebufferErrorCode : int
    {
        /// <summary>
        /// Original was GL_FRAMEBUFFER_UNDEFINED = 0x8219
        /// </summary>
        FramebufferUndefined = ((int)0x8219),
        /// <summary>
        /// Original was GL_FRAMEBUFFER_COMPLETE = 0x8CD5
        /// </summary>
        FramebufferComplete = ((int)0x8CD5),
        /// <summary>
        /// Original was GL_FRAMEBUFFER_COMPLETE_EXT = 0x8CD5
        /// </summary>
        FramebufferCompleteExt = ((int)0x8CD5),
        /// <summary>
        /// Original was GL_FRAMEBUFFER_INCOMPLETE_ATTACHMENT = 0x8CD6
        /// </summary>
        FramebufferIncompleteAttachment = ((int)0x8CD6),
        /// <summary>
        /// Original was GL_FRAMEBUFFER_INCOMPLETE_ATTACHMENT_EXT = 0x8CD6
        /// </summary>
        FramebufferIncompleteAttachmentExt = ((int)0x8CD6),
        /// <summary>
        /// Original was GL_FRAMEBUFFER_INCOMPLETE_MISSING_ATTACHMENT = 0x8CD7
        /// </summary>
        FramebufferIncompleteMissingAttachment = ((int)0x8CD7),
        /// <summary>
        /// Original was GL_FRAMEBUFFER_INCOMPLETE_MISSING_ATTACHMENT_EXT = 0x8CD7
        /// </summary>
        FramebufferIncompleteMissingAttachmentExt = ((int)0x8CD7),
        /// <summary>
        /// Original was GL_FRAMEBUFFER_INCOMPLETE_DIMENSIONS_EXT = 0x8CD9
        /// </summary>
        FramebufferIncompleteDimensionsExt = ((int)0x8CD9),
        /// <summary>
        /// Original was GL_FRAMEBUFFER_INCOMPLETE_FORMATS_EXT = 0x8CDA
        /// </summary>
        FramebufferIncompleteFormatsExt = ((int)0x8CDA),
        /// <summary>
        /// Original was GL_FRAMEBUFFER_INCOMPLETE_DRAW_BUFFER = 0x8CDB
        /// </summary>
        FramebufferIncompleteDrawBuffer = ((int)0x8CDB),
        /// <summary>
        /// Original was GL_FRAMEBUFFER_INCOMPLETE_DRAW_BUFFER_EXT = 0x8CDB
        /// </summary>
        FramebufferIncompleteDrawBufferExt = ((int)0x8CDB),
        /// <summary>
        /// Original was GL_FRAMEBUFFER_INCOMPLETE_READ_BUFFER = 0x8CDC
        /// </summary>
        FramebufferIncompleteReadBuffer = ((int)0x8CDC),
        /// <summary>
        /// Original was GL_FRAMEBUFFER_INCOMPLETE_READ_BUFFER_EXT = 0x8CDC
        /// </summary>
        FramebufferIncompleteReadBufferExt = ((int)0x8CDC),
        /// <summary>
        /// Original was GL_FRAMEBUFFER_UNSUPPORTED = 0x8CDD
        /// </summary>
        FramebufferUnsupported = ((int)0x8CDD),
        /// <summary>
        /// Original was GL_FRAMEBUFFER_UNSUPPORTED_EXT = 0x8CDD
        /// </summary>
        FramebufferUnsupportedExt = ((int)0x8CDD),
        /// <summary>
        /// Original was GL_FRAMEBUFFER_INCOMPLETE_MULTISAMPLE = 0x8D56
        /// </summary>
        FramebufferIncompleteMultisample = ((int)0x8D56),
        /// <summary>
        /// Original was GL_FRAMEBUFFER_INCOMPLETE_LAYER_TARGETS = 0x8DA8
        /// </summary>
        FramebufferIncompleteLayerTargets = ((int)0x8DA8),
        /// <summary>
        /// Original was GL_FRAMEBUFFER_INCOMPLETE_LAYER_COUNT = 0x8DA9
        /// </summary>
        FramebufferIncompleteLayerCount = ((int)0x8DA9),
    }
    public enum FramebufferAttachment : int
    {
        /// <summary>
        /// Original was GL_FRONT_LEFT = 0x0400
        /// </summary>
        FrontLeft = ((int)0x0400),
        /// <summary>
        /// Original was GL_FRONT_RIGHT = 0x0401
        /// </summary>
        FrontRight = ((int)0x0401),
        /// <summary>
        /// Original was GL_BACK_LEFT = 0x0402
        /// </summary>
        BackLeft = ((int)0x0402),
        /// <summary>
        /// Original was GL_BACK_RIGHT = 0x0403
        /// </summary>
        BackRight = ((int)0x0403),
        /// <summary>
        /// Original was GL_AUX0 = 0x0409
        /// </summary>
        Aux0 = ((int)0x0409),
        /// <summary>
        /// Original was GL_AUX1 = 0x040A
        /// </summary>
        Aux1 = ((int)0x040A),
        /// <summary>
        /// Original was GL_AUX2 = 0x040B
        /// </summary>
        Aux2 = ((int)0x040B),
        /// <summary>
        /// Original was GL_AUX3 = 0x040C
        /// </summary>
        Aux3 = ((int)0x040C),
        /// <summary>
        /// Original was GL_COLOR = 0x1800
        /// </summary>
        Color = ((int)0x1800),
        /// <summary>
        /// Original was GL_DEPTH = 0x1801
        /// </summary>
        Depth = ((int)0x1801),
        /// <summary>
        /// Original was GL_STENCIL = 0x1802
        /// </summary>
        Stencil = ((int)0x1802),
        /// <summary>
        /// Original was GL_DEPTH_STENCIL_ATTACHMENT = 0x821A
        /// </summary>
        DepthStencilAttachment = ((int)0x821A),
        /// <summary>
        /// Original was GL_COLOR_ATTACHMENT0 = 0x8CE0
        /// </summary>
        ColorAttachment0 = ((int)0x8CE0),
        /// <summary>
        /// Original was GL_COLOR_ATTACHMENT0_EXT = 0x8CE0
        /// </summary>
        ColorAttachment0Ext = ((int)0x8CE0),
        /// <summary>
        /// Original was GL_COLOR_ATTACHMENT1 = 0x8CE1
        /// </summary>
        ColorAttachment1 = ((int)0x8CE1),
        /// <summary>
        /// Original was GL_COLOR_ATTACHMENT1_EXT = 0x8CE1
        /// </summary>
        ColorAttachment1Ext = ((int)0x8CE1),
        /// <summary>
        /// Original was GL_COLOR_ATTACHMENT2 = 0x8CE2
        /// </summary>
        ColorAttachment2 = ((int)0x8CE2),
        /// <summary>
        /// Original was GL_COLOR_ATTACHMENT2_EXT = 0x8CE2
        /// </summary>
        ColorAttachment2Ext = ((int)0x8CE2),
        /// <summary>
        /// Original was GL_COLOR_ATTACHMENT3 = 0x8CE3
        /// </summary>
        ColorAttachment3 = ((int)0x8CE3),
        /// <summary>
        /// Original was GL_COLOR_ATTACHMENT3_EXT = 0x8CE3
        /// </summary>
        ColorAttachment3Ext = ((int)0x8CE3),
        /// <summary>
        /// Original was GL_COLOR_ATTACHMENT4 = 0x8CE4
        /// </summary>
        ColorAttachment4 = ((int)0x8CE4),
        /// <summary>
        /// Original was GL_COLOR_ATTACHMENT4_EXT = 0x8CE4
        /// </summary>
        ColorAttachment4Ext = ((int)0x8CE4),
        /// <summary>
        /// Original was GL_COLOR_ATTACHMENT5 = 0x8CE5
        /// </summary>
        ColorAttachment5 = ((int)0x8CE5),
        /// <summary>
        /// Original was GL_COLOR_ATTACHMENT5_EXT = 0x8CE5
        /// </summary>
        ColorAttachment5Ext = ((int)0x8CE5),
        /// <summary>
        /// Original was GL_COLOR_ATTACHMENT6 = 0x8CE6
        /// </summary>
        ColorAttachment6 = ((int)0x8CE6),
        /// <summary>
        /// Original was GL_COLOR_ATTACHMENT6_EXT = 0x8CE6
        /// </summary>
        ColorAttachment6Ext = ((int)0x8CE6),
        /// <summary>
        /// Original was GL_COLOR_ATTACHMENT7 = 0x8CE7
        /// </summary>
        ColorAttachment7 = ((int)0x8CE7),
        /// <summary>
        /// Original was GL_COLOR_ATTACHMENT7_EXT = 0x8CE7
        /// </summary>
        ColorAttachment7Ext = ((int)0x8CE7),
        /// <summary>
        /// Original was GL_COLOR_ATTACHMENT8 = 0x8CE8
        /// </summary>
        ColorAttachment8 = ((int)0x8CE8),
        /// <summary>
        /// Original was GL_COLOR_ATTACHMENT8_EXT = 0x8CE8
        /// </summary>
        ColorAttachment8Ext = ((int)0x8CE8),
        /// <summary>
        /// Original was GL_COLOR_ATTACHMENT9 = 0x8CE9
        /// </summary>
        ColorAttachment9 = ((int)0x8CE9),
        /// <summary>
        /// Original was GL_COLOR_ATTACHMENT9_EXT = 0x8CE9
        /// </summary>
        ColorAttachment9Ext = ((int)0x8CE9),
        /// <summary>
        /// Original was GL_COLOR_ATTACHMENT10 = 0x8CEA
        /// </summary>
        ColorAttachment10 = ((int)0x8CEA),
        /// <summary>
        /// Original was GL_COLOR_ATTACHMENT10_EXT = 0x8CEA
        /// </summary>
        ColorAttachment10Ext = ((int)0x8CEA),
        /// <summary>
        /// Original was GL_COLOR_ATTACHMENT11 = 0x8CEB
        /// </summary>
        ColorAttachment11 = ((int)0x8CEB),
        /// <summary>
        /// Original was GL_COLOR_ATTACHMENT11_EXT = 0x8CEB
        /// </summary>
        ColorAttachment11Ext = ((int)0x8CEB),
        /// <summary>
        /// Original was GL_COLOR_ATTACHMENT12 = 0x8CEC
        /// </summary>
        ColorAttachment12 = ((int)0x8CEC),
        /// <summary>
        /// Original was GL_COLOR_ATTACHMENT12_EXT = 0x8CEC
        /// </summary>
        ColorAttachment12Ext = ((int)0x8CEC),
        /// <summary>
        /// Original was GL_COLOR_ATTACHMENT13 = 0x8CED
        /// </summary>
        ColorAttachment13 = ((int)0x8CED),
        /// <summary>
        /// Original was GL_COLOR_ATTACHMENT13_EXT = 0x8CED
        /// </summary>
        ColorAttachment13Ext = ((int)0x8CED),
        /// <summary>
        /// Original was GL_COLOR_ATTACHMENT14 = 0x8CEE
        /// </summary>
        ColorAttachment14 = ((int)0x8CEE),
        /// <summary>
        /// Original was GL_COLOR_ATTACHMENT14_EXT = 0x8CEE
        /// </summary>
        ColorAttachment14Ext = ((int)0x8CEE),
        /// <summary>
        /// Original was GL_COLOR_ATTACHMENT15 = 0x8CEF
        /// </summary>
        ColorAttachment15 = ((int)0x8CEF),
        /// <summary>
        /// Original was GL_COLOR_ATTACHMENT15_EXT = 0x8CEF
        /// </summary>
        ColorAttachment15Ext = ((int)0x8CEF),
        /// <summary>
        /// Original was GL_DEPTH_ATTACHMENT = 0x8D00
        /// </summary>
        DepthAttachment = ((int)0x8D00),
        /// <summary>
        /// Original was GL_DEPTH_ATTACHMENT_EXT = 0x8D00
        /// </summary>
        DepthAttachmentExt = ((int)0x8D00),
        /// <summary>
        /// Original was GL_STENCIL_ATTACHMENT = 0x8D20
        /// </summary>
        StencilAttachment = ((int)0x8D20),
        /// <summary>
        /// Original was GL_STENCIL_ATTACHMENT_EXT = 0x8D20
        /// </summary>
        StencilAttachmentExt = ((int)0x8D20),
    }
    public enum FramebufferTarget : int
    {
        /// <summary>
        /// Original was GL_READ_FRAMEBUFFER = 0x8CA8
        /// </summary>
        ReadFramebuffer = ((int)0x8CA8),
        /// <summary>
        /// Original was GL_DRAW_FRAMEBUFFER = 0x8CA9
        /// </summary>
        DrawFramebuffer = ((int)0x8CA9),
        /// <summary>
        /// Original was GL_FRAMEBUFFER = 0x8D40
        /// </summary>
        Framebuffer = ((int)0x8D40),
        /// <summary>
        /// Original was GL_FRAMEBUFFER_EXT = 0x8D40
        /// </summary>
        FramebufferExt = ((int)0x8D40),
    }
    #endregion

    #region Renderbuffer
    //
    // Summary:
    //     Used in GL.BindRenderbuffer, GL.FramebufferRenderbuffer and 11 other functions
    public enum RenderbufferTarget
    {
        //
        // Summary:
        //     Original was GL_RENDERBUFFER = 0x8D41
        Renderbuffer = 36161,
        //
        // Summary:
        //     Original was GL_RENDERBUFFER_EXT = 0x8D41
        RenderbufferExt = 36161
    }

    //
    // Summary:
    //     Used in GL.NamedRenderbufferStorage, GL.NamedRenderbufferStorageMultisample and
    //     6 other functions
    public enum RenderbufferStorage
    {
        //
        // Summary:
        //     Original was GL_DEPTH_COMPONENT = 0x1902
        DepthComponent = 6402,
        //
        // Summary:
        //     Original was GL_R3_G3_B2 = 0x2A10
        R3G3B2 = 10768,
        //
        // Summary:
        //     Original was GL_ALPHA4 = 0x803B
        Alpha4 = 32827,
        //
        // Summary:
        //     Original was GL_ALPHA8 = 0x803C
        Alpha8 = 32828,
        //
        // Summary:
        //     Original was GL_ALPHA12 = 0x803D
        Alpha12 = 32829,
        //
        // Summary:
        //     Original was GL_ALPHA16 = 0x803E
        Alpha16 = 32830,
        //
        // Summary:
        //     Original was GL_RGB4 = 0x804F
        Rgb4 = 32847,
        //
        // Summary:
        //     Original was GL_RGB5 = 0x8050
        Rgb5 = 32848,
        //
        // Summary:
        //     Original was GL_RGB8 = 0x8051
        Rgb8 = 32849,
        //
        // Summary:
        //     Original was GL_RGB10 = 0x8052
        Rgb10 = 32850,
        //
        // Summary:
        //     Original was GL_RGB12 = 0x8053
        Rgb12 = 32851,
        //
        // Summary:
        //     Original was GL_RGB16 = 0x8054
        Rgb16 = 32852,
        //
        // Summary:
        //     Original was GL_RGBA2 = 0x8055
        Rgba2 = 32853,
        //
        // Summary:
        //     Original was GL_RGBA4 = 0x8056
        Rgba4 = 32854,
        //
        // Summary:
        //     Original was GL_RGBA8 = 0x8058
        Rgba8 = 32856,
        //
        // Summary:
        //     Original was GL_RGB10_A2 = 0x8059
        Rgb10A2 = 32857,
        //
        // Summary:
        //     Original was GL_RGBA12 = 0x805A
        Rgba12 = 32858,
        //
        // Summary:
        //     Original was GL_RGBA16 = 0x805B
        Rgba16 = 32859,
        //
        // Summary:
        //     Original was GL_DEPTH_COMPONENT16 = 0x81a5
        DepthComponent16 = 33189,
        //
        // Summary:
        //     Original was GL_DEPTH_COMPONENT24 = 0x81a6
        DepthComponent24 = 33190,
        //
        // Summary:
        //     Original was GL_DEPTH_COMPONENT32 = 0x81a7
        DepthComponent32 = 33191,
        //
        // Summary:
        //     Original was GL_R8 = 0x8229
        R8 = 33321,
        //
        // Summary:
        //     Original was GL_R16 = 0x822A
        R16 = 33322,
        //
        // Summary:
        //     Original was GL_RG8 = 0x822B
        Rg8 = 33323,
        //
        // Summary:
        //     Original was GL_RG16 = 0x822C
        Rg16 = 33324,
        //
        // Summary:
        //     Original was GL_R16F = 0x822D
        R16f = 33325,
        //
        // Summary:
        //     Original was GL_R32F = 0x822E
        R32f = 33326,
        //
        // Summary:
        //     Original was GL_RG16F = 0x822F
        Rg16f = 33327,
        //
        // Summary:
        //     Original was GL_RG32F = 0x8230
        Rg32f = 33328,
        //
        // Summary:
        //     Original was GL_R8I = 0x8231
        R8i = 33329,
        //
        // Summary:
        //     Original was GL_R8UI = 0x8232
        R8ui = 33330,
        //
        // Summary:
        //     Original was GL_R16I = 0x8233
        R16i = 33331,
        //
        // Summary:
        //     Original was GL_R16UI = 0x8234
        R16ui = 33332,
        //
        // Summary:
        //     Original was GL_R32I = 0x8235
        R32i = 33333,
        //
        // Summary:
        //     Original was GL_R32UI = 0x8236
        R32ui = 33334,
        //
        // Summary:
        //     Original was GL_RG8I = 0x8237
        Rg8i = 33335,
        //
        // Summary:
        //     Original was GL_RG8UI = 0x8238
        Rg8ui = 33336,
        //
        // Summary:
        //     Original was GL_RG16I = 0x8239
        Rg16i = 33337,
        //
        // Summary:
        //     Original was GL_RG16UI = 0x823A
        Rg16ui = 33338,
        //
        // Summary:
        //     Original was GL_RG32I = 0x823B
        Rg32i = 33339,
        //
        // Summary:
        //     Original was GL_RG32UI = 0x823C
        Rg32ui = 33340,
        //
        // Summary:
        //     Original was GL_DEPTH_STENCIL = 0x84F9
        DepthStencil = 34041,
        //
        // Summary:
        //     Original was GL_RGBA32F = 0x8814
        Rgba32f = 34836,
        //
        // Summary:
        //     Original was GL_RGB32F = 0x8815
        Rgb32f = 34837,
        //
        // Summary:
        //     Original was GL_RGBA16F = 0x881A
        Rgba16f = 34842,
        //
        // Summary:
        //     Original was GL_RGB16F = 0x881B
        Rgb16f = 34843,
        //
        // Summary:
        //     Original was GL_DEPTH24_STENCIL8 = 0x88F0
        Depth24Stencil8 = 35056,
        //
        // Summary:
        //     Original was GL_R11F_G11F_B10F = 0x8C3A
        R11fG11fB10f = 35898,
        //
        // Summary:
        //     Original was GL_RGB9_E5 = 0x8C3D
        Rgb9E5 = 35901,
        //
        // Summary:
        //     Original was GL_SRGB8 = 0x8C41
        Srgb8 = 35905,
        //
        // Summary:
        //     Original was GL_SRGB8_ALPHA8 = 0x8C43
        Srgb8Alpha8 = 35907,
        //
        // Summary:
        //     Original was GL_DEPTH_COMPONENT32F = 0x8CAC
        DepthComponent32f = 36012,
        //
        // Summary:
        //     Original was GL_DEPTH32F_STENCIL8 = 0x8CAD
        Depth32fStencil8 = 36013,
        //
        // Summary:
        //     Original was GL_STENCIL_INDEX1 = 0x8D46
        StencilIndex1 = 36166,
        //
        // Summary:
        //     Original was GL_STENCIL_INDEX1_EXT = 0x8D46
        StencilIndex1Ext = 36166,
        //
        // Summary:
        //     Original was GL_STENCIL_INDEX4 = 0x8D47
        StencilIndex4 = 36167,
        //
        // Summary:
        //     Original was GL_STENCIL_INDEX4_EXT = 0x8D47
        StencilIndex4Ext = 36167,
        //
        // Summary:
        //     Original was GL_STENCIL_INDEX8 = 0x8D48
        StencilIndex8 = 36168,
        //
        // Summary:
        //     Original was GL_STENCIL_INDEX8_EXT = 0x8D48
        StencilIndex8Ext = 36168,
        //
        // Summary:
        //     Original was GL_STENCIL_INDEX16 = 0x8D49
        StencilIndex16 = 36169,
        //
        // Summary:
        //     Original was GL_STENCIL_INDEX16_EXT = 0x8D49
        StencilIndex16Ext = 36169,
        //
        // Summary:
        //     Original was GL_RGBA32UI = 0x8D70
        Rgba32ui = 36208,
        //
        // Summary:
        //     Original was GL_RGB32UI = 0x8D71
        Rgb32ui = 36209,
        //
        // Summary:
        //     Original was GL_RGBA16UI = 0x8D76
        Rgba16ui = 36214,
        //
        // Summary:
        //     Original was GL_RGB16UI = 0x8D77
        Rgb16ui = 36215,
        //
        // Summary:
        //     Original was GL_RGBA8UI = 0x8D7C
        Rgba8ui = 36220,
        //
        // Summary:
        //     Original was GL_RGB8UI = 0x8D7D
        Rgb8ui = 36221,
        //
        // Summary:
        //     Original was GL_RGBA32I = 0x8D82
        Rgba32i = 36226,
        //
        // Summary:
        //     Original was GL_RGB32I = 0x8D83
        Rgb32i = 36227,
        //
        // Summary:
        //     Original was GL_RGBA16I = 0x8D88
        Rgba16i = 36232,
        //
        // Summary:
        //     Original was GL_RGB16I = 0x8D89
        Rgb16i = 36233,
        //
        // Summary:
        //     Original was GL_RGBA8I = 0x8D8E
        Rgba8i = 36238,
        //
        // Summary:
        //     Original was GL_RGB8I = 0x8D8F
        Rgb8i = 36239,
        //
        // Summary:
        //     Original was GL_RGB10_A2UI = 0x906F
        Rgb10A2ui = 36975
    }

    //
    // Summary:
    //     Used in GL.NamedFramebufferReadBuffer, GL.ReadBuffer and 1 other function
    public enum ReadBufferMode
    {
        //
        // Summary:
        //     Original was GL_NONE = 0
        None = 0,
        //
        // Summary:
        //     Original was GL_FRONT_LEFT = 0x0400
        FrontLeft = 1024,
        //
        // Summary:
        //     Original was GL_FRONT_RIGHT = 0x0401
        FrontRight = 1025,
        //
        // Summary:
        //     Original was GL_BACK_LEFT = 0x0402
        BackLeft = 1026,
        //
        // Summary:
        //     Original was GL_BACK_RIGHT = 0x0403
        BackRight = 1027,
        //
        // Summary:
        //     Original was GL_FRONT = 0x0404
        Front = 1028,
        //
        // Summary:
        //     Original was GL_BACK = 0x0405
        Back = 1029,
        //
        // Summary:
        //     Original was GL_LEFT = 0x0406
        Left = 1030,
        //
        // Summary:
        //     Original was GL_RIGHT = 0x0407
        Right = 1031,
        //
        // Summary:
        //     Original was GL_FRONT_AND_BACK = 0x0408
        FrontAndBack = 1032,
        //
        // Summary:
        //     Original was GL_AUX0 = 0x0409
        Aux0 = 1033,
        //
        // Summary:
        //     Original was GL_AUX1 = 0x040A
        Aux1 = 1034,
        //
        // Summary:
        //     Original was GL_AUX2 = 0x040B
        Aux2 = 1035,
        //
        // Summary:
        //     Original was GL_AUX3 = 0x040C
        Aux3 = 1036,
        //
        // Summary:
        //     Original was GL_COLOR_ATTACHMENT0 = 0x8CE0
        ColorAttachment0 = 36064,
        //
        // Summary:
        //     Original was GL_COLOR_ATTACHMENT1 = 0x8CE1
        ColorAttachment1 = 36065,
        //
        // Summary:
        //     Original was GL_COLOR_ATTACHMENT2 = 0x8CE2
        ColorAttachment2 = 36066,
        //
        // Summary:
        //     Original was GL_COLOR_ATTACHMENT3 = 0x8CE3
        ColorAttachment3 = 36067,
        //
        // Summary:
        //     Original was GL_COLOR_ATTACHMENT4 = 0x8CE4
        ColorAttachment4 = 36068,
        //
        // Summary:
        //     Original was GL_COLOR_ATTACHMENT5 = 0x8CE5
        ColorAttachment5 = 36069,
        //
        // Summary:
        //     Original was GL_COLOR_ATTACHMENT6 = 0x8CE6
        ColorAttachment6 = 36070,
        //
        // Summary:
        //     Original was GL_COLOR_ATTACHMENT7 = 0x8CE7
        ColorAttachment7 = 36071,
        //
        // Summary:
        //     Original was GL_COLOR_ATTACHMENT8 = 0x8CE8
        ColorAttachment8 = 36072,
        //
        // Summary:
        //     Original was GL_COLOR_ATTACHMENT9 = 0x8CE9
        ColorAttachment9 = 36073,
        //
        // Summary:
        //     Original was GL_COLOR_ATTACHMENT10 = 0x8CEA
        ColorAttachment10 = 36074,
        //
        // Summary:
        //     Original was GL_COLOR_ATTACHMENT11 = 0x8CEB
        ColorAttachment11 = 36075,
        //
        // Summary:
        //     Original was GL_COLOR_ATTACHMENT12 = 0x8CEC
        ColorAttachment12 = 36076,
        //
        // Summary:
        //     Original was GL_COLOR_ATTACHMENT13 = 0x8CED
        ColorAttachment13 = 36077,
        //
        // Summary:
        //     Original was GL_COLOR_ATTACHMENT14 = 0x8CEE
        ColorAttachment14 = 36078,
        //
        // Summary:
        //     Original was GL_COLOR_ATTACHMENT15 = 0x8CEF
        ColorAttachment15 = 36079
    }

    //
    // Summary:
    //     Used in GL.DrawBuffer, GL.NamedFramebufferDrawBuffer and 2 other functions
    public enum DrawBufferMode
    {
        //
        // Summary:
        //     Original was GL_NONE = 0
        None = 0,
        //
        // Summary:
        //     Original was GL_NONE_OES = 0
        NoneOes = 0,
        //
        // Summary:
        //     Original was GL_FRONT_LEFT = 0x0400
        FrontLeft = 1024,
        //
        // Summary:
        //     Original was GL_FRONT_RIGHT = 0x0401
        FrontRight = 1025,
        //
        // Summary:
        //     Original was GL_BACK_LEFT = 0x0402
        BackLeft = 1026,
        //
        // Summary:
        //     Original was GL_BACK_RIGHT = 0x0403
        BackRight = 1027,
        //
        // Summary:
        //     Original was GL_FRONT = 0x0404
        Front = 1028,
        //
        // Summary:
        //     Original was GL_BACK = 0x0405
        Back = 1029,
        //
        // Summary:
        //     Original was GL_LEFT = 0x0406
        Left = 1030,
        //
        // Summary:
        //     Original was GL_RIGHT = 0x0407
        Right = 1031,
        //
        // Summary:
        //     Original was GL_FRONT_AND_BACK = 0x0408
        FrontAndBack = 1032,
        //
        // Summary:
        //     Original was GL_AUX0 = 0x0409
        Aux0 = 1033,
        //
        // Summary:
        //     Original was GL_AUX1 = 0x040A
        Aux1 = 1034,
        //
        // Summary:
        //     Original was GL_AUX2 = 0x040B
        Aux2 = 1035,
        //
        // Summary:
        //     Original was GL_AUX3 = 0x040C
        Aux3 = 1036,
        //
        // Summary:
        //     Original was GL_COLOR_ATTACHMENT0 = 0x8CE0
        ColorAttachment0 = 36064,
        //
        // Summary:
        //     Original was GL_COLOR_ATTACHMENT1 = 0x8CE1
        ColorAttachment1 = 36065,
        //
        // Summary:
        //     Original was GL_COLOR_ATTACHMENT2 = 0x8CE2
        ColorAttachment2 = 36066,
        //
        // Summary:
        //     Original was GL_COLOR_ATTACHMENT3 = 0x8CE3
        ColorAttachment3 = 36067,
        //
        // Summary:
        //     Original was GL_COLOR_ATTACHMENT4 = 0x8CE4
        ColorAttachment4 = 36068,
        //
        // Summary:
        //     Original was GL_COLOR_ATTACHMENT5 = 0x8CE5
        ColorAttachment5 = 36069,
        //
        // Summary:
        //     Original was GL_COLOR_ATTACHMENT6 = 0x8CE6
        ColorAttachment6 = 36070,
        //
        // Summary:
        //     Original was GL_COLOR_ATTACHMENT7 = 0x8CE7
        ColorAttachment7 = 36071,
        //
        // Summary:
        //     Original was GL_COLOR_ATTACHMENT8 = 0x8CE8
        ColorAttachment8 = 36072,
        //
        // Summary:
        //     Original was GL_COLOR_ATTACHMENT9 = 0x8CE9
        ColorAttachment9 = 36073,
        //
        // Summary:
        //     Original was GL_COLOR_ATTACHMENT10 = 0x8CEA
        ColorAttachment10 = 36074,
        //
        // Summary:
        //     Original was GL_COLOR_ATTACHMENT11 = 0x8CEB
        ColorAttachment11 = 36075,
        //
        // Summary:
        //     Original was GL_COLOR_ATTACHMENT12 = 0x8CEC
        ColorAttachment12 = 36076,
        //
        // Summary:
        //     Original was GL_COLOR_ATTACHMENT13 = 0x8CED
        ColorAttachment13 = 36077,
        //
        // Summary:
        //     Original was GL_COLOR_ATTACHMENT14 = 0x8CEE
        ColorAttachment14 = 36078,
        //
        // Summary:
        //     Original was GL_COLOR_ATTACHMENT15 = 0x8CEF
        ColorAttachment15 = 36079
    }
    #endregion

#region GL
#if OPENTK3
    public enum BlendingFactor
    {
        Zero = 0,
        One = 1,
        SrcColor = 768,
        OneMinusSrcColor = 769,
        SrcAlpha = 770,
        OneMinusSrcAlpha = 771,
        DstAlpha = 772,
        OneMinusDstAlpha = 773,
        DstColor = 774,
        OneMinusDstColor = 775,
        SrcAlphaSaturate = 776,
        ConstantColor = 32769,
        OneMinusConstantColor = 32770,
        ConstantAlpha = 32771,
        OneMinusConstantAlpha = 32772,
        Src1Alpha = 34185,
        Src1Color = 35065
    }
#else
    public enum BlendingFactorDest : int
    {
        /// <summary>
        /// Original was GL_ZERO = 0
        /// </summary>
        Zero = ((int)0),
        /// <summary>
        /// Original was GL_SRC_COLOR = 0x0300
        /// </summary>
        SrcColor = ((int)0x0300),
        /// <summary>
        /// Original was GL_ONE_MINUS_SRC_COLOR = 0x0301
        /// </summary>
        OneMinusSrcColor = ((int)0x0301),
        /// <summary>
        /// Original was GL_SRC_ALPHA = 0x0302
        /// </summary>
        SrcAlpha = ((int)0x0302),
        /// <summary>
        /// Original was GL_ONE_MINUS_SRC_ALPHA = 0x0303
        /// </summary>
        OneMinusSrcAlpha = ((int)0x0303),
        /// <summary>
        /// Original was GL_DST_ALPHA = 0x0304
        /// </summary>
        DstAlpha = ((int)0x0304),
        /// <summary>
        /// Original was GL_ONE_MINUS_DST_ALPHA = 0x0305
        /// </summary>
        OneMinusDstAlpha = ((int)0x0305),
        /// <summary>
        /// Original was GL_DST_COLOR = 0x0306
        /// </summary>
        DstColor = ((int)0x0306),
        /// <summary>
        /// Original was GL_ONE_MINUS_DST_COLOR = 0x0307
        /// </summary>
        OneMinusDstColor = ((int)0x0307),
        /// <summary>
        /// Original was GL_SRC_ALPHA_SATURATE = 0x0308
        /// </summary>
        SrcAlphaSaturate = ((int)0x0308),
        /// <summary>
        /// Original was GL_CONSTANT_COLOR = 0x8001
        /// </summary>
        ConstantColor = ((int)0x8001),
        /// <summary>
        /// Original was GL_CONSTANT_COLOR_EXT = 0x8001
        /// </summary>
        ConstantColorExt = ((int)0x8001),
        /// <summary>
        /// Original was GL_ONE_MINUS_CONSTANT_COLOR = 0x8002
        /// </summary>
        OneMinusConstantColor = ((int)0x8002),
        /// <summary>
        /// Original was GL_ONE_MINUS_CONSTANT_COLOR_EXT = 0x8002
        /// </summary>
        OneMinusConstantColorExt = ((int)0x8002),
        /// <summary>
        /// Original was GL_CONSTANT_ALPHA = 0x8003
        /// </summary>
        ConstantAlpha = ((int)0x8003),
        /// <summary>
        /// Original was GL_CONSTANT_ALPHA_EXT = 0x8003
        /// </summary>
        ConstantAlphaExt = ((int)0x8003),
        /// <summary>
        /// Original was GL_ONE_MINUS_CONSTANT_ALPHA = 0x8004
        /// </summary>
        OneMinusConstantAlpha = ((int)0x8004),
        /// <summary>
        /// Original was GL_ONE_MINUS_CONSTANT_ALPHA_EXT = 0x8004
        /// </summary>
        OneMinusConstantAlphaExt = ((int)0x8004),
        /// <summary>
        /// Original was GL_SRC1_ALPHA = 0x8589
        /// </summary>
        Src1Alpha = ((int)0x8589),
        /// <summary>
        /// Original was GL_SRC1_COLOR = 0x88F9
        /// </summary>
        Src1Color = ((int)0x88F9),
        /// <summary>
        /// Original was GL_ONE_MINUS_SRC1_COLOR = 0x88FA
        /// </summary>
        OneMinusSrc1Color = ((int)0x88FA),
        /// <summary>
        /// Original was GL_ONE_MINUS_SRC1_ALPHA = 0x88FB
        /// </summary>
        OneMinusSrc1Alpha = ((int)0x88FB),
        /// <summary>
        /// Original was GL_ONE = 1
        /// </summary>
        One = ((int)1),
    }
    public enum BlendingFactorSrc : int
    {
        /// <summary>
        /// Original was GL_ZERO = 0
        /// </summary>
        Zero = ((int)0),
        /// <summary>
        /// Original was GL_SRC_COLOR = 0x0300
        /// </summary>
        SrcColor = ((int)0x0300),
        /// <summary>
        /// Original was GL_ONE_MINUS_SRC_COLOR = 0x0301
        /// </summary>
        OneMinusSrcColor = ((int)0x0301),
        /// <summary>
        /// Original was GL_SRC_ALPHA = 0x0302
        /// </summary>
        SrcAlpha = ((int)0x0302),
        /// <summary>
        /// Original was GL_ONE_MINUS_SRC_ALPHA = 0x0303
        /// </summary>
        OneMinusSrcAlpha = ((int)0x0303),
        /// <summary>
        /// Original was GL_DST_ALPHA = 0x0304
        /// </summary>
        DstAlpha = ((int)0x0304),
        /// <summary>
        /// Original was GL_ONE_MINUS_DST_ALPHA = 0x0305
        /// </summary>
        OneMinusDstAlpha = ((int)0x0305),
        /// <summary>
        /// Original was GL_DST_COLOR = 0x0306
        /// </summary>
        DstColor = ((int)0x0306),
        /// <summary>
        /// Original was GL_ONE_MINUS_DST_COLOR = 0x0307
        /// </summary>
        OneMinusDstColor = ((int)0x0307),
        /// <summary>
        /// Original was GL_SRC_ALPHA_SATURATE = 0x0308
        /// </summary>
        SrcAlphaSaturate = ((int)0x0308),
        /// <summary>
        /// Original was GL_CONSTANT_COLOR = 0x8001
        /// </summary>
        ConstantColor = ((int)0x8001),
        /// <summary>
        /// Original was GL_CONSTANT_COLOR_EXT = 0x8001
        /// </summary>
        ConstantColorExt = ((int)0x8001),
        /// <summary>
        /// Original was GL_ONE_MINUS_CONSTANT_COLOR = 0x8002
        /// </summary>
        OneMinusConstantColor = ((int)0x8002),
        /// <summary>
        /// Original was GL_ONE_MINUS_CONSTANT_COLOR_EXT = 0x8002
        /// </summary>
        OneMinusConstantColorExt = ((int)0x8002),
        /// <summary>
        /// Original was GL_CONSTANT_ALPHA = 0x8003
        /// </summary>
        ConstantAlpha = ((int)0x8003),
        /// <summary>
        /// Original was GL_CONSTANT_ALPHA_EXT = 0x8003
        /// </summary>
        ConstantAlphaExt = ((int)0x8003),
        /// <summary>
        /// Original was GL_ONE_MINUS_CONSTANT_ALPHA = 0x8004
        /// </summary>
        OneMinusConstantAlpha = ((int)0x8004),
        /// <summary>
        /// Original was GL_ONE_MINUS_CONSTANT_ALPHA_EXT = 0x8004
        /// </summary>
        OneMinusConstantAlphaExt = ((int)0x8004),
        /// <summary>
        /// Original was GL_SRC1_ALPHA = 0x8589
        /// </summary>
        Src1Alpha = ((int)0x8589),
        /// <summary>
        /// Original was GL_SRC1_COLOR = 0x88F9
        /// </summary>
        Src1Color = ((int)0x88F9),
        /// <summary>
        /// Original was GL_ONE_MINUS_SRC1_COLOR = 0x88FA
        /// </summary>
        OneMinusSrc1Color = ((int)0x88FA),
        /// <summary>
        /// Original was GL_ONE_MINUS_SRC1_ALPHA = 0x88FB
        /// </summary>
        OneMinusSrc1Alpha = ((int)0x88FB),
        /// <summary>
        /// Original was GL_ONE = 1
        /// </summary>
        One = ((int)1),
    }
#endif
    //
    // Summary:
    //     Used in GL.BlitFramebuffer, GL.BlitNamedFramebuffer and 1 other function
    public enum BlitFramebufferFilter
    {
        //
        // Summary:
        //     Original was GL_NEAREST = 0x2600
        Nearest = 9728,
        //
        // Summary:
        //     Original was GL_LINEAR = 0x2601
        Linear = 9729
    }
    [Flags]
    public enum ClearBufferMask : int
    {
        /// <summary>
        /// Original was GL_NONE = 0
        /// </summary>
        None = ((int)0),
        /// <summary>
        /// Original was GL_DEPTH_BUFFER_BIT = 0x00000100
        /// </summary>
        DepthBufferBit = ((int)0x00000100),
        /// <summary>
        /// Original was GL_ACCUM_BUFFER_BIT = 0x00000200
        /// </summary>
        AccumBufferBit = ((int)0x00000200),
        /// <summary>
        /// Original was GL_STENCIL_BUFFER_BIT = 0x00000400
        /// </summary>
        StencilBufferBit = ((int)0x00000400),
        /// <summary>
        /// Original was GL_COLOR_BUFFER_BIT = 0x00004000
        /// </summary>
        ColorBufferBit = ((int)0x00004000),
        /// <summary>
        /// Original was GL_COVERAGE_BUFFER_BIT_NV = 0x00008000
        /// </summary>
        CoverageBufferBitNv = ((int)0x00008000),
    }
    public enum CullFaceMode : int
    {
        /// <summary>
        /// Original was GL_FRONT = 0x0404
        /// </summary>
        Front = ((int)0x0404),
        /// <summary>
        /// Original was GL_BACK = 0x0405
        /// </summary>
        Back = ((int)0x0405),
        /// <summary>
        /// Original was GL_FRONT_AND_BACK = 0x0408
        /// </summary>
        FrontAndBack = ((int)0x0408),
    }
    public enum DepthFunction : int
    {
        /// <summary>
        /// Original was GL_NEVER = 0x0200
        /// </summary>
        Never = ((int)0x0200),
        /// <summary>
        /// Original was GL_LESS = 0x0201
        /// </summary>
        Less = ((int)0x0201),
        /// <summary>
        /// Original was GL_EQUAL = 0x0202
        /// </summary>
        Equal = ((int)0x0202),
        /// <summary>
        /// Original was GL_LEQUAL = 0x0203
        /// </summary>
        Lequal = ((int)0x0203),
        /// <summary>
        /// Original was GL_GREATER = 0x0204
        /// </summary>
        Greater = ((int)0x0204),
        /// <summary>
        /// Original was GL_NOTEQUAL = 0x0205
        /// </summary>
        Notequal = ((int)0x0205),
        /// <summary>
        /// Original was GL_GEQUAL = 0x0206
        /// </summary>
        Gequal = ((int)0x0206),
        /// <summary>
        /// Original was GL_ALWAYS = 0x0207
        /// </summary>
        Always = ((int)0x0207),
    }
    public enum EnableCap : int
    {
        /// <summary>
        /// Original was GL_POINT_SMOOTH = 0x0B10
        /// </summary>
        PointSmooth = ((int)0x0B10),
        /// <summary>
        /// Original was GL_LINE_SMOOTH = 0x0B20
        /// </summary>
        LineSmooth = ((int)0x0B20),
        /// <summary>
        /// Original was GL_LINE_STIPPLE = 0x0B24
        /// </summary>
        LineStipple = ((int)0x0B24),
        /// <summary>
        /// Original was GL_POLYGON_SMOOTH = 0x0B41
        /// </summary>
        PolygonSmooth = ((int)0x0B41),
        /// <summary>
        /// Original was GL_POLYGON_STIPPLE = 0x0B42
        /// </summary>
        PolygonStipple = ((int)0x0B42),
        /// <summary>
        /// Original was GL_CULL_FACE = 0x0B44
        /// </summary>
        CullFace = ((int)0x0B44),
        /// <summary>
        /// Original was GL_LIGHTING = 0x0B50
        /// </summary>
        Lighting = ((int)0x0B50),
        /// <summary>
        /// Original was GL_COLOR_MATERIAL = 0x0B57
        /// </summary>
        ColorMaterial = ((int)0x0B57),
        /// <summary>
        /// Original was GL_FOG = 0x0B60
        /// </summary>
        Fog = ((int)0x0B60),
        /// <summary>
        /// Original was GL_DEPTH_TEST = 0x0B71
        /// </summary>
        DepthTest = ((int)0x0B71),
        /// <summary>
        /// Original was GL_STENCIL_TEST = 0x0B90
        /// </summary>
        StencilTest = ((int)0x0B90),
        /// <summary>
        /// Original was GL_NORMALIZE = 0x0BA1
        /// </summary>
        Normalize = ((int)0x0BA1),
        /// <summary>
        /// Original was GL_ALPHA_TEST = 0x0BC0
        /// </summary>
        AlphaTest = ((int)0x0BC0),
        /// <summary>
        /// Original was GL_DITHER = 0x0BD0
        /// </summary>
        Dither = ((int)0x0BD0),
        /// <summary>
        /// Original was GL_BLEND = 0x0BE2
        /// </summary>
        Blend = ((int)0x0BE2),
        /// <summary>
        /// Original was GL_INDEX_LOGIC_OP = 0x0BF1
        /// </summary>
        IndexLogicOp = ((int)0x0BF1),
        /// <summary>
        /// Original was GL_COLOR_LOGIC_OP = 0x0BF2
        /// </summary>
        ColorLogicOp = ((int)0x0BF2),
        /// <summary>
        /// Original was GL_SCISSOR_TEST = 0x0C11
        /// </summary>
        ScissorTest = ((int)0x0C11),
        /// <summary>
        /// Original was GL_TEXTURE_GEN_S = 0x0C60
        /// </summary>
        TextureGenS = ((int)0x0C60),
        /// <summary>
        /// Original was GL_TEXTURE_GEN_T = 0x0C61
        /// </summary>
        TextureGenT = ((int)0x0C61),
        /// <summary>
        /// Original was GL_TEXTURE_GEN_R = 0x0C62
        /// </summary>
        TextureGenR = ((int)0x0C62),
        /// <summary>
        /// Original was GL_TEXTURE_GEN_Q = 0x0C63
        /// </summary>
        TextureGenQ = ((int)0x0C63),
        /// <summary>
        /// Original was GL_AUTO_NORMAL = 0x0D80
        /// </summary>
        AutoNormal = ((int)0x0D80),
        /// <summary>
        /// Original was GL_MAP1_COLOR_4 = 0x0D90
        /// </summary>
        Map1Color4 = ((int)0x0D90),
        /// <summary>
        /// Original was GL_MAP1_INDEX = 0x0D91
        /// </summary>
        Map1Index = ((int)0x0D91),
        /// <summary>
        /// Original was GL_MAP1_NORMAL = 0x0D92
        /// </summary>
        Map1Normal = ((int)0x0D92),
        /// <summary>
        /// Original was GL_MAP1_TEXTURE_COORD_1 = 0x0D93
        /// </summary>
        Map1TextureCoord1 = ((int)0x0D93),
        /// <summary>
        /// Original was GL_MAP1_TEXTURE_COORD_2 = 0x0D94
        /// </summary>
        Map1TextureCoord2 = ((int)0x0D94),
        /// <summary>
        /// Original was GL_MAP1_TEXTURE_COORD_3 = 0x0D95
        /// </summary>
        Map1TextureCoord3 = ((int)0x0D95),
        /// <summary>
        /// Original was GL_MAP1_TEXTURE_COORD_4 = 0x0D96
        /// </summary>
        Map1TextureCoord4 = ((int)0x0D96),
        /// <summary>
        /// Original was GL_MAP1_VERTEX_3 = 0x0D97
        /// </summary>
        Map1Vertex3 = ((int)0x0D97),
        /// <summary>
        /// Original was GL_MAP1_VERTEX_4 = 0x0D98
        /// </summary>
        Map1Vertex4 = ((int)0x0D98),
        /// <summary>
        /// Original was GL_MAP2_COLOR_4 = 0x0DB0
        /// </summary>
        Map2Color4 = ((int)0x0DB0),
        /// <summary>
        /// Original was GL_MAP2_INDEX = 0x0DB1
        /// </summary>
        Map2Index = ((int)0x0DB1),
        /// <summary>
        /// Original was GL_MAP2_NORMAL = 0x0DB2
        /// </summary>
        Map2Normal = ((int)0x0DB2),
        /// <summary>
        /// Original was GL_MAP2_TEXTURE_COORD_1 = 0x0DB3
        /// </summary>
        Map2TextureCoord1 = ((int)0x0DB3),
        /// <summary>
        /// Original was GL_MAP2_TEXTURE_COORD_2 = 0x0DB4
        /// </summary>
        Map2TextureCoord2 = ((int)0x0DB4),
        /// <summary>
        /// Original was GL_MAP2_TEXTURE_COORD_3 = 0x0DB5
        /// </summary>
        Map2TextureCoord3 = ((int)0x0DB5),
        /// <summary>
        /// Original was GL_MAP2_TEXTURE_COORD_4 = 0x0DB6
        /// </summary>
        Map2TextureCoord4 = ((int)0x0DB6),
        /// <summary>
        /// Original was GL_MAP2_VERTEX_3 = 0x0DB7
        /// </summary>
        Map2Vertex3 = ((int)0x0DB7),
        /// <summary>
        /// Original was GL_MAP2_VERTEX_4 = 0x0DB8
        /// </summary>
        Map2Vertex4 = ((int)0x0DB8),
        /// <summary>
        /// Original was GL_TEXTURE_1D = 0x0DE0
        /// </summary>
        Texture1D = ((int)0x0DE0),
        /// <summary>
        /// Original was GL_TEXTURE_2D = 0x0DE1
        /// </summary>
        Texture2D = ((int)0x0DE1),
        /// <summary>
        /// Original was GL_POLYGON_OFFSET_POINT = 0x2A01
        /// </summary>
        PolygonOffsetPoint = ((int)0x2A01),
        /// <summary>
        /// Original was GL_POLYGON_OFFSET_LINE = 0x2A02
        /// </summary>
        PolygonOffsetLine = ((int)0x2A02),
        /// <summary>
        /// Original was GL_CLIP_DISTANCE0 = 0x3000
        /// </summary>
        ClipDistance0 = ((int)0x3000),
        /// <summary>
        /// Original was GL_CLIP_PLANE0 = 0x3000
        /// </summary>
        ClipPlane0 = ((int)0x3000),
        /// <summary>
        /// Original was GL_CLIP_DISTANCE1 = 0x3001
        /// </summary>
        ClipDistance1 = ((int)0x3001),
        /// <summary>
        /// Original was GL_CLIP_PLANE1 = 0x3001
        /// </summary>
        ClipPlane1 = ((int)0x3001),
        /// <summary>
        /// Original was GL_CLIP_DISTANCE2 = 0x3002
        /// </summary>
        ClipDistance2 = ((int)0x3002),
        /// <summary>
        /// Original was GL_CLIP_PLANE2 = 0x3002
        /// </summary>
        ClipPlane2 = ((int)0x3002),
        /// <summary>
        /// Original was GL_CLIP_DISTANCE3 = 0x3003
        /// </summary>
        ClipDistance3 = ((int)0x3003),
        /// <summary>
        /// Original was GL_CLIP_PLANE3 = 0x3003
        /// </summary>
        ClipPlane3 = ((int)0x3003),
        /// <summary>
        /// Original was GL_CLIP_DISTANCE4 = 0x3004
        /// </summary>
        ClipDistance4 = ((int)0x3004),
        /// <summary>
        /// Original was GL_CLIP_PLANE4 = 0x3004
        /// </summary>
        ClipPlane4 = ((int)0x3004),
        /// <summary>
        /// Original was GL_CLIP_DISTANCE5 = 0x3005
        /// </summary>
        ClipDistance5 = ((int)0x3005),
        /// <summary>
        /// Original was GL_CLIP_PLANE5 = 0x3005
        /// </summary>
        ClipPlane5 = ((int)0x3005),
        /// <summary>
        /// Original was GL_CLIP_DISTANCE6 = 0x3006
        /// </summary>
        ClipDistance6 = ((int)0x3006),
        /// <summary>
        /// Original was GL_CLIP_DISTANCE7 = 0x3007
        /// </summary>
        ClipDistance7 = ((int)0x3007),
        /// <summary>
        /// Original was GL_LIGHT0 = 0x4000
        /// </summary>
        Light0 = ((int)0x4000),
        /// <summary>
        /// Original was GL_LIGHT1 = 0x4001
        /// </summary>
        Light1 = ((int)0x4001),
        /// <summary>
        /// Original was GL_LIGHT2 = 0x4002
        /// </summary>
        Light2 = ((int)0x4002),
        /// <summary>
        /// Original was GL_LIGHT3 = 0x4003
        /// </summary>
        Light3 = ((int)0x4003),
        /// <summary>
        /// Original was GL_LIGHT4 = 0x4004
        /// </summary>
        Light4 = ((int)0x4004),
        /// <summary>
        /// Original was GL_LIGHT5 = 0x4005
        /// </summary>
        Light5 = ((int)0x4005),
        /// <summary>
        /// Original was GL_LIGHT6 = 0x4006
        /// </summary>
        Light6 = ((int)0x4006),
        /// <summary>
        /// Original was GL_LIGHT7 = 0x4007
        /// </summary>
        Light7 = ((int)0x4007),
        /// <summary>
        /// Original was GL_CONVOLUTION_1D = 0x8010
        /// </summary>
        Convolution1D = ((int)0x8010),
        /// <summary>
        /// Original was GL_CONVOLUTION_1D_EXT = 0x8010
        /// </summary>
        Convolution1DExt = ((int)0x8010),
        /// <summary>
        /// Original was GL_CONVOLUTION_2D = 0x8011
        /// </summary>
        Convolution2D = ((int)0x8011),
        /// <summary>
        /// Original was GL_CONVOLUTION_2D_EXT = 0x8011
        /// </summary>
        Convolution2DExt = ((int)0x8011),
        /// <summary>
        /// Original was GL_SEPARABLE_2D = 0x8012
        /// </summary>
        Separable2D = ((int)0x8012),
        /// <summary>
        /// Original was GL_SEPARABLE_2D_EXT = 0x8012
        /// </summary>
        Separable2DExt = ((int)0x8012),
        /// <summary>
        /// Original was GL_HISTOGRAM = 0x8024
        /// </summary>
        Histogram = ((int)0x8024),
        /// <summary>
        /// Original was GL_HISTOGRAM_EXT = 0x8024
        /// </summary>
        HistogramExt = ((int)0x8024),
        /// <summary>
        /// Original was GL_MINMAX_EXT = 0x802E
        /// </summary>
        MinmaxExt = ((int)0x802E),
        /// <summary>
        /// Original was GL_POLYGON_OFFSET_FILL = 0x8037
        /// </summary>
        PolygonOffsetFill = ((int)0x8037),
        /// <summary>
        /// Original was GL_RESCALE_NORMAL = 0x803A
        /// </summary>
        RescaleNormal = ((int)0x803A),
        /// <summary>
        /// Original was GL_RESCALE_NORMAL_EXT = 0x803A
        /// </summary>
        RescaleNormalExt = ((int)0x803A),
        /// <summary>
        /// Original was GL_TEXTURE_3D_EXT = 0x806F
        /// </summary>
        Texture3DExt = ((int)0x806F),
        /// <summary>
        /// Original was GL_VERTEX_ARRAY = 0x8074
        /// </summary>
        VertexArray = ((int)0x8074),
        /// <summary>
        /// Original was GL_NORMAL_ARRAY = 0x8075
        /// </summary>
        NormalArray = ((int)0x8075),
        /// <summary>
        /// Original was GL_COLOR_ARRAY = 0x8076
        /// </summary>
        ColorArray = ((int)0x8076),
        /// <summary>
        /// Original was GL_INDEX_ARRAY = 0x8077
        /// </summary>
        IndexArray = ((int)0x8077),
        /// <summary>
        /// Original was GL_TEXTURE_COORD_ARRAY = 0x8078
        /// </summary>
        TextureCoordArray = ((int)0x8078),
        /// <summary>
        /// Original was GL_EDGE_FLAG_ARRAY = 0x8079
        /// </summary>
        EdgeFlagArray = ((int)0x8079),
        /// <summary>
        /// Original was GL_INTERLACE_SGIX = 0x8094
        /// </summary>
        InterlaceSgix = ((int)0x8094),
        /// <summary>
        /// Original was GL_MULTISAMPLE = 0x809D
        /// </summary>
        Multisample = ((int)0x809D),
        /// <summary>
        /// Original was GL_MULTISAMPLE_SGIS = 0x809D
        /// </summary>
        MultisampleSgis = ((int)0x809D),
        /// <summary>
        /// Original was GL_SAMPLE_ALPHA_TO_COVERAGE = 0x809E
        /// </summary>
        SampleAlphaToCoverage = ((int)0x809E),
        /// <summary>
        /// Original was GL_SAMPLE_ALPHA_TO_MASK_SGIS = 0x809E
        /// </summary>
        SampleAlphaToMaskSgis = ((int)0x809E),
        /// <summary>
        /// Original was GL_SAMPLE_ALPHA_TO_ONE = 0x809F
        /// </summary>
        SampleAlphaToOne = ((int)0x809F),
        /// <summary>
        /// Original was GL_SAMPLE_ALPHA_TO_ONE_SGIS = 0x809F
        /// </summary>
        SampleAlphaToOneSgis = ((int)0x809F),
        /// <summary>
        /// Original was GL_SAMPLE_COVERAGE = 0x80A0
        /// </summary>
        SampleCoverage = ((int)0x80A0),
        /// <summary>
        /// Original was GL_SAMPLE_MASK_SGIS = 0x80A0
        /// </summary>
        SampleMaskSgis = ((int)0x80A0),
        /// <summary>
        /// Original was GL_TEXTURE_COLOR_TABLE_SGI = 0x80BC
        /// </summary>
        TextureColorTableSgi = ((int)0x80BC),
        /// <summary>
        /// Original was GL_COLOR_TABLE = 0x80D0
        /// </summary>
        ColorTable = ((int)0x80D0),
        /// <summary>
        /// Original was GL_COLOR_TABLE_SGI = 0x80D0
        /// </summary>
        ColorTableSgi = ((int)0x80D0),
        /// <summary>
        /// Original was GL_POST_CONVOLUTION_COLOR_TABLE = 0x80D1
        /// </summary>
        PostConvolutionColorTable = ((int)0x80D1),
        /// <summary>
        /// Original was GL_POST_CONVOLUTION_COLOR_TABLE_SGI = 0x80D1
        /// </summary>
        PostConvolutionColorTableSgi = ((int)0x80D1),
        /// <summary>
        /// Original was GL_POST_COLOR_MATRIX_COLOR_TABLE = 0x80D2
        /// </summary>
        PostColorMatrixColorTable = ((int)0x80D2),
        /// <summary>
        /// Original was GL_POST_COLOR_MATRIX_COLOR_TABLE_SGI = 0x80D2
        /// </summary>
        PostColorMatrixColorTableSgi = ((int)0x80D2),
        /// <summary>
        /// Original was GL_TEXTURE_4D_SGIS = 0x8134
        /// </summary>
        Texture4DSgis = ((int)0x8134),
        /// <summary>
        /// Original was GL_PIXEL_TEX_GEN_SGIX = 0x8139
        /// </summary>
        PixelTexGenSgix = ((int)0x8139),
        /// <summary>
        /// Original was GL_SPRITE_SGIX = 0x8148
        /// </summary>
        SpriteSgix = ((int)0x8148),
        /// <summary>
        /// Original was GL_REFERENCE_PLANE_SGIX = 0x817D
        /// </summary>
        ReferencePlaneSgix = ((int)0x817D),
        /// <summary>
        /// Original was GL_IR_INSTRUMENT1_SGIX = 0x817F
        /// </summary>
        IrInstrument1Sgix = ((int)0x817F),
        /// <summary>
        /// Original was GL_CALLIGRAPHIC_FRAGMENT_SGIX = 0x8183
        /// </summary>
        CalligraphicFragmentSgix = ((int)0x8183),
        /// <summary>
        /// Original was GL_FRAMEZOOM_SGIX = 0x818B
        /// </summary>
        FramezoomSgix = ((int)0x818B),
        /// <summary>
        /// Original was GL_FOG_OFFSET_SGIX = 0x8198
        /// </summary>
        FogOffsetSgix = ((int)0x8198),
        /// <summary>
        /// Original was GL_SHARED_TEXTURE_PALETTE_EXT = 0x81FB
        /// </summary>
        SharedTexturePaletteExt = ((int)0x81FB),
        /// <summary>
        /// Original was GL_DEBUG_OUTPUT_SYNCHRONOUS = 0x8242
        /// </summary>
        DebugOutputSynchronous = ((int)0x8242),
        /// <summary>
        /// Original was GL_ASYNC_HISTOGRAM_SGIX = 0x832C
        /// </summary>
        AsyncHistogramSgix = ((int)0x832C),
        /// <summary>
        /// Original was GL_PIXEL_TEXTURE_SGIS = 0x8353
        /// </summary>
        PixelTextureSgis = ((int)0x8353),
        /// <summary>
        /// Original was GL_ASYNC_TEX_IMAGE_SGIX = 0x835C
        /// </summary>
        AsyncTexImageSgix = ((int)0x835C),
        /// <summary>
        /// Original was GL_ASYNC_DRAW_PIXELS_SGIX = 0x835D
        /// </summary>
        AsyncDrawPixelsSgix = ((int)0x835D),
        /// <summary>
        /// Original was GL_ASYNC_READ_PIXELS_SGIX = 0x835E
        /// </summary>
        AsyncReadPixelsSgix = ((int)0x835E),
        /// <summary>
        /// Original was GL_FRAGMENT_LIGHTING_SGIX = 0x8400
        /// </summary>
        FragmentLightingSgix = ((int)0x8400),
        /// <summary>
        /// Original was GL_FRAGMENT_COLOR_MATERIAL_SGIX = 0x8401
        /// </summary>
        FragmentColorMaterialSgix = ((int)0x8401),
        /// <summary>
        /// Original was GL_FRAGMENT_LIGHT0_SGIX = 0x840C
        /// </summary>
        FragmentLight0Sgix = ((int)0x840C),
        /// <summary>
        /// Original was GL_FRAGMENT_LIGHT1_SGIX = 0x840D
        /// </summary>
        FragmentLight1Sgix = ((int)0x840D),
        /// <summary>
        /// Original was GL_FRAGMENT_LIGHT2_SGIX = 0x840E
        /// </summary>
        FragmentLight2Sgix = ((int)0x840E),
        /// <summary>
        /// Original was GL_FRAGMENT_LIGHT3_SGIX = 0x840F
        /// </summary>
        FragmentLight3Sgix = ((int)0x840F),
        /// <summary>
        /// Original was GL_FRAGMENT_LIGHT4_SGIX = 0x8410
        /// </summary>
        FragmentLight4Sgix = ((int)0x8410),
        /// <summary>
        /// Original was GL_FRAGMENT_LIGHT5_SGIX = 0x8411
        /// </summary>
        FragmentLight5Sgix = ((int)0x8411),
        /// <summary>
        /// Original was GL_FRAGMENT_LIGHT6_SGIX = 0x8412
        /// </summary>
        FragmentLight6Sgix = ((int)0x8412),
        /// <summary>
        /// Original was GL_FRAGMENT_LIGHT7_SGIX = 0x8413
        /// </summary>
        FragmentLight7Sgix = ((int)0x8413),
        /// <summary>
        /// Original was GL_FOG_COORD_ARRAY = 0x8457
        /// </summary>
        FogCoordArray = ((int)0x8457),
        /// <summary>
        /// Original was GL_COLOR_SUM = 0x8458
        /// </summary>
        ColorSum = ((int)0x8458),
        /// <summary>
        /// Original was GL_SECONDARY_COLOR_ARRAY = 0x845E
        /// </summary>
        SecondaryColorArray = ((int)0x845E),
        /// <summary>
        /// Original was GL_TEXTURE_RECTANGLE = 0x84F5
        /// </summary>
        TextureRectangle = ((int)0x84F5),
        /// <summary>
        /// Original was GL_TEXTURE_CUBE_MAP = 0x8513
        /// </summary>
        TextureCubeMap = ((int)0x8513),
        /// <summary>
        /// Original was GL_PROGRAM_POINT_SIZE = 0x8642
        /// </summary>
        ProgramPointSize = ((int)0x8642),
        /// <summary>
        /// Original was GL_VERTEX_PROGRAM_POINT_SIZE = 0x8642
        /// </summary>
        VertexProgramPointSize = ((int)0x8642),
        /// <summary>
        /// Original was GL_VERTEX_PROGRAM_TWO_SIDE = 0x8643
        /// </summary>
        VertexProgramTwoSide = ((int)0x8643),
        /// <summary>
        /// Original was GL_DEPTH_CLAMP = 0x864F
        /// </summary>
        DepthClamp = ((int)0x864F),
        /// <summary>
        /// Original was GL_TEXTURE_CUBE_MAP_SEAMLESS = 0x884F
        /// </summary>
        TextureCubeMapSeamless = ((int)0x884F),
        /// <summary>
        /// Original was GL_POINT_SPRITE = 0x8861
        /// </summary>
        PointSprite = ((int)0x8861),
        /// <summary>
        /// Original was GL_SAMPLE_SHADING = 0x8C36
        /// </summary>
        SampleShading = ((int)0x8C36),
        /// <summary>
        /// Original was GL_RASTERIZER_DISCARD = 0x8C89
        /// </summary>
        RasterizerDiscard = ((int)0x8C89),
        /// <summary>
        /// Original was GL_PRIMITIVE_RESTART_FIXED_INDEX = 0x8D69
        /// </summary>
        PrimitiveRestartFixedIndex = ((int)0x8D69),
        /// <summary>
        /// Original was GL_FRAMEBUFFER_SRGB = 0x8DB9
        /// </summary>
        FramebufferSrgb = ((int)0x8DB9),
        /// <summary>
        /// Original was GL_SAMPLE_MASK = 0x8E51
        /// </summary>
        SampleMask = ((int)0x8E51),
        /// <summary>
        /// Original was GL_PRIMITIVE_RESTART = 0x8F9D
        /// </summary>
        PrimitiveRestart = ((int)0x8F9D),
        /// <summary>
        /// Original was GL_DEBUG_OUTPUT = 0x92E0
        /// </summary>
        DebugOutput = ((int)0x92E0),
    }
    //
    // Summary:
    //     Used in GL.FrontFace
    public enum FrontFaceDirection
    {
        //
        // Summary:
        //     Original was GL_CW = 0x0900
        Cw = 2304,
        //
        // Summary:
        //     Original was GL_CCW = 0x0901
        Ccw = 2305
    }

#if OPENTK3
    public enum InternalFormat
    {
        DepthComponent = 6402,
        Red = 6403,
        RedExt = 6403,
        Rgb = 6407,
        Rgba = 6408,
        R3G3B2 = 10768,
        Alpha4 = 32827,
        Alpha8 = 32828,
        Alpha12 = 32829,
        Alpha16 = 32830,
        Luminance4 = 32831,
        Luminance8 = 32832,
        Luminance12 = 32833,
        Luminance16 = 32834,
        Luminance4Alpha4 = 32835,
        Luminance6Alpha2 = 32836,
        Luminance8Alpha8 = 32837,
        Luminance12Alpha4 = 32838,
        Luminance12Alpha12 = 32839,
        Luminance16Alpha16 = 32840,
        Intensity = 32841,
        Intensity4 = 32842,
        Intensity8 = 32843,
        Intensity12 = 32844,
        Intensity16 = 32845,
        Rgb2Ext = 32846,
        Rgb4 = 32847,
        Rgb4Ext = 32847,
        Rgb5 = 32848,
        Rgb5Ext = 32848,
        Rgb8 = 32849,
        Rgb8Ext = 32849,
        Rgb8Oes = 32849,
        Rgb10 = 32850,
        Rgb10Ext = 32850,
        Rgb12 = 32851,
        Rgb12Ext = 32851,
        Rgb16 = 32852,
        Rgb16Ext = 32852,
        Rgba4 = 32854,
        Rgba4Ext = 32854,
        Rgba4Oes = 32854,
        Rgb5A1 = 32855,
        Rgb5A1Ext = 32855,
        Rgb5A1Oes = 32855,
        Rgba8 = 32856,
        Rgba8Ext = 32856,
        Rgba8Oes = 32856,
        Rgb10A2 = 32857,
        Rgb10A2Ext = 32857,
        Rgba12 = 32858,
        Rgba12Ext = 32858,
        Rgba16 = 32859,
        Rgba16Ext = 32859,
        DualAlpha4Sgis = 33040,
        DualAlpha8Sgis = 33041,
        DualAlpha12Sgis = 33042,
        DualAlpha16Sgis = 33043,
        DualLuminance4Sgis = 33044,
        DualLuminance8Sgis = 33045,
        DualLuminance12Sgis = 33046,
        DualLuminance16Sgis = 33047,
        DualIntensity4Sgis = 33048,
        DualIntensity8Sgis = 33049,
        DualIntensity12Sgis = 33050,
        DualIntensity16Sgis = 33051,
        DualLuminanceAlpha4Sgis = 33052,
        DualLuminanceAlpha8Sgis = 33053,
        QuadAlpha4Sgis = 33054,
        QuadAlpha8Sgis = 33055,
        QuadLuminance4Sgis = 33056,
        QuadLuminance8Sgis = 33057,
        QuadIntensity4Sgis = 33058,
        QuadIntensity8Sgis = 33059,
        DepthComponent16 = 33189,
        DepthComponent16Arb = 33189,
        DepthComponent16Oes = 33189,
        DepthComponent16Sgix = 33189,
        DepthComponent24Arb = 33190,
        DepthComponent24Oes = 33190,
        DepthComponent24Sgix = 33190,
        DepthComponent32Arb = 33191,
        DepthComponent32Oes = 33191,
        DepthComponent32Sgix = 33191,
        CompressedRed = 33317,
        CompressedRg = 33318,
        Rg = 33319,
        R8 = 33321,
        R8Ext = 33321,
        R16 = 33322,
        R16Ext = 33322,
        Rg8 = 33323,
        Rg8Ext = 33323,
        Rg16 = 33324,
        Rg16Ext = 33324,
        R16f = 33325,
        R16fExt = 33325,
        R32f = 33326,
        R32fExt = 33326,
        Rg16f = 33327,
        Rg16fExt = 33327,
        Rg32f = 33328,
        Rg32fExt = 33328,
        R8i = 33329,
        R8ui = 33330,
        R16i = 33331,
        R16ui = 33332,
        R32i = 33333,
        R32ui = 33334,
        Rg8i = 33335,
        Rg8ui = 33336,
        Rg16i = 33337,
        Rg16ui = 33338,
        Rg32i = 33339,
        Rg32ui = 33340,
        CompressedRgbS3tcDxt1Ext = 33776,
        CompressedRgbaS3tcDxt1Ext = 33777,
        CompressedRgbaS3tcDxt3Ext = 33778,
        CompressedRgbaS3tcDxt5Ext = 33779,
        CompressedRgb = 34029,
        CompressedRgba = 34030,
        DepthStencil = 34041,
        DepthStencilExt = 34041,
        DepthStencilNv = 34041,
        DepthStencilOes = 34041,
        DepthStencilMesa = 34640,
        Rgba32f = 34836,
        Rgba32fArb = 34836,
        Rgba32fExt = 34836,
        Rgba16f = 34842,
        Rgba16fArb = 34842,
        Rgba16fExt = 34842,
        Rgb16f = 34843,
        Rgb16fArb = 34843,
        Rgb16fExt = 34843,
        Depth24Stencil8 = 35056,
        Depth24Stencil8Ext = 35056,
        Depth24Stencil8Oes = 35056,
        R11fG11fB10f = 35898,
        R11fG11fB10fApple = 35898,
        R11fG11fB10fExt = 35898,
        Rgb9E5 = 35901,
        Rgb9E5Apple = 35901,
        Rgb9E5Ext = 35901,
        Srgb = 35904,
        SrgbExt = 35904,
        Srgb8 = 35905,
        Srgb8Ext = 35905,
        Srgb8Nv = 35905,
        SrgbAlpha = 35906,
        SrgbAlphaExt = 35906,
        Srgb8Alpha8 = 35907,
        Srgb8Alpha8Ext = 35907,
        CompressedSrgb = 35912,
        CompressedSrgbAlpha = 35913,
        CompressedSrgbS3tcDxt1Ext = 35916,
        CompressedSrgbAlphaS3tcDxt1Ext = 35917,
        CompressedSrgbAlphaS3tcDxt3Ext = 35918,
        CompressedSrgbAlphaS3tcDxt5Ext = 35919,
        DepthComponent32f = 36012,
        Depth32fStencil8 = 36013,
        Rgba32ui = 36208,
        Rgb32ui = 36209,
        Rgba16ui = 36214,
        Rgb16ui = 36215,
        Rgba8ui = 36220,
        Rgb8ui = 36221,
        Rgba32i = 36226,
        Rgb32i = 36227,
        Rgba16i = 36232,
        Rgb16i = 36233,
        Rgba8i = 36238,
        Rgb8i = 36239,
        DepthComponent32fNv = 36267,
        Depth32fStencil8Nv = 36268,
        CompressedRedRgtc1 = 36283,
        CompressedRedRgtc1Ext = 36283,
        CompressedSignedRedRgtc1 = 36284,
        CompressedSignedRedRgtc1Ext = 36284,
        CompressedRgRgtc2 = 36285,
        CompressedSignedRgRgtc2 = 36286,
        CompressedRgbaBptcUnorm = 36492,
        CompressedSrgbAlphaBptcUnorm = 36493,
        CompressedRgbBptcSignedFloat = 36494,
        CompressedRgbBptcUnsignedFloat = 36495,
        R8Snorm = 36756,
        Rg8Snorm = 36757,
        Rgb8Snorm = 36758,
        Rgba8Snorm = 36759,
        R16Snorm = 36760,
        R16SnormExt = 36760,
        Rg16Snorm = 36761,
        Rg16SnormExt = 36761,
        Rgb16Snorm = 36762,
        Rgb16SnormExt = 36762,
        Rgb10A2ui = 36975,
        CompressedR11Eac = 37488,
        CompressedSignedR11Eac = 37489,
        CompressedRg11Eac = 37490,
        CompressedSignedRg11Eac = 37491,
        CompressedRgb8Etc2 = 37492,
        CompressedSrgb8Etc2 = 37493,
        CompressedRgb8PunchthroughAlpha1Etc2 = 37494,
        CompressedSrgb8PunchthroughAlpha1Etc2 = 37495,
        CompressedRgba8Etc2Eac = 37496,
        CompressedSrgb8Alpha8Etc2Eac = 37497
    }
#endif

    public enum MatrixMode
    {
        Modelview = 5888,
        Projection = 5889,
        Texture = 5890,
        Color = 6144
    }
    public enum ErrorCode : int
    {
        /// <summary>
        /// Original was GL_NO_ERROR = 0
        /// </summary>
        NoError = ((int)0),
        /// <summary>
        /// Original was GL_INVALID_ENUM = 0x0500
        /// </summary>
        InvalidEnum = ((int)0x0500),
        /// <summary>
        /// Original was GL_INVALID_VALUE = 0x0501
        /// </summary>
        InvalidValue = ((int)0x0501),
        /// <summary>
        /// Original was GL_INVALID_OPERATION = 0x0502
        /// </summary>
        InvalidOperation = ((int)0x0502),
        /// <summary>
        /// Original was GL_STACK_OVERFLOW = 0x0503
        /// </summary>
        StackOverflow = ((int)0x0503),
        /// <summary>
        /// Original was GL_STACK_UNDERFLOW = 0x0504
        /// </summary>
        StackUnderflow = ((int)0x0504),
        /// <summary>
        /// Original was GL_OUT_OF_MEMORY = 0x0505
        /// </summary>
        OutOfMemory = ((int)0x0505),
        /// <summary>
        /// Original was GL_INVALID_FRAMEBUFFER_OPERATION = 0x0506
        /// </summary>
        InvalidFramebufferOperation = ((int)0x0506),
        /// <summary>
        /// Original was GL_INVALID_FRAMEBUFFER_OPERATION_EXT = 0x0506
        /// </summary>
        InvalidFramebufferOperationExt = ((int)0x0506),
        /// <summary>
        /// Original was GL_INVALID_FRAMEBUFFER_OPERATION_OES = 0x0506
        /// </summary>
        InvalidFramebufferOperationOes = ((int)0x0506),
        /// <summary>
        /// Original was GL_CONTEXT_LOST = 0x0507
        /// </summary>
        ContextLost = ((int)0x0507),
        /// <summary>
        /// Original was GL_TABLE_TOO_LARGE = 0x8031
        /// </summary>
        TableTooLarge = ((int)0x8031),
        /// <summary>
        /// Original was GL_TABLE_TOO_LARGE_EXT = 0x8031
        /// </summary>
        TableTooLargeExt = ((int)0x8031),
        /// <summary>
        /// Original was GL_TEXTURE_TOO_LARGE_EXT = 0x8065
        /// </summary>
        TextureTooLargeExt = ((int)0x8065),
    }
    //public enum GetPName : int
    //{
    //    /// <summary>
    //    /// Original was GL_CURRENT_COLOR = 0x0B00
    //    /// </summary>
    //    CurrentColor = ((int)0x0B00),
    //    /// <summary>
    //    /// Original was GL_CURRENT_INDEX = 0x0B01
    //    /// </summary>
    //    CurrentIndex = ((int)0x0B01),
    //    /// <summary>
    //    /// Original was GL_CURRENT_NORMAL = 0x0B02
    //    /// </summary>
    //    CurrentNormal = ((int)0x0B02),
    //    /// <summary>
    //    /// Original was GL_CURRENT_TEXTURE_COORDS = 0x0B03
    //    /// </summary>
    //    CurrentTextureCoords = ((int)0x0B03),
    //    /// <summary>
    //    /// Original was GL_CURRENT_RASTER_COLOR = 0x0B04
    //    /// </summary>
    //    CurrentRasterColor = ((int)0x0B04),
    //    /// <summary>
    //    /// Original was GL_CURRENT_RASTER_INDEX = 0x0B05
    //    /// </summary>
    //    CurrentRasterIndex = ((int)0x0B05),
    //    /// <summary>
    //    /// Original was GL_CURRENT_RASTER_TEXTURE_COORDS = 0x0B06
    //    /// </summary>
    //    CurrentRasterTextureCoords = ((int)0x0B06),
    //    /// <summary>
    //    /// Original was GL_CURRENT_RASTER_POSITION = 0x0B07
    //    /// </summary>
    //    CurrentRasterPosition = ((int)0x0B07),
    //    /// <summary>
    //    /// Original was GL_CURRENT_RASTER_POSITION_VALID = 0x0B08
    //    /// </summary>
    //    CurrentRasterPositionValid = ((int)0x0B08),
    //    /// <summary>
    //    /// Original was GL_CURRENT_RASTER_DISTANCE = 0x0B09
    //    /// </summary>
    //    CurrentRasterDistance = ((int)0x0B09),
    //    /// <summary>
    //    /// Original was GL_POINT_SMOOTH = 0x0B10
    //    /// </summary>
    //    PointSmooth = ((int)0x0B10),
    //    /// <summary>
    //    /// Original was GL_POINT_SIZE = 0x0B11
    //    /// </summary>
    //    PointSize = ((int)0x0B11),
    //    /// <summary>
    //    /// Original was GL_POINT_SIZE_RANGE = 0x0B12
    //    /// </summary>
    //    PointSizeRange = ((int)0x0B12),
    //    /// <summary>
    //    /// Original was GL_SMOOTH_POINT_SIZE_RANGE = 0x0B12
    //    /// </summary>
    //    SmoothPointSizeRange = ((int)0x0B12),
    //    /// <summary>
    //    /// Original was GL_POINT_SIZE_GRANULARITY = 0x0B13
    //    /// </summary>
    //    PointSizeGranularity = ((int)0x0B13),
    //    /// <summary>
    //    /// Original was GL_SMOOTH_POINT_SIZE_GRANULARITY = 0x0B13
    //    /// </summary>
    //    SmoothPointSizeGranularity = ((int)0x0B13),
    //    /// <summary>
    //    /// Original was GL_LINE_SMOOTH = 0x0B20
    //    /// </summary>
    //    LineSmooth = ((int)0x0B20),
    //    /// <summary>
    //    /// Original was GL_LINE_WIDTH = 0x0B21
    //    /// </summary>
    //    LineWidth = ((int)0x0B21),
    //    /// <summary>
    //    /// Original was GL_LINE_WIDTH_RANGE = 0x0B22
    //    /// </summary>
    //    LineWidthRange = ((int)0x0B22),
    //    /// <summary>
    //    /// Original was GL_SMOOTH_LINE_WIDTH_RANGE = 0x0B22
    //    /// </summary>
    //    SmoothLineWidthRange = ((int)0x0B22),
    //    /// <summary>
    //    /// Original was GL_LINE_WIDTH_GRANULARITY = 0x0B23
    //    /// </summary>
    //    LineWidthGranularity = ((int)0x0B23),
    //    /// <summary>
    //    /// Original was GL_SMOOTH_LINE_WIDTH_GRANULARITY = 0x0B23
    //    /// </summary>
    //    SmoothLineWidthGranularity = ((int)0x0B23),
    //    /// <summary>
    //    /// Original was GL_LINE_STIPPLE = 0x0B24
    //    /// </summary>
    //    LineStipple = ((int)0x0B24),
    //    /// <summary>
    //    /// Original was GL_LINE_STIPPLE_PATTERN = 0x0B25
    //    /// </summary>
    //    LineStipplePattern = ((int)0x0B25),
    //    /// <summary>
    //    /// Original was GL_LINE_STIPPLE_REPEAT = 0x0B26
    //    /// </summary>
    //    LineStippleRepeat = ((int)0x0B26),
    //    /// <summary>
    //    /// Original was GL_LIST_MODE = 0x0B30
    //    /// </summary>
    //    ListMode = ((int)0x0B30),
    //    /// <summary>
    //    /// Original was GL_MAX_LIST_NESTING = 0x0B31
    //    /// </summary>
    //    MaxListNesting = ((int)0x0B31),
    //    /// <summary>
    //    /// Original was GL_LIST_BASE = 0x0B32
    //    /// </summary>
    //    ListBase = ((int)0x0B32),
    //    /// <summary>
    //    /// Original was GL_LIST_INDEX = 0x0B33
    //    /// </summary>
    //    ListIndex = ((int)0x0B33),
    //    /// <summary>
    //    /// Original was GL_POLYGON_MODE = 0x0B40
    //    /// </summary>
    //    PolygonMode = ((int)0x0B40),
    //    /// <summary>
    //    /// Original was GL_POLYGON_SMOOTH = 0x0B41
    //    /// </summary>
    //    PolygonSmooth = ((int)0x0B41),
    //    /// <summary>
    //    /// Original was GL_POLYGON_STIPPLE = 0x0B42
    //    /// </summary>
    //    PolygonStipple = ((int)0x0B42),
    //    /// <summary>
    //    /// Original was GL_EDGE_FLAG = 0x0B43
    //    /// </summary>
    //    EdgeFlag = ((int)0x0B43),
    //    /// <summary>
    //    /// Original was GL_CULL_FACE = 0x0B44
    //    /// </summary>
    //    CullFace = ((int)0x0B44),
    //    /// <summary>
    //    /// Original was GL_CULL_FACE_MODE = 0x0B45
    //    /// </summary>
    //    CullFaceMode = ((int)0x0B45),
    //    /// <summary>
    //    /// Original was GL_FRONT_FACE = 0x0B46
    //    /// </summary>
    //    FrontFace = ((int)0x0B46),
    //    /// <summary>
    //    /// Original was GL_LIGHTING = 0x0B50
    //    /// </summary>
    //    Lighting = ((int)0x0B50),
    //    /// <summary>
    //    /// Original was GL_LIGHT_MODEL_LOCAL_VIEWER = 0x0B51
    //    /// </summary>
    //    LightModelLocalViewer = ((int)0x0B51),
    //    /// <summary>
    //    /// Original was GL_LIGHT_MODEL_TWO_SIDE = 0x0B52
    //    /// </summary>
    //    LightModelTwoSide = ((int)0x0B52),
    //    /// <summary>
    //    /// Original was GL_LIGHT_MODEL_AMBIENT = 0x0B53
    //    /// </summary>
    //    LightModelAmbient = ((int)0x0B53),
    //    /// <summary>
    //    /// Original was GL_SHADE_MODEL = 0x0B54
    //    /// </summary>
    //    ShadeModel = ((int)0x0B54),
    //    /// <summary>
    //    /// Original was GL_COLOR_MATERIAL_FACE = 0x0B55
    //    /// </summary>
    //    ColorMaterialFace = ((int)0x0B55),
    //    /// <summary>
    //    /// Original was GL_COLOR_MATERIAL_PARAMETER = 0x0B56
    //    /// </summary>
    //    ColorMaterialParameter = ((int)0x0B56),
    //    /// <summary>
    //    /// Original was GL_COLOR_MATERIAL = 0x0B57
    //    /// </summary>
    //    ColorMaterial = ((int)0x0B57),
    //    /// <summary>
    //    /// Original was GL_FOG = 0x0B60
    //    /// </summary>
    //    Fog = ((int)0x0B60),
    //    /// <summary>
    //    /// Original was GL_FOG_INDEX = 0x0B61
    //    /// </summary>
    //    FogIndex = ((int)0x0B61),
    //    /// <summary>
    //    /// Original was GL_FOG_DENSITY = 0x0B62
    //    /// </summary>
    //    FogDensity = ((int)0x0B62),
    //    /// <summary>
    //    /// Original was GL_FOG_START = 0x0B63
    //    /// </summary>
    //    FogStart = ((int)0x0B63),
    //    /// <summary>
    //    /// Original was GL_FOG_END = 0x0B64
    //    /// </summary>
    //    FogEnd = ((int)0x0B64),
    //    /// <summary>
    //    /// Original was GL_FOG_MODE = 0x0B65
    //    /// </summary>
    //    FogMode = ((int)0x0B65),
    //    /// <summary>
    //    /// Original was GL_FOG_COLOR = 0x0B66
    //    /// </summary>
    //    FogColor = ((int)0x0B66),
    //    /// <summary>
    //    /// Original was GL_DEPTH_RANGE = 0x0B70
    //    /// </summary>
    //    DepthRange = ((int)0x0B70),
    //    /// <summary>
    //    /// Original was GL_DEPTH_TEST = 0x0B71
    //    /// </summary>
    //    DepthTest = ((int)0x0B71),
    //    /// <summary>
    //    /// Original was GL_DEPTH_WRITEMASK = 0x0B72
    //    /// </summary>
    //    DepthWritemask = ((int)0x0B72),
    //    /// <summary>
    //    /// Original was GL_DEPTH_CLEAR_VALUE = 0x0B73
    //    /// </summary>
    //    DepthClearValue = ((int)0x0B73),
    //    /// <summary>
    //    /// Original was GL_DEPTH_FUNC = 0x0B74
    //    /// </summary>
    //    DepthFunc = ((int)0x0B74),
    //    /// <summary>
    //    /// Original was GL_ACCUM_CLEAR_VALUE = 0x0B80
    //    /// </summary>
    //    AccumClearValue = ((int)0x0B80),
    //    /// <summary>
    //    /// Original was GL_STENCIL_TEST = 0x0B90
    //    /// </summary>
    //    StencilTest = ((int)0x0B90),
    //    /// <summary>
    //    /// Original was GL_STENCIL_CLEAR_VALUE = 0x0B91
    //    /// </summary>
    //    StencilClearValue = ((int)0x0B91),
    //    /// <summary>
    //    /// Original was GL_STENCIL_FUNC = 0x0B92
    //    /// </summary>
    //    StencilFunc = ((int)0x0B92),
    //    /// <summary>
    //    /// Original was GL_STENCIL_VALUE_MASK = 0x0B93
    //    /// </summary>
    //    StencilValueMask = ((int)0x0B93),
    //    /// <summary>
    //    /// Original was GL_STENCIL_FAIL = 0x0B94
    //    /// </summary>
    //    StencilFail = ((int)0x0B94),
    //    /// <summary>
    //    /// Original was GL_STENCIL_PASS_DEPTH_FAIL = 0x0B95
    //    /// </summary>
    //    StencilPassDepthFail = ((int)0x0B95),
    //    /// <summary>
    //    /// Original was GL_STENCIL_PASS_DEPTH_PASS = 0x0B96
    //    /// </summary>
    //    StencilPassDepthPass = ((int)0x0B96),
    //    /// <summary>
    //    /// Original was GL_STENCIL_REF = 0x0B97
    //    /// </summary>
    //    StencilRef = ((int)0x0B97),
    //    /// <summary>
    //    /// Original was GL_STENCIL_WRITEMASK = 0x0B98
    //    /// </summary>
    //    StencilWritemask = ((int)0x0B98),
    //    /// <summary>
    //    /// Original was GL_MATRIX_MODE = 0x0BA0
    //    /// </summary>
    //    MatrixMode = ((int)0x0BA0),
    //    /// <summary>
    //    /// Original was GL_NORMALIZE = 0x0BA1
    //    /// </summary>
    //    Normalize = ((int)0x0BA1),
    //    /// <summary>
    //    /// Original was GL_VIEWPORT = 0x0BA2
    //    /// </summary>
    //    Viewport = ((int)0x0BA2),
    //    /// <summary>
    //    /// Original was GL_MODELVIEW0_STACK_DEPTH_EXT = 0x0BA3
    //    /// </summary>
    //    Modelview0StackDepthExt = ((int)0x0BA3),
    //    /// <summary>
    //    /// Original was GL_MODELVIEW_STACK_DEPTH = 0x0BA3
    //    /// </summary>
    //    ModelviewStackDepth = ((int)0x0BA3),
    //    /// <summary>
    //    /// Original was GL_PROJECTION_STACK_DEPTH = 0x0BA4
    //    /// </summary>
    //    ProjectionStackDepth = ((int)0x0BA4),
    //    /// <summary>
    //    /// Original was GL_TEXTURE_STACK_DEPTH = 0x0BA5
    //    /// </summary>
    //    TextureStackDepth = ((int)0x0BA5),
    //    /// <summary>
    //    /// Original was GL_MODELVIEW0_MATRIX_EXT = 0x0BA6
    //    /// </summary>
    //    //Modelview0MatrixExt = ((int)0x0BA6),
    //    /// <summary>
    //    /// Original was GL_MODELVIEW_MATRIX = 0x0BA6
    //    /// </summary>
    //    ModelviewMatrix = ((int)0x0BA6),
    //    /// <summary>
    //    /// Original was GL_PROJECTION_MATRIX = 0x0BA7
    //    /// </summary>
    //    ProjectionMatrix = ((int)0x0BA7),
    //    /// <summary>
    //    /// Original was GL_TEXTURE_MATRIX = 0x0BA8
    //    /// </summary>
    //    TextureMatrix = ((int)0x0BA8),
    //    /// <summary>
    //    /// Original was GL_ATTRIB_STACK_DEPTH = 0x0BB0
    //    /// </summary>
    //    AttribStackDepth = ((int)0x0BB0),
    //    /// <summary>
    //    /// Original was GL_CLIENT_ATTRIB_STACK_DEPTH = 0x0BB1
    //    /// </summary>
    //    ClientAttribStackDepth = ((int)0x0BB1),
    //    /// <summary>
    //    /// Original was GL_ALPHA_TEST = 0x0BC0
    //    /// </summary>
    //    AlphaTest = ((int)0x0BC0),
    //    /// <summary>
    //    /// Original was GL_ALPHA_TEST_QCOM = 0x0BC0
    //    /// </summary>
    //    AlphaTestQcom = ((int)0x0BC0),
    //    /// <summary>
    //    /// Original was GL_ALPHA_TEST_FUNC = 0x0BC1
    //    /// </summary>
    //    AlphaTestFunc = ((int)0x0BC1),
    //    /// <summary>
    //    /// Original was GL_ALPHA_TEST_FUNC_QCOM = 0x0BC1
    //    /// </summary>
    //    AlphaTestFuncQcom = ((int)0x0BC1),
    //    /// <summary>
    //    /// Original was GL_ALPHA_TEST_REF = 0x0BC2
    //    /// </summary>
    //    AlphaTestRef = ((int)0x0BC2),
    //    /// <summary>
    //    /// Original was GL_ALPHA_TEST_REF_QCOM = 0x0BC2
    //    /// </summary>
    //    AlphaTestRefQcom = ((int)0x0BC2),
    //    /// <summary>
    //    /// Original was GL_DITHER = 0x0BD0
    //    /// </summary>
    //    Dither = ((int)0x0BD0),
    //    /// <summary>
    //    /// Original was GL_BLEND_DST = 0x0BE0
    //    /// </summary>
    //    BlendDst = ((int)0x0BE0),
    //    /// <summary>
    //    /// Original was GL_BLEND_SRC = 0x0BE1
    //    /// </summary>
    //    BlendSrc = ((int)0x0BE1),
    //    /// <summary>
    //    /// Original was GL_BLEND = 0x0BE2
    //    /// </summary>
    //    Blend = ((int)0x0BE2),
    //    /// <summary>
    //    /// Original was GL_LOGIC_OP_MODE = 0x0BF0
    //    /// </summary>
    //    LogicOpMode = ((int)0x0BF0),
    //    /// <summary>
    //    /// Original was GL_INDEX_LOGIC_OP = 0x0BF1
    //    /// </summary>
    //    IndexLogicOp = ((int)0x0BF1),
    //    /// <summary>
    //    /// Original was GL_LOGIC_OP = 0x0BF1
    //    /// </summary>
    //    LogicOp = ((int)0x0BF1),
    //    /// <summary>
    //    /// Original was GL_COLOR_LOGIC_OP = 0x0BF2
    //    /// </summary>
    //    ColorLogicOp = ((int)0x0BF2),
    //    /// <summary>
    //    /// Original was GL_AUX_BUFFERS = 0x0C00
    //    /// </summary>
    //    AuxBuffers = ((int)0x0C00),
    //    /// <summary>
    //    /// Original was GL_DRAW_BUFFER = 0x0C01
    //    /// </summary>
    //    DrawBuffer = ((int)0x0C01),
    //    /// <summary>
    //    /// Original was GL_DRAW_BUFFER_EXT = 0x0C01
    //    /// </summary>
    //    DrawBufferExt = ((int)0x0C01),
    //    /// <summary>
    //    /// Original was GL_READ_BUFFER = 0x0C02
    //    /// </summary>
    //    ReadBuffer = ((int)0x0C02),
    //    /// <summary>
    //    /// Original was GL_READ_BUFFER_EXT = 0x0C02
    //    /// </summary>
    //    ReadBufferExt = ((int)0x0C02),
    //    /// <summary>
    //    /// Original was GL_READ_BUFFER_NV = 0x0C02
    //    /// </summary>
    //    ReadBufferNv = ((int)0x0C02),
    //    /// <summary>
    //    /// Original was GL_SCISSOR_BOX = 0x0C10
    //    /// </summary>
    //    ScissorBox = ((int)0x0C10),
    //    /// <summary>
    //    /// Original was GL_SCISSOR_TEST = 0x0C11
    //    /// </summary>
    //    ScissorTest = ((int)0x0C11),
    //    /// <summary>
    //    /// Original was GL_INDEX_CLEAR_VALUE = 0x0C20
    //    /// </summary>
    //    IndexClearValue = ((int)0x0C20),
    //    /// <summary>
    //    /// Original was GL_INDEX_WRITEMASK = 0x0C21
    //    /// </summary>
    //    IndexWritemask = ((int)0x0C21),
    //    /// <summary>
    //    /// Original was GL_COLOR_CLEAR_VALUE = 0x0C22
    //    /// </summary>
    //    ColorClearValue = ((int)0x0C22),
    //    /// <summary>
    //    /// Original was GL_COLOR_WRITEMASK = 0x0C23
    //    /// </summary>
    //    ColorWritemask = ((int)0x0C23),
    //    /// <summary>
    //    /// Original was GL_INDEX_MODE = 0x0C30
    //    /// </summary>
    //    IndexMode = ((int)0x0C30),
    //    /// <summary>
    //    /// Original was GL_RGBA_MODE = 0x0C31
    //    /// </summary>
    //    RgbaMode = ((int)0x0C31),
    //    /// <summary>
    //    /// Original was GL_DOUBLEBUFFER = 0x0C32
    //    /// </summary>
    //    Doublebuffer = ((int)0x0C32),
    //    /// <summary>
    //    /// Original was GL_STEREO = 0x0C33
    //    /// </summary>
    //    Stereo = ((int)0x0C33),
    //    /// <summary>
    //    /// Original was GL_RENDER_MODE = 0x0C40
    //    /// </summary>
    //    RenderMode = ((int)0x0C40),
    //    /// <summary>
    //    /// Original was GL_PERSPECTIVE_CORRECTION_HINT = 0x0C50
    //    /// </summary>
    //    PerspectiveCorrectionHint = ((int)0x0C50),
    //    /// <summary>
    //    /// Original was GL_POINT_SMOOTH_HINT = 0x0C51
    //    /// </summary>
    //    PointSmoothHint = ((int)0x0C51),
    //    /// <summary>
    //    /// Original was GL_LINE_SMOOTH_HINT = 0x0C52
    //    /// </summary>
    //    LineSmoothHint = ((int)0x0C52),
    //    /// <summary>
    //    /// Original was GL_POLYGON_SMOOTH_HINT = 0x0C53
    //    /// </summary>
    //    PolygonSmoothHint = ((int)0x0C53),
    //    /// <summary>
    //    /// Original was GL_FOG_HINT = 0x0C54
    //    /// </summary>
    //    FogHint = ((int)0x0C54),
    //    /// <summary>
    //    /// Original was GL_TEXTURE_GEN_S = 0x0C60
    //    /// </summary>
    //    TextureGenS = ((int)0x0C60),
    //    /// <summary>
    //    /// Original was GL_TEXTURE_GEN_T = 0x0C61
    //    /// </summary>
    //    TextureGenT = ((int)0x0C61),
    //    /// <summary>
    //    /// Original was GL_TEXTURE_GEN_R = 0x0C62
    //    /// </summary>
    //    TextureGenR = ((int)0x0C62),
    //    /// <summary>
    //    /// Original was GL_TEXTURE_GEN_Q = 0x0C63
    //    /// </summary>
    //    TextureGenQ = ((int)0x0C63),
    //    /// <summary>
    //    /// Original was GL_PIXEL_MAP_I_TO_I_SIZE = 0x0CB0
    //    /// </summary>
    //    PixelMapIToISize = ((int)0x0CB0),
    //    /// <summary>
    //    /// Original was GL_PIXEL_MAP_S_TO_S_SIZE = 0x0CB1
    //    /// </summary>
    //    PixelMapSToSSize = ((int)0x0CB1),
    //    /// <summary>
    //    /// Original was GL_PIXEL_MAP_I_TO_R_SIZE = 0x0CB2
    //    /// </summary>
    //    PixelMapIToRSize = ((int)0x0CB2),
    //    /// <summary>
    //    /// Original was GL_PIXEL_MAP_I_TO_G_SIZE = 0x0CB3
    //    /// </summary>
    //    PixelMapIToGSize = ((int)0x0CB3),
    //    /// <summary>
    //    /// Original was GL_PIXEL_MAP_I_TO_B_SIZE = 0x0CB4
    //    /// </summary>
    //    PixelMapIToBSize = ((int)0x0CB4),
    //    /// <summary>
    //    /// Original was GL_PIXEL_MAP_I_TO_A_SIZE = 0x0CB5
    //    /// </summary>
    //    PixelMapIToASize = ((int)0x0CB5),
    //    /// <summary>
    //    /// Original was GL_PIXEL_MAP_R_TO_R_SIZE = 0x0CB6
    //    /// </summary>
    //    PixelMapRToRSize = ((int)0x0CB6),
    //    /// <summary>
    //    /// Original was GL_PIXEL_MAP_G_TO_G_SIZE = 0x0CB7
    //    /// </summary>
    //    PixelMapGToGSize = ((int)0x0CB7),
    //    /// <summary>
    //    /// Original was GL_PIXEL_MAP_B_TO_B_SIZE = 0x0CB8
    //    /// </summary>
    //    PixelMapBToBSize = ((int)0x0CB8),
    //    /// <summary>
    //    /// Original was GL_PIXEL_MAP_A_TO_A_SIZE = 0x0CB9
    //    /// </summary>
    //    PixelMapAToASize = ((int)0x0CB9),
    //    /// <summary>
    //    /// Original was GL_UNPACK_SWAP_BYTES = 0x0CF0
    //    /// </summary>
    //    UnpackSwapBytes = ((int)0x0CF0),
    //    /// <summary>
    //    /// Original was GL_UNPACK_LSB_FIRST = 0x0CF1
    //    /// </summary>
    //    UnpackLsbFirst = ((int)0x0CF1),
    //    /// <summary>
    //    /// Original was GL_UNPACK_ROW_LENGTH = 0x0CF2
    //    /// </summary>
    //    UnpackRowLength = ((int)0x0CF2),
    //    /// <summary>
    //    /// Original was GL_UNPACK_SKIP_ROWS = 0x0CF3
    //    /// </summary>
    //    UnpackSkipRows = ((int)0x0CF3),
    //    /// <summary>
    //    /// Original was GL_UNPACK_SKIP_PIXELS = 0x0CF4
    //    /// </summary>
    //    UnpackSkipPixels = ((int)0x0CF4),
    //    /// <summary>
    //    /// Original was GL_UNPACK_ALIGNMENT = 0x0CF5
    //    /// </summary>
    //    UnpackAlignment = ((int)0x0CF5),
    //    /// <summary>
    //    /// Original was GL_PACK_SWAP_BYTES = 0x0D00
    //    /// </summary>
    //    PackSwapBytes = ((int)0x0D00),
    //    /// <summary>
    //    /// Original was GL_PACK_LSB_FIRST = 0x0D01
    //    /// </summary>
    //    PackLsbFirst = ((int)0x0D01),
    //    /// <summary>
    //    /// Original was GL_PACK_ROW_LENGTH = 0x0D02
    //    /// </summary>
    //    PackRowLength = ((int)0x0D02),
    //    /// <summary>
    //    /// Original was GL_PACK_SKIP_ROWS = 0x0D03
    //    /// </summary>
    //    PackSkipRows = ((int)0x0D03),
    //    /// <summary>
    //    /// Original was GL_PACK_SKIP_PIXELS = 0x0D04
    //    /// </summary>
    //    PackSkipPixels = ((int)0x0D04),
    //    /// <summary>
    //    /// Original was GL_PACK_ALIGNMENT = 0x0D05
    //    /// </summary>
    //    PackAlignment = ((int)0x0D05),
    //    /// <summary>
    //    /// Original was GL_MAP_COLOR = 0x0D10
    //    /// </summary>
    //    MapColor = ((int)0x0D10),
    //    /// <summary>
    //    /// Original was GL_MAP_STENCIL = 0x0D11
    //    /// </summary>
    //    MapStencil = ((int)0x0D11),
    //    /// <summary>
    //    /// Original was GL_INDEX_SHIFT = 0x0D12
    //    /// </summary>
    //    IndexShift = ((int)0x0D12),
    //    /// <summary>
    //    /// Original was GL_INDEX_OFFSET = 0x0D13
    //    /// </summary>
    //    IndexOffset = ((int)0x0D13),
    //    /// <summary>
    //    /// Original was GL_RED_SCALE = 0x0D14
    //    /// </summary>
    //    RedScale = ((int)0x0D14),
    //    /// <summary>
    //    /// Original was GL_RED_BIAS = 0x0D15
    //    /// </summary>
    //    RedBias = ((int)0x0D15),
    //    /// <summary>
    //    /// Original was GL_ZOOM_X = 0x0D16
    //    /// </summary>
    //    ZoomX = ((int)0x0D16),
    //    /// <summary>
    //    /// Original was GL_ZOOM_Y = 0x0D17
    //    /// </summary>
    //    ZoomY = ((int)0x0D17),
    //    /// <summary>
    //    /// Original was GL_GREEN_SCALE = 0x0D18
    //    /// </summary>
    //    GreenScale = ((int)0x0D18),
    //    /// <summary>
    //    /// Original was GL_GREEN_BIAS = 0x0D19
    //    /// </summary>
    //    GreenBias = ((int)0x0D19),
    //    /// <summary>
    //    /// Original was GL_BLUE_SCALE = 0x0D1A
    //    /// </summary>
    //    BlueScale = ((int)0x0D1A),
    //    /// <summary>
    //    /// Original was GL_BLUE_BIAS = 0x0D1B
    //    /// </summary>
    //    BlueBias = ((int)0x0D1B),
    //    /// <summary>
    //    /// Original was GL_ALPHA_SCALE = 0x0D1C
    //    /// </summary>
    //    AlphaScale = ((int)0x0D1C),
    //    /// <summary>
    //    /// Original was GL_ALPHA_BIAS = 0x0D1D
    //    /// </summary>
    //    AlphaBias = ((int)0x0D1D),
    //    /// <summary>
    //    /// Original was GL_DEPTH_SCALE = 0x0D1E
    //    /// </summary>
    //    DepthScale = ((int)0x0D1E),
    //    /// <summary>
    //    /// Original was GL_DEPTH_BIAS = 0x0D1F
    //    /// </summary>
    //    DepthBias = ((int)0x0D1F),
    //    /// <summary>
    //    /// Original was GL_MAX_EVAL_ORDER = 0x0D30
    //    /// </summary>
    //    MaxEvalOrder = ((int)0x0D30),
    //    /// <summary>
    //    /// Original was GL_MAX_LIGHTS = 0x0D31
    //    /// </summary>
    //    MaxLights = ((int)0x0D31),
    //    /// <summary>
    //    /// Original was GL_MAX_CLIP_DISTANCES = 0x0D32
    //    /// </summary>
    //    MaxClipDistances = ((int)0x0D32),
    //    /// <summary>
    //    /// Original was GL_MAX_CLIP_PLANES = 0x0D32
    //    /// </summary>
    //    MaxClipPlanes = ((int)0x0D32),
    //    /// <summary>
    //    /// Original was GL_MAX_TEXTURE_SIZE = 0x0D33
    //    /// </summary>
    //    MaxTextureSize = ((int)0x0D33),
    //    /// <summary>
    //    /// Original was GL_MAX_PIXEL_MAP_TABLE = 0x0D34
    //    /// </summary>
    //    MaxPixelMapTable = ((int)0x0D34),
    //    /// <summary>
    //    /// Original was GL_MAX_ATTRIB_STACK_DEPTH = 0x0D35
    //    /// </summary>
    //    MaxAttribStackDepth = ((int)0x0D35),
    //    /// <summary>
    //    /// Original was GL_MAX_MODELVIEW_STACK_DEPTH = 0x0D36
    //    /// </summary>
    //    MaxModelviewStackDepth = ((int)0x0D36),
    //    /// <summary>
    //    /// Original was GL_MAX_NAME_STACK_DEPTH = 0x0D37
    //    /// </summary>
    //    MaxNameStackDepth = ((int)0x0D37),
    //    /// <summary>
    //    /// Original was GL_MAX_PROJECTION_STACK_DEPTH = 0x0D38
    //    /// </summary>
    //    MaxProjectionStackDepth = ((int)0x0D38),
    //    /// <summary>
    //    /// Original was GL_MAX_TEXTURE_STACK_DEPTH = 0x0D39
    //    /// </summary>
    //    MaxTextureStackDepth = ((int)0x0D39),
    //    /// <summary>
    //    /// Original was GL_MAX_VIEWPORT_DIMS = 0x0D3A
    //    /// </summary>
    //    MaxViewportDims = ((int)0x0D3A),
    //    /// <summary>
    //    /// Original was GL_MAX_CLIENT_ATTRIB_STACK_DEPTH = 0x0D3B
    //    /// </summary>
    //    MaxClientAttribStackDepth = ((int)0x0D3B),
    //    /// <summary>
    //    /// Original was GL_SUBPIXEL_BITS = 0x0D50
    //    /// </summary>
    //    SubpixelBits = ((int)0x0D50),
    //    /// <summary>
    //    /// Original was GL_INDEX_BITS = 0x0D51
    //    /// </summary>
    //    IndexBits = ((int)0x0D51),
    //    /// <summary>
    //    /// Original was GL_RED_BITS = 0x0D52
    //    /// </summary>
    //    RedBits = ((int)0x0D52),
    //    /// <summary>
    //    /// Original was GL_GREEN_BITS = 0x0D53
    //    /// </summary>
    //    GreenBits = ((int)0x0D53),
    //    /// <summary>
    //    /// Original was GL_BLUE_BITS = 0x0D54
    //    /// </summary>
    //    BlueBits = ((int)0x0D54),
    //    /// <summary>
    //    /// Original was GL_ALPHA_BITS = 0x0D55
    //    /// </summary>
    //    AlphaBits = ((int)0x0D55),
    //    /// <summary>
    //    /// Original was GL_DEPTH_BITS = 0x0D56
    //    /// </summary>
    //    DepthBits = ((int)0x0D56),
    //    /// <summary>
    //    /// Original was GL_STENCIL_BITS = 0x0D57
    //    /// </summary>
    //    StencilBits = ((int)0x0D57),
    //    /// <summary>
    //    /// Original was GL_ACCUM_RED_BITS = 0x0D58
    //    /// </summary>
    //    AccumRedBits = ((int)0x0D58),
    //    /// <summary>
    //    /// Original was GL_ACCUM_GREEN_BITS = 0x0D59
    //    /// </summary>
    //    AccumGreenBits = ((int)0x0D59),
    //    /// <summary>
    //    /// Original was GL_ACCUM_BLUE_BITS = 0x0D5A
    //    /// </summary>
    //    AccumBlueBits = ((int)0x0D5A),
    //    /// <summary>
    //    /// Original was GL_ACCUM_ALPHA_BITS = 0x0D5B
    //    /// </summary>
    //    AccumAlphaBits = ((int)0x0D5B),
    //    /// <summary>
    //    /// Original was GL_NAME_STACK_DEPTH = 0x0D70
    //    /// </summary>
    //    NameStackDepth = ((int)0x0D70),
    //    /// <summary>
    //    /// Original was GL_AUTO_NORMAL = 0x0D80
    //    /// </summary>
    //    AutoNormal = ((int)0x0D80),
    //    /// <summary>
    //    /// Original was GL_MAP1_COLOR_4 = 0x0D90
    //    /// </summary>
    //    Map1Color4 = ((int)0x0D90),
    //    /// <summary>
    //    /// Original was GL_MAP1_INDEX = 0x0D91
    //    /// </summary>
    //    Map1Index = ((int)0x0D91),
    //    /// <summary>
    //    /// Original was GL_MAP1_NORMAL = 0x0D92
    //    /// </summary>
    //    Map1Normal = ((int)0x0D92),
    //    /// <summary>
    //    /// Original was GL_MAP1_TEXTURE_COORD_1 = 0x0D93
    //    /// </summary>
    //    Map1TextureCoord1 = ((int)0x0D93),
    //    /// <summary>
    //    /// Original was GL_MAP1_TEXTURE_COORD_2 = 0x0D94
    //    /// </summary>
    //    Map1TextureCoord2 = ((int)0x0D94),
    //    /// <summary>
    //    /// Original was GL_MAP1_TEXTURE_COORD_3 = 0x0D95
    //    /// </summary>
    //    Map1TextureCoord3 = ((int)0x0D95),
    //    /// <summary>
    //    /// Original was GL_MAP1_TEXTURE_COORD_4 = 0x0D96
    //    /// </summary>
    //    Map1TextureCoord4 = ((int)0x0D96),
    //    /// <summary>
    //    /// Original was GL_MAP1_VERTEX_3 = 0x0D97
    //    /// </summary>
    //    Map1Vertex3 = ((int)0x0D97),
    //    /// <summary>
    //    /// Original was GL_MAP1_VERTEX_4 = 0x0D98
    //    /// </summary>
    //    Map1Vertex4 = ((int)0x0D98),
    //    /// <summary>
    //    /// Original was GL_MAP2_COLOR_4 = 0x0DB0
    //    /// </summary>
    //    Map2Color4 = ((int)0x0DB0),
    //    /// <summary>
    //    /// Original was GL_MAP2_INDEX = 0x0DB1
    //    /// </summary>
    //    Map2Index = ((int)0x0DB1),
    //    /// <summary>
    //    /// Original was GL_MAP2_NORMAL = 0x0DB2
    //    /// </summary>
    //    Map2Normal = ((int)0x0DB2),
    //    /// <summary>
    //    /// Original was GL_MAP2_TEXTURE_COORD_1 = 0x0DB3
    //    /// </summary>
    //    Map2TextureCoord1 = ((int)0x0DB3),
    //    /// <summary>
    //    /// Original was GL_MAP2_TEXTURE_COORD_2 = 0x0DB4
    //    /// </summary>
    //    Map2TextureCoord2 = ((int)0x0DB4),
    //    /// <summary>
    //    /// Original was GL_MAP2_TEXTURE_COORD_3 = 0x0DB5
    //    /// </summary>
    //    Map2TextureCoord3 = ((int)0x0DB5),
    //    /// <summary>
    //    /// Original was GL_MAP2_TEXTURE_COORD_4 = 0x0DB6
    //    /// </summary>
    //    Map2TextureCoord4 = ((int)0x0DB6),
    //    /// <summary>
    //    /// Original was GL_MAP2_VERTEX_3 = 0x0DB7
    //    /// </summary>
    //    Map2Vertex3 = ((int)0x0DB7),
    //    /// <summary>
    //    /// Original was GL_MAP2_VERTEX_4 = 0x0DB8
    //    /// </summary>
    //    Map2Vertex4 = ((int)0x0DB8),
    //    /// <summary>
    //    /// Original was GL_MAP1_GRID_DOMAIN = 0x0DD0
    //    /// </summary>
    //    Map1GridDomain = ((int)0x0DD0),
    //    /// <summary>
    //    /// Original was GL_MAP1_GRID_SEGMENTS = 0x0DD1
    //    /// </summary>
    //    Map1GridSegments = ((int)0x0DD1),
    //    /// <summary>
    //    /// Original was GL_MAP2_GRID_DOMAIN = 0x0DD2
    //    /// </summary>
    //    Map2GridDomain = ((int)0x0DD2),
    //    /// <summary>
    //    /// Original was GL_MAP2_GRID_SEGMENTS = 0x0DD3
    //    /// </summary>
    //    Map2GridSegments = ((int)0x0DD3),
    //    /// <summary>
    //    /// Original was GL_TEXTURE_1D = 0x0DE0
    //    /// </summary>
    //    Texture1D = ((int)0x0DE0),
    //    /// <summary>
    //    /// Original was GL_TEXTURE_2D = 0x0DE1
    //    /// </summary>
    //    Texture2D = ((int)0x0DE1),
    //    /// <summary>
    //    /// Original was GL_FEEDBACK_BUFFER_SIZE = 0x0DF1
    //    /// </summary>
    //    FeedbackBufferSize = ((int)0x0DF1),
    //    /// <summary>
    //    /// Original was GL_FEEDBACK_BUFFER_TYPE = 0x0DF2
    //    /// </summary>
    //    FeedbackBufferType = ((int)0x0DF2),
    //    /// <summary>
    //    /// Original was GL_SELECTION_BUFFER_SIZE = 0x0DF4
    //    /// </summary>
    //    SelectionBufferSize = ((int)0x0DF4),
    //    /// <summary>
    //    /// Original was GL_POLYGON_OFFSET_UNITS = 0x2A00
    //    /// </summary>
    //    PolygonOffsetUnits = ((int)0x2A00),
    //    /// <summary>
    //    /// Original was GL_POLYGON_OFFSET_POINT = 0x2A01
    //    /// </summary>
    //    PolygonOffsetPoint = ((int)0x2A01),
    //    /// <summary>
    //    /// Original was GL_POLYGON_OFFSET_LINE = 0x2A02
    //    /// </summary>
    //    PolygonOffsetLine = ((int)0x2A02),
    //    /// <summary>
    //    /// Original was GL_CLIP_PLANE0 = 0x3000
    //    /// </summary>
    //    ClipPlane0 = ((int)0x3000),
    //    /// <summary>
    //    /// Original was GL_CLIP_PLANE1 = 0x3001
    //    /// </summary>
    //    ClipPlane1 = ((int)0x3001),
    //    /// <summary>
    //    /// Original was GL_CLIP_PLANE2 = 0x3002
    //    /// </summary>
    //    ClipPlane2 = ((int)0x3002),
    //    /// <summary>
    //    /// Original was GL_CLIP_PLANE3 = 0x3003
    //    /// </summary>
    //    ClipPlane3 = ((int)0x3003),
    //    /// <summary>
    //    /// Original was GL_CLIP_PLANE4 = 0x3004
    //    /// </summary>
    //    ClipPlane4 = ((int)0x3004),
    //    /// <summary>
    //    /// Original was GL_CLIP_PLANE5 = 0x3005
    //    /// </summary>
    //    ClipPlane5 = ((int)0x3005),
    //    /// <summary>
    //    /// Original was GL_LIGHT0 = 0x4000
    //    /// </summary>
    //    Light0 = ((int)0x4000),
    //    /// <summary>
    //    /// Original was GL_LIGHT1 = 0x4001
    //    /// </summary>
    //    Light1 = ((int)0x4001),
    //    /// <summary>
    //    /// Original was GL_LIGHT2 = 0x4002
    //    /// </summary>
    //    Light2 = ((int)0x4002),
    //    /// <summary>
    //    /// Original was GL_LIGHT3 = 0x4003
    //    /// </summary>
    //    Light3 = ((int)0x4003),
    //    /// <summary>
    //    /// Original was GL_LIGHT4 = 0x4004
    //    /// </summary>
    //    Light4 = ((int)0x4004),
    //    /// <summary>
    //    /// Original was GL_LIGHT5 = 0x4005
    //    /// </summary>
    //    Light5 = ((int)0x4005),
    //    /// <summary>
    //    /// Original was GL_LIGHT6 = 0x4006
    //    /// </summary>
    //    Light6 = ((int)0x4006),
    //    /// <summary>
    //    /// Original was GL_LIGHT7 = 0x4007
    //    /// </summary>
    //    Light7 = ((int)0x4007),
    //    /// <summary>
    //    /// Original was GL_BLEND_COLOR_EXT = 0x8005
    //    /// </summary>
    //    BlendColorExt = ((int)0x8005),
    //    /// <summary>
    //    /// Original was GL_BLEND_EQUATION_EXT = 0x8009
    //    /// </summary>
    //    BlendEquationExt = ((int)0x8009),
    //    /// <summary>
    //    /// Original was GL_BLEND_EQUATION_RGB = 0x8009
    //    /// </summary>
    //    BlendEquationRgb = ((int)0x8009),
    //    /// <summary>
    //    /// Original was GL_PACK_CMYK_HINT_EXT = 0x800E
    //    /// </summary>
    //    PackCmykHintExt = ((int)0x800E),
    //    /// <summary>
    //    /// Original was GL_UNPACK_CMYK_HINT_EXT = 0x800F
    //    /// </summary>
    //    UnpackCmykHintExt = ((int)0x800F),
    //    /// <summary>
    //    /// Original was GL_CONVOLUTION_1D_EXT = 0x8010
    //    /// </summary>
    //    Convolution1DExt = ((int)0x8010),
    //    /// <summary>
    //    /// Original was GL_CONVOLUTION_2D_EXT = 0x8011
    //    /// </summary>
    //    Convolution2DExt = ((int)0x8011),
    //    /// <summary>
    //    /// Original was GL_SEPARABLE_2D_EXT = 0x8012
    //    /// </summary>
    //    Separable2DExt = ((int)0x8012),
    //    /// <summary>
    //    /// Original was GL_POST_CONVOLUTION_RED_SCALE_EXT = 0x801C
    //    /// </summary>
    //    PostConvolutionRedScaleExt = ((int)0x801C),
    //    /// <summary>
    //    /// Original was GL_POST_CONVOLUTION_GREEN_SCALE_EXT = 0x801D
    //    /// </summary>
    //    PostConvolutionGreenScaleExt = ((int)0x801D),
    //    /// <summary>
    //    /// Original was GL_POST_CONVOLUTION_BLUE_SCALE_EXT = 0x801E
    //    /// </summary>
    //    PostConvolutionBlueScaleExt = ((int)0x801E),
    //    /// <summary>
    //    /// Original was GL_POST_CONVOLUTION_ALPHA_SCALE_EXT = 0x801F
    //    /// </summary>
    //    PostConvolutionAlphaScaleExt = ((int)0x801F),
    //    /// <summary>
    //    /// Original was GL_POST_CONVOLUTION_RED_BIAS_EXT = 0x8020
    //    /// </summary>
    //    PostConvolutionRedBiasExt = ((int)0x8020),
    //    /// <summary>
    //    /// Original was GL_POST_CONVOLUTION_GREEN_BIAS_EXT = 0x8021
    //    /// </summary>
    //    PostConvolutionGreenBiasExt = ((int)0x8021),
    //    /// <summary>
    //    /// Original was GL_POST_CONVOLUTION_BLUE_BIAS_EXT = 0x8022
    //    /// </summary>
    //    PostConvolutionBlueBiasExt = ((int)0x8022),
    //    /// <summary>
    //    /// Original was GL_POST_CONVOLUTION_ALPHA_BIAS_EXT = 0x8023
    //    /// </summary>
    //    PostConvolutionAlphaBiasExt = ((int)0x8023),
    //    /// <summary>
    //    /// Original was GL_HISTOGRAM_EXT = 0x8024
    //    /// </summary>
    //    HistogramExt = ((int)0x8024),
    //    /// <summary>
    //    /// Original was GL_MINMAX_EXT = 0x802E
    //    /// </summary>
    //    MinmaxExt = ((int)0x802E),
    //    /// <summary>
    //    /// Original was GL_POLYGON_OFFSET_FILL = 0x8037
    //    /// </summary>
    //    PolygonOffsetFill = ((int)0x8037),
    //    /// <summary>
    //    /// Original was GL_POLYGON_OFFSET_FACTOR = 0x8038
    //    /// </summary>
    //    PolygonOffsetFactor = ((int)0x8038),
    //    /// <summary>
    //    /// Original was GL_POLYGON_OFFSET_BIAS_EXT = 0x8039
    //    /// </summary>
    //    PolygonOffsetBiasExt = ((int)0x8039),
    //    /// <summary>
    //    /// Original was GL_RESCALE_NORMAL_EXT = 0x803A
    //    /// </summary>
    //    RescaleNormalExt = ((int)0x803A),
    //    /// <summary>
    //    /// Original was GL_TEXTURE_BINDING_1D = 0x8068
    //    /// </summary>
    //    TextureBinding1D = ((int)0x8068),
    //    /// <summary>
    //    /// Original was GL_TEXTURE_BINDING_2D = 0x8069
    //    /// </summary>
    //    TextureBinding2D = ((int)0x8069),
    //    /// <summary>
    //    /// Original was GL_TEXTURE_3D_BINDING_EXT = 0x806A
    //    /// </summary>
    //    Texture3DBindingExt = ((int)0x806A),
    //    /// <summary>
    //    /// Original was GL_TEXTURE_BINDING_3D = 0x806A
    //    /// </summary>
    //    TextureBinding3D = ((int)0x806A),
    //    /// <summary>
    //    /// Original was GL_PACK_SKIP_IMAGES_EXT = 0x806B
    //    /// </summary>
    //    PackSkipImagesExt = ((int)0x806B),
    //    /// <summary>
    //    /// Original was GL_PACK_IMAGE_HEIGHT_EXT = 0x806C
    //    /// </summary>
    //    PackImageHeightExt = ((int)0x806C),
    //    /// <summary>
    //    /// Original was GL_UNPACK_SKIP_IMAGES_EXT = 0x806D
    //    /// </summary>
    //    UnpackSkipImagesExt = ((int)0x806D),
    //    /// <summary>
    //    /// Original was GL_UNPACK_IMAGE_HEIGHT_EXT = 0x806E
    //    /// </summary>
    //    UnpackImageHeightExt = ((int)0x806E),
    //    /// <summary>
    //    /// Original was GL_TEXTURE_3D_EXT = 0x806F
    //    /// </summary>
    //    Texture3DExt = ((int)0x806F),
    //    /// <summary>
    //    /// Original was GL_MAX_3D_TEXTURE_SIZE = 0x8073
    //    /// </summary>
    //    Max3DTextureSize = ((int)0x8073),
    //    /// <summary>
    //    /// Original was GL_MAX_3D_TEXTURE_SIZE_EXT = 0x8073
    //    /// </summary>
    //    Max3DTextureSizeExt = ((int)0x8073),
    //    /// <summary>
    //    /// Original was GL_VERTEX_ARRAY = 0x8074
    //    /// </summary>
    //    VertexArray = ((int)0x8074),
    //    /// <summary>
    //    /// Original was GL_NORMAL_ARRAY = 0x8075
    //    /// </summary>
    //    NormalArray = ((int)0x8075),
    //    /// <summary>
    //    /// Original was GL_COLOR_ARRAY = 0x8076
    //    /// </summary>
    //    ColorArray = ((int)0x8076),
    //    /// <summary>
    //    /// Original was GL_INDEX_ARRAY = 0x8077
    //    /// </summary>
    //    IndexArray = ((int)0x8077),
    //    /// <summary>
    //    /// Original was GL_TEXTURE_COORD_ARRAY = 0x8078
    //    /// </summary>
    //    TextureCoordArray = ((int)0x8078),
    //    /// <summary>
    //    /// Original was GL_EDGE_FLAG_ARRAY = 0x8079
    //    /// </summary>
    //    EdgeFlagArray = ((int)0x8079),
    //    /// <summary>
    //    /// Original was GL_VERTEX_ARRAY_SIZE = 0x807A
    //    /// </summary>
    //    VertexArraySize = ((int)0x807A),
    //    /// <summary>
    //    /// Original was GL_VERTEX_ARRAY_TYPE = 0x807B
    //    /// </summary>
    //    VertexArrayType = ((int)0x807B),
    //    /// <summary>
    //    /// Original was GL_VERTEX_ARRAY_STRIDE = 0x807C
    //    /// </summary>
    //    VertexArrayStride = ((int)0x807C),
    //    /// <summary>
    //    /// Original was GL_VERTEX_ARRAY_COUNT_EXT = 0x807D
    //    /// </summary>
    //    VertexArrayCountExt = ((int)0x807D),
    //    /// <summary>
    //    /// Original was GL_NORMAL_ARRAY_TYPE = 0x807E
    //    /// </summary>
    //    NormalArrayType = ((int)0x807E),
    //    /// <summary>
    //    /// Original was GL_NORMAL_ARRAY_STRIDE = 0x807F
    //    /// </summary>
    //    NormalArrayStride = ((int)0x807F),
    //    /// <summary>
    //    /// Original was GL_NORMAL_ARRAY_COUNT_EXT = 0x8080
    //    /// </summary>
    //    NormalArrayCountExt = ((int)0x8080),
    //    /// <summary>
    //    /// Original was GL_COLOR_ARRAY_SIZE = 0x8081
    //    /// </summary>
    //    ColorArraySize = ((int)0x8081),
    //    /// <summary>
    //    /// Original was GL_COLOR_ARRAY_TYPE = 0x8082
    //    /// </summary>
    //    ColorArrayType = ((int)0x8082),
    //    /// <summary>
    //    /// Original was GL_COLOR_ARRAY_STRIDE = 0x8083
    //    /// </summary>
    //    ColorArrayStride = ((int)0x8083),
    //    /// <summary>
    //    /// Original was GL_COLOR_ARRAY_COUNT_EXT = 0x8084
    //    /// </summary>
    //    ColorArrayCountExt = ((int)0x8084),
    //    /// <summary>
    //    /// Original was GL_INDEX_ARRAY_TYPE = 0x8085
    //    /// </summary>
    //    IndexArrayType = ((int)0x8085),
    //    /// <summary>
    //    /// Original was GL_INDEX_ARRAY_STRIDE = 0x8086
    //    /// </summary>
    //    IndexArrayStride = ((int)0x8086),
    //    /// <summary>
    //    /// Original was GL_INDEX_ARRAY_COUNT_EXT = 0x8087
    //    /// </summary>
    //    IndexArrayCountExt = ((int)0x8087),
    //    /// <summary>
    //    /// Original was GL_TEXTURE_COORD_ARRAY_SIZE = 0x8088
    //    /// </summary>
    //    TextureCoordArraySize = ((int)0x8088),
    //    /// <summary>
    //    /// Original was GL_TEXTURE_COORD_ARRAY_TYPE = 0x8089
    //    /// </summary>
    //    TextureCoordArrayType = ((int)0x8089),
    //    /// <summary>
    //    /// Original was GL_TEXTURE_COORD_ARRAY_STRIDE = 0x808A
    //    /// </summary>
    //    TextureCoordArrayStride = ((int)0x808A),
    //    /// <summary>
    //    /// Original was GL_TEXTURE_COORD_ARRAY_COUNT_EXT = 0x808B
    //    /// </summary>
    //    TextureCoordArrayCountExt = ((int)0x808B),
    //    /// <summary>
    //    /// Original was GL_EDGE_FLAG_ARRAY_STRIDE = 0x808C
    //    /// </summary>
    //    EdgeFlagArrayStride = ((int)0x808C),
    //    /// <summary>
    //    /// Original was GL_EDGE_FLAG_ARRAY_COUNT_EXT = 0x808D
    //    /// </summary>
    //    EdgeFlagArrayCountExt = ((int)0x808D),
    //    /// <summary>
    //    /// Original was GL_INTERLACE_SGIX = 0x8094
    //    /// </summary>
    //    InterlaceSgix = ((int)0x8094),
    //    /// <summary>
    //    /// Original was GL_DETAIL_TEXTURE_2D_BINDING_SGIS = 0x8096
    //    /// </summary>
    //    DetailTexture2DBindingSgis = ((int)0x8096),
    //    /// <summary>
    //    /// Original was GL_MULTISAMPLE = 0x809D
    //    /// </summary>
    //    Multisample = ((int)0x809D),
    //    /// <summary>
    //    /// Original was GL_MULTISAMPLE_SGIS = 0x809D
    //    /// </summary>
    //    MultisampleSgis = ((int)0x809D),
    //    /// <summary>
    //    /// Original was GL_SAMPLE_ALPHA_TO_COVERAGE = 0x809E
    //    /// </summary>
    //    SampleAlphaToCoverage = ((int)0x809E),
    //    /// <summary>
    //    /// Original was GL_SAMPLE_ALPHA_TO_MASK_SGIS = 0x809E
    //    /// </summary>
    //    SampleAlphaToMaskSgis = ((int)0x809E),
    //    /// <summary>
    //    /// Original was GL_SAMPLE_ALPHA_TO_ONE = 0x809F
    //    /// </summary>
    //    SampleAlphaToOne = ((int)0x809F),
    //    /// <summary>
    //    /// Original was GL_SAMPLE_ALPHA_TO_ONE_SGIS = 0x809F
    //    /// </summary>
    //    SampleAlphaToOneSgis = ((int)0x809F),
    //    /// <summary>
    //    /// Original was GL_SAMPLE_COVERAGE = 0x80A0
    //    /// </summary>
    //    SampleCoverage = ((int)0x80A0),
    //    /// <summary>
    //    /// Original was GL_SAMPLE_MASK_SGIS = 0x80A0
    //    /// </summary>
    //    SampleMaskSgis = ((int)0x80A0),
    //    /// <summary>
    //    /// Original was GL_SAMPLE_BUFFERS = 0x80A8
    //    /// </summary>
    //    SampleBuffers = ((int)0x80A8),
    //    /// <summary>
    //    /// Original was GL_SAMPLE_BUFFERS_SGIS = 0x80A8
    //    /// </summary>
    //    SampleBuffersSgis = ((int)0x80A8),
    //    /// <summary>
    //    /// Original was GL_SAMPLES = 0x80A9
    //    /// </summary>
    //    Samples = ((int)0x80A9),
    //    /// <summary>
    //    /// Original was GL_SAMPLES_SGIS = 0x80A9
    //    /// </summary>
    //    SamplesSgis = ((int)0x80A9),
    //    /// <summary>
    //    /// Original was GL_SAMPLE_COVERAGE_VALUE = 0x80AA
    //    /// </summary>
    //    SampleCoverageValue = ((int)0x80AA),
    //    /// <summary>
    //    /// Original was GL_SAMPLE_MASK_VALUE_SGIS = 0x80AA
    //    /// </summary>
    //    SampleMaskValueSgis = ((int)0x80AA),
    //    /// <summary>
    //    /// Original was GL_SAMPLE_COVERAGE_INVERT = 0x80AB
    //    /// </summary>
    //    SampleCoverageInvert = ((int)0x80AB),
    //    /// <summary>
    //    /// Original was GL_SAMPLE_MASK_INVERT_SGIS = 0x80AB
    //    /// </summary>
    //    SampleMaskInvertSgis = ((int)0x80AB),
    //    /// <summary>
    //    /// Original was GL_SAMPLE_PATTERN_SGIS = 0x80AC
    //    /// </summary>
    //    SamplePatternSgis = ((int)0x80AC),
    //    /// <summary>
    //    /// Original was GL_COLOR_MATRIX_SGI = 0x80B1
    //    /// </summary>
    //    ColorMatrixSgi = ((int)0x80B1),
    //    /// <summary>
    //    /// Original was GL_COLOR_MATRIX_STACK_DEPTH_SGI = 0x80B2
    //    /// </summary>
    //    ColorMatrixStackDepthSgi = ((int)0x80B2),
    //    /// <summary>
    //    /// Original was GL_MAX_COLOR_MATRIX_STACK_DEPTH_SGI = 0x80B3
    //    /// </summary>
    //    MaxColorMatrixStackDepthSgi = ((int)0x80B3),
    //    /// <summary>
    //    /// Original was GL_POST_COLOR_MATRIX_RED_SCALE_SGI = 0x80B4
    //    /// </summary>
    //    PostColorMatrixRedScaleSgi = ((int)0x80B4),
    //    /// <summary>
    //    /// Original was GL_POST_COLOR_MATRIX_GREEN_SCALE_SGI = 0x80B5
    //    /// </summary>
    //    PostColorMatrixGreenScaleSgi = ((int)0x80B5),
    //    /// <summary>
    //    /// Original was GL_POST_COLOR_MATRIX_BLUE_SCALE_SGI = 0x80B6
    //    /// </summary>
    //    PostColorMatrixBlueScaleSgi = ((int)0x80B6),
    //    /// <summary>
    //    /// Original was GL_POST_COLOR_MATRIX_ALPHA_SCALE_SGI = 0x80B7
    //    /// </summary>
    //    PostColorMatrixAlphaScaleSgi = ((int)0x80B7),
    //    /// <summary>
    //    /// Original was GL_POST_COLOR_MATRIX_RED_BIAS_SGI = 0x80B8
    //    /// </summary>
    //    PostColorMatrixRedBiasSgi = ((int)0x80B8),
    //    /// <summary>
    //    /// Original was GL_POST_COLOR_MATRIX_GREEN_BIAS_SGI = 0x80B9
    //    /// </summary>
    //    PostColorMatrixGreenBiasSgi = ((int)0x80B9),
    //    /// <summary>
    //    /// Original was GL_POST_COLOR_MATRIX_BLUE_BIAS_SGI = 0x80BA
    //    /// </summary>
    //    PostColorMatrixBlueBiasSgi = ((int)0x80BA),
    //    /// <summary>
    //    /// Original was GL_POST_COLOR_MATRIX_ALPHA_BIAS_SGI = 0x80BB
    //    /// </summary>
    //    PostColorMatrixAlphaBiasSgi = ((int)0x80BB),
    //    /// <summary>
    //    /// Original was GL_TEXTURE_COLOR_TABLE_SGI = 0x80BC
    //    /// </summary>
    //    TextureColorTableSgi = ((int)0x80BC),
    //    /// <summary>
    //    /// Original was GL_BLEND_DST_RGB = 0x80C8
    //    /// </summary>
    //    BlendDstRgb = ((int)0x80C8),
    //    /// <summary>
    //    /// Original was GL_BLEND_SRC_RGB = 0x80C9
    //    /// </summary>
    //    BlendSrcRgb = ((int)0x80C9),
    //    /// <summary>
    //    /// Original was GL_BLEND_DST_ALPHA = 0x80CA
    //    /// </summary>
    //    BlendDstAlpha = ((int)0x80CA),
    //    /// <summary>
    //    /// Original was GL_BLEND_SRC_ALPHA = 0x80CB
    //    /// </summary>
    //    BlendSrcAlpha = ((int)0x80CB),
    //    /// <summary>
    //    /// Original was GL_COLOR_TABLE_SGI = 0x80D0
    //    /// </summary>
    //    ColorTableSgi = ((int)0x80D0),
    //    /// <summary>
    //    /// Original was GL_POST_CONVOLUTION_COLOR_TABLE_SGI = 0x80D1
    //    /// </summary>
    //    PostConvolutionColorTableSgi = ((int)0x80D1),
    //    /// <summary>
    //    /// Original was GL_POST_COLOR_MATRIX_COLOR_TABLE_SGI = 0x80D2
    //    /// </summary>
    //    PostColorMatrixColorTableSgi = ((int)0x80D2),
    //    /// <summary>
    //    /// Original was GL_MAX_ELEMENTS_VERTICES = 0x80E8
    //    /// </summary>
    //    MaxElementsVertices = ((int)0x80E8),
    //    /// <summary>
    //    /// Original was GL_MAX_ELEMENTS_INDICES = 0x80E9
    //    /// </summary>
    //    MaxElementsIndices = ((int)0x80E9),
    //    /// <summary>
    //    /// Original was GL_POINT_SIZE_MIN = 0x8126
    //    /// </summary>
    //    PointSizeMin = ((int)0x8126),
    //    /// <summary>
    //    /// Original was GL_POINT_SIZE_MIN_SGIS = 0x8126
    //    /// </summary>
    //    PointSizeMinSgis = ((int)0x8126),
    //    /// <summary>
    //    /// Original was GL_POINT_SIZE_MAX = 0x8127
    //    /// </summary>
    //    PointSizeMax = ((int)0x8127),
    //    /// <summary>
    //    /// Original was GL_POINT_SIZE_MAX_SGIS = 0x8127
    //    /// </summary>
    //    PointSizeMaxSgis = ((int)0x8127),
    //    /// <summary>
    //    /// Original was GL_POINT_FADE_THRESHOLD_SIZE = 0x8128
    //    /// </summary>
    //    PointFadeThresholdSize = ((int)0x8128),
    //    /// <summary>
    //    /// Original was GL_POINT_FADE_THRESHOLD_SIZE_SGIS = 0x8128
    //    /// </summary>
    //    PointFadeThresholdSizeSgis = ((int)0x8128),
    //    /// <summary>
    //    /// Original was GL_DISTANCE_ATTENUATION_SGIS = 0x8129
    //    /// </summary>
    //    DistanceAttenuationSgis = ((int)0x8129),
    //    /// <summary>
    //    /// Original was GL_POINT_DISTANCE_ATTENUATION = 0x8129
    //    /// </summary>
    //    PointDistanceAttenuation = ((int)0x8129),
    //    /// <summary>
    //    /// Original was GL_FOG_FUNC_POINTS_SGIS = 0x812B
    //    /// </summary>
    //    FogFuncPointsSgis = ((int)0x812B),
    //    /// <summary>
    //    /// Original was GL_MAX_FOG_FUNC_POINTS_SGIS = 0x812C
    //    /// </summary>
    //    MaxFogFuncPointsSgis = ((int)0x812C),
    //    /// <summary>
    //    /// Original was GL_PACK_SKIP_VOLUMES_SGIS = 0x8130
    //    /// </summary>
    //    PackSkipVolumesSgis = ((int)0x8130),
    //    /// <summary>
    //    /// Original was GL_PACK_IMAGE_DEPTH_SGIS = 0x8131
    //    /// </summary>
    //    PackImageDepthSgis = ((int)0x8131),
    //    /// <summary>
    //    /// Original was GL_UNPACK_SKIP_VOLUMES_SGIS = 0x8132
    //    /// </summary>
    //    UnpackSkipVolumesSgis = ((int)0x8132),
    //    /// <summary>
    //    /// Original was GL_UNPACK_IMAGE_DEPTH_SGIS = 0x8133
    //    /// </summary>
    //    UnpackImageDepthSgis = ((int)0x8133),
    //    /// <summary>
    //    /// Original was GL_TEXTURE_4D_SGIS = 0x8134
    //    /// </summary>
    //    Texture4DSgis = ((int)0x8134),
    //    /// <summary>
    //    /// Original was GL_MAX_4D_TEXTURE_SIZE_SGIS = 0x8138
    //    /// </summary>
    //    Max4DTextureSizeSgis = ((int)0x8138),
    //    /// <summary>
    //    /// Original was GL_PIXEL_TEX_GEN_SGIX = 0x8139
    //    /// </summary>
    //    PixelTexGenSgix = ((int)0x8139),
    //    /// <summary>
    //    /// Original was GL_PIXEL_TILE_BEST_ALIGNMENT_SGIX = 0x813E
    //    /// </summary>
    //    PixelTileBestAlignmentSgix = ((int)0x813E),
    //    /// <summary>
    //    /// Original was GL_PIXEL_TILE_CACHE_INCREMENT_SGIX = 0x813F
    //    /// </summary>
    //    PixelTileCacheIncrementSgix = ((int)0x813F),
    //    /// <summary>
    //    /// Original was GL_PIXEL_TILE_WIDTH_SGIX = 0x8140
    //    /// </summary>
    //    PixelTileWidthSgix = ((int)0x8140),
    //    /// <summary>
    //    /// Original was GL_PIXEL_TILE_HEIGHT_SGIX = 0x8141
    //    /// </summary>
    //    PixelTileHeightSgix = ((int)0x8141),
    //    /// <summary>
    //    /// Original was GL_PIXEL_TILE_GRID_WIDTH_SGIX = 0x8142
    //    /// </summary>
    //    PixelTileGridWidthSgix = ((int)0x8142),
    //    /// <summary>
    //    /// Original was GL_PIXEL_TILE_GRID_HEIGHT_SGIX = 0x8143
    //    /// </summary>
    //    PixelTileGridHeightSgix = ((int)0x8143),
    //    /// <summary>
    //    /// Original was GL_PIXEL_TILE_GRID_DEPTH_SGIX = 0x8144
    //    /// </summary>
    //    PixelTileGridDepthSgix = ((int)0x8144),
    //    /// <summary>
    //    /// Original was GL_PIXEL_TILE_CACHE_SIZE_SGIX = 0x8145
    //    /// </summary>
    //    PixelTileCacheSizeSgix = ((int)0x8145),
    //    /// <summary>
    //    /// Original was GL_SPRITE_SGIX = 0x8148
    //    /// </summary>
    //    SpriteSgix = ((int)0x8148),
    //    /// <summary>
    //    /// Original was GL_SPRITE_MODE_SGIX = 0x8149
    //    /// </summary>
    //    SpriteModeSgix = ((int)0x8149),
    //    /// <summary>
    //    /// Original was GL_SPRITE_AXIS_SGIX = 0x814A
    //    /// </summary>
    //    SpriteAxisSgix = ((int)0x814A),
    //    /// <summary>
    //    /// Original was GL_SPRITE_TRANSLATION_SGIX = 0x814B
    //    /// </summary>
    //    SpriteTranslationSgix = ((int)0x814B),
    //    /// <summary>
    //    /// Original was GL_TEXTURE_4D_BINDING_SGIS = 0x814F
    //    /// </summary>
    //    Texture4DBindingSgis = ((int)0x814F),
    //    /// <summary>
    //    /// Original was GL_MAX_CLIPMAP_DEPTH_SGIX = 0x8177
    //    /// </summary>
    //    MaxClipmapDepthSgix = ((int)0x8177),
    //    /// <summary>
    //    /// Original was GL_MAX_CLIPMAP_VIRTUAL_DEPTH_SGIX = 0x8178
    //    /// </summary>
    //    MaxClipmapVirtualDepthSgix = ((int)0x8178),
    //    /// <summary>
    //    /// Original was GL_POST_TEXTURE_FILTER_BIAS_RANGE_SGIX = 0x817B
    //    /// </summary>
    //    PostTextureFilterBiasRangeSgix = ((int)0x817B),
    //    /// <summary>
    //    /// Original was GL_POST_TEXTURE_FILTER_SCALE_RANGE_SGIX = 0x817C
    //    /// </summary>
    //    PostTextureFilterScaleRangeSgix = ((int)0x817C),
    //    /// <summary>
    //    /// Original was GL_REFERENCE_PLANE_SGIX = 0x817D
    //    /// </summary>
    //    ReferencePlaneSgix = ((int)0x817D),
    //    /// <summary>
    //    /// Original was GL_REFERENCE_PLANE_EQUATION_SGIX = 0x817E
    //    /// </summary>
    //    ReferencePlaneEquationSgix = ((int)0x817E),
    //    /// <summary>
    //    /// Original was GL_IR_INSTRUMENT1_SGIX = 0x817F
    //    /// </summary>
    //    IrInstrument1Sgix = ((int)0x817F),
    //    /// <summary>
    //    /// Original was GL_INSTRUMENT_MEASUREMENTS_SGIX = 0x8181
    //    /// </summary>
    //    InstrumentMeasurementsSgix = ((int)0x8181),
    //    /// <summary>
    //    /// Original was GL_CALLIGRAPHIC_FRAGMENT_SGIX = 0x8183
    //    /// </summary>
    //    CalligraphicFragmentSgix = ((int)0x8183),
    //    /// <summary>
    //    /// Original was GL_FRAMEZOOM_SGIX = 0x818B
    //    /// </summary>
    //    FramezoomSgix = ((int)0x818B),
    //    /// <summary>
    //    /// Original was GL_FRAMEZOOM_FACTOR_SGIX = 0x818C
    //    /// </summary>
    //    FramezoomFactorSgix = ((int)0x818C),
    //    /// <summary>
    //    /// Original was GL_MAX_FRAMEZOOM_FACTOR_SGIX = 0x818D
    //    /// </summary>
    //    MaxFramezoomFactorSgix = ((int)0x818D),
    //    /// <summary>
    //    /// Original was GL_GENERATE_MIPMAP_HINT = 0x8192
    //    /// </summary>
    //    GenerateMipmapHint = ((int)0x8192),
    //    /// <summary>
    //    /// Original was GL_GENERATE_MIPMAP_HINT_SGIS = 0x8192
    //    /// </summary>
    //    GenerateMipmapHintSgis = ((int)0x8192),
    //    /// <summary>
    //    /// Original was GL_DEFORMATIONS_MASK_SGIX = 0x8196
    //    /// </summary>
    //    DeformationsMaskSgix = ((int)0x8196),
    //    /// <summary>
    //    /// Original was GL_FOG_OFFSET_SGIX = 0x8198
    //    /// </summary>
    //    FogOffsetSgix = ((int)0x8198),
    //    /// <summary>
    //    /// Original was GL_FOG_OFFSET_VALUE_SGIX = 0x8199
    //    /// </summary>
    //    FogOffsetValueSgix = ((int)0x8199),
    //    /// <summary>
    //    /// Original was GL_LIGHT_MODEL_COLOR_CONTROL = 0x81F8
    //    /// </summary>
    //    LightModelColorControl = ((int)0x81F8),
    //    /// <summary>
    //    /// Original was GL_SHARED_TEXTURE_PALETTE_EXT = 0x81FB
    //    /// </summary>
    //    SharedTexturePaletteExt = ((int)0x81FB),
    //    /// <summary>
    //    /// Original was GL_MAJOR_VERSION = 0x821B
    //    /// </summary>
    //    MajorVersion = ((int)0x821B),
    //    /// <summary>
    //    /// Original was GL_MINOR_VERSION = 0x821C
    //    /// </summary>
    //    MinorVersion = ((int)0x821C),
    //    /// <summary>
    //    /// Original was GL_NUM_EXTENSIONS = 0x821D
    //    /// </summary>
    //    NumExtensions = ((int)0x821D),
    //    /// <summary>
    //    /// Original was GL_CONTEXT_FLAGS = 0x821E
    //    /// </summary>
    //    ContextFlags = ((int)0x821E),
    //    /// <summary>
    //    /// Original was GL_RESET_NOTIFICATION_STRATEGY = 0x8256
    //    /// </summary>
    //    ResetNotificationStrategy = ((int)0x8256),
    //    /// <summary>
    //    /// Original was GL_PROGRAM_PIPELINE_BINDING = 0x825A
    //    /// </summary>
    //    ProgramPipelineBinding = ((int)0x825A),
    //    /// <summary>
    //    /// Original was GL_MAX_VIEWPORTS = 0x825B
    //    /// </summary>
    //    MaxViewports = ((int)0x825B),
    //    /// <summary>
    //    /// Original was GL_VIEWPORT_SUBPIXEL_BITS = 0x825C
    //    /// </summary>
    //    ViewportSubpixelBits = ((int)0x825C),
    //    /// <summary>
    //    /// Original was GL_VIEWPORT_BOUNDS_RANGE = 0x825D
    //    /// </summary>
    //    ViewportBoundsRange = ((int)0x825D),
    //    /// <summary>
    //    /// Original was GL_LAYER_PROVOKING_VERTEX = 0x825E
    //    /// </summary>
    //    LayerProvokingVertex = ((int)0x825E),
    //    /// <summary>
    //    /// Original was GL_VIEWPORT_INDEX_PROVOKING_VERTEX = 0x825F
    //    /// </summary>
    //    ViewportIndexProvokingVertex = ((int)0x825F),
    //    /// <summary>
    //    /// Original was GL_MAX_CULL_DISTANCES = 0x82F9
    //    /// </summary>
    //    MaxCullDistances = ((int)0x82F9),
    //    /// <summary>
    //    /// Original was GL_MAX_COMBINED_CLIP_AND_CULL_DISTANCES = 0x82FA
    //    /// </summary>
    //    MaxCombinedClipAndCullDistances = ((int)0x82FA),
    //    /// <summary>
    //    /// Original was GL_CONTEXT_RELEASE_BEHAVIOR = 0x82FB
    //    /// </summary>
    //    ContextReleaseBehavior = ((int)0x82FB),
    //    /// <summary>
    //    /// Original was GL_CONVOLUTION_HINT_SGIX = 0x8316
    //    /// </summary>
    //    ConvolutionHintSgix = ((int)0x8316),
    //    /// <summary>
    //    /// Original was GL_ASYNC_MARKER_SGIX = 0x8329
    //    /// </summary>
    //    AsyncMarkerSgix = ((int)0x8329),
    //    /// <summary>
    //    /// Original was GL_PIXEL_TEX_GEN_MODE_SGIX = 0x832B
    //    /// </summary>
    //    PixelTexGenModeSgix = ((int)0x832B),
    //    /// <summary>
    //    /// Original was GL_ASYNC_HISTOGRAM_SGIX = 0x832C
    //    /// </summary>
    //    AsyncHistogramSgix = ((int)0x832C),
    //    /// <summary>
    //    /// Original was GL_MAX_ASYNC_HISTOGRAM_SGIX = 0x832D
    //    /// </summary>
    //    MaxAsyncHistogramSgix = ((int)0x832D),
    //    /// <summary>
    //    /// Original was GL_PIXEL_TEXTURE_SGIS = 0x8353
    //    /// </summary>
    //    PixelTextureSgis = ((int)0x8353),
    //    /// <summary>
    //    /// Original was GL_ASYNC_TEX_IMAGE_SGIX = 0x835C
    //    /// </summary>
    //    AsyncTexImageSgix = ((int)0x835C),
    //    /// <summary>
    //    /// Original was GL_ASYNC_DRAW_PIXELS_SGIX = 0x835D
    //    /// </summary>
    //    AsyncDrawPixelsSgix = ((int)0x835D),
    //    /// <summary>
    //    /// Original was GL_ASYNC_READ_PIXELS_SGIX = 0x835E
    //    /// </summary>
    //    AsyncReadPixelsSgix = ((int)0x835E),
    //    /// <summary>
    //    /// Original was GL_MAX_ASYNC_TEX_IMAGE_SGIX = 0x835F
    //    /// </summary>
    //    MaxAsyncTexImageSgix = ((int)0x835F),
    //    /// <summary>
    //    /// Original was GL_MAX_ASYNC_DRAW_PIXELS_SGIX = 0x8360
    //    /// </summary>
    //    MaxAsyncDrawPixelsSgix = ((int)0x8360),
    //    /// <summary>
    //    /// Original was GL_MAX_ASYNC_READ_PIXELS_SGIX = 0x8361
    //    /// </summary>
    //    MaxAsyncReadPixelsSgix = ((int)0x8361),
    //    /// <summary>
    //    /// Original was GL_VERTEX_PRECLIP_SGIX = 0x83EE
    //    /// </summary>
    //    VertexPreclipSgix = ((int)0x83EE),
    //    /// <summary>
    //    /// Original was GL_VERTEX_PRECLIP_HINT_SGIX = 0x83EF
    //    /// </summary>
    //    VertexPreclipHintSgix = ((int)0x83EF),
    //    /// <summary>
    //    /// Original was GL_FRAGMENT_LIGHTING_SGIX = 0x8400
    //    /// </summary>
    //    FragmentLightingSgix = ((int)0x8400),
    //    /// <summary>
    //    /// Original was GL_FRAGMENT_COLOR_MATERIAL_SGIX = 0x8401
    //    /// </summary>
    //    FragmentColorMaterialSgix = ((int)0x8401),
    //    /// <summary>
    //    /// Original was GL_FRAGMENT_COLOR_MATERIAL_FACE_SGIX = 0x8402
    //    /// </summary>
    //    FragmentColorMaterialFaceSgix = ((int)0x8402),
    //    /// <summary>
    //    /// Original was GL_FRAGMENT_COLOR_MATERIAL_PARAMETER_SGIX = 0x8403
    //    /// </summary>
    //    FragmentColorMaterialParameterSgix = ((int)0x8403),
    //    /// <summary>
    //    /// Original was GL_MAX_FRAGMENT_LIGHTS_SGIX = 0x8404
    //    /// </summary>
    //    MaxFragmentLightsSgix = ((int)0x8404),
    //    /// <summary>
    //    /// Original was GL_MAX_ACTIVE_LIGHTS_SGIX = 0x8405
    //    /// </summary>
    //    MaxActiveLightsSgix = ((int)0x8405),
    //    /// <summary>
    //    /// Original was GL_LIGHT_ENV_MODE_SGIX = 0x8407
    //    /// </summary>
    //    LightEnvModeSgix = ((int)0x8407),
    //    /// <summary>
    //    /// Original was GL_FRAGMENT_LIGHT_MODEL_LOCAL_VIEWER_SGIX = 0x8408
    //    /// </summary>
    //    FragmentLightModelLocalViewerSgix = ((int)0x8408),
    //    /// <summary>
    //    /// Original was GL_FRAGMENT_LIGHT_MODEL_TWO_SIDE_SGIX = 0x8409
    //    /// </summary>
    //    FragmentLightModelTwoSideSgix = ((int)0x8409),
    //    /// <summary>
    //    /// Original was GL_FRAGMENT_LIGHT_MODEL_AMBIENT_SGIX = 0x840A
    //    /// </summary>
    //    FragmentLightModelAmbientSgix = ((int)0x840A),
    //    /// <summary>
    //    /// Original was GL_FRAGMENT_LIGHT_MODEL_NORMAL_INTERPOLATION_SGIX = 0x840B
    //    /// </summary>
    //    FragmentLightModelNormalInterpolationSgix = ((int)0x840B),
    //    /// <summary>
    //    /// Original was GL_FRAGMENT_LIGHT0_SGIX = 0x840C
    //    /// </summary>
    //    FragmentLight0Sgix = ((int)0x840C),
    //    /// <summary>
    //    /// Original was GL_PACK_RESAMPLE_SGIX = 0x842C
    //    /// </summary>
    //    PackResampleSgix = ((int)0x842C),
    //    /// <summary>
    //    /// Original was GL_UNPACK_RESAMPLE_SGIX = 0x842D
    //    /// </summary>
    //    UnpackResampleSgix = ((int)0x842D),
    //    /// <summary>
    //    /// Original was GL_CURRENT_FOG_COORD = 0x8453
    //    /// </summary>
    //    CurrentFogCoord = ((int)0x8453),
    //    /// <summary>
    //    /// Original was GL_FOG_COORD_ARRAY_TYPE = 0x8454
    //    /// </summary>
    //    FogCoordArrayType = ((int)0x8454),
    //    /// <summary>
    //    /// Original was GL_FOG_COORD_ARRAY_STRIDE = 0x8455
    //    /// </summary>
    //    FogCoordArrayStride = ((int)0x8455),
    //    /// <summary>
    //    /// Original was GL_COLOR_SUM = 0x8458
    //    /// </summary>
    //    ColorSum = ((int)0x8458),
    //    /// <summary>
    //    /// Original was GL_CURRENT_SECONDARY_COLOR = 0x8459
    //    /// </summary>
    //    CurrentSecondaryColor = ((int)0x8459),
    //    /// <summary>
    //    /// Original was GL_SECONDARY_COLOR_ARRAY_SIZE = 0x845A
    //    /// </summary>
    //    SecondaryColorArraySize = ((int)0x845A),
    //    /// <summary>
    //    /// Original was GL_SECONDARY_COLOR_ARRAY_TYPE = 0x845B
    //    /// </summary>
    //    SecondaryColorArrayType = ((int)0x845B),
    //    /// <summary>
    //    /// Original was GL_SECONDARY_COLOR_ARRAY_STRIDE = 0x845C
    //    /// </summary>
    //    SecondaryColorArrayStride = ((int)0x845C),
    //    /// <summary>
    //    /// Original was GL_CURRENT_RASTER_SECONDARY_COLOR = 0x845F
    //    /// </summary>
    //    CurrentRasterSecondaryColor = ((int)0x845F),
    //    /// <summary>
    //    /// Original was GL_ALIASED_POINT_SIZE_RANGE = 0x846D
    //    /// </summary>
    //    AliasedPointSizeRange = ((int)0x846D),
    //    /// <summary>
    //    /// Original was GL_ALIASED_LINE_WIDTH_RANGE = 0x846E
    //    /// </summary>
    //    AliasedLineWidthRange = ((int)0x846E),
    //    /// <summary>
    //    /// Original was GL_ACTIVE_TEXTURE = 0x84E0
    //    /// </summary>
    //    ActiveTexture = ((int)0x84E0),
    //    /// <summary>
    //    /// Original was GL_CLIENT_ACTIVE_TEXTURE = 0x84E1
    //    /// </summary>
    //    ClientActiveTexture = ((int)0x84E1),
    //    /// <summary>
    //    /// Original was GL_MAX_TEXTURE_UNITS = 0x84E2
    //    /// </summary>
    //    MaxTextureUnits = ((int)0x84E2),
    //    /// <summary>
    //    /// Original was GL_TRANSPOSE_MODELVIEW_MATRIX = 0x84E3
    //    /// </summary>
    //    TransposeModelviewMatrix = ((int)0x84E3),
    //    /// <summary>
    //    /// Original was GL_TRANSPOSE_PROJECTION_MATRIX = 0x84E4
    //    /// </summary>
    //    TransposeProjectionMatrix = ((int)0x84E4),
    //    /// <summary>
    //    /// Original was GL_TRANSPOSE_TEXTURE_MATRIX = 0x84E5
    //    /// </summary>
    //    TransposeTextureMatrix = ((int)0x84E5),
    //    /// <summary>
    //    /// Original was GL_TRANSPOSE_COLOR_MATRIX = 0x84E6
    //    /// </summary>
    //    TransposeColorMatrix = ((int)0x84E6),
    //    /// <summary>
    //    /// Original was GL_MAX_RENDERBUFFER_SIZE = 0x84E8
    //    /// </summary>
    //    MaxRenderbufferSize = ((int)0x84E8),
    //    /// <summary>
    //    /// Original was GL_MAX_RENDERBUFFER_SIZE_EXT = 0x84E8
    //    /// </summary>
    //    MaxRenderbufferSizeExt = ((int)0x84E8),
    //    /// <summary>
    //    /// Original was GL_TEXTURE_COMPRESSION_HINT = 0x84EF
    //    /// </summary>
    //    TextureCompressionHint = ((int)0x84EF),
    //    /// <summary>
    //    /// Original was GL_TEXTURE_BINDING_RECTANGLE = 0x84F6
    //    /// </summary>
    //    TextureBindingRectangle = ((int)0x84F6),
    //    /// <summary>
    //    /// Original was GL_MAX_RECTANGLE_TEXTURE_SIZE = 0x84F8
    //    /// </summary>
    //    MaxRectangleTextureSize = ((int)0x84F8),
    //    /// <summary>
    //    /// Original was GL_MAX_TEXTURE_LOD_BIAS = 0x84FD
    //    /// </summary>
    //    MaxTextureLodBias = ((int)0x84FD),
    //    /// <summary>
    //    /// Original was GL_TEXTURE_CUBE_MAP = 0x8513
    //    /// </summary>
    //    TextureCubeMap = ((int)0x8513),
    //    /// <summary>
    //    /// Original was GL_TEXTURE_BINDING_CUBE_MAP = 0x8514
    //    /// </summary>
    //    TextureBindingCubeMap = ((int)0x8514),
    //    /// <summary>
    //    /// Original was GL_MAX_CUBE_MAP_TEXTURE_SIZE = 0x851C
    //    /// </summary>
    //    MaxCubeMapTextureSize = ((int)0x851C),
    //    /// <summary>
    //    /// Original was GL_PACK_SUBSAMPLE_RATE_SGIX = 0x85A0
    //    /// </summary>
    //    PackSubsampleRateSgix = ((int)0x85A0),
    //    /// <summary>
    //    /// Original was GL_UNPACK_SUBSAMPLE_RATE_SGIX = 0x85A1
    //    /// </summary>
    //    UnpackSubsampleRateSgix = ((int)0x85A1),
    //    /// <summary>
    //    /// Original was GL_VERTEX_ARRAY_BINDING = 0x85B5
    //    /// </summary>
    //    VertexArrayBinding = ((int)0x85B5),
    //    /// <summary>
    //    /// Original was GL_PROGRAM_POINT_SIZE = 0x8642
    //    /// </summary>
    //    ProgramPointSize = ((int)0x8642),
    //    /// <summary>
    //    /// Original was GL_DEPTH_CLAMP = 0x864F
    //    /// </summary>
    //    DepthClamp = ((int)0x864F),
    //    /// <summary>
    //    /// Original was GL_NUM_COMPRESSED_TEXTURE_FORMATS = 0x86A2
    //    /// </summary>
    //    NumCompressedTextureFormats = ((int)0x86A2),
    //    /// <summary>
    //    /// Original was GL_COMPRESSED_TEXTURE_FORMATS = 0x86A3
    //    /// </summary>
    //    CompressedTextureFormats = ((int)0x86A3),
    //    /// <summary>
    //    /// Original was GL_NUM_PROGRAM_BINARY_FORMATS = 0x87FE
    //    /// </summary>
    //    NumProgramBinaryFormats = ((int)0x87FE),
    //    /// <summary>
    //    /// Original was GL_PROGRAM_BINARY_FORMATS = 0x87FF
    //    /// </summary>
    //    ProgramBinaryFormats = ((int)0x87FF),
    //    /// <summary>
    //    /// Original was GL_STENCIL_BACK_FUNC = 0x8800
    //    /// </summary>
    //    StencilBackFunc = ((int)0x8800),
    //    /// <summary>
    //    /// Original was GL_STENCIL_BACK_FAIL = 0x8801
    //    /// </summary>
    //    StencilBackFail = ((int)0x8801),
    //    /// <summary>
    //    /// Original was GL_STENCIL_BACK_PASS_DEPTH_FAIL = 0x8802
    //    /// </summary>
    //    StencilBackPassDepthFail = ((int)0x8802),
    //    /// <summary>
    //    /// Original was GL_STENCIL_BACK_PASS_DEPTH_PASS = 0x8803
    //    /// </summary>
    //    StencilBackPassDepthPass = ((int)0x8803),
    //    /// <summary>
    //    /// Original was GL_RGBA_FLOAT_MODE = 0x8820
    //    /// </summary>
    //    RgbaFloatMode = ((int)0x8820),
    //    /// <summary>
    //    /// Original was GL_MAX_DRAW_BUFFERS = 0x8824
    //    /// </summary>
    //    MaxDrawBuffers = ((int)0x8824),
    //    /// <summary>
    //    /// Original was GL_DRAW_BUFFER0 = 0x8825
    //    /// </summary>
    //    DrawBuffer0 = ((int)0x8825),
    //    /// <summary>
    //    /// Original was GL_DRAW_BUFFER1 = 0x8826
    //    /// </summary>
    //    DrawBuffer1 = ((int)0x8826),
    //    /// <summary>
    //    /// Original was GL_DRAW_BUFFER2 = 0x8827
    //    /// </summary>
    //    DrawBuffer2 = ((int)0x8827),
    //    /// <summary>
    //    /// Original was GL_DRAW_BUFFER3 = 0x8828
    //    /// </summary>
    //    DrawBuffer3 = ((int)0x8828),
    //    /// <summary>
    //    /// Original was GL_DRAW_BUFFER4 = 0x8829
    //    /// </summary>
    //    DrawBuffer4 = ((int)0x8829),
    //    /// <summary>
    //    /// Original was GL_DRAW_BUFFER5 = 0x882A
    //    /// </summary>
    //    DrawBuffer5 = ((int)0x882A),
    //    /// <summary>
    //    /// Original was GL_DRAW_BUFFER6 = 0x882B
    //    /// </summary>
    //    DrawBuffer6 = ((int)0x882B),
    //    /// <summary>
    //    /// Original was GL_DRAW_BUFFER7 = 0x882C
    //    /// </summary>
    //    DrawBuffer7 = ((int)0x882C),
    //    /// <summary>
    //    /// Original was GL_DRAW_BUFFER8 = 0x882D
    //    /// </summary>
    //    DrawBuffer8 = ((int)0x882D),
    //    /// <summary>
    //    /// Original was GL_DRAW_BUFFER9 = 0x882E
    //    /// </summary>
    //    DrawBuffer9 = ((int)0x882E),
    //    /// <summary>
    //    /// Original was GL_DRAW_BUFFER10 = 0x882F
    //    /// </summary>
    //    DrawBuffer10 = ((int)0x882F),
    //    /// <summary>
    //    /// Original was GL_DRAW_BUFFER11 = 0x8830
    //    /// </summary>
    //    DrawBuffer11 = ((int)0x8830),
    //    /// <summary>
    //    /// Original was GL_DRAW_BUFFER12 = 0x8831
    //    /// </summary>
    //    DrawBuffer12 = ((int)0x8831),
    //    /// <summary>
    //    /// Original was GL_DRAW_BUFFER13 = 0x8832
    //    /// </summary>
    //    DrawBuffer13 = ((int)0x8832),
    //    /// <summary>
    //    /// Original was GL_DRAW_BUFFER14 = 0x8833
    //    /// </summary>
    //    DrawBuffer14 = ((int)0x8833),
    //    /// <summary>
    //    /// Original was GL_DRAW_BUFFER15 = 0x8834
    //    /// </summary>
    //    DrawBuffer15 = ((int)0x8834),
    //    /// <summary>
    //    /// Original was GL_BLEND_EQUATION_ALPHA = 0x883D
    //    /// </summary>
    //    BlendEquationAlpha = ((int)0x883D),
    //    /// <summary>
    //    /// Original was GL_TEXTURE_CUBE_MAP_SEAMLESS = 0x884F
    //    /// </summary>
    //    TextureCubeMapSeamless = ((int)0x884F),
    //    /// <summary>
    //    /// Original was GL_POINT_SPRITE = 0x8861
    //    /// </summary>
    //    PointSprite = ((int)0x8861),
    //    /// <summary>
    //    /// Original was GL_MAX_VERTEX_ATTRIBS = 0x8869
    //    /// </summary>
    //    MaxVertexAttribs = ((int)0x8869),
    //    /// <summary>
    //    /// Original was GL_MAX_TESS_CONTROL_INPUT_COMPONENTS = 0x886C
    //    /// </summary>
    //    MaxTessControlInputComponents = ((int)0x886C),
    //    /// <summary>
    //    /// Original was GL_MAX_TESS_EVALUATION_INPUT_COMPONENTS = 0x886D
    //    /// </summary>
    //    MaxTessEvaluationInputComponents = ((int)0x886D),
    //    /// <summary>
    //    /// Original was GL_MAX_TEXTURE_COORDS = 0x8871
    //    /// </summary>
    //    MaxTextureCoords = ((int)0x8871),
    //    /// <summary>
    //    /// Original was GL_MAX_TEXTURE_IMAGE_UNITS = 0x8872
    //    /// </summary>
    //    MaxTextureImageUnits = ((int)0x8872),
    //    /// <summary>
    //    /// Original was GL_ARRAY_BUFFER_BINDING = 0x8894
    //    /// </summary>
    //    ArrayBufferBinding = ((int)0x8894),
    //    /// <summary>
    //    /// Original was GL_ELEMENT_ARRAY_BUFFER_BINDING = 0x8895
    //    /// </summary>
    //    ElementArrayBufferBinding = ((int)0x8895),
    //    /// <summary>
    //    /// Original was GL_VERTEX_ARRAY_BUFFER_BINDING = 0x8896
    //    /// </summary>
    //    VertexArrayBufferBinding = ((int)0x8896),
    //    /// <summary>
    //    /// Original was GL_NORMAL_ARRAY_BUFFER_BINDING = 0x8897
    //    /// </summary>
    //    NormalArrayBufferBinding = ((int)0x8897),
    //    /// <summary>
    //    /// Original was GL_COLOR_ARRAY_BUFFER_BINDING = 0x8898
    //    /// </summary>
    //    ColorArrayBufferBinding = ((int)0x8898),
    //    /// <summary>
    //    /// Original was GL_INDEX_ARRAY_BUFFER_BINDING = 0x8899
    //    /// </summary>
    //    IndexArrayBufferBinding = ((int)0x8899),
    //    /// <summary>
    //    /// Original was GL_TEXTURE_COORD_ARRAY_BUFFER_BINDING = 0x889A
    //    /// </summary>
    //    TextureCoordArrayBufferBinding = ((int)0x889A),
    //    /// <summary>
    //    /// Original was GL_EDGE_FLAG_ARRAY_BUFFER_BINDING = 0x889B
    //    /// </summary>
    //    EdgeFlagArrayBufferBinding = ((int)0x889B),
    //    /// <summary>
    //    /// Original was GL_SECONDARY_COLOR_ARRAY_BUFFER_BINDING = 0x889C
    //    /// </summary>
    //    SecondaryColorArrayBufferBinding = ((int)0x889C),
    //    /// <summary>
    //    /// Original was GL_FOG_COORD_ARRAY_BUFFER_BINDING = 0x889D
    //    /// </summary>
    //    FogCoordArrayBufferBinding = ((int)0x889D),
    //    /// <summary>
    //    /// Original was GL_WEIGHT_ARRAY_BUFFER_BINDING = 0x889E
    //    /// </summary>
    //    WeightArrayBufferBinding = ((int)0x889E),
    //    /// <summary>
    //    /// Original was GL_VERTEX_ATTRIB_ARRAY_BUFFER_BINDING = 0x889F
    //    /// </summary>
    //    VertexAttribArrayBufferBinding = ((int)0x889F),
    //    /// <summary>
    //    /// Original was GL_PIXEL_PACK_BUFFER_BINDING = 0x88ED
    //    /// </summary>
    //    PixelPackBufferBinding = ((int)0x88ED),
    //    /// <summary>
    //    /// Original was GL_PIXEL_UNPACK_BUFFER_BINDING = 0x88EF
    //    /// </summary>
    //    PixelUnpackBufferBinding = ((int)0x88EF),
    //    /// <summary>
    //    /// Original was GL_MAX_DUAL_SOURCE_DRAW_BUFFERS = 0x88FC
    //    /// </summary>
    //    MaxDualSourceDrawBuffers = ((int)0x88FC),
    //    /// <summary>
    //    /// Original was GL_MAX_ARRAY_TEXTURE_LAYERS = 0x88FF
    //    /// </summary>
    //    MaxArrayTextureLayers = ((int)0x88FF),
    //    /// <summary>
    //    /// Original was GL_MIN_PROGRAM_TEXEL_OFFSET = 0x8904
    //    /// </summary>
    //    MinProgramTexelOffset = ((int)0x8904),
    //    /// <summary>
    //    /// Original was GL_MAX_PROGRAM_TEXEL_OFFSET = 0x8905
    //    /// </summary>
    //    MaxProgramTexelOffset = ((int)0x8905),
    //    /// <summary>
    //    /// Original was GL_SAMPLER_BINDING = 0x8919
    //    /// </summary>
    //    SamplerBinding = ((int)0x8919),
    //    /// <summary>
    //    /// Original was GL_CLAMP_VERTEX_COLOR = 0x891A
    //    /// </summary>
    //    ClampVertexColor = ((int)0x891A),
    //    /// <summary>
    //    /// Original was GL_CLAMP_FRAGMENT_COLOR = 0x891B
    //    /// </summary>
    //    ClampFragmentColor = ((int)0x891B),
    //    /// <summary>
    //    /// Original was GL_CLAMP_READ_COLOR = 0x891C
    //    /// </summary>
    //    ClampReadColor = ((int)0x891C),
    //    /// <summary>
    //    /// Original was GL_MAX_VERTEX_UNIFORM_BLOCKS = 0x8A2B
    //    /// </summary>
    //    MaxVertexUniformBlocks = ((int)0x8A2B),
    //    /// <summary>
    //    /// Original was GL_MAX_GEOMETRY_UNIFORM_BLOCKS = 0x8A2C
    //    /// </summary>
    //    MaxGeometryUniformBlocks = ((int)0x8A2C),
    //    /// <summary>
    //    /// Original was GL_MAX_FRAGMENT_UNIFORM_BLOCKS = 0x8A2D
    //    /// </summary>
    //    MaxFragmentUniformBlocks = ((int)0x8A2D),
    //    /// <summary>
    //    /// Original was GL_MAX_COMBINED_UNIFORM_BLOCKS = 0x8A2E
    //    /// </summary>
    //    MaxCombinedUniformBlocks = ((int)0x8A2E),
    //    /// <summary>
    //    /// Original was GL_MAX_UNIFORM_BUFFER_BINDINGS = 0x8A2F
    //    /// </summary>
    //    MaxUniformBufferBindings = ((int)0x8A2F),
    //    /// <summary>
    //    /// Original was GL_MAX_UNIFORM_BLOCK_SIZE = 0x8A30
    //    /// </summary>
    //    MaxUniformBlockSize = ((int)0x8A30),
    //    /// <summary>
    //    /// Original was GL_MAX_COMBINED_VERTEX_UNIFORM_COMPONENTS = 0x8A31
    //    /// </summary>
    //    MaxCombinedVertexUniformComponents = ((int)0x8A31),
    //    /// <summary>
    //    /// Original was GL_MAX_COMBINED_GEOMETRY_UNIFORM_COMPONENTS = 0x8A32
    //    /// </summary>
    //    MaxCombinedGeometryUniformComponents = ((int)0x8A32),
    //    /// <summary>
    //    /// Original was GL_MAX_COMBINED_FRAGMENT_UNIFORM_COMPONENTS = 0x8A33
    //    /// </summary>
    //    MaxCombinedFragmentUniformComponents = ((int)0x8A33),
    //    /// <summary>
    //    /// Original was GL_UNIFORM_BUFFER_OFFSET_ALIGNMENT = 0x8A34
    //    /// </summary>
    //    UniformBufferOffsetAlignment = ((int)0x8A34),
    //    /// <summary>
    //    /// Original was GL_MAX_FRAGMENT_UNIFORM_COMPONENTS = 0x8B49
    //    /// </summary>
    //    MaxFragmentUniformComponents = ((int)0x8B49),
    //    /// <summary>
    //    /// Original was GL_MAX_VERTEX_UNIFORM_COMPONENTS = 0x8B4A
    //    /// </summary>
    //    MaxVertexUniformComponents = ((int)0x8B4A),
    //    /// <summary>
    //    /// Original was GL_MAX_VARYING_COMPONENTS = 0x8B4B
    //    /// </summary>
    //    MaxVaryingComponents = ((int)0x8B4B),
    //    /// <summary>
    //    /// Original was GL_MAX_VARYING_FLOATS = 0x8B4B
    //    /// </summary>
    //    MaxVaryingFloats = ((int)0x8B4B),
    //    /// <summary>
    //    /// Original was GL_MAX_VERTEX_TEXTURE_IMAGE_UNITS = 0x8B4C
    //    /// </summary>
    //    MaxVertexTextureImageUnits = ((int)0x8B4C),
    //    /// <summary>
    //    /// Original was GL_MAX_COMBINED_TEXTURE_IMAGE_UNITS = 0x8B4D
    //    /// </summary>
    //    MaxCombinedTextureImageUnits = ((int)0x8B4D),
    //    /// <summary>
    //    /// Original was GL_FRAGMENT_SHADER_DERIVATIVE_HINT = 0x8B8B
    //    /// </summary>
    //    FragmentShaderDerivativeHint = ((int)0x8B8B),
    //    /// <summary>
    //    /// Original was GL_CURRENT_PROGRAM = 0x8B8D
    //    /// </summary>
    //    CurrentProgram = ((int)0x8B8D),
    //    /// <summary>
    //    /// Original was GL_IMPLEMENTATION_COLOR_READ_TYPE = 0x8B9A
    //    /// </summary>
    //    ImplementationColorReadType = ((int)0x8B9A),
    //    /// <summary>
    //    /// Original was GL_IMPLEMENTATION_COLOR_READ_FORMAT = 0x8B9B
    //    /// </summary>
    //    ImplementationColorReadFormat = ((int)0x8B9B),
    //    /// <summary>
    //    /// Original was GL_TEXTURE_BINDING_1D_ARRAY = 0x8C1C
    //    /// </summary>
    //    TextureBinding1DArray = ((int)0x8C1C),
    //    /// <summary>
    //    /// Original was GL_TEXTURE_BINDING_2D_ARRAY = 0x8C1D
    //    /// </summary>
    //    TextureBinding2DArray = ((int)0x8C1D),
    //    /// <summary>
    //    /// Original was GL_MAX_GEOMETRY_TEXTURE_IMAGE_UNITS = 0x8C29
    //    /// </summary>
    //    MaxGeometryTextureImageUnits = ((int)0x8C29),
    //    /// <summary>
    //    /// Original was GL_TEXTURE_BUFFER = 0x8C2A
    //    /// </summary>
    //    TextureBuffer = ((int)0x8C2A),
    //    /// <summary>
    //    /// Original was GL_MAX_TEXTURE_BUFFER_SIZE = 0x8C2B
    //    /// </summary>
    //    MaxTextureBufferSize = ((int)0x8C2B),
    //    /// <summary>
    //    /// Original was GL_TEXTURE_BINDING_BUFFER = 0x8C2C
    //    /// </summary>
    //    TextureBindingBuffer = ((int)0x8C2C),
    //    /// <summary>
    //    /// Original was GL_TEXTURE_BUFFER_DATA_STORE_BINDING = 0x8C2D
    //    /// </summary>
    //    TextureBufferDataStoreBinding = ((int)0x8C2D),
    //    /// <summary>
    //    /// Original was GL_SAMPLE_SHADING = 0x8C36
    //    /// </summary>
    //    SampleShading = ((int)0x8C36),
    //    /// <summary>
    //    /// Original was GL_MIN_SAMPLE_SHADING_VALUE = 0x8C37
    //    /// </summary>
    //    MinSampleShadingValue = ((int)0x8C37),
    //    /// <summary>
    //    /// Original was GL_MAX_TRANSFORM_FEEDBACK_SEPARATE_COMPONENTS = 0x8C80
    //    /// </summary>
    //    MaxTransformFeedbackSeparateComponents = ((int)0x8C80),
    //    /// <summary>
    //    /// Original was GL_MAX_TRANSFORM_FEEDBACK_INTERLEAVED_COMPONENTS = 0x8C8A
    //    /// </summary>
    //    MaxTransformFeedbackInterleavedComponents = ((int)0x8C8A),
    //    /// <summary>
    //    /// Original was GL_MAX_TRANSFORM_FEEDBACK_SEPARATE_ATTRIBS = 0x8C8B
    //    /// </summary>
    //    MaxTransformFeedbackSeparateAttribs = ((int)0x8C8B),
    //    /// <summary>
    //    /// Original was GL_STENCIL_BACK_REF = 0x8CA3
    //    /// </summary>
    //    StencilBackRef = ((int)0x8CA3),
    //    /// <summary>
    //    /// Original was GL_STENCIL_BACK_VALUE_MASK = 0x8CA4
    //    /// </summary>
    //    StencilBackValueMask = ((int)0x8CA4),
    //    /// <summary>
    //    /// Original was GL_STENCIL_BACK_WRITEMASK = 0x8CA5
    //    /// </summary>
    //    StencilBackWritemask = ((int)0x8CA5),
    //    /// <summary>
    //    /// Original was GL_DRAW_FRAMEBUFFER_BINDING = 0x8CA6
    //    /// </summary>
    //    DrawFramebufferBinding = ((int)0x8CA6),
    //    /// <summary>
    //    /// Original was GL_FRAMEBUFFER_BINDING = 0x8CA6
    //    /// </summary>
    //    FramebufferBinding = ((int)0x8CA6),
    //    /// <summary>
    //    /// Original was GL_FRAMEBUFFER_BINDING_EXT = 0x8CA6
    //    /// </summary>
    //    FramebufferBindingExt = ((int)0x8CA6),
    //    /// <summary>
    //    /// Original was GL_RENDERBUFFER_BINDING = 0x8CA7
    //    /// </summary>
    //    RenderbufferBinding = ((int)0x8CA7),
    //    /// <summary>
    //    /// Original was GL_RENDERBUFFER_BINDING_EXT = 0x8CA7
    //    /// </summary>
    //    RenderbufferBindingExt = ((int)0x8CA7),
    //    /// <summary>
    //    /// Original was GL_READ_FRAMEBUFFER_BINDING = 0x8CAA
    //    /// </summary>
    //    ReadFramebufferBinding = ((int)0x8CAA),
    //    /// <summary>
    //    /// Original was GL_MAX_COLOR_ATTACHMENTS = 0x8CDF
    //    /// </summary>
    //    MaxColorAttachments = ((int)0x8CDF),
    //    /// <summary>
    //    /// Original was GL_MAX_COLOR_ATTACHMENTS_EXT = 0x8CDF
    //    /// </summary>
    //    MaxColorAttachmentsExt = ((int)0x8CDF),
    //    /// <summary>
    //    /// Original was GL_MAX_SAMPLES = 0x8D57
    //    /// </summary>
    //    MaxSamples = ((int)0x8D57),
    //    /// <summary>
    //    /// Original was GL_FRAMEBUFFER_SRGB = 0x8DB9
    //    /// </summary>
    //    FramebufferSrgb = ((int)0x8DB9),
    //    /// <summary>
    //    /// Original was GL_MAX_GEOMETRY_VARYING_COMPONENTS = 0x8DDD
    //    /// </summary>
    //    MaxGeometryVaryingComponents = ((int)0x8DDD),
    //    /// <summary>
    //    /// Original was GL_MAX_VERTEX_VARYING_COMPONENTS = 0x8DDE
    //    /// </summary>
    //    MaxVertexVaryingComponents = ((int)0x8DDE),
    //    /// <summary>
    //    /// Original was GL_MAX_GEOMETRY_UNIFORM_COMPONENTS = 0x8DDF
    //    /// </summary>
    //    MaxGeometryUniformComponents = ((int)0x8DDF),
    //    /// <summary>
    //    /// Original was GL_MAX_GEOMETRY_OUTPUT_VERTICES = 0x8DE0
    //    /// </summary>
    //    MaxGeometryOutputVertices = ((int)0x8DE0),
    //    /// <summary>
    //    /// Original was GL_MAX_GEOMETRY_TOTAL_OUTPUT_COMPONENTS = 0x8DE1
    //    /// </summary>
    //    MaxGeometryTotalOutputComponents = ((int)0x8DE1),
    //    /// <summary>
    //    /// Original was GL_MAX_SUBROUTINES = 0x8DE7
    //    /// </summary>
    //    MaxSubroutines = ((int)0x8DE7),
    //    /// <summary>
    //    /// Original was GL_MAX_SUBROUTINE_UNIFORM_LOCATIONS = 0x8DE8
    //    /// </summary>
    //    MaxSubroutineUniformLocations = ((int)0x8DE8),
    //    /// <summary>
    //    /// Original was GL_SHADER_BINARY_FORMATS = 0x8DF8
    //    /// </summary>
    //    ShaderBinaryFormats = ((int)0x8DF8),
    //    /// <summary>
    //    /// Original was GL_NUM_SHADER_BINARY_FORMATS = 0x8DF9
    //    /// </summary>
    //    NumShaderBinaryFormats = ((int)0x8DF9),
    //    /// <summary>
    //    /// Original was GL_SHADER_COMPILER = 0x8DFA
    //    /// </summary>
    //    ShaderCompiler = ((int)0x8DFA),
    //    /// <summary>
    //    /// Original was GL_MAX_VERTEX_UNIFORM_VECTORS = 0x8DFB
    //    /// </summary>
    //    MaxVertexUniformVectors = ((int)0x8DFB),
    //    /// <summary>
    //    /// Original was GL_MAX_VARYING_VECTORS = 0x8DFC
    //    /// </summary>
    //    MaxVaryingVectors = ((int)0x8DFC),
    //    /// <summary>
    //    /// Original was GL_MAX_FRAGMENT_UNIFORM_VECTORS = 0x8DFD
    //    /// </summary>
    //    MaxFragmentUniformVectors = ((int)0x8DFD),
    //    /// <summary>
    //    /// Original was GL_MAX_COMBINED_TESS_CONTROL_UNIFORM_COMPONENTS = 0x8E1E
    //    /// </summary>
    //    MaxCombinedTessControlUniformComponents = ((int)0x8E1E),
    //    /// <summary>
    //    /// Original was GL_MAX_COMBINED_TESS_EVALUATION_UNIFORM_COMPONENTS = 0x8E1F
    //    /// </summary>
    //    MaxCombinedTessEvaluationUniformComponents = ((int)0x8E1F),
    //    /// <summary>
    //    /// Original was GL_TRANSFORM_FEEDBACK_BUFFER_PAUSED = 0x8E23
    //    /// </summary>
    //    TransformFeedbackBufferPaused = ((int)0x8E23),
    //    /// <summary>
    //    /// Original was GL_TRANSFORM_FEEDBACK_BUFFER_ACTIVE = 0x8E24
    //    /// </summary>
    //    TransformFeedbackBufferActive = ((int)0x8E24),
    //    /// <summary>
    //    /// Original was GL_TRANSFORM_FEEDBACK_BINDING = 0x8E25
    //    /// </summary>
    //    TransformFeedbackBinding = ((int)0x8E25),
    //    /// <summary>
    //    /// Original was GL_TIMESTAMP = 0x8E28
    //    /// </summary>
    //    Timestamp = ((int)0x8E28),
    //    /// <summary>
    //    /// Original was GL_QUADS_FOLLOW_PROVOKING_VERTEX_CONVENTION = 0x8E4C
    //    /// </summary>
    //    QuadsFollowProvokingVertexConvention = ((int)0x8E4C),
    //    /// <summary>
    //    /// Original was GL_PROVOKING_VERTEX = 0x8E4F
    //    /// </summary>
    //    ProvokingVertex = ((int)0x8E4F),
    //    /// <summary>
    //    /// Original was GL_SAMPLE_MASK = 0x8E51
    //    /// </summary>
    //    SampleMask = ((int)0x8E51),
    //    /// <summary>
    //    /// Original was GL_MAX_SAMPLE_MASK_WORDS = 0x8E59
    //    /// </summary>
    //    MaxSampleMaskWords = ((int)0x8E59),
    //    /// <summary>
    //    /// Original was GL_MAX_GEOMETRY_SHADER_INVOCATIONS = 0x8E5A
    //    /// </summary>
    //    MaxGeometryShaderInvocations = ((int)0x8E5A),
    //    /// <summary>
    //    /// Original was GL_MIN_FRAGMENT_INTERPOLATION_OFFSET = 0x8E5B
    //    /// </summary>
    //    MinFragmentInterpolationOffset = ((int)0x8E5B),
    //    /// <summary>
    //    /// Original was GL_MAX_FRAGMENT_INTERPOLATION_OFFSET = 0x8E5C
    //    /// </summary>
    //    MaxFragmentInterpolationOffset = ((int)0x8E5C),
    //    /// <summary>
    //    /// Original was GL_FRAGMENT_INTERPOLATION_OFFSET_BITS = 0x8E5D
    //    /// </summary>
    //    FragmentInterpolationOffsetBits = ((int)0x8E5D),
    //    /// <summary>
    //    /// Original was GL_MIN_PROGRAM_TEXTURE_GATHER_OFFSET = 0x8E5E
    //    /// </summary>
    //    MinProgramTextureGatherOffset = ((int)0x8E5E),
    //    /// <summary>
    //    /// Original was GL_MAX_PROGRAM_TEXTURE_GATHER_OFFSET = 0x8E5F
    //    /// </summary>
    //    MaxProgramTextureGatherOffset = ((int)0x8E5F),
    //    /// <summary>
    //    /// Original was GL_MAX_TRANSFORM_FEEDBACK_BUFFERS = 0x8E70
    //    /// </summary>
    //    MaxTransformFeedbackBuffers = ((int)0x8E70),
    //    /// <summary>
    //    /// Original was GL_MAX_VERTEX_STREAMS = 0x8E71
    //    /// </summary>
    //    MaxVertexStreams = ((int)0x8E71),
    //    /// <summary>
    //    /// Original was GL_PATCH_VERTICES = 0x8E72
    //    /// </summary>
    //    PatchVertices = ((int)0x8E72),
    //    /// <summary>
    //    /// Original was GL_PATCH_DEFAULT_INNER_LEVEL = 0x8E73
    //    /// </summary>
    //    PatchDefaultInnerLevel = ((int)0x8E73),
    //    /// <summary>
    //    /// Original was GL_PATCH_DEFAULT_OUTER_LEVEL = 0x8E74
    //    /// </summary>
    //    PatchDefaultOuterLevel = ((int)0x8E74),
    //    /// <summary>
    //    /// Original was GL_MAX_PATCH_VERTICES = 0x8E7D
    //    /// </summary>
    //    MaxPatchVertices = ((int)0x8E7D),
    //    /// <summary>
    //    /// Original was GL_MAX_TESS_GEN_LEVEL = 0x8E7E
    //    /// </summary>
    //    MaxTessGenLevel = ((int)0x8E7E),
    //    /// <summary>
    //    /// Original was GL_MAX_TESS_CONTROL_UNIFORM_COMPONENTS = 0x8E7F
    //    /// </summary>
    //    MaxTessControlUniformComponents = ((int)0x8E7F),
    //    /// <summary>
    //    /// Original was GL_MAX_TESS_EVALUATION_UNIFORM_COMPONENTS = 0x8E80
    //    /// </summary>
    //    MaxTessEvaluationUniformComponents = ((int)0x8E80),
    //    /// <summary>
    //    /// Original was GL_MAX_TESS_CONTROL_TEXTURE_IMAGE_UNITS = 0x8E81
    //    /// </summary>
    //    MaxTessControlTextureImageUnits = ((int)0x8E81),
    //    /// <summary>
    //    /// Original was GL_MAX_TESS_EVALUATION_TEXTURE_IMAGE_UNITS = 0x8E82
    //    /// </summary>
    //    MaxTessEvaluationTextureImageUnits = ((int)0x8E82),
    //    /// <summary>
    //    /// Original was GL_MAX_TESS_CONTROL_OUTPUT_COMPONENTS = 0x8E83
    //    /// </summary>
    //    MaxTessControlOutputComponents = ((int)0x8E83),
    //    /// <summary>
    //    /// Original was GL_MAX_TESS_PATCH_COMPONENTS = 0x8E84
    //    /// </summary>
    //    MaxTessPatchComponents = ((int)0x8E84),
    //    /// <summary>
    //    /// Original was GL_MAX_TESS_CONTROL_TOTAL_OUTPUT_COMPONENTS = 0x8E85
    //    /// </summary>
    //    MaxTessControlTotalOutputComponents = ((int)0x8E85),
    //    /// <summary>
    //    /// Original was GL_MAX_TESS_EVALUATION_OUTPUT_COMPONENTS = 0x8E86
    //    /// </summary>
    //    MaxTessEvaluationOutputComponents = ((int)0x8E86),
    //    /// <summary>
    //    /// Original was GL_MAX_TESS_CONTROL_UNIFORM_BLOCKS = 0x8E89
    //    /// </summary>
    //    MaxTessControlUniformBlocks = ((int)0x8E89),
    //    /// <summary>
    //    /// Original was GL_MAX_TESS_EVALUATION_UNIFORM_BLOCKS = 0x8E8A
    //    /// </summary>
    //    MaxTessEvaluationUniformBlocks = ((int)0x8E8A),
    //    /// <summary>
    //    /// Original was GL_DRAW_INDIRECT_BUFFER_BINDING = 0x8F43
    //    /// </summary>
    //    DrawIndirectBufferBinding = ((int)0x8F43),
    //    /// <summary>
    //    /// Original was GL_MAX_VERTEX_IMAGE_UNIFORMS = 0x90CA
    //    /// </summary>
    //    MaxVertexImageUniforms = ((int)0x90CA),
    //    /// <summary>
    //    /// Original was GL_MAX_TESS_CONTROL_IMAGE_UNIFORMS = 0x90CB
    //    /// </summary>
    //    MaxTessControlImageUniforms = ((int)0x90CB),
    //    /// <summary>
    //    /// Original was GL_MAX_TESS_EVALUATION_IMAGE_UNIFORMS = 0x90CC
    //    /// </summary>
    //    MaxTessEvaluationImageUniforms = ((int)0x90CC),
    //    /// <summary>
    //    /// Original was GL_MAX_GEOMETRY_IMAGE_UNIFORMS = 0x90CD
    //    /// </summary>
    //    MaxGeometryImageUniforms = ((int)0x90CD),
    //    /// <summary>
    //    /// Original was GL_MAX_FRAGMENT_IMAGE_UNIFORMS = 0x90CE
    //    /// </summary>
    //    MaxFragmentImageUniforms = ((int)0x90CE),
    //    /// <summary>
    //    /// Original was GL_MAX_COMBINED_IMAGE_UNIFORMS = 0x90CF
    //    /// </summary>
    //    MaxCombinedImageUniforms = ((int)0x90CF),
    //    /// <summary>
    //    /// Original was GL_CONTEXT_ROBUST_ACCESS = 0x90F3
    //    /// </summary>
    //    ContextRobustAccess = ((int)0x90F3),
    //    /// <summary>
    //    /// Original was GL_TEXTURE_BINDING_2D_MULTISAMPLE = 0x9104
    //    /// </summary>
    //    TextureBinding2DMultisample = ((int)0x9104),
    //    /// <summary>
    //    /// Original was GL_TEXTURE_BINDING_2D_MULTISAMPLE_ARRAY = 0x9105
    //    /// </summary>
    //    TextureBinding2DMultisampleArray = ((int)0x9105),
    //    /// <summary>
    //    /// Original was GL_MAX_COLOR_TEXTURE_SAMPLES = 0x910E
    //    /// </summary>
    //    MaxColorTextureSamples = ((int)0x910E),
    //    /// <summary>
    //    /// Original was GL_MAX_DEPTH_TEXTURE_SAMPLES = 0x910F
    //    /// </summary>
    //    MaxDepthTextureSamples = ((int)0x910F),
    //    /// <summary>
    //    /// Original was GL_MAX_INTEGER_SAMPLES = 0x9110
    //    /// </summary>
    //    MaxIntegerSamples = ((int)0x9110),
    //    /// <summary>
    //    /// Original was GL_MAX_VERTEX_OUTPUT_COMPONENTS = 0x9122
    //    /// </summary>
    //    MaxVertexOutputComponents = ((int)0x9122),
    //    /// <summary>
    //    /// Original was GL_MAX_GEOMETRY_INPUT_COMPONENTS = 0x9123
    //    /// </summary>
    //    MaxGeometryInputComponents = ((int)0x9123),
    //    /// <summary>
    //    /// Original was GL_MAX_GEOMETRY_OUTPUT_COMPONENTS = 0x9124
    //    /// </summary>
    //    MaxGeometryOutputComponents = ((int)0x9124),
    //    /// <summary>
    //    /// Original was GL_MAX_FRAGMENT_INPUT_COMPONENTS = 0x9125
    //    /// </summary>
    //    MaxFragmentInputComponents = ((int)0x9125),
    //    /// <summary>
    //    /// Original was GL_MAX_COMPUTE_IMAGE_UNIFORMS = 0x91BD
    //    /// </summary>
    //    MaxComputeImageUniforms = ((int)0x91BD),
    //    /// <summary>
    //    /// Original was GL_CLIP_ORIGIN = 0x935C
    //    /// </summary>
    //    ClipOrigin = ((int)0x935C),
    //    /// <summary>
    //    /// Original was GL_CLIP_DEPTH_MODE = 0x935D
    //    /// </summary>
    //    ClipDepthMode = ((int)0x935D),
    //}

    public enum GetPName
    {
        CurrentColor = 2816,
        CurrentIndex,
        CurrentNormal,
        CurrentTextureCoords,
        CurrentRasterColor,
        CurrentRasterIndex,
        CurrentRasterTextureCoords,
        CurrentRasterPosition,
        CurrentRasterPositionValid,
        CurrentRasterDistance,
        PointSmooth = 2832,
        PointSize,
        PointSizeRange,
        SmoothPointSizeRange = 2834,
        PointSizeGranularity,
        SmoothPointSizeGranularity = 2835,
        LineSmooth = 2848,
        LineWidth,
        LineWidthRange,
        SmoothLineWidthRange = 2850,
        LineWidthGranularity,
        SmoothLineWidthGranularity = 2851,
        LineStipple,
        LineStipplePattern,
        LineStippleRepeat,
        ListMode = 2864,
        MaxListNesting,
        ListBase,
        ListIndex,
        PolygonMode = 2880,
        PolygonSmooth,
        PolygonStipple,
        EdgeFlag,
        CullFace,
        CullFaceMode,
        FrontFace,
        Lighting = 2896,
        LightModelLocalViewer,
        LightModelTwoSide,
        LightModelAmbient,
        ShadeModel,
        ColorMaterialFace,
        ColorMaterialParameter,
        ColorMaterial,
        Fog = 2912,
        FogIndex,
        FogDensity,
        FogStart,
        FogEnd,
        FogMode,
        FogColor,
        DepthRange = 2928,
        DepthTest,
        DepthWritemask,
        DepthClearValue,
        DepthFunc,
        AccumClearValue = 2944,
        StencilTest = 2960,
        StencilClearValue,
        StencilFunc,
        StencilValueMask,
        StencilFail,
        StencilPassDepthFail,
        StencilPassDepthPass,
        StencilRef,
        StencilWritemask,
        MatrixMode = 2976,
        Normalize,
        Viewport,
        Modelview0StackDepthExt,
        ModelviewStackDepth = 2979,
        ProjectionStackDepth,
        TextureStackDepth,
        Modelview0MatrixExt,
        ModelviewMatrix = 2982,
        ProjectionMatrix,
        TextureMatrix,
        AttribStackDepth = 2992,
        ClientAttribStackDepth,
        AlphaTest = 3008,
        AlphaTestQcom = 3008,
        AlphaTestFunc,
        AlphaTestFuncQcom = 3009,
        AlphaTestRef,
        AlphaTestRefQcom = 3010,
        Dither = 3024,
        BlendDst = 3040,
        BlendSrc,
        Blend,
        LogicOpMode = 3056,
        IndexLogicOp,
        LogicOp = 3057,
        ColorLogicOp,
        AuxBuffers = 3072,
        DrawBuffer,
        DrawBufferExt = 3073,
        ReadBuffer,
        ReadBufferExt = 3074,
        ReadBufferNv = 3074,
        ScissorBox = 3088,
        ScissorTest,
        IndexClearValue = 3104,
        IndexWritemask,
        ColorClearValue,
        ColorWritemask,
        IndexMode = 3120,
        RgbaMode,
        Doublebuffer,
        Stereo,
        RenderMode = 3136,
        PerspectiveCorrectionHint = 3152,
        PointSmoothHint,
        LineSmoothHint,
        PolygonSmoothHint,
        FogHint,
        TextureGenS = 3168,
        TextureGenT,
        TextureGenR,
        TextureGenQ,
        PixelMapIToISize = 3248,
        PixelMapSToSSize,
        PixelMapIToRSize,
        PixelMapIToGSize,
        PixelMapIToBSize,
        PixelMapIToASize,
        PixelMapRToRSize,
        PixelMapGToGSize,
        PixelMapBToBSize,
        PixelMapAToASize,
        UnpackSwapBytes = 3312,
        UnpackLsbFirst,
        UnpackRowLength,
        UnpackSkipRows,
        UnpackSkipPixels,
        UnpackAlignment,
        PackSwapBytes = 3328,
        PackLsbFirst,
        PackRowLength,
        PackSkipRows,
        PackSkipPixels,
        PackAlignment,
        MapColor = 3344,
        MapStencil,
        IndexShift,
        IndexOffset,
        RedScale,
        RedBias,
        ZoomX,
        ZoomY,
        GreenScale,
        GreenBias,
        BlueScale,
        BlueBias,
        AlphaScale,
        AlphaBias,
        DepthScale,
        DepthBias,
        MaxEvalOrder = 3376,
        MaxLights,
        MaxClipDistances,
        MaxClipPlanes = 3378,
        MaxTextureSize,
        MaxPixelMapTable,
        MaxAttribStackDepth,
        MaxModelviewStackDepth,
        MaxNameStackDepth,
        MaxProjectionStackDepth,
        MaxTextureStackDepth,
        MaxViewportDims,
        MaxClientAttribStackDepth,
        SubpixelBits = 3408,
        IndexBits,
        RedBits,
        GreenBits,
        BlueBits,
        AlphaBits,
        DepthBits,
        StencilBits,
        AccumRedBits,
        AccumGreenBits,
        AccumBlueBits,
        AccumAlphaBits,
        NameStackDepth = 3440,
        AutoNormal = 3456,
        Map1Color4 = 3472,
        Map1Index,
        Map1Normal,
        Map1TextureCoord1,
        Map1TextureCoord2,
        Map1TextureCoord3,
        Map1TextureCoord4,
        Map1Vertex3,
        Map1Vertex4,
        Map2Color4 = 3504,
        Map2Index,
        Map2Normal,
        Map2TextureCoord1,
        Map2TextureCoord2,
        Map2TextureCoord3,
        Map2TextureCoord4,
        Map2Vertex3,
        Map2Vertex4,
        Map1GridDomain = 3536,
        Map1GridSegments,
        Map2GridDomain,
        Map2GridSegments,
        Texture1D = 3552,
        Texture2D,
        FeedbackBufferSize = 3569,
        FeedbackBufferType,
        SelectionBufferSize = 3572,
        PolygonOffsetUnits = 10752,
        PolygonOffsetPoint,
        PolygonOffsetLine,
        ClipPlane0 = 12288,
        ClipPlane1,
        ClipPlane2,
        ClipPlane3,
        ClipPlane4,
        ClipPlane5,
        Light0 = 16384,
        Light1,
        Light2,
        Light3,
        Light4,
        Light5,
        Light6,
        Light7,
        BlendColorExt = 32773,
        BlendEquationExt = 32777,
        BlendEquationRgb = 32777,
        PackCmykHintExt = 32782,
        UnpackCmykHintExt,
        Convolution1DExt,
        Convolution2DExt,
        Separable2DExt,
        PostConvolutionRedScaleExt = 32796,
        PostConvolutionGreenScaleExt,
        PostConvolutionBlueScaleExt,
        PostConvolutionAlphaScaleExt,
        PostConvolutionRedBiasExt,
        PostConvolutionGreenBiasExt,
        PostConvolutionBlueBiasExt,
        PostConvolutionAlphaBiasExt,
        HistogramExt,
        MinmaxExt = 32814,
        PolygonOffsetFill = 32823,
        PolygonOffsetFactor,
        PolygonOffsetBiasExt,
        RescaleNormalExt,
        TextureBinding1D = 32872,
        TextureBinding2D,
        Texture3DBindingExt,
        TextureBinding3D = 32874,
        PackSkipImagesExt,
        PackImageHeightExt,
        UnpackSkipImagesExt,
        UnpackImageHeightExt,
        Texture3DExt,
        Max3DTextureSize = 32883,
        Max3DTextureSizeExt = 32883,
        VertexArray,
        NormalArray,
        ColorArray,
        IndexArray,
        TextureCoordArray,
        EdgeFlagArray,
        VertexArraySize,
        VertexArrayType,
        VertexArrayStride,
        VertexArrayCountExt,
        NormalArrayType,
        NormalArrayStride,
        NormalArrayCountExt,
        ColorArraySize,
        ColorArrayType,
        ColorArrayStride,
        ColorArrayCountExt,
        IndexArrayType,
        IndexArrayStride,
        IndexArrayCountExt,
        TextureCoordArraySize,
        TextureCoordArrayType,
        TextureCoordArrayStride,
        TextureCoordArrayCountExt,
        EdgeFlagArrayStride,
        EdgeFlagArrayCountExt,
        InterlaceSgix = 32916,
        DetailTexture2DBindingSgis = 32918,
        Multisample = 32925,
        MultisampleSgis = 32925,
        SampleAlphaToCoverage,
        SampleAlphaToMaskSgis = 32926,
        SampleAlphaToOne,
        SampleAlphaToOneSgis = 32927,
        SampleCoverage,
        SampleMaskSgis = 32928,
        SampleBuffers = 32936,
        SampleBuffersSgis = 32936,
        Samples,
        SamplesSgis = 32937,
        SampleCoverageValue,
        SampleMaskValueSgis = 32938,
        SampleCoverageInvert,
        SampleMaskInvertSgis = 32939,
        SamplePatternSgis,
        ColorMatrixSgi = 32945,
        ColorMatrixStackDepthSgi,
        MaxColorMatrixStackDepthSgi,
        PostColorMatrixRedScaleSgi,
        PostColorMatrixGreenScaleSgi,
        PostColorMatrixBlueScaleSgi,
        PostColorMatrixAlphaScaleSgi,
        PostColorMatrixRedBiasSgi,
        PostColorMatrixGreenBiasSgi,
        PostColorMatrixBlueBiasSgi,
        PostColorMatrixAlphaBiasSgi,
        TextureColorTableSgi,
        BlendDstRgb = 32968,
        BlendSrcRgb,
        BlendDstAlpha,
        BlendSrcAlpha,
        ColorTableSgi = 32976,
        PostConvolutionColorTableSgi,
        PostColorMatrixColorTableSgi,
        MaxElementsVertices = 33000,
        MaxElementsIndices,
        PointSizeMin = 33062,
        PointSizeMinSgis = 33062,
        PointSizeMax,
        PointSizeMaxSgis = 33063,
        PointFadeThresholdSize,
        PointFadeThresholdSizeSgis = 33064,
        DistanceAttenuationSgis,
        PointDistanceAttenuation = 33065,
        FogFuncPointsSgis = 33067,
        MaxFogFuncPointsSgis,
        PackSkipVolumesSgis = 33072,
        PackImageDepthSgis,
        UnpackSkipVolumesSgis,
        UnpackImageDepthSgis,
        Texture4DSgis,
        Max4DTextureSizeSgis = 33080,
        PixelTexGenSgix,
        PixelTileBestAlignmentSgix = 33086,
        PixelTileCacheIncrementSgix,
        PixelTileWidthSgix,
        PixelTileHeightSgix,
        PixelTileGridWidthSgix,
        PixelTileGridHeightSgix,
        PixelTileGridDepthSgix,
        PixelTileCacheSizeSgix,
        SpriteSgix = 33096,
        SpriteModeSgix,
        SpriteAxisSgix,
        SpriteTranslationSgix,
        Texture4DBindingSgis = 33103,
        MaxClipmapDepthSgix = 33143,
        MaxClipmapVirtualDepthSgix,
        PostTextureFilterBiasRangeSgix = 33147,
        PostTextureFilterScaleRangeSgix,
        ReferencePlaneSgix,
        ReferencePlaneEquationSgix,
        IrInstrument1Sgix,
        InstrumentMeasurementsSgix = 33153,
        CalligraphicFragmentSgix = 33155,
        FramezoomSgix = 33163,
        FramezoomFactorSgix,
        MaxFramezoomFactorSgix,
        GenerateMipmapHint = 33170,
        GenerateMipmapHintSgis = 33170,
        DeformationsMaskSgix = 33174,
        FogOffsetSgix = 33176,
        FogOffsetValueSgix,
        LightModelColorControl = 33272,
        SharedTexturePaletteExt = 33275,
        MajorVersion = 33307,
        MinorVersion,
        NumExtensions,
        ContextFlags,
        ProgramPipelineBinding = 33370,
        MaxViewports,
        ViewportSubpixelBits,
        ViewportBoundsRange,
        LayerProvokingVertex,
        ViewportIndexProvokingVertex,
        ConvolutionHintSgix = 33558,
        AsyncMarkerSgix = 33577,
        PixelTexGenModeSgix = 33579,
        AsyncHistogramSgix,
        MaxAsyncHistogramSgix,
        PixelTextureSgis = 33619,
        AsyncTexImageSgix = 33628,
        AsyncDrawPixelsSgix,
        AsyncReadPixelsSgix,
        MaxAsyncTexImageSgix,
        MaxAsyncDrawPixelsSgix,
        MaxAsyncReadPixelsSgix,
        VertexPreclipSgix = 33774,
        VertexPreclipHintSgix,
        FragmentLightingSgix = 33792,
        FragmentColorMaterialSgix,
        FragmentColorMaterialFaceSgix,
        FragmentColorMaterialParameterSgix,
        MaxFragmentLightsSgix,
        MaxActiveLightsSgix,
        LightEnvModeSgix = 33799,
        FragmentLightModelLocalViewerSgix,
        FragmentLightModelTwoSideSgix,
        FragmentLightModelAmbientSgix,
        FragmentLightModelNormalInterpolationSgix,
        FragmentLight0Sgix,
        PackResampleSgix = 33836,
        UnpackResampleSgix,
        CurrentFogCoord = 33875,
        FogCoordArrayType,
        FogCoordArrayStride,
        ColorSum = 33880,
        CurrentSecondaryColor,
        SecondaryColorArraySize,
        SecondaryColorArrayType,
        SecondaryColorArrayStride,
        CurrentRasterSecondaryColor = 33887,
        AliasedPointSizeRange = 33901,
        AliasedLineWidthRange,
        ActiveTexture = 34016,
        ClientActiveTexture,
        MaxTextureUnits,
        TransposeModelviewMatrix,
        TransposeProjectionMatrix,
        TransposeTextureMatrix,
        TransposeColorMatrix,
        MaxRenderbufferSize = 34024,
        MaxRenderbufferSizeExt = 34024,
        TextureCompressionHint = 34031,
        TextureBindingRectangle = 34038,
        MaxRectangleTextureSize = 34040,
        MaxTextureLodBias = 34045,
        TextureCubeMap = 34067,
        TextureBindingCubeMap,
        MaxCubeMapTextureSize = 34076,
        PackSubsampleRateSgix = 34208,
        UnpackSubsampleRateSgix,
        VertexArrayBinding = 34229,
        ProgramPointSize = 34370,
        DepthClamp = 34383,
        NumCompressedTextureFormats = 34466,
        CompressedTextureFormats,
        ATI_VBO_FREE_MEMORY = 34811,
        ATI_TEXTURE_FREE_MEMORY = 34812,
        ATI_RENDERBUFFER_FREE_MEMORY = 34813,
        NumProgramBinaryFormats = 34814,
        ProgramBinaryFormats,
        StencilBackFunc,
        StencilBackFail,
        StencilBackPassDepthFail,
        StencilBackPassDepthPass,
        RgbaFloatMode = 34848,
        MaxDrawBuffers = 34852,
        DrawBuffer0,
        DrawBuffer1,
        DrawBuffer2,
        DrawBuffer3,
        DrawBuffer4,
        DrawBuffer5,
        DrawBuffer6,
        DrawBuffer7,
        DrawBuffer8,
        DrawBuffer9,
        DrawBuffer10,
        DrawBuffer11,
        DrawBuffer12,
        DrawBuffer13,
        DrawBuffer14,
        DrawBuffer15,
        BlendEquationAlpha = 34877,
        TextureCubeMapSeamless = 34895,
        PointSprite = 34913,
        MaxVertexAttribs = 34921,
        MaxTessControlInputComponents = 34924,
        MaxTessEvaluationInputComponents,
        MaxTextureCoords = 34929,
        MaxTextureImageUnits,
        ArrayBufferBinding = 34964,
        ElementArrayBufferBinding,
        VertexArrayBufferBinding,
        NormalArrayBufferBinding,
        ColorArrayBufferBinding,
        IndexArrayBufferBinding,
        TextureCoordArrayBufferBinding,
        EdgeFlagArrayBufferBinding,
        SecondaryColorArrayBufferBinding,
        FogCoordArrayBufferBinding,
        WeightArrayBufferBinding,
        VertexAttribArrayBufferBinding,
        PixelPackBufferBinding = 35053,
        PixelUnpackBufferBinding = 35055,
        MaxDualSourceDrawBuffers = 35068,
        MaxArrayTextureLayers = 35071,
        MinProgramTexelOffset = 35076,
        MaxProgramTexelOffset,
        SamplerBinding = 35097,
        ClampVertexColor,
        ClampFragmentColor,
        ClampReadColor,
        MaxVertexUniformBlocks = 35371,
        MaxGeometryUniformBlocks,
        MaxFragmentUniformBlocks,
        MaxCombinedUniformBlocks,
        MaxUniformBufferBindings,
        MaxUniformBlockSize,
        MaxCombinedVertexUniformComponents,
        MaxCombinedGeometryUniformComponents,
        MaxCombinedFragmentUniformComponents,
        UniformBufferOffsetAlignment,
        MaxFragmentUniformComponents = 35657,
        MaxVertexUniformComponents,
        MaxVaryingComponents,
        MaxVaryingFloats = 35659,
        MaxVertexTextureImageUnits,
        MaxCombinedTextureImageUnits,
        FragmentShaderDerivativeHint = 35723,
        CurrentProgram = 35725,
        ImplementationColorReadType = 35738,
        ImplementationColorReadFormat,
        TextureBinding1DArray = 35868,
        TextureBinding2DArray,
        MaxGeometryTextureImageUnits = 35881,
        TextureBuffer,
        MaxTextureBufferSize,
        TextureBindingBuffer,
        TextureBufferDataStoreBinding,
        SampleShading = 35894,
        MinSampleShadingValue,
        MaxTransformFeedbackSeparateComponents = 35968,
        MaxTransformFeedbackInterleavedComponents = 35978,
        MaxTransformFeedbackSeparateAttribs,
        StencilBackRef = 36003,
        StencilBackValueMask,
        StencilBackWritemask,
        DrawFramebufferBinding,
        FramebufferBinding = 36006,
        FramebufferBindingExt = 36006,
        RenderbufferBinding,
        RenderbufferBindingExt = 36007,
        ReadFramebufferBinding = 36010,
        MaxColorAttachments = 36063,
        MaxColorAttachmentsExt = 36063,
        MaxSamples = 36183,
        FramebufferSrgb = 36281,
        MaxGeometryVaryingComponents = 36317,
        MaxVertexVaryingComponents,
        MaxGeometryUniformComponents,
        MaxGeometryOutputVertices,
        MaxGeometryTotalOutputComponents,
        MaxSubroutines = 36327,
        MaxSubroutineUniformLocations,
        ShaderBinaryFormats = 36344,
        NumShaderBinaryFormats,
        ShaderCompiler,
        MaxVertexUniformVectors,
        MaxVaryingVectors,
        MaxFragmentUniformVectors,
        MaxCombinedTessControlUniformComponents = 36382,
        MaxCombinedTessEvaluationUniformComponents,
        TransformFeedbackBufferPaused = 36387,
        TransformFeedbackBufferActive,
        TransformFeedbackBinding,
        Timestamp = 36392,
        QuadsFollowProvokingVertexConvention = 36428,
        ProvokingVertex = 36431,
        SampleMask = 36433,
        MaxSampleMaskWords = 36441,
        MaxGeometryShaderInvocations,
        MinFragmentInterpolationOffset,
        MaxFragmentInterpolationOffset,
        FragmentInterpolationOffsetBits,
        MinProgramTextureGatherOffset,
        MaxProgramTextureGatherOffset,
        MaxTransformFeedbackBuffers = 36464,
        MaxVertexStreams,
        PatchVertices,
        PatchDefaultInnerLevel,
        PatchDefaultOuterLevel,
        MaxPatchVertices = 36477,
        MaxTessGenLevel,
        MaxTessControlUniformComponents,
        MaxTessEvaluationUniformComponents,
        MaxTessControlTextureImageUnits,
        MaxTessEvaluationTextureImageUnits,
        MaxTessControlOutputComponents,
        MaxTessPatchComponents,
        MaxTessControlTotalOutputComponents,
        MaxTessEvaluationOutputComponents,
        MaxTessControlUniformBlocks = 36489,
        MaxTessEvaluationUniformBlocks,
        DrawIndirectBufferBinding = 36675,
        NVX_VIDEO_MEMORY_DEDICATED = 36935,
        NVX_TOTAL_MEMORY_AVAILABLE = 36936,
        NVX_CURRENT_MEMORY_AVAILABLE = 36937,
        NVX_EVICTION_MEMORY_COUNT = 36938,
        NVX_EVICTED_MEMORY = 36939,
        MaxVertexImageUniforms = 37066,
        MaxTessControlImageUniforms,
        MaxTessEvaluationImageUniforms,
        MaxGeometryImageUniforms,
        MaxFragmentImageUniforms,
        MaxCombinedImageUniforms,
        TextureBinding2DMultisample = 37124,
        TextureBinding2DMultisampleArray,
        MaxColorTextureSamples = 37134,
        MaxDepthTextureSamples,
        MaxIntegerSamples,
        MaxVertexOutputComponents = 37154,
        MaxGeometryInputComponents,
        MaxGeometryOutputComponents,
        MaxFragmentInputComponents,
        MaxComputeImageUniforms = 37309
    }
    public enum StringName : int
    {
        /// <summary>
        /// Original was GL_VENDOR = 0x1F00
        /// </summary>
        Vendor = ((int)0x1F00),
        /// <summary>
        /// Original was GL_RENDERER = 0x1F01
        /// </summary>
        Renderer = ((int)0x1F01),
        /// <summary>
        /// Original was GL_VERSION = 0x1F02
        /// </summary>
        Version = ((int)0x1F02),
        /// <summary>
        /// Original was GL_EXTENSIONS = 0x1F03
        /// </summary>
        Extensions = ((int)0x1F03),
        /// <summary>
        /// Original was GL_SHADING_LANGUAGE_VERSION = 0x8B8C
        /// </summary>
        ShadingLanguageVersion = ((int)0x8B8C),
    }
    //
    // Summary:
    //     Used in GL.Ati.StencilOpSeparate, GL.StencilOp and 1 other function
    public enum StencilOp
    {
        //
        // Summary:
        //     Original was GL_ZERO = 0
        Zero = 0,
        //
        // Summary:
        //     Original was GL_INVERT = 0x150A
        Invert = 5386,
        //
        // Summary:
        //     Original was GL_KEEP = 0x1E00
        Keep = 7680,
        //
        // Summary:
        //     Original was GL_REPLACE = 0x1E01
        Replace = 7681,
        //
        // Summary:
        //     Original was GL_INCR = 0x1E02
        Incr = 7682,
        //
        // Summary:
        //     Original was GL_DECR = 0x1E03
        Decr = 7683,
        //
        // Summary:
        //     Original was GL_INCR_WRAP = 0x8507
        IncrWrap = 34055,
        //
        // Summary:
        //     Original was GL_DECR_WRAP = 0x8508
        DecrWrap = 34056
    }
    //
    // Summary:
    //     Used in GL.Ati.StencilFuncSeparate, GL.StencilFunc and 2 other functions
    public enum StencilFunction
    {
        //
        // Summary:
        //     Original was GL_NEVER = 0x0200
        Never = 512,
        //
        // Summary:
        //     Original was GL_LESS = 0x0201
        Less = 513,
        //
        // Summary:
        //     Original was GL_EQUAL = 0x0202
        Equal = 514,
        //
        // Summary:
        //     Original was GL_LEQUAL = 0x0203
        Lequal = 515,
        //
        // Summary:
        //     Original was GL_GREATER = 0x0204
        Greater = 516,
        //
        // Summary:
        //     Original was GL_NOTEQUAL = 0x0205
        Notequal = 517,
        //
        // Summary:
        //     Original was GL_GEQUAL = 0x0206
        Gequal = 518,
        //
        // Summary:
        //     Original was GL_ALWAYS = 0x0207
        Always = 519
    }
#endregion

#region Pixel
    public enum PixelFormat : int
    {
        /// <summary>
        /// Original was GL_UNSIGNED_SHORT = 0x1403
        /// </summary>
        UnsignedShort = ((int)0x1403),
        /// <summary>
        /// Original was GL_UNSIGNED_INT = 0x1405
        /// </summary>
        UnsignedInt = ((int)0x1405),
        /// <summary>
        /// Original was GL_COLOR_INDEX = 0x1900
        /// </summary>
        ColorIndex = ((int)0x1900),
        /// <summary>
        /// Original was GL_STENCIL_INDEX = 0x1901
        /// </summary>
        StencilIndex = ((int)0x1901),
        /// <summary>
        /// Original was GL_DEPTH_COMPONENT = 0x1902
        /// </summary>
        DepthComponent = ((int)0x1902),
        /// <summary>
        /// Original was GL_RED = 0x1903
        /// </summary>
        Red = ((int)0x1903),
        /// <summary>
        /// Original was GL_RED_EXT = 0x1903
        /// </summary>
        RedExt = ((int)0x1903),
        /// <summary>
        /// Original was GL_GREEN = 0x1904
        /// </summary>
        Green = ((int)0x1904),
        /// <summary>
        /// Original was GL_BLUE = 0x1905
        /// </summary>
        Blue = ((int)0x1905),
        /// <summary>
        /// Original was GL_ALPHA = 0x1906
        /// </summary>
        Alpha = ((int)0x1906),
        /// <summary>
        /// Original was GL_RGB = 0x1907
        /// </summary>
        Rgb = ((int)0x1907),
        /// <summary>
        /// Original was GL_RGBA = 0x1908
        /// </summary>
        Rgba = ((int)0x1908),
        /// <summary>
        /// Original was GL_LUMINANCE = 0x1909
        /// </summary>
        Luminance = ((int)0x1909),
        /// <summary>
        /// Original was GL_LUMINANCE_ALPHA = 0x190A
        /// </summary>
        LuminanceAlpha = ((int)0x190A),
        /// <summary>
        /// Original was GL_ABGR_EXT = 0x8000
        /// </summary>
        AbgrExt = ((int)0x8000),
        /// <summary>
        /// Original was GL_CMYK_EXT = 0x800C
        /// </summary>
        CmykExt = ((int)0x800C),
        /// <summary>
        /// Original was GL_CMYKA_EXT = 0x800D
        /// </summary>
        CmykaExt = ((int)0x800D),
        /// <summary>
        /// Original was GL_BGR = 0x80E0
        /// </summary>
        Bgr = ((int)0x80E0),
        /// <summary>
        /// Original was GL_BGRA = 0x80E1
        /// </summary>
        Bgra = ((int)0x80E1),
        /// <summary>
        /// Original was GL_YCRCB_422_SGIX = 0x81BB
        /// </summary>
        Ycrcb422Sgix = ((int)0x81BB),
        /// <summary>
        /// Original was GL_YCRCB_444_SGIX = 0x81BC
        /// </summary>
        Ycrcb444Sgix = ((int)0x81BC),
        /// <summary>
        /// Original was GL_RG = 0x8227
        /// </summary>
        Rg = ((int)0x8227),
        /// <summary>
        /// Original was GL_RG_INTEGER = 0x8228
        /// </summary>
        RgInteger = ((int)0x8228),
        /// <summary>
        /// Original was GL_R5_G6_B5_ICC_SGIX = 0x8466
        /// </summary>
        R5G6B5IccSgix = ((int)0x8466),
        /// <summary>
        /// Original was GL_R5_G6_B5_A8_ICC_SGIX = 0x8467
        /// </summary>
        R5G6B5A8IccSgix = ((int)0x8467),
        /// <summary>
        /// Original was GL_ALPHA16_ICC_SGIX = 0x8468
        /// </summary>
        Alpha16IccSgix = ((int)0x8468),
        /// <summary>
        /// Original was GL_LUMINANCE16_ICC_SGIX = 0x8469
        /// </summary>
        Luminance16IccSgix = ((int)0x8469),
        /// <summary>
        /// Original was GL_LUMINANCE16_ALPHA8_ICC_SGIX = 0x846B
        /// </summary>
        Luminance16Alpha8IccSgix = ((int)0x846B),
        /// <summary>
        /// Original was GL_DEPTH_STENCIL = 0x84F9
        /// </summary>
        DepthStencil = ((int)0x84F9),
        /// <summary>
        /// Original was GL_RED_INTEGER = 0x8D94
        /// </summary>
        RedInteger = ((int)0x8D94),
        /// <summary>
        /// Original was GL_GREEN_INTEGER = 0x8D95
        /// </summary>
        GreenInteger = ((int)0x8D95),
        /// <summary>
        /// Original was GL_BLUE_INTEGER = 0x8D96
        /// </summary>
        BlueInteger = ((int)0x8D96),
        /// <summary>
        /// Original was GL_ALPHA_INTEGER = 0x8D97
        /// </summary>
        AlphaInteger = ((int)0x8D97),
        /// <summary>
        /// Original was GL_RGB_INTEGER = 0x8D98
        /// </summary>
        RgbInteger = ((int)0x8D98),
        /// <summary>
        /// Original was GL_RGBA_INTEGER = 0x8D99
        /// </summary>
        RgbaInteger = ((int)0x8D99),
        /// <summary>
        /// Original was GL_BGR_INTEGER = 0x8D9A
        /// </summary>
        BgrInteger = ((int)0x8D9A),
        /// <summary>
        /// Original was GL_BGRA_INTEGER = 0x8D9B
        /// </summary>
        BgraInteger = ((int)0x8D9B),
    }
    public enum PixelInternalFormat : int
    {
        /// <summary>
        /// Original was GL_DEPTH_COMPONENT = 0x1902
        /// </summary>
        DepthComponent = ((int)0x1902),
        /// <summary>
        /// Original was GL_ALPHA = 0x1906
        /// </summary>
        Alpha = ((int)0x1906),
        /// <summary>
        /// Original was GL_RGB = 0x1907
        /// </summary>
        Rgb = ((int)0x1907),
        /// <summary>
        /// Original was GL_RGBA = 0x1908
        /// </summary>
        Rgba = ((int)0x1908),
        /// <summary>
        /// Original was GL_LUMINANCE = 0x1909
        /// </summary>
        Luminance = ((int)0x1909),
        /// <summary>
        /// Original was GL_LUMINANCE_ALPHA = 0x190A
        /// </summary>
        LuminanceAlpha = ((int)0x190A),
        /// <summary>
        /// Original was GL_R3_G3_B2 = 0x2A10
        /// </summary>
        R3G3B2 = ((int)0x2A10),
        /// <summary>
        /// Original was GL_ALPHA4 = 0x803B
        /// </summary>
        Alpha4 = ((int)0x803B),
        /// <summary>
        /// Original was GL_ALPHA8 = 0x803C
        /// </summary>
        Alpha8 = ((int)0x803C),
        /// <summary>
        /// Original was GL_ALPHA12 = 0x803D
        /// </summary>
        Alpha12 = ((int)0x803D),
        /// <summary>
        /// Original was GL_ALPHA16 = 0x803E
        /// </summary>
        Alpha16 = ((int)0x803E),
        /// <summary>
        /// Original was GL_LUMINANCE4 = 0x803F
        /// </summary>
        Luminance4 = ((int)0x803F),
        /// <summary>
        /// Original was GL_LUMINANCE8 = 0x8040
        /// </summary>
        Luminance8 = ((int)0x8040),
        /// <summary>
        /// Original was GL_LUMINANCE12 = 0x8041
        /// </summary>
        Luminance12 = ((int)0x8041),
        /// <summary>
        /// Original was GL_LUMINANCE16 = 0x8042
        /// </summary>
        Luminance16 = ((int)0x8042),
        /// <summary>
        /// Original was GL_LUMINANCE4_ALPHA4 = 0x8043
        /// </summary>
        Luminance4Alpha4 = ((int)0x8043),
        /// <summary>
        /// Original was GL_LUMINANCE6_ALPHA2 = 0x8044
        /// </summary>
        Luminance6Alpha2 = ((int)0x8044),
        /// <summary>
        /// Original was GL_LUMINANCE8_ALPHA8 = 0x8045
        /// </summary>
        Luminance8Alpha8 = ((int)0x8045),
        /// <summary>
        /// Original was GL_LUMINANCE12_ALPHA4 = 0x8046
        /// </summary>
        Luminance12Alpha4 = ((int)0x8046),
        /// <summary>
        /// Original was GL_LUMINANCE12_ALPHA12 = 0x8047
        /// </summary>
        Luminance12Alpha12 = ((int)0x8047),
        /// <summary>
        /// Original was GL_LUMINANCE16_ALPHA16 = 0x8048
        /// </summary>
        Luminance16Alpha16 = ((int)0x8048),
        /// <summary>
        /// Original was GL_INTENSITY = 0x8049
        /// </summary>
        Intensity = ((int)0x8049),
        /// <summary>
        /// Original was GL_INTENSITY4 = 0x804A
        /// </summary>
        Intensity4 = ((int)0x804A),
        /// <summary>
        /// Original was GL_INTENSITY8 = 0x804B
        /// </summary>
        Intensity8 = ((int)0x804B),
        /// <summary>
        /// Original was GL_INTENSITY12 = 0x804C
        /// </summary>
        Intensity12 = ((int)0x804C),
        /// <summary>
        /// Original was GL_INTENSITY16 = 0x804D
        /// </summary>
        Intensity16 = ((int)0x804D),
        /// <summary>
        /// Original was GL_RGB2_EXT = 0x804E
        /// </summary>
        Rgb2Ext = ((int)0x804E),
        /// <summary>
        /// Original was GL_RGB4 = 0x804F
        /// </summary>
        Rgb4 = ((int)0x804F),
        /// <summary>
        /// Original was GL_RGB5 = 0x8050
        /// </summary>
        Rgb5 = ((int)0x8050),
        /// <summary>
        /// Original was GL_RGB8 = 0x8051
        /// </summary>
        Rgb8 = ((int)0x8051),
        /// <summary>
        /// Original was GL_RGB10 = 0x8052
        /// </summary>
        Rgb10 = ((int)0x8052),
        /// <summary>
        /// Original was GL_RGB12 = 0x8053
        /// </summary>
        Rgb12 = ((int)0x8053),
        /// <summary>
        /// Original was GL_RGB16 = 0x8054
        /// </summary>
        Rgb16 = ((int)0x8054),
        /// <summary>
        /// Original was GL_RGBA2 = 0x8055
        /// </summary>
        Rgba2 = ((int)0x8055),
        /// <summary>
        /// Original was GL_RGBA4 = 0x8056
        /// </summary>
        Rgba4 = ((int)0x8056),
        /// <summary>
        /// Original was GL_RGB5_A1 = 0x8057
        /// </summary>
        Rgb5A1 = ((int)0x8057),
        /// <summary>
        /// Original was GL_RGBA8 = 0x8058
        /// </summary>
        Rgba8 = ((int)0x8058),
        /// <summary>
        /// Original was GL_RGB10_A2 = 0x8059
        /// </summary>
        Rgb10A2 = ((int)0x8059),
        /// <summary>
        /// Original was GL_RGBA12 = 0x805A
        /// </summary>
        Rgba12 = ((int)0x805A),
        /// <summary>
        /// Original was GL_RGBA16 = 0x805B
        /// </summary>
        Rgba16 = ((int)0x805B),
        /// <summary>
        /// Original was GL_DUAL_ALPHA4_SGIS = 0x8110
        /// </summary>
        DualAlpha4Sgis = ((int)0x8110),
        /// <summary>
        /// Original was GL_DUAL_ALPHA8_SGIS = 0x8111
        /// </summary>
        DualAlpha8Sgis = ((int)0x8111),
        /// <summary>
        /// Original was GL_DUAL_ALPHA12_SGIS = 0x8112
        /// </summary>
        DualAlpha12Sgis = ((int)0x8112),
        /// <summary>
        /// Original was GL_DUAL_ALPHA16_SGIS = 0x8113
        /// </summary>
        DualAlpha16Sgis = ((int)0x8113),
        /// <summary>
        /// Original was GL_DUAL_LUMINANCE4_SGIS = 0x8114
        /// </summary>
        DualLuminance4Sgis = ((int)0x8114),
        /// <summary>
        /// Original was GL_DUAL_LUMINANCE8_SGIS = 0x8115
        /// </summary>
        DualLuminance8Sgis = ((int)0x8115),
        /// <summary>
        /// Original was GL_DUAL_LUMINANCE12_SGIS = 0x8116
        /// </summary>
        DualLuminance12Sgis = ((int)0x8116),
        /// <summary>
        /// Original was GL_DUAL_LUMINANCE16_SGIS = 0x8117
        /// </summary>
        DualLuminance16Sgis = ((int)0x8117),
        /// <summary>
        /// Original was GL_DUAL_INTENSITY4_SGIS = 0x8118
        /// </summary>
        DualIntensity4Sgis = ((int)0x8118),
        /// <summary>
        /// Original was GL_DUAL_INTENSITY8_SGIS = 0x8119
        /// </summary>
        DualIntensity8Sgis = ((int)0x8119),
        /// <summary>
        /// Original was GL_DUAL_INTENSITY12_SGIS = 0x811A
        /// </summary>
        DualIntensity12Sgis = ((int)0x811A),
        /// <summary>
        /// Original was GL_DUAL_INTENSITY16_SGIS = 0x811B
        /// </summary>
        DualIntensity16Sgis = ((int)0x811B),
        /// <summary>
        /// Original was GL_DUAL_LUMINANCE_ALPHA4_SGIS = 0x811C
        /// </summary>
        DualLuminanceAlpha4Sgis = ((int)0x811C),
        /// <summary>
        /// Original was GL_DUAL_LUMINANCE_ALPHA8_SGIS = 0x811D
        /// </summary>
        DualLuminanceAlpha8Sgis = ((int)0x811D),
        /// <summary>
        /// Original was GL_QUAD_ALPHA4_SGIS = 0x811E
        /// </summary>
        QuadAlpha4Sgis = ((int)0x811E),
        /// <summary>
        /// Original was GL_QUAD_ALPHA8_SGIS = 0x811F
        /// </summary>
        QuadAlpha8Sgis = ((int)0x811F),
        /// <summary>
        /// Original was GL_QUAD_LUMINANCE4_SGIS = 0x8120
        /// </summary>
        QuadLuminance4Sgis = ((int)0x8120),
        /// <summary>
        /// Original was GL_QUAD_LUMINANCE8_SGIS = 0x8121
        /// </summary>
        QuadLuminance8Sgis = ((int)0x8121),
        /// <summary>
        /// Original was GL_QUAD_INTENSITY4_SGIS = 0x8122
        /// </summary>
        QuadIntensity4Sgis = ((int)0x8122),
        /// <summary>
        /// Original was GL_QUAD_INTENSITY8_SGIS = 0x8123
        /// </summary>
        QuadIntensity8Sgis = ((int)0x8123),
        /// <summary>
        /// Original was GL_DEPTH_COMPONENT16 = 0x81a5
        /// </summary>
        DepthComponent16 = ((int)0x81a5),
        /// <summary>
        /// Original was GL_DEPTH_COMPONENT16_SGIX = 0x81A5
        /// </summary>
        DepthComponent16Sgix = ((int)0x81A5),
        /// <summary>
        /// Original was GL_DEPTH_COMPONENT24 = 0x81a6
        /// </summary>
        DepthComponent24 = ((int)0x81a6),
        /// <summary>
        /// Original was GL_DEPTH_COMPONENT24_SGIX = 0x81A6
        /// </summary>
        DepthComponent24Sgix = ((int)0x81A6),
        /// <summary>
        /// Original was GL_DEPTH_COMPONENT32 = 0x81a7
        /// </summary>
        DepthComponent32 = ((int)0x81a7),
        /// <summary>
        /// Original was GL_DEPTH_COMPONENT32_SGIX = 0x81A7
        /// </summary>
        DepthComponent32Sgix = ((int)0x81A7),
        /// <summary>
        /// Original was GL_COMPRESSED_RED = 0x8225
        /// </summary>
        CompressedRed = ((int)0x8225),
        /// <summary>
        /// Original was GL_COMPRESSED_RG = 0x8226
        /// </summary>
        CompressedRg = ((int)0x8226),
        /// <summary>
        /// Original was GL_R8 = 0x8229
        /// </summary>
        R8 = ((int)0x8229),
        /// <summary>
        /// Original was GL_R16 = 0x822A
        /// </summary>
        R16 = ((int)0x822A),
        /// <summary>
        /// Original was GL_RG8 = 0x822B
        /// </summary>
        Rg8 = ((int)0x822B),
        /// <summary>
        /// Original was GL_RG16 = 0x822C
        /// </summary>
        Rg16 = ((int)0x822C),
        /// <summary>
        /// Original was GL_R16F = 0x822D
        /// </summary>
        R16f = ((int)0x822D),
        /// <summary>
        /// Original was GL_R32F = 0x822E
        /// </summary>
        R32f = ((int)0x822E),
        /// <summary>
        /// Original was GL_RG16F = 0x822F
        /// </summary>
        Rg16f = ((int)0x822F),
        /// <summary>
        /// Original was GL_RG32F = 0x8230
        /// </summary>
        Rg32f = ((int)0x8230),
        /// <summary>
        /// Original was GL_R8I = 0x8231
        /// </summary>
        R8i = ((int)0x8231),
        /// <summary>
        /// Original was GL_R8UI = 0x8232
        /// </summary>
        R8ui = ((int)0x8232),
        /// <summary>
        /// Original was GL_R16I = 0x8233
        /// </summary>
        R16i = ((int)0x8233),
        /// <summary>
        /// Original was GL_R16UI = 0x8234
        /// </summary>
        R16ui = ((int)0x8234),
        /// <summary>
        /// Original was GL_R32I = 0x8235
        /// </summary>
        R32i = ((int)0x8235),
        /// <summary>
        /// Original was GL_R32UI = 0x8236
        /// </summary>
        R32ui = ((int)0x8236),
        /// <summary>
        /// Original was GL_RG8I = 0x8237
        /// </summary>
        Rg8i = ((int)0x8237),
        /// <summary>
        /// Original was GL_RG8UI = 0x8238
        /// </summary>
        Rg8ui = ((int)0x8238),
        /// <summary>
        /// Original was GL_RG16I = 0x8239
        /// </summary>
        Rg16i = ((int)0x8239),
        /// <summary>
        /// Original was GL_RG16UI = 0x823A
        /// </summary>
        Rg16ui = ((int)0x823A),
        /// <summary>
        /// Original was GL_RG32I = 0x823B
        /// </summary>
        Rg32i = ((int)0x823B),
        /// <summary>
        /// Original was GL_RG32UI = 0x823C
        /// </summary>
        Rg32ui = ((int)0x823C),
        /// <summary>
        /// Original was GL_COMPRESSED_RGB_S3TC_DXT1_EXT = 0x83F0
        /// </summary>
        CompressedRgbS3tcDxt1Ext = ((int)0x83F0),
        /// <summary>
        /// Original was GL_COMPRESSED_RGBA_S3TC_DXT1_EXT = 0x83F1
        /// </summary>
        CompressedRgbaS3tcDxt1Ext = ((int)0x83F1),
        /// <summary>
        /// Original was GL_COMPRESSED_RGBA_S3TC_DXT3_EXT = 0x83F2
        /// </summary>
        CompressedRgbaS3tcDxt3Ext = ((int)0x83F2),
        /// <summary>
        /// Original was GL_COMPRESSED_RGBA_S3TC_DXT5_EXT = 0x83F3
        /// </summary>
        CompressedRgbaS3tcDxt5Ext = ((int)0x83F3),
        /// <summary>
        /// Original was GL_RGB_ICC_SGIX = 0x8460
        /// </summary>
        RgbIccSgix = ((int)0x8460),
        /// <summary>
        /// Original was GL_RGBA_ICC_SGIX = 0x8461
        /// </summary>
        RgbaIccSgix = ((int)0x8461),
        /// <summary>
        /// Original was GL_ALPHA_ICC_SGIX = 0x8462
        /// </summary>
        AlphaIccSgix = ((int)0x8462),
        /// <summary>
        /// Original was GL_LUMINANCE_ICC_SGIX = 0x8463
        /// </summary>
        LuminanceIccSgix = ((int)0x8463),
        /// <summary>
        /// Original was GL_INTENSITY_ICC_SGIX = 0x8464
        /// </summary>
        IntensityIccSgix = ((int)0x8464),
        /// <summary>
        /// Original was GL_LUMINANCE_ALPHA_ICC_SGIX = 0x8465
        /// </summary>
        LuminanceAlphaIccSgix = ((int)0x8465),
        /// <summary>
        /// Original was GL_R5_G6_B5_ICC_SGIX = 0x8466
        /// </summary>
        R5G6B5IccSgix = ((int)0x8466),
        /// <summary>
        /// Original was GL_R5_G6_B5_A8_ICC_SGIX = 0x8467
        /// </summary>
        R5G6B5A8IccSgix = ((int)0x8467),
        /// <summary>
        /// Original was GL_ALPHA16_ICC_SGIX = 0x8468
        /// </summary>
        Alpha16IccSgix = ((int)0x8468),
        /// <summary>
        /// Original was GL_LUMINANCE16_ICC_SGIX = 0x8469
        /// </summary>
        Luminance16IccSgix = ((int)0x8469),
        /// <summary>
        /// Original was GL_INTENSITY16_ICC_SGIX = 0x846A
        /// </summary>
        Intensity16IccSgix = ((int)0x846A),
        /// <summary>
        /// Original was GL_LUMINANCE16_ALPHA8_ICC_SGIX = 0x846B
        /// </summary>
        Luminance16Alpha8IccSgix = ((int)0x846B),
        /// <summary>
        /// Original was GL_COMPRESSED_ALPHA = 0x84E9
        /// </summary>
        CompressedAlpha = ((int)0x84E9),
        /// <summary>
        /// Original was GL_COMPRESSED_LUMINANCE = 0x84EA
        /// </summary>
        CompressedLuminance = ((int)0x84EA),
        /// <summary>
        /// Original was GL_COMPRESSED_LUMINANCE_ALPHA = 0x84EB
        /// </summary>
        CompressedLuminanceAlpha = ((int)0x84EB),
        /// <summary>
        /// Original was GL_COMPRESSED_INTENSITY = 0x84EC
        /// </summary>
        CompressedIntensity = ((int)0x84EC),
        /// <summary>
        /// Original was GL_COMPRESSED_RGB = 0x84ED
        /// </summary>
        CompressedRgb = ((int)0x84ED),
        /// <summary>
        /// Original was GL_COMPRESSED_RGBA = 0x84EE
        /// </summary>
        CompressedRgba = ((int)0x84EE),
        /// <summary>
        /// Original was GL_DEPTH_STENCIL = 0x84F9
        /// </summary>
        DepthStencil = ((int)0x84F9),
        /// <summary>
        /// Original was GL_RGBA32F = 0x8814
        /// </summary>
        Rgba32f = ((int)0x8814),
        /// <summary>
        /// Original was GL_RGB32F = 0x8815
        /// </summary>
        Rgb32f = ((int)0x8815),
        /// <summary>
        /// Original was GL_RGBA16F = 0x881A
        /// </summary>
        Rgba16f = ((int)0x881A),
        /// <summary>
        /// Original was GL_RGB16F = 0x881B
        /// </summary>
        Rgb16f = ((int)0x881B),
        /// <summary>
        /// Original was GL_DEPTH24_STENCIL8 = 0x88F0
        /// </summary>
        Depth24Stencil8 = ((int)0x88F0),
        /// <summary>
        /// Original was GL_R11F_G11F_B10F = 0x8C3A
        /// </summary>
        R11fG11fB10f = ((int)0x8C3A),
        /// <summary>
        /// Original was GL_RGB9_E5 = 0x8C3D
        /// </summary>
        Rgb9E5 = ((int)0x8C3D),
        /// <summary>
        /// Original was GL_SRGB = 0x8C40
        /// </summary>
        Srgb = ((int)0x8C40),
        /// <summary>
        /// Original was GL_SRGB8 = 0x8C41
        /// </summary>
        Srgb8 = ((int)0x8C41),
        /// <summary>
        /// Original was GL_SRGB_ALPHA = 0x8C42
        /// </summary>
        SrgbAlpha = ((int)0x8C42),
        /// <summary>
        /// Original was GL_SRGB8_ALPHA8 = 0x8C43
        /// </summary>
        Srgb8Alpha8 = ((int)0x8C43),
        /// <summary>
        /// Original was GL_SLUMINANCE_ALPHA = 0x8C44
        /// </summary>
        SluminanceAlpha = ((int)0x8C44),
        /// <summary>
        /// Original was GL_SLUMINANCE8_ALPHA8 = 0x8C45
        /// </summary>
        Sluminance8Alpha8 = ((int)0x8C45),
        /// <summary>
        /// Original was GL_SLUMINANCE = 0x8C46
        /// </summary>
        Sluminance = ((int)0x8C46),
        /// <summary>
        /// Original was GL_SLUMINANCE8 = 0x8C47
        /// </summary>
        Sluminance8 = ((int)0x8C47),
        /// <summary>
        /// Original was GL_COMPRESSED_SRGB = 0x8C48
        /// </summary>
        CompressedSrgb = ((int)0x8C48),
        /// <summary>
        /// Original was GL_COMPRESSED_SRGB_ALPHA = 0x8C49
        /// </summary>
        CompressedSrgbAlpha = ((int)0x8C49),
        /// <summary>
        /// Original was GL_COMPRESSED_SLUMINANCE = 0x8C4A
        /// </summary>
        CompressedSluminance = ((int)0x8C4A),
        /// <summary>
        /// Original was GL_COMPRESSED_SLUMINANCE_ALPHA = 0x8C4B
        /// </summary>
        CompressedSluminanceAlpha = ((int)0x8C4B),
        /// <summary>
        /// Original was GL_COMPRESSED_SRGB_S3TC_DXT1_EXT = 0x8C4C
        /// </summary>
        CompressedSrgbS3tcDxt1Ext = ((int)0x8C4C),
        /// <summary>
        /// Original was GL_COMPRESSED_SRGB_ALPHA_S3TC_DXT1_EXT = 0x8C4D
        /// </summary>
        CompressedSrgbAlphaS3tcDxt1Ext = ((int)0x8C4D),
        /// <summary>
        /// Original was GL_COMPRESSED_SRGB_ALPHA_S3TC_DXT3_EXT = 0x8C4E
        /// </summary>
        CompressedSrgbAlphaS3tcDxt3Ext = ((int)0x8C4E),
        /// <summary>
        /// Original was GL_COMPRESSED_SRGB_ALPHA_S3TC_DXT5_EXT = 0x8C4F
        /// </summary>
        CompressedSrgbAlphaS3tcDxt5Ext = ((int)0x8C4F),
        /// <summary>
        /// Original was GL_DEPTH_COMPONENT32F = 0x8CAC
        /// </summary>
        DepthComponent32f = ((int)0x8CAC),
        /// <summary>
        /// Original was GL_DEPTH32F_STENCIL8 = 0x8CAD
        /// </summary>
        Depth32fStencil8 = ((int)0x8CAD),
        /// <summary>
        /// Original was GL_RGBA32UI = 0x8D70
        /// </summary>
        Rgba32ui = ((int)0x8D70),
        /// <summary>
        /// Original was GL_RGB32UI = 0x8D71
        /// </summary>
        Rgb32ui = ((int)0x8D71),
        /// <summary>
        /// Original was GL_RGBA16UI = 0x8D76
        /// </summary>
        Rgba16ui = ((int)0x8D76),
        /// <summary>
        /// Original was GL_RGB16UI = 0x8D77
        /// </summary>
        Rgb16ui = ((int)0x8D77),
        /// <summary>
        /// Original was GL_RGBA8UI = 0x8D7C
        /// </summary>
        Rgba8ui = ((int)0x8D7C),
        /// <summary>
        /// Original was GL_RGB8UI = 0x8D7D
        /// </summary>
        Rgb8ui = ((int)0x8D7D),
        /// <summary>
        /// Original was GL_RGBA32I = 0x8D82
        /// </summary>
        Rgba32i = ((int)0x8D82),
        /// <summary>
        /// Original was GL_RGB32I = 0x8D83
        /// </summary>
        Rgb32i = ((int)0x8D83),
        /// <summary>
        /// Original was GL_RGBA16I = 0x8D88
        /// </summary>
        Rgba16i = ((int)0x8D88),
        /// <summary>
        /// Original was GL_RGB16I = 0x8D89
        /// </summary>
        Rgb16i = ((int)0x8D89),
        /// <summary>
        /// Original was GL_RGBA8I = 0x8D8E
        /// </summary>
        Rgba8i = ((int)0x8D8E),
        /// <summary>
        /// Original was GL_RGB8I = 0x8D8F
        /// </summary>
        Rgb8i = ((int)0x8D8F),
        /// <summary>
        /// Original was GL_FLOAT_32_UNSIGNED_INT_24_8_REV = 0x8DAD
        /// </summary>
        Float32UnsignedInt248Rev = ((int)0x8DAD),
        /// <summary>
        /// Original was GL_COMPRESSED_RED_RGTC1 = 0x8DBB
        /// </summary>
        CompressedRedRgtc1 = ((int)0x8DBB),
        /// <summary>
        /// Original was GL_COMPRESSED_SIGNED_RED_RGTC1 = 0x8DBC
        /// </summary>
        CompressedSignedRedRgtc1 = ((int)0x8DBC),
        /// <summary>
        /// Original was GL_COMPRESSED_RG_RGTC2 = 0x8DBD
        /// </summary>
        CompressedRgRgtc2 = ((int)0x8DBD),
        /// <summary>
        /// Original was GL_COMPRESSED_SIGNED_RG_RGTC2 = 0x8DBE
        /// </summary>
        CompressedSignedRgRgtc2 = ((int)0x8DBE),
        /// <summary>
        /// Original was GL_COMPRESSED_RGBA_BPTC_UNORM = 0x8E8C
        /// </summary>
        CompressedRgbaBptcUnorm = ((int)0x8E8C),
        /// <summary>
        /// Original was GL_COMPRESSED_SRGB_ALPHA_BPTC_UNORM = 0x8E8D
        /// </summary>
        CompressedSrgbAlphaBptcUnorm = ((int)0x8E8D),
        /// <summary>
        /// Original was GL_COMPRESSED_RGB_BPTC_SIGNED_FLOAT = 0x8E8E
        /// </summary>
        CompressedRgbBptcSignedFloat = ((int)0x8E8E),
        /// <summary>
        /// Original was GL_COMPRESSED_RGB_BPTC_UNSIGNED_FLOAT = 0x8E8F
        /// </summary>
        CompressedRgbBptcUnsignedFloat = ((int)0x8E8F),
        /// <summary>
        /// Original was GL_R8_SNORM = 0x8F94
        /// </summary>
        R8Snorm = ((int)0x8F94),
        /// <summary>
        /// Original was GL_RG8_SNORM = 0x8F95
        /// </summary>
        Rg8Snorm = ((int)0x8F95),
        /// <summary>
        /// Original was GL_RGB8_SNORM = 0x8F96
        /// </summary>
        Rgb8Snorm = ((int)0x8F96),
        /// <summary>
        /// Original was GL_RGBA8_SNORM = 0x8F97
        /// </summary>
        Rgba8Snorm = ((int)0x8F97),
        /// <summary>
        /// Original was GL_R16_SNORM = 0x8F98
        /// </summary>
        R16Snorm = ((int)0x8F98),
        /// <summary>
        /// Original was GL_RG16_SNORM = 0x8F99
        /// </summary>
        Rg16Snorm = ((int)0x8F99),
        /// <summary>
        /// Original was GL_RGB16_SNORM = 0x8F9A
        /// </summary>
        Rgb16Snorm = ((int)0x8F9A),
        /// <summary>
        /// Original was GL_RGBA16_SNORM = 0x8F9B
        /// </summary>
        Rgba16Snorm = ((int)0x8F9B),
        /// <summary>
        /// Original was GL_RGB10_A2UI = 0x906F
        /// </summary>
        Rgb10A2ui = ((int)0x906F),
        /// <summary>
        /// Original was GL_ONE = 1
        /// </summary>
        One = ((int)1),
        /// <summary>
        /// Original was GL_TWO = 2
        /// </summary>
        Two = ((int)2),
        /// <summary>
        /// Original was GL_THREE = 3
        /// </summary>
        Three = ((int)3),
        /// <summary>
        /// Original was GL_FOUR = 4
        /// </summary>
        Four = ((int)4),
    }
    public enum PixelType : int
    {
        /// <summary>
        /// Original was GL_BYTE = 0x1400
        /// </summary>
        Byte = ((int)0x1400),
        /// <summary>
        /// Original was GL_UNSIGNED_BYTE = 0x1401
        /// </summary>
        UnsignedByte = ((int)0x1401),
        /// <summary>
        /// Original was GL_SHORT = 0x1402
        /// </summary>
        Short = ((int)0x1402),
        /// <summary>
        /// Original was GL_UNSIGNED_SHORT = 0x1403
        /// </summary>
        UnsignedShort = ((int)0x1403),
        /// <summary>
        /// Original was GL_INT = 0x1404
        /// </summary>
        Int = ((int)0x1404),
        /// <summary>
        /// Original was GL_UNSIGNED_INT = 0x1405
        /// </summary>
        UnsignedInt = ((int)0x1405),
        /// <summary>
        /// Original was GL_FLOAT = 0x1406
        /// </summary>
        Float = ((int)0x1406),
        /// <summary>
        /// Original was GL_HALF_FLOAT = 0x140B
        /// </summary>
        HalfFloat = ((int)0x140B),
        /// <summary>
        /// Original was GL_BITMAP = 0x1A00
        /// </summary>
        Bitmap = ((int)0x1A00),
        /// <summary>
        /// Original was GL_UNSIGNED_BYTE_3_3_2 = 0x8032
        /// </summary>
        UnsignedByte332 = ((int)0x8032),
        /// <summary>
        /// Original was GL_UNSIGNED_BYTE_3_3_2_EXT = 0x8032
        /// </summary>
        UnsignedByte332Ext = ((int)0x8032),
        /// <summary>
        /// Original was GL_UNSIGNED_SHORT_4_4_4_4 = 0x8033
        /// </summary>
        UnsignedShort4444 = ((int)0x8033),
        /// <summary>
        /// Original was GL_UNSIGNED_SHORT_4_4_4_4_EXT = 0x8033
        /// </summary>
        UnsignedShort4444Ext = ((int)0x8033),
        /// <summary>
        /// Original was GL_UNSIGNED_SHORT_5_5_5_1 = 0x8034
        /// </summary>
        UnsignedShort5551 = ((int)0x8034),
        /// <summary>
        /// Original was GL_UNSIGNED_SHORT_5_5_5_1_EXT = 0x8034
        /// </summary>
        UnsignedShort5551Ext = ((int)0x8034),
        /// <summary>
        /// Original was GL_UNSIGNED_INT_8_8_8_8 = 0x8035
        /// </summary>
        UnsignedInt8888 = ((int)0x8035),
        /// <summary>
        /// Original was GL_UNSIGNED_INT_8_8_8_8_EXT = 0x8035
        /// </summary>
        UnsignedInt8888Ext = ((int)0x8035),
        /// <summary>
        /// Original was GL_UNSIGNED_INT_10_10_10_2 = 0x8036
        /// </summary>
        UnsignedInt1010102 = ((int)0x8036),
        /// <summary>
        /// Original was GL_UNSIGNED_INT_10_10_10_2_EXT = 0x8036
        /// </summary>
        UnsignedInt1010102Ext = ((int)0x8036),
        /// <summary>
        /// Original was GL_UNSIGNED_BYTE_2_3_3_REVERSED = 0x8362
        /// </summary>
        UnsignedByte233Reversed = ((int)0x8362),
        /// <summary>
        /// Original was GL_UNSIGNED_SHORT_5_6_5 = 0x8363
        /// </summary>
        UnsignedShort565 = ((int)0x8363),
        /// <summary>
        /// Original was GL_UNSIGNED_SHORT_5_6_5_REVERSED = 0x8364
        /// </summary>
        UnsignedShort565Reversed = ((int)0x8364),
        /// <summary>
        /// Original was GL_UNSIGNED_SHORT_4_4_4_4_REVERSED = 0x8365
        /// </summary>
        UnsignedShort4444Reversed = ((int)0x8365),
        /// <summary>
        /// Original was GL_UNSIGNED_SHORT_1_5_5_5_REVERSED = 0x8366
        /// </summary>
        UnsignedShort1555Reversed = ((int)0x8366),
        /// <summary>
        /// Original was GL_UNSIGNED_INT_8_8_8_8_REVERSED = 0x8367
        /// </summary>
        UnsignedInt8888Reversed = ((int)0x8367),
        /// <summary>
        /// Original was GL_UNSIGNED_INT_2_10_10_10_REVERSED = 0x8368
        /// </summary>
        UnsignedInt2101010Reversed = ((int)0x8368),
        /// <summary>
        /// Original was GL_UNSIGNED_INT_24_8 = 0x84FA
        /// </summary>
        UnsignedInt248 = ((int)0x84FA),
        /// <summary>
        /// Original was GL_UNSIGNED_INT_10F_11F_11F_REV = 0x8C3B
        /// </summary>
        UnsignedInt10F11F11FRev = ((int)0x8C3B),
        /// <summary>
        /// Original was GL_UNSIGNED_INT_5_9_9_9_REV = 0x8C3E
        /// </summary>
        UnsignedInt5999Rev = ((int)0x8C3E),
        /// <summary>
        /// Original was GL_FLOAT_32_UNSIGNED_INT_24_8_REV = 0x8DAD
        /// </summary>
        Float32UnsignedInt248Rev = ((int)0x8DAD),
    }

    public enum ExtTextureCompressionS3tc : int
    {
        /// <summary>
        /// Original was GL_COMPRESSED_RGB_S3TC_DXT1_EXT = 0x83F0
        /// </summary>
        CompressedRgbS3tcDxt1Ext = ((int)0x83F0),
        /// <summary>
        /// Original was GL_COMPRESSED_RGBA_S3TC_DXT1_EXT = 0x83F1
        /// </summary>
        CompressedRgbaS3tcDxt1Ext = ((int)0x83F1),
        /// <summary>
        /// Original was GL_COMPRESSED_RGBA_S3TC_DXT3_EXT = 0x83F2
        /// </summary>
        CompressedRgbaS3tcDxt3Ext = ((int)0x83F2),
        /// <summary>
        /// Original was GL_COMPRESSED_RGBA_S3TC_DXT5_EXT = 0x83F3
        /// </summary>
        CompressedRgbaS3tcDxt5Ext = ((int)0x83F3),
    }

#endregion

#region Shader
    public enum ShaderType : int
    {
        /// <summary>
        /// Original was GL_FRAGMENT_SHADER = 0x8B30
        /// </summary>
        FragmentShader = ((int)0x8B30),
        /// <summary>
        /// Original was GL_VERTEX_SHADER = 0x8B31
        /// </summary>
        VertexShader = ((int)0x8B31),
        /// <summary>
        /// Original was GL_GEOMETRY_SHADER = 0x8DD9
        /// </summary>
        GeometryShader = ((int)0x8DD9),
        /// <summary>
        /// Original was GL_GEOMETRY_SHADER_EXT = 0x8DD9
        /// </summary>
        GeometryShaderExt = ((int)0x8DD9),
        /// <summary>
        /// Original was GL_TESS_EVALUATION_SHADER = 0x8E87
        /// </summary>
        TessEvaluationShader = ((int)0x8E87),
        /// <summary>
        /// Original was GL_TESS_CONTROL_SHADER = 0x8E88
        /// </summary>
        TessControlShader = ((int)0x8E88),
        /// <summary>
        /// Original was GL_COMPUTE_SHADER = 0x91B9
        /// </summary>
        ComputeShader = ((int)0x91B9),
    }
    public enum ShaderParameter
    {
        //
        // Summary:
        //     Original was GL_SHADER_TYPE = 0x8B4F
        ShaderType = 35663,
        //
        // Summary:
        //     Original was GL_DELETE_STATUS = 0x8B80
        DeleteStatus = 35712,
        //
        // Summary:
        //     Original was GL_COMPILE_STATUS = 0x8B81
        CompileStatus = 35713,
        //
        // Summary:
        //     Original was GL_INFO_LOG_LENGTH = 0x8B84
        InfoLogLength = 35716,
        //
        // Summary:
        //     Original was GL_SHADER_SOURCE_LENGTH = 0x8B88
        ShaderSourceLength = 35720
    }
    public enum GetProgramParameterName
    {
        //
        // Summary:
        //     Original was GL_PROGRAM_BINARY_RETRIEVABLE_HINT = 0x8257
        ProgramBinaryRetrievableHint = 33367,
        //
        // Summary:
        //     Original was GL_PROGRAM_SEPARABLE = 0x8258
        ProgramSeparable = 33368,
        //
        // Summary:
        //     Original was GL_GEOMETRY_SHADER_INVOCATIONS = 0x887F
        GeometryShaderInvocations = 34943,
        //
        // Summary:
        //     Original was GL_GEOMETRY_VERTICES_OUT = 0x8916
        GeometryVerticesOut = 35094,
        //
        // Summary:
        //     Original was GL_GEOMETRY_INPUT_TYPE = 0x8917
        GeometryInputType = 35095,
        //
        // Summary:
        //     Original was GL_GEOMETRY_OUTPUT_TYPE = 0x8918
        GeometryOutputType = 35096,
        //
        // Summary:
        //     Original was GL_ACTIVE_UNIFORM_BLOCK_MAX_NAME_LENGTH = 0x8A35
        ActiveUniformBlockMaxNameLength = 35381,
        //
        // Summary:
        //     Original was GL_ACTIVE_UNIFORM_BLOCKS = 0x8A36
        ActiveUniformBlocks = 35382,
        //
        // Summary:
        //     Original was GL_DELETE_STATUS = 0x8B80
        DeleteStatus = 35712,
        //
        // Summary:
        //     Original was GL_LINK_STATUS = 0x8B82
        LinkStatus = 35714,
        //
        // Summary:
        //     Original was GL_VALIDATE_STATUS = 0x8B83
        ValidateStatus = 35715,
        //
        // Summary:
        //     Original was GL_INFO_LOG_LENGTH = 0x8B84
        InfoLogLength = 35716,
        //
        // Summary:
        //     Original was GL_ATTACHED_SHADERS = 0x8B85
        AttachedShaders = 35717,
        //
        // Summary:
        //     Original was GL_ACTIVE_UNIFORMS = 0x8B86
        ActiveUniforms = 35718,
        //
        // Summary:
        //     Original was GL_ACTIVE_UNIFORM_MAX_LENGTH = 0x8B87
        ActiveUniformMaxLength = 35719,
        //
        // Summary:
        //     Original was GL_ACTIVE_ATTRIBUTES = 0x8B89
        ActiveAttributes = 35721,
        //
        // Summary:
        //     Original was GL_ACTIVE_ATTRIBUTE_MAX_LENGTH = 0x8B8A
        ActiveAttributeMaxLength = 35722,
        //
        // Summary:
        //     Original was GL_TRANSFORM_FEEDBACK_VARYING_MAX_LENGTH = 0x8C76
        TransformFeedbackVaryingMaxLength = 35958,
        //
        // Summary:
        //     Original was GL_TRANSFORM_FEEDBACK_BUFFER_MODE = 0x8C7F
        TransformFeedbackBufferMode = 35967,
        //
        // Summary:
        //     Original was GL_TRANSFORM_FEEDBACK_VARYINGS = 0x8C83
        TransformFeedbackVaryings = 35971,
        //
        // Summary:
        //     Original was GL_TESS_CONTROL_OUTPUT_VERTICES = 0x8E75
        TessControlOutputVertices = 36469,
        //
        // Summary:
        //     Original was GL_TESS_GEN_MODE = 0x8E76
        TessGenMode = 36470,
        //
        // Summary:
        //     Original was GL_TESS_GEN_SPACING = 0x8E77
        TessGenSpacing = 36471,
        //
        // Summary:
        //     Original was GL_TESS_GEN_VERTEX_ORDER = 0x8E78
        TessGenVertexOrder = 36472,
        //
        // Summary:
        //     Original was GL_TESS_GEN_POINT_MODE = 0x8E79
        TessGenPointMode = 36473,
        //
        // Summary:
        //     Original was GL_MAX_COMPUTE_WORK_GROUP_SIZE = 0x91BF
        MaxComputeWorkGroupSize = 37311,
        //
        // Summary:
        //     Original was GL_ACTIVE_ATOMIC_COUNTER_BUFFERS = 0x92D9
        ActiveAtomicCounterBuffers = 37593
    }
    #endregion

    #region Texture
    public enum GenerateMipmapTarget : int
    {
        /// <summary>
        /// Original was GL_TEXTURE_1D = 0x0DE0
        /// </summary>
        Texture1D = ((int)0x0DE0),
        /// <summary>
        /// Original was GL_TEXTURE_2D = 0x0DE1
        /// </summary>
        Texture2D = ((int)0x0DE1),
        /// <summary>
        /// Original was GL_TEXTURE_3D = 0x806F
        /// </summary>
        Texture3D = ((int)0x806F),
        /// <summary>
        /// Original was GL_TEXTURE_CUBE_MAP = 0x8513
        /// </summary>
        TextureCubeMap = ((int)0x8513),
        /// <summary>
        /// Original was GL_TEXTURE_1D_ARRAY = 0x8C18
        /// </summary>
        Texture1DArray = ((int)0x8C18),
        /// <summary>
        /// Original was GL_TEXTURE_2D_ARRAY = 0x8C1A
        /// </summary>
        Texture2DArray = ((int)0x8C1A),
        /// <summary>
        /// Original was GL_TEXTURE_CUBE_MAP_ARRAY = 0x9009
        /// </summary>
        TextureCubeMapArray = ((int)0x9009),
        /// <summary>
        /// Original was GL_TEXTURE_2D_MULTISAMPLE = 0x9100
        /// </summary>
        Texture2DMultisample = ((int)0x9100),
        /// <summary>
        /// Original was GL_TEXTURE_2D_MULTISAMPLE_ARRAY = 0x9102
        /// </summary>
        Texture2DMultisampleArray = ((int)0x9102),
    }
    public enum GetTextureParameter : int
    {
        /// <summary>
        /// Original was GL_TEXTURE_WIDTH = 0x1000
        /// </summary>
        TextureWidth = ((int)0x1000),
        /// <summary>
        /// Original was GL_TEXTURE_HEIGHT = 0x1001
        /// </summary>
        TextureHeight = ((int)0x1001),
        /// <summary>
        /// Original was GL_TEXTURE_COMPONENTS = 0x1003
        /// </summary>
        TextureComponents = ((int)0x1003),
        /// <summary>
        /// Original was GL_TEXTURE_INTERNAL_FORMAT = 0x1003
        /// </summary>
        TextureInternalFormat = ((int)0x1003),
        /// <summary>
        /// Original was GL_TEXTURE_BORDER_COLOR = 0x1004
        /// </summary>
        TextureBorderColor = ((int)0x1004),
        /// <summary>
        /// Original was GL_TEXTURE_BORDER_COLOR_NV = 0x1004
        /// </summary>
        TextureBorderColorNv = ((int)0x1004),
        /// <summary>
        /// Original was GL_TEXTURE_BORDER = 0x1005
        /// </summary>
        TextureBorder = ((int)0x1005),
        /// <summary>
        /// Original was GL_TEXTURE_TARGET = 0x1006
        /// </summary>
        TextureTarget = ((int)0x1006),
        /// <summary>
        /// Original was GL_TEXTURE_MAG_FILTER = 0x2800
        /// </summary>
        TextureMagFilter = ((int)0x2800),
        /// <summary>
        /// Original was GL_TEXTURE_MIN_FILTER = 0x2801
        /// </summary>
        TextureMinFilter = ((int)0x2801),
        /// <summary>
        /// Original was GL_TEXTURE_WRAP_S = 0x2802
        /// </summary>
        TextureWrapS = ((int)0x2802),
        /// <summary>
        /// Original was GL_TEXTURE_WRAP_T = 0x2803
        /// </summary>
        TextureWrapT = ((int)0x2803),
        /// <summary>
        /// Original was GL_TEXTURE_RED_SIZE = 0x805C
        /// </summary>
        TextureRedSize = ((int)0x805C),
        /// <summary>
        /// Original was GL_TEXTURE_GREEN_SIZE = 0x805D
        /// </summary>
        TextureGreenSize = ((int)0x805D),
        /// <summary>
        /// Original was GL_TEXTURE_BLUE_SIZE = 0x805E
        /// </summary>
        TextureBlueSize = ((int)0x805E),
        /// <summary>
        /// Original was GL_TEXTURE_ALPHA_SIZE = 0x805F
        /// </summary>
        TextureAlphaSize = ((int)0x805F),
        /// <summary>
        /// Original was GL_TEXTURE_LUMINANCE_SIZE = 0x8060
        /// </summary>
        TextureLuminanceSize = ((int)0x8060),
        /// <summary>
        /// Original was GL_TEXTURE_INTENSITY_SIZE = 0x8061
        /// </summary>
        TextureIntensitySize = ((int)0x8061),
        /// <summary>
        /// Original was GL_TEXTURE_PRIORITY = 0x8066
        /// </summary>
        TexturePriority = ((int)0x8066),
        /// <summary>
        /// Original was GL_TEXTURE_RESIDENT = 0x8067
        /// </summary>
        TextureResident = ((int)0x8067),
        /// <summary>
        /// Original was GL_TEXTURE_DEPTH = 0x8071
        /// </summary>
        TextureDepth = ((int)0x8071),
        /// <summary>
        /// Original was GL_TEXTURE_DEPTH_EXT = 0x8071
        /// </summary>
        TextureDepthExt = ((int)0x8071),
        /// <summary>
        /// Original was GL_TEXTURE_WRAP_R = 0x8072
        /// </summary>
        TextureWrapR = ((int)0x8072),
        /// <summary>
        /// Original was GL_TEXTURE_WRAP_R_EXT = 0x8072
        /// </summary>
        TextureWrapRExt = ((int)0x8072),
        /// <summary>
        /// Original was GL_DETAIL_TEXTURE_LEVEL_SGIS = 0x809A
        /// </summary>
        DetailTextureLevelSgis = ((int)0x809A),
        /// <summary>
        /// Original was GL_DETAIL_TEXTURE_MODE_SGIS = 0x809B
        /// </summary>
        DetailTextureModeSgis = ((int)0x809B),
        /// <summary>
        /// Original was GL_DETAIL_TEXTURE_FUNC_POINTS_SGIS = 0x809C
        /// </summary>
        DetailTextureFuncPointsSgis = ((int)0x809C),
        /// <summary>
        /// Original was GL_SHARPEN_TEXTURE_FUNC_POINTS_SGIS = 0x80B0
        /// </summary>
        SharpenTextureFuncPointsSgis = ((int)0x80B0),
        /// <summary>
        /// Original was GL_SHADOW_AMBIENT_SGIX = 0x80BF
        /// </summary>
        ShadowAmbientSgix = ((int)0x80BF),
        /// <summary>
        /// Original was GL_DUAL_TEXTURE_SELECT_SGIS = 0x8124
        /// </summary>
        DualTextureSelectSgis = ((int)0x8124),
        /// <summary>
        /// Original was GL_QUAD_TEXTURE_SELECT_SGIS = 0x8125
        /// </summary>
        QuadTextureSelectSgis = ((int)0x8125),
        /// <summary>
        /// Original was GL_TEXTURE_4DSIZE_SGIS = 0x8136
        /// </summary>
        Texture4DsizeSgis = ((int)0x8136),
        /// <summary>
        /// Original was GL_TEXTURE_WRAP_Q_SGIS = 0x8137
        /// </summary>
        TextureWrapQSgis = ((int)0x8137),
        /// <summary>
        /// Original was GL_TEXTURE_MIN_LOD = 0x813A
        /// </summary>
        TextureMinLod = ((int)0x813A),
        /// <summary>
        /// Original was GL_TEXTURE_MIN_LOD_SGIS = 0x813A
        /// </summary>
        TextureMinLodSgis = ((int)0x813A),
        /// <summary>
        /// Original was GL_TEXTURE_MAX_LOD = 0x813B
        /// </summary>
        TextureMaxLod = ((int)0x813B),
        /// <summary>
        /// Original was GL_TEXTURE_MAX_LOD_SGIS = 0x813B
        /// </summary>
        TextureMaxLodSgis = ((int)0x813B),
        /// <summary>
        /// Original was GL_TEXTURE_BASE_LEVEL = 0x813C
        /// </summary>
        TextureBaseLevel = ((int)0x813C),
        /// <summary>
        /// Original was GL_TEXTURE_BASE_LEVEL_SGIS = 0x813C
        /// </summary>
        TextureBaseLevelSgis = ((int)0x813C),
        /// <summary>
        /// Original was GL_TEXTURE_MAX_LEVEL = 0x813D
        /// </summary>
        TextureMaxLevel = ((int)0x813D),
        /// <summary>
        /// Original was GL_TEXTURE_MAX_LEVEL_SGIS = 0x813D
        /// </summary>
        TextureMaxLevelSgis = ((int)0x813D),
        /// <summary>
        /// Original was GL_TEXTURE_FILTER4_SIZE_SGIS = 0x8147
        /// </summary>
        TextureFilter4SizeSgis = ((int)0x8147),
        /// <summary>
        /// Original was GL_TEXTURE_CLIPMAP_CENTER_SGIX = 0x8171
        /// </summary>
        TextureClipmapCenterSgix = ((int)0x8171),
        /// <summary>
        /// Original was GL_TEXTURE_CLIPMAP_FRAME_SGIX = 0x8172
        /// </summary>
        TextureClipmapFrameSgix = ((int)0x8172),
        /// <summary>
        /// Original was GL_TEXTURE_CLIPMAP_OFFSET_SGIX = 0x8173
        /// </summary>
        TextureClipmapOffsetSgix = ((int)0x8173),
        /// <summary>
        /// Original was GL_TEXTURE_CLIPMAP_VIRTUAL_DEPTH_SGIX = 0x8174
        /// </summary>
        TextureClipmapVirtualDepthSgix = ((int)0x8174),
        /// <summary>
        /// Original was GL_TEXTURE_CLIPMAP_LOD_OFFSET_SGIX = 0x8175
        /// </summary>
        TextureClipmapLodOffsetSgix = ((int)0x8175),
        /// <summary>
        /// Original was GL_TEXTURE_CLIPMAP_DEPTH_SGIX = 0x8176
        /// </summary>
        TextureClipmapDepthSgix = ((int)0x8176),
        /// <summary>
        /// Original was GL_POST_TEXTURE_FILTER_BIAS_SGIX = 0x8179
        /// </summary>
        PostTextureFilterBiasSgix = ((int)0x8179),
        /// <summary>
        /// Original was GL_POST_TEXTURE_FILTER_SCALE_SGIX = 0x817A
        /// </summary>
        PostTextureFilterScaleSgix = ((int)0x817A),
        /// <summary>
        /// Original was GL_TEXTURE_LOD_BIAS_S_SGIX = 0x818E
        /// </summary>
        TextureLodBiasSSgix = ((int)0x818E),
        /// <summary>
        /// Original was GL_TEXTURE_LOD_BIAS_T_SGIX = 0x818F
        /// </summary>
        TextureLodBiasTSgix = ((int)0x818F),
        /// <summary>
        /// Original was GL_TEXTURE_LOD_BIAS_R_SGIX = 0x8190
        /// </summary>
        TextureLodBiasRSgix = ((int)0x8190),
        /// <summary>
        /// Original was GL_GENERATE_MIPMAP = 0x8191
        /// </summary>
        GenerateMipmap = ((int)0x8191),
        /// <summary>
        /// Original was GL_GENERATE_MIPMAP_SGIS = 0x8191
        /// </summary>
        GenerateMipmapSgis = ((int)0x8191),
        /// <summary>
        /// Original was GL_TEXTURE_COMPARE_SGIX = 0x819A
        /// </summary>
        TextureCompareSgix = ((int)0x819A),
        /// <summary>
        /// Original was GL_TEXTURE_COMPARE_OPERATOR_SGIX = 0x819B
        /// </summary>
        TextureCompareOperatorSgix = ((int)0x819B),
        /// <summary>
        /// Original was GL_TEXTURE_LEQUAL_R_SGIX = 0x819C
        /// </summary>
        TextureLequalRSgix = ((int)0x819C),
        /// <summary>
        /// Original was GL_TEXTURE_GEQUAL_R_SGIX = 0x819D
        /// </summary>
        TextureGequalRSgix = ((int)0x819D),
        /// <summary>
        /// Original was GL_TEXTURE_VIEW_MIN_LEVEL = 0x82DB
        /// </summary>
        TextureViewMinLevel = ((int)0x82DB),
        /// <summary>
        /// Original was GL_TEXTURE_VIEW_NUM_LEVELS = 0x82DC
        /// </summary>
        TextureViewNumLevels = ((int)0x82DC),
        /// <summary>
        /// Original was GL_TEXTURE_VIEW_MIN_LAYER = 0x82DD
        /// </summary>
        TextureViewMinLayer = ((int)0x82DD),
        /// <summary>
        /// Original was GL_TEXTURE_VIEW_NUM_LAYERS = 0x82DE
        /// </summary>
        TextureViewNumLayers = ((int)0x82DE),
        /// <summary>
        /// Original was GL_TEXTURE_IMMUTABLE_LEVELS = 0x82DF
        /// </summary>
        TextureImmutableLevels = ((int)0x82DF),
        /// <summary>
        /// Original was GL_TEXTURE_MAX_CLAMP_S_SGIX = 0x8369
        /// </summary>
        TextureMaxClampSSgix = ((int)0x8369),
        /// <summary>
        /// Original was GL_TEXTURE_MAX_CLAMP_T_SGIX = 0x836A
        /// </summary>
        TextureMaxClampTSgix = ((int)0x836A),
        /// <summary>
        /// Original was GL_TEXTURE_MAX_CLAMP_R_SGIX = 0x836B
        /// </summary>
        TextureMaxClampRSgix = ((int)0x836B),
        /// <summary>
        /// Original was GL_TEXTURE_COMPRESSED_IMAGE_SIZE = 0x86A0
        /// </summary>
        TextureCompressedImageSize = ((int)0x86A0),
        /// <summary>
        /// Original was GL_TEXTURE_COMPRESSED = 0x86A1
        /// </summary>
        TextureCompressed = ((int)0x86A1),
        /// <summary>
        /// Original was GL_TEXTURE_DEPTH_SIZE = 0x884A
        /// </summary>
        TextureDepthSize = ((int)0x884A),
        /// <summary>
        /// Original was GL_DEPTH_TEXTURE_MODE = 0x884B
        /// </summary>
        DepthTextureMode = ((int)0x884B),
        /// <summary>
        /// Original was GL_TEXTURE_COMPARE_MODE = 0x884C
        /// </summary>
        TextureCompareMode = ((int)0x884C),
        /// <summary>
        /// Original was GL_TEXTURE_COMPARE_FUNC = 0x884D
        /// </summary>
        TextureCompareFunc = ((int)0x884D),
        /// <summary>
        /// Original was GL_TEXTURE_STENCIL_SIZE = 0x88F1
        /// </summary>
        TextureStencilSize = ((int)0x88F1),
        /// <summary>
        /// Original was GL_TEXTURE_RED_TYPE = 0x8C10
        /// </summary>
        TextureRedType = ((int)0x8C10),
        /// <summary>
        /// Original was GL_TEXTURE_GREEN_TYPE = 0x8C11
        /// </summary>
        TextureGreenType = ((int)0x8C11),
        /// <summary>
        /// Original was GL_TEXTURE_BLUE_TYPE = 0x8C12
        /// </summary>
        TextureBlueType = ((int)0x8C12),
        /// <summary>
        /// Original was GL_TEXTURE_ALPHA_TYPE = 0x8C13
        /// </summary>
        TextureAlphaType = ((int)0x8C13),
        /// <summary>
        /// Original was GL_TEXTURE_LUMINANCE_TYPE = 0x8C14
        /// </summary>
        TextureLuminanceType = ((int)0x8C14),
        /// <summary>
        /// Original was GL_TEXTURE_INTENSITY_TYPE = 0x8C15
        /// </summary>
        TextureIntensityType = ((int)0x8C15),
        /// <summary>
        /// Original was GL_TEXTURE_DEPTH_TYPE = 0x8C16
        /// </summary>
        TextureDepthType = ((int)0x8C16),
        /// <summary>
        /// Original was GL_TEXTURE_SHARED_SIZE = 0x8C3F
        /// </summary>
        TextureSharedSize = ((int)0x8C3F),
        /// <summary>
        /// Original was GL_TEXTURE_SWIZZLE_R = 0x8E42
        /// </summary>
        TextureSwizzleR = ((int)0x8E42),
        /// <summary>
        /// Original was GL_TEXTURE_SWIZZLE_G = 0x8E43
        /// </summary>
        TextureSwizzleG = ((int)0x8E43),
        /// <summary>
        /// Original was GL_TEXTURE_SWIZZLE_B = 0x8E44
        /// </summary>
        TextureSwizzleB = ((int)0x8E44),
        /// <summary>
        /// Original was GL_TEXTURE_SWIZZLE_A = 0x8E45
        /// </summary>
        TextureSwizzleA = ((int)0x8E45),
        /// <summary>
        /// Original was GL_TEXTURE_SWIZZLE_RGBA = 0x8E46
        /// </summary>
        TextureSwizzleRgba = ((int)0x8E46),
        /// <summary>
        /// Original was GL_IMAGE_FORMAT_COMPATIBILITY_TYPE = 0x90C7
        /// </summary>
        ImageFormatCompatibilityType = ((int)0x90C7),
        /// <summary>
        /// Original was GL_TEXTURE_SAMPLES = 0x9106
        /// </summary>
        TextureSamples = ((int)0x9106),
        /// <summary>
        /// Original was GL_TEXTURE_FIXED_SAMPLE_LOCATIONS = 0x9107
        /// </summary>
        TextureFixedSampleLocations = ((int)0x9107),
        /// <summary>
        /// Original was GL_TEXTURE_IMMUTABLE_FORMAT = 0x912F
        /// </summary>
        TextureImmutableFormat = ((int)0x912F),
    }
    public enum TextureEnvMode : int
    {
        /// <summary>
        /// Original was GL_ADD = 0x0104
        /// </summary>
        Add = ((int)0x0104),
        /// <summary>
        /// Original was GL_BLEND = 0x0BE2
        /// </summary>
        Blend = ((int)0x0BE2),
        /// <summary>
        /// Original was GL_REPLACE = 0x1E01
        /// </summary>
        Replace = ((int)0x1E01),
        /// <summary>
        /// Original was GL_MODULATE = 0x2100
        /// </summary>
        Modulate = ((int)0x2100),
        /// <summary>
        /// Original was GL_DECAL = 0x2101
        /// </summary>
        Decal = ((int)0x2101),
        /// <summary>
        /// Original was GL_REPLACE_EXT = 0x8062
        /// </summary>
        ReplaceExt = ((int)0x8062),
        /// <summary>
        /// Original was GL_TEXTURE_ENV_BIAS_SGIX = 0x80BE
        /// </summary>
        TextureEnvBiasSgix = ((int)0x80BE),
        /// <summary>
        /// Original was GL_COMBINE = 0x8570
        /// </summary>
        Combine = ((int)0x8570),
    }
    public enum TextureMinFilter : int
    {
        /// <summary>
        /// Original was GL_NEAREST = 0x2600
        /// </summary>
        Nearest = ((int)0x2600),
        /// <summary>
        /// Original was GL_LINEAR = 0x2601
        /// </summary>
        Linear = ((int)0x2601),
        /// <summary>
        /// Original was GL_NEAREST_MIPMAP_NEAREST = 0x2700
        /// </summary>
        NearestMipmapNearest = ((int)0x2700),
        /// <summary>
        /// Original was GL_LINEAR_MIPMAP_NEAREST = 0x2701
        /// </summary>
        LinearMipmapNearest = ((int)0x2701),
        /// <summary>
        /// Original was GL_NEAREST_MIPMAP_LINEAR = 0x2702
        /// </summary>
        NearestMipmapLinear = ((int)0x2702),
        /// <summary>
        /// Original was GL_LINEAR_MIPMAP_LINEAR = 0x2703
        /// </summary>
        LinearMipmapLinear = ((int)0x2703),
        /// <summary>
        /// Original was GL_FILTER4_SGIS = 0x8146
        /// </summary>
        Filter4Sgis = ((int)0x8146),
        /// <summary>
        /// Original was GL_LINEAR_CLIPMAP_LINEAR_SGIX = 0x8170
        /// </summary>
        LinearClipmapLinearSgix = ((int)0x8170),
        /// <summary>
        /// Original was GL_PIXEL_TEX_GEN_Q_CEILING_SGIX = 0x8184
        /// </summary>
        PixelTexGenQCeilingSgix = ((int)0x8184),
        /// <summary>
        /// Original was GL_PIXEL_TEX_GEN_Q_ROUND_SGIX = 0x8185
        /// </summary>
        PixelTexGenQRoundSgix = ((int)0x8185),
        /// <summary>
        /// Original was GL_PIXEL_TEX_GEN_Q_FLOOR_SGIX = 0x8186
        /// </summary>
        PixelTexGenQFloorSgix = ((int)0x8186),
        /// <summary>
        /// Original was GL_NEAREST_CLIPMAP_NEAREST_SGIX = 0x844D
        /// </summary>
        NearestClipmapNearestSgix = ((int)0x844D),
        /// <summary>
        /// Original was GL_NEAREST_CLIPMAP_LINEAR_SGIX = 0x844E
        /// </summary>
        NearestClipmapLinearSgix = ((int)0x844E),
        /// <summary>
        /// Original was GL_LINEAR_CLIPMAP_NEAREST_SGIX = 0x844F
        /// </summary>
        LinearClipmapNearestSgix = ((int)0x844F),
    }
    public enum TextureMagFilter : int
    {
        /// <summary>
        /// Original was GL_NEAREST = 0x2600
        /// </summary>
        Nearest = ((int)0x2600),
        /// <summary>
        /// Original was GL_LINEAR = 0x2601
        /// </summary>
        Linear = ((int)0x2601),
        /// <summary>
        /// Original was GL_LINEAR_DETAIL_SGIS = 0x8097
        /// </summary>
        LinearDetailSgis = ((int)0x8097),
        /// <summary>
        /// Original was GL_LINEAR_DETAIL_ALPHA_SGIS = 0x8098
        /// </summary>
        LinearDetailAlphaSgis = ((int)0x8098),
        /// <summary>
        /// Original was GL_LINEAR_DETAIL_COLOR_SGIS = 0x8099
        /// </summary>
        LinearDetailColorSgis = ((int)0x8099),
        /// <summary>
        /// Original was GL_LINEAR_SHARPEN_SGIS = 0x80AD
        /// </summary>
        LinearSharpenSgis = ((int)0x80AD),
        /// <summary>
        /// Original was GL_LINEAR_SHARPEN_ALPHA_SGIS = 0x80AE
        /// </summary>
        LinearSharpenAlphaSgis = ((int)0x80AE),
        /// <summary>
        /// Original was GL_LINEAR_SHARPEN_COLOR_SGIS = 0x80AF
        /// </summary>
        LinearSharpenColorSgis = ((int)0x80AF),
        /// <summary>
        /// Original was GL_FILTER4_SGIS = 0x8146
        /// </summary>
        Filter4Sgis = ((int)0x8146),
        /// <summary>
        /// Original was GL_PIXEL_TEX_GEN_Q_CEILING_SGIX = 0x8184
        /// </summary>
        PixelTexGenQCeilingSgix = ((int)0x8184),
        /// <summary>
        /// Original was GL_PIXEL_TEX_GEN_Q_ROUND_SGIX = 0x8185
        /// </summary>
        PixelTexGenQRoundSgix = ((int)0x8185),
        /// <summary>
        /// Original was GL_PIXEL_TEX_GEN_Q_FLOOR_SGIX = 0x8186
        /// </summary>
        PixelTexGenQFloorSgix = ((int)0x8186),
    }
    public enum TextureParameterName : int
    {
        /// <summary>
        /// Original was GL_TEXTURE_BORDER_COLOR = 0x1004
        /// </summary>
        TextureBorderColor = ((int)0x1004),
        /// <summary>
        /// Original was GL_TEXTURE_MAG_FILTER = 0x2800
        /// </summary>
        TextureMagFilter = ((int)0x2800),
        /// <summary>
        /// Original was GL_TEXTURE_MIN_FILTER = 0x2801
        /// </summary>
        TextureMinFilter = ((int)0x2801),
        /// <summary>
        /// Original was GL_TEXTURE_WRAP_S = 0x2802
        /// </summary>
        TextureWrapS = ((int)0x2802),
        /// <summary>
        /// Original was GL_TEXTURE_WRAP_T = 0x2803
        /// </summary>
        TextureWrapT = ((int)0x2803),
        /// <summary>
        /// Original was GL_TEXTURE_PRIORITY = 0x8066
        /// </summary>
        TexturePriority = ((int)0x8066),
        /// <summary>
        /// Original was GL_TEXTURE_PRIORITY_EXT = 0x8066
        /// </summary>
        TexturePriorityExt = ((int)0x8066),
        /// <summary>
        /// Original was GL_TEXTURE_DEPTH = 0x8071
        /// </summary>
        TextureDepth = ((int)0x8071),
        /// <summary>
        /// Original was GL_TEXTURE_WRAP_R = 0x8072
        /// </summary>
        TextureWrapR = ((int)0x8072),
        /// <summary>
        /// Original was GL_TEXTURE_WRAP_R_EXT = 0x8072
        /// </summary>
        TextureWrapRExt = ((int)0x8072),
        /// <summary>
        /// Original was GL_TEXTURE_WRAP_R_OES = 0x8072
        /// </summary>
        TextureWrapROes = ((int)0x8072),
        /// <summary>
        /// Original was GL_DETAIL_TEXTURE_LEVEL_SGIS = 0x809A
        /// </summary>
        DetailTextureLevelSgis = ((int)0x809A),
        /// <summary>
        /// Original was GL_DETAIL_TEXTURE_MODE_SGIS = 0x809B
        /// </summary>
        DetailTextureModeSgis = ((int)0x809B),
        /// <summary>
        /// Original was GL_SHADOW_AMBIENT_SGIX = 0x80BF
        /// </summary>
        ShadowAmbientSgix = ((int)0x80BF),
        /// <summary>
        /// Original was GL_TEXTURE_COMPARE_FAIL_VALUE = 0x80BF
        /// </summary>
        TextureCompareFailValue = ((int)0x80BF),
        /// <summary>
        /// Original was GL_DUAL_TEXTURE_SELECT_SGIS = 0x8124
        /// </summary>
        DualTextureSelectSgis = ((int)0x8124),
        /// <summary>
        /// Original was GL_QUAD_TEXTURE_SELECT_SGIS = 0x8125
        /// </summary>
        QuadTextureSelectSgis = ((int)0x8125),
        /// <summary>
        /// Original was GL_CLAMP_TO_BORDER = 0x812D
        /// </summary>
        ClampToBorder = ((int)0x812D),
        /// <summary>
        /// Original was GL_CLAMP_TO_EDGE = 0x812F
        /// </summary>
        ClampToEdge = ((int)0x812F),
        /// <summary>
        /// Original was GL_TEXTURE_WRAP_Q_SGIS = 0x8137
        /// </summary>
        TextureWrapQSgis = ((int)0x8137),
        /// <summary>
        /// Original was GL_TEXTURE_MIN_LOD = 0x813A
        /// </summary>
        TextureMinLod = ((int)0x813A),
        /// <summary>
        /// Original was GL_TEXTURE_MAX_LOD = 0x813B
        /// </summary>
        TextureMaxLod = ((int)0x813B),
        /// <summary>
        /// Original was GL_TEXTURE_BASE_LEVEL = 0x813C
        /// </summary>
        TextureBaseLevel = ((int)0x813C),
        /// <summary>
        /// Original was GL_TEXTURE_MAX_LEVEL = 0x813D
        /// </summary>
        TextureMaxLevel = ((int)0x813D),
        /// <summary>
        /// Original was GL_TEXTURE_CLIPMAP_CENTER_SGIX = 0x8171
        /// </summary>
        TextureClipmapCenterSgix = ((int)0x8171),
        /// <summary>
        /// Original was GL_TEXTURE_CLIPMAP_FRAME_SGIX = 0x8172
        /// </summary>
        TextureClipmapFrameSgix = ((int)0x8172),
        /// <summary>
        /// Original was GL_TEXTURE_CLIPMAP_OFFSET_SGIX = 0x8173
        /// </summary>
        TextureClipmapOffsetSgix = ((int)0x8173),
        /// <summary>
        /// Original was GL_TEXTURE_CLIPMAP_VIRTUAL_DEPTH_SGIX = 0x8174
        /// </summary>
        TextureClipmapVirtualDepthSgix = ((int)0x8174),
        /// <summary>
        /// Original was GL_TEXTURE_CLIPMAP_LOD_OFFSET_SGIX = 0x8175
        /// </summary>
        TextureClipmapLodOffsetSgix = ((int)0x8175),
        /// <summary>
        /// Original was GL_TEXTURE_CLIPMAP_DEPTH_SGIX = 0x8176
        /// </summary>
        TextureClipmapDepthSgix = ((int)0x8176),
        /// <summary>
        /// Original was GL_POST_TEXTURE_FILTER_BIAS_SGIX = 0x8179
        /// </summary>
        PostTextureFilterBiasSgix = ((int)0x8179),
        /// <summary>
        /// Original was GL_POST_TEXTURE_FILTER_SCALE_SGIX = 0x817A
        /// </summary>
        PostTextureFilterScaleSgix = ((int)0x817A),
        /// <summary>
        /// Original was GL_TEXTURE_LOD_BIAS_S_SGIX = 0x818E
        /// </summary>
        TextureLodBiasSSgix = ((int)0x818E),
        /// <summary>
        /// Original was GL_TEXTURE_LOD_BIAS_T_SGIX = 0x818F
        /// </summary>
        TextureLodBiasTSgix = ((int)0x818F),
        /// <summary>
        /// Original was GL_TEXTURE_LOD_BIAS_R_SGIX = 0x8190
        /// </summary>
        TextureLodBiasRSgix = ((int)0x8190),
        /// <summary>
        /// Original was GL_GENERATE_MIPMAP = 0x8191
        /// </summary>
        GenerateMipmap = ((int)0x8191),
        /// <summary>
        /// Original was GL_GENERATE_MIPMAP_SGIS = 0x8191
        /// </summary>
        GenerateMipmapSgis = ((int)0x8191),
        /// <summary>
        /// Original was GL_TEXTURE_COMPARE_SGIX = 0x819A
        /// </summary>
        TextureCompareSgix = ((int)0x819A),
        /// <summary>
        /// Original was GL_TEXTURE_MAX_CLAMP_S_SGIX = 0x8369
        /// </summary>
        TextureMaxClampSSgix = ((int)0x8369),
        /// <summary>
        /// Original was GL_TEXTURE_MAX_CLAMP_T_SGIX = 0x836A
        /// </summary>
        TextureMaxClampTSgix = ((int)0x836A),
        /// <summary>
        /// Original was GL_TEXTURE_MAX_CLAMP_R_SGIX = 0x836B
        /// </summary>
        TextureMaxClampRSgix = ((int)0x836B),
        /// <summary>
        /// Original was GL_TEXTURE_LOD_BIAS = 0x8501
        /// </summary>
        TextureLodBias = ((int)0x8501),
        /// <summary>
        /// Original was GL_DEPTH_TEXTURE_MODE = 0x884B
        /// </summary>
        DepthTextureMode = ((int)0x884B),
        /// <summary>
        /// Original was GL_TEXTURE_COMPARE_MODE = 0x884C
        /// </summary>
        TextureCompareMode = ((int)0x884C),
        /// <summary>
        /// Original was GL_TEXTURE_COMPARE_FUNC = 0x884D
        /// </summary>
        TextureCompareFunc = ((int)0x884D),
        /// <summary>
        /// Original was GL_TEXTURE_SWIZZLE_R = 0x8E42
        /// </summary>
        TextureSwizzleR = ((int)0x8E42),
        /// <summary>
        /// Original was GL_TEXTURE_SWIZZLE_G = 0x8E43
        /// </summary>
        TextureSwizzleG = ((int)0x8E43),
        /// <summary>
        /// Original was GL_TEXTURE_SWIZZLE_B = 0x8E44
        /// </summary>
        TextureSwizzleB = ((int)0x8E44),
        /// <summary>
        /// Original was GL_TEXTURE_SWIZZLE_A = 0x8E45
        /// </summary>
        TextureSwizzleA = ((int)0x8E45),
        /// <summary>
        /// Original was GL_TEXTURE_SWIZZLE_RGBA = 0x8E46
        /// </summary>
        TextureSwizzleRgba = ((int)0x8E46),
    }
    public enum TextureTarget : int
    {
        /// <summary>
        /// Original was GL_TEXTURE_1D = 0x0DE0
        /// </summary>
        Texture1D = ((int)0x0DE0),
        /// <summary>
        /// Original was GL_TEXTURE_2D = 0x0DE1
        /// </summary>
        Texture2D = ((int)0x0DE1),
        /// <summary>
        /// Original was GL_PROXY_TEXTURE_1D = 0x8063
        /// </summary>
        ProxyTexture1D = ((int)0x8063),
        /// <summary>
        /// Original was GL_PROXY_TEXTURE_1D_EXT = 0x8063
        /// </summary>
        ProxyTexture1DExt = ((int)0x8063),
        /// <summary>
        /// Original was GL_PROXY_TEXTURE_2D = 0x8064
        /// </summary>
        ProxyTexture2D = ((int)0x8064),
        /// <summary>
        /// Original was GL_PROXY_TEXTURE_2D_EXT = 0x8064
        /// </summary>
        ProxyTexture2DExt = ((int)0x8064),
        /// <summary>
        /// Original was GL_TEXTURE_3D = 0x806F
        /// </summary>
        Texture3D = ((int)0x806F),
        /// <summary>
        /// Original was GL_TEXTURE_3D_EXT = 0x806F
        /// </summary>
        Texture3DExt = ((int)0x806F),
        /// <summary>
        /// Original was GL_TEXTURE_3D_OES = 0x806F
        /// </summary>
        Texture3DOes = ((int)0x806F),
        /// <summary>
        /// Original was GL_PROXY_TEXTURE_3D = 0x8070
        /// </summary>
        ProxyTexture3D = ((int)0x8070),
        /// <summary>
        /// Original was GL_PROXY_TEXTURE_3D_EXT = 0x8070
        /// </summary>
        ProxyTexture3DExt = ((int)0x8070),
        /// <summary>
        /// Original was GL_DETAIL_TEXTURE_2D_SGIS = 0x8095
        /// </summary>
        DetailTexture2DSgis = ((int)0x8095),
        /// <summary>
        /// Original was GL_TEXTURE_4D_SGIS = 0x8134
        /// </summary>
        Texture4DSgis = ((int)0x8134),
        /// <summary>
        /// Original was GL_PROXY_TEXTURE_4D_SGIS = 0x8135
        /// </summary>
        ProxyTexture4DSgis = ((int)0x8135),
        /// <summary>
        /// Original was GL_TEXTURE_MIN_LOD = 0x813A
        /// </summary>
        TextureMinLod = ((int)0x813A),
        /// <summary>
        /// Original was GL_TEXTURE_MIN_LOD_SGIS = 0x813A
        /// </summary>
        TextureMinLodSgis = ((int)0x813A),
        /// <summary>
        /// Original was GL_TEXTURE_MAX_LOD = 0x813B
        /// </summary>
        TextureMaxLod = ((int)0x813B),
        /// <summary>
        /// Original was GL_TEXTURE_MAX_LOD_SGIS = 0x813B
        /// </summary>
        TextureMaxLodSgis = ((int)0x813B),
        /// <summary>
        /// Original was GL_TEXTURE_BASE_LEVEL = 0x813C
        /// </summary>
        TextureBaseLevel = ((int)0x813C),
        /// <summary>
        /// Original was GL_TEXTURE_BASE_LEVEL_SGIS = 0x813C
        /// </summary>
        TextureBaseLevelSgis = ((int)0x813C),
        /// <summary>
        /// Original was GL_TEXTURE_MAX_LEVEL = 0x813D
        /// </summary>
        TextureMaxLevel = ((int)0x813D),
        /// <summary>
        /// Original was GL_TEXTURE_MAX_LEVEL_SGIS = 0x813D
        /// </summary>
        TextureMaxLevelSgis = ((int)0x813D),
        /// <summary>
        /// Original was GL_TEXTURE_RECTANGLE = 0x84F5
        /// </summary>
        TextureRectangle = ((int)0x84F5),
        /// <summary>
        /// Original was GL_TEXTURE_RECTANGLE_ARB = 0x84F5
        /// </summary>
        TextureRectangleArb = ((int)0x84F5),
        /// <summary>
        /// Original was GL_TEXTURE_RECTANGLE_NV = 0x84F5
        /// </summary>
        TextureRectangleNv = ((int)0x84F5),
        /// <summary>
        /// Original was GL_PROXY_TEXTURE_RECTANGLE = 0x84F7
        /// </summary>
        ProxyTextureRectangle = ((int)0x84F7),
        /// <summary>
        /// Original was GL_TEXTURE_CUBE_MAP = 0x8513
        /// </summary>
        TextureCubeMap = ((int)0x8513),
        /// <summary>
        /// Original was GL_TEXTURE_BINDING_CUBE_MAP = 0x8514
        /// </summary>
        TextureBindingCubeMap = ((int)0x8514),
        /// <summary>
        /// Original was GL_TEXTURE_CUBE_MAP_POSITIVE_X = 0x8515
        /// </summary>
        TextureCubeMapPositiveX = ((int)0x8515),
        /// <summary>
        /// Original was GL_TEXTURE_CUBE_MAP_NEGATIVE_X = 0x8516
        /// </summary>
        TextureCubeMapNegativeX = ((int)0x8516),
        /// <summary>
        /// Original was GL_TEXTURE_CUBE_MAP_POSITIVE_Y = 0x8517
        /// </summary>
        TextureCubeMapPositiveY = ((int)0x8517),
        /// <summary>
        /// Original was GL_TEXTURE_CUBE_MAP_NEGATIVE_Y = 0x8518
        /// </summary>
        TextureCubeMapNegativeY = ((int)0x8518),
        /// <summary>
        /// Original was GL_TEXTURE_CUBE_MAP_POSITIVE_Z = 0x8519
        /// </summary>
        TextureCubeMapPositiveZ = ((int)0x8519),
        /// <summary>
        /// Original was GL_TEXTURE_CUBE_MAP_NEGATIVE_Z = 0x851A
        /// </summary>
        TextureCubeMapNegativeZ = ((int)0x851A),
        /// <summary>
        /// Original was GL_PROXY_TEXTURE_CUBE_MAP = 0x851B
        /// </summary>
        ProxyTextureCubeMap = ((int)0x851B),
        /// <summary>
        /// Original was GL_TEXTURE_1D_ARRAY = 0x8C18
        /// </summary>
        Texture1DArray = ((int)0x8C18),
        /// <summary>
        /// Original was GL_PROXY_TEXTURE_1D_ARRAY = 0x8C19
        /// </summary>
        ProxyTexture1DArray = ((int)0x8C19),
        /// <summary>
        /// Original was GL_TEXTURE_2D_ARRAY = 0x8C1A
        /// </summary>
        Texture2DArray = ((int)0x8C1A),
        /// <summary>
        /// Original was GL_PROXY_TEXTURE_2D_ARRAY = 0x8C1B
        /// </summary>
        ProxyTexture2DArray = ((int)0x8C1B),
        /// <summary>
        /// Original was GL_TEXTURE_BUFFER = 0x8C2A
        /// </summary>
        TextureBuffer = ((int)0x8C2A),
        /// <summary>
        /// Original was GL_TEXTURE_CUBE_MAP_ARRAY = 0x9009
        /// </summary>
        TextureCubeMapArray = ((int)0x9009),
        /// <summary>
        /// Original was GL_PROXY_TEXTURE_CUBE_MAP_ARRAY = 0x900B
        /// </summary>
        ProxyTextureCubeMapArray = ((int)0x900B),
        /// <summary>
        /// Original was GL_TEXTURE_2D_MULTISAMPLE = 0x9100
        /// </summary>
        Texture2DMultisample = ((int)0x9100),
        /// <summary>
        /// Original was GL_PROXY_TEXTURE_2D_MULTISAMPLE = 0x9101
        /// </summary>
        ProxyTexture2DMultisample = ((int)0x9101),
        /// <summary>
        /// Original was GL_TEXTURE_2D_MULTISAMPLE_ARRAY = 0x9102
        /// </summary>
        Texture2DMultisampleArray = ((int)0x9102),
        /// <summary>
        /// Original was GL_PROXY_TEXTURE_2D_MULTISAMPLE_ARRAY = 0x9103
        /// </summary>
        ProxyTexture2DMultisampleArray = ((int)0x9103),
    }
    public enum TextureUnit : int
    {
        /// <summary>
        /// Original was GL_TEXTURE0 = 0x84C0
        /// </summary>
        Texture0 = ((int)0x84C0),
        /// <summary>
        /// Original was GL_TEXTURE1 = 0x84C1
        /// </summary>
        Texture1 = ((int)0x84C1),
        /// <summary>
        /// Original was GL_TEXTURE2 = 0x84C2
        /// </summary>
        Texture2 = ((int)0x84C2),
        /// <summary>
        /// Original was GL_TEXTURE3 = 0x84C3
        /// </summary>
        Texture3 = ((int)0x84C3),
        /// <summary>
        /// Original was GL_TEXTURE4 = 0x84C4
        /// </summary>
        Texture4 = ((int)0x84C4),
        /// <summary>
        /// Original was GL_TEXTURE5 = 0x84C5
        /// </summary>
        Texture5 = ((int)0x84C5),
        /// <summary>
        /// Original was GL_TEXTURE6 = 0x84C6
        /// </summary>
        Texture6 = ((int)0x84C6),
        /// <summary>
        /// Original was GL_TEXTURE7 = 0x84C7
        /// </summary>
        Texture7 = ((int)0x84C7),
        /// <summary>
        /// Original was GL_TEXTURE8 = 0x84C8
        /// </summary>
        Texture8 = ((int)0x84C8),
        /// <summary>
        /// Original was GL_TEXTURE9 = 0x84C9
        /// </summary>
        Texture9 = ((int)0x84C9),
        /// <summary>
        /// Original was GL_TEXTURE10 = 0x84CA
        /// </summary>
        Texture10 = ((int)0x84CA),
        /// <summary>
        /// Original was GL_TEXTURE11 = 0x84CB
        /// </summary>
        Texture11 = ((int)0x84CB),
        /// <summary>
        /// Original was GL_TEXTURE12 = 0x84CC
        /// </summary>
        Texture12 = ((int)0x84CC),
        /// <summary>
        /// Original was GL_TEXTURE13 = 0x84CD
        /// </summary>
        Texture13 = ((int)0x84CD),
        /// <summary>
        /// Original was GL_TEXTURE14 = 0x84CE
        /// </summary>
        Texture14 = ((int)0x84CE),
        /// <summary>
        /// Original was GL_TEXTURE15 = 0x84CF
        /// </summary>
        Texture15 = ((int)0x84CF),
        /// <summary>
        /// Original was GL_TEXTURE16 = 0x84D0
        /// </summary>
        Texture16 = ((int)0x84D0),
        /// <summary>
        /// Original was GL_TEXTURE17 = 0x84D1
        /// </summary>
        Texture17 = ((int)0x84D1),
        /// <summary>
        /// Original was GL_TEXTURE18 = 0x84D2
        /// </summary>
        Texture18 = ((int)0x84D2),
        /// <summary>
        /// Original was GL_TEXTURE19 = 0x84D3
        /// </summary>
        Texture19 = ((int)0x84D3),
        /// <summary>
        /// Original was GL_TEXTURE20 = 0x84D4
        /// </summary>
        Texture20 = ((int)0x84D4),
        /// <summary>
        /// Original was GL_TEXTURE21 = 0x84D5
        /// </summary>
        Texture21 = ((int)0x84D5),
        /// <summary>
        /// Original was GL_TEXTURE22 = 0x84D6
        /// </summary>
        Texture22 = ((int)0x84D6),
        /// <summary>
        /// Original was GL_TEXTURE23 = 0x84D7
        /// </summary>
        Texture23 = ((int)0x84D7),
        /// <summary>
        /// Original was GL_TEXTURE24 = 0x84D8
        /// </summary>
        Texture24 = ((int)0x84D8),
        /// <summary>
        /// Original was GL_TEXTURE25 = 0x84D9
        /// </summary>
        Texture25 = ((int)0x84D9),
        /// <summary>
        /// Original was GL_TEXTURE26 = 0x84DA
        /// </summary>
        Texture26 = ((int)0x84DA),
        /// <summary>
        /// Original was GL_TEXTURE27 = 0x84DB
        /// </summary>
        Texture27 = ((int)0x84DB),
        /// <summary>
        /// Original was GL_TEXTURE28 = 0x84DC
        /// </summary>
        Texture28 = ((int)0x84DC),
        /// <summary>
        /// Original was GL_TEXTURE29 = 0x84DD
        /// </summary>
        Texture29 = ((int)0x84DD),
        /// <summary>
        /// Original was GL_TEXTURE30 = 0x84DE
        /// </summary>
        Texture30 = ((int)0x84DE),
        /// <summary>
        /// Original was GL_TEXTURE31 = 0x84DF
        /// </summary>
        Texture31 = ((int)0x84DF),
    }
    public enum TextureWrapMode : int
    {
        /// <summary>
        /// Original was GL_CLAMP = 0x2900
        /// </summary>
        Clamp = ((int)0x2900),
        /// <summary>
        /// Original was GL_REPEAT = 0x2901
        /// </summary>
        Repeat = ((int)0x2901),
        /// <summary>
        /// Original was GL_CLAMP_TO_BORDER = 0x812D
        /// </summary>
        ClampToBorder = ((int)0x812D),
        /// <summary>
        /// Original was GL_CLAMP_TO_BORDER_ARB = 0x812D
        /// </summary>
        ClampToBorderArb = ((int)0x812D),
        /// <summary>
        /// Original was GL_CLAMP_TO_BORDER_NV = 0x812D
        /// </summary>
        ClampToBorderNv = ((int)0x812D),
        /// <summary>
        /// Original was GL_CLAMP_TO_BORDER_SGIS = 0x812D
        /// </summary>
        ClampToBorderSgis = ((int)0x812D),
        /// <summary>
        /// Original was GL_CLAMP_TO_EDGE = 0x812F
        /// </summary>
        ClampToEdge = ((int)0x812F),
        /// <summary>
        /// Original was GL_CLAMP_TO_EDGE_SGIS = 0x812F
        /// </summary>
        ClampToEdgeSgis = ((int)0x812F),
        /// <summary>
        /// Original was GL_MIRRORED_REPEAT = 0x8370
        /// </summary>
        MirroredRepeat = ((int)0x8370),
    }
#endregion



#region Custom

    public enum DeviceVendor
    {
        UNDEFINED,
        INTEL,
        ATI,
        NVIDIA
    }
#endregion
}
