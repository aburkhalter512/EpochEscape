using UnityEngine;

namespace MapEditor
{
    public class PlaceableEntranceDoor : PlaceableSimpleDoor
    {
    	protected override void Start ()
		{
			base.Start ();

			mBackSide.deactivate();
		}

        #region Interface Methods
        public static GameObject getPrefab()
        {
			return ResourceManager.ResourceManager.Get().prefab("Prefabs/MapEditor/Placeables/EntranceDoor");
        }
        #endregion

        #region Instance Methods
        protected override GameObject loadPrefab()
        {
            return Resources.Load<GameObject>("Prefabs/MapEditor/Placeables/EntranceDoor");
        }
        #endregion
    }
}
