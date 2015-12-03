using UnityEngine;
using System;

namespace Input
{
    public class Hotkey
    {
        public enum Modifier
        {
            CONTROL = 0,
            SHIFT,
            ALT
        }

        #region Instance Variables
        private ButtonCombo _keys;
        private bool[] _modifiers;

        // ButtonCombos because there are left and right buttons for each
		private static Button _leftControl = null;
        private static Button _rightControl = null;
		private static Button _leftShift = null;
        private static Button _rightShift = null;
		private static Button _leftAlt = null;
        private static Button _rightAlt = null;
        #endregion
        public Hotkey(Button button, Modifier[] modifiers = null)
        {
            _keys = new ButtonCombo(new[] { button });

            initModifiers();

            _modifiers = new bool[Enum.GetNames(typeof(Modifier)).Length];
            for (int i = 0; i < _modifiers.Length; i++)
                _modifiers[i] = false;

            setModifiers(modifiers);
        }

        public Hotkey(Button[] buttons, Modifier[] modifiers = null)
        {
            _keys = new ButtonCombo(buttons);

            initModifiers();

            _modifiers = new bool[Enum.GetNames(typeof(Modifier)).Length];
            for (int i = 0; i < _modifiers.Length; i++)
                _modifiers[i] = false;

            setModifiers(modifiers);
        }

        #region Interface Methods
        public bool get()
        {
            return areModsGood() && _keys.get();
        }

        public bool getDown()
        {
            return areModsGood() && _keys.getDown();
        }

        public bool getUp()
        {
            return areModsGood() && _keys.getUp();
        }

        public void setModifiers(Modifier[] modifiers)
        {
        	if (modifiers == null)
        		return;

            foreach (Modifier mod in modifiers)
                _modifiers[(int) mod] = true;
        }

        public void setInput(Button[] keys)
        {
            _keys.setInput(keys);
        }

        public Button[] getInput()
        {
            return _keys.getInput();
        }
        #endregion

        #region Instance Methods
        private bool areModsGood()
        {
            for (int i = 0; i < _modifiers.Length; i++)
            {
                switch ((Modifier)i)
                {
                    case Modifier.CONTROL:
                		if (!(_leftControl.get() || _rightControl.get()) == _modifiers[i])
                			return false;
                        break;
                    case Modifier.SHIFT:
						if (_modifiers[i] != _leftShift.get() || _modifiers[i] != _rightShift.get())
                            return false;
                        break;
                    case Modifier.ALT:
						if (_modifiers[i] != _leftAlt.get() || _modifiers[i] != _rightAlt.get())
                            return false;
                        break;
                }
            }

            return true;
        }

        private static void initModifiers()
        {
            if (_leftControl != null)
                return;

            _leftControl = new Button(KeyCode.LeftControl);
            _rightControl = new Button(KeyCode.RightControl);

            _leftShift = new Button(KeyCode.LeftShift);
            _rightShift = new Button(KeyCode.RightShift);

            _leftAlt = new Button(KeyCode.LeftAlt);
            _rightAlt = new Button(KeyCode.RightAlt);
        }
        #endregion
    }
}
