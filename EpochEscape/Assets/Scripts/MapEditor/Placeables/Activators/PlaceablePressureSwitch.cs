using UnityEngine;

namespace MapEditor
{
    public class PlaceablePressureSwitch : PlaceableActivator
    {
        #region Interface Methods
        public static GameObject getPrefab()
        {
			return ResourceManager.ResourceManager.Get().prefab("Prefabs/MapEditor/Placeables/PressureSwitch");
        }
        #endregion

        #region Instance Methods
        protected override GameObject loadPrefab()
        {
            return Resources.Load<GameObject>("Prefabs/MapEditor/Placeables/PressureSwitch");
        }
        #endregion
    }
}
