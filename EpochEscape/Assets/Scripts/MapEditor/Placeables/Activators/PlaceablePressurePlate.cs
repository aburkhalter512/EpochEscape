using UnityEngine;

namespace MapEditor
{
    public class PlaceablePressurePlate : PlaceableActivator
    {
        #region Instance Variables
        bool mIsActive = true;

        #endregion

        #region Interface Methods
        public static GameObject getPrefab()
        {
			return ResourceManager.ResourceManager.Get().prefab("Prefabs/MapEditor/Placeables/PressurePlate");
        }
        #endregion

        #region Instance Methods
        protected override GameObject loadPrefab()
        {
            return Resources.Load<GameObject>("Prefabs/MapEditor/Placeables/PressurePlate");
        }
        #endregion
    }
}
