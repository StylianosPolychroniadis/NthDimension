/* LICENSE
 * Copyright (C) 2008 - 2018 SYSCON Technologies, Hellas - All Rights Reserved
 * Written by Stylianos N. Polychroniadis (info@polytronic.gr) http://www.polytronic.gr
 * 
 * This file is part of nthDimension Platform
 * 
 * WARNING! Commercial Software, All Use Must Be Licensed
 * This software is protected by Hellenic Copyright Law and International Treaties. 
 * Unauthorized use, duplication, reverse engineering, any form of redistribution, or 
 * use in part or in whole other than by prior, express, printed and signed license 
 * for use is subject to civil and criminal prosecution. 
*/


namespace NthDimension.Rendering.Drawables.Models
{
    using NthDimension.Algebra;
    using NthDimension.Network;

    public class PlayerRemoteModel : PlayerModel
    {
        private object                  _lock = new object();
        private object                  _lock0 = new object();
        private const float             _LerpFactor = 0.005f;

        private Vector3                  m_goto;
        private Vector3                  m_receivedPositionGoto        = new Vector3();
        private Vector3                  m_receivedPositionCurrent     = new Vector3();
        private Matrix4                  m_receivedOrientationCurrent  = Matrix4.Identity;

        private volatile bool           m_updateTransformation          = false;

        //public string                   FaceTexture = string.Empty;

        #region Ctor
        public PlayerRemoteModel(ApplicationObject parent, enuAvatarSex sex) 
            : base(parent, sex)
        {
            
        }
        #endregion

        #region Update
        public override void Update()
        {
            if (m_updateTransformation)
            {
                this.Position       = this.m_receivedPositionCurrent;
                this.Orientation    = this.m_receivedOrientationCurrent;
                this.m_goto         = this.m_receivedPositionGoto;

                this.m_updateTransformation = false;
            }

            this.Position = Vector3.Lerp(this.Position, this.m_goto, _LerpFactor);
            base.Update();
          
        }
        #endregion

        public void SetBodyAnimation(string name)
        {
            this.SetAnimationByName(name, false);
        }
        public void SetShirtAnimation(string name)
        {
            this.avatarShirtModel.SetAnimationByName(name, false);
        }

        public void SetLeggingsAnimation(string name)
        {
            this.avatarPantsModel.SetAnimationByName(name, false);
        }

        public void SetShoesAnimation(string name)
        {
            this.avatarShoesModel.SetAnimationByName(name, false);
        }

        public void UpdateTransformation(Vector3 currentPosition, Vector3 gotoPosition, Matrix4 currentOrientation)
        {
            lock (_lock)
            {
                Vector3 oc = this.m_receivedPositionCurrent;
                Vector3 og = this.m_receivedPositionGoto;
                Matrix4 oo = this.m_receivedOrientationCurrent;

                this.m_receivedPositionCurrent          = currentPosition;
                this.m_receivedPositionGoto             = gotoPosition;
                this.m_receivedOrientationCurrent       = currentOrientation;

                //this.Position = this.m_receivedPositionCurrent;
                //this.Orientation = this.m_receivedOrientationCurrent;
                this.m_updateTransformation = true;

#if DEBUG
                if (oc == this.m_receivedPositionCurrent)
                    NthDimension.Rendering.Utilities.ConsoleUtil.errorlog("Received Pos ", "Failed to Assign");

                if (og == this.m_receivedPositionGoto)
                    NthDimension.Rendering.Utilities.ConsoleUtil.errorlog("Received Goto ", "Failed to Assign");

                if (oo == this.m_receivedOrientationCurrent)
                    NthDimension.Rendering.Utilities.ConsoleUtil.errorlog("Received Ori ", "Failed to Assign");
#endif
            }
        }

        public void SetGotoOnce(Vector3 go)
        {
            this.m_goto = this.m_receivedPositionGoto = go;
        }
    }
}
