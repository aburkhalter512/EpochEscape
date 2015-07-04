using UnityEngine;

public class DynamicMonochromeSprite : DynamicSprite
{
    #region Instance Variables
    protected Color _baseColor;
    #endregion

    public DynamicMonochromeSprite(Utilities.Vec2Int size, Color color = default(Color)) : base(size)
    {
        baseColor(color);
    }

    #region Interface Methods
    public void baseColor(Color color)
    {
        _baseColor = color;

        int expandedSize = _size.x * _size.y;
        for (int i = 0; i < expandedSize; i++)
            _data[i] = _baseColor;
    }

    public Color baseColor()
    {
        return _baseColor;
    }
    #endregion
}
