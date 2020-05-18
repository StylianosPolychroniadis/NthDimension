namespace NthDimension.Scripting
{
    using System;
    using System.Security;
    using System.Security.Permissions;   
    using System.Reflection;

    public delegate void CompilerOutputDelegate(string outputLine);
    public delegate void ScriptSourceChangedHandler(string newSource, string oldSource);

    public class Script
    {
        public event ScriptSourceChangedHandler ScriptSourceChanged;

        /// <summary>
        /// This method contructs the default permission set our scripts may use.
        /// Host applications will need to supply an custom PermissionSet to the
        /// script's Run method if more permissions are needed (like the 
        /// SecurityPermissionFlag.UnmanagedCode for MDX applications).
        /// </summary>
        /// <returns>The default permission set our scripts may use</returns>
        public static PermissionSet GetDefaultScriptPermissionSet()
        {
            //PermissionSet internalDefScriptPermSet = new PermissionSet(PermissionState.None);
            PermissionSet internalDefScriptPermSet = new PermissionSet(PermissionState.Unrestricted);

            internalDefScriptPermSet.AddPermission(new SecurityPermission(SecurityPermissionFlag.Execution));
            internalDefScriptPermSet.AddPermission(new ReflectionPermission(ReflectionPermissionFlag.MemberAccess));   // TODO: See RestrictedMemberAccess         

            return internalDefScriptPermSet;
        }


        private ReferencedAssemblyCollection referencedAssemblies = new ReferencedAssemblyCollection();
        public ReferencedAssemblyCollection ReferencedAssemblies
        {
            get { return referencedAssemblies; }
            set { referencedAssemblies = value; }
        }

        private Assembly cachedScriptAssembly = null;
        private bool sourceChanged = false;

        public bool SourceChanged
        {
            get { return sourceChanged; }
            set { sourceChanged = value; }
        }

        private ScriptTemplate template = ScriptTemplate.Default;
        public ScriptTemplate Template
        {
            get { return template; }
            set { template = value; }
        }

        private string source = string.Empty;

        public string Source
        {
            get { return source; }
            set
            {
                sourceChanged = source != value;

                if (sourceChanged)
                {
                    if (ScriptSourceChanged != null)
                    {
                        ScriptSourceChanged(value, source);
                    }
                }

                source = value;
            }
        }

        internal string TemplatedSource
        {
            get
            {
                return this.Template.ConstructScript(this.Source);
            }
        }

        public Script()
        {

        }

        public void Compile(CompilerOutputDelegate cod)
        {
            try
            {
                cachedScriptAssembly = ScriptCompiler.DefaultCompiler.Compile(this, cod).CompiledAssembly;
                sourceChanged = cachedScriptAssembly != null;
            }
            catch (System.IO.FileNotFoundException exp)
            {
                throw new Exception("Unable to load script assembly (probably a compiler error, check debug output)", exp);
            }
        }

        public void Run(CompilerOutputDelegate cod, object subject, object[] parameters)
        {
            this.Run(cod, subject, parameters, GetDefaultScriptPermissionSet());
        }

        public void Run(CompilerOutputDelegate cod, object subject, object[] parameters, PermissionSet permissionSet)
        {
            if (cachedScriptAssembly == null || sourceChanged)
            {
                // compile if necessary
                cachedScriptAssembly = ScriptCompiler.DefaultCompiler.Compile(this, cod).CompiledAssembly;
                sourceChanged = cachedScriptAssembly != null;
            }


            if (cachedScriptAssembly != null)
            {
                // restrict code security
                permissionSet.PermitOnly();

                // run script
                try
                {
                    this.cachedScriptAssembly.GetType(Template.ScriptClassName).GetMethod(Template.ScriptMainMethodName).Invoke(subject, parameters);
                }
                catch (Exception e)
                {
                    Rendering.Utilities.ConsoleUtil.errorlog("Failed to run.", string.Format("{0} {1}", e.Message, e.StackTrace));
                }

                // revert security restrictions
                CodeAccessPermission.RevertPermitOnly();
            }
        }

    }
}
