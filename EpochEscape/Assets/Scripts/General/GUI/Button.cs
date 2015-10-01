using System;
using System.Xml;
using UnityEngine.EventSystems;

namespace GUI
{
    class Button : GUIObject
    {
        public XmlElement baseBackround;
        public XmlElement overBackground;
        public XmlElement downBackground;

        public Button(String text, Action<GUIObject> readyCallback = null) :
            base((obj) => 
            { 
                (obj as Button).initHelper(); 
                readyCallback(obj); 
            })
        { }

        private void initHelper()
        {
            addListener(EventTriggerType.PointerEnter, (e) =>
            {
                set(overBackground);
            });

            addListener(EventTriggerType.PointerExit, (e) =>
            {
                set(baseBackround);
            });

            addListener(EventTriggerType.PointerDown, (e) =>
            {
                set(downBackground);
            });

            addListener(EventTriggerType.PointerUp, (e) =>
            {
                set(baseBackround);
            });
        }
    }
}
