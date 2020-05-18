using System;
using System.Collections.Generic;
using System.Linq;
using Json;
using NthDimension.Collections;
using NthDimension.Utilities;

namespace NthDimension.Procedural.Dungeon
{
    public static class JsonLevel
    {
        struct LevelTileComparer : IEqualityComparer<LevelTile>
        {
            public bool Equals(LevelTile x, LevelTile y)
            {
                return x.TileType == y.TileType && x.Region == y.Region && x.Object == y.Object;
            }

            public int GetHashCode(LevelTile obj)
            {
                int code = (int)obj.TileType.Id;
                if (obj.Region != null)
                    code = code * 7 + obj.Region.GetHashCode();
                if (obj.Object != null)
                    code = code * 13 + obj.Object.GetHashCode();
                return code;
            }
        }

        public static LevelTile[,] Load(string json)
        {
            var map = (JsonObject)SimpleJson.DeserializeObject(json);
            uint w = (uint)(long)map["width"], h = (uint)(long)map["height"];
            var result = new LevelTile[w, h];

            var tiles = new Dictionary<ushort, LevelTile>();
            ushort id = 0;
            foreach (JsonObject tile in (JsonArray)map["dict"])
            {
                var mapTile = new LevelTile();
                var tileType = (string)tile.GetValueOrDefault("ground", "Space");
                mapTile.TileType = new LevelTileType(tileType == "Space" ? 0xfe : (uint)tileType.GetHashCode(), tileType);

                mapTile.Region = tile.ContainsKey("regions") ? (string)((JsonObject)((JsonArray)tile["regions"])[0])["id"] : null;
                if (tile.ContainsKey("objs"))
                {
                    var obj = (JsonObject)((JsonArray)tile["objs"])[0];
                    var tileObj = new LevelObject();
                    tileObj.ObjectType = new LevelObjectType((uint)((string)obj["id"]).GetHashCode(), (string)obj["id"]);
                    if (obj.ContainsKey("name"))
                    {
                        var attrs = (string)obj["name"];
                        tileObj.Attributes = attrs.Split(';')
                            .Where(attr => !string.IsNullOrEmpty(attr))
                            .Select(attr => attr.Split(':'))
                            .Select(attr => new KeyValuePair<string, string>(attr[0], attr[1]))
                            .ToArray();
                    }
                    else
                        tileObj.Attributes = Empty<KeyValuePair<string, string>>.Array;

                    mapTile.Object = tileObj;
                }
                else
                    mapTile.Object = null;
                tiles[id++] = mapTile;
            }

            byte[] data = Zlib.Decompress(Convert.FromBase64String((string)map["data"]));
            int index = 0;
            for (int y = 0; y < h; y++)
                for (int x = 0; x < w; x++)
                {
                    result[x, y] = tiles[(ushort)((data[index++] << 8) | data[index++])];
                }

            return result;
        }

        public static string Save(LevelTile[,] map)
        {
            int w = map.GetUpperBound(0) + 1, h = map.GetUpperBound(1) + 1;

            var tiles = new JsonArray();
            var indexLookup = new Dictionary<LevelTile, short>(new LevelTileComparer());
            byte[] data = new byte[w * h * 2];
            int ptr = 0;

            for (int y = 0; y < h; y++)
                for (int x = 0; x < w; x++)
                {
                    var tile = map[x, y];
                    short index;
                    if (!indexLookup.TryGetValue(tile, out index))
                    {
                        indexLookup.Add(tile, index = (short)tiles.Count);
                        tiles.Add(tile);
                    }
                    data[ptr++] = (byte)(index >> 8);
                    data[ptr++] = (byte)(index & 0xff);
                }

            for (int i = 0; i < tiles.Count; i++)
            {
                var tile = (LevelTile)tiles[i];

                var jsonTile = new JsonObject();
                jsonTile["ground"] = tile.TileType.Name;
                if (!string.IsNullOrEmpty(tile.Region))
                {
                    var region = new JsonObject {
                        { "id", tile.Region }
                    };
                    jsonTile["regions"] = new JsonArray { region };
                }
                if (tile.Object != null)
                {
                    var obj = new JsonObject {
                        { "id", tile.Object.ObjectType.Name }
                    };
                    if (tile.Object.Attributes.Length > 0)
                    {
                        var objAttrs = tile.Object.Attributes.Select(kvp => kvp.Key + ":" + kvp.Value).ToArray();
                        obj["name"] = string.Join(",", objAttrs);
                    }
                    jsonTile["objs"] = new JsonArray { obj };
                }

                tiles[i] = jsonTile;
            }

            var mapObj = new JsonObject();
            mapObj["width"] = w;
            mapObj["height"] = h;
            mapObj["dict"] = tiles;
            mapObj["data"] = Convert.ToBase64String(Zlib.Compress(data));

            return mapObj.ToString();
        }
    }
}
