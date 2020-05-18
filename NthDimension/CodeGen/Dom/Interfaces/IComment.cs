namespace NthDimension.CodeGen.Dom
{
    public interface IComment
    {
        bool IsBlockComment
        {
            get;
        }

        string CommentTag
        {
            get;
        }

        string CommentText
        {
            get;
        }

        DomRegion Region
        {
            get;
        }
    }
}
