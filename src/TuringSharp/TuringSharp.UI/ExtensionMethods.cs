using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TuringSharp.UI
{
    public static class ExtensionMethods
    {

        /// <summary>
        /// Trim the string counting the number of characters removed from each side.
        /// </summary>
        public static string TrimWithCount(this string s, out int removedStart, out int removedEnd)
        {
            // Trim start
            var ts = s.TrimStart();
            removedStart = s.Length - ts.Length;

            ts = ts.TrimEnd(); // Reuse the previously created string without allocating a new one
            removedEnd = s.Length - ts.Length + removedStart;

            return ts;
        }

    }
}
