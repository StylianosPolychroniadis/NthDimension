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
using ProtoBuf;

namespace NthDimension.Rendering.Serialization
{
    [ProtoContract]
    public class ProtoArray<T>
    {
        [ProtoMember(1)]
        public int[] Dimensions { get; set; }
        [ProtoMember(2)]
        public T[] Data { get; set; }
    }

    public static class ProtoArrayExtensions
    {
        #region Multidimensional array handling

        public static ProtoArray<T> ToProtoArray<T>(this Array array)
        {
            if(null == array)
                return new ProtoArray<T> { Dimensions = new int[0], Data = null };

            if(array.Rank == 1)
                return new ProtoArray<T> { Dimensions = new int[0], Data = null };

            //if (array.Rank == 1)
            //{

                
            //    List<T[]> row = new List<T[]>();
            //    for (int i = 0; i < array.Length; i++)
            //    {
            //        T[] rowdat = (T[]) array.GetValue(i);
            //        row.Add(rowdat);

            //    }

            //    return new ProtoArray<T>
            //    {
            //        Dimensions = new int[1],
            //        Data = row.ToArray();
            //    };
            //}

            // Copy dimensions (to be used for reconstruction).
            var dims = new int[array.Rank];
            for (int i = 0; i < array.Rank; i++) dims[i] = array.GetLength(i);
            // Copy the underlying data.
            var data = new T[array.Length];
            var k = 0;
            array.MultiLoop(indices => data[k++] = (T)array.GetValue(indices));

            return new ProtoArray<T> { Dimensions = dims, Data = data };
        }

        public static Array ToArray<T>(this ProtoArray<T> protoArray)
        {
            if (protoArray.Data == null)
                return null;

            // Initialize array dynamically.
            var result = Array.CreateInstance(typeof(T), protoArray.Dimensions);
            // Copy the underlying data.
            var k = 0;
            result.MultiLoop(indices => result.SetValue(protoArray.Data[k++], indices));

            return result;
        }

        #endregion

        #region Array extensions

        public static void MultiLoop(this Array array, Action<int[]> action)
        {
            array.RecursiveLoop(0, new int[array.Rank], action);
        }

        private static void RecursiveLoop(this Array array, int level, int[] indices, Action<int[]> action)
        {
            if (level == array.Rank)
            {
                action(indices);
            }
            else
            {
                for (indices[level] = 0; indices[level] < array.GetLength(level); indices[level]++)
                {
                    RecursiveLoop(array, level + 1, indices, action);
                }
            }
        }

        #endregion
    }
}
