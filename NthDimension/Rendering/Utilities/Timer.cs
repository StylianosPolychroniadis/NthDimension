using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthDimension
{
    public class Timer
    {
        static Stopwatch stopw = new Stopwatch();
        static double tiempoPorFrame = 0;
        static double startTime = 0;
        static int contadorFps = 0;
        static int _Fps = 0;
        static double TiempoPromedio_ = 0;
        static int numPromedio = 10;
        static double tiempoTemp = 0;
        static double acumTiempoPro = 0;
        static double factorPromedio = 10;
        /// <summary>
        /// 
        /// </summary>
        static Timer()
        {
            stopw.Start();
        }
        /// <summary>
        /// 
        /// </summary>
        public static int Fps
        {
            get { return _Fps; }
        }
        /// <summary>
        /// Tiempo transcurrido en segundos desde el inicio de la aplicación.
        /// </summary>
        public static double ElapsedTime
        {
            get { return stopw.Elapsed.TotalSeconds; }
        }
        /// <summary>
        /// Tiempo promediado en segundos de la duración del renderizado de un frame.
        /// </summary>
        public static double AverageTime
        {
            get { return TiempoPromedio_; }
        }
        /// <summary>
        /// Tiempo en segundos que ha durado el renderizado del frame.
        /// </summary>
        public static double RenderTime
        {
            get { return tiempoPorFrame; }
        }
        /// <summary>
        /// 
        /// </summary>
        public static void StartRenderFrame()
        {
            startTime = stopw.Elapsed.TotalSeconds;
        }
        /// <summary>
        /// 
        /// </summary>
        public static void EndRenderFrame()
        {
            tiempoPorFrame = stopw.Elapsed.TotalSeconds - startTime;
            tiempoTemp += tiempoPorFrame;
            numPromedio++;
            acumTiempoPro += tiempoPorFrame;
            if (numPromedio >= factorPromedio)
            {
                TiempoPromedio_ = acumTiempoPro / numPromedio;
                acumTiempoPro = 0;
                numPromedio = 0;
            }

            // Determina el framerate en fps (frames por segundo)
            if (tiempoTemp >= 1)
            {
                _Fps = contadorFps;
                contadorFps = 0;
                tiempoTemp = 0;
            }
            else
                contadorFps++;
        }
    }
}
