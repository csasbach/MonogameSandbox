using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using System;
using System.Linq;
using System.Linq.Expressions;
using Utilities.Abstractions;

namespace Utilities.Services
{
    public class InputService : ComponentService, IInputService
    {
        private FrameState _currentFrame;
        private FrameState _previousFrame;
        public float ScrollWheelSensitivity { get; set; } = 0.01f;

        public InputService(Game game) : base(game, typeof(IInputService)) { }

        public override void Update(GameTime gameTime)
        {
            CacheInput();

            base.Update(gameTime);
        }

        /// <summary>
        /// Allows thie input service to make decisions based on the state of the previous frame
        /// </summary>
        public void CacheInput()
        {
            _previousFrame = _currentFrame;
            _currentFrame = new FrameState
            {
                GamePadState = GamePad.GetState(PlayerIndex.One),
                KeyboardState = Keyboard.GetState(),
                MouseState = Mouse.GetState(),
                Touches = TouchPanel.GetState()
            };
        }

        /*
         * Create methods below to handle various forms if input.
         * Method signatures should stay close to the following template:
         * 
         * public void OnSomeInput(SomeDelegateType del, SomeExpressionType mappingExpression, ..., SomeOtherExpressionType someOtherMapping, params Keys[] keys)
         * 
         * Where SomeDelegateType is a delegate that can be passed that delares the actual method that will be called when the input occurs
         * Where SomeExpressionType, ..., and SomeOtherExpressionType are expressions that are used to map to a state property on some device like a gamepad or mouse
         * Where params Keys[] is a param list of zero to many Keys inputs
         */

        /// <summary>
        /// Passed action to be performed only if the passed gamepad button,
        /// the passed mouse button,
        /// or one of the passed keys has just been released.
        /// </summary>
        /// <param name="del"></param>
        /// <param name="buttonState"></param>
        /// <param name="mouseButtonState"></param>
        /// <param name="keys"></param>
        public void OnReleased(Action del, Expression<Func<GamePadButtons, ButtonState>> buttonState, Expression<Func<MouseState, ButtonState>> mouseButtonState, params Keys[] keys)
        {
            if (buttonState.Compile().Invoke(_currentFrame.GamePadState.Buttons) == ButtonState.Released &&
                buttonState.Compile().Invoke(_previousFrame.GamePadState.Buttons) != ButtonState.Released ||
                mouseButtonState?.Compile().Invoke(_currentFrame.MouseState) == ButtonState.Released &&
                mouseButtonState?.Compile().Invoke(_previousFrame.MouseState) != ButtonState.Released ||
                keys.Any(k => _currentFrame.KeyboardState.IsKeyUp(k) &&
                !_previousFrame.KeyboardState.IsKeyUp(k)))
            {
                del();
            }
        }

        /// <summary>
        /// Passed action to be performed only if either the passed gamepad button
        /// or one of the passed keys has just been released.
        /// </summary>
        /// <param name="del"></param>
        /// <param name="buttonState"></param>
        /// <param name="keys"></param>
        public void OnReleased(Action del, Expression<Func<GamePadButtons, ButtonState>> buttonState, params Keys[] keys)
        {
            OnReleased(del, buttonState, null, keys);
        }

        /// <summary>
        /// Passed action to be performed only if the passed gamepad button,
        /// the passed mouse button,
        /// or one of the passed keys has just been pressed.
        /// </summary>
        /// <param name="del"></param>
        /// <param name="buttonState"></param>
        /// <param name="mouseButtonState"></param>
        /// <param name="keys"></param>
        public void OnPressed(Action del, Expression<Func<GamePadButtons, ButtonState>> buttonState, Expression<Func<MouseState, ButtonState>> mouseButtonState, params Keys[] keys)
        {
            if (buttonState.Compile().Invoke(_currentFrame.GamePadState.Buttons) == ButtonState.Pressed &&
                buttonState.Compile().Invoke(_previousFrame.GamePadState.Buttons) != ButtonState.Pressed ||
                mouseButtonState?.Compile().Invoke(_currentFrame.MouseState) == ButtonState.Pressed &&
                mouseButtonState?.Compile().Invoke(_previousFrame.MouseState) != ButtonState.Pressed ||
                keys.Any(k => _currentFrame.KeyboardState.IsKeyDown(k) &&
                !_previousFrame.KeyboardState.IsKeyDown(k)))
            {
                del();
            }
        }

        /// <summary>
        /// Passed action to be performed only if either the passed gamepad button
        /// or one of the passed keys has just been pressed.
        /// </summary>
        /// <param name="del"></param>
        /// <param name="buttonState"></param>
        /// <param name="keys"></param>
        public void OnPressed(Action del, Expression<Func<GamePadButtons, ButtonState>> buttonState, params Keys[] keys)
        {
            OnPressed(del, buttonState, null, keys);
        }

        /// <summary>
        /// Passed action to be performed only if the passed gamepad button,
        /// the passed mouse button,
        /// or one of the passed keys has been held down.
        /// </summary>
        /// <param name="del"></param>
        /// <param name="buttonState"></param>
        /// <param name="mouseButtonState"></param>
        /// <param name="keys"></param>
        public void OnHeld(Action del, Expression<Func<GamePadState, ButtonState>> buttonState, Expression<Func<MouseState, ButtonState>> mouseButtonState, params Keys[] keys)
        {
            if (buttonState.Compile().Invoke(_currentFrame.GamePadState) == ButtonState.Pressed &&
                buttonState.Compile().Invoke(_previousFrame.GamePadState) == ButtonState.Pressed ||
                mouseButtonState?.Compile().Invoke(_currentFrame.MouseState) == ButtonState.Pressed &&
                mouseButtonState?.Compile().Invoke(_previousFrame.MouseState) == ButtonState.Pressed ||
                keys.Any(k => _currentFrame.KeyboardState.IsKeyDown(k) &&
                _previousFrame.KeyboardState.IsKeyDown(k)))
            {
                del();
            }
        }

        /// <summary>
        /// Passed action to be performed only if either the passed gamepad button
        /// or one of the passed keys has been held down.
        /// </summary>
        /// <param name="del"></param>
        /// <param name="buttonState"></param>
        /// <param name="keys"></param>
        public void OnHeld(Action del, Expression<Func<GamePadState, ButtonState>> buttonState, params Keys[] keys)
        {
            OnHeld(del, buttonState, null, keys);
        }

        /// <summary>
        /// Passed action to be performed only if the scroll wheel value has changed.
        /// Action will be passed the value of the scroll wheel change multiplied by
        /// the ScrollWheelSensitivity
        /// </summary>
        /// <param name="del"></param>
        public void OnScrollWheelChanged(Action<float> del)
        {
            var current = _currentFrame.MouseState.ScrollWheelValue;
            var previous = _previousFrame.MouseState.ScrollWheelValue;
            if (current != previous) del((current - previous) * ScrollWheelSensitivity);
        }
    }
}
