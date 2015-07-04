using UnityEngine;
using System.Collections.Generic;

public abstract class DynamicSprite
{
	#region Instance Variables
    protected List<Color> _data;
    protected Texture2D _tex;
    protected Sprite _sprite;
    protected Utilities.Vec2Int _size;
	#endregion

    public DynamicSprite(Utilities.Vec2Int size)
    {
        _size = new Utilities.Vec2Int(1, 1);
        if (size.x > 1)
            _size.x = size.x;
        if (size.y > 1)
            _size.y = size.y;

        _sprite = null;
        _data = new List<Color>(size.x * size.y);
        for (int i = 0; i < _data.Capacity; i++)
            _data.Add(default(Color));
    }
	
	#region Interface Methods
    public Utilities.Vec2Int size()
    {
        return _size.clone();
    }

    public void resize(Utilities.Vec2Int size)
    {
        if (_size.Equals(size))
            return;

        if (size.x > 1)
            _size.x = size.x;
        if (size.y > 1)
            _size.y = size.y;

        if (_data.Capacity < size.x * size.y)
            _data.Capacity = size.x * size.y;

        for (int i = _data.Count; i < _size.x * _size.y; i++)
            _data.Add(default(Color));
    }

    public Sprite apply()
    {
        _tex = new Texture2D(_size.x, _size.y);
        _tex.SetPixels(_data.ToArray());
        _tex.Apply();

        _sprite = Sprite.Create(_tex, new Rect(0, 0, _size.x, _size.y), new Vector2(0.5f, 0.5f));

        return _sprite;
    }

    public Sprite get()
    {
        return _sprite;
    }
	#endregion
}
