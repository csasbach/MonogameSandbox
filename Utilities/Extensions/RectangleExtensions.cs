using Microsoft.Xna.Framework;

namespace Utilities.Extensions
{
    public static class RectangleExtensions
    {
        public static Vector2 GetCenter(this Rectangle rectangle)
        {
            return new Vector2(rectangle.X + (rectangle.Width * 0.5f), rectangle.Y + (rectangle.Height * 0.5f));
        }

        public static Vector2 CenterInside(this Rectangle rectangle, Rectangle containerRectangle)
        {
            var rectangleCenter = rectangle.GetCenter();
            var containerCenter = containerRectangle.GetCenter();
            return containerCenter - rectangleCenter;
        }
    }
}
