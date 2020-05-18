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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthDimension.Rendering
{
#pragma warning disable CS0661
#pragma warning disable CS0659
    public struct ApplicationState
    {
        
        public static ApplicationState         ContextActive   = new ApplicationState(0);             // OpenGL context active
        public static ApplicationState         Login           = new ApplicationState(1);             // Log into the Server
        public static ApplicationState         AssetLoad         = new ApplicationState(2);             // Initialize Loading Assets
        public static ApplicationState         Playing         = new ApplicationState(3);             // Application in Main Business Loop
        public static ApplicationState         Pause           = new ApplicationState(4);             // Application Displays Main Menu
       // public static ApplicationState         Booting          = new ApplicationState(5);             // Aplication has just started
        public static ApplicationState         ShutDown        = new ApplicationState(6);             // Application is Shutting Down
        public static ApplicationState         Editor          = new ApplicationState(7); // Application in Editor Mode;

        private int                     curState;

       
        private ApplicationState(int curState)
        {
            this.curState = curState;
        }

        public static bool operator ==(ApplicationState a, ApplicationState b)
        {
            return a.curState == b.curState;
        }

        public static bool operator !=(ApplicationState a, ApplicationState b)
        {
            return a.curState != b.curState;
        }

        public override string ToString()
        {
            switch(this.curState)
            {
                case 0:
                    return "Started";
                case 1:
                    return "Login";
                case 2:
                    return "InitLoading";
                case 3:
                    return "Playing";
                case 4:
                    return "Menu";
                case 5:
                    return "Starting";
                case 6:
                    return "ShutDown";
            }
            return string.Empty;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is ApplicationState)) return false;
            return (curState == ((ApplicationState)obj).curState);
        }
    }
}
