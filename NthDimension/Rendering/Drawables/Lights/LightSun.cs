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

using NthDimension.Rendering.GameViews;

namespace NthDimension.Rendering.Drawables.Lights
{
    using NthDimension.Algebra;
    using Rendering.Configuration;
    using Rendering.Drawables.Models;
    using Rendering.Scenegraph;
    using Rendering.Shaders;
    using System.Linq;
    using System;
    using System.Xml;
    using NthDimension.Utilities;

    //using OpenGL.GLSL.API3x;

#if _WINDOWS_
    //using NthDimension.OpenGL.Windows.GLSL.API3x;
#endif

    public sealed class LightDirectional : Light
    {
        new public static string    nodename            = "sunlight";
        private readonly string     _defaultMaterial    = "defShading\\lightSun.xmf";

        private float               m_long3ShadowDist       = 480;      // Added May-20-19
        private float               m_long2ShadowDist       = 120;      // Added May-20-19
        private float               m_longShadowDist        = 60;       // original 120f;
        private float               m_shortShadowDist       = 20f; //10f;      // original 30 -> large area of ground z fragments black

        public float                LongRange           { get { return m_longShadowDist; } set { m_longShadowDist = value; } }
        public float                ShortRange          { get { return m_shortShadowDist; } set { m_shortShadowDist = value; } } 

        public Vector3              lightAmbient;
        public ViewInfoSun          shortViewInfo;
        public Matrix4              nearShadowMatrix;
        private float               nextFarUpdate;
        public Vector3              lookAt              = Vector3.Zero;

        public Matrix4              shadowView;
        public Matrix4              shadowProj;
        public int                  splitNo;

        private bool                m_assetsInitialized = false;

        

        public LightDirectional(ApplicationObject obj):this(obj as SceneGame)
        {
            // Ugly hack
        }
        public LightDirectional(SceneGame scene)
        {
            Parent          = scene;
            this.Name       = "sunlight";
            this.buildMatrix();

            if(!Scene.DirectionalLights.Contains(this))
                Scene.DirectionalLights.Add(this);
            //Do NOT do here: Scene.AddDrawable(this); 
        }
        ~LightDirectional()
        {
            if (Scene.DirectionalLights.Contains(this))
                Scene.DirectionalLights.Remove(this);

        }

        public LightDirectional(SceneGame scene, Vector3 ambientColor): this(scene)
        {
            lightAmbient            = ambientColor;
            //Color                   = new Vector4(1f, 1f, 1f, 1f); // White, saturation full
            Color                   = new Vector4(ambientColor, 1f); // Original
        }

        private void buildMatrix()
        {
            viewInfo        = new ViewInfoSun(this, LongRange,   LongRange);
            //viewInfo.projectionMatrix = Matrix4.CreateOrthographic(LongRange, LongRange, -LongRange * 1.5f, LongRange * 0.5f);
            shortViewInfo   = new ViewInfoSun(this, ShortRange, LongRange);
            //innerViewInfo.projectionMatrix = Matrix4.CreateOrthographic(ShortRange, ShortRange, -LongRange * 1.5f, LongRange * 0.5f);
            shadowQuality   = Settings.Instance.video.shadowQuality;
            nextFarUpdate   = ApplicationBase.Instance.VAR_FrameTime;
        }

        private void createRenderObject()
        {
            m_assetsInitialized     = true;
            drawable                = new LightVolume(this);
            drawable.setMaterial(   _defaultMaterial);
            drawable.setMesh(       "base\\sprite_plane.obj");
            drawable.IsVisible      = true;
            
        }

        public override void save(ref System.Text.StringBuilder sb, int level)
        {
            // reading Object Atrributes and Converting them to Strings
            string min = GenericMethods.StringFromFloat(m_shortShadowDist);
            string max = GenericMethods.StringFromFloat(m_longShadowDist);
            string color = GenericMethods.StringFromVector4(Color);
            string ambient = GenericMethods.StringFromVector3(lightAmbient);
            string direction = GenericMethods.StringFromVector3(PointingDirection);

            string tab = GenericMethods.tabify(level - 1);
            string tab2 = GenericMethods.tabify(level);

            sb.AppendLine(tab + "<" + nodename + " name='" + Name + "' enabled='" + (Enabled ? "true" : "false") + "' >");
            sb.AppendLine(tab2 + "<min>" + min + "</min>");
            sb.AppendLine(tab2 + "<max>" + max + "</max>");
            sb.AppendLine(tab2 + "<color>" + color + "</color>");
            sb.AppendLine(tab2 + "<ambient>" + ambient + "</ambient>");
            sb.AppendLine(tab2 + "<direction>" + direction + "</direction>");

            if (IgnoreCulling)
                sb.AppendLine(tab2 + "<cullignore/>");

            /*
            // Creating Sql Command
            sb.Append("INSERT INTO WorldObjects (id, name, position, rotation , material, meshes, pboxes, static )" +
                " VALUES(NULL, '" + name + "', '" + position + "', '" + rotation + "' , '" + stringMaterial + "' , '"
                + meshes + "' , '" + pboxes + "' , " + isstatic + ");");

             */

            //Utilities.ConsoleUtil.log(string.Format("@ Saving Light Sun: '{0}'", Name));

            saveChilds(ref sb, level);

            sb.AppendLine(tab + "</" + nodename + ">");
        }
        protected override void specialLoad(ref XmlTextReader reader, string type)
        {
            if (reader.Name.ToLower() == "max" && reader.NodeType != XmlNodeType.EndElement)
            {
                reader.Read();
                float fval = GenericMethods.FloatFromString(reader.Value);
                m_longShadowDist = fval;
                this.buildMatrix();
            }

            if (reader.Name.ToLower() == "min" && reader.NodeType != XmlNodeType.EndElement)
            {
                reader.Read();
                float fval = GenericMethods.FloatFromString(reader.Value);
                m_shortShadowDist = fval;
                this.buildMatrix();
            }

            if (reader.Name.ToLower() == "color" && reader.NodeType != XmlNodeType.EndElement)
            {
                reader.Read();
                Vector4 color = GenericMethods.Vector4FromString(reader.Value);
                this.Color = color;
            }

            if (reader.Name.ToLower() == "ambient" && reader.NodeType != XmlNodeType.EndElement)
            {
                reader.Read();
                Vector3 ambient = GenericMethods.Vector3FromString(reader.Value);
                this.lightAmbient = ambient;
            }

            if (reader.Name.ToLower() == "position" && reader.NodeType != XmlNodeType.EndElement)
                position = GenericMethods.Vector3FromString(reader.Value);

            if (reader.Name.ToLower() == "direction" && reader.NodeType != XmlNodeType.EndElement)
                pointingDirection = GenericMethods.Vector3FromString(reader.Value);
        }
       
        public override void Update()
        {
            if (!m_assetsInitialized)
                this.createRenderObject();

            Position = Parent.Position;   // YES IN ORIGINAL

            if (ApplicationBase.Instance.VAR_FrameTime > nextFarUpdate)
            {
                updateChilds();

                #region Original
                nearShadowMatrix       = Matrix4.Mult(shortViewInfo.modelviewProjectionMatrix, shadowBias);
                shadowMatrix            = Matrix4.Mult(viewInfo.modelviewProjectionMatrix, shadowBias);

                #endregion

                #region 3rd Person test 
                //Matrix4 model = Matrix4.Mult(viewInfo.modelviewMatrix, ApplicationBase.Instance.Player.ViewInfo.modelviewMatrix);
                //Matrix4 modelInner = Matrix4.Mult(innerViewInfo.modelviewMatrix, ApplicationBase.Instance.Player.ViewInfo.modelviewMatrix);
                //Matrix4 invModel = Matrix4.Mult(viewInfo.modelviewMatrix, ApplicationBase.Instance.Player.ViewInfo.invModelviewMatrix);
                //Matrix4 invModelInner = Matrix4.Mult(innerViewInfo.modelviewMatrix, ApplicationBase.Instance.Player.ViewInfo.invModelviewMatrix);

                //Matrix4 modelProj = Matrix4.Mult(viewInfo.modelviewMatrix, viewInfo.projectionMatrix);
                //Matrix4 modelProjInner = Matrix4.Mult(innerViewInfo.modelviewMatrix, viewInfo.projectionMatrix);

                //innerShadowMatrix = Matrix4.Mult(modelProjInner, shadowBias);
                //shadowMatrix = Matrix4.Mult(modelProj, shadowBias);
                #endregion 3rd person test


                viewInfo.wasUpdated         = true;
                nextFarUpdate               = ApplicationBase.Instance.VAR_FrameTime + Settings.Instance.video.shadowUpdateInterval;
            }
        }

        public override void activate(Shaders.Shader shader, Drawable drawble)
        {
            ApplicationBase.Instance.Renderer.Uniform3(shader.SunDirection,                    ref pointingDirection);
            ApplicationBase.Instance.Renderer.Uniform3(shader.SunColor,                        ref colorRgb);
            shader.InsertUniform(Uniform.in_lightambient,                                      ref lightAmbient);
            ApplicationBase.Instance.Renderer.UniformMatrix4(shader.SunMatrix,         false,  ref shadowMatrix);
            ApplicationBase.Instance.Renderer.UniformMatrix4(shader.SunInnerMatrix,    false,  ref nearShadowMatrix);
        }

        public override void activateDeferred(Shaders.Shader shader)
        {
            shader.InsertUniform(Uniform.defDirection,      ref pointingDirection);
            shader.InsertUniform(Uniform.defColor,          ref colorRgb);
            shader.InsertUniform(Uniform.in_lightambient,   ref lightAmbient);
            shader.InsertUniform(Uniform.defMatrix,         ref shadowMatrix);
            shader.InsertUniform(Uniform.defInnerMatrix,    ref nearShadowMatrix);
            shader.InsertUniform(Uniform.shadowQuality,     ref shadowQuality);
        }

        public ViewInfo ViewInfo
        {
            get { return this.viewInfo; }
        }


#if KAILASHTEST
#region Kailash Cascaded Shadows
        private int _num_cascades = 4;
        private float[] _cascade_splits;
        private Matrix4[] _shadow_view_matrices;
        public Matrix4[] shadow_view_matrices
        {
            get { return _shadow_view_matrices; }
        }

        private Matrix4[] _shadow_ortho_matrices;
        public Matrix4[] shadow_ortho_matrices
        {
            get { return _shadow_ortho_matrices; }
        }

        private Matrix4[] _cascade_perspective_matrices;
       
        public void update_Cascades(SpatialData camera_spatial, Vector3 light_direction)
        {
            Matrix4[] temp_view_matrices = new Matrix4[_num_cascades];
            Matrix4[] temp_ortho_matrices = new Matrix4[_num_cascades];

            float cascade_backup_distance = 20.0f;
            float shadow_texture_width = 840.0f;

            for (int cascade = 0; cascade < _num_cascades; cascade++)
            {
                float near = _cascade_splits[cascade];
                float far = _cascade_splits[cascade + 1];

                //------------------------------------------------------
                // Create Frustum Bounds
                //------------------------------------------------------
                float frustum_near = -1.0f;
                float frustum_far = 1.0f;
                Vector3[] frustum_corners =
                {
                    // Near Plane
                    new Vector3(-1.0f,1.0f,frustum_near),
                    new Vector3(1.0f,1.0f,frustum_near),
                    new Vector3(1.0f,-1.0f,frustum_near),
                    new Vector3(-1.0f,-1.0f,frustum_near),
                    // Far Plane
                    new Vector3(-1.0f,1.0f,frustum_far),
                    new Vector3(1.0f,1.0f,frustum_far),
                    new Vector3(1.0f,-1.0f,frustum_far),
                    new Vector3(-1.0f,-1.0f,frustum_far),
                };

                frustum_corners = frustum_corners.Select(corner =>
                {
                    return Vector3.TransformPerspective(corner, Matrix4.Invert(camera_spatial.model_view * _cascade_perspective_matrices[cascade]));
                }).ToArray();


                //------------------------------------------------------
                // Get Frustum center and radius
                //------------------------------------------------------
                Vector3 frustum_center = Vector3.Zero;
                for (int i = 0; i < 8; i++)
                {
                    frustum_center = frustum_center + frustum_corners[i];
                }
                frustum_center = frustum_center / 8.0f;

                float radius_max = 0.0f;
                for (int i = 0; i < 8; i++)
                {
                    radius_max = (float)Math.Max((frustum_center - frustum_corners[i]).Length, radius_max);
                }
                radius_max *= 2.0f;
                float radius = radius_max;

                //------------------------------------------------------
                // Trying to fix shimmering
                //------------------------------------------------------
                radius = (float)Math.Floor(radius * shadow_texture_width) / shadow_texture_width;

                float scaler = shadow_texture_width / (radius * (shadow_texture_width / 5.0f));
                frustum_center *= scaler;
                frustum_center.X = (float)Math.Floor(frustum_center.X);
                frustum_center.Y = (float)Math.Floor(frustum_center.Y);
                frustum_center.Z = (float)Math.Floor(frustum_center.Z);
                frustum_center /= scaler;


                //------------------------------------------------------
                // Create Matrices
                //------------------------------------------------------

                Vector3 eye = frustum_center + (light_direction * (radius / 2.0f * cascade_backup_distance));
                temp_view_matrices[cascade] = Matrix4.LookAt(eye, frustum_center, Vector3.UnitY);
                temp_ortho_matrices[cascade] = Matrix4.CreateOrthographic(radius, radius, _cascade_splits[0], radius * cascade_backup_distance);
            }

            _shadow_view_matrices = temp_view_matrices;
            _shadow_ortho_matrices = temp_ortho_matrices;
        }
      
        public class SpatialData
        {

            private Vector3 _position;
            public Vector3 position
            {
                get { return _position; }
                set { _position = value; }
            }

            private Vector3 _look;
            public Vector3 look
            {
                get { return _look; }
                set { _look = value; }
            }

            private Vector3 _up;
            public Vector3 up
            {
                get { return _up; }
                set { _up = value; }
            }

            private Vector3 _strafe;
            public Vector3 strafe
            {
                get { return _strafe; }
                set { _strafe = value; }
            }

            private Vector3 _scale;
            public Vector3 scale
            {
                get { return _scale; }
            }



            private Vector3 _rotation_angles;
            public Vector3 rotation_angles
            {
                get { return _rotation_angles; }
                set { _rotation_angles = value; }
            }


            public Matrix4 position_matrix
            {
                get { return Matrix4.CreateTranslation(_position); }
            }


            private Matrix4 _rotation_matrix;
            public Matrix4 rotation_matrix
            {
                get { return _rotation_matrix; }
                set
                {
                    _rotation_matrix = value;

                    _look = -_rotation_matrix.Row2.Xyz;
                    _up = -_rotation_matrix.Row1.Xyz;
                    _strafe = -_rotation_matrix.Row0.Xyz;
                }
            }


            public Matrix4 scale_matrix
            {
                get { return Matrix4.CreateScale(_scale); }
            }



            public Matrix4 transformation
            {
                get
                {
                    return (position_matrix * rotation_matrix);
                }
                set
                {
                    _position = value.ExtractTranslation();
                    _scale = value.ExtractScale();
                    rotation_matrix = Matrix4.Identity;  //Matrix4.CreateFromQuaternion(value.ExtractRotation());
                }
            }


            public Matrix4 model_view
            {
                get
                {
                    return transformation;
                }
            }


            private Matrix4 _perspective;
            public Matrix4 perspective
            {
                get { return _perspective; }
            }



            public SpatialData()
                : this(new Vector3(), new Vector3(), new Vector3())
            { }

            public SpatialData(Vector3 position, Vector3 look, Vector3 up)
            {
                _position = position;
                _look = look;
                _up = up;
                _scale = new Vector3(1.0f);

                rotation_matrix = Matrix4.CreateFromQuaternion( Matrix4.LookAt(position, _look, _up).ExtractRotation());
            }

            public SpatialData(Matrix4 transformation)
            {
                this.transformation = transformation;
            }


            public void setPerspective(float fov_degrees, float aspect, Vector2 near_far)
            {
                _perspective = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(fov_degrees), aspect, near_far.X, near_far.Y);
            }

            public void setPerspective(float fov_degrees, float aspect, float near, float far)
            {
                _perspective = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(fov_degrees), aspect, near, far);
            }

            public void setOrthographic(float left, float right, float bottom, float top, float near, float far)
            {
                _perspective = Matrix4.CreateOrthographicOffCenter(left, right, bottom, top, near, far);
            }

            public void setOrthographic(float width, float height, float near, float far)
            {
                _perspective = Matrix4.CreateOrthographic(width, height, near, far);
            }
        }

#endregion
#endif


    }
}
