namespace NthDimension.CodeGen.Dom
{
    public class Constructor : DefaultMethod
    {
        public Constructor(ModifierEnum m, DomRegion region, DomRegion bodyRegion, IClass declaringType)
            : base("#ctor", declaringType.DefaultReturnType,
                   m, region, bodyRegion, declaringType)
        {
        }

        public Constructor(ModifierEnum m, IReturnType returnType, IClass declaringType)
            : base("#ctor", returnType, m, DomRegion.Empty, DomRegion.Empty, declaringType)
        {
        }

        /// <summary>
        /// Creates a default constructor for the class.
        /// The constructor has the region of the class and a documentation comment saying
        /// it is a default constructor.
        /// </summary>
        public static Constructor CreateDefault(IClass c)
        {
            Constructor con = new Constructor(ModifierEnum.Public, c.Region, c.Region, c);
            con.Documentation = "Default constructor of " + c.Name;
            return con;
        }
    }
}
