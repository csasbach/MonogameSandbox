using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Utilities.Abstractions;

namespace Utilities.Services
{
    public class Camera2d : ICameraService
    {
        private Viewport _viewport;

        public bool InvertVerticalMovement { get; }
        public bool InvertHorizontalMovement { get; }
        public bool ReverseRotation { get; }
        public bool ReverseZoom { get; }
        public float MovementSpeed { get; } = 1.0f;
        public float ZoomSpeed { get; } = 0.01f;
        public float RotationSpeed { get; } = 0.001f;

        private float _zoom = 1.0f;
        private readonly float _maxZoom = 100.0f;
        private readonly float _minZoom = 0.01f;
        public float Zoom
        {
            get => _zoom;
            set
            {
                var zoom = ReverseZoom ? value * -1 : value;
                _zoom = zoom < _minZoom ? _minZoom : zoom > _maxZoom ? _maxZoom : zoom;
            }
        }
        private float _rotation;
        private readonly float _maxRotation = 6.28319f; // ~360°
        private readonly float _minRotation = -6.28319f;
        public float Rotation
        {
            get => _rotation;
            set
            {
                var rotation = ReverseRotation ? value * -1 : value;
                _rotation = rotation < _minRotation ? 0 : rotation > _maxRotation ? 0 : rotation;
            }
        }
        private float _x = 0.0f;
        private readonly float _minX = -10000.0f;
        private readonly float _maxX = 10000.0f;
        private float _y = 0.0f;
        private readonly float _minY = -10000.0f;
        private readonly float _maxY = 10000.0f;
        public Vector2 Position
        {
            get => new Vector2(_x, _y);
            set
            {
                var x = InvertHorizontalMovement ? value.X * -1 : value.X;
                var y = InvertVerticalMovement ? value.Y * -1 : value.Y;
                _x = x < _minX ? _minX : x > _maxX ? _maxX : x;
                _y = y < _minX ? _minY : y > _maxY ? _maxY : y;
            }
        }
        /// <summary>
        /// Used to apply this camera's transform to a SpriteBatch
        /// </summary>
        public Matrix Transform => GetTransformation();

        public Camera2d(Viewport viewport)
        {
            _viewport = viewport;
            CenterCamera();
        }

        public Camera2d(Viewport viewport, ICameraOptions options)
        {
            _viewport = viewport;
            _minX = options.MinX;
            _maxX = options.MaxX;
            _x = options.DefaultX;
            _minY = options.MinY;
            _maxY = options.MaxY;
            _y = options.DefaultY;
            MovementSpeed = options.MovementSpeed;
            _minZoom = options.MinZoom;
            _maxZoom = options.MaxZoom;
            _zoom = options.DefaultZoom;
            ZoomSpeed = options.ZoomSpeed;
            _minRotation = options.MinRotation;
            _maxRotation = options.MaxRotation;
            _rotation = options.DefaultRotation;
            RotationSpeed = options.RotationSpeed;
            InvertVerticalMovement = options.InvertVerticalMovement;
            InvertHorizontalMovement = options.InvertHorizontalMovement;
            if (options.CenterCameraOnInitialization) CenterCamera();
        }

        public void Move(float deltaX, float deltaY)
        {
            Position += new Vector2(deltaX, deltaY);
        }

        /// <summary>
        /// Returns the camera to its initial transform
        /// </summary>
        public void Reset()
        {
            CenterCamera();
            Rotation = 0.0f;
            Zoom = 1;
        }

        private void CenterCamera()
        {
            Position = Vector2.Zero;
            Position -= new Vector2(0 - _viewport.Bounds.Width * 0.5f, 0 - _viewport.Bounds.Height * 0.5f);
        }

        private Matrix GetTransformation()
        {
            return Matrix.CreateTranslation(new Vector3(-Position.X, -Position.Y, 0)) *
                Matrix.CreateRotationZ(Rotation) *
                Matrix.CreateScale(Zoom) *
                Matrix.CreateTranslation(new Vector3(_viewport.Bounds.Width * 0.5f, _viewport.Bounds.Height * 0.5f, 0));
        }
    }
}
