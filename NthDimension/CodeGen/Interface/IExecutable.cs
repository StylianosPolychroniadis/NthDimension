using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthDimension.CodeGen
{
    /// <summary>
    /// The real plug-in interface we use to communicate across app-domains
    /// </summary>
    public interface IExecutable
    {
        void Initialize(IDictionary<string, object> Variables);
        object Run(string StartMethod, params object[] Parameters);
        void Dispose(IDictionary<string, object> Variables);
    }
}
