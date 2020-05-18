using System.Collections.Generic;

using NthDimension.Collections;


namespace NthDimension.Procedural.Dungeon
{
    public class LevelObject
    {
        public LevelObjectType                  ObjectType;
        public KeyValuePair<string, string>[]   Attributes = Empty<KeyValuePair<string, string>>.Array;
    }
}
