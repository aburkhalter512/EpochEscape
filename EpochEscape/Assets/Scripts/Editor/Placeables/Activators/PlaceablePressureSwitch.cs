using UnityEngine;

public class PlaceablePressureSwitch : PlaceableActivator
{
	#region Interface Methods
    public static GameObject getPrefab()
    {
        GameObject retVal = Resources.Load<GameObject>("Prefabs/MapEditor/Placeables/PressureSwitch");

        if (retVal == null)
            Debug.LogError("_prefab is null!");

        return retVal;
    }
	#endregion

    #region Instance Methods
    protected override GameObject loadPrefab()
    {
        return Resources.Load<GameObject>("Prefabs/MapEditor/Placeables/PressureSwitch");
    }
	#endregion
}
