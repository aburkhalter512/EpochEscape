using UnityEngine;
using System.Collections.Generic;

namespace MapEditor
{
    public class TileManipulator : SquarePlacer<TileManipulator>, IActivatable
    {
        #region Interface Variables
        #endregion

        #region Instance Variables
        SortedDictionary<Utilities.Vec2Int, Tile> mSelection;

		protected ChunkManager _cm;

        protected GameObject cursor;

        protected bool _isControlEnabled;
        #endregion

        protected override void Awaken()
        {
        	_step = 2;

            base.Awaken();

            mSelection = new SortedDictionary<Utilities.Vec2Int, Tile>(Utilities.Vec2IntComparer.Get());

			cursor = new GameObject();

			_isControlEnabled = true;
            deactivate();
        }

        protected override void Initialize()
        {
            base.Initialize();

            _cm = ChunkManager.Get();
        }

        protected override void Update()
        {
            base.Update();

            if (mIsActivated)
                updatePosition();
        }

        #region Interface Methods
        public override void activate()
        {
        	base.activate();

            cursor.SetActive(true);

            updatePosition();
        }
        public override void deactivate()
        {
            base.deactivate();

            cursor.SetActive(false);
        }

        public virtual void enableControls()
        {
			_isControlEnabled = true;
        }
        public virtual void disableControls()
        {
			_isControlEnabled = false;
        }
        public override bool isActive ()
		{
			return base.isActive() && _isControlEnabled;
		}
        #endregion

        #region Instance Methods
        private void updatePosition()
        {
            transform.position = _map.toTilePos(mIM.mouse.inWorld());
        }

        protected override void initSelection()
        {
            mSelection.Clear();

			Utilities.Vec2Int pos = _map.toLogicalTilePos(mIM.mouse.inWorld());
            Tile tile = _map.getExistingTile(pos);

            mBasePos = new Utilities.Vec2Int(pos);
            mOldCursor = new Utilities.Vec2Int(pos);

            // Does the tile already exist?
            if (tile != null)
                return;

    		tile = _map.getTile(pos.clone());
			mSelection.Add(pos, tile);

            QuadNodeProcessors.createTile(tile.node(), tile.position());
        }
        protected override void finalizeSelection()
        {
            if (mIM.exit.get())
            {
                foreach (Tile tile in mSelection.Values)
                {
                    _map.deleteTile(tile.position());
                }
            }
            mSelection.Clear();
        }

        protected override void selectionGrow(Utilities.Vec2Int tilePos)
        {
            Tile tile = _map.getExistingTile(tilePos);

            // Does the tile already exist?
            if (tile != null)
                return;

            tile = _map.getTile(tilePos);
            mSelection.Add(tilePos.clone(), tile);

            QuadNodeProcessors.createTile(tile.node(), tile.position());
        }
        protected override void selectionShrink(Utilities.Vec2Int tilePos)
        {
            Tile tile = _map.getExistingTile(tilePos);

            // Does the tile already exist?
            if (tile == null || tile.hasObject())
                return;

            if (mSelection.Remove(tilePos))
                _map.deleteTile(tile.position());
        }
        #endregion
    }
}
