using UnityEngine;
using SIDE = Utilities.SIDE_4;

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
    public void attach(QuadNode<T> node, SIDE side)
    {
        rawAttach(node, side);
        node.rawAttach(this, Utilities.flip(side));
    }

    public void detach(SIDE side)
    {
        QuadNode<T> tmp = getSide(side);

        if (tmp != null)
        {
            rawAttach(null, side);
            tmp.rawAttach(null, Utilities.flip(side));
        }
    }
    
    public QuadNode<T> getSide(SIDE side)
    {
        switch (side)
        {
            case SIDE.RIGHT:
                return _right;
            case SIDE.TOP:
                return _top;
            case SIDE.LEFT:
                return _left;
            case SIDE.BOTTOM:
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
    private void rawAttach(QuadNode<T> node, SIDE side)
    {
        switch (side)
        {
            case SIDE.RIGHT:
                _right = node;
                break;
            case SIDE.TOP:
                _top = node;
                break;
            case SIDE.LEFT:
                _left = node;
                break;
            case SIDE.BOTTOM:
                _bottom = node;
                break;
        }
    }
	#endregion
}
