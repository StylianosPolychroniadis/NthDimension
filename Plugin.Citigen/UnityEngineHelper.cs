﻿using NthDimension.Algebra;
using NthDimension.Rendering;
using System.Collections.Generic;

namespace RoadGen
{
    public static class UnityEngineHelper
    {
        public static List<T> GetInterfaces<T>(ApplicationObject gameObject) where T : class
        {
            ////UnityEngine.Component[] components = gameObject.GetComponents(typeof(T));
           
            
           


            //GameObject[] components = new GameObject[gameObject.Chi]

            //if (components.Length == 0)
            //    return null;
            List<T> interfaces = new List<T>();
            //foreach (var component in components)
            //    interfaces.Add((component as T));
            return interfaces;
        }

        public static T GetInterface<T>(ApplicationObject gameObject) where T : class
        {
            //UnityEngine.Component[] components = gameObject.GetComponents(typeof(T));
            //if (components.Length == 0)
            //    return null;
            //foreach (var component in components)
            //    return (component as T);
            return null;
        }

        //public static Bounds GetRenderingBounds(GameObject gameObject)        // Requires octree
        //{
        //    var meshRenderer = gameObject.GetComponentInChildren<MeshRenderer>();
        //    return meshRenderer.bounds;
        //}

    }

}