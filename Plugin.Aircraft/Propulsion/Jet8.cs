using NthDimension.Algebra;
using NthDimension.Rendering.Drawables;
using NthDimension.Rendering.Drawables.Models;
using NthDimension.Physics.Collision.Shapes;

namespace Plugin.Aircraft
{
    public partial class Jet8 : Model
    {
        Model body                  = new Model(); // Drop Use base class as body
        Model Glass                 = new Model();
        Model Flap_ZL               = new Model();
        Model Flap_ZR               = new Model();
        Model Aileron_EL            = new Model();
        Model Aileron_ER            = new Model();
        Model LevelWing_RL          = new Model();
        Model LevelWing_RR          = new Model();

        public Jet8()
        {
            this.AvatarBody = new NthDimension.Physics.Dynamics.RigidBody(new BoxShape(new NthDimension.Physics.LinearMath.JVector(1f)));
        }



    }


    
}
