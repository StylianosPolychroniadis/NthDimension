namespace NthStudio.Compiler.Scripting
{
    public delegate void ReferencedAssembliesChanged();

    public class ReferencedAssemblyCollection : System.Collections.ArrayList
    {
        internal event ReferencedAssembliesChanged      ReferencedAssembliesChanged;

        private void                                    FireChangedEvent()
        {
            if (ReferencedAssembliesChanged != null)
            {
                ReferencedAssembliesChanged();
            }
        }

        public override void                            Clear()
        {
            base.Clear();
            FireChangedEvent();
        }

        public string[]                                 ToStringArray()
        {
            string[] array = new string[this.Count];
            int i = 0;

            foreach (string str in this)
            {
                array[i++] = str;
            }

            return array;
        }

        public override int                             Add(object value)
        {
            int index = base.Add(value);
            FireChangedEvent();

            return index;
        }

        public override void                            AddRange(System.Collections.ICollection c)
        {
            base.AddRange(c);
            FireChangedEvent();
        }

        public override void                            Insert(int index, object value)
        {
            base.Insert(index, value);
            FireChangedEvent();
        }

        public override void                            InsertRange(int index, System.Collections.ICollection c)
        {
            base.InsertRange(index, c);
            FireChangedEvent();
        }

        public override void                            Remove(object obj)
        {
            base.Remove(obj);
            FireChangedEvent();
        }

        public override void                            RemoveAt(int index)
        {
            base.RemoveAt(index);
            FireChangedEvent();
        }

        public override void                            RemoveRange(int index, int count)
        {
            base.RemoveRange(index, count);
            FireChangedEvent();
        }

        public override object                          this[int index]
        {
            get
            {
                return base[index];
            }
            set
            {
                base[index] = value;
                FireChangedEvent();
            }
        }
    }
}
