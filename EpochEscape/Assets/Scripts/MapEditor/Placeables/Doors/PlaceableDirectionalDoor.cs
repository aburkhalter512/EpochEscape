using UnityEngine;

namespace MapEditor
{
    public class PlaceableDirectionalDoor : PlaceableSimpleDoor
    {
    	protected override void Start ()
		{
			base.Start ();

			mFrontSide.deactivate();
		}

        #region Interface Methods
        public static GameObject getPrefab()
        {
			return ResourceManager.ResourceManager.Get().prefab("Prefabs/MapEditor/Placeables/DirectionalDoor");
        }
        #endregion

        #region Instance Methods
        protected override GameObject loadPrefab()
        {
            return Resources.Load<GameObject>("Prefabs/MapEditor/Placeables/DirectionalDoor");
        }
        #endregion
    }
}
