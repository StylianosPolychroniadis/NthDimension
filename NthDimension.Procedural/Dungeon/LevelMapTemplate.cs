using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NthDimension.Algebra;

namespace NthDimension.Procedural.Dungeon
{
    public abstract class LevelMapTemplate
    {
        protected Random Rand { get; private set; }

        internal void SetRandom(Random rand)
        {
            Rand = rand;
        }

        public abstract int MaxDepth { get; }
        public abstract NormDist TargetDepth { get; }
        public virtual Range NumRoomRate { get { return new Range(3, 5); } }

        public abstract NormDist SpecialRmCount { get; }
        public abstract NormDist SpecialRmDepthDist { get; }

        public abstract int CorridorWidth { get; }
        public abstract Range RoomSeparation { get; }

        public virtual void Initialize()
        {
        }

        public abstract LevelRoom CreateStart(int depth);
        public abstract LevelRoom CreateTarget(int depth, LevelRoom prev);
        public abstract LevelRoom CreateSpecial(int depth, LevelRoom prev);
        public abstract LevelRoom CreateNormal(int depth, LevelRoom prev);

        public virtual void InitializeRasterization(LevelGraph graph)
        {
        }

        public virtual LevelMapRender CreateBackground()
        {
            return new LevelMapRender();
        }

        public virtual LevelMapRender CreateOverlay()
        {
            return new LevelMapRender();
        }

        public virtual LevelMapCorridor CreateCorridor()
        {
            return new LevelMapCorridor();
        }


        protected static LevelTile[,] ReadTemplate(Type templateType)
        {
            var templateName = templateType.Namespace + ".template.jm";
            var stream = templateType.Assembly.GetManifestResourceStream(templateName);
            using (var reader = new StreamReader(stream))
                return JsonLevel.Load(reader.ReadToEnd());
        }
    }


}
