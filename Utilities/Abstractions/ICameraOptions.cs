namespace Utilities.Abstractions
{
    /// <summary>
    /// Options for configuring a 2D camera
    /// </summary>
    public interface ICameraOptions
    {
        float MinX { get; set; }
        float MaxX { get; set; }
        float DefaultX { get; set; }
        float MinY { get; set; }
        float MaxY { get; set; }
        float DefaultY { get; set; }
        float MovementSpeed { get; set; }
        float MinZoom { get; set; }
        float MaxZoom { get; set; }
        float DefaultZoom { get; set; }
        float ZoomSpeed { get; }
        float MinRotation { get; set; }
        float MaxRotation { get; set; }
        float DefaultRotation { get; set; }
        float RotationSpeed { get; }
        bool InvertVerticalMovement { get; set; }
        bool InvertHorizontalMovement { get; set; }
        bool ReverseRotation { get; set; }
        bool ReverseZoom { get; set; }
        bool CenterCameraOnInitialization { get; set; }
    }
}
