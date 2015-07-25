using System;
using UnityEngine;

namespace Utilities
{
    public class GOCallback : MonoBehaviour
    {
        public Action<GameObject> startCallback;
        public Action<GameObject> updateCallback;

        private void Start()
        {
            if (startCallback != null)
                startCallback(gameObject);
        }

        private void Update()
        {
            if (updateCallback != null)
                updateCallback(gameObject);
        }
    }
}