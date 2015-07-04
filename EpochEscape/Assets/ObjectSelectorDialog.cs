using UnityEngine;

public class ObjectSelectorDialog : MonoBehaviour, IShowable, IToggleable
{
	#region Interface Variables
	#endregion
	
	#region Instance Variables
    bool isVisible;

    ToolManager _tm;
    CoroutineManager _cm;
	#endregion
	
	protected void Start ()
    {
        _tm = ToolManager.Get();
        _cm = CoroutineManager.Get();

        hide();
	}
	
	protected void Update ()
	{
	
	}
	
	#region Interface Methods
    public void toggle()
    {
        if (isVisible)
            hide();
        else
            show();
    }

    public void show()
    {
        _tm.deactivate();

        gameObject.SetActive(true);

        isVisible = true;
    }
    public void hide()
    {
        /*_cm.delay(() =>
        {
            _tm.activate();
        });*/

        gameObject.SetActive(false);

        isVisible = false;
    }
	#endregion
	
	#region Instance Methods
	#endregion
}
