using System;
using System.Collections.Generic;

using Utilities;

using UnityEngine;

namespace Input
{
	public class InputListener : Manager<InputListener>
	{
		private List<Pair<Hotkey, Action<Hotkey>>> _hotkeyListeners;
		private List<Pair<ButtonCombo, Action<Axis>>> _axisListeners;
		private List<Pair<ButtonCombo, Action<Joystick>>> _joystickListeners;
		private List<Pair<ButtonCombo, Action<Mouse>>> _mouseListeners;

		protected override void Awaken ()
		{
			_hotkeyListeners = new List<Pair<Hotkey, Action<Hotkey>>>();
			_axisListeners = new List<Pair<ButtonCombo, Action<Axis>>>();
			_joystickListeners = new List<Pair<ButtonCombo, Action<Joystick>>>();
			_mouseListeners = new List<Pair<ButtonCombo, Action<Mouse>>>();
		}

		protected override void Initialize ()
		{ }

		public void addListener(Hotkey hotkey, Action<Hotkey> listener)
		{
			if (hotkey == null || listener == null)
				return;

			_hotkeyListeners.Add(new Pair<Hotkey, Action<Hotkey>>(hotkey, listener));
		}

		protected void Update()
		{
			foreach (Pair<Hotkey, Action<Hotkey>> pair in _hotkeyListeners)
				if (pair.first.getDown())
					pair.second(pair.first);
		}
	}
}
