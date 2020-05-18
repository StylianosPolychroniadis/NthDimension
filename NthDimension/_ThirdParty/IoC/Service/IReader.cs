using System;

namespace NthDimension.Service
{
    public interface IReader
    {
        DateTime GetCreatedOn();
        string Read();
    }
}
