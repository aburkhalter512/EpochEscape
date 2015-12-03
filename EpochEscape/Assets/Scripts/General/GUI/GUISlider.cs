using System;
using Input;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GUI
{
	public class GUISlider
	{
		/*private GUIObject _rail;
		private GUIObject _slider;
		private bool _isVert;

		public Action<float, float> onValueChanged;
		public float minVal;
		public float maxVal;

		private static Mouse mouse;

		public GUISlider(bool isVertical = false, Action<GUISlider> callback = null)
		{
			_isVert = isVertical;
			onValueChanged = null;
			minVal = -1;
			maxVal = 1;

			if (mouse == null)
				mouse = Mouse.Get();

			_rail = new GUIObject((railObj) =>
			{
				
			});
			_slider = new GUIObject((sliderObj) =>
			{
				sliderObj.name("slider");
				bool isDown = false;

				addChild(sliderObj);

				sliderObj.position(new Vector2(0, 0), GUIObject.POSITION_TYPE.PERCENT);
				sliderObj.addListener(EventTriggerType.BeginDrag, (e) =>
				{
					isDown = true;
					float initialValue = value();

					Vector2 pos = Utilities.Math.toVector2(mouse.inScreen()) - sliderObj.parent().position(GUIObject.POSITION_TYPE.ABSOLUTE);
					if (_isVert)
					{
						pos.x = 0;
						float yRange = _rail.size().y / 2;
						if (pos.y / yRange > 1)
							pos.y = yRange;
						else if (pos.y / yRange < -1)
							pos.y = -yRange;
					}
					else
					{
						pos.y = 0;
						float xRange = _rail.size().x / 2;
						if (pos.x / xRange > 1)
							pos.x = xRange;
						else if (pos.x / xRange < -1)
							pos.x = -xRange;
					}
					sliderObj.position(pos, GUIObject.POSITION_TYPE.RELATIVE);

					if (onValueChanged != null)
						onValueChanged(initialValue, value());
				});
				sliderObj.addListener(EventTriggerType.Drag, (e) =>
				{
					if (!isDown)
						return;

					float initialValue = value();

					Vector2 pos = Utilities.Math.toVector2(mouse.inScreen()) - sliderObj.parent().position(GUIObject.POSITION_TYPE.ABSOLUTE);
					if (_isVert)
					{
						pos.x = 0;
						float yRange = _rail.size().y / 2;
						if (pos.y / yRange > 1)
							pos.y = yRange;
						else if (pos.y / yRange < -1)
							pos.y = -yRange;
					}
					else
					{
						pos.y = 0;
						float xRange = _rail.size().x / 2;
						if (pos.x / xRange > 1)
							pos.x = xRange;
						else if (pos.x / xRange < -1)
							pos.x = -xRange;
					}
					sliderObj.position(pos, GUIObject.POSITION_TYPE.RELATIVE);

					if (onValueChanged != null)
						onValueChanged(initialValue, value());
				});
				sliderObj.addListener(EventTriggerType.EndDrag, (e) =>
				{
					isDown = false;	
				});

				callback(this);
			});
		}

		public bool isReady()
		{
			return _rail != null && _slider != null && _rail.isReady() && _slider.isReady();
		}

		public GUIObject rail()
		{
			if (_rail == null || _slider == null || !_rail.isReady() || !_slider.isReady())
				return null;

			return _rail;
		}
		public GUIObject slider()
		{
			if (_rail == null || _slider == null || !_rail.isReady() || !_slider.isReady())
				return null;

			return _rail;
		}

		public bool value(float value)
		{
			if (!isReady())
				return false;

			if (!isValidValue(value))
				return false;

			float avgVal = (maxVal + minVal) / 2;
			float percentVal = (value - avgVal) / avgVal;

			float initialValue = this.value();

			Vector2 pos = Vector2.zero;
			if (_isVert)
				pos.y = percentVal;
			else
				pos.x = percentVal;

			_slider.position(pos, GUIObject.POSITION_TYPE.PERCENT);

			if (onValueChanged != null)
				onValueChanged(initialValue, this.value());

			return true;
		}

		public float value()
		{
			if (!isReady())
				return 0.0f;

			float value;
			float avgVal = (maxVal + minVal) / 2;

			if (_isVert)
				value = _slider.position(GUIObject.POSITION_TYPE.PERCENT).y;
			else
				value = _slider.position(GUIObject.POSITION_TYPE.PERCENT).x;
			
			value = avgVal + avgVal * value;

			return value;
		}

		public bool isValidValue(float value)
		{
			return (value > minVal && value < maxVal) || 
				(value > minVal && value < maxVal);
		}*/
	}
}
