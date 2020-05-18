namespace NthDimension.CodeGen.Dom
{
    internal sealed class VoidClass : DefaultClass
    {
        internal static readonly string VoidName = typeof(void).FullName;
        public static readonly VoidClass Instance = new VoidClass();

        private VoidClass()
            : base(DefaultCompilationUnit.DummyCompilationUnit, VoidName)
        {
        }

        protected override IReturnType CreateDefaultReturnType()
        {
            return VoidReturnType.Instance;
        }
    }
}
