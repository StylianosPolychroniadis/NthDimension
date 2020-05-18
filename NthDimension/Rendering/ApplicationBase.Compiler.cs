namespace NthDimension.Rendering
{
   
    using System;
    using System.CodeDom.Compiler;
    using System.IO;
    using System.Collections;

    public partial class ApplicationBase
    {

        #region Application Compiler
        private ApplicationCompiler _compiler;
        private CompilerResults     m_results;

        public void CreateCompiler()
        {
            _compiler = new ApplicationCompiler();
            CodeGen.Project.IProject project = _compiler.GetDotNetProject();
            string assembly = project.AssemblyName;            
            string file = project.FileName;
            string root = project.RootNamespace;
            string path = string.Empty;

            try
            {
                //path = project.OutputAssemblyFullPath != null ? project.OutputAssemblyFullPath : string.Empty;
            }
            catch(NullReferenceException e) { path = "Null"; }

            //DocumentForm docProgram;
            //docProgram = this.AddDocument("Program.cs");

            Utilities.ConsoleUtil.log("    \tUser .Net Framework assembly:");
            Utilities.ConsoleUtil.log(String.Format("    \tAssembly Name      {1}{0}" +
                                                    "    \tAssembly Filename  {2}{0}" +
                                                    "    \tAssembly Namespace {3}{0}" +
                                                    "    \tAssembly Path      {4}{0}", System.Environment.NewLine,  
                                                    assembly, file, root, path ));
            Utilities.ConsoleUtil.log(Environment.NewLine);

        }
        #endregion


        #region Application Script Compiler
        private System.Security.PermissionSet   m_scriptPermissions;
        private ArrayList                       m_scriptReferenceAssemblies     = new ArrayList
        {
            "System.dll", 
            "System.Windows.Forms.dll", 
            "System.Data.dll",
            //Application.StartupPath + "\\nDimension.Reflex.exe"
        };

        // Public API
        private delegate void                   AddCompilerOutputLineDelegate(string line);
        private void                            AddCompilerOutputLine(string line)
        {
            //this.textBox1.Text += line + "\r\n";
        }
        public System.Security.PermissionSet    GetScriptPermissionSet()
        {
            this.m_scriptPermissions = Scripting.Script.GetDefaultScriptPermissionSet();

            // We'll want to show a MessageBox from our demo script, so
            // we need to add permission for that to our scripts permission
            // set. The scriptPermissions variable is passed into the script's
            // Run method and used there to restrict access for the script.
            //this.scriptPermissions.AddPermission( new UIPermission( UIPermissionWindow.SafeSubWindows));
            this.m_scriptPermissions.AddPermission(
                new System.Security.Permissions.UIPermission(
                    System.Security.Permissions.UIPermissionWindow.AllWindows));


            return this.m_scriptPermissions;
        }
        private void                            HandleCompilerOutput(string line)
        {
            //textBox1.BeginInvoke(new AddCompilerOutputLineDelegate(this.AddCompilerOutputLine), line);
        }
        public bool                             CompileScript(Scripting.Script script)
        {
            try
            {
                script.Compile(new Scripting.CompilerOutputDelegate(this.HandleCompilerOutput));
            }
            catch
            {
                return false;
            }

            return true;
        }
        #endregion Application Script Compiler




    }
}
