namespace NthStudio.Game.Dungeon
{
    interface IGrid<T>
    {
        void SetValue(int x, int y, T value);
        T GetValue(int x, int y);
    }
    internal static class GridUtil
    {
        /// <summary>
        /// Fills a rectangle of the specified dimensions with the specified value
        /// </summary>
        public static void FillRectangle<T>(IGrid<T> grid, int left, int top, int width, int height, T value)
        {
            for (int y = 0; y < height; y++)
                for (int x = 0; x < width; x++)
                    grid.SetValue(x + left, y + top, value);
        }

        /// <summary>
        /// Draws the outline of a rectangle with the specified dimensions,
        /// but does not fill it.
        /// </summary>
        public static void DrawRectangle<T>(IGrid<T> grid, int left, int top, int width, int height, T value)
        {
            int right = left + width - 1;
            int bottom = top + height - 1;
            for (int y = 0; y < height; y++)
            {
                grid.SetValue(left, y + top, value);
                grid.SetValue(right, y + top, value);
            }
            for (int x = 0; x < width; x++)
            {
                grid.SetValue(x + left, top, value);
                grid.SetValue(x + left, bottom, value);
            }
        }

    }
}
