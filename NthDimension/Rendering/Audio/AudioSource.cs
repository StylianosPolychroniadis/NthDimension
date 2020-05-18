using NthDimension.Algebra;
using OpenTK.Audio;

namespace NthDimension.Audio
{
    public class AudioSource : System.IDisposable
    {
        internal readonly AudioContext _context;
        internal readonly int _buffer;
        internal readonly int _source;

        private bool m_loop = false;
        public bool Loop
        {
            get { return m_loop; }
            set { m_loop = value; Repeat(value); }
        }

        public AudioSource()
        {
            _context = new AudioContext();
            _buffer = Rendering.ApplicationBase.Instance.Audio.GenBuffer();
            _source = Rendering.ApplicationBase.Instance.Audio.GenSource();
        }

        public void Dispose()
        {
            this.Release();
        }

        public virtual void Play() { }
        public virtual void Repeat(bool enabled) { }
        public virtual void Pause() { }
        public virtual void Stop() { }
        public virtual void Release() { }
        public virtual void SetGainSource(float gain) { }
        public virtual void SetPositionSource(Vector3 position) { }
        public virtual void SetPositionListener(Vector3 position) { }
        public virtual void SetVelocitySource(Vector3 velocity) { }
        public virtual void SetVelocityListener(Vector3 velocity) { }
    }
}
