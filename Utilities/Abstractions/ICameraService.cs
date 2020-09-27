using Microsoft.Xna.Framework;

namespace Utilities.Abstractions
{
    /// <summary>
    /// Provides a render transform with controllable Position, Rotation, and Zoom
    /// </summary>
    public interface ICameraService : ITransformer
    {
        bool Enabled { get; set; }
        bool MoveEnabled { get; set; }
        Vector2 Position { get; set; }
        bool RotationEnabled { get; set; }
        float Rotation { get; set; }
        bool ZoomEnabled { get; set; }
        float Zoom { get; set; }
        bool ResetEnabled { get; set; }
        void Reset();
    }
}