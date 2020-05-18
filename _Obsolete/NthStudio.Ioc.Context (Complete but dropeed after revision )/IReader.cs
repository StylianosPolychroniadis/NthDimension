using System;

namespace NthStudio.IoC.Context
{
    public interface IReader
    {
        DateTime GetCreatedOn();
        string Read();
    }
}
