using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utilities
{
    public class Pair<T, U>
    {
        public T first;
        public U second;

        public Pair(T f, U s)
        {
            first = f;
            second = s;
        }
    }
}
