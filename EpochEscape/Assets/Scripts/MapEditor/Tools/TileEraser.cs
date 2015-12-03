using UnityEngine;
using System.Collections.Generic;

using Utilities;

namespace MapEditor
{
    public class TileEraser : SquarePlacer<TileEraser>, IActivatable
    {
        #region Interface Variables
        #endregion

        #region Instance Variables
        SortedDictionary<Utilities.Vec2Int, bool> mSelection; //bool is a dummy value

		protected ChunkManager _cm;

        protected GameObject cursor;

        protected bool _isControlEnabled;
        #endregion

        protected override void Awaken()
        {
        	_step = 2;

            base.Awaken();

            mSelection = new SortedDictionary<Utilities.Vec2Int, bool>(Utilities.Vec2IntComparer.Get());

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
            if (tile == null || tile.hasObject())
                return;

            mSelection.Add(tile.position(), false);

            _map.deleteTile(tile.position());
        }
        protected override void finalizeSelection()
        {
            if (mIM.exit.get())
            {
                foreach (Vec2Int pos in mSelection.Keys)
                {
					_map.getTile(pos);
                }
            }
            mSelection.Clear();
        }

        protected override void selectionGrow(Utilities.Vec2Int tilePos)
        {
            Tile tile = _map.getExistingTile(tilePos);

            // Does the tile already exist?
            if (tile == null || tile.hasObject())
                return;

            mSelection.Add(tile.position(), false);

            _map.deleteTile(tile.position());
        }
        protected override void selectionShrink(Utilities.Vec2Int tilePos)
        {
            if (mSelection.Remove(tilePos))
            {
            	Tile tile = _map.getTile(tilePos);
            	QuadNodeProcessors.createTile(tile.node(), tilePos);
            }
        }
        #endregion
    }
}
