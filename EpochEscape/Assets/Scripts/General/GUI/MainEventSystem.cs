using UnityEngine.EventSystems;
namespace GUI
{
    public class MainEventSystem : Manager<MainEventSystem>
    {
        private EventSystem _eventSystem;
        private StandaloneInputModule _standardInput;

        protected override void Awaken()
        {
            _eventSystem = gameObject.AddComponent<EventSystem>();
            _standardInput = gameObject.AddComponent<StandaloneInputModule>();
        }

        protected override void Initialize()
        { }

        public EventSystem eventSystem()
        {
            return _eventSystem;
        }

        public BaseInputModule inputModule()
        {
            return _standardInput;
        }
    }
}
