using System;

namespace NthStudio.Game.Dungeon
{
    /// <summary>
    /// Stores a two-dimensional grid of values which automatically expands in any direction to hold new values
    /// which are set into the grid.  Implementation is similar to a quadtree data structure.
    /// </summary>
    /// <typeparam name="T">Type of values to store in the grid</typeparam>
    class ExpandableGrid<T> : IGrid<T>
    {
        /// <summary>
        /// Minimum size for sub-grids (both width and height).
        /// </summary>
        const int MinGridSize = 16;

        /// <summary>
        /// The default value to return if no SetValue has been called for the specified coordinates
        /// </summary>
        T _defaultValue;

        /// <summary>
        /// The root grid contains all other grids which have been allocated so far
        /// </summary>
        AbstractGrid _root;

        /// <summary>
        /// Creates a new ExpandableGrid with the specified default value
        /// </summary>
        /// <param name="defaultValue">Default value to return if no SetValue has been called for
        /// the specified coordinates, or if the coordinates have not been allocated</param>
        public ExpandableGrid(T defaultValue)
        {
            // Remember the default value for when the user calls GetValue() with unallocated coordinates.
            _defaultValue = defaultValue;

            // Create a 16x16 real grid to start with.  We can expand it as needed when SetValue is called.
            _root = new RealGrid(0, 0, MinGridSize, MinGridSize, defaultValue);
        }

        /// <summary>
        /// Gets the stored grid value at the specified coordinates.
        /// </summary>
        /// <param name="x">X</param>
        /// <param name="y">Y</param>
        /// <returns>Returns the grid value at the specified coordinates.  If SetValue(x,y) has not yet
        /// been called then the default value will be returned.</returns>
        public T GetValue(int x, int y)
        {
            if (_root.Contains(x, y))
                return _root.GetValue(x, y);
            else
                return _defaultValue;
        }

        /// <summary>
        /// Sets the grid value at the specified coordinates.
        /// </summary>
        /// <param name="x">X</param>
        /// <param name="y">Y</param>
        /// <param name="value">Value to store in the grid</param>
        public void SetValue(int x, int y, T value)
        {
            // Make sure that we have allocated sub-grids to contain these coordinates
            ExpandToContain(x, y);

            // Store the value
            _root.SetValue(x, y, value);
        }

        /// <summary>
        /// Expand the size of our root grid in order to contain the specified coordinates.
        /// </summary>
        /// <param name="x">X</param>
        /// <param name="y">Y</param>
        void ExpandToContain(int x, int y)
        {
            // Do nothing if our grid is already big enough
            if (_root.Contains(x, y))
                return;

            // The expanded grid will be twice as large
            int targetWidth = _root.Width * 2;
            int targetHeight = _root.Height * 2;

            // Determine which direction we should expand in
            int targetLeft = _root.Left;
            int targetTop = _root.Top;
            if (x < _root.Left)
                targetLeft -= _root.Width;
            if (y < _root.Top)
                targetTop -= _root.Height;

            // Create the new root grid
            VirtualGrid newRoot = new VirtualGrid(targetLeft, targetTop, targetWidth, targetHeight, _defaultValue);

            // Put the old root grid inside the new root
            newRoot.SetQuadrant(_root);

            // Remember the new root
            _root = newRoot;

            // Are we big enough yet?
            ExpandToContain(x, y);
        }

        /// <summary>
        /// Attempts to "clean" adjacent sub-grids which are full by merging them into one grid.  This method can
        /// improve grid performance if it is called after setting many values.
        /// </summary>
        public void MergeFullGrids()
        {
            _root.MergeFullGrids();
        }

        abstract class AbstractGrid
        {
            protected T _defaultValue;
            protected int _left, _top, _width, _height;
            protected int _centerx, _centery;

            public AbstractGrid(int left, int top, int width, int height,
                T defaultValue)
            {
                _left = left;
                _top = top;
                _width = width;
                _height = height;
                _centerx = _left + _width / 2;
                _centery = _top + _height / 2;
                _defaultValue = defaultValue;
            }

            public int Left { get { return _left; } }
            public int Top { get { return _top; } }
            public int Width { get { return _width; } }
            public int Height { get { return _height; } }

            public bool Contains(int x, int y)
            {
                return (x >= _left && y >= _top &&
                    x < _left + _width && y < _top + _height);
            }

            public abstract T GetValue(int x, int y);
            public abstract void SetValue(int x, int y, T value);
            public abstract bool IsFull { get; }
            public abstract void CopyToGrid(T[] grid, int x, int y, int gridWidth, int gridHeight);
            public abstract AbstractGrid MergeFullGrids();
        }

        class VirtualGrid : AbstractGrid
        {
            const int QuadrantTopRight = 0;
            const int QuadrantTopLeft = 1;
            const int QuadrantBottomLeft = 2;
            const int QuadrantBottomRight = 3;

            AbstractGrid[] _quadrants = new AbstractGrid[4];

            public VirtualGrid(int left, int top, int width, int height,
                T defaultValue)
                : base(left, top, width, height, defaultValue)
            {
            }

            public void SetQuadrant(AbstractGrid grid)
            {
                int index;
                if (grid.Top == _top)
                {
                    if (grid.Left == _left)
                        index = QuadrantTopLeft;
                    else if (grid.Left == _centerx)
                        index = QuadrantTopRight;
                    else throw new ApplicationException("Unexpected grid coordinates during SetQuadrant");
                }
                else if (grid.Top == _centery)
                {
                    if (grid.Left == _left)
                        index = QuadrantBottomLeft;
                    else if (grid.Left == _centerx)
                        index = QuadrantBottomRight;
                    else throw new ApplicationException("Unexpected grid coordinates during SetQuadrant");
                }
                else throw new ApplicationException("Unexpected grid coordinates during SetQuadrant");
                _quadrants[index] = grid;
            }

            public override T GetValue(int x, int y)
            {
                int index;
                if (y < _centery)
                {
                    if (x < _centerx)
                        index = QuadrantTopLeft;
                    else
                        index = QuadrantTopRight;
                }
                else
                {
                    if (x < _centerx)
                        index = QuadrantBottomLeft;
                    else
                        index = QuadrantBottomRight;
                }
                AbstractGrid grid = _quadrants[index];
                if (grid == null)
                    return _defaultValue;
                return grid.GetValue(x, y);
            }

            public override void SetValue(int x, int y, T value)
            {
                int index;
                if (y < _centery)
                {
                    if (x < _centerx)
                        index = QuadrantTopLeft;
                    else
                        index = QuadrantTopRight;
                }
                else
                {
                    if (x < _centerx)
                        index = QuadrantBottomLeft;
                    else
                        index = QuadrantBottomRight;
                }
                AbstractGrid grid = _quadrants[index];
                if (grid == null)
                {
                    int targetWidth = _width / 2;
                    int targetHeight = _height / 2;
                    int targetLeft = _left;
                    int targetTop = _top;
                    if (index == QuadrantTopRight ||
                        index == QuadrantBottomRight)
                        targetLeft = _centerx;
                    if (index == QuadrantBottomLeft ||
                        index == QuadrantBottomRight)
                        targetTop = _centery;
                    if (targetWidth > MinGridSize)
                        grid = new VirtualGrid(targetLeft, targetTop, targetWidth, targetHeight, _defaultValue);
                    else
                        grid = new RealGrid(targetLeft, targetTop, targetWidth, targetHeight, _defaultValue);
                    _quadrants[index] = grid;
                }
                grid.SetValue(x, y, value);
            }

            public override bool IsFull
            {
                get
                {
                    for (int i = 0; i < _quadrants.Length; i++)
                    {
                        if (_quadrants[i] == null)
                            return false;
                        if (!_quadrants[i].IsFull)
                            return false;
                    }
                    return true;
                }
            }

            public override void CopyToGrid(T[] grid, int x, int y, int gridWidth, int gridHeight)
            {
                _quadrants[QuadrantTopRight].CopyToGrid(grid, x + _width / 2, y, gridWidth, gridHeight);
                _quadrants[QuadrantTopLeft].CopyToGrid(grid, x, y, gridWidth, gridHeight);
                _quadrants[QuadrantBottomLeft].CopyToGrid(grid, x, y + _height / 2, gridWidth, gridHeight);
                _quadrants[QuadrantBottomRight].CopyToGrid(grid, x + _width / 2, y + _height / 2, gridWidth, gridHeight);
            }

            public override AbstractGrid MergeFullGrids()
            {
                if (IsFull)
                {
                    T[] fullGrid = new T[_width * _height];
                    CopyToGrid(fullGrid, 0, 0, _width, _height);
                    return new RealGrid(_left, _top, _width, _height, _defaultValue, fullGrid);
                }
                else
                {
                    for (int i = 0; i < _quadrants.Length; i++)
                        if (_quadrants[i] != null)
                            _quadrants[i] = _quadrants[i].MergeFullGrids();
                    return this;
                }
            }
        }

        class RealGrid : AbstractGrid
        {
            T[] _grid;

            public RealGrid(int left, int top, int width, int height,
                T defaultValue)
                : base(left, top, width, height, defaultValue)
            {
                _grid = new T[width * height];
                for (int i = 0; i < _grid.Length; i++)
                    _grid[i] = defaultValue;
            }

            public RealGrid(int left, int top, int width, int height,
                T defaultValue, T[] grid) : base(left, top, width, height, defaultValue)
            {
                _grid = grid;
            }

            public override void CopyToGrid(T[] grid, int x, int y, int gridWidth, int gridHeight)
            {
                for (int i = 0; i < _height; i++)
                    Array.Copy(_grid, i * _width, grid, (y + i) * gridHeight + x, _width);
            }

            public override T GetValue(int x, int y)
            {
                return _grid[(y - _top) * _width + x - _left];
            }

            public override void SetValue(int x, int y, T value)
            {
                _grid[(y - _top) * _width + x - _left] = value;
            }

            public override bool IsFull
            {
                get { return true; }
            }

            public override AbstractGrid MergeFullGrids()
            {
                return this;
            }
        }
    }
}
