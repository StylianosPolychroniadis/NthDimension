namespace NthDimension.CodeGen.Dom
{
    public class ParseInformation
    {
        ICompilationUnit validCompilationUnit;
        ICompilationUnit dirtyCompilationUnit;

        public ICompilationUnit ValidCompilationUnit
        {
            get
            {
                return validCompilationUnit;
            }
            set
            {
                validCompilationUnit = value;
            }
        }

        public ICompilationUnit DirtyCompilationUnit
        {
            get
            {
                return dirtyCompilationUnit;
            }
            set
            {
                dirtyCompilationUnit = value;
            }
        }

        public ICompilationUnit BestCompilationUnit
        {
            get
            {
                return validCompilationUnit == null ? dirtyCompilationUnit : validCompilationUnit;
            }
        }

        public ICompilationUnit MostRecentCompilationUnit
        {
            get
            {
                return dirtyCompilationUnit == null ? validCompilationUnit : dirtyCompilationUnit;
            }
        }
    }
}
