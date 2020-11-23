namespace NthDimension.Rendering
{
    using NthDimension.Algebra;

    public partial class ApplicationBase
    {
        public class ApplicationSounds
        {
            public virtual void playBlip(Vector3 source, Vector3 listener) { }
            public virtual void playBlip(float sourceX = 0, float sourceY = 0, float sourceZ = 0, float listenerX = 0, float listenerY = 0, float listenerZ = 0) { }
        }

    }
}
