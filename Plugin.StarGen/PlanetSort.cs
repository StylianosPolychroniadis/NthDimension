using System.Collections;

namespace Plugin.StarGen
{
    public enum SortDirection
    {
        // Summary:
        //     Sort from smallest to largest. For example, from A to Z.
        Ascending = 0,
        //
        // Summary:
        //     Sort from largest to smallest. For example, from Z to A.
        Descending = 1,
    }

    public sealed class PlanetSort : IComparer
    {
        private SortDirection m_direction = SortDirection.Ascending;

        public PlanetSort() : base() { }

        /// <summary>
        /// Can be used for both planet and protoplanet sorting by average distance from star.
        /// This replaced unclear code in Protoplanet.coalesce_planetesimals
        /// </summary>
        public PlanetSort(SortDirection direction)
        {
            this.m_direction = direction;
        }

        int IComparer.Compare(object x, object y)
        {
            if ((x is Protoplanet) && (y is Protoplanet))
            {
                Protoplanet p1 = (Protoplanet)x;
                Protoplanet p2 = (Protoplanet)y;

                if (p1 == null && p2 == null)
                {
                    return 0;
                }
                else if (p1 == null && p2 != null)
                {
                    return (this.m_direction == SortDirection.Ascending) ? -1 : 1;
                }
                else if (p1 != null && p2 == null)
                {
                    return (this.m_direction == SortDirection.Ascending) ? 1 : -1;
                }
                else
                {
                    if (p1.a > 0 && p2.a > 0)
                    {
                        return (this.m_direction == SortDirection.Ascending) ?
                         p1.a.CompareTo(p2.a) :
                         p2.a.CompareTo(p1.a);
                    }
                    else
                    {
                        return 0;
                    }
                }
            }
            else if ((x is Planet) && (y is Planet))
            {
                Planet p1 = (Planet)x;
                Planet p2 = (Planet)y;

                if (p1 == null && p2 == null)
                {
                    return 0;
                }
                else if (p1 == null && p2 != null)
                {
                    return (this.m_direction == SortDirection.Ascending) ? -1 : 1;
                }
                else if (p1 != null && p2 == null)
                {
                    return (this.m_direction == SortDirection.Ascending) ? 1 : -1;
                }
                else
                {
                    if (p1.a > 0 && p2.a > 0)
                    {
                        return (this.m_direction == SortDirection.Ascending) ?
                         p1.a.CompareTo(p2.a) :
                         p2.a.CompareTo(p1.a);
                    }
                    else
                    {
                        return 0;
                    }
                }
            }
            else
            {
                return 0;
            }
        }
    }
}
