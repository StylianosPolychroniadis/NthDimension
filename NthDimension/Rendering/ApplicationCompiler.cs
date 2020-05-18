namespace NthDimension.Rendering
{
    using NthDimension.CodeGen.Project;
    using NthDimension.CodeGen;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Threading;
    using System.IO;
    using NRefactory;
    using System.CodeDom.Compiler;
    using System.Xml;

    public class ApplicationCompiler : ApplicationObject // ScriptControl in 2017
    {
        private string[] _referenceDlls          = new string[8] {
                                                                    "System.dll",
                                                                    "System.Drawing.dll",
                                                                    "NthDimension.Math.dll",
                                                                    "NthDimension.Compute.dll",
                                                                    "NthDimension.dll",
                                                                    "NthDimension.Rasterizer.dll",
                                                                    "NthDimension.Forms.dll",
                                                                    "NthDimension.FFMpeg"
                                                                };

        private const string    _defaultClassName = "Program";
        private const string    _entryMethodName = "Main";

        private string _mainMethodCode
        {
            get
            {


                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                sb.AppendLine("using System;");
                sb.AppendLine("using System.Drawing;");
                sb.AppendLine("using NthDimension.Algebra;");
                sb.AppendLine("using NthDimension.Rendering;");
                sb.AppendLine("using NthDimension.Rendering.Scenegraph;");
                sb.AppendLine("using NthDimension.Rendering.Drawables;");
                sb.AppendLine("using NthDimension.Rasterizer;");                
                sb.AppendLine(string.Empty);
                sb.AppendLine("// -----------");
                sb.AppendLine("//  API Usage");
                sb.AppendLine("// -----------");
                sb.AppendLine("//  -Use 'Scene' variable to access the Scenegraph structure");
                sb.AppendLine("//  -Use 'Renderer' variable to instanciate new objects");
                sb.AppendLine("//");
                sb.AppendLine("//  Example:");
                sb.AppendLine("//       Scene.Add(new Cube(Renderer));              // Adds a new Cube into the Scenegraph");
                sb.AppendLine(string.Empty);
                sb.AppendLine("namespace " + _dotNetProject.RootNamespace);
                sb.AppendLine("{");
                sb.AppendLine("\tpublic partial class " + _defaultClassName);
                sb.AppendLine("\t{");
                sb.AppendLine(string.Empty);
                sb.AppendLine(string.Empty);
                sb.AppendLine("\t\tpublic int " + _entryMethodName + "()");
                sb.AppendLine("\t\t{");
                sb.AppendLine("\t\t\t");
                sb.AppendLine("\t\t\treturn 0;");
                sb.AppendLine("\t\t}");
                sb.AppendLine("\t}");
                sb.AppendLine("}");

                return sb.ToString();
            }
        }

        private string _programCode
        {
            get
            {
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                sb.AppendLine("#region System Generated Source Code.Please do not change ...");
                sb.AppendLine("namespace " + _dotNetProject.RootNamespace);
                sb.AppendLine("{");
                sb.AppendLine("\tusing System;");
                sb.AppendLine("\tusing System.Collections.Generic;");
                sb.AppendLine("\tusing System.Diagnostics;");
                sb.AppendLine("\tusing System.Reflection;");
                sb.AppendLine("\tpublic partial class " + _defaultClassName + " : MarshalByRefObject, IRun");
                sb.AppendLine("\t{");

                System.Text.StringBuilder ub = new System.Text.StringBuilder();
                foreach (string key in _compiler.RemoteVariables.Keys)
                {
                    object obj = null;
                    if (_compiler.RemoteVariables.TryGetValue(key, out obj))
                    {
                        ub.Append("\t\tpublic static ");
                        ub.Append(obj.GetType().BaseType.FullName);
                        ub.Append(" ");
                        ub.Append(key);
                        ub.Append(" = null;");
                        ub.AppendLine();
                    }
                }
                ub.AppendLine();

                string objSrc = ub.ToString(); // Just for debug?
                sb.AppendLine(objSrc);
                sb.AppendLine("\t\t[DebuggerStepperBoundary()]");
                sb.AppendLine("\t\tvoid IRun.Initialize(IDictionary<string, object> Variables)");
                sb.AppendLine("\t\t{");
                sb.AppendLine("\t\t\tforeach (string name in Variables.Keys)");
                sb.AppendLine("\t\t\t{");
                sb.AppendLine("\t\t\t\tobject value = null;");
                sb.AppendLine("\t\t\t\ttry");
                sb.AppendLine("\t\t\t\t{");
                sb.AppendLine("\t\t\t\t\tVariables.TryGetValue(name, out value);");
                sb.AppendLine("\t\t\t\t\tFieldInfo fInfo = this.GetType().GetField(name, BindingFlags.Public | BindingFlags.Static);");
                sb.AppendLine("\t\t\t\t\tfInfo.SetValue(this, value);");
                sb.AppendLine("\t\t\t\t}");
                sb.AppendLine("\t\t\t\tcatch(Exception ex)");
                sb.AppendLine("\t\t\t\t{");
                sb.AppendLine("\t\t\t\t\tthrow ex;");
                sb.AppendLine("\t\t\t\t}");
                sb.AppendLine("\t\t\t}");
                sb.AppendLine("\t\t}");
                sb.AppendLine("");
                sb.AppendLine("\t\t[DebuggerStepperBoundary()]");
                sb.AppendLine("\t\tobject IRun.Run(string StartMethod, params object[] Parameters)");
                sb.AppendLine("\t\t{");
                sb.AppendLine("\t\t\ttry");
                sb.AppendLine("\t\t\t{");
                sb.AppendLine("\t\t\t\tMethodInfo methodInfo = this.GetType().GetMethod(StartMethod,BindingFlags.Instance | BindingFlags.Public | BindingFlags.CreateInstance);");
                sb.AppendLine("\t\t\t\treturn methodInfo.Invoke(this, Parameters);");
                sb.AppendLine("\t\t\t}");
                sb.AppendLine("\t\t\tcatch (Exception ex)");
                sb.AppendLine("\t\t\t{");
                sb.AppendLine("\t\t\t\tthrow ex;");
                sb.AppendLine("\t\t\t}");
                sb.AppendLine("\t\t}");
                sb.AppendLine("");
                sb.AppendLine("\t\t[DebuggerStepperBoundary()]");
                sb.AppendLine("\t\tvoid IRun.Dispose(IDictionary<string, object> Variables)");
                sb.AppendLine("\t\t{");
                sb.AppendLine("\t\t\tforeach (string name in Variables.Keys)");
                sb.AppendLine("\t\t\t{");
                sb.AppendLine("\t\t\t\tobject value = null; ;");
                sb.AppendLine("\t\t\t\ttry");
                sb.AppendLine("\t\t\t\t{");
                sb.AppendLine("\t\t\t\t\tFieldInfo fInfo = this.GetType().GetField(name, BindingFlags.Public | BindingFlags.Static);");
                sb.AppendLine("\t\t\t\t\tfInfo.SetValue(this, value);");
                sb.AppendLine("\t\t\t\t}");
                sb.AppendLine("\t\t\t\tcatch (Exception ex)");
                sb.AppendLine("\t\t\t\t{");
                sb.AppendLine("\t\t\t\t\tthrow ex;");
                sb.AppendLine("\t\t\t\t}");
                sb.AppendLine("\t\t\t}");
                sb.AppendLine("\t\t}");
                sb.AppendLine("\t}");
                sb.AppendLine("}");
                sb.AppendLine("#endregion");
                return sb.ToString();
            }
        }

        public event EventHandler               Execute;
        public event EventHandler               Build;

        private CodeGen.CSharpEngine            _compiler                   = new CodeGen.CSharpEngine();
        private ScriptLanguage                  _scriptLanguage             = ScriptLanguage.CSharp;
        private static IProject                 _dotNetProject              = new DefaultProject();

        public IDictionary<string, object>     RemoteVariables            = new System.Collections.Generic.Dictionary<string, object>();

        private const string                    _nthDimensionReferenceDll   = "NthDimension.dll";

        public ApplicationCompiler()
        {
            AddReference(new ReferenceAssembly(_dotNetProject, "System"));
            AddReference(new ReferenceAssembly(_dotNetProject, "System.Drawing"));

            foreach (var reference in _referenceDlls)
            {
                ReferenceAssembly ra = getReferenceFromDll(reference);
                {
                    if (null != ra)
                    {

                        this.AddReference(ra);
                    }
                    else
                        throw new ReferenceAssemblyException(string.Format("Failed to load reference assembly {0}", reference));
                }
            }

            this.AddObject("Application", ApplicationBase.Instance);    // Note: Contains both Scene and Renderer

            
        }

        public IProject GetDotNetProject()
        {
            return _dotNetProject;
        }

        public void AddObject(string Name, object Value)
        {
            try
            {
                RemoteVariables.Add(Name, Value);
            }
            catch (Exception Ex)
            {
                throw Ex;
            }

        }
        public void AddReference(ProjectItem Reference)
        {
            //TreeNode refNode = _winProjExplorer.RefrenceNode;
            ArrayList list = new ArrayList();
            list.Add(Reference);
            ConvertCOM(null, list/*, refNode*/);
        }

        
        #region Component Object Model
        private ArrayList ImportCom(ComReferenceProjectItem t)
        {
            ArrayList refrences = new ArrayList();
            refrences.Add(t as ReferenceAssembly);
            //BeginInvoke(new MethodInvoker(delegate { StatusMessage.Text = "Compiling COM component '" + t.Include + "' ..."; }));
            CodeGen.Converter.TlbImp importer = new CodeGen.Converter.TlbImp(refrences);
            importer.ReportEvent += new EventHandler<CodeGen.Converter.ReportEventEventArgs>(importer_ReportEvent);
            importer.ResolveRef += new EventHandler<CodeGen.Converter.ResolveRefEventArgs>(importer_ResolveRef);
            string outputFolder = Path.GetDirectoryName(_dotNetProject.OutputAssemblyFullPath);
            string interopFileName = Path.Combine(outputFolder, String.Concat("Interop.", t.Include, ".dll"));
            string asmPath = interopFileName;
            importer.Import(asmPath, t.FilePath, t.Name);
            return refrences;
        }
        private void ConvertCOM(object sender, ArrayList refrences/*, TreeNode node*/)
        {
            object[] param = new object[] { sender, (object)refrences/*, (object)node*/ };
            ThreadPool.QueueUserWorkItem(new WaitCallback(ConvertCOMThread), (object)param);
        }
        private void ConvertCOMThread(Object stateInfo)
        {
            object[] param = (object[])stateInfo;
            object sender = param[0];
            ArrayList refrences = param[1] as ArrayList;
            
            foreach (ReferenceAssembly reference in refrences)
            {
                try
                {
                    if (reference.ItemType == ItemType.COMReference)
                    {
                        if (Path.IsPathRooted(reference.FileName))
                        {
                            _dotNetProject.AddProjectItem(reference);
                        }
                        else
                        {
                            ArrayList addedRefs = ImportCom(reference as ComReferenceProjectItem);
                            foreach (ReferenceAssembly refs in addedRefs)
                            {
                                _dotNetProject.AddProjectItem(refs);
                                //BeginInvoke(new MethodInvoker(delegate
                                //{
                                //    TreeNode refNode = node.Nodes.Add(refs.Name);
                                //    refNode.ImageKey = "Reference.ico";
                                //    refNode.Tag = NodeType.Reference;
                                //}));
                            }

                        }
                    }
                    else if (reference.ItemType == ItemType.Reference)
                    {
                        _dotNetProject.AddProjectItem(reference);
                        //BeginInvoke(new MethodInvoker(delegate
                        //{
                        //    TreeNode refNode = node.Nodes.Add(reference.Name);
                        //    refNode.ImageKey = "Reference.ico";
                        //    refNode.Tag = NodeType.Reference;
                        //}));
                    }

                }
                catch (Exception Ex)
                {
                    //MessageBox.Show(Ex.Message);
                }
                //BeginInvoke(new MethodInvoker(delegate { StatusMessage.Text = "Ready"; }));
            }
        }
        #endregion Convert 

        #region Script Language
        private void ConvertToLanguage(ScriptLanguage OldLang, ScriptLanguage NewLanguage)
        {

            ResetParserLanguage(NewLanguage);
            ////Disable On Change Event


            throw new NotImplementedException("Transfer from NthDimension 2017");
            //foreach (WeifenLuo.WinFormsUI.Docking.IDockContent docWin in dockContainer1.Contents) //foreach (IDockableWindow docWin in dockContainer1.Contents)
            //{
            //    DocumentForm doc = null;
            //    if (docWin is DocumentForm)
            //    {
            //        doc = docWin as DocumentForm;
            //        DocumentEvents(doc, false);
            //        if (OldLang != NewLanguage)
            //        {
            //            doc.FileName = Path.GetFileNameWithoutExtension(doc.FileName) + (NewLanguage == ScriptLanguage.CSharp ? ".cs" : ".vb");
            //            if (NewLanguage == ScriptLanguage.CSharp)
            //            {
            //                doc.ScriptLanguage = NewLanguage;
            //                doc.Contents = Parser.ProjectParser.GetFileContents(doc.FileName);
            //            }
            //            else
            //            {
            //                doc.Contents = Parser.ProjectParser.GetFileContents(doc.FileName);
            //                doc.ScriptLanguage = NewLanguage;
            //            }
            //        }

            //        DocumentEvents(doc, true);
            //    }
            //}
            ////Enable On Change Event
            //if (_winErrorList != null)
            //    _winErrorList.ConvertToLanguage(OldLang, NewLanguage);
            //_winProjExplorer.Language = NewLanguage;
        }
        private void ResetParserLanguage(ScriptLanguage lang)
        {
            if (lang == ScriptLanguage.CSharp)
                CodeGen.Parser.ProjectParser.Language = SupportedLanguage.CSharp;
            else
                CodeGen.Parser.ProjectParser.Language = SupportedLanguage.VBNet;

        }
        #endregion

        #region Logging
        void importer_ReportEvent(object sender, CodeGen.Converter.ReportEventEventArgs e)
        {
            string msg;
            msg = Environment.NewLine + "COM Importer Event ..." + Environment.NewLine;
            msg += "Kind: " + e.EventKind.ToString() + Environment.NewLine;
            msg += "Code: " + e.EventCode + Environment.NewLine;
            msg += "Message: " + e.EventMsg;
            //BeginInvoke(new MethodInvoker(delegate { StatusMessage.Text = e.EventMsg; }));
            ////BeginInvoke(new MethodInvoker(delegate { _winOutput.AppendLine(msg); }));
        }

        void importer_ResolveRef(object sender, CodeGen.Converter.ResolveRefEventArgs e)
        {
            //BeginInvoke(new MethodInvoker(delegate { StatusMessage.Text = e.Message; }));
            ////BeginInvoke(new MethodInvoker(delegate { _winOutput.AppendLine(e.Message); }));
        }
        #endregion Logging

        protected virtual void OnExecute()
        {
            if (Execute != null)
                Execute(this, null);
        }

        protected virtual void OnBuild()
        {


            if (Build != null)
                Build(this, null);
        }

        public CompilerResults CompileScript()
        {
            CodeDomProvider provider = _dotNetProject.LanguageProperties.CodeDomProvider;
            CompilerParameters parameters = new CompilerParameters();
            parameters.GenerateExecutable = false;
            parameters.GenerateInMemory = false;
            parameters.IncludeDebugInformation = true;
            parameters.OutputAssembly = _dotNetProject.OutputAssemblyFullPath;

            foreach (ProjectItem item in _dotNetProject.Items)
            {
                if (null == item)
                    continue;

                parameters.ReferencedAssemblies.Add(item.Include + ".dll");
            }
            parameters.ReferencedAssemblies.Add(_nthDimensionReferenceDll);      // TODO:: Add nthDimension references here

            string[] sourceCode = new string[NthDimension.CodeGen.Parser.ProjectParser.ProjectFiles.Count];
            int counter = 0;
            string tmpFilePath = Path.Combine(Path.GetDirectoryName(_dotNetProject.OutputAssemblyFullPath), "Temp");

            if (Directory.Exists(tmpFilePath))
                Directory.Delete(tmpFilePath, true);
            Directory.CreateDirectory(tmpFilePath);

            foreach (NthDimension.CodeGen.Parser.ProjectContentItem pcItem in NthDimension.CodeGen.Parser.ProjectParser.ProjectFiles.Values)
            {
                StreamWriter writer = new StreamWriter(Path.Combine(tmpFilePath, pcItem.FileName), false);
                writer.Write(pcItem.Contents);
                writer.Close();
                sourceCode[counter++] = Path.Combine(tmpFilePath, pcItem.FileName);
            }

            CompilerResults results = provider.CompileAssemblyFromFile(parameters, sourceCode);
            Directory.Delete(tmpFilePath, true);
            return results;

        }

        protected override void specialLoad(ref XmlTextReader reader, string type)
        {

            Utilities.ConsoleUtil.log(string.Format("TEST:\tLoading {0} {1}", reader.Name, type));

            
        }

        private ReferenceAssembly getReferenceFromDll(string dllFile, bool specificVersion = false)
        {
            ReferenceAssembly assemblyRef   = null;

            assemblyRef                     = new ReferenceAssembly(_dotNetProject);
            assemblyRef.Include             = Path.GetFileNameWithoutExtension(dllFile);
            assemblyRef.HintPath            = NthDimension.Utilities.FileUtility.GetRelativePath(NthDimension.Utilities.DirectoryUtil.AssemblyDirectory, dllFile);
            assemblyRef.SpecificVersion     = specificVersion;

            return assemblyRef;
        }

        public CompilerResults BuildScript()
        {
            CodeGen.Parser.ProjectParser.ParseProjectContents("Program.Sys.cs", _programCode);

            if (_compiler.RemoteVariables.ContainsKey("Scene"))
                _compiler.RemoteVariables.Add("Scene", ApplicationBase.Instance.Scene);
            if (_compiler.RemoteVariables.ContainsKey("Renderer"))
                _compiler.RemoteVariables.Add("Renderer", ApplicationBase.Instance.Renderer);

            return this.CompileScript();
        }

        #region Was used for UI Winforms logic (DocumentForm)
        public class ParseContentEventArgs : EventArgs
        {
            public string FileName = "";
            public string Content = "";
            public int Column = 0;
            public int Line = 0;
            public ParseContentEventArgs(string fileName, string content)
            {
                FileName = fileName;
                Content = content;
            }
        }

        void DoParsing(object sender, ParseContentEventArgs e, bool IsOpened)
        {
            object[] param = new object[] { sender, (object)e, true };
            ThreadPool.QueueUserWorkItem(new WaitCallback(ParseContentThread), (object)param);
        }
        void ParseContentThread(Object stateInfo)
        {
            object[] param = (object[])stateInfo;
            object sender = param[0];
            ParseContentEventArgs e = param[1] as ParseContentEventArgs;
            bool IsOpened = (bool)param[2];
            CodeGen.Dom.ParseInformation pi = CodeGen.Parser.ProjectParser.ParseProjectContents(e.FileName, e.Content, IsOpened);
            NRefactory.Parser.Errors errors = CodeGen.Parser.ProjectParser.LastParserErrors;

            if (errors.ColumnNo == 0 && errors.LineNo == 0)
                return;
            //DocumentForm doc = sender as DocumentForm;
            //{
            //if (_winErrorList != null)
            //   _winErrorList.AddItem(e.ErrorOutput + "\t" + e.Error.Target.ToString(), doc.ToString(), e.LineNo, e.ColumnNo, ErrorSeverity.ERROR, doc);
            //}
        }
        #endregion

    }
}
