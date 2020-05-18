using NthDimension.Algebra;
using NthDimension.Rendering.Culling;
using NthDimension.Rendering.Scenegraph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthDimension.Rendering.Drawables.Models
{
    public partial class AnimatedModel
    {
        public class AnimatedModelSelection : ModelSelection
        {
            public AnimatedModel Model;

            public AnimatedModelSelection(AnimatedModel model)
                : base(model as AnimatedModel)
            {
                this.Model = model;
            }
        }

        public Culling.BoundingAABB Bounds { get; set; }
        public Vector2[] ScreenBounds { get; set; }

        private bool m_isHovering = false;
        public event HoveredEvent OnSelected;
        public event UnhoveredEvent OnUnselected;

        #region IClickable Implementation
        public bool ClickEnabled
        {
            get;
            set;
        } = false;
        public void Hover()
        {
            if (this.m_isHovering) return;

            this.Selected = 1.0f;
            this.m_isHovering = true;
            //ConsoleUtil.log(string.Format("{0} Hover", this.Name));
        }

        public void UnHover()
        {
            if (!this.m_isHovering) return;

            this.Selected = 0.0f;
            this.m_isHovering = false;
            //ConsoleUtil.log(string.Format("{0} Unhover", this.Name));
        }

        public void Select()
        {
            if (null != this.OnSelected && m_isHovering)
                this.OnSelected(new AnimatedModelSelection(this));
            //ConsoleUtil.log(string.Format("{0} Select", this.Name));
        }

        public void Unselect()
        {
            if (null != OnUnselected)
                this.OnUnselected(new AnimatedModelSelection(this));
            //ConsoleUtil.log(string.Format("{0} Unselect", this.Name));
        }
        #endregion

        internal void updateBounds()
        {
            if (Bounds.Min == Vector3.Zero &&
                Bounds.Max == Vector3.Zero)
            {
                Matrix4 bScaleMat = Matrix4.CreateScale(this.Size);
                Vector3[] positions = this.meshes[0].GetPositions(Geometry.MeshVbo.MeshLod.Level0);
                Vector3[] vertices = new Vector3[positions.Length];

                for (int i = 0; i < positions.Length; i++)
                {
                    vertices[i] = new Vector3(positions[i].X, positions[i].Y, positions[i].Z);
                }

                for (int i = 0; i < vertices.Length; i++)
                {
                    bScaleMat.TransformVector(ref vertices[i]);
                }
                this.Bounds = Culling.BoundingAABB.CreateFromPoints(vertices);
            }
        }


        /// <summary>
        /// TODO:: Experimental. The goal is to execute a user defined call (ie start dialog engine with npc, click to trade, make npc move(), rotate(), etc)
        /// </summary>
        /// <returns></returns>
        public virtual Func<string[], object[]> ExecuteOnSelect()
        {
            return null;
        }

    }
}
