using UnityEngine;

public class ScrollableDialog : MonoBehaviour
{
	#region Interface Variables
    public RectTransform rectTransform;
    public DialogContent content;
	#endregion
	
	#region Instance Variables
    protected Utilities.Vec2Int _size;
	#endregion
	
    protected void Awake()
    {
        _size = new Utilities.Vec2Int(100, 100);
    }

	protected void Start ()
	{
	
	}
	
	protected void Update ()
	{
	
	}
	
	#region Interface Methods
    public void size(Utilities.Vec2Int size)
    {
        if (size.x > 0)
            _size.x = size.x;
        if (size.y > 0)
            _size.y = size.y;

        rectTransform.sizeDelta = Utilities.toVector2(_size);
    }

    public void OnScroll(Vector2 delta)
    {

    }
	#endregion
	
	#region Instance Methods
	#endregion
}
