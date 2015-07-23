using UnityEngine;

namespace GUI
{
    class MainCanvas : Manager<MainCanvas>
    {
        private Canvas _canvas;

        protected override void Awaken()
        {
            _canvas = GetComponent<Canvas>();
        }

        protected override void Initialize()
        { }

        public Canvas canvas()
        {
            return _canvas;
        }
    }
}
