using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;

namespace Utilities.Abstractions
{
    /// <summary>
    /// The complete input state for a given frame
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1815:Override equals and operator equals on value types", Justification = "<Pending>")]
    public struct FrameState
    {
        public GamePadState GamePadState { get; set; }
        public KeyboardState KeyboardState { get; set; }
        public MouseState MouseState { get; set; }
        public TouchCollection Touches { get; set; }
    }
}
