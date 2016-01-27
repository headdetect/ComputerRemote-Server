using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLI.Utils
{
    public static class Colors
    {

        /// <summary>
        /// Converts a <see cref="Color"/> to a <see cref="ConsoleColor"/>
        /// </summary>
        /// <param name="color">The color.</param>
        /// <returns></returns>
        public static ConsoleColor ToConsoleColor(this Color color)
        {
            var index = (color.R > 128 | color.G > 128 | color.B > 128) ? 8 : 0;
            index |= (color.R > 64) ? 4 : 0;
            index |= (color.G > 64) ? 2 : 0; 
            index |= (color.B > 64) ? 1 : 0;
            return (ConsoleColor) index;
        }
    }
}
