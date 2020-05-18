using NthDimension.Algebra;
using NthDimension.Rendering.Culling;
using NthDimension.Rendering.Drawables.Models;
using NthDimension.Rendering.Geometry;
using NthDimension.Rendering.Scenegraph;
using NthDimension.Rendering.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthStudio
{
    public partial class StudioWindow
    {
        public object PlayerInput { get; private set; }

        public override void OnSelectAvatar(ModelSelection args)
        {
            //UserInfoDesc info = new UserInfoDesc();
            //UserInfos.TryGetValue(((PlayerModel)args.Model).UserId, out info);


            //if (null != info)
            //{
            //    AvatarSelectionInfo avatarSelection = new AvatarSelectionInfo((PlayerModel)args.Model, info);

            //    if (AvatarSelection != avatarSelection)
            //    {
            //        AvatarSelection = avatarSelection;
            //        this.m_screen.AvatarSelect(info);
            //    }
            //}
            //else
            //    Program.gameClient.RequestUserDetails(((PlayerModel)args.Model).UserId, string.Empty);

            ConsoleUtil.log(string.Format("Avatar {0} selected on NthStudio", args.Model.Name));
        }

        public override void OnUnselectAvatar(ModelSelection e)
        {
            base.OnUnselectAvatar(e);
        }
        private static bool canClick(NthDimension.Rendering.Drawables.Drawable drawable)
        {
            if (!(drawable is IClickable)) return false;

            return ((IClickable)drawable).ClickEnabled;
        }
        private void mousePickAvatars()
        {
            bool pickedAvatar = false;
            var list = this.Scene.Drawables.FindAll(canClick);
            Parallel.ForEach(list, new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount }, (drawable) =>
            {
                try
                {
                    if (//drawable is IClickable &&
                        drawable is AnimatedModel)
                    {
                        

                        Vector3 modelPosition = ((Model)drawable).Position;



                        var model = (AnimatedModel)drawable;

                        
                            Vector3[] corners = model.Bounds.GetCorners();
                            Vector3[] cornersCopy = new Vector3[corners.Length];

                            for (int i = 0; i < corners.Length; i++)
                            {
                                cornersCopy[i] = new Vector3(corners[i].X, corners[i].Y, corners[i].Z) + /*model.Position*/ modelPosition;
                            }

                            BoundingAABB pickBox = BoundingAABB.CreateFromPoints(cornersCopy);


                            Ray mousePick = new Ray(Player.ThirdPersonView.MousePickRayStart,
                                Player.ThirdPersonView.MousePickRayEnd);

                            float? pickResult = 0f;


                            pickBox.Intersects(ref mousePick, out pickResult);

                            if (pickResult > 0f)
                            {
                                pickedAvatar = true;
                                if (NthDimension.Forms.Cursors.Cursor != NthDimension.Forms.Cursors.Hand)
                                    NthDimension.Forms.Cursors.Cursor = NthDimension.Forms.Cursors.Hand;
                                model.Hover();
                                

                                if (GameInput.MouseState.LeftButton == OpenTK.Input.ButtonState.Pressed)
                                    OnSelectAvatar(new ModelSelection((Model)model));
                            }
                            else
                            {
                                //if (!((GameInput)PlayerInput).CursorOverAvatarButton)
                                    if (NthDimension.Forms.Cursors.Cursor != NthDimension.Forms.Cursors.Default)
                                        NthDimension.Forms.Cursors.Cursor = NthDimension.Forms.Cursors.Default;
                                model.UnHover();
                            }

                            if (GameInput.MouseState.LeftButton == OpenTK.Input.ButtonState.Pressed)
                            {
                                model.Select();
                            }

                            // Used mostly for aid debbuging visualization
                            model.ScreenBounds = new Vector2[corners.Length];

                            for (int i = 0; i < corners.Length; i++)
                            {
                                Vector3 dpos = modelPosition + corners[i];
                                model.ScreenBounds[i] = this.Convert(dpos,
                                    StudioWindow.Instance.Player.ViewInfo.modelviewMatrix,
                                    StudioWindow.Instance.Player.ViewInfo.projectionMatrix,
                                    Width, Height);

                                model.ScreenBounds[i].Y = this.Height - model.ScreenBounds[i].Y;
                            }
                        
                    }
                }
                catch (Exception pE)
                {
                    ConsoleUtil.errorlog(string.Format("WindowsGame.mousePickAvatars() {0}", pE.StackTrace), pE.Message);
                }
            }
            );
            StudioWindow.Instance.CursorOverAvatar = pickedAvatar;
        }

        private Vector2 Convert(Vector3 pos, Matrix4 viewMatrix, Matrix4 projectionMatrix, int screenWidth, int screenHeight)
        {
            pos = Vector3.Transform(pos, viewMatrix);
            pos = Vector3.Transform(pos, projectionMatrix);
            pos.X /= pos.Z;
            pos.Y /= pos.Z;
            pos.X = (pos.X + 1) * screenWidth / 2;
            pos.Y = (pos.Y + 1) * screenHeight / 2;

            return new Vector2(pos.X, pos.Y);
        }
    }
}
