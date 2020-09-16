using Microsoft.Xna.Framework;

namespace Utilities.Abstractions
{
    /// <summary>
    /// Provides a render transform with controllable Position, Rotation, and Zoom
    /// </summary>
    public interface ICameraService : ITransformer
    {
        Vector2 Position { get; }
        float Rotation { get; }
        float Zoom { get; }
    }
}