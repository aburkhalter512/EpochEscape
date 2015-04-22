using UnityEngine;
using System.Collections.Generic;
using Node = QuadMatrix<TileData>.Node;

public class EpochMap : Manager<EpochMap>
{
	#region Interface Variables
	#endregion
	
	#region Instance Variables
    public QuadMatrix<TileData> mMatrix;

    DefaultTileCreator tileCreator;
    StaticWallCreator mStaticWallCreator;

    Vector2 mTileSize = new Vector2(.2f, .2f); //This needs to be the same as DEFAULT_TILE_SIZE since we don't have access to the constructor`
	#endregion 

    #region Class Constants
    public readonly Utilities.IntPair DEFAULT_MAP_SIZE = new Utilities.IntPair(16, 16);
    public readonly Vector2 DEFAULT_TILE_SIZE = new Vector2(1.0f, 1.0f);

    private class DefaultTileCreator : IProcessQuadNode<TileData>
    {
        public void ProcessQuadNode(Node node)
        {
            TileCreator.createTile(node);
        }
    }

    private class StaticWallCreator : IProcessQuadNode<TileData>
    {
        public void ProcessQuadNode(Node node)
        {
            StaticWallPlacer.addStaticWallToNode(node);
        }
    }
    #endregion
	
	protected override void Awaken()
	{
        mMatrix = new QuadMatrix<TileData>(DEFAULT_MAP_SIZE);

        tileCreator = new DefaultTileCreator();
	}
	
	protected override void Initialize()
    {
        mMatrix.processAllNodes(tileCreator);

        mStaticWallCreator = new StaticWallCreator();
        processBoundary(mStaticWallCreator);

        return;
	}
	
	#region Interface Methods
    public Vector2 getTileSize()
    {
        return mTileSize;
    }

    public bool setTileSize(Vector2 tileSize)
    {
        if (tileSize.x <= 0 || tileSize.y <= 0)
            return false;

        mTileSize = tileSize;

        //Re-position all tiles at this point!!!!!!!!!!!

        return true;
    }
	#endregion
	
	#region Instance Methods
    protected void processBoundary(IProcessQuadNode<TileData> processor)
    {
        Node curNode = mMatrix.getHead();
        Node nextNode = null;
        while ((nextNode = curNode.getNode(Node.SIDE.LEFT)) != null)
            curNode = nextNode;
        nextNode = curNode.getNode(Node.SIDE.TOP);

        Node.SIDE moveDirection = Node.SIDE.TOP;
        Node.SIDE nextDirection = Node.SIDE.TOP;

        do
        {
            processor.ProcessQuadNode(curNode);
            curNode = nextNode;

            rotateToNode(curNode, rotate(moveDirection, false), out nextNode, out nextDirection, true);
            moveDirection = nextDirection;

        } while (curNode != mMatrix.getHead());
    }

    protected Node.SIDE rotate(Node.SIDE side, bool isClockwise)
    {
        if (isClockwise)
        {
            switch (side)
            {
                case Node.SIDE.TOP:
                    return Node.SIDE.RIGHT;
                case Node.SIDE.RIGHT:
                    return Node.SIDE.BOTTOM;
                case Node.SIDE.BOTTOM:
                    return Node.SIDE.LEFT;
                case Node.SIDE.LEFT:
                    return Node.SIDE.TOP;
            }
        }
        else
        {
            switch (side)
            {
                case Node.SIDE.TOP:
                    return Node.SIDE.LEFT;
                case Node.SIDE.RIGHT:
                    return Node.SIDE.TOP;
                case Node.SIDE.BOTTOM:
                    return Node.SIDE.RIGHT;
                case Node.SIDE.LEFT:
                    return Node.SIDE.BOTTOM;
            }
        }

        return Node.SIDE.TOP; //Impossible to get to this location
    }

    protected void rotateToNode(
        Node baseNode, 
        Node.SIDE baseSide, 
        out Node dest, 
        out Node.SIDE destSide, 
        bool isClockwise)
    {
        dest = null;
        destSide = baseSide;

        for (int i = 0; i < 4; i++)
        {
            destSide = rotate(destSide, isClockwise);
            dest = baseNode.getNode(destSide);

            if (dest != null)
                return;
        }

        dest = null;
    }
	#endregion
}
