using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthDimension.Utilities
{
    public static class Utils
    {
        public static void Swap<T>(ref T a, ref T b)
        {
            T t = a;
            a = b;
            b = t;
        }

        public static int ToUnixTimestamp(this DateTime dt)
        {
            return (int)(dt.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
        }

        public static bool IsValidEmail(string email)
        {
            bool result;
            try
            {
                System.Net.Mail.MailAddress mailAddress = new System.Net.Mail.MailAddress(email);
                result = (mailAddress.Address == email);
            }
            catch
            {
                result = false;
            }
            return result;
        }

        public static T Convert<T>(this string value)
        {
            return (T)((object)System.ComponentModel.TypeDescriptor.GetConverter(typeof(T)).ConvertFrom(value));
        }

        public static T[] FromCSV<T>(this string csv)
        {
            return (from value in csv.Split(new char[]
            {
                ','
            })
                    select value.Trim().Convert<T>()).ToArray<T>();
        }
    }
}
