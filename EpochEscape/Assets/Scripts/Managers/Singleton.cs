using UnityEngine;
using System.Collections;

public class UnitySingleton<T> : MonoBehaviour
    where T : Component
{
    #region Instance Variables
    #endregion

    #region Class Constants
    #endregion

    #region Class Variables
    private static T mInstance;
    #endregion

    public virtual void Awake()
    {
        if (mInstance == null)
        {
            DontDestroyOnLoad(gameObject);
            mInstance = this as T;
        }
        else
            Destroy(gameObject);
    }
    #region Static Methods
    public static T getInstance()
    {
        if (mInstance == null)
        {
            mInstance = FindObjectOfType<T>() as T;
            if (mInstance == null) //No instance exists anywhere
            {
                GameObject go = new GameObject();
                mInstance = go.AddComponent<T>();
            }
        }
        return mInstance;
    }

    public static void destroy()
    {
        Destroy(mInstance);
        mInstance = null;
    }
    #endregion

    #region Utilities
    #endregion
}
