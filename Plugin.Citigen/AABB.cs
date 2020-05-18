namespace RoadGen
{
    public class AABB
    {
        public float X;
        public float Y;
        public float width;
        public float height;
        public object reference;

        public AABB(float x, float y, float width, float height, object reference)
        {
            this.X = x; this.Y = y; this.width = width; this.height = height; this.reference = reference;
        }

    }
}
