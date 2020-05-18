using NthDimension.Algebra;
using System.Linq;
using System;

namespace RoadGen
{
    public abstract class Collider
    {
        protected int colliderRevision;
        private int aabbRevision;
        private AABB cachedAABB;
        protected object reference;

        protected Collider(object reference)
        {
            colliderRevision = 0;
            aabbRevision = -1;
            cachedAABB = null;
            this.reference = reference;
        }

        protected abstract AABB CreateAABB();

        public AABB GetAABB()
        {
            if (aabbRevision != colliderRevision)
            {
                aabbRevision = colliderRevision;
                cachedAABB = CreateAABB();
            }
            return cachedAABB;
        }

        public abstract bool Collide(Collider other, out Vector2 offset);

    }

    public class RectangleCollider : Collider
    {
        private Vector2[] corners;

        public RectangleCollider(object reference, Vector2[] corners) : base(reference)
        {
            this.corners = corners;
        }

        public Vector2[] Corners
        {
            get
            {
                return corners;
            }
            set
            {
                corners = value;
                colliderRevision++;
            }
        }

        protected override AABB CreateAABB()
        {
            float minX = corners.Min((c) =>
            {
                return c.X;
            });
            float minY = corners.Min((c) =>
            {
                return c.Y;
            });
            return new AABB(
                minX,
                minY,
                corners.Max((c) =>
                {
                    return c.X - minX;
                }),
                corners.Max((c) =>
                {
                    return c.Y - minY;
                }),
                reference
            );
        }

        public override bool Collide(Collider other, out Vector2 offset)
        {
            offset = default(Vector2);

            if (other == null)
                return false;

            if (other is RectangleCollider)
                return Collision.RectangleRectangleIntersection(corners, ((RectangleCollider)other).corners, out offset);
            else if (other is LineCollider)
            {
                var lineCollider = (LineCollider)other;
                return Collision.RectangleRectangleIntersection(corners, Collision.GetCorners(lineCollider.Start, lineCollider.End, lineCollider.Width), out offset);
            }
            else if (other is CircleCollider)
            {
                var circleCollider = (CircleCollider)other;
                return Collision.RectangleCircleIntersection(corners, circleCollider.Center, circleCollider.Radius);
            }
            else
                throw new NotImplementedException();
        }

    }

    public class LineCollider : Collider
    {
        private Vector2 start;
        private Vector2 end;
        private float width;

        public LineCollider(object reference, Vector2 start, Vector2 end, float width) : base(reference)
        {
            this.start = start;
            this.end = end;
            this.width = width;
        }

        public Vector2 Start
        {
            get
            {
                return start;
            }
            set
            {
                start = value;
                colliderRevision++;
            }
        }

        public Vector2 End
        {
            get
            {
                return end;
            }
            set
            {
                end = value;
                colliderRevision++;
            }
        }

        public float Width
        {
            get
            {
                return width;
            }
            set
            {
                width = value;
                colliderRevision++;
            }
        }

        protected override AABB CreateAABB()
        {
            return new AABB(
                Math.Min(start.X, end.X),
                Math.Min(start.Y, end.Y),
                Math.Abs(start.X - end.X),
                Math.Abs(start.Y - end.Y),
                reference
            );
        }

        public override bool Collide(Collider other, out Vector2 offset)
        {
            offset = default(Vector2);

            if (other == null)
                return false;

            if (other is RectangleCollider)
                return Collision.RectangleRectangleIntersection(Collision.GetCorners(start, end, width), ((RectangleCollider)other).Corners, out offset);
            else if (other is LineCollider)
            {
                var lineCollider = (LineCollider)other;
                return Collision.RectangleRectangleIntersection(Collision.GetCorners(start, end, width), Collision.GetCorners(lineCollider.start, lineCollider.end, lineCollider.width), out offset);
            }
            else
                throw new NotImplementedException();
        }

    }

    public class CircleCollider : Collider
    {
        private Vector2 center;
        private float radius;

        public CircleCollider(object reference, Vector2 center, float radius) : base(reference)
        {
            this.center = center;
            this.radius = radius;
        }

        public Vector2 Center
        {
            get
            {
                return center;
            }
            set
            {
                center = value;
                colliderRevision++;
            }
        }

        public float Radius
        {
            get
            {
                return radius;
            }
            set
            {
                radius = value;
                colliderRevision++;
            }
        }

        protected override AABB CreateAABB()
        {
            return new AABB(
                center.X - radius,
                center.Y - radius,
                radius * 2,
                radius * 2,
                reference
            );
        }

        public override bool Collide(Collider other, out Vector2 offset)
        {
            offset = default(Vector2);

            if (other == null)
                return false;

            if (other is RectangleCollider)
                return Collision.RectangleCircleIntersection(((RectangleCollider)other).Corners, center, radius);
            else
                throw new NotImplementedException();
        }

    }

}
