namespace Utilities.Abstractions
{
    /// <summary>
    /// Provides all of the default 2D camera options so that they may be
    /// configured individually when this object is passed into a Camera2d
    /// constructor
    /// </summary>
    public class CameraOptions : ICameraOptions
    {
        public float MinX { get; set; } = -10000.0f;
        public float MaxX { get; set; } = 10000.0f;
        public float DefaultX { get; set; } = 0.0f;
        public float MinY { get; set; } = -10000.0f;
        public float MaxY { get; set; } = 10000.0f;
        public float DefaultY { get; set; } = 0.0f;
        public float MovementSpeed { get; set; } = 1.0f;
        public float MinZoom { get; set; } = 0.01f;
        public float MaxZoom { get; set; } = 100.0f;
        public float DefaultZoom { get; set; } = 1.0f;
        public float ZoomSpeed { get; set; } = 0.01f;
        public float MinRotation { get; set; } = -6.28319f;
        public float MaxRotation { get; set; } = 6.28319f;
        public float DefaultRotation { get; set; } = 0.0f;
        public float RotationSpeed { get; set; } = 0.001f;
        public bool InvertVerticalMovement { get; set; }
        public bool InvertHorizontalMovement { get; set; }
        public bool CenterCameraOnInitialization { get; set; } = true;
        public bool ReverseRotation { get; set; }
        public bool ReverseZoom { get; set; }
    }
}
