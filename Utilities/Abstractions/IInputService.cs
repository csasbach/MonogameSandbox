using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Linq.Expressions;

namespace Utilities.Abstractions
{
    /// <summary>
    /// Provides standard input methods for use by the game and game components
    /// </summary>
    public interface IInputService
    {
        Point MousePosition { get; }
        void CacheInput();
        void OnHeld(Action del, Expression<Func<GamePadState, ButtonState>> buttonState, Expression<Func<MouseState, ButtonState>> mouseButtonState, params Keys[] keys);
        void OnHeld(Action del, Expression<Func<GamePadState, ButtonState>> buttonState, params Keys[] keys);
        void OnPressed(Action del, Expression<Func<GamePadButtons, ButtonState>> buttonState, Expression<Func<MouseState, ButtonState>> mouseButtonState, params Keys[] keys);
        void OnPressed(Action del, Expression<Func<GamePadButtons, ButtonState>> buttonState, params Keys[] keys);
        void OnReleased(Action del, Expression<Func<GamePadButtons, ButtonState>> buttonState, Expression<Func<MouseState, ButtonState>> mouseButtonState, params Keys[] keys);
        void OnReleased(Action del, Expression<Func<GamePadButtons, ButtonState>> buttonState, params Keys[] keys);
        void OnScrollWheelChanged(Action<float> del);
    }
}