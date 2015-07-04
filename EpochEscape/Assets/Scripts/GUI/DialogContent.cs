using UnityEngine;
using UnityEngine.UI;

public class DialogContent : MonoBehaviour
{
	#region Interface Variables
    public RectTransform rectTransform;
    public Image background;
	#endregion
	
	#region Instance Variables
    protected DynamicMonochromeSprite _background;
    protected Utilities.Vec2Int _size;

    private bool didBackgroundUpdate;
	#endregion
	
    protected void Awake()
    {
        _size = new Utilities.Vec2Int(100, 100);
        _background = new DynamicMonochromeSprite(_size);

        didBackgroundUpdate = false;

        //rectTransform.sizeDelta = Utilities.toVector2(_size);
        /*rectTransform.position = Vector3.zero;
        rectTransform.pivot = new Vector2(0.5f, 0.5f);*/
    }
	
	protected void Update ()
	{
	    if (didBackgroundUpdate)
        {
            Sprite sprite = _background.apply();

            if (sprite != null)
                background.sprite = sprite;
        }
	}
	
	#region Interface Methods
    public void size(Utilities.Vec2Int size)
    {
        didBackgroundUpdate = true;

        _background.resize(size);
        _size = _background.size();
    }
    public Utilities.Vec2Int size()
    {
        return _size;
    }

    public void backgroundColor(Color color)
    {
        didBackgroundUpdate = true;

        _background.baseColor(color);
    }
    public Color backgroundColor()
    {
        return _background.baseColor();
    }
	#endregion
	
	#region Instance Methods
	#endregion
}
