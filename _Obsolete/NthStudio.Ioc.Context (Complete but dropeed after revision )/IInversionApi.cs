using System;

namespace NthStudio.IoC.Context
{
    public interface IInversionApi
    {
        #region Standard SYSCON API Declarations - Common to ALL interfaces
        DateTime GetCreatedOn();
        string LastErrorMessage { get; }
        int LastError { get; }
        string SystemErrorMessage { get; }
        #endregion

    }
}
