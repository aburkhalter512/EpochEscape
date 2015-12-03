using UnityEngine;
using UnityEngine.UI;

namespace GUI
{
    public class MainCanvas : Manager<MainCanvas>
    {
        private Canvas _canvas;
        private RectTransform _canvasRect;

        protected override void Awaken()
        {
            _canvas = gameObject.AddComponent<Canvas>();
            _canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            gameObject.AddComponent<GraphicRaycaster>();

            _canvasRect = GetComponent<RectTransform>();

           MainEventSystem.Get();
        }

        protected override void Initialize()
        { }

        public Canvas canvas()
        {
            return _canvas;
        }

        public Vector2 canvasSize()
        {
        	return _canvasRect.sizeDelta;
        }
    }
}
