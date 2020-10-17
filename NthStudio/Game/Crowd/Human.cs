// CrowdSimulator - Human.cs
// 
// Copyright (c) 2012, Dominik Gander
// Copyright (c) 2012, Pascal Minder
// 
//  Permission to use, copy, modify, and distribute this software for any
//  purpose without fee is hereby granted, provided that the above
//  copyright notice and this permission notice appear in all copies.
// 
// THE SOFTWARE IS PROVIDED "AS IS" AND THE AUTHOR DISCLAIMS ALL WARRANTIES
// WITH REGARD TO THIS SOFTWARE INCLUDING ALL IMPLIED WARRANTIES OF
// MERCHANTABILITY AND FITNESS. IN NO EVENT SHALL THE AUTHOR BE LIABLE FOR
// ANY SPECIAL, DIRECT, INDIRECT, OR CONSEQUENTIAL DAMAGES OR ANY DAMAGES
// WATSOEVER RESULTING FROM LOSS OF USE, DATA OR PROFITS, WHETHER IN AN
// ACTION OF CONTRACT, NEGLIGENCE OR OTHER TORTIOUS ACTION, ARISING OUT OF
// OR IN CONNECTION WITH THE USE OR PERFORMANCE OF THIS SOFTWARE.

using System;
using System.Collections.Generic;
using System.Linq;

using NthStudio.Game.Crowd.Behaviour;
using NthDimension.Algebra;
using NthDimension.Rendering.Drawables.Models;
using NthDimension.Rendering.Scenegraph;
using NthDimension.Network;
using NthDimension.Rendering;
using NthDimension.Rasterizer.NanoVG;
using System.Drawing;
using static NthStudio.Gui.Displays.NanoGContext;

namespace NthStudio.Game.Crowd
{
    public enum HumanType
    { 
        Normal,
        Agent,
        Victim,
        Dead
    }

    public class Human
    {
        AvatarInfoDesc avatarInfo;
        PlayerRemoteModel model;

        public Human(HumanType HumanType, Vector2 Position, Vector2 Node)
        {
            player = StudioWindow.Instance.Player; // Used only for unproject (see draw function below)
            this.HumanType = HumanType;
            this.Position = Position;
            this.Node = Node;
            this.MovementBehaviour = new UsualMovementBehaviour();
            this.DecisionBehaviour = new UsualDecisionBehaviour();

            avatarInfo = Game.AvatarFactory.GenerateRandom();

            NthDimension.Rendering.Utilities.ConsoleUtil.log(string.Format("{0} - {1} {2}\n\t{3}\n\t{4}\n\t{5}\n\t{6}", avatarInfo.AvatarName,
                                                                               avatarInfo.AvatarSex,
                                                                               avatarInfo.BodyType,
                                                                               avatarInfo.Attachments[0].Material,
                                                                               avatarInfo.Attachments[1].Material,
                                                                               avatarInfo.Attachments[2].Material,
                                                                               avatarInfo.Attachments[3].Material,
                                                                               avatarInfo.Attachments[4].Material));

            StudioWindow.Instance.Scene.RemotePlayersPending.Add(
                new PendingAvatar(StudioWindow.Instance.Scene.RemotePlayersPending.Count,
                                  ((StudioWindow)StudioWindow.Instance).ActiveSceneFile.SceneName, 
                                  avatarInfo));            
        }

        public Vector2              Position { get; set; }

        public Vector2              Node { get; set; }

        public Vector2              FieldIndex { get; set; }

        public Vector2              Incident { get; set; }

        public Human                Victim { get; set; }

        public HumanType            HumanType { get; set; }

        public IMovementBehaviour   MovementBehaviour { get; set; }

        public IDecisionBehaviour   DecisionBehaviour { get; set; }

        private Vector3 prevPos;

        public void Update(List<Human> NearestNeighbour)
        {
            try
            {
                if (null == prevPos)
                {
                    prevPos = new Vector3(Position.X,
                                          StudioWindow.Instance.Scene.GetHeightAtNavMesh(Position.X, Position.Y) + 10, 
                                          Position.Y);
                }

                var humanInSight = NearestNeighbour.Where(OnPredicate).ToList();
                this.DecisionBehaviour.CheckSurrounding(this, humanInSight);
                this.Position = this.MovementBehaviour.Move(this, NearestNeighbour);

                float navMeshY = StudioWindow.Instance.Scene.GetHeightAtNavMesh(Position.X, Position.Y);

                if (null == model)
                    model = StudioWindow.Instance.Scene.RemotePlayers.Where(x => x.UserId == avatarInfo.UserId).FirstOrDefault();
                if (null != model)
                {
                    Vector3 position = prevPos;
                    Vector3 go = new Vector3(Position.X, navMeshY, Position.Y);
                    float dot = Vector3.Dot(position, go);
                    float l0 = Math.Abs(position.LengthFast);
                    float l1 = Math.Abs(go.LengthFast);
                    float res = dot / (l0 * l1);

                    double theta = -Math.Acos(res);

                    Matrix4 orientation = Matrix4.CreateFromAxisAngle(new Vector3(0, 1, 0), ((float)theta * (float)(180d / Math.PI) - (float)(180d / Math.PI)));

                    model.UpdateTransformation(position, go, orientation);

                    if (go != position)
                        this.model.SetAnimationWalk();
                    else
                        this.model.SetAnimationIdle();

                    prevPos = new Vector3(Position.X, navMeshY, Position.Y);
                }

                // Update Colors
                switch(this.HumanType)
                {
                    case HumanType.Agent:
                        this.model.Color = new Vector4(0.55f, 0.8f, 0.9f, 1f);
                        break;
                    case HumanType.Dead:
                        this.model.Color = new Vector4(0.95f, 0.2f, 0.2f, 1f);
                        break;
                    case HumanType.Normal:
                        if (this.MovementBehaviour is UsualMovementBehaviour)
                            this.model.Color = new Vector4(0.98f, 0.98f, 0.98f, 1f);
                        break;
                    default:
                        break;
                }
            }
            catch { }
        }

        private bool OnPredicate(Human Human)
        {
            return Vector2.Dot((this.Node - this.Position), (Human.Position - this.Position)) >= 0.02f && (Human.Position != this.Position);
        }

        public Vector2 RequestNewRandomPosition()
        {
            return CrowdSimulator.GetRandomPosition();
        }

        public void Kill()
        {
            this.HumanType = HumanType.Dead;
            this.MovementBehaviour=new DeadMovementBehaviour();
            //this.model.dissolve();
        }


        NVGcolor vrColorMale = Color.FromArgb(128, 0, 115, 255).ToNVGColor();
        NVGcolor vrColorFemale = Color.FromArgb(128, 240, 0, 159).ToNVGColor();
        NVGcolor vrColorDefault = Color.FromArgb(128, 16, 16, 16).ToNVGColor();
        NVGcolor vrColorUnselected = Color.FromArgb(64, 32, 32, 32).ToNVGColor();
        static ApplicationUser player;
        public void DrawCrowdInfo2D()
        {
            if (null != model)
            {
                float width = StudioWindow.Instance.Width;
                float height = StudioWindow.Instance.Height;

                Vector3 topVertex = model.GetTopVertex();

                Matrix4 scl = Matrix4.CreateScale(model.Size);
                scl.TransformVector(ref topVertex);

                Vector3 avatarPos = Vector3.Zero;
                Vector3 avatarTopVertex = model.Position + topVertex;
                Vector3 avatarPoint = new Vector3(avatarPos.X + avatarTopVertex.X,
                                                  avatarPos.Y + avatarTopVertex.Y,
                                                  avatarPos.Z + avatarTopVertex.Z);

                Vector2 infoPos = ((StudioWindow)StudioWindow.Instance).UnProject(avatarPoint, player.ViewInfo.modelviewMatrix, player.ViewInfo.projectionMatrix, (int)width, (int)height);
                infoPos.Y += 15;

                NanoVG.nvgSave(StudioWindow.vg);

                #region NanoVg draw
                NanoVG.nvgScissor(StudioWindow.vg, 0, 45, width, height);

                string avatarName = avatarInfo.AvatarName;
                var bounds = new float[4];

                NanoVG.nvgFontSize(StudioWindow.vg, 20);
                NanoVG.nvgFontFace(StudioWindow.vg, /*"default-regular"*/ NthDimension.Forms.NanoFont.DefaultRegular.InternalName);
                NanoVG.nvgTextBounds(StudioWindow.vg, 0, 0, avatarName, bounds);

                var s = new Size((int)(bounds[2] - bounds[0]), (int)(bounds[3] - bounds[1]));

                NanoVG.nvgBeginPath(StudioWindow.vg);
                NanoVG.nvgRoundedRect(StudioWindow.vg, infoPos.X - s.Width / 2 - 5, height - infoPos.Y - s.Height / 2 - 3, s.Width + 10, s.Height + 5, 5f);
                NanoVG.nvgClosePath(StudioWindow.vg);

                NVGcolor print = vrColorUnselected;//vrColorDefault;

                //if (info.Gender.ToLower() == "male") print = vrColorMale;
                //if (info.Gender.ToLower() == "female") print = vrColorFemale;

                NanoVG.nvgFillColor(StudioWindow.vg, print);
                NanoVG.nvgFill(StudioWindow.vg);

                switch(HumanType)
                {
                    case HumanType.Victim:
                        NanoVG.nvgFillColor(StudioWindow.vg, NanoVG.nvgRGBA(255, 128, 128, 255));
                        break;
                    case HumanType.Dead:
                        NanoVG.nvgFillColor(StudioWindow.vg, NanoVG.nvgRGBA(255, 0, 0, 255));
                        break;
                    case HumanType.Agent:
                        NanoVG.nvgFillColor(StudioWindow.vg, NanoVG.nvgRGBA(72, 72, 255, 255));
                        break;
                    case HumanType.Normal:
                        if (MovementBehaviour is UsualMovementBehaviour)
                            goto default;
                        if(MovementBehaviour is EvadeMovementBehaviour)
                            NanoVG.nvgFillColor(StudioWindow.vg, NanoVG.nvgRGBA(239, 138, 37, 255));
                        break;
                    default:
                        NanoVG.nvgFillColor(StudioWindow.vg, NanoVG.nvgRGBA(255, 255, 255, 255));
                        break;
                }
                
                NanoVG.nvgTextAlign(StudioWindow.vg, (int)(EAlign.ALIGN_LEFT | EAlign.ALIGN_MIDDLE));
                NanoVG.nvgText(StudioWindow.vg, infoPos.X - s.Width / 2, height - infoPos.Y, avatarName);
                #endregion

                NanoVG.nvgRestore(StudioWindow.vg);
            }
        }
    }
}
