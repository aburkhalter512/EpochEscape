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
    protected TileCursor mTileCursor;

    protected InputManager mIM;
    protected TileFinder mTF;

    protected Utilities.IntPair mOldCursor = new Utilities.IntPair(0, 0);
    protected Utilities.IntPair mBasePos = new Utilities.IntPair(0, 0);

    private List<Utilities.Pair<bool, bool>> mCoTriggers;
    private List<Action> mCoroutines;

    private bool mIsActive = false;
    private bool mIsActivated = false;
    #endregion

    protected override void Awaken()
    {
        registerCoroutine(resizeHorizontal);
        registerCoroutine(resizeVertical);
    }

    protected override void Initialize()
    {
        mTileCursor = TileCursor.Get();
        mIM = InputManager.Get();
        mTF = TileFinder.Get();
    }
	
	protected virtual void Update()
	{
        if (mIsActivated)
            updateSelection();
	}

    #region Interface Methods
    public void activate()
    {
        mIsActivated = true;
    }
    public void deactivate()
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

        Utilities.IntPair curCursor = mTileCursor.getLogicalCursor();
        Utilities.IntPair sizeDelta = curCursor - mOldCursor;

        if (sizeDelta.first == 0 && sizeDelta.second == 0)
            return;

        foreach (Action coWrapper in mCoroutines)
            coWrapper();

        if (didCoroutinesFinish())
            resetCoroutines();
    }

    private IEnumerator resizeHorizontal()
    {
        Utilities.IntPair curCursor = mTileCursor.getLogicalCursor();

        int iIterator = (curCursor.first - mOldCursor.first >= 0) ? 1 : -1;
        Utilities.Comparer.OPERATOR iOP =
            (iIterator == 1) ? Utilities.Comparer.OPERATOR.LE : Utilities.Comparer.OPERATOR.GE;

        int jIterator = (mOldCursor.second >= mBasePos.second) ? 1 : -1;
        Utilities.Comparer.OPERATOR jOP =
            (jIterator == 1) ? Utilities.Comparer.OPERATOR.LE : Utilities.Comparer.OPERATOR.GE;

        int counter = 0;

        for (int i = mOldCursor.first;
            Utilities.Comparer.Compare(i, iOP, curCursor.first);
            i += iIterator)
        {
            for (int j = mBasePos.second;
                Utilities.Comparer.Compare(j, jOP, mOldCursor.second);
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

        mOldCursor.first = curCursor.first;
    }

    private IEnumerator resizeVertical()
    {
        Utilities.IntPair curCursor = mTileCursor.getLogicalCursor();

        int iIterator = (mOldCursor.first >= mBasePos.first) ? 1 : -1;
        Utilities.Comparer.OPERATOR iOP =
            (iIterator == 1) ? Utilities.Comparer.OPERATOR.LE : Utilities.Comparer.OPERATOR.GE;

        int jIterator = (curCursor.second - mOldCursor.second >= 0) ? 1 : -1;
        Utilities.Comparer.OPERATOR jOP =
            (jIterator == 1) ? Utilities.Comparer.OPERATOR.LE : Utilities.Comparer.OPERATOR.GE;

        int counter = 0;

        for (int i = mBasePos.first;
            Utilities.Comparer.Compare(i, iOP, mOldCursor.first);
            i += iIterator)
        {
            for (int j = mOldCursor.second;
                Utilities.Comparer.Compare(j, jOP, curCursor.second);
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

        mOldCursor.second = curCursor.second;
    }

    // It is assumed by calling this method that j is a valid tile that 
    // needs to be created or destoryed
    //
    // My mindset is that the contextual information required by calling this
    // method forces the dev to make i correct anyways.
    private void processHorizontalTile(int i, int j, Utilities.IntPair currentCursor)
    {
        Utilities.IntPair tilePos = new Utilities.IntPair(i, j);

        if (mOldCursor.first >= mBasePos.first) // Old is right of base
        {
            if (currentCursor.first >= mOldCursor.first) // Current is right of old
            {
                // Bounds checking
                if (i <= currentCursor.first && i > mOldCursor.first)
                    selectionGrow(tilePos);
            }
            else if (currentCursor.first < mOldCursor.first) // Current is left of old
            {
                // Bounds checking
                if (i >= currentCursor.first && i <= mOldCursor.first)
                {
                    if (i < mBasePos.first) // i is past 
                        selectionGrow(tilePos);
                    else if (i > mBasePos.first && i > currentCursor.first)
                        selectionShrink(tilePos);
                }

            }
        }
        else
        {
            if (currentCursor.first <= mOldCursor.first)
            {
                // Bounds checking
                if (i < mOldCursor.first && i >= currentCursor.first)
                    selectionGrow(tilePos);
            }
            else
            {
                // Bounds checking
                if (i <= currentCursor.first && i >= mOldCursor.first)
                {
                    if (i > mBasePos.first)
                        selectionGrow(tilePos);
                    else if (i < mBasePos.first && i < currentCursor.first)
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
    private void processVerticalTile(int i, int j, Utilities.IntPair currentCursor)
    {
        Utilities.IntPair tilePos = new Utilities.IntPair(i, j);

        if (mOldCursor.second >= mBasePos.second) // Old is top of base
        {
            if (currentCursor.second >= mOldCursor.second) // Current is top of old
            {
                // Bounds checking
                if (j > mOldCursor.second && j <= currentCursor.second)
                    selectionGrow(tilePos);
            }
            else // Current is bottom of old
            {
                // Bounds checking
                if (j >= currentCursor.second && j <= mOldCursor.second)
                {
                    if (j < mBasePos.second) // j is past 
                        selectionGrow(tilePos);
                    else if (j > mBasePos.second && j > currentCursor.second)
                        selectionShrink(tilePos);
                }

            }
        }
        else
        {
            if (currentCursor.second <= mOldCursor.second)
            {
                // Bounds checking
                if (j < mOldCursor.second && j >= currentCursor.second)
                    selectionGrow(tilePos);
            }
            else
            {
                // Bounds checking
                if (j <= currentCursor.second && j >= mOldCursor.second)
                {
                    if (j > mBasePos.second)
                        selectionGrow(tilePos);
                    else if (j < mBasePos.second && j < currentCursor.second)
                        selectionShrink(tilePos);
                }

            }
        }
    }

    protected abstract void selectionGrow(Utilities.IntPair tilePos);
    protected abstract void selectionShrink(Utilities.IntPair tilePos);

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
