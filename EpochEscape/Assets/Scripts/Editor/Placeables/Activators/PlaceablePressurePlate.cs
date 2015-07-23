using UnityEngine;

namespace Editor
{
    public class PlaceablePressurePlate : PlaceableActivator
    {
        #region Instance Variables
        bool mIsActive = true;

        #endregion

        #region Interface Methods
        public static GameObject getPrefab()
        {
            GameObject retVal = Resources.Load<GameObject>("Prefabs/MapEditor/Placeables/PressurePlate");

            if (retVal == null)
                Debug.LogError("_prefab is null!");

            return retVal;
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
