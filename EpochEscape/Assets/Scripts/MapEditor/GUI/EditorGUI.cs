using UnityEngine;
using GUI;
using Utilities;
using System.Xml;

namespace MapEditor
{
    class EditorGUI : MonoBehaviour
    {
        protected void Start()
        {
            GUIObject testDialog = new GUIObject((guiObj) =>
            {
                guiObj.id("TestDialog");

                XmlElement style = guiObj.get(GUIObject.Style.ANCHOR);
                style.SetAttribute("value", Serialization.toString(new Vector2(0.5f, 0.5f)));
                guiObj.set(style);

                style = guiObj.get(GUIObject.Style.POSITION);
                style.SetAttribute("value", Serialization.toString(Vector3.zero));
                guiObj.set(style);

                style = guiObj.get(GUIObject.Style.SIZE);
                style.SetAttribute("value", Serialization.toString(new Vector2(200, 200)));
                guiObj.set(style);

                style = guiObj.get(GUIObject.Style.PADDING);
                style.SetAttribute("value", (10.0f).ToString());
                guiObj.set(style);

                style = guiObj.get(GUIObject.Style.BACKGROUND);
                style.SetAttribute("color", Serialization.toString(new Color(0, 0, 0, 1)));
                guiObj.set(style);

                guiObj.addListener(UnityEngine.EventSystems.EventTriggerType.PointerDown, (eventData) =>
                {
                    Debug.Log("SUCCESS!!!!!!!!!!!!!!!!!!!!!");
                });

                GUIObject subDialog = new GUIObject((subObj) =>
                {
                    subObj.id("subObj");

                    style = subObj.get(GUIObject.Style.BACKGROUND);
                    style.SetAttribute("color", Serialization.toString(new Color(1, 0, 0, 1)));
                    subObj.set(style);

                    style = subObj.get(GUIObject.Style.ANCHOR);
                    style.SetAttribute("value", Serialization.toString(new Vector2(0.5f, 0.5f)));
                    subObj.set(style);

                    style = subObj.get(GUIObject.Style.POSITION);
                    style.SetAttribute("value", Serialization.toString(new Vector3(1.0f, 1.0f)));
                    subObj.set(style);

                    style = subObj.get(GUIObject.Style.SIZE);
                    style.SetAttribute("value", Serialization.toString(new Vector2(90, 90)));
                    subObj.set(style);

                    guiObj.addChild(subObj);
                });
            });
        }
    }
}
