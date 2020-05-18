using System;

namespace NthStudio.Game.Dungeon
{
    class FixedGrid<T> : IGrid<T>
    {
        int _width;
        int _height;
        T[] _grid;
        T _default;

        public FixedGrid(int width, int height, T defaultValue)
        {
            _default = defaultValue;
            _width = width;
            _height = height;
            _grid = new T[width * height];
            for (int i = 0; i < width * height; i++)
                _grid[i] = defaultValue;
        }

        public T GetValue(int x, int y)
        {
            if (x < 0 || x >= _width || y < 0 || y >= _height)
                return _default;
            else
                return _grid[y * _width + x];
        }

        public void SetValue(int x, int y, T value)
        {
            _grid[y * _width + x] = value;
        }

        public void Clear()
        {
            Array.Clear(_grid, 0, _grid.Length);
        }

        public void CopyFrom(FixedGrid<T> other)
        {
            Array.Copy(other._grid, _grid, _grid.Length);
        }
    }
}
