namespace NthDimension.Rendering
{
    public partial class ApplicationObject
    {
        internal void resetUpdateState()
        {
            wasUpdated = false;
            resetUpdateStateChilds();
        }

        private void resetUpdateStateChilds()
        {
            foreach (var child in childs)
            //Parallel.ForEach(this.childs, new ParallelOptions {MaxDegreeOfParallelism = Environment.ProcessorCount}, (child) =>
            {
                child.resetUpdateState();
            }
            //);
        }
    }
}
