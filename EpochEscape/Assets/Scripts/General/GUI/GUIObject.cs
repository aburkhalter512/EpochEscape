using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

using Utilities;

namespace GUI
{
    public class GUIObject
    {
        private class GUITriggerHandler : MonoBehaviour
        {
            public Dictionary<EventTriggerType, List<UnityAction<BaseEventData>>> _listeners;

            private void Awake()
            {
                _listeners = new Dictionary<EventTriggerType, List<UnityAction<BaseEventData>>>();

                foreach (EventTriggerType triggerType in Enum.GetValues(typeof(EventTriggerType)))
                    _listeners.Add(triggerType, new List<UnityAction<BaseEventData>>());
            }

			#region Event Handlers/Callers
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
			#endregion
         }

        protected static ResourceManager.ResourceManager _rm = ResourceManager.ResourceManager.Get();

        protected GameObject _box;
        private GUITriggerHandler _triggerHandler;

		#region GUIObject Styles
		public enum POSITION_TYPE
		{
			RELATIVE,
			ABSOLUTE,
			PERCENT
		}

        // Style Values
        protected RectTransform _boxRect; // Contains anchor, position, size
		#endregion

        // Structure
        private GUIObject _parent;
        private SortedDictionary<string, GUIObject> _children;

        protected string _name;
        private string _id;

        protected static MainCanvas _mc;

        private bool _isReady;
        private Action<GUIObject> _readyCallback;

        public GUIObject(Action<GUIObject> readyCallback = null)
        {
            _readyCallback = readyCallback;
            _isReady = false;

            _mc = MainCanvas.Get();

            _box = new GameObject("", typeof(RectTransform));
            _box.name = typeof(GUIObject).ToString();
            _box.transform.SetParent(_mc.canvas().transform, false);
				
			_triggerHandler = _box.AddComponent<GUITriggerHandler>();
            EventTrigger eventTrigger = _box.AddComponent<EventTrigger>();
            foreach (EventTriggerType type in Enum.GetValues(typeof(EventTriggerType)))
            {
                EventTrigger.Entry entry = new EventTrigger.Entry();
                entry.eventID = type;
                entry.callback.AddListener(_triggerHandler.handler(type));
				eventTrigger.triggers.Add(entry);
            }

			_children = new SortedDictionary<string, GUIObject>(Utilities.StringComparer.Get());

        	_name = "";
			_id = Serialization.generateUUID(_box);
			Debug.Log("id: " + _id);

            GOCallback cb = _box.AddComponent<GOCallback>();
            cb.startCallback = (go) =>
            {
                _gameobjectInit();
                GameObject.Destroy(go.GetComponent<GOCallback>());
            };
		}

		#region Init Callbacks
        private void _gameobjectInit()
        {
            _isReady = true;

            _boxRect = _box.transform as RectTransform;
           	Debug.Assert(_boxRect != null);

            if (isReady())
            	styleInit();

            if (_readyCallback != null)
                _readyCallback(this);
		}
		public virtual bool isReady()
		{
			return _isReady;
		}
		protected virtual void styleInit()
		{
            anchor(defaultAnchor());
            position(defaultPosition());
            size(defaultSize());
		}
		#endregion

		~GUIObject()
		{
			destroy();
		}

		public void destroy()
		{
			foreach (GUIObject child in _children.Values)
				child.destroy();

			_children.Clear();
			_children = null;

			GameObject.Destroy(_box);
			_box = null;

			_parent = null;
		}

		public string id()
		{
			return _id.Clone() as string;
		}

        public bool name(string objName)
        {
			if (objName == null)
				return false;

            _name = objName;

            _box.name = _name;

            return true;
        }
        public string name()
        {
            return _box.name;
        }

        #region Style Methods
        public virtual bool visible()
        {
        	if (!isReady())
        		return false;

        	return _box.activeInHierarchy;
        }
        public virtual bool show()
        {
        	if (!isReady())
        		return false;

        	_box.SetActive(true);

        	return true;
        }
        public virtual bool hide()
		{
			if (!isReady())
        		return false;

        	_box.SetActive(false);

        	return true;
        }

        public virtual bool anchor(Vector2 anchor)
        {
			if (!isReady())
        		return false;

        	if (anchor.x < 0 || anchor.x > 1 || anchor.y < 0 || anchor.y > 1)
        		return false;

        	_boxRect.anchorMin = anchor;
        	_boxRect.anchorMax = anchor;

        	return true;
        }
        public virtual Vector2 anchor()
		{
			if (!isReady())
        		return Vector2.zero;

        	return Utilities.Math.copy(_boxRect.anchorMin);
        }
        public virtual Vector2 defaultAnchor()
        {
        	return new Vector2(0.5f, 0.5f);
        }

		public virtual bool size(Vector2 size)
		{
        	if (!isReady())
        		return false;

        	if (size.x <= 0 || size.y <= 0)
        		return false;

        	_boxRect.sizeDelta = Utilities.Math.copy(size);

        	return true;
        }
		public virtual Vector2 size()
		{
        	if (!isReady())
        		return Vector2.zero;

        	return Utilities.Math.copy(_boxRect.sizeDelta);
        }
        public virtual Vector2 defaultSize()
        {
        	return new Vector2(100, 100);
        }

		public virtual bool position(Vector2 position, POSITION_TYPE posType = POSITION_TYPE.PERCENT)
		{
        	if (!isReady())
        		return false;

			switch (posType)
			{
				case POSITION_TYPE.PERCENT:
		            Vector2 parentSize = _mc.canvas().GetComponent<RectTransform>().sizeDelta / 2;

					if (_parent != null)
						parentSize = _parent.size() / 2;

		            _boxRect.anchoredPosition = new Vector2(
						position.x * parentSize.x, 
						position.y * parentSize.y);

					return true;
				case POSITION_TYPE.ABSOLUTE:
					_boxRect.position = position;

					return true;
				case POSITION_TYPE.RELATIVE:
					_boxRect.anchoredPosition = position;

					return true;
				default:
					return false;
			}
        }
		public virtual Vector2 position(POSITION_TYPE posType = POSITION_TYPE.RELATIVE)
		{
        	if (!isReady())
        		return Vector2.zero;

        	switch (posType)
        	{
        		case POSITION_TYPE.ABSOLUTE:
					return Utilities.Math.copy(_boxRect.position);
        		case POSITION_TYPE.RELATIVE:
        			return Utilities.Math.copy(_boxRect.anchoredPosition);
				case POSITION_TYPE.PERCENT:
		            Vector2 parentSize = _mc.canvas().GetComponent<RectTransform>().sizeDelta / 2;

					if (_parent != null)
						parentSize = _parent.size() / 2;

					Vector2 retVal = Utilities.Math.copy(_boxRect.anchoredPosition);
					retVal.x -= parentSize.x;
					retVal.y -= parentSize.y;

					retVal.x /= parentSize.x;
					retVal.y /= parentSize.y;

		            return retVal;
        		default:
        			return Vector2.zero;

        	}
       	}
       	public virtual Vector2 defaultPosition()
       	{
       		return Vector2.zero;
                	}
        #endregion

        #region Relationship Methods
        public virtual GUIObject getChild(string uniqueID)
        {
            if (uniqueID == null || uniqueID == "")
                return null;

            GUIObject retVal = null;
            if (_children.TryGetValue(uniqueID, out retVal))
                return retVal;

            return null;
        }
        public virtual GUIObject[] getChildren()
        {
        	if (!isReady())
        		return null;

        	GUIObject[] children = new GUIObject[_children.Count];
        	_children.Values.CopyTo(children, 0);

        	return children;
        }
        public virtual bool addChild(GUIObject child)
        {
            if (!isReady() || !child.isReady() || child == null)
                return false;

            Debug.Assert(_children != null);

            try
            {
				_children.Add(child.id(), child);
            }
            catch (ArgumentException e)
            {
            	Debug.LogException(e);
            	Debug.LogError("Could not add child with id: " + child.id());

            	Debug.LogError("Children Dump");
            	foreach (string childId in _children.Keys)
            		Debug.LogError("Child id: " + child.id() + ", name: " + child.name());
            }
            child._parent = this;

            child._boxRect.SetParent(_boxRect);

            return true;
        }
        public virtual GUIObject removeChild(GUIObject obj)
        {
            if (obj == null)
                return null;

            return removeChild(obj.id());
        }
        public virtual GUIObject removeChild(string uniqueID)
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
        public virtual GUIObject[] removeAllChildren()
        {
        	if (!isReady())
        		return null;

        	GUIObject[] children = new GUIObject[_children.Count];
        	_children.Values.CopyTo(children, 0);

        	_children.Clear();

        	foreach (GUIObject child in children)
        	{
        		child._parent = null;
        		child._boxRect.transform.parent = MainCanvas.Get().canvas().transform;
        	}

        	Debug.Assert(_children != null);

        	return children;
        }
        public virtual bool destroyAllChildren()
        {
        	if (!isReady())
        		return false;

			foreach (GUIObject child in _children.Values)
        		child.destroy();

			_children.Clear();

        	Debug.Assert(_children != null);

        	return true;
        }

        public virtual GUIObject findChildByName(string name)
        {
        	if (!isReady())
        		return null;

        	foreach (GUIObject child in _children.Values)
        		if (child.name() == name)
        			return child;

        	return null;
        }

        public GUIObject parent()
        {
            return _parent;
        }
       	#endregion

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
    }

    public class GUIButton : GUIImage
	{
    	public enum STATE
    	{
    		NORMAL,
    		OVER,
    		DOWN
    	}

    	protected Sprite baseBackgroundImage;
    	protected Color baseBackgroundColor;

    	protected Sprite overBackgroundImage;
    	protected Color overBackgroundColor;

    	protected Sprite downBackgroundImage;
    	protected Color downBackgroundColor;

    	protected STATE _state;

    	public GUIButton(Action<GUIObject> callback = null) :
    		base((obj) =>
    		{
    			GUIButton _this = obj as GUIButton;
    			_this.initHelper();

    			if (callback != null)
    				callback(obj);
    		})
    	{
    		_state = STATE.NORMAL;
    	}

        private void initHelper()
        {
        	addListener(EventTriggerType.PointerExit, (e) =>
        	{
        		image(baseBackgroundImage);
        		imageColor(baseBackgroundColor);
			});

        	addListener(EventTriggerType.PointerEnter, (e) =>
        	{
        		image(overBackgroundImage);
        		imageColor(overBackgroundColor);
        	});

        	addListener(EventTriggerType.PointerUp, (e) =>
        	{
        		image(overBackgroundImage);
        		imageColor(overBackgroundColor);

        		_state = STATE.OVER;
        	});

        	addListener(EventTriggerType.PointerDown, (e) =>
        	{
        		image(downBackgroundImage);
        		imageColor(downBackgroundColor);

        		_state = STATE.DOWN;
        	});
		}

        public void setBaseBackground(Sprite sprite, Color color)
        {
        	if (_state == STATE.NORMAL)
        	{
        		image(sprite);
        		imageColor(color);
        	}

			baseBackgroundImage = sprite;
        	baseBackgroundColor = color;
        }
        public void setOverBackground(Sprite sprite, Color color)
		{
        	if (_state == STATE.OVER)
        	{
        		image(sprite);
        		imageColor(color);
        	}

        	overBackgroundImage = sprite;
        	overBackgroundColor = color;
        }
        public void setDownBackground(Sprite sprite, Color color)
		{
        	if (_state == STATE.DOWN)
        	{
        		image(sprite);
        		imageColor(color);
        	}

        	downBackgroundImage = sprite;
        	downBackgroundColor = color;
        }
    }

    public class GUIText : GUIObject
	{
		protected Text _text; // Contains font size/name/style/material

		public GUIText(Action<GUIObject> callback = null) :
			base(callback)
		{
			_text = _box.AddComponent<Text>();
		}


		protected override void styleInit ()
		{
			base.styleInit ();

            fontColor(defaultFontColor());
            font(defaultFont());
            fontSize(defaultFontSize());
		}

		public virtual bool fontColor(Color color)
		{
        	if (!isReady())
        		return false;

        	_text.color = color;

        	return true;
        }
		public virtual Color fontColor()
		{
        	if (!isReady())
        		return Color.black;

        	return _text.color;
        }
        public virtual Color defaultFontColor()
        {
        	return Color.black;
        }

		public virtual bool font(Font font)
		{
        	if (!isReady())
        		return false;

			if (font == null)
        		return false;

        	_text.font = font;

        	return true;
        }
		public virtual Font font()
		{
        	if (!isReady())
        		return null;

        	return _text.font;
        }
        public virtual Font defaultFont()
        {
			return new Font("Arial");
        }

        public virtual bool fontSize(int fontSize)
        {
        	if (!isReady())
        		return false;

        	if (fontSize < 1)
        		return false;

        	_text.fontSize = fontSize;

        	return true;
        }
        public virtual int fontSize()
        {
        	if (!isReady())
        		return 0;

        	return _text.fontSize;
        }
        public virtual int defaultFontSize()
        {
        	return 14;
        }

        public virtual bool text(String text)
        {
        	if (!isReady())
        		return false;

        	if (text == null)
        		return false;

        	_text.text = text;

        	return true;
        }
        public virtual String text()
        {
        	if (!isReady())
        		return null;

        	return _text.text.Clone() as String;
        }
    }

    public class GUIImage : GUIObject
	{
        protected Image _backgroundImage;

        public GUIImage(Action<GUIObject> callback = null) :
        	base (callback)
        {
            _backgroundImage = _box.AddComponent<Image>();
		}

		protected override void styleInit ()
		{
			base.styleInit ();

            image(defaultImage());
            imageColor(defaultImageColor());
		}

		public virtual bool imageColor(Color color)
		{
        	if (!isReady())
        		return false;

        	_backgroundImage.color = color;

        	return true;
        }
		public virtual Color imageColor()
		{
        	if (!isReady())
        		return Color.white;

        	return _backgroundImage.color;
        }
        public virtual Color defaultImageColor()
        {
        	return Color.white;
        }

		public virtual bool image(Sprite sprite)
		{
        	if (!isReady())
        		return false;

        	_backgroundImage.sprite = sprite;

        	return true;
        }
		public virtual Sprite image()
        {
        	if (!isReady())
        		return null;

        	return _backgroundImage.sprite;
        }
        public virtual Sprite defaultImage()
        {
        	return null;
        }
    }

    public class GUIContent : GUIObject
	{
		public enum OVERFLOW_TYPE
		{
			HIDE,
			SHOW
		}

		protected Image _imageMask;
        protected Mask _overflowMask;

        public GUIContent(Action<GUIObject> callback = null) : base(callback)
		{
			_imageMask = _box.AddComponent<Image>();
			_imageMask.color = Color.white;

            _overflowMask = _box.AddComponent<Mask>();
            _overflowMask.enabled = false;
            _overflowMask.showMaskGraphic = false;
        }

        protected override void styleInit ()
		{
			base.styleInit ();

            overflow(defaultOverflow());
		}

        public virtual bool overflow(OVERFLOW_TYPE overflow)
        {
        	if (!isReady())
        		return false;

        	switch (overflow)
        	{
        		case OVERFLOW_TYPE.HIDE:
        			_overflowMask.enabled = true;
        			break;
				case OVERFLOW_TYPE.SHOW:
        			_overflowMask.enabled = false;
        			break;
        		default:
        			Debug.Assert(false);
        			return false;
        	}

        	return true;
        }
        public virtual OVERFLOW_TYPE overflow()
        {
        	if (!isReady())
        		return OVERFLOW_TYPE.SHOW;

        	return (_overflowMask.enabled) ? OVERFLOW_TYPE.HIDE : OVERFLOW_TYPE.SHOW;
        }
        public virtual OVERFLOW_TYPE defaultOverflow()
        {
        	return OVERFLOW_TYPE.SHOW;
        }
    }

    public class GUIVerticalDialog : GUIContent
    {
    	protected GUIImage background;
    	protected GUIObject content;
    	protected GUIButton upButton;
    	protected GUIButton downButton;

    	protected float _contentHeight;
    	protected float _scrollDelta;
    	protected Vector2 _currentScroll;

    	public static readonly float SCROLL_DELTA = 2;

    	public static readonly Vector2 ARROW_SIZE = new Vector2(40, 40);

    	public GUIVerticalDialog(Action<GUIObject> callback = null) : 
    		base((obj) =>
    		{
    			GUIVerticalDialog _this = obj as GUIVerticalDialog;
    			_this._contentHeight = _this.size().y;
    			_this._scrollDelta = 0;
    			_this._currentScroll = new Vector2(0, 0);
    		})
    	{
    		int readyCounter = 0;
    		Action<GUIObject> initHelper = (obj) =>
    		{
    			Debug.Log("readyCounter: " + readyCounter);
    			if (readyCounter++ < 3)
    				return;

    			base.addChild(background);
    			base.addChild(content);
    			base.addChild(upButton);
    			base.addChild(downButton);

    			background.name("Background");
    			background.position(Vector2.zero);
    			background.size(base.size());
    			background.imageColor(new Color(0, 0, 0, 0));

    			upButton.name("Up Button");
				upButton.anchor(new Vector2(1.0f, 1.0f));
				upButton.position(new Vector2(-ARROW_SIZE.x, -ARROW_SIZE.y / 2), POSITION_TYPE.RELATIVE);
    			upButton.size(ARROW_SIZE);
				upButton.setBaseBackground(_rm.sprite("Textures/GUI/Buttons/UpArrow"), Color.white);
				upButton.setOverBackground(_rm.sprite("Textures/GUI/Buttons/UpArrowHover"), Color.white);
    			upButton.setDownBackground(_rm.sprite("Textures/GUI/Buttons/UpArrowHover"), Color.white);

    			downButton.name("Down Button");
				downButton.anchor(new Vector2(1.0f, 0.0f));
				downButton.position(new Vector2(-ARROW_SIZE.x, ARROW_SIZE.y / 2), POSITION_TYPE.RELATIVE);
				downButton.size(ARROW_SIZE);
				downButton.setBaseBackground(_rm.sprite("Textures/GUI/Buttons/DownArrow"), Color.white);
				downButton.setOverBackground(_rm.sprite("Textures/GUI/Buttons/DownArrowHover"), Color.white);
				downButton.setDownBackground(_rm.sprite("Textures/GUI/Buttons/DownArrowHover"), Color.white);

				content.name("Content");

				IEnumerator scrollUpdate = null;
    			CoroutineManager CO = CoroutineManager.Get();
    			// Up Button ===========================================================================================
    			upButton.addListener(EventTriggerType.PointerDown, (e) =>
    			{
    				_scrollDelta = -SCROLL_DELTA;

    				if (scrollUpdate != null)
    					return;

    				scrollUpdate = this.scrollUpdate();
    				CO.StartCoroutine(scrollUpdate);
				});
    			upButton.addListener(EventTriggerType.PointerUp, (e) =>
    			{
					_scrollDelta = 0;

    				if (scrollUpdate == null)
    					return;

    				CO.StopCoroutine(scrollUpdate);
    				scrollUpdate = null;
				});
    			upButton.addListener(EventTriggerType.PointerExit, (e) =>
    			{
					_scrollDelta = 0;

    				if (scrollUpdate == null)
    					return;

    				CO.StopCoroutine(scrollUpdate);
    				scrollUpdate = null;
    			});

    			// Down Button =========================================================================================
    			downButton.addListener(EventTriggerType.PointerDown, (e) =>
    			{
					_scrollDelta = SCROLL_DELTA;

    				if (scrollUpdate != null)
    					return;

    				scrollUpdate = this.scrollUpdate();
    				CO.StartCoroutine(scrollUpdate);
				});
    			downButton.addListener(EventTriggerType.PointerUp, (e) =>
    			{
					_scrollDelta = 0;

    				if (scrollUpdate == null)
    					return;

    				CO.StopCoroutine(scrollUpdate);
    				scrollUpdate = null;
				});
    			downButton.addListener(EventTriggerType.PointerExit, (e) =>
    			{
					_scrollDelta = 0;

    				if (scrollUpdate == null)
    					return;

    				CO.StopCoroutine(scrollUpdate);
    				scrollUpdate = null;
    			});

    			if (callback != null)
    				callback(this);
    		};

    		background = new GUIImage(initHelper);
    		content = new GUIObject(initHelper);
    		upButton = new GUIButton(initHelper);
    		downButton = new GUIButton(initHelper);
    	}

    	public override bool isReady ()
		{
			return base.isReady() && 
				background.isReady() && 
				content.isReady() && 
				upButton.isReady() && 
				downButton.isReady();
		}

    	public override OVERFLOW_TYPE defaultOverflow()
		{
			return OVERFLOW_TYPE.HIDE;
		}

		public bool backgroundColor(Color color)
		{
			if (!isReady())
				return false;

			background.imageColor(color);

			return true;
		}
		public Color backgroundColor()
		{
			if (!isReady())
				return default(Color);

			return background.imageColor();
		}

		public bool backgroundImage(Sprite sprite)
		{
			if (!isReady())
				return false;

			background.image(sprite);

			return true;
		}
		public Sprite backgroundImage()
		{
			if (!isReady())
				return null;

			return background.image();
		}

		public override bool size(Vector2 size)
		{
			return base.size(size) && background.size(size) && content.size(size) && contentHeight(_contentHeight);
		}

		public bool contentHeight(float contentHeight)
		{
			if (!isReady())
				return false;

			if (contentHeight < size().y)
				contentHeight = size().y;

			_contentHeight = contentHeight;

			if (_contentHeight < _currentScroll.y)
				scrollTo(_contentHeight);

			return true;
		}
		public float contentHeight()
		{
			return _contentHeight;
		}

		public override bool addChild (GUIObject child)
		{
			return content.addChild (child);
		}
		public override GUIObject getChild (string uniqueID)
		{
			return content.getChild (uniqueID);
		}
		public override GUIObject[] getChildren ()
		{
			return content.getChildren ();
		}
		public override GUIObject removeChild (GUIObject obj)
		{
			return content.removeChild (obj);
		}
		public override GUIObject removeChild (string uniqueID)
		{
			return content.removeChild (uniqueID);
		}
		public override GUIObject[] removeAllChildren ()
		{
			return content.removeAllChildren ();
		}
		public override bool destroyAllChildren ()
		{
			return content.destroyAllChildren ();
		}
		public override GUIObject findChildByName (string name)
		{
			return content.findChildByName (name);
		}

		public bool scrollTo(float scrollValue)
		{
			if (scrollValue < 0 || scrollValue > _contentHeight)
				return false;

			_currentScroll.y = scrollValue;
			content.position(_currentScroll, POSITION_TYPE.RELATIVE);

			return true;
		}

		protected virtual IEnumerator scrollUpdate()
		{
			yield return null;

			while(true)
			{
				_currentScroll.y += _scrollDelta;

				if (_currentScroll.y < 0)
					_currentScroll.y = 0;
				else if (_currentScroll.y > _contentHeight - size().y)
					_currentScroll.y = _contentHeight - size().y;

				//Debug.Log("_currentScroll: " + _currentScroll + ", _scrollDelta: " + _scrollDelta);
				content.position(_currentScroll, POSITION_TYPE.RELATIVE);

				yield return null;
			}
		}
	}

    public class GUIHorizontalDialog : GUIContent
    {
    	protected GUIImage background;
    	protected GUIObject content;
    	protected GUIButton leftButton;
    	protected GUIButton rightButton;

    	protected float _contentWidth;
    	protected float _scrollDelta;
    	protected Vector2 _currentScroll;

    	public static readonly float SCROLL_DELTA = 4;

    	public static readonly Vector2 ARROW_SIZE = new Vector2(40, 40);

    	public GUIHorizontalDialog(Action<GUIObject> callback = null) : 
    		base((obj) =>
    		{
    			GUIHorizontalDialog _this = obj as GUIHorizontalDialog;
    			_this._contentWidth = _this.size().x;
    			_this._scrollDelta = 0;
    			_this._currentScroll = new Vector2(0, 0);
    		})
    	{
    		int readyCounter = 0;
    		Action<GUIObject> initHelper = (obj) =>
    		{
    			Debug.Log("readyCounter: " + readyCounter);
    			if (readyCounter++ < 3)
    				return;

				base.addChild(background);
    			base.addChild(content);
    			base.addChild(leftButton);
    			base.addChild(rightButton);

    			background.name("Background");
    			background.position(Vector2.zero);
    			background.size(base.size());
    			background.imageColor(new Color(0, 0, 0, 0));

    			leftButton.name("Left Button");
				leftButton.anchor(new Vector2(0.0f, 0.0f));
				leftButton.position(new Vector2(ARROW_SIZE.x / 2, ARROW_SIZE.y), POSITION_TYPE.RELATIVE);
    			leftButton.size(ARROW_SIZE);
				leftButton.setBaseBackground(_rm.sprite("Textures/GUI/Buttons/LeftArrow"), Color.white);
				leftButton.setOverBackground(_rm.sprite("Textures/GUI/Buttons/LeftArrowHover"), Color.white);
    			leftButton.setDownBackground(_rm.sprite("Textures/GUI/Buttons/LeftArrowHover"), Color.white);

    			rightButton.name("Right Button");
				rightButton.anchor(new Vector2(1.0f, 0.0f));
				rightButton.position(new Vector2(-ARROW_SIZE.x / 2, ARROW_SIZE.y), POSITION_TYPE.RELATIVE);
				rightButton.size(ARROW_SIZE);
				rightButton.setBaseBackground(_rm.sprite("Textures/GUI/Buttons/RightArrow"), Color.white);
				rightButton.setOverBackground(_rm.sprite("Textures/GUI/Buttons/RightArrowHover"), Color.white);
				rightButton.setDownBackground(_rm.sprite("Textures/GUI/Buttons/RightArrowHover"), Color.white);

				content.name("Content");

				IEnumerator scrollUpdate = null;
    			CoroutineManager CO = CoroutineManager.Get();
    			// Up Button ===========================================================================================
    			rightButton.addListener(EventTriggerType.PointerDown, (e) =>
    			{
    				_scrollDelta = -SCROLL_DELTA;

    				if (scrollUpdate != null)
    					return;

    				scrollUpdate = this.scrollUpdate();
    				CO.StartCoroutine(scrollUpdate);
				});
    			rightButton.addListener(EventTriggerType.PointerUp, (e) =>
    			{
					_scrollDelta = 0;

    				if (scrollUpdate == null)
    					return;

    				CO.StopCoroutine(scrollUpdate);
    				scrollUpdate = null;
				});
    			rightButton.addListener(EventTriggerType.PointerExit, (e) =>
    			{
					_scrollDelta = 0;

    				if (scrollUpdate == null)
    					return;

    				CO.StopCoroutine(scrollUpdate);
    				scrollUpdate = null;
    			});

    			// Down Button =========================================================================================
    			leftButton.addListener(EventTriggerType.PointerDown, (e) =>
    			{
					_scrollDelta = SCROLL_DELTA;

    				if (scrollUpdate != null)
    					return;

    				scrollUpdate = this.scrollUpdate();
    				CO.StartCoroutine(scrollUpdate);
				});
    			leftButton.addListener(EventTriggerType.PointerUp, (e) =>
    			{
					_scrollDelta = 0;

    				if (scrollUpdate == null)
    					return;

    				CO.StopCoroutine(scrollUpdate);
    				scrollUpdate = null;
				});
    			leftButton.addListener(EventTriggerType.PointerExit, (e) =>
    			{
					_scrollDelta = 0;

    				if (scrollUpdate == null)
    					return;

    				CO.StopCoroutine(scrollUpdate);
    				scrollUpdate = null;
    			});

    			if (callback != null)
    				callback(this);
    		};

			content = new GUIObject(initHelper);
    		background = new GUIImage(initHelper);
    		leftButton = new GUIButton(initHelper);
    		rightButton = new GUIButton(initHelper);
    	}

    	public override bool isReady ()
		{
			return base.isReady() && 
				background.isReady() && 
				content.isReady() && 
				leftButton.isReady() && 
				rightButton.isReady();
		}

    	public override OVERFLOW_TYPE defaultOverflow()
		{
			return OVERFLOW_TYPE.HIDE;
		}

		public bool backgroundColor(Color color)
		{
			if (!isReady())
				return false;

			background.imageColor(color);

			return true;
		}
		public Color backgroundColor()
		{
			if (!isReady())
				return default(Color);

			return background.imageColor();
		}

		public bool backgroundImage(Sprite sprite)
		{
			if (!isReady())
				return false;

			background.image(sprite);

			return true;
		}
		public Sprite backgroundImage()
		{
			if (!isReady())
				return null;

			return background.image();
		}

		public override bool size(Vector2 size)
		{
			return base.size(size) && background.size(size) && content.size(size) && contentWidth(_contentWidth);
		}

		public bool scrollButtonPosition(float y)
		{
			if (!isReady())
				return false;

			return leftButton.position(new Vector2(leftButton.position().x, y), POSITION_TYPE.RELATIVE) && 
				rightButton.position(new Vector2(rightButton.position().x, y), POSITION_TYPE.RELATIVE);
		}

		public bool contentWidth(float contentWidth)
		{
			if (!isReady())
				return false;

			if (contentWidth < size().y)
				contentWidth = size().y;

			_contentWidth = contentWidth;

			if (_contentWidth < _currentScroll.x)
				scrollTo(_contentWidth);

			return true;
		}
		public float contentWidth()
		{
			return _contentWidth;
		}

		public override bool addChild (GUIObject child)
		{
			return content.addChild (child);
		}
		public override GUIObject getChild (string uniqueID)
		{
			return content.getChild (uniqueID);
		}
		public override GUIObject removeChild (GUIObject obj)
		{
			return content.removeChild (obj);
		}
		public override GUIObject removeChild (string uniqueID)
		{
			return content.removeChild (uniqueID);
		}
		public override GUIObject[] removeAllChildren ()
		{
			return content.removeAllChildren ();
		}

		public bool scrollTo(float scrollValue)
		{
			if (scrollValue < 0 || scrollValue > _contentWidth)
				return false;

			_currentScroll.x = -scrollValue;
			content.position(_currentScroll, POSITION_TYPE.RELATIVE);

			return true;
		}

		protected virtual IEnumerator scrollUpdate()
		{
			yield return null;

			while(true)
			{
				_currentScroll.x += _scrollDelta;

				if (_currentScroll.x < -(_contentWidth - size().x))
					_currentScroll.x = -(_contentWidth - size().x);
				else if (_currentScroll.x > 0)
					_currentScroll.x = 0;

				//Debug.Log("_currentScroll: " + _currentScroll + ", _scrollDelta: " + _scrollDelta);
				content.position(_currentScroll, POSITION_TYPE.RELATIVE);

				yield return null;
			}
		}
    }

    public class GUICheckbox : GUIObject
    {
    	public enum STATE
    	{
    		CHECKED,
    		UNCHECKED
    	}

    	protected STATE _state;
    	protected GUIImage _checkbox;
    	protected GUIText _label;

    	public Action<STATE> onToggle;

    	public static readonly Vector2 CHECKBOX_SIZE = new Vector2(20, 20);
    	public static readonly float PADDING = 8;

    	public GUICheckbox(Action<GUIObject> callback = null) : base()
    	{
    		_state = STATE.UNCHECKED;

    		Action<GUIObject> initHelper = (obj) =>
    		{
				if (!isReady())
					return;

				size(new Vector2(100, 20));

				addChild(_checkbox);
				addChild(_label);

				_checkbox.size(CHECKBOX_SIZE);
				_checkbox.anchor(new Vector2(0.0f, 0.5f));
				_checkbox.position(new Vector2(CHECKBOX_SIZE.x / 2, 0), POSITION_TYPE.RELATIVE);
				_checkbox.imageColor(Color.white);
				_checkbox.addListener(EventTriggerType.PointerClick, (e) =>
				{
					switch (_state)
					{
						case STATE.CHECKED:
							_checkbox.imageColor(Color.white);
							_state = STATE.UNCHECKED;
							break;
						case STATE.UNCHECKED:
							_checkbox.imageColor(Color.blue);
							_state = STATE.CHECKED;
							break;
					}

					if (onToggle != null)
						onToggle(_state);
				});
				_checkbox.addListener(EventTriggerType.PointerEnter, (e) =>
				{
					_checkbox.imageColor(new Color(0.3f, 0.3f, 1.0f));
				});
				_checkbox.addListener(EventTriggerType.PointerExit, (e) =>
				{
					switch (_state)
					{
						case STATE.CHECKED:
							_checkbox.imageColor(Color.blue);
							break;
						case STATE.UNCHECKED:
							_checkbox.imageColor(Color.white);
							break;
					}
				});

				_label.size(new Vector2(size().x - CHECKBOX_SIZE.x - PADDING, CHECKBOX_SIZE.y));
				_label.anchor(new Vector2(1.0f, 0.5f));
				_label.position(new Vector2(-_label.size().x / 2, 0), POSITION_TYPE.RELATIVE);
				_label.text("Checkbox");
				_label.fontSize(14);
				_label.font(_rm.font("Fonts/Sans"));

				if (callback != null)
					callback(this);
    		};

    		_checkbox = new GUIButton(initHelper);
    		_label = new GUIText(initHelper);
		}

		protected override void styleInit ()
		{
			base.styleInit ();

            fontColor(defaultFontColor());
            font(defaultFont());
            fontSize(defaultFontSize());
		}

    	public override bool isReady ()
		{
			return base.isReady () && _checkbox.isReady() && _label.isReady();
		}

		public virtual bool state(STATE state)
		{
			if (!isReady())
				return false;

			switch (state)
			{
				case STATE.CHECKED:
					_checkbox.imageColor(Color.red);
					break;
				case STATE.UNCHECKED:
					_checkbox.imageColor(Color.white);
					break;
			}

			_state = state;

			if (onToggle != null)
				onToggle(_state);

			return true;
		}
		public virtual STATE state()
		{
			return _state;
		}

		public virtual bool text(string text)
		{
			if (!isReady())
				return false;

			_label.text(text);

			return true;
		}
		public virtual string text()
		{
			if (!isReady())
				return "";

			return _label.text();
   		}

		public virtual bool fontColor(Color color)
		{
        	if (!isReady())
        		return false;

        	return _label.fontColor(color);
        }
		public virtual Color fontColor()
		{
        	if (!isReady())
        		return Color.black;

        	return _label.fontColor();
        }
        public virtual Color defaultFontColor()
        {
        	return Color.black;
        }

		public virtual bool font(Font font)
		{
        	if (!isReady())
        		return false;

			if (font == null)
        		return false;

        	return _label.font(font);
        }
		public virtual Font font()
		{
        	if (!isReady())
        		return null;

        	return _label.font();
        }
        public virtual Font defaultFont()
        {
			return new Font("Arial");
        }

        public virtual bool fontSize(int fontSize)
        {
        	if (!isReady())
        		return false;

        	if (fontSize < 1)
        		return false;

        	return _label.fontSize(fontSize);
        }
        public virtual int fontSize()
        {
        	if (!isReady())
        		return 0;

        	return _label.fontSize();
        }
        public virtual int defaultFontSize()
        {
        	return 14;
        }

		public override bool size(Vector2 size)
		{
			if (!isReady())
				return false;

			if (size.x < CHECKBOX_SIZE.x + PADDING)
				return false;

			return base.size(new Vector2(size.x, CHECKBOX_SIZE.y)) && 
				_label.size(new Vector2(base.size().x - CHECKBOX_SIZE.x - PADDING, CHECKBOX_SIZE.y)) &&
				_label.position(new Vector2(-_label.size().x / 2, 0), POSITION_TYPE.RELATIVE);
		}
    }

    public class GUIDropDown : GUIObject
    {
    	public Action<string, string> onSelect;

    	protected GUIImage _arrow;
    	protected GUIText _selected;
    	protected GUIImage _mainBack;
    	protected GUIImage _optionContainer;
    	protected List<GUIText> _options;

    	public GUIDropDown(Action<GUIObject> callback = null) : base()
    	{
    		_options = new List<GUIText>();

    		Action<GUIObject> initHelper = (obj) =>
    		{
    			if (!isReady())
					return;

				styleInit();

				addChild(_mainBack);
				_mainBack.position(Vector2.zero);
				_mainBack.size(_selected.size());

				_mainBack.size(size());
				_mainBack.imageColor(Color.white);

    			addChild(_selected);
    			_selected.position(Vector2.zero);
    			_selected.size(size());
    			_selected.font(_rm.font("Fonts/Sans"));
    			_selected.fontSize(14);
				_selected.fontColor(Color.black);

				_selected.addListener(EventTriggerType.PointerEnter, (e) =>
				{
					_selected.fontColor(Color.blue);
					_arrow.image(_rm.sprite("Textures/GUI/Buttons/DownArrowHover"));
				});
				_selected.addListener(EventTriggerType.PointerExit, (e) =>
				{
					_selected.fontColor(Color.black);
					_arrow.image(_rm.sprite("Textures/GUI/Buttons/DownArrow"));
				});
				_selected.addListener(EventTriggerType.PointerClick, (e) =>
				{
					if (_optionContainer.visible())
						_optionContainer.hide();
					else
						_optionContainer.show();
				});

    			addChild(_arrow);
    			_arrow.anchor(new Vector2(1.0f, 0.5f));
    			_arrow.position(new Vector2(- size().y / 2, 0.0f), POSITION_TYPE.RELATIVE);
    			_arrow.size(new Vector2(size().y, size().y));
				_arrow.image(_rm.sprite("Textures/GUI/Buttons/DownArrow"));

				_arrow.addListener(EventTriggerType.PointerClick, (e) =>
				{
					Debug.Log("Showing container!");

					_optionContainer.show();
				});

				addChild(_optionContainer);
				_optionContainer.anchor(new Vector2(0.5f, 0.0f));
				_optionContainer.size(size());
				_optionContainer.position(new Vector2(0, -_optionContainer.size().y / 2), POSITION_TYPE.RELATIVE);
				_optionContainer.imageColor(Color.white);
				_optionContainer.hide();

				callback(this);
    		};

    		_selected = new GUIText(initHelper);
    		_arrow = new GUIImage(initHelper);
    		_optionContainer = new GUIImage(initHelper);
    		_mainBack = new GUIImage(initHelper);
    	}

    	public override bool isReady ()
		{
			return base.isReady() &&
				_selected.isReady() && 
				_mainBack.isReady() && 
				_arrow.isReady() && 
				_optionContainer.isReady();
		}

    	public override Vector2 defaultSize ()
		{
			return new Vector2(80, 20);
   		}

   		public override bool size (Vector2 size)
		{
			if (!isReady())
				return false;

			if (base.size(size) && 
				_mainBack.size(size) && 
				_selected.size(size) && 
				_arrow.size(new Vector2(size.y, size.y)))
			{
				foreach (GUIObject obj in _optionContainer.removeAllChildren())
					obj.destroy();

				_optionContainer.size(size);
				_optionContainer.position(new Vector2(0, -_optionContainer.size().y / 2), POSITION_TYPE.RELATIVE);

				List<GUIText> options = _options;
				_options = new List<GUIText>();

				foreach (GUIText option in options)
				{
					addOption(option.text());
					option.destroy();
				}

				options.Clear();


				return true;
			}

			return false;
		}

		public virtual void addOption(string option)
		{
			new GUIText((obj) =>
			{
				GUIText text = obj as GUIText;

				_optionContainer.addChild(text);
				text.anchor(new Vector2(0.5f, 0.0f));
				text.size(size());
				text.text(option);
				text.font(_rm.font("Fonts/Sans"));
    			text.fontSize(14);
    			text.fontColor(Color.black);

    			text.addListener(EventTriggerType.PointerExit, (e) =>
    			{
    				if (_selected.text() == text.text())
    					return;

    				text.fontColor(Color.black);
    			});
    			text.addListener(EventTriggerType.PointerEnter, (e) =>
    			{
    				text.fontColor(Color.blue);
    			});
				text.addListener(EventTriggerType.PointerClick, (e) =>
				{
					_selected.fontColor(Color.black);
					text.fontColor(Color.blue);

					_onSelect(text);

					_optionContainer.hide();
				});

				Vector2 oldOptionsSize = _optionContainer.size();

				if (_options.Count == 0)
				{
					_selected.text(option);
					text.position(new Vector2(0, size().y / 2), POSITION_TYPE.RELATIVE);
				}
				else
				{
					text.position(new Vector2(0, oldOptionsSize.y + size().y / 2), POSITION_TYPE.RELATIVE);
					_optionContainer.size(new Vector2(oldOptionsSize.x, oldOptionsSize.y + size().y));
					_optionContainer.position(new Vector2(0, -_optionContainer.size().y / 2), POSITION_TYPE.RELATIVE);
				}

				_options.Add(text);
			});
		}

		protected virtual void _onSelect(GUIText selectedOption)
		{
			string oldSelect = _selected.text();
			_selected.text(selectedOption.text());

			if (onSelect != null)
				onSelect(oldSelect, _selected.text());
		}
    }
}
