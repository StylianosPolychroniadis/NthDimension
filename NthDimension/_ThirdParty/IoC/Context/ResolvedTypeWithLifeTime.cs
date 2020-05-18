using System;

namespace NthDimension.Context
{
    public class ResolvedTypeWithLifeTime
    {
        public Type ResolvedType { get; set; }
        public LifeTimeOptions LifeTimeOption { get; set; }
        public object InstanceValue { get; set; }

        public ResolvedTypeWithLifeTime(Type resolvedType)
        {
            ResolvedType = resolvedType;
            LifeTimeOption = LifeTimeOptions.TransientControlledLifeTimeOption;
            InstanceValue = null;
        }
        public ResolvedTypeWithLifeTime(Type resolvedType, LifeTimeOptions lifeTimeOption)
        {
            ResolvedType = resolvedType;
            LifeTimeOption = lifeTimeOption;
            InstanceValue = null;
        }
    }
}
