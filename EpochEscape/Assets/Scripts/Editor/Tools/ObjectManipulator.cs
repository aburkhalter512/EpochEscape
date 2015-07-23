using UnityEngine;
using System.Collections.Generic;

namespace Editor
{
    public class ObjectManipulator : Manager<ObjectManipulator>, IActivatable
    {
        #region Instance Variables
        private bool mIsActive = false;

        private GameObject _placementPrefab;
        private PlaceableObject _placement;
        private PlaceableObject _selection;

        List<GameObject> _placeables;
        int _selectionIndex;

        bool isChangingSelection;
        float changingSelectionBaseTime;

        protected InputManager mIM;
        protected Map _map;
        #endregion

        protected override void Awaken()
        {
            _placeables = new List<GameObject>();

            _placeables.Add(PlaceableStaticWall.getPrefab());
            _placeables.Add(PlaceableTeleporterDoor.getPrefab());
            _placeables.Add(PlaceableStandardDoor.getPrefab());
            _placeables.Add(PlaceableDirectionalDoor.getPrefab());
            _placeables.Add(PlaceableCheckpointDoor.getPrefab());
            _placeables.Add(PlaceableEntranceDoor.getPrefab());
            _placeables.Add(PlaceablePressurePlate.getPrefab());
            _placeables.Add(PlaceablePressureSwitch.getPrefab());
            _placeables.Add(PlaceableRotatingWall.getPrefab());
            _placeables.Add(PlaceableSlidingWall.getPrefab());

            _selectionIndex = 0;

            isChangingSelection = false;
            changingSelectionBaseTime = 0.0f;
        }

        protected override void Initialize()
        {
            mIM = InputManager.Get();
            _map = Map.Get();
        }

        protected void Update()
        {
            if (mIsActive)
            {
                updatePosition();
                updateSelection();

                manipulateSelection();

                if (_placement != null)
                    _placement.stateUpdate();
            }
        }

        #region Interface Methods
        public void activate()
        {
            if (mIsActive)
                return;

            mIsActive = true;

            if (_placementPrefab != null)
                _placement = (GameObject.Instantiate(_placementPrefab) as GameObject).GetComponent<PlaceableObject>();

            updatePosition();
        }
        public void deactivate()
        {
            if (!mIsActive)
                return;

            mIsActive = false;

            if (_placement != null)
            {
                _placement.remove();
                GameObject.Destroy(_placement.gameObject);
                _placement = null;
                _placementPrefab = null;
            }

            if (_selection != null)
            {
                _selection.deselect();
                _selection = null;
            }
        }

        public void setObjectPrefab(GameObject prefab)
        {
            if (prefab == null)
            {
                Debug.LogError("Prefab is null!");
                return;
            }

            _placementPrefab = prefab;

            if (_placement != null)
                GameObject.Destroy(_placement.gameObject);

            if (mIsActive)
            {
                _placement = (GameObject.Instantiate(_placementPrefab) as GameObject).GetComponent<PlaceableObject>();

                if (_placement == null)
                    Debug.LogError("Prefab could not be instatiated!");
            }
        }
        #endregion

        #region Instance Methods
        private void updatePosition()
        {
            if (_placement == null)
                return;

            if (mIM == null)
                Debug.LogError("Mouse is null!");

            _placement.moveTo(mIM.mouse.inWorld());
        }
        private void updateSelection()
        {
            if (isChangingSelection)
            {
                if (Utilities.Math.IsApproximately(mIM.objectChanger.get(), 0))
                    isChangingSelection = false;
                else if (Time.realtimeSinceStartup > changingSelectionBaseTime + 0.33f)
                {
                    if (mIM.objectChanger.get() > 0)
                        _selectionIndex = (_selectionIndex + 1) % _placeables.Count;
                    else
                        _selectionIndex = (_selectionIndex - 1) % _placeables.Count;

                    _selectionIndex = (_selectionIndex < 0) ? _selectionIndex + _placeables.Count : _selectionIndex;

                    setObjectPrefab(_placeables[_selectionIndex]);

                    changingSelectionBaseTime = Time.realtimeSinceStartup;
                }
            }
            else
            {
                if (!Utilities.Math.IsApproximately(mIM.objectChanger.get(), 0))
                {
                    if (_placement == null)
                        _selectionIndex = 0;
                    else if (mIM.objectChanger.get() > 0)
                        _selectionIndex = (_selectionIndex + 1) % _placeables.Count;
                    else
                        _selectionIndex = (_selectionIndex - 1) % _placeables.Count;

                    _selectionIndex = (_selectionIndex < 0) ? _selectionIndex + _placeables.Count : _selectionIndex;

                    setObjectPrefab(_placeables[_selectionIndex]);

                    changingSelectionBaseTime = Time.realtimeSinceStartup;

                    isChangingSelection = true;
                }
            }
        }

        private void manipulateSelection()
        {
            if (mIM.objectDeleter.getUp())
            {
                if (_placement != null)
                {
                    _placement.remove();
                    GameObject.Destroy(_placement.gameObject);
                    _placement = null;
                }

                if (_selection != null)
                {
                    _selection.deselect();
                    _selection = null;
                }
            }
            else if (mIM.rotate.getUp())
            {
                if (_placement != null)
                    _placement.rotate();
            }
            else if (mIM.objectSelector.getUp())
            {
                if (_placement != null && _placement.place())
                {
                    if (_selection != null)
                    {
                        _selection.deselect();
                        _selection = null;
                    }

                    _selection = _placement;
                    _selection.select();

                    _placement = (GameObject.Instantiate(_placement.prefab()) as GameObject).GetComponent<PlaceableObject>();
                    _placement.moveTo(mIM.mouse.inWorld());
                }
                else
                {
                    if (_selection != null)
                    {
                        _selection.deselect();
                        _selection = null;
                    }

                    Tile tile = _map.getExistingTile(mIM.mouse.inWorld());

                    if (tile != null && tile.hasObject())
                    {
                        if (_placement != null)
                        {
                            _placement.remove();
                            GameObject.Destroy(_placement.gameObject);
                        }

                        _placement = tile.getObject();
                        _placement.remove();
                        _placementPrefab = _placement.prefab();
                    }
                }
            }
            else if (mIM.multiObjectSelector.getUp())
            {
                if (_selection != null)
                {
                    _selection.deselect();
                    _selection = null;
                }

                if (_placement != null)
                {
                    _placement.remove();
                    GameObject.Destroy(_placement.gameObject);
                    _placement = null;
                }

                Tile tile = _map.getExistingTile(mIM.mouse.inWorld());
                if (tile != null && tile.hasObject())
                {
                    _selection = tile.getObject();
                    _selection.select();
                }
            }
        }
        #endregion
    }
}
