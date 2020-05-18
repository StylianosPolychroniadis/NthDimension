namespace NthStudio.Compiler.Scripting
{
    // TODO:: NOTE:: This is the old version of 2010 NthDimension Compiler Template.
    // TODO::           Need to modify to match present flow
    public class ScriptTemplate
    {
        public static ScriptTemplate Default        // TODO: Revise, perhaps a collection with name/id retrieval would be ideal
        {
            get
            {
                ScriptTemplate st           = new ScriptTemplate();
                st.template                 = "class Script { " + defaultScriptInsertionToken + " } ";
                st.ScriptClassName          = "Default script template"; //st.scriptClassName = "Script";
                st.ScriptMainMethodName     = "Main";                
                st.NewScriptStub            = "// The Main entry point of the Script\r\npublic static void Main(Reflex.nScript Base)\r\n{\r\n   // Code is entered here\r\n}";

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
