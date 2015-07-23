using UnityEngine.EventSystems;
using System.Collections.Generic;

namespace Utilities
{
    public class Comparer
    {
        public enum OPERATOR
        {
            LT,
            LE,
            GT,
            GE,
            EQ,
            NE,
        }

        public static bool Compare(int t1, OPERATOR op, int t2)
        {
            switch (op)
            {
                case OPERATOR.LT:
                    return t1 < t2;
                case OPERATOR.LE:
                    return t1 <= t2;
                case OPERATOR.GT:
                    return t1 > t2;
                case OPERATOR.GE:
                    return t1 >= t2;
                case OPERATOR.EQ:
                    return t1 == t2;
                case OPERATOR.NE:
                    return t1 != t2;
            }

            return false;
        }
    }

    public class EventTriggerTypeComparer : IComparer<EventTriggerType>
    {
        private static EventTriggerTypeComparer _instance;

        private EventTriggerTypeComparer() { }

        public static EventTriggerTypeComparer Get()
        {
            if (_instance == null)
                _instance = new EventTriggerTypeComparer();

            return _instance;
        }

        public int Compare(EventTriggerType f, EventTriggerType s)
        {
            return ((int)f) - ((int)s);
        }
    }

    public class StringComparer : IComparer<string>
    {
        private static StringComparer mInstance;

        private StringComparer() { }

        public static StringComparer Get()
        {
            if (mInstance == null)
                mInstance = new StringComparer();

            return mInstance;
        }

        public int Compare(string f, string s)
        {
            return f.CompareTo(s);
        }
    }

    public class Vec2IntComparer : IComparer<Vec2Int>
    {
        private static Vec2IntComparer _instance;

        public static Vec2IntComparer Get()
        {
            if (_instance == null)
                _instance = new Vec2IntComparer();

            return _instance;
        }

        public int Compare(Vec2Int x, Vec2Int y)
        {
            if (x.x < y.x)
                return -1;
            else if (x.x > y.x)
                return 1;
            else if (x.y < y.y)
                return -1;
            else if (x.y > y.y)
                return 1;
            else
                return 0;
        }
    }

}