using UnityEngine;

namespace GUI
{
    public class MainCanvas : Manager<MainCanvas>
    {
        private Canvas _canvas;
        private MainEventSystem _es;

        protected override void Awaken()
        {
            _canvas = gameObject.AddComponent<Canvas>();
            _canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            _es = MainEventSystem.Get();
        }

        protected override void Initialize()
        { }

        public Canvas canvas()
        {
            return _canvas;
        }
    }
}
