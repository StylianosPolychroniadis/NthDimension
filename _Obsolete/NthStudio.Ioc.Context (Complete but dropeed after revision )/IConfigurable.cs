namespace NthStudio.IoC.Context
{
    /// <summary>
    ///   Defines the interface for the classes which can be configured.
    /// </summary>
    public interface IConfigurable
    {
        /// <summary>
        ///   When implemented in a class, determines whether configuration of this instance is complete.
        /// </summary>
        bool IsConfigured { get; set; }

        /// <summary>
        ///   When implemented in a class, signals the implementer that it should do configure process.
        /// </summary>
        void Configure();
    }
}
