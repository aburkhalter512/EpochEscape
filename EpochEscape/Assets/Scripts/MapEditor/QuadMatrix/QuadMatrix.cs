using UnityEngine;
using System.Collections.Generic;

public class QuadMatrix<T> where T : QuadMatrix<T>.Node.NodeData
{
	#region Interface Variables
	#endregion
	
	#region Instance Variables
    Node mHead = null;
	#endregion

    #region Class Constants
    public readonly Utilities.IntPair DEFAULT_SIZE = new Utilities.IntPair(10, 10);

    public class Node
    {
        #region Instance Variables
        T mData = null;

        Node mTop = null;
        Node mRight = null;
        Node mBottom = null;
        Node mLeft = null;

        Utilities.IntPair mLogicalPosition;

        bool m_isProcessed = false;
        #endregion

        #region Class Constants
        public enum SIDE
        {
            TOP,
            RIGHT,
            BOTTOM,
            LEFT
        }

        public abstract class NodeData
        {
            #region Interface Variables
            public Node quadNode;

            public T data;
            #endregion

            #region Interface Methods
            #endregion
        }
        #endregion

        #region Interface Methods
        public Node()
        {
            mLogicalPosition = new Utilities.IntPair(0, 0);
        }

        public Node(int x, int y)
        {
            mLogicalPosition = new Utilities.IntPair(x, y);
        }

        public Node(T data)
        {
            if (data != null)
                mData = data;
        }

        public bool attachNode(Node toAttach, Node.SIDE side)
        {
            if (toAttach == null || this == toAttach)
                return false;

            toAttach.mLogicalPosition.assign(mLogicalPosition);
            switch (side)
            {
                case SIDE.TOP:
                    mTop = toAttach;
                    toAttach.mBottom = this;
                    toAttach.mLogicalPosition.second++;
                    break;
                case SIDE.RIGHT:
                    mRight = toAttach;
                    toAttach.mLeft = this;
                    toAttach.mLogicalPosition.first++;
                    break;
                case SIDE.BOTTOM:
                    mBottom = toAttach;
                    toAttach.mTop = this;
                    toAttach.mLogicalPosition.second--;
                    break;
                case SIDE.LEFT:
                    mLeft = toAttach;
                    toAttach.mRight = this;
                    toAttach.mLogicalPosition.first--;
                    break;
            }

            return true;
        }

        public Node detachNode(Node.SIDE side)
        {
            Node retVal = null;

            switch (side)
            {
                case SIDE.TOP:
                    if (mTop != null)
                    {
                        retVal = mTop.mBottom;
                        mTop.mBottom = null;
                        mTop = null;
                    }

                    break;
                case SIDE.RIGHT:
                    if (mRight != null)
                    {
                        retVal = mRight;
                        retVal.mBottom = null;
                        mRight = null;
                    }

                    break;
                case SIDE.BOTTOM:
                    if (mBottom != null)
                    {
                        retVal = mBottom;
                        retVal.mTop = null;
                        mBottom = null;
                    }

                    break;
                case SIDE.LEFT:
                    if (mLeft != null)
                    {
                        retVal = mLeft;
                        retVal.mRight = null;
                        mLeft = null;
                    }

                    break;
            }

            return retVal;
        }

        public Node[] detachAll()
        {
            Node[] retVal = new Node[4];

            retVal[0] = mTop;
            if (mTop != null)
                mTop.mBottom = null;

            retVal[1] = mRight;
            if (mRight != null)
                mRight.mLeft = null;

            retVal[2] = mBottom;
            if (mBottom != null)
                mBottom.mTop = null;

            retVal[3] = mLeft;
            if (mLeft != null)
                mLeft.mRight = null;

            return retVal;
        }

        public Node getNode(Node.SIDE side)
        {
            switch (side)
            {
                case SIDE.TOP:
                    return mTop;
                case SIDE.RIGHT:
                    return mRight;
                case SIDE.BOTTOM:
                    return mBottom;
                case SIDE.LEFT:
                    return mLeft;
                default:
                    return null;
            }
        }

        public Utilities.IntPair getPosition()
        {
            return new Utilities.IntPair(mLogicalPosition);
        }

        public T getData()
        {
            return mData;
        }

        public bool setData(T data)
        {
            if (data == null)
                return false;

            mData = data;
            mData.quadNode = this;

            return true;
        }

        public void clear()
        {
            mTop = null;
            mRight = null;
            mBottom = null;
            mLeft = null;

            mData = null;

            m_isProcessed = false;
        }

        public bool IsProcessed
        {
            get { return m_isProcessed; }
            set { m_isProcessed = value; }
        }
        #endregion
    }
    #endregion

    public QuadMatrix()
    { }

    public QuadMatrix(Utilities.IntPair initialSize)
    {
        if (initialSize.first < 1)
            initialSize.first = DEFAULT_SIZE.first;

        if (initialSize.second < 1)
            initialSize.second = DEFAULT_SIZE.second;

        createMatrix(initialSize);
    }

    #region Interface Methods
    public void createMatrix(Utilities.IntPair boundingRect)
    {
        if (mHead != null)
            return;

        Node curNode = null;
        Node startNode = null;
        Node leftNode = null;
        Node bottomNode = null;

        int i;
        for (int j = 0; j < boundingRect.second; j++)
        {
            i = 0;

            if (startNode != null)
            {
                startNode.attachNode(new Node(i, j), Node.SIDE.TOP);
                bottomNode = startNode;
                startNode = startNode.getNode(Node.SIDE.TOP);
                leftNode = null;
            }
            else
            {
                mHead = new Node(i, j);
                startNode = mHead;
                bottomNode = null;
                leftNode = null;
            }

            for (; i < boundingRect.first; i++)
            {
                if (i == 0)
                {
                    curNode = startNode;
                }
                else
                {
                    curNode = new Node(i, j);
                }

                if (leftNode != null)
                {
                    leftNode.attachNode(curNode, Node.SIDE.RIGHT);
                }
                leftNode = curNode;

                if (bottomNode != null)
                {
                    bottomNode.attachNode(curNode, Node.SIDE.TOP);
                    bottomNode = bottomNode.getNode(Node.SIDE.RIGHT);
                }
            }
        }
    }

    public void clear()
    {
        if (mHead != null)
        {
            Stack<Node> toDelete = new Stack<Node>();

            Node curNode = mHead;
            Node nodeToPush;

            toDelete.Push(curNode);

            while (toDelete.Count >= 1)
            {
                curNode = toDelete.Pop();

                nodeToPush = curNode.detachNode(Node.SIDE.LEFT);
                if (nodeToPush != null)
                    toDelete.Push(nodeToPush);

                nodeToPush = curNode.detachNode(Node.SIDE.BOTTOM);
                if (nodeToPush != null)
                    toDelete.Push(nodeToPush);

                nodeToPush = curNode.detachNode(Node.SIDE.RIGHT);
                if (nodeToPush != null)
                    toDelete.Push(nodeToPush);

                nodeToPush = curNode.detachNode(Node.SIDE.TOP);
                if (nodeToPush != null)
                    toDelete.Push(nodeToPush);

                curNode.clear();
            }

            mHead = null;
        }
    }

    public Node getHead()
    {
        return mHead;
    }

    //Current complexity:   O(n^2)
    //Can be reduced to:    O(4n)
    public void processAllNodes(IProcessQuadNode<T> processor)
    {
        if (mHead == null)
            return;

        List<Node> toProcess = new List<Node>();

        Node curNode;
        Node nodeToPush;

        toProcess.Add(mHead);

        int index = 0;

        while (index < toProcess.Count)
        {
            curNode = toProcess[index++];

            nodeToPush = curNode.getNode(Node.SIDE.LEFT);
            if (nodeToPush != null && ! toProcess.Contains(nodeToPush))
                toProcess.Add(nodeToPush);

            nodeToPush = curNode.getNode(Node.SIDE.BOTTOM);
            if (nodeToPush != null && !toProcess.Contains(nodeToPush))
                toProcess.Add(nodeToPush);

            nodeToPush = curNode.getNode(Node.SIDE.RIGHT);
            if (nodeToPush != null && !toProcess.Contains(nodeToPush))
                toProcess.Add(nodeToPush);

            nodeToPush = curNode.getNode(Node.SIDE.TOP);
            if (nodeToPush != null && !toProcess.Contains(nodeToPush))
                toProcess.Add(nodeToPush);

            processor.ProcessQuadNode(curNode);
        }

        foreach (Node node in toProcess)
            node.IsProcessed = false;
    }
	#endregion
}
