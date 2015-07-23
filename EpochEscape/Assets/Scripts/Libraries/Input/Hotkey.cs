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
        private static ButtonCombo _control;
        private static ButtonCombo _shift;
        private static ButtonCombo _alt;
        #endregion
        public Hotkey(Button button, Modifier[] modifiers = null)
        {
            _keys = new ButtonCombo(new[] { button });

            _modifiers = new bool[Enum.GetNames(typeof(Modifier)).Length];
            for (int i = 0; i < _modifiers.Length; i++)
                _modifiers[i] = false;

            setModifiers(modifiers);
        }

        public Hotkey(Button[] buttons, Modifier[] modifiers = null)
        {
            _keys = new ButtonCombo(buttons);

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
                        if (_modifiers[i] != _control.get())
                            return false;
                        break;
                    case Modifier.SHIFT:
                        if (_modifiers[i] != _shift.get())
                            return false;
                        break;
                    case Modifier.ALT:
                        if (_modifiers[i] != _alt.get())
                            return false;
                        break;
                }
            }

            return true;
        }

        private static void initModifiers()
        {
            if (_control == null)
                return;

            _control = new ButtonCombo(new [] {
                new Button(KeyCode.LeftControl), 
                new Button(KeyCode.RightControl)
            });

            _shift = new ButtonCombo(new[] {
                new Button(KeyCode.LeftShift), 
                new Button(KeyCode.RightShift)
            });

            _alt = new ButtonCombo(new[] {
                new Button(KeyCode.LeftAlt), 
                new Button(KeyCode.RightAlt)
            });
        }
        #endregion
    }
}
