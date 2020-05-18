//using System;

//namespace NthDimension.Rendering.Drawables.Models
//{
//    using NthDimension.Algebra;
//    using NthDimension.Rendering.Geometry;
//    using NthDimension.Rendering.Scenegraph;

//    public partial class PlayerModel : IClickable 
//    {
//        public class PlayerModelSelection : ModelSelection
//        {
//            public PlayerModel PlayerModel;

//            public PlayerModelSelection(PlayerModel playerModel)
//                : base(playerModel as PlayerModel)
//            {
//                this.PlayerModel = playerModel;
//            }
//        }

//        private bool m_isHovering = false;

//        //public delegate void UnhoveredEvent(PlayerModelSelection e);
//        //public delegate void HoveredEvent(PlayerModelSelection e);
//        ////public delegate void        SelectedEvent();
//        ////public event                SelectedEvent           OnSelected;
//        public event                HoveredEvent            OnSelected;
//        public event                UnhoveredEvent          OnUnselected;
//        public Culling.BoundingAABB                         Bounds { get; set; }
//        public Vector2[]                                    ScreenBounds { get; set; }
       
//        public void                                         Hover()
//        {
//            if (this.m_isHovering) return;

//            this.Selected = 1.0f;
//            this.m_isHovering = true;
//        }

//        public void                                         UnHover()
//        {
//            if (!this.m_isHovering) return;

//            this.Selected = 0.0f;

            

//            this.m_isHovering = false;
//        }

//        public void                                         Select()
//        {

//            if (null != this.OnSelected && m_isHovering)
//                this.OnSelected(new PlayerModelSelection(this));
                
//        }

//        public void                             Unselect()
//        {
//            if (null != OnUnselected)
//                this.OnUnselected(new PlayerModelSelection(this));
//        }

//        private void                            updateBounds()
//        {
//            if (Bounds.Min == Vector3.Zero &&
//                Bounds.Max == Vector3.Zero)
//            {
//                Matrix4 bScaleMat = Matrix4.CreateScale(this.Size);
//                Vector3[] positions = this.meshes[0].GetPositions(MeshVbo.MeshLod.Level0);
//                Vector3[] vertices = new Vector3[positions.Length];

//                for (int i = 0; i < positions.Length; i++)
//                {
//                    vertices[i] = new Vector3(positions[i].X, positions[i].Y, positions[i].Z);
//                }

//                for (int i = 0; i < vertices.Length; i++)
//                {
//                    bScaleMat.TransformVector(ref vertices[i]);
//                }
//                this.Bounds = Culling.BoundingAABB.CreateFromPoints(vertices);
//            }
//        }
//    }
//}
