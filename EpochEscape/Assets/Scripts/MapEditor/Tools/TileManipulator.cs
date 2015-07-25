using UnityEngine;
using System.Collections.Generic;

namespace MapEditor
{
    public class TileManipulator : SquarePlacer<TileManipulator>, IActivatable
    {
        #region Interface Variables
        public GameObject cursor;
        #endregion

        #region Instance Variables
        SortedDictionary<Utilities.Vec2Int, Tile> mSelection;

        protected ChunkManager _cm;
        #endregion

        protected override void Awaken()
        {
            base.Awaken();

            deactivate();

            mSelection = new SortedDictionary<Utilities.Vec2Int, Tile>(Utilities.Vec2IntComparer.Get());
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

            tile = _map.getTile(pos); //Creates a returns the tile
            mSelection.Add(mBasePos.clone(), tile);

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
