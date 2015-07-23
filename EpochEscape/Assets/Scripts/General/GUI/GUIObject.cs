using System;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using Utilities;

namespace GUI
{
    class GUIObject
    {
        public enum Style
        {
            POSITION = 0,
            ANCHOR,
            SIZE,
            OVERFLOW,
            PADDING,
            BACKGROUND,
            FONT
        }

        public enum StyleOverflow
        {
            NORMAL,
            CLIP
        }

        // XML Representation
        private static XmlDocument _doc;
        private XmlElement[] _styles;

        private GameObject _box;
        private GameObject _content;
        private SortedDictionary<EventTriggerType, EventTrigger.Entry> _eventEntries;
        private EventTrigger _eventTrigger;

        // Style Values
        private RectTransform _boxRect; // Contains anchor, position, size
        private RectTransform _contentRect;
        private Text _text; // Contains font size/name/style/material
        private StyleOverflow _overflow;

        // Margins/padding follow web box model for rendering content.
        private float _padding;
        private Image _backgroundImage;

        // Structure
        private GUIObject _parent;
        private SortedDictionary<string, GUIObject> _children;

        private string _id;

        protected static MainCanvas _mc;

        public GUIObject()
        {
            _styles = new XmlElement[Enum.GetNames(typeof(Style)).Length];

            _box = new GameObject();
            _boxRect = new GameObject().transform as RectTransform;

            _eventTrigger = _box.AddComponent<EventTrigger>();
            _eventEntries = new SortedDictionary<EventTriggerType,EventTrigger.Entry>(EventTriggerTypeComparer.Get());
            foreach (EventTriggerType type in Enum.GetValues(typeof(EventTriggerType)))
            {
                EventTrigger.Entry entry = new EventTrigger.Entry();
                entry.eventID = type;
                _eventEntries.Add(entry.eventID, entry);
                _eventTrigger.delegates.Add(entry);
            }

            _backgroundImage = _box.AddComponent<Image>();

            _content = new GameObject();
            _contentRect = _contentRect.transform as RectTransform;
            _contentRect.SetParent(_boxRect, false);
            _contentRect.anchorMax = new Vector2(0.5f, 0.5f);
            _contentRect.anchorMin = new Vector2(0.5f, 0.5f);

            _text = _content.AddComponent<Text>();

            _mc = MainCanvas.Get();

            #region Style Value Initializations
            for (int i = 0; i < _styles.Length; i++)
                _styles[i] = doc().CreateElement(((Style) i).ToString().ToLower());

            _styles[(int)Style.ANCHOR].SetAttribute("value", Serialization.toString(Vector2.zero));
            _styles[(int)Style.POSITION].SetAttribute("value", Serialization.toString(Vector3.zero));
            _styles[(int)Style.SIZE].SetAttribute("value", Serialization.toString(Vector2.zero));
            _styles[(int)Style.OVERFLOW].SetAttribute("value", StyleOverflow.NORMAL.ToString());
            _styles[(int)Style.PADDING].SetAttribute("value", (0.0f).ToString("0.##"));
            _styles[(int)Style.BACKGROUND].SetAttribute("image", "");
            _styles[(int)Style.FONT].SetAttribute("color", Serialization.toString(Color.black));
            _styles[(int)Style.FONT].SetAttribute("name", "Arial");

            for (int i = 0; i < _styles.Length; i++)
                set(_styles[i]);
            #endregion
        }

        public static XmlDocument doc()
        {
            if (_doc == null)
                _doc = new XmlDocument();

            return _doc;
        }

        public bool id(string uniqueID)
        {
            if (uniqueID == null)
                return false;

            if (_parent != null)
            {
                _parent._children.Remove(_id);
                _parent._children.Add(uniqueID, this);
            }

            _id = uniqueID;

            return true;
        }
        public string id()
        {
            return _id;
        }

        public bool set(XmlElement data)
        {
            Style style;

            try
            {
                style = Serialization.ParseEnum<Style>(data.Name.ToUpper());
            }
            catch (ArgumentException e)
            {
                Debug.LogError(e.Message);

                return false;
            }

            switch (style)
            {
                case Style.ANCHOR:
                    callbackIfExist(data, "value", (val) => {
                        Vector2 anchorMin = Serialization.toVector2(val);
                        if (anchorMin.x < 0 || anchorMin.x > 1 || anchorMin.y < 0 || anchorMin.y > 1)
                            return;

                        _boxRect.anchorMin = anchorMin;
                        _boxRect.anchorMax = new Vector2(1.0f - anchorMin.x, 1.0f - anchorMin.y);

                        _styles[(int)Style.ANCHOR].GetAttributeNode("value").Value = val;
                    });

                    break;
                case Style.BACKGROUND:
                    break;
                case Style.FONT:
                    callbackIfExist(data, "color", (val) => {
                        _text.color = Serialization.toColor(val);

                        _styles[(int)Style.FONT].GetAttributeNode("color").Value = val;
                    });

                    callbackIfExist(data, "name", (val) => {
                        _text.font = new Font(val);

                        _styles[(int)Style.FONT].GetAttributeNode("name").Value = val;
                    });
                    break;
                case Style.SIZE:
                    callbackIfExist(data, "value", (val) =>
                    {
                        Vector2 size = Serialization.toVector2(val);
                        if (size.x <= _padding * 2 || size.y <= _padding * 2)
                            return;

                        _boxRect.sizeDelta = size;

                        set(_styles[(int)Style.PADDING]); // Recalculate content size

                        _styles[(int)Style.SIZE].GetAttributeNode("value").Value = val;
                    });
                    break;
                case Style.POSITION:
                    callbackIfExist(data, "value", (val) =>
                    {
                        Vector3 viewVec = Serialization.toVector3(val);
                        viewVec = Camera.main.WorldToViewportPoint(viewVec);

                        Vector2 canvasSize = _mc.canvas().GetComponent<RectTransform>().sizeDelta;

                        _boxRect.anchoredPosition = new Vector2(
                            viewVec.x * canvasSize.x - canvasSize.x * 0.5f, 
                            viewVec.y * canvasSize.y - canvasSize.y * 0.5f);

                        _styles[(int)Style.POSITION].GetAttributeNode("value").Value = val;
                    });
                    break;
                case Style.PADDING:
                    callbackIfExist(data, "value", (val) =>
                    {
                        _padding = float.Parse(val);

                        if (_padding * 2 > _boxRect.sizeDelta.x)
                            return;

                        Vector2 vec2 = Utilities.Math.copy(_boxRect.sizeDelta);
                        vec2.x -= _padding * 2;
                        vec2.y -= _padding * 2;

                        _contentRect.sizeDelta = vec2;

                        _styles[(int)Style.PADDING].GetAttributeNode("value").Value = val;
                    });
                    break;
            }

            return true;
        }
        public XmlElement get(Style style)
        {
            bool NOT_DEEP = false;
            return (XmlElement) _styles[(int) style].CloneNode(NOT_DEEP);
        }

        public GUIObject getChild(string uniqueID)
        {
            if (uniqueID == null || uniqueID == "")
                return null;

            GUIObject retVal = null;
            if (_children.TryGetValue(uniqueID, out retVal))
                return retVal;

            return null;
        }
        public bool addChild(GUIObject child)
        {
            if (child == null || child.id() == "")
                return false;

            _children.Add(child.id(), child);
            child._parent = this;

            return true;
        }
        public GUIObject removeChild(GUIObject obj)
        {
            if (obj == null)
                return null;

            return removeChild(obj.id());
        }
        public GUIObject removeChild(string uniqueID)
        {
            if (uniqueID == null || uniqueID == "")
                return null;
            
            GUIObject retVal = null;
            if (_children.TryGetValue(uniqueID, out retVal))
            {
                _children.Remove(uniqueID);
                return retVal;
            }

            return null;
        }

        public GUIObject parent()
        {
            return _parent;
        }

        public UnityAction<BaseEventData> addListener(EventTriggerType eventType, UnityAction<BaseEventData> listener)
        {
            if (listener == null)
                return null;

            EventTrigger.Entry triggerEntry = _eventEntries[eventType];
            triggerEntry.callback.AddListener(listener);

            return listener;
        }
        public UnityAction<BaseEventData> removeListener(EventTriggerType eventType, UnityAction<BaseEventData> listener)
        {
            if (listener == null)
                return null;

            EventTrigger.Entry triggerEntry = _eventEntries[eventType];
            triggerEntry.callback.RemoveListener(listener);

            return listener;
        }

        private void callbackIfExist(XmlElement name, string attr, Action<string> cb)
        {
            string tmp = name.GetAttribute(attr);
            if (tmp != "")
                cb(tmp);
        }
    }
}
