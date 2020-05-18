namespace NthDimension.Scripting
{
    public class ScriptTemplate
    {
        public static ScriptTemplate Default
        {
            get
            {
                ScriptTemplate st = new ScriptTemplate();
                st.template                 = "class Script { " + defaultScriptInsertionToken + " } ";
                st.ScriptClassName          = "Default script template";
                st.ScriptMainMethodName     = "Main";
                st.scriptClassName          = "Script";
                st.NewScriptStub            = "// The Main entry point of the Script\r\npublic static void Main(Reflex.nScript Base)\r\n{\r\n   // Your code here...\r\n}";

                return st;
            }
        }

        private string name = "untitled";

        protected static string defaultScriptInsertionToken = "[script]";

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        private string template = string.Empty;

        public string Template
        {
            get { return template; }
            set { template = value; }
        }

        private string newScriptStub = string.Empty;

        public string NewScriptStub
        {
            get { return newScriptStub; }
            set { newScriptStub = value; }
        }

        private string scriptClassName = string.Empty;

        public string ScriptClassName
        {
            get { return scriptClassName; }
            set { scriptClassName = value; }
        }

        private string scriptMainMethodName = string.Empty;

        public string ScriptMainMethodName
        {
            get { return scriptMainMethodName; }
            set { scriptMainMethodName = value; }
        }

        public ScriptTemplate()
        {
        }

        public virtual string ConstructScript(string partialScriptSource)
        {
            return this.Template.Replace(defaultScriptInsertionToken, partialScriptSource);
        }
    }
}
