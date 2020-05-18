using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NthDimension.Procedural.City;

namespace NthDimension.Procedural
{
    public class CityGenerator
    {

        public CityGenerator(string name)
        {
            this.m_name = name;
        }

        public void proceed(float time)
        {

        }
        public virtual bool isFinished() // const =0??
        {
            return false;
        }

        public virtual void commitChange()
        {

        }

        // GUI Functions
        //public Widget newControl();
        //public virtual void attachControl(Widget control) { }
        //protected virtual void createParameters(Widget control) { }


        protected virtual void init()
        {

        }
        protected virtual void finish()
        {

        }
        protected virtual void proceed() // =0; ????
        {

        }
       

        private float       m_seed;
        private string      m_name;
        private bool        m_init;
    }
}
