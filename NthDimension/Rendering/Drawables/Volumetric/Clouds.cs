namespace NthDimension.Rendering.Drawables.Volumetric
{
    using NthDimension.Algebra;
    using NthDimension.Rendering.Drawables.Models;
    using NthDimension.Rendering.Geometry;
    using NthDimension.Rendering.Shaders;


    public class Clouds : Model
    {
        private float                   cloudSpeed                  = 450.0f;
        private float                   coverage                    = .45f;
        private float                   crispines                   = 40.0f;
        private float                   density                     = .02f;
        private float                   absorption                  = .35f;

        private float                   earthRadius                 = 637500.0f;
        private float                   sphereInnerRadius           = 5000.0f;
        private float                   sphereOuterRadius           = 17000.0f;

        private float                   perlinFrequency             = .8f;

        private bool                    enableGodRays               = false;
        private bool                    enablePowder                = false;
        private bool                    postProcess                 = true;

        private Vector3                 seed                        = Vector3.Zero;
        private Vector3                 oldSeed                     = Vector3.Zero;

        private Vector3                 cloudColorTop               = new Vector3(169, 149, 149) * (1.5f/255f);
        private Vector3                 cloudBottomColor            = new Vector3(65,70,80) * (1.5f/255f);

        private int                     watherTex                   = 0;
        private int                     perlintex                   = 0;
        private int                     worley32                    = 0;

        public Clouds(ApplicationObject parent)
            :base(parent)
        {
            this.generateModelTextures();
        }

        private void generateModelTextures()
        {
            this.perlintex = ApplicationBase.Instance.TextureLoader.generateTexture3D(128, 128, 128);
            this.worley32 = ApplicationBase.Instance.TextureLoader.generateTexture3D(128, 128, 128);

            // TODO GLDispatchCompute - GLSL 400
        }

        protected override void setSpecialUniforms(ref Shader curShader, ref MeshVbo CurMesh)
        {
            // TODO
            base.setSpecialUniforms(ref curShader, ref CurMesh);
        }

        public override void Update()
        {
            // TODO
            base.Update();
        }
    }
}
