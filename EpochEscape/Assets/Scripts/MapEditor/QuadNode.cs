using UnityEngine;
using Utilities;

namespace MapEditor
{
    public class QuadNode<T>
    {
        #region Interface Variables
        #endregion

        #region Instance Variables
        T _data;

        QuadNode<T> _right;
        QuadNode<T> _top;
        QuadNode<T> _left;
        QuadNode<T> _bottom;
        #endregion

        public QuadNode()
        {
            _data = default(T);

            _right = null;
            _top = null;
            _left = null;
            _bottom = null;
        }

        public QuadNode(T data)
        {
            _data = data;

            _right = null;
            _top = null;
            _left = null;
            _bottom = null;
        }

        public QuadNode(T data, QuadNode<T> right, QuadNode<T> top, QuadNode<T> left, QuadNode<T> bottom)
        {
            _data = data;

            _right = right;
            _top = top;
            _left = left;
            _bottom = bottom;
        }

        #region Interface Methods
        public void attach(QuadNode<T> node, SIDE_4 side)
        {
            rawAttach(node, side);
            node.rawAttach(this, Side.flip(side));
        }

        public void detach(SIDE_4 side)
        {
            QuadNode<T> tmp = getSide(side);

            if (tmp != null)
            {
                rawAttach(null, side);
                tmp.rawAttach(null, Side.flip(side));
            }
        }

        public QuadNode<T> getSide(SIDE_4 side)
        {
            switch (side)
            {
                case SIDE_4.RIGHT:
                    return _right;
                case SIDE_4.TOP:
                    return _top;
                case SIDE_4.LEFT:
                    return _left;
                case SIDE_4.BOTTOM:
                    return _bottom;
                default:
                    return null; //Impossible to get here
            }
        }

        public T data()
        {
            return _data;
        }
        public T data(T data)
        {
            _data = data;

            return _data;
        }
        #endregion

        #region Instance Methods
        private void rawAttach(QuadNode<T> node, SIDE_4 side)
        {
            switch (side)
            {
                case SIDE_4.RIGHT:
                    _right = node;
                    break;
                case SIDE_4.TOP:
                    _top = node;
                    break;
                case SIDE_4.LEFT:
                    _left = node;
                    break;
                case SIDE_4.BOTTOM:
                    _bottom = node;
                    break;
            }
        }
        #endregion
    }
}
