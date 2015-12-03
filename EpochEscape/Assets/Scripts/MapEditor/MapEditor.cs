using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

using Utilities;

namespace MapEditor
{
    public class MapEditor : Manager<MapEditor>, ISerializable
    {
        #region Instance Variables
        Map _map;
        Queue<System.Action> _processorQueue;
        private bool _processing;

        ActivatorManager _am;
        ChunkManager _cm;
        DoorManager _dm;
        DynamicWallManager _dwm;
        InputManager _im;
        SaveManager _sm;
        StaticWallManager _swm;

        ObjectManipulator _objectManip;
        TileManipulator _tileManip;
        TileEraser _tileEraser;
        #endregion

        protected override void Awaken()
        {
            _map = Map.Get();
            _processorQueue = new Queue<System.Action>();
            _processing = false;

            _am = ActivatorManager.Get();
            _cm = ChunkManager.Get();
            _dm = DoorManager.Get();
            _dwm = DynamicWallManager.Get();
            _im = InputManager.Get();
            _sm = SaveManager.Get();
            _swm = StaticWallManager.Get();

            _objectManip = ObjectManipulator.Get();
            _tileManip = TileManipulator.Get();
            _tileEraser = TileEraser.Get();
        }

        protected override void Initialize()
        { }

        protected void Update()
        {
            if (_map.doneProcessing())
            {
                if (_processing)
                {
                    _processorQueue.Dequeue();
                    _processing = false;
                }

                if (_processorQueue.Count > 0)
                {
                    _processorQueue.Peek()(); // Call the lambda inside the queue
                    _processing = true;
                }
            }
        }

        #region Interface Methods
        public void initDefault()
        {
			_map.initialize();

            queueNodeProcessor(QuadNodeProcessors.createTile);

            queueTileProcessor(TileProcessors.createEdgeWall);
        }

        public Map map()
        {
        	return _map;
        }

    	public override void destroy()
    	{
    		_am.destroy();
    		_am = null;

    		_cm.destroy();
    		_cm = null;

    		_dm.destroy();
    		_dm = null;

    		_dwm.destroy();
    		_dwm = null;

    		_im.destroy();
    		_im = null;

    		_sm = null;

    		_swm.destroy();
    		_swm = null;

    		PlaceableStaticWall.cleanup();
    		PlaceableDoor.cleanup();

    		_objectManip.destroy();
    		_tileManip.destroy();
    		_tileEraser.destroy();

    		base.destroy();
    	}

        public void queueTileProcessor(Action<Tile, Vec2Int> processor)
        {
            _processorQueue.Enqueue(() =>
                {
                    _map.processAllTiles(processor);
                });
        }
        public void queueNodeProcessor(Action<QuadNode<Tile>, Vec2Int> processor)
        {
            _processorQueue.Enqueue(() =>
                {
                    _map.processAllNodes(processor);
                });
        }

        public bool doneProcessing()
        {
            return _processorQueue.Count == 0 && _map.doneProcessing();
        }

        public IEnumerator serialize(XmlDocument doc, Action<XmlElement> callback)
        {
        	if (doc == null)
        		yield break;

        	yield return StartCoroutine(_map.serialize(doc, (tiles) =>
            {
                callback(tiles);
            }));
        }
        #endregion

        #region Instance Methods
        #endregion
    }
}
