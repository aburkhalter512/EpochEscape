using UnityEngine;

using System;
using System.Collections.Generic;

namespace MapEditor
{
    public class ObjectManipulator : Manager<ObjectManipulator>, IActivatable
    {
    	public Action<PlaceableObject> onNewPlacement;
    	public Action<PlaceableObject> onSelect;
    	public Action onCancelSelect;
    	public Action onCancelPlacement;

        #region Instance Variables
        private bool mIsActive = false;
        private bool _isControlEnabled = true;

        private GameObject _placementPrefab;
        private PlaceableObject _placement;
        private PlaceableObject _selection;

        #endregion

        protected override void Awaken()
		{ }

        protected override void Initialize()
        { }

        protected void Update()
        {
            if (mIsActive)
            {
                updatePosition();
                //updateSelection();

                if (_isControlEnabled)
                	manipulateSelection();

                if (_placement != null)
                    _placement.stateUpdate();
            }
        }

        #region Interface Methods
        public override void destroy ()
		{
			deactivate();

			base.destroy ();
		}

        public void activate()
        {
            if (mIsActive)
                return;

            mIsActive = true;

            if (_placementPrefab != null)
            {
                _placement = (GameObject.Instantiate(_placementPrefab) as GameObject).GetComponent<PlaceableObject>();

                if (onNewPlacement != null)
                	onNewPlacement(_placement);
            }

            updatePosition();
        }
        public void deactivate()
        {
            if (!mIsActive)
                return;

            mIsActive = false;

            clear();
        }

        public void enableControls()
        {
        	_isControlEnabled = true;
        }
        public void disableControls()
        {
        	_isControlEnabled = false;
        }

        public void clear()
		{
            if (_placement != null)
            {
                _placement.remove();
                GameObject.Destroy(_placement.gameObject);
                _placement = null;
                _placementPrefab = null;

	            if (onCancelPlacement != null)
	            	onCancelPlacement();
            }

            if (_selection != null)
            {
                _selection.deselect();
                _selection = null;

                if (onCancelSelect != null)
                	onCancelSelect();
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
            	if (_selection != null)
            	{
            		_selection.deselect();
            		_selection = null;

            		if (onCancelSelect != null)
            			onCancelSelect();
            	}

                _placement = (GameObject.Instantiate(_placementPrefab) as GameObject).GetComponent<PlaceableObject>();

                if (_placement == null)
					Debug.LogError("Prefab could not be instatiated!");

	            if (onNewPlacement != null)
	            	onNewPlacement(_placement);
            }
        }

        public PlaceableObject getObject()
        {
        	if (_placement != null)
        		return _placement;
        	else
        		return _selection;
        }
        #endregion

        #region Instance Methods
        private void updatePosition()
        {
            if (_placement == null)
                return;

            _placement.moveTo(InputManager.Get().mouse.inWorld());
        }

        private void manipulateSelection()
        {
			InputManager IM = InputManager.Get();
            if (IM.objectSelector.getUp())
			{
				if (_placement != null)
				{
					if (_placement.place())
					{
	                    _placement = (GameObject.Instantiate(_placement.prefab()) as GameObject).GetComponent<PlaceableObject>();
						_placement.moveTo(InputManager.Get().mouse.inWorld());

	                    if (onNewPlacement != null)
	                    	onNewPlacement(_placement);
                    }
                    else
					{
	                    _placement.remove();
	                    GameObject.Destroy(_placement.gameObject);
	                    _placement = null;

		                if (onCancelPlacement != null)
		                	onCancelPlacement();
                    }
				}
				else
				{
					if (_selection != null)
					{
						_selection.deselect();
						_selection = null;

		                if (onCancelSelect != null)
		                	onCancelSelect();
					}

					Tile tile = Map.Get().getExistingTile(InputManager.Get().mouse.inWorld());
	                if (tile != null && tile.hasObject())
	                {
	                    _selection = tile.getObject();
	                    _selection.select();

	                    if (onSelect != null)
	                    	onSelect(_selection);
	                }
				}
            }
            else if (IM.objectDetacher.getUp()) // Also deletes objects
            {
            	if (_selection != null)
				{
					_selection.deselect();
					_placement = _selection;
					_placementPrefab = _placement.prefab();

	                if (onCancelSelect != null)
	                	onCancelSelect();

                    if (onNewPlacement != null)
                    	onNewPlacement(_placement);

                    _selection = null;
            	}
            	else if (_placement != null)
            		clear();
			}
            else if (IM.multiObjectSelector.getUp())
            {
                if (_placement == null && _selection != null)
				{
	                Tile tile = Map.Get().getExistingTile(InputManager.Get().mouse.inWorld());
	                if (tile != null && tile.hasObject())
	                {
	                	IConnector connector = _selection as IConnector;

	                	if (connector != null)
	                	{
	                		IConnectable connectable = tile.getObject();
	                		connector.connect(connectable);
	                	}
	                }
                }
			}
            else if (IM.rotate.getUp())
            {
                if (_placement != null)
                    _placement.rotate();
           }
        }
        #endregion
    }
}
