using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthStudio
{
    //[DataContract(Name = "MATRIXMODE", Namespace = "SYSCON.Graphics")]
    public enum MATRIXMODE
    {
        //[EnumMember(Value = "View")]
        View,
        //[EnumMember(Value = "World")]
        World,
        //[EnumMember(Value = "Local")]
        Local,
    };
}
