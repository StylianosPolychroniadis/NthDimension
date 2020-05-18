using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;

namespace NthDimension
{
    public class angleXYZ
    {
        public angleXYZ()
        {
            this.X = 0;
            this.Y = 0;
            this.Z = 0;
        }
        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }
    }

    // TODO>> Use for smooth transformations
    public class EasingTimer : System.Timers.Timer
    {
        private static EasingTimer t;

        public bool _run = false;
        public DateTime begin_time;
        public int duration = 500;
        public int firstAngle = 0;
        public int rotateN = 0;
        public ElapsedEventHandler func;
        public enum rotateAxisType { X, Y, Z };
        public rotateAxisType rotateAxis;
        public int dir;

        public delegate void TimerFunction(object sender, ElapsedEventArgs e);
        public delegate void timerUPFunction();
        public timerUPFunction funcUp;

        private EasingTimer(int n, TimerFunction func, List<angleXYZ> angles, int dir, rotateAxisType rotateAxis, timerUPFunction funcUp = null)
            : base(100)
        {
            
            this.firstAngle = 0;
            this.rotateN = n;
            this.begin_time = DateTime.Now;
            this.func = new ElapsedEventHandler(func);
            this.Elapsed += this.func;
            this._run = false;
            this.dir = dir;
            this.rotateAxis = rotateAxis;
            this.funcUp = funcUp;
        }
        private EasingTimer(int n, TimerFunction func, List<angleXYZ> angles, int dir, rotateAxisType rotateAxis, timerUPFunction funcUp = null, int interval = 100)
            : base(interval)
        {
            this.firstAngle = 0;
            this.rotateN = n;
            this.begin_time = DateTime.Now;
            this.func = new ElapsedEventHandler(func);
            this.Elapsed += this.func;
            this._run = false;
            this.dir = dir;
            this.rotateAxis = rotateAxis;
            this.funcUp = funcUp;
        }

        public static void rotate(int n, TimerFunction func, List<angleXYZ> angles, int dir, rotateAxisType rotateAxis, timerUPFunction funcUp = null)
        {
            if (t == null)
            {
                //MessageBox.Show("firstAngle = " + firstAngle.ToString());
                t = new EasingTimer(n, func, angles, dir, rotateAxis, funcUp);
                t.rotateAxis = rotateAxis;
            }
            else
            {
                //если прокручивание работает, то просто возвращаем таймер
                //если прокручинваие уже не работает, то заново инициализируем переменные, как бы вызываем конструктор
                if (!t._run)
                {
                    //MessageBox.Show("2: firstAngle = " + firstAngle.ToString());
                    t.firstAngle = 0;
                    t.rotateN = n;
                    t.begin_time = DateTime.Now;
                    t.Elapsed -= t.func;
                    t.func = new ElapsedEventHandler(func);
                    t.Elapsed += t.func;
                    t.dir = dir;
                    t.rotateAxis = rotateAxis;
                    t.funcUp = funcUp;
                }
            }

            //Если уже пошёл процесс кручения, то выходим
            if (t._run)
            {
                return;
            }

            t._run = true;
            t.Start();
        }

        public bool run
        {
            get
            {
                return _run;
            }

            set
            {
                bool prev_run = this._run;
                this._run = value;
                if (prev_run && !value)
                {
                    if (this.funcUp != null)
                    {
                        this.funcUp();
                    }
                }
            }
        }
    }
}
