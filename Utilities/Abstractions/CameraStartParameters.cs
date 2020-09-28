using Microsoft.Xna.Framework;

namespace Utilities.Abstractions
{
    public class CameraStartParameters
    {
        public Vector2? Position { get; set; }
        public float? Zoom { get; set; }
        public float? Rotation { get; set; }
    }
}
