using NthDimension.Rendering.Drawables;
using NthDimension.Rendering.Drawables.Models;

namespace NthStudio.Game.City
{
    public class CityDrawable : Drawable
    {
        private NthDimension.Rendering.Drawables.Models.Terrain     m_terrain;
        private NthDimension.Procedural.City.City                   m_city;
        private NthDimension.Procedural.City.Density                m_density;

        public CityDrawable(Terrain terrain)
        {
            this.m_terrain      = terrain;
            this.m_density      = new NthDimension.Procedural.City.Density(terrain);            
            this.m_city         = new NthDimension.Procedural.City.City(this.m_density);
        }
            


        public override void Update()
        {
            m_city.update(NthStudio.StudioWindow.Instance.GetElapsedTime());
            base.Update();
        }

    }
}
