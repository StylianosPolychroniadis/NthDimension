namespace NthDimension.CodeGen
{
    using System;
    using System.IO;
    using System.Collections.Generic;
    using System.ComponentModel;

    public class CSharpEngine : Component// 
    {
        private string _AssemblyName = string.Empty;
        private string _Namespace = string.Empty;
        private string _EntryMethod = string.Empty;
        private string _DefaultClassName = string.Empty;
        private IDictionary<string, object> _RemoteVariables = null;

        //used in IsRunMode // private const string  //_appFriendlyName = "CsharpIDE.vshost.exe";       // The final executable assembly (with .vshost.exe???) ie nthDimension.vshost.exe???                                                         // "AIMSEngine.vshost.exe";
        private const string _instanceAssemblyName = "NthDimension"; //"SYSCON.ScriptRun";                  // The assembly name that contain IRun interface        // "AIMS.Scripting.ScriptRun"
        private const string _instanceTypeName     = "ScriptInstance";
        private const string _domainName           = "NthDimensionUserDomain";

        public CSharpEngine()
        {

        }

        public bool IsRunMode // TODO:: FIX OR MAKE VIRTUAL OR MOVE TO ApplicationBase -> DONE:: Fixed
        {
            get
            {
                AppDomain app = AppDomain.CurrentDomain;
                // if (app.FriendlyName == _appFriendlyName)
                if (app.FriendlyName.Contains(".exe" /*"NthDimension.exe"*/) ||
                    app.FriendlyName.Contains("vshost.exe" /*"NthDimension.vshost.exe"*/))
                    return true;
                return false;
            }
        }
        public string OutputAssemblyName
        {
            get { return _AssemblyName; }
            set { _AssemblyName = value; }
        }
        public string DefaultNameSpace
        {
            get { return _Namespace; }
            set { _Namespace = value; }
        }
        public string StartMethodName
        {
            get { return _EntryMethod; }
            set { _EntryMethod = value; }
        }
        public string DefaultClassName
        {
            get { return _DefaultClassName; }
            set { _DefaultClassName = value; }
        }
        public IDictionary<string, object> RemoteVariables
        {
            get { return _RemoteVariables; }
            set { _RemoteVariables = value; }
        }
        public object Execute(params object[] Parameters)
        {
            AppDomain secDom = null;
            bool isRunMode = this.IsRunMode;
            if (isRunMode)
                secDom = AppDomain.CurrentDomain;
            else
                secDom = AppDomain.CreateDomain(_domainName);


            //string f = secDom.BaseDirectory;

            //System.Windows.Forms.MessageBox.Show(f);

            // create the factory class in the secondary app-domain
            ScriptInstance sInstance = (ScriptInstance)secDom.CreateInstance(_instanceAssemblyName, _instanceTypeName).Unwrap();
            // with the help of this factory, we can now create a real 'LiveClass' instance
            //If Not Compiled Then Compiled
            //In furture version compiled assemblies are stored as Blob in database alone with package

            IExecutable sRun;

            //sRun = sInstance.Create(secDom,  m_CompilerResults.CompiledAssembly,"IRun", Parameters);
            sRun = sInstance.Create(secDom, Path.GetFileName(_AssemblyName), _Namespace + "." + _DefaultClassName, Parameters);

            object RetObj = null;
            try
            {
                sRun.Initialize(_RemoteVariables);
                RetObj = sRun.Run(_EntryMethod, Parameters);
                sRun.Dispose(_RemoteVariables);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (!isRunMode)
                    AppDomain.Unload(secDom);

            }
            return RetObj;
        }
    }
}
