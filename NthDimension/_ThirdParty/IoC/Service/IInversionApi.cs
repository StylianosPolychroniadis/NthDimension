using System;

namespace NthDimension.Service
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
