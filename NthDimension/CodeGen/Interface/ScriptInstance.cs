namespace NthDimension.CodeGen
{
    using System;
    using System.Threading.Tasks;
    using System.Reflection;
    using System.Runtime.Remoting;

    public class ScriptInstance
    {
        private const BindingFlags bindings =   BindingFlags.Instance |
                                                BindingFlags.Public |
                                                BindingFlags.CreateInstance;
        public ScriptInstance() { }

        /// <summary> Factory method to create an instance of the type whose name is specified,
        /// using the named assembly file and the constructor that best matches the specified parameters. </summary>
        /// <param name="assemblyFile"> The name of a file that contains an assembly where the type named typeName is sought. </param>
        /// <param name="typeName"> The name of the preferred type. </param>
        /// <param name="constructArgs"> An array of arguments that match in number, order, and type the parameters of the constructor to invoke, or null for default constructor. </param>
        /// <returns> The return value is the created object represented as IRun. </returns>
        public IExecutable Create(string assemblyFile, string typeName, object[] constructArgs)
        {
            return (IExecutable)Activator.CreateInstanceFrom(assemblyFile, typeName, false, bindings, null, constructArgs, System.Globalization.CultureInfo.CurrentCulture, null).Unwrap();

            //return (IRun)Activator.CreateInstanceFrom(
            //assemblyFile, typeName, false, bfi, null, constructArgs,
            //null, null, null).Unwrap();
        }

        public IExecutable Create(AppDomain appdomain, string assemblyFile, string typeName, object[] constructArgs)
        {
            return (IExecutable)Activator.CreateInstanceFrom(appdomain, assemblyFile, typeName, false, bindings, null, constructArgs, System.Globalization.CultureInfo.CurrentCulture, null).Unwrap();

            //return (IRun)Activator.CreateInstanceFrom(appdomain,
            //assemblyFile, typeName, false, bfi, null, constructArgs,
            //null, null, null).Unwrap();
        }

        public IExecutable Create(Type type, object[] constructArgs)
        {
            ObjectHandle h = (ObjectHandle)Activator.CreateInstance(type, constructArgs);
            return (IExecutable)h.Unwrap();
        }
    }
}
