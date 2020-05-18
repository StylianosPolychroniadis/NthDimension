using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;

namespace NthDimension.Database
{
    public sealed class NthDatabase
    {
        #region Singleton
        /// <summary>
        /// Singleton instance.
        /// </summary>
        private static NthDatabase instance;           //static member to ensure single placeholder for object throughout scope of the class
        //used for locking block of code
        private static readonly object locker = new object();

        public static NthDatabase Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (locker)
                    {
                        instance = new NthDatabase();
                    }
                }
                return instance;
            }
        }

        #endregion
    }
}
