using UnityEngine;
using Input;

namespace Editor
{
    public class InputManager : Manager<InputManager>
    {
        public Joystick primaryJoystick;
        public Joystick secondaryJoystick;
        public Hotkey primaryPlace;

        public Hotkey objectSelector;
        public Hotkey multiObjectSelector;

        public Axis objectChanger;
        public Hotkey objectDeleter;

        public Hotkey rotate;
        public Hotkey revRotate;

        public Hotkey toggleActivate;

        public Joystick cameraJoystick;
        public Axis cameraZoomNormal;
        public Axis cameraZoomInPlace;

        public Hotkey save;
        public Hotkey open;
        public Hotkey play;
        public Hotkey stop;
        public Hotkey exit;

        public Mouse mouse;

        protected override void Awaken()
        {
            Hotkey.Modifier[] cntlMod = new[] { Hotkey.Modifier.CONTROL };
            Hotkey.Modifier[] shiftCntlMod = new[] { Hotkey.Modifier.CONTROL, Hotkey.Modifier.SHIFT };
            Hotkey.Modifier[] altMod = new[] { Hotkey.Modifier.ALT };

            mouse = Mouse.Get();

            primaryJoystick = new Joystick(
                // Horizontal
                new Axis(
                    new Hotkey(new Button(KeyCode.LeftArrow)), 
                    new Hotkey(new Button(KeyCode.RightArrow))
                ), 
                // Vertical
                new Axis(
                    new Hotkey(new Button(KeyCode.DownArrow)), 
                    new Hotkey(new Button(KeyCode.UpArrow))
                )
            );
            secondaryJoystick = new Joystick(
                // Horizontal
                new Axis(
                    new Hotkey(new Button(KeyCode.LeftArrow), cntlMod),
                    new Hotkey(new Button(KeyCode.RightArrow), cntlMod)
                ),
                // Vertical
                new Axis(
                    new Hotkey(new Button(KeyCode.DownArrow), cntlMod),
                    new Hotkey(new Button(KeyCode.UpArrow), cntlMod)
                )
            );

            primaryPlace = new Hotkey(new Button(KeyCode.Return));

            objectSelector = new Hotkey(mouse.button(Mouse.BUTTONS.LEFT));

            multiObjectSelector = new Hotkey(
                mouse.button(Mouse.BUTTONS.LEFT),
                cntlMod
            );

            objectChanger = new Axis(
                new Hotkey(
                    new Button(KeyCode.LeftArrow), 
                    cntlMod
                ),
                new Hotkey(
                    new Button(KeyCode.RightArrow), 
                    cntlMod
                )
            );

            objectDeleter = new Hotkey(new Button(KeyCode.Backspace));

            rotate = new Hotkey(new Button(KeyCode.R));
            revRotate = new Hotkey(
                new Button(KeyCode.R), 
                cntlMod
            );

            toggleActivate = new Hotkey(new Button(KeyCode.A));

            cameraJoystick = new Joystick(
                // Horizontal
                new Axis(
                    new Hotkey(new Button(KeyCode.LeftArrow), cntlMod),
                    new Hotkey(new Button(KeyCode.RightArrow), cntlMod)
                ),
                // Vertical
                new Axis(
                    new Hotkey(new Button(KeyCode.DownArrow), cntlMod),
                    new Hotkey(new Button(KeyCode.UpArrow), cntlMod)
                )
            );

            cameraZoomNormal = new Axis(
                new Button(KeyCode.Minus),
                new Button(KeyCode.Plus)
            );

            cameraZoomInPlace = new Axis(
                new Hotkey(new Button(KeyCode.Minus), cntlMod),
                new Hotkey(new Button(KeyCode.Plus), cntlMod)
            );

            save = new Hotkey(new Button(KeyCode.S), cntlMod);
            open = new Hotkey(new Button(KeyCode.O), cntlMod);
            play = new Hotkey(new Button(KeyCode.P), cntlMod);
            stop = new Hotkey(new Button(KeyCode.Escape), cntlMod);
            exit = new Hotkey(new Button(KeyCode.Escape));
        }

        protected override void Initialize() { }
    }
}
