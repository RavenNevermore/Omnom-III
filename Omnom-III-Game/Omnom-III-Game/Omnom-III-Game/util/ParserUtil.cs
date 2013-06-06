using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Omnom_III_Game.util {
    public class ParserUtil {

        public static float toFloat(String value) {
            return float.Parse(value,
                        System.Globalization.CultureInfo.InvariantCulture);
        }

        public static T[] addArrays<T>(params T[][] sources) {
            List<T> buffer = new List<T>();
            
            foreach (T[] array in sources){
                if (null != array)
                    buffer.AddRange(array);
            }
            return buffer.ToArray();
        }
    }
}
