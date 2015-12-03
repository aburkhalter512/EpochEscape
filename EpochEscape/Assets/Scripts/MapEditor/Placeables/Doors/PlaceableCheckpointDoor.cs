using UnityEngine;

namespace MapEditor
{
    public class PlaceableCheckpointDoor : PlaceableSimpleDoor
    {
    	protected override void Start ()
		{
			base.Start ();

			mFrontSide.deactivate();
		}

        #region Interface Methods
        public static GameObject getPrefab()
        {
			return ResourceManager.ResourceManager.Get().prefab("Prefabs/MapEditor/Placeables/CheckpointDoor");
        }
        #endregion

        #region Instance Methods
        protected override GameObject loadPrefab()
        {
            return Resources.Load<GameObject>("Prefabs/MapEditor/Placeables/CheckpointDoor");
        }
        #endregion
    }
}
