using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthDimension
{
    /// <summary>
    /// Classes of the error event severity.
    /// </summary>
    public enum ErrorSeverity
    {
        MESSAGE,
        WARNING,
        ERROR,
    }

    /// <summary>
    /// Class provides info for errors, warnings and messages during 
    /// dataflow and workflow initialization.
    /// </summary>
    public class ErrorInfo
    {

        /// <summary>
        /// Error string message.
        /// </summary>
        public string Message { get; private set; }

        /// <summary>
        /// Line number where error occured in the source file.
        /// Lines are indexed from one. Zero or negative values
        /// mean that line number is undefined.
        /// </summary>
        public int Line { get; private set; }

        /// <summary>
        /// Column where error occured in the source file.
        /// Columns are indexed from one. Zero or negative values
        /// mean that column number is undefined.
        /// </summary>
        public int Column { get; private set; }

        /// <summary>
        /// Severity of the error.
        /// </summary>
        public ErrorSeverity Severity { get; private set; }

        /// <summary>
        /// Source file or module path where the error, 
        /// warning or the message occured.
        /// </summary>
        public string Location { get; private set; }

        /// <summary>
        /// Additional information specific for error.
        /// If you don't want to use additional tag, use 
        /// null value.
        /// </summary>
        public object Tag { get; private set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="message">String message related to the event that occured.</param>        
        /// <param name="line">Line where event occured in source location. Zero or negative number means undefined value.</param>
        /// <param name="column">Column where event occured in source location. Zero or negative number means undefined value.</param>
        /// <param name="severity">Severity of the error.</param>
        /// <param name="location">Source file path or module where event occured.</param>
        /// <param name="tag">Additional information specific for error message.</param>
        public ErrorInfo(string messsage, int line, int column, ErrorSeverity severity, string location, object tag)
        {
            Message = messsage;
            Severity = severity;
            Location = location;
            Line = line;
            Column = column;
            Tag = tag;
            if (Line < 0) Line = 0;
            if (Column < 0) Column = 0;
        }
    }
}
