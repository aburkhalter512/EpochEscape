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
    public class GUIObject
    {
        private class GUIScrollbar
        {
            public enum Orientation
            {
                HORIZONTAL = 0,
                VERTICAL
            }

            private GUIObject _obj;
            private bool _isReady;

            private GameObject _back;
            private RectTransform _backRect;
            private Image _backgroundImage;
            private bool _backReady;

            private GameObject _slider;
            private RectTransform _sliderRect;
            private Image _sliderImage;
            private bool _sliderReady;

            private Action<GUIScrollbar> _readyCallback;

            private Orientation _orientation;

            private static readonly Vector2 SLIDER_SIZE = new Vector2(15.0f, 40.0f);

            public GUIScrollbar(GUIObject obj, Action<GUIScrollbar> callback, Orientation orientation)
            {
                _orientation = orientation;

                _readyCallback = callback;
                _isReady = false;
                _backReady = false;
                _sliderReady = false;

                _obj = obj;

                _back = new GameObject();
                _back.transform.SetParent(obj._box.transform, false);
                _backgroundImage = _back.AddComponent<Image>();
                GOCallback cb = _back.AddComponent<GOCallback>();
                cb.startCallback = (go) =>
                {
                    GameObject.Destroy(go.GetComponent<GOCallback>());

                    backReady();
                };

                _slider = new GameObject();
                _slider.transform.SetParent(_back.transform, false);
                _sliderImage = _slider.AddComponent<Image>();
                cb = _slider.AddComponent<GOCallback>();
                cb.startCallback = (go) =>
                {
                    GameObject.Destroy(go.GetComponent<GOCallback>());

                    backReady();
                };
            }
            private void backReady()
            {
                _backReady = true;

                if (_sliderReady)
                    _gameobjectInit();
            }
            private void sliderReady()
            {
                _backReady = true;

                if (_sliderReady)
                    _gameobjectInit();
            }

            private void _gameobjectInit()
            {
                _backRect = _back.transform as RectTransform;
                _sliderRect = _slider.transform as RectTransform;

                switch (_orientation)
                {
                    case Orientation.HORIZONTAL:
                        _backRect.anchorMin = new Vector2(1.0f, 0.5f);
                        _backRect.anchorMax = new Vector2(1.0f, 0.5f);
                        _backRect.anchoredPosition = new Vector2(SLIDER_SIZE.x / -2, 0.0f);

                        _sliderRect.sizeDelta = Utilities.Math.copy(SLIDER_SIZE);
                        _sliderRect.anchorMin = new Vector2(0.5f, 1.0f);
                        _sliderRect.anchorMax = new Vector2(0.5f, 1.0f);
                        _sliderRect.anchoredPosition = new Vector2(0.0f, SLIDER_SIZE.y / -2);
                        break;
                    case Orientation.VERTICAL:
                        _backRect.anchorMin = new Vector2(0.5f, 0.0f);
                        _backRect.anchorMax = new Vector2(0.5f, 0.0f);
                        _backRect.anchoredPosition = new Vector2(0.0f, SLIDER_SIZE.x / 2);

                        _sliderRect.sizeDelta = new Vector2(SLIDER_SIZE.y, SLIDER_SIZE.x);
                        _sliderRect.anchorMin = new Vector2(0.0f, 0.5f);
                        _sliderRect.anchorMax = new Vector2(0.0f, 0.5f);
                        _sliderRect.anchoredPosition = new Vector2(SLIDER_SIZE.y / 2, 0.0f);
                        break;
                }

                updateSize();

                _readyCallback(this);
            }

            public void updateSize()
            {
                _backRect.sizeDelta = new Vector2(_obj._boxRect.sizeDelta.y, SLIDER_SIZE.x);
            }
        }

        private class GUITriggerHandler : MonoBehaviour
        {
            public Dictionary<EventTriggerType, List<UnityAction<BaseEventData>>> _listeners;

            private void Awake()
            {
                _listeners = new Dictionary<EventTriggerType, List<UnityAction<BaseEventData>>>();

                foreach (EventTriggerType triggerType in Enum.GetValues(typeof(EventTriggerType)))
                    _listeners.Add(triggerType, new List<UnityAction<BaseEventData>>());
            }

            public UnityAction<BaseEventData> handler(EventTriggerType triggerType)
            {
                switch (triggerType)
                {
                    case EventTriggerType.PointerDown:
                        return onPointerDown;
                    case EventTriggerType.PointerUp:
                        return onPointerUp;
                    case EventTriggerType.PointerClick:
                        return onPointerClick;
                    case EventTriggerType.PointerEnter:
                        return onPointerEnter;
                    case EventTriggerType.PointerExit:
                        return onPointerExit;
                    case EventTriggerType.InitializePotentialDrag:
                        return onInitializePotentialDrag;
                    case EventTriggerType.BeginDrag:
                        return onBeginDrag;
                    case EventTriggerType.EndDrag:
                        return onEndDrag;
                    case EventTriggerType.Drag:
                        return onDrag;
                    case EventTriggerType.Drop:
                        return onDrop;
                    case EventTriggerType.Select:
                        return onSelect;
                    case EventTriggerType.Deselect:
                        return onDeselect;
                    case EventTriggerType.UpdateSelected:
                        return onUpdateSelected;
                    case EventTriggerType.Submit:
                        return onSubmit;
                    case EventTriggerType.Cancel:
                        return onCancel;
                    case EventTriggerType.Move:
                        return onMove;
                    case EventTriggerType.Scroll:
                        return onScroll;
                }

                return null;
            }

            public void onPointerDown(BaseEventData data)
            {
                callListeners(EventTriggerType.PointerDown, data);
            }
            public void onPointerUp(BaseEventData data)
            {
                callListeners(EventTriggerType.PointerUp, data);
            }
            public void onPointerClick(BaseEventData data)
            {
                callListeners(EventTriggerType.PointerClick, data);
            }
            public void onPointerEnter(BaseEventData data)
            {
                callListeners(EventTriggerType.PointerEnter, data);
            }
            public void onPointerExit(BaseEventData data)
            {
                callListeners(EventTriggerType.PointerExit, data);
            }

            public void onInitializePotentialDrag(BaseEventData data)
            {
                callListeners(EventTriggerType.InitializePotentialDrag, data);
            }
            public void onBeginDrag(BaseEventData data)
            {
                callListeners(EventTriggerType.BeginDrag, data);
            }
            public void onEndDrag(BaseEventData data)
            {
                callListeners(EventTriggerType.EndDrag, data);
            }
            public void onDrag(BaseEventData data)
            {
                callListeners(EventTriggerType.Drag, data);
            }
            public void onDrop(BaseEventData data)
            {
                callListeners(EventTriggerType.Drop, data);
            }

            public void onSelect(BaseEventData data)
            {
                callListeners(EventTriggerType.Select, data);
            }
            public void onDeselect(BaseEventData data)
            {
                callListeners(EventTriggerType.Deselect, data);
            }
            public void onUpdateSelected(BaseEventData data)
            {
                callListeners(EventTriggerType.UpdateSelected, data);
            }

            public void onSubmit(BaseEventData data)
            {
                callListeners(EventTriggerType.Submit, data);
            }
            public void onCancel(BaseEventData data)
            {
                callListeners(EventTriggerType.Cancel, data);
            }

            public void onMove(BaseEventData data)
            {
                callListeners(EventTriggerType.Move, data);
            }

            public void onScroll(BaseEventData data)
            {
                callListeners(EventTriggerType.Scroll, data);
            }

            private void callListeners(EventTriggerType triggerType, BaseEventData data)
            {
                foreach (UnityAction<BaseEventData> listener in _listeners[triggerType])
                    listener(data);
            }
        }

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
        private EventTrigger _eventTrigger;
        private GUITriggerHandler _triggerHandler;

        // Style Values
        private RectTransform _boxRect; // Contains anchor, position, size
        private RectTransform _contentRect;
        private Text _text; // Contains font size/name/style/material
        private StyleOverflow _overflow;

        // padding follow web box model for rendering content.
        private float _padding;
        private Image _backgroundImage;
        private Mask _mask;

        // Structure
        private GUIObject _parent;
        private SortedDictionary<string, GUIObject> _children;

        private string _id;

        protected static MainCanvas _mc;

        private bool _boxReady;
        private bool _contentReady;
        private bool _isReady;
        private Action<GUIObject> _readyCallback;

        public GUIObject(Action<GUIObject> readyCallback = null)
        {
            _id = "";

            _readyCallback = readyCallback;
            _boxReady = false;
            _contentReady = false;
            _isReady = false;

            _styles = new XmlElement[Enum.GetNames(typeof(Style)).Length];

            _mc = MainCanvas.Get();

            _box = new GameObject();
            _box.name = typeof(GUIObject).ToString();
            _box.transform.SetParent(_mc.canvas().transform, false);
            _backgroundImage = _box.AddComponent<Image>();
            _mask = _box.AddComponent<Mask>();

            _triggerHandler = _box.AddComponent<GUITriggerHandler>();
            GOCallback cb = _box.AddComponent<GOCallback>();
            cb.startCallback = (go) =>
            {
                boxReady();
                GameObject.Destroy(go.GetComponent<GOCallback>());
            };

            _eventTrigger = _box.AddComponent<EventTrigger>();
            foreach (EventTriggerType type in Enum.GetValues(typeof(EventTriggerType)))
            {
                EventTrigger.Entry entry = new EventTrigger.Entry();
                entry.eventID = type;
                entry.callback.AddListener(_triggerHandler.handler(type));
                _eventTrigger.triggers.Add(entry);
            }

            _content = new GameObject();
            _content.name = _box.name + ".Content";
            _content.transform.SetParent(_box.transform, false);
            _text = _content.AddComponent<Text>();

            cb = _content.AddComponent<GOCallback>();
            cb.startCallback = (go) =>
            {
                contentReady();
                GameObject.Destroy(go.GetComponent<GOCallback>());
            };

            _children = new SortedDictionary<string, GUIObject>(Utilities.StringComparer.Get());

            #region Style Value Initializations
            for (int i = 0; i < _styles.Length; i++)
                _styles[i] = doc().CreateElement(((Style) i).ToString().ToLower());

            _styles[(int)Style.ANCHOR].SetAttribute("value", Serialization.toString(Vector2.zero));
            _styles[(int)Style.POSITION].SetAttribute("value", Serialization.toString(Vector3.zero));
            _styles[(int)Style.SIZE].SetAttribute("value", Serialization.toString(Vector2.zero));
            _styles[(int)Style.OVERFLOW].SetAttribute("value", StyleOverflow.NORMAL.ToString());
            _styles[(int)Style.PADDING].SetAttribute("value", (0.0f).ToString("0.##"));
            _styles[(int)Style.BACKGROUND].SetAttribute("color", Serialization.toString(Color.white));
            _styles[(int)Style.BACKGROUND].SetAttribute("image", "");
            _styles[(int)Style.FONT].SetAttribute("color", Serialization.toString(Color.black));
            _styles[(int)Style.FONT].SetAttribute("name", "Arial");
            #endregion
        }
        private void _gameobjectInit()
        {
            _isReady = true;

            _boxRect = _box.transform as RectTransform;
            _boxRect.anchorMax = new Vector2(0.5f, 0.5f);
            _boxRect.anchorMin = new Vector2(0.5f, 0.5f);

            _contentRect = _content.transform as RectTransform;

            for (int i = 0; i < _styles.Length; i++)
                set(_styles[i]);
            if (_readyCallback != null)
                _readyCallback(this);
        }

        public static XmlDocument doc()
        {
            if (_doc == null)
                _doc = new XmlDocument();

            return _doc;
        }

        public bool id(string uniqueID)
        {
            if (uniqueID == null || uniqueID == "")
                return false;

            if (_parent != null)
            {
                _parent._children.Remove(_id);
                _parent._children.Add(uniqueID, this);
            }

            _id = uniqueID;

            _box.name = typeof(GUIObject).ToString() + " #" + _id;
            _content.name = typeof(GUIObject).ToString() + ".Content #" + _id;

            return true;
        }
        public string id()
        {
            return _id;
        }

        // This is where most of the UI magic happens
        public bool set(XmlElement data)
        {
            if (!_isReady)
                return false;

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
                        Vector2 anchor = Serialization.toVector2(val);
                        if (anchor.x < 0 || anchor.x > 1 || anchor.y < 0 || anchor.y > 1)
                            return;

                        _boxRect.anchorMin = anchor;
                        _boxRect.anchorMax = anchor;

                        _styles[(int)Style.ANCHOR].GetAttributeNode("value").Value = val;
                    });

                    break;
                case Style.BACKGROUND:
                    callbackIfExist(data, "color", (val) =>
                    {
                        _backgroundImage.color = Serialization.toColor(val);

                        _styles[(int)Style.BACKGROUND].GetAttributeNode("color").Value = val;
                    });

                    callbackIfExist(data, "image", (val) =>
                    {
                        if (val == "")
                            _backgroundImage.sprite = null;
                        else
                            _backgroundImage.sprite = Resources.Load<Sprite>(val);

                        _styles[(int)Style.BACKGROUND].GetAttributeNode("image").Value = val;
                    });

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
            if (!_isReady)
                return null;

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
            if (!_isReady || !child._isReady || child == null || child.id() == "")
                return false;

            _children.Add(child.id(), child);
            child._parent = this;

            child._boxRect.SetParent(_contentRect);

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

            _triggerHandler._listeners[eventType].Add(listener);

            return listener;
        }
        public UnityAction<BaseEventData> removeListener(EventTriggerType eventType, UnityAction<BaseEventData> listener)
        {
            if (listener == null)
                return null;

            _triggerHandler._listeners[eventType].Remove(listener);

            return listener;
        }

        private void callbackIfExist(XmlElement name, string attr, Action<string> cb)
        {
            string tmp = name.GetAttribute(attr);
            if (tmp != "")
                cb(tmp);
        }
        private void boxReady()
        {
            _boxReady = true;

            if (_contentReady)
                _gameobjectInit();
        }
        private void contentReady()
        {
            _contentReady = true;

            if (_boxReady)
                _gameobjectInit();
        }
    }
}
