using System.Collections.Generic;

namespace NthDimension.Rendering.Culling
{
    using NthDimension.Algebra;


    public class BoundingOctree<T>
    {
        
        #region class OctreeNode

        private class OctreeNode
        {
            #region Properties

            public BoundingAABB Bounds
            {
                get { return _bounds; }
            }

            public Vector3 Center { get; private set; }
            public float BaseLength { get; private set; }

            #endregion

            #region Fields

            private float _looseness;
            private float _minSize;
            private float _adjLength;

            private BoundingAABB _bounds = default(BoundingAABB);
#pragma warning disable CS0649
            private OctreeNode[] _children;
            private BoundingAABB[] _childBounds;

            private const int NumObjectsAllowed = 8;
            private readonly List<OctreeObject> _objects = new List<OctreeObject>();

            #endregion

            #region class OctreeObject

            private class OctreeObject
            {
#pragma warning disable CS0649
                public T Obj;
                public BoundingAABB Bounds;
            }

            #endregion

            #region Ctor

            public OctreeNode(float baseLengthVal, float minSizeVal, float loosenessVal, Vector3 centerVal)
            {
                this.setValues(baseLengthVal, minSizeVal, loosenessVal, centerVal);
            }

            #endregion

            public bool HasAnyObjects()
            {
                if (_objects.Count > 0) return true;

                if (_children != null)
                {
                    for (int i = 0; i < 8; i++)
                    {
                        if (_children[i].HasAnyObjects()) return true;
                    }
                }

                return false;
            }

            private void setValues(float baseLengthVal, float minSizeVal, float loosenessVal, Vector3 centerVal)
            {
                BaseLength = baseLengthVal;
                _minSize = minSizeVal;
                _looseness = loosenessVal;
                Center = centerVal;
                _adjLength = _looseness * baseLengthVal;

                // Create the bounding box.
                Vector3 size = new Vector3(_adjLength, _adjLength, _adjLength);
                _bounds = new BoundingAABB(Center, size);

                float quarter = BaseLength / 4f;
                float childActualLength = (BaseLength / 2) * _looseness;
                Vector3 childActualSize = new Vector3(childActualLength, childActualLength, childActualLength);
                _childBounds = new BoundingAABB[8];
                _childBounds[0] = new BoundingAABB(Center + new Vector3(-quarter, quarter, -quarter), childActualSize);
                _childBounds[1] = new BoundingAABB(Center + new Vector3(quarter, quarter, -quarter), childActualSize);
                _childBounds[2] = new BoundingAABB(Center + new Vector3(-quarter, quarter, quarter), childActualSize);
                _childBounds[3] = new BoundingAABB(Center + new Vector3(quarter, quarter, quarter), childActualSize);
                _childBounds[4] = new BoundingAABB(Center + new Vector3(-quarter, -quarter, -quarter), childActualSize);
                _childBounds[5] = new BoundingAABB(Center + new Vector3(quarter, -quarter, -quarter), childActualSize);
                _childBounds[6] = new BoundingAABB(Center + new Vector3(-quarter, -quarter, quarter), childActualSize);
                _childBounds[7] = new BoundingAABB(Center + new Vector3(quarter, -quarter, quarter), childActualSize);
            }
        }

        #endregion
    }
}
