using UnityEngine;
using GUI;
using Utilities;
using System.Xml;

namespace Editor
{
    class EditorGUI : MonoBehaviour
    {
        protected void Start()
        {
            GUIObject testDialog = new GUIObject();
            XmlElement style = testDialog.get(GUIObject.Style.ANCHOR);
            style.SetAttribute("value", Serialization.toString(new Vector2(0.5f, 0.5f)));
            testDialog.set(style);

            style = testDialog.get(GUIObject.Style.POSITION);
            style.SetAttribute("value", Serialization.toString(Vector3.zero));
            testDialog.set(style);
        }
    }
}
