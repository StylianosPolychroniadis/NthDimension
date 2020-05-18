//#define PROCEED_TIME 0.01f
//#define PROCEED_TIME_STEP 0.0001f
//#define NB_AGENTS 2000

//#define RENDER_WATER				0x0001
//#define RENDER_LANDSCAPE			0x0002
//#define RENDER_TEXTUREDLANDSCAPE	0x0004
//#define RENDER_ROADS				0x0008
//#define RENDER_ROADS_GEO			0x0010
//#define RENDER_BLOCKS				0x0020
//#define RENDER_ALLOTMENTS			0x0040
//#define RENDER_BUILDINGS			0x0080
//#define RENDER_AGENTS				0x0100

using System;

namespace NthDimension.Procedural.City
{
    using NthDimension.Procedural.MockTBD;

    public class City   // CityRenderer to be later refactored to CityBlockRenderer
    {
        public class Renderer { }

        //private Environment                     m_environment;
        private Density                         m_density;
        private RoadNetwork                     m_roadNetwork;
        private Geometry                        m_roadGeometry;
        private BlockSet                        m_blocks;
        private BlockSet                        m_allotments;
        private BuildingSet                     m_buildings;
        private CityGenerator[]                     m_pipeline;
        //std::vector<Generator*> m_pipeline;
        //std::vector<unsigned> m_renderStates;
        private uint                            m_actStep;
        //private AgentSet                        m_agentSet;
        //// variables d'affichage
        //private MediaManager                    m_mediaManager;
        //CommandFrame*                         m_interface;                    // Similar to our WHUD functionality
        private bool                            m_run;
        private bool                            m_stopAtNextStep;

        const float                             PROCEED_TIME                = 0.01f;
        const float                             PROCEED_TIME_STEP           = 0.0001f;
        const int                               NB_AGENTS                   = 2000;
        const uint                              RENDER_WATER                = 0x0001;
        const uint                              RENDER_LANDSCAPE            = 0x0002;
        const uint                              RENDER_TEXTUREDLANDSCAPE    = 0x0004;
        const uint                              RENDER_ROADS                = 0x0008;
        const uint                              RENDER_ROADS_GEO            = 0x0010;
        const uint                              RENDER_BLOCKS               = 0x0020;
        const uint                              RENDER_ALLOTMENTS           = 0x0040;
        const uint                              RENDER_BUILDINGS            = 0x0080;
        const uint                              RENDER_AGENTS               = 0x0100;

        public City(Density density)
        {
            
        }

        public void Dispose()
        {
            //while (m_pipeline.Length > 0)
            //{
            //    if (m_pipeline[m_pipeline.Length - 1] != null)
            //    {
            //        m_pipeline[m_pipeline.Length - 1] = null;
            //    }
            //    //m_pipeline.RemoveAt(m_pipeline.Count - 1);
            //}
        }

        public virtual void render(Rendering.enuGameRenderPass pass, Rendering.GameViews.ViewInfo view)
        {

            switch(pass)
            {
                case Rendering.enuGameRenderPass.transparent:
                    Rendering.ApplicationBase.Instance.Renderer.BlendEnabled = true;

                    break;
            }

            throw new NotImplementedException();
        }
        public virtual void update(float timeElapsed, Action<string> uiCallback = null)
        {
            if (m_run && m_actStep < m_pipeline.Length)// .size())
            {
                m_pipeline[m_actStep].proceed(PROCEED_TIME);

                if (m_pipeline[m_actStep].isFinished())
                {
                    ++m_actStep;

                    if (m_actStep >= m_pipeline.Length/*.size()*/ || m_stopAtNextStep)
                    {
                        m_run = false;
                    }

                   // m_interface->setCurrentStage(m_actStep);                  // TODO:: uiCallback

                    if (m_actStep >= m_pipeline.Length/*.size()*/)
                    {
                  //      m_interface->setMode(CFM_SIMULATION);
                    }
                }
            }
            else if (m_run)
            {
                //// lancement du moteur de simulation
                //m_agentSet.run(timeElapsed);
                //// m_interface->setAverageSpeed(m_agentSet.getAverageVelocity());
            }
        }

        public virtual void runAll() { }
        public virtual void run() { }
        public virtual void runStep() { }
        public virtual void reset() { }

        public virtual void setupAgents() { }
        public virtual void startSimulation() { }
        public virtual void stopSimulation() { }
        public virtual void showBottlenecks(bool show) { }
    }
}
