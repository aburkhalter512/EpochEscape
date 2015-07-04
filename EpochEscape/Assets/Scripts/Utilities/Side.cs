namespace Utilities
{
    public class Side
    {

        public enum SIDE_4
        {
            RIGHT,
            TOP,
            LEFT,
            BOTTOM
        }

        public static SIDE_4 rotateLeft(SIDE_4 side, int turnCount = 1)
        {
            SIDE_4 retVal = side;

            for (int i = 0; i < turnCount; i++)
            {
                switch (retVal)
                {
                    case SIDE_4.RIGHT:
                        retVal = SIDE_4.TOP;
                        break;
                    case SIDE_4.TOP:
                        retVal = SIDE_4.LEFT;
                        break;
                    case SIDE_4.LEFT:
                        retVal = SIDE_4.BOTTOM;
                        break;
                    case SIDE_4.BOTTOM:
                        retVal = SIDE_4.RIGHT;
                        break;
                }
            }

            return retVal;
        }
        public static SIDE_4 rotateRight(SIDE_4 side, int turnCount = 1)
        {
            SIDE_4 retVal = side;

            for (int i = 0; i < turnCount; i++)
            {
                switch (retVal)
                {
                    case SIDE_4.RIGHT:
                        retVal = SIDE_4.BOTTOM;
                        break;
                    case SIDE_4.TOP:
                        retVal = SIDE_4.RIGHT;
                        break;
                    case SIDE_4.LEFT:
                        retVal = SIDE_4.TOP;
                        break;
                    case SIDE_4.BOTTOM:
                        retVal = SIDE_4.LEFT;
                        break;
                }
            }

            return retVal;
        }
        public static SIDE_4 flip(SIDE_4 side)
        {
            return rotateLeft(side, 2);
        }
    }
}
