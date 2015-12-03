using UnityEngine;

namespace MapEditor
{
    public class PlaceableStandardDoor : PlaceableSimpleDoor
    {
        #region Interface Methods
        public static GameObject getPrefab()
        {
			return ResourceManager.ResourceManager.Get().prefab("Prefabs/MapEditor/Placeables/StandardDoor");
        }
        #endregion

        #region Instance Methods
        protected override GameObject loadPrefab()
        {
            return Resources.Load<GameObject>("Prefabs/MapEditor/Placeables/StandardDoor");
        }
        #endregion
    }
}
