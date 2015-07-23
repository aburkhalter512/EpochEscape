using UnityEngine;

namespace Editor
{
    public class MapEditorGUI : MonoBehaviour
    {
        #region Interface Variables
        public const int StandardDoor = 0;
        public const int OneWayDoor = 1;
        public const int TeleporterDoor = 2;
        public const int PressurePlate = 3;
        public const int PressureSwitch = 4;
        public const int StaticWall = 5;
        #endregion

        #region Instance Variables
        ObjectManipulator objManip;
        #endregion

        public void Awake()
        {
            objManip = ObjectManipulator.Get();
        }

        #region Interface Methods
        public void selectObject(int objectType)
        {
            switch (objectType)
            {
                case StandardDoor:
                    objManip.setObjectPrefab(PlaceableStandardDoor.getPrefab());
                    break;
                case OneWayDoor:
                    objManip.setObjectPrefab(PlaceableDirectionalDoor.getPrefab());
                    break;
                case TeleporterDoor:
                    objManip.setObjectPrefab(PlaceableTeleporterDoor.getPrefab());
                    break;
                case PressurePlate:
                    objManip.setObjectPrefab(PlaceablePressurePlate.getPrefab());
                    break;
                case PressureSwitch:
                    objManip.setObjectPrefab(PlaceablePressureSwitch.getPrefab());
                    break;
                case StaticWall:
                    objManip.setObjectPrefab(PlaceableStaticWall.getPrefab());
                    break;
                default:
                    return;
            }

            objManip.activate();
        }
        #endregion

        #region Instance Methods
        #endregion
    }
}
