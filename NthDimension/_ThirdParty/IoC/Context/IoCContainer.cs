using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NthDimension.Context
{
    public class IoCContainer
    {
        // Note: ToDo: Created IServiceRegistry class. See how is usefull to establish Services Execution from injection controllers
        // Note: Maybe a 

        private Dictionary<Type, ResolvedTypeWithLifeTime> iocMap = new Dictionary<Type, ResolvedTypeWithLifeTime>();

        public void Register<T1, T2>()
        {
            Register<T1, T2>(LifeTimeOptions.TransientControlledLifeTimeOption);
        }

        public void Register<T1, T2>(LifeTimeOptions lifeTimeOption)
        {
            if (iocMap.ContainsKey(typeof(T1)))
            {
                throw new Exception(string.Format("Type {0} already registered.", typeof(T1).FullName));
            }
            ResolvedTypeWithLifeTime targetType = new ResolvedTypeWithLifeTime(typeof(T2), lifeTimeOption);
            iocMap.Add(typeof(T1), targetType);
        }

        public T Resolve<T>()
        {
            return (T)Resolve(typeof(T));
        }

        public object Resolve(Type typeToResolve)
        {
            // Find the registered type for typeToResolve
            if (!iocMap.ContainsKey(typeToResolve))
                throw new Exception(string.Format("Can't resolve {0}. Type is not registed.", typeToResolve.FullName));

            ResolvedTypeWithLifeTime resolvedType = iocMap[typeToResolve];

            if (resolvedType.LifeTimeOption == LifeTimeOptions.ContainerControlledLifeTimeOption && resolvedType.InstanceValue != null)
                return resolvedType.InstanceValue;


            #region Factory
            // Try to construct the object
            // Step-1: find the constructor (ideally first constructor if multiple constructos present for the type)
            ConstructorInfo ctorInfo = resolvedType.ResolvedType.GetConstructors().First();

            // Step-2: find the parameters for the constructor and try to resolve those
            List<ParameterInfo> paramsInfo = ctorInfo.GetParameters().ToList();
            List<object> resolvedParams = new List<object>();
            foreach (ParameterInfo param in paramsInfo)
            {
                Type t = param.ParameterType;
                object res = Resolve(t);
                resolvedParams.Add(res);
            }
            #endregion

            // Step-3: using reflection invoke constructor to create the object
            object retObject = ctorInfo.Invoke(resolvedParams.ToArray());

            resolvedType.InstanceValue = retObject;

            return retObject;
        }
    }
}
