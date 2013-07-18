using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Omnom_III_Game.util {
    public class ListUtil {
        public static void removeAllFromList<T>(List<T> source, ICollection<T> toRemove) {
            foreach (T item in toRemove) {
                source.Remove(item);
            }
        }
    }
}
