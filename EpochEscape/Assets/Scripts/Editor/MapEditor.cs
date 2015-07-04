using UnityEngine;
using System.Collections.Generic;

public class MapEditor : Manager<MapEditor>
{
	#region Interface Variables
	#endregion
	
	#region Instance Variables
    Map _map;
    CameraManager mCameraManager;
    Queue<System.Action> _processorQueue;
    private bool _processing;
    #endregion 
	
	protected override void Awaken()
	{
        _map = Map.Get();
        _processorQueue = new Queue<System.Action>();
        _processing = false;
	}
	
	protected override void Initialize()
    {
        mCameraManager = CameraManager.Get();
        mCameraManager.camType(CameraManager.CAM_TYPE.EDITOR);

        queueNodeProcessor(QuadNodeProcessors.createTile);

        queueTileProcessor(TileProcessors.createEdgeWall);
	}

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
    public void queueTileProcessor(System.Action<Tile, Utilities.Vec2Int> processor)
    {
        _processorQueue.Enqueue(() =>
            {
                _map.processAllTiles(processor);
            });
    }
    public void queueNodeProcessor(System.Action<QuadNode<Tile>, Utilities.Vec2Int> processor)
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
	#endregion
	
	#region Instance Methods
	#endregion
}
