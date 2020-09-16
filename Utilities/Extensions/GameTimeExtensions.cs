using Microsoft.Xna.Framework;

namespace Utilities.Extensions
{
    public static class GameTimeExtensions
    {
        /// <summary>
        /// The time in milliseconds since update was last called.
        /// </summary>
        /// <param name="gameTime"></param>
        /// <returns></returns>
        public static float DeltaT(this GameTime gameTime)
        {
            return (float)gameTime.ElapsedGameTime.TotalMilliseconds;
        }
    }

}
