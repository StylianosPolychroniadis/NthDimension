using System;

namespace NthStudio.Plugins
{
    [global::System.AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class PluginAttribute : Attribute
    {
        private readonly string                 name;
        private readonly string                 description;
        private readonly string                 author;
        private readonly string                 version;
        private string                          date;

        // TODO: Add more (Author, logo, email, contact, license, runmode, etc)

        // This is a positional argument
        public PluginAttribute(string name, string description, string author, string version, string date)
        {
            this.name = name;
            this.description = description;
            this.author = author;
            this.version = version;
            this.date = date;
        }

        public string                           Name
        {
            get { return name; }
        }
        public string                           Description
        {
            get { return description; }
        }
        public string                           Author
        {
            get { return this.author; }
        }
        public string                           Version
        {
            get { return this.version; }
        }
        public string                           Date
        {
            get { return this.date; }
        }
    }
}
