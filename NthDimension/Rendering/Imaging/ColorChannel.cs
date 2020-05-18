namespace NthDimension.Rendering.Imaging
{
    public enum enuColorChannel : int
    {
        /// <summary>
        /// Red Channel
        /// </summary>
        R = 0x01,

        /// <summary>
        /// Green Channel
        /// </summary>
        G = 0x02,

        /// <summary>
        /// Blue Channel
        /// </summary>
        B = 0x03,

        /// <summary>
        /// Alpha Channel (Transparency)
        /// </summary>
        A = 0x04,
    }

    public struct PixelData
    {
        public byte Blue;

        public byte Green;

        public byte Red;

        public byte Alpha;
    }
}
