using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public abstract class SquarePlacer<T> : Manager<T>, IActivatable where T : MonoBehaviour
{
	#region Interface Variables
	#endregion

    #region Instance Variables
    protected int processCountYield = 80;

    protected InputManager mIM;
    protected Map _map;

    protected Utilities.Vec2Int mOldCursor = new Utilities.Vec2Int(0, 0);
    protected Utilities.Vec2Int mBasePos = new Utilities.Vec2Int(0, 0);

    private List<Utilities.Pair<bool, bool>> mCoTriggers;
    private List<Action> mCoroutines;

    private bool mIsActive = false;
    protected bool mIsActivated = false;
    #endregion

    protected override void Awaken()
    {
        registerCoroutine(resizeHorizontal);
        registerCoroutine(resizeVertical);
    }

    protected override void Initialize()
    {
        mIM = InputManager.Get();
        _map = Map.Get();
    }
	
	protected virtual void Update()
	{
        if (mIsActivated)
            updateSelection();
	}

    #region Interface Methods
    public virtual void activate()
    {
        mIsActivated = true;
    }
    public virtual void deactivate()
    {
        mIsActivated = false;

        if (mIsActive)
            finalizeSelection();
    }

    public void registerCoroutine(Func<IEnumerator> coroutine)
    {
        if (mCoroutines == null)
        {
            mCoroutines = new List<Action>();
            mCoTriggers = new List<Utilities.Pair<bool, bool>>();
        }

        int index = mCoroutines.Count;
        Action coWrapper = () =>
            {
                StartCoroutine(coroutineWrapper(index, coroutine));
            };

        mCoroutines.Add(coWrapper);
        mCoTriggers.Add(new Utilities.Pair<bool, bool>(false, false));
    }
	#endregion

    #region Instance Methods
    private IEnumerator coroutineWrapper(int index, Func<IEnumerator> coroutine)
    {
        if (mCoTriggers[index].first || mCoTriggers[index].second)
            yield break;

        // This magic loop maintains the ordering of the coroutines maintained
        while (true)
        {
            bool coroutineBreak = isCoroutineRunning();

            for (int i = 0; i < index && !coroutineBreak; i++)
                coroutineBreak |= !mCoTriggers[i].second;

            if (coroutineBreak)
                yield return new WaitForSeconds(.1f);
            else
                break;
        }

        mCoTriggers[index].first = true;

        StartCoroutine(coroutine());

        mCoTriggers[index].first = false;
        mCoTriggers[index].second = true;
    }

    private void updateSelection()
    {
        if (mIM.primaryPlace.getDown() && !mIsActive)
        {
            mIsActive = true;
            initSelection();
        }
        else if (mIM.primaryPlace.get() && mIsActive)
            resizeSelection();
        else if (mIM.primaryPlace.getUp() && mIsActive)
        {
            mIsActive = false;
            finalizeSelection();
        }
    }

    protected abstract void initSelection();
    protected abstract void finalizeSelection();

    private void resizeSelection()
    {
        if (isCoroutineRunning())
            return;

        Utilities.Vec2Int curCursor = _map.toLogicalTilePos(mIM.mouse.inWorld());
        Utilities.Vec2Int sizeDelta = curCursor - mOldCursor;

        if (sizeDelta.x == 0 && sizeDelta.y == 0)
            return;

        foreach (Action coWrapper in mCoroutines)
            coWrapper();

        if (didCoroutinesFinish())
            resetCoroutines();
    }

    private IEnumerator resizeHorizontal()
    {
        Utilities.Vec2Int curCursor = _map.toLogicalTilePos(mIM.mouse.inWorld());

        int iIterator = (curCursor.x - mOldCursor.x >= 0) ? 1 : -1;
        Utilities.Comparer.OPERATOR iOP =
            (iIterator == 1) ? Utilities.Comparer.OPERATOR.LE : Utilities.Comparer.OPERATOR.GE;

        int jIterator = (mOldCursor.y >= mBasePos.y) ? 1 : -1;
        Utilities.Comparer.OPERATOR jOP =
            (jIterator == 1) ? Utilities.Comparer.OPERATOR.LE : Utilities.Comparer.OPERATOR.GE;

        int counter = 0;

        for (int i = mOldCursor.x;
            Utilities.Comparer.Compare(i, iOP, curCursor.x);
            i += iIterator)
        {
            for (int j = mBasePos.y;
                Utilities.Comparer.Compare(j, jOP, mOldCursor.y);
                j += jIterator)
            {
                processHorizontalTile(i, j, curCursor);

                if (counter++ >= processCountYield)
                {
                    counter = 0;
                    yield return null;
                }
            }
        }

        mOldCursor.x = curCursor.x;
    }

    private IEnumerator resizeVertical()
    {
        Utilities.Vec2Int curCursor = _map.toLogicalTilePos(mIM.mouse.inWorld());

        int iIterator = (mOldCursor.x >= mBasePos.x) ? 1 : -1;
        Utilities.Comparer.OPERATOR iOP =
            (iIterator == 1) ? Utilities.Comparer.OPERATOR.LE : Utilities.Comparer.OPERATOR.GE;

        int jIterator = (curCursor.y - mOldCursor.y >= 0) ? 1 : -1;
        Utilities.Comparer.OPERATOR jOP =
            (jIterator == 1) ? Utilities.Comparer.OPERATOR.LE : Utilities.Comparer.OPERATOR.GE;

        int counter = 0;

        for (int i = mBasePos.x;
            Utilities.Comparer.Compare(i, iOP, mOldCursor.x);
            i += iIterator)
        {
            for (int j = mOldCursor.y;
                Utilities.Comparer.Compare(j, jOP, curCursor.y);
                j += jIterator)
            {
                processVerticalTile(i, j, curCursor);

                if (counter++ >= processCountYield)
                {
                    counter = 0;
                    yield return null;
                }
            }
        }

        mOldCursor.y = curCursor.y;
    }

    // It is assumed by calling this method that j is a valid tile that 
    // needs to be created or destoryed
    //
    // My mindset is that the contextual information required by calling this
    // method forces the dev to make i correct anyways.
    private void processHorizontalTile(int i, int j, Utilities.Vec2Int currentCursor)
    {
        Utilities.Vec2Int tilePos = new Utilities.Vec2Int(i, j);

        if (mOldCursor.x >= mBasePos.x) // Old is right of base
        {
            if (currentCursor.x >= mOldCursor.x) // Current is right of old
            {
                // Bounds checking
                if (i <= currentCursor.x && i > mOldCursor.x)
                    selectionGrow(tilePos);
            }
            else if (currentCursor.x < mOldCursor.x) // Current is left of old
            {
                // Bounds checking
                if (i >= currentCursor.x && i <= mOldCursor.x)
                {
                    if (i < mBasePos.x) // i is past 
                        selectionGrow(tilePos);
                    else if (i > mBasePos.x && i > currentCursor.x)
                        selectionShrink(tilePos);
                }

            }
        }
        else
        {
            if (currentCursor.x <= mOldCursor.x)
            {
                // Bounds checking
                if (i < mOldCursor.x && i >= currentCursor.x)
                    selectionGrow(tilePos);
            }
            else
            {
                // Bounds checking
                if (i <= currentCursor.x && i >= mOldCursor.x)
                {
                    if (i > mBasePos.x)
                        selectionGrow(tilePos);
                    else if (i < mBasePos.x && i < currentCursor.x)
                        selectionShrink(tilePos);
                }

            }
        }
    }

    // It is assumed by calling this method that i is a valid tile that 
    // needs to be created or destoryed
    //
    // My mindset is that the contextual information required by calling this
    // method forces the dev to make i correct anyways.
    private void processVerticalTile(int i, int j, Utilities.Vec2Int currentCursor)
    {
        Utilities.Vec2Int tilePos = new Utilities.Vec2Int(i, j);

        if (mOldCursor.y >= mBasePos.y) // Old is top of base
        {
            if (currentCursor.y >= mOldCursor.y) // Current is top of old
            {
                // Bounds checking
                if (j > mOldCursor.y && j <= currentCursor.y)
                    selectionGrow(tilePos);
            }
            else // Current is bottom of old
            {
                // Bounds checking
                if (j >= currentCursor.y && j <= mOldCursor.y)
                {
                    if (j < mBasePos.y) // j is past 
                        selectionGrow(tilePos);
                    else if (j > mBasePos.y && j > currentCursor.y)
                        selectionShrink(tilePos);
                }

            }
        }
        else
        {
            if (currentCursor.y <= mOldCursor.y)
            {
                // Bounds checking
                if (j < mOldCursor.y && j >= currentCursor.y)
                    selectionGrow(tilePos);
            }
            else
            {
                // Bounds checking
                if (j <= currentCursor.y && j >= mOldCursor.y)
                {
                    if (j > mBasePos.y)
                        selectionGrow(tilePos);
                    else if (j < mBasePos.y && j < currentCursor.y)
                        selectionShrink(tilePos);
                }

            }
        }
    }

    protected abstract void selectionGrow(Utilities.Vec2Int tilePos);
    protected abstract void selectionShrink(Utilities.Vec2Int tilePos);

    protected bool isCoroutineRunning()
    {
        bool retVal = false;

        foreach (Utilities.Pair<bool, bool> pair in mCoTriggers)
        {
            retVal |= pair.first;
            if (retVal)
                break;
        }

        return retVal;
    }

    protected bool didCoroutinesFinish()
    {
        bool retVal = true;

        foreach (Utilities.Pair<bool, bool> pair in mCoTriggers)
        {
            retVal &= pair.second;
            if (!retVal)
                break;
        }

        return retVal;
    }

    protected void resetCoroutines()
    {
        foreach (Utilities.Pair<bool, bool> pair in mCoTriggers)
            pair.second = false;
    }
	#endregion
}
