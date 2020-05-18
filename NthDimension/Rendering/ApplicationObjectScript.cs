using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace NthDimension.Rendering
{
    public class ApplicationScript : ApplicationObject
    {
        new public static string nodename = "script";

        private const string _nodeAttributeScriptSource = "code";

        private List<string> _sources;

        public ApplicationScript() : this(new List<string>())
        {
            
        }
        public ApplicationScript(List<string> sourceFiles)
        {
            _sources = new List<string>(); // Try to keep declaration in Ctor
        }


        protected override void specialLoad(ref XmlTextReader reader, string type)
        {
            Utilities.ConsoleUtil.log(string.Format("TEST:\tLoading {0} {1} - ToDo: Implement ApplicationObjectScript specialLoad()", reader.Name, type));
        }

        /// <summary>
        /// Will compile a collection of source-correct CSharp script file(s) using [TBD] compiler
        /// </summary>
        protected virtual void Compile()
        {

        }
        /// <summary>
        /// Will compile and run a collection of source-correct CSharp script file(s) using [TBD] compiler
        /// </summary>
        protected virtual void CompileAndRun()
        {

        }

        public override void Update()
        {
            this.CompileAndRun();
        }

    }
}
