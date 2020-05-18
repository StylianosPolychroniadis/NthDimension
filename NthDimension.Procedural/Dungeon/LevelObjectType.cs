using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthDimension.Procedural.Dungeon
{
    public struct LevelObjectType
    {
        public readonly uint Id;
        public readonly string Name;

        public LevelObjectType(uint id, string name)
        {
            Id = id;
            Name = name;
        }

        public static bool operator ==(LevelObjectType a, LevelObjectType b)
        {
            return a.Id == b.Id || a.Name == b.Name;
        }

        public static bool operator !=(LevelObjectType a, LevelObjectType b)
        {
            return a.Id != b.Id && a.Name != b.Name;
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return obj is LevelObjectType && (LevelObjectType)obj == this;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
