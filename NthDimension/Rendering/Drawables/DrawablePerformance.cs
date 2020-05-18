
namespace NthDimension.Rendering.Drawables
{
    using System;
    public sealed class DrawablePerformanceMetric
    {
        private long diffuseAvg         = 0L;
        private long normalAvg          = 0L;
        private long shadowAvg          = 0L;
        private long deferredAvg        = 0L;
        private long transparentAvg     = 0L;
        private long selectionAvg       = 0L;

        private long diffuseAcc = 0L;
        private long normalAcc = 0L;
        private long shadowAcc = 0L;
        private long deferredAcc = 0L;
        private long transparentAcc = 0L;
        private long selectionAcc = 0L;

        private long diffuseSamples     = 0L;
        private long normalSamples      = 0L;
        private long shadowSamples      = 0L;
        private long deferredSamples    = 0L;
        private long transparentSamples = 0L;
        private long selectionSamples   = 0L;

        public long DiffusePassTime                     {   get { return diffuseAvg; }
                                                            /*set { diffuseAcc += value;      diffuseSamples++;       diffuseAvg = diffuseAcc / diffuseSamples; }*/ }
        public long NormalPassTime                      {   get { return normalAvg; }
                                                            /*set { normalAcc += value;       normalSamples++;        normalAvg = normalAcc / normalSamples; }*/ }
        public long ShadowPassTime                      {   get { return shadowAvg; }
                                                            /*set { shadowAcc += value;       shadowSamples++;        shadowAvg = shadowAcc / shadowSamples; }*/ }
        public long DeferredInfoPassTime                {   get { return deferredAvg; }
                                                            /*set { deferredAcc += value;     deferredSamples++;      deferredAvg = deferredAcc / deferredSamples; }*/ }
        public long TransparentPassTime                 {   get { return transparentAvg; }
                                                            /*set { transparentAcc += value;  transparentSamples++;   transparentAvg = transparentAcc / transparentSamples; }*/ }
        public long SelectionPassTime                   {   get { return selectionAvg; }
                                                            /*set { selectionAcc += value;    selectionSamples++;     selectionAvg = selectionAcc / selectionSamples; }*/ }

        public long DiffusePassTimePrevious = 0L;
        public long NormalPassTimePrevious = 0L;
        public long ShadowPassTimePrevious = 0L;
        public long DeferredInfoPassTimePrevious = 0L;
        public long TransparentPassTimePrevious = 0L;
        public long SelectionPassTimePrevious = 0L;

        public long DrawTimeAllPasses
        {
            get { return DiffusePassTime + NormalPassTime + ShadowPassTime + DeferredInfoPassTime + TransparentPassTime + SelectionPassTime; }
        }

        public long DrawTimeAllPassesPrevious = 0L;

        public void SetDiffusePassTime(long time)
        {
            diffuseAcc += time;
            diffuseSamples++;
            diffuseAvg = diffuseAcc / diffuseSamples;
        }
        public void SetNormalPassTime(long time)
        {
            normalAcc += time;
            normalSamples++;
            normalAvg = normalAcc / normalSamples;
        }
        public void SetShadowPassTime(long time)
        {
            shadowAcc += time;
            shadowSamples++;
            shadowAvg = shadowAcc / shadowSamples;
        }
        public void SetDeferredPassTime(long time)
        {
            deferredAcc += time;
            deferredSamples++;
            deferredAvg = deferredAcc / deferredSamples;
        }
        public void SetSelectionPassTime(long time)
        {
            selectionAcc += time;
            selectionSamples++;
            selectionAvg = selectionAcc / selectionSamples;
        }
        public void SetTransparentPassTime(long time)
        {
            transparentAcc += time;
            transparentSamples++;
            transparentAvg = transparentAcc / transparentSamples;
        }
        

        // FILTER PASSES (Quad2D)
        //public long SSAOPassTime { get; set; }
        //public long ReflectionPassTime { get; set; }
        //public long LightmapSmoothingPassTime { get; set; }
        //public long DepthOfFieldPassTime { get; set; }
    }
}
