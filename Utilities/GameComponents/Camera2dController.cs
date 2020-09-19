using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using Utilities.Abstractions;
using Utilities.Extensions;
using Utilities.Models;
using Utilities.Services;

namespace Utilities.GameComponents
{
    public class Camera2dController : ComponentService, ICameraService
    {
        private readonly GraphicsDeviceManager _graphicsDeviceManager;
        private readonly ICameraOptions _cameraOptions;
        private Camera2d _camera;
        private readonly IInputService _input;

        public Vector2 Position => _camera.Position;
        public float Rotation => _camera.Rotation;
        public float Zoom => _camera.Zoom;
        public Matrix Transform => _camera.Transform;

        public Camera2dController(Game game, GraphicsDeviceManager graphicsDeviceManager, IInputService inputService) : base(game, typeof(ICameraService))
        {
            _graphicsDeviceManager = graphicsDeviceManager;
            _input = inputService;
            Enabled = false;
        }

        public Camera2dController(Game game, GraphicsDeviceManager graphicsDeviceManager, ICameraOptions cameraOptions, IInputService inputService) : base(game, typeof(ICameraService))
        {
            _graphicsDeviceManager = graphicsDeviceManager;
            _cameraOptions = cameraOptions;
            _input = inputService;
            Enabled = false;
        }

        public override void Initialize()
        {
            _camera = _cameraOptions is null ?
                new Camera2d(_graphicsDeviceManager.GraphicsDevice.Viewport) :
                new Camera2d(_graphicsDeviceManager.GraphicsDevice.Viewport, _cameraOptions);

            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            float deltaT = gameTime.DeltaT();

            HandleReset();
            HandleRotation(deltaT);
            HandleZoom(deltaT);
            HandleMovement(deltaT);

            base.Update(gameTime);
        }

        private void HandleReset()
        {
            _input.OnReleased(() => _camera.Reset(), g => g.RightStick, Keys.R);
        }

        private void HandleRotation(float delta)
        {
            var speedDelta = ScaleRotationDeltaByRotationSpeed(_camera.RotationSpeed, delta);
            var constrainedDelta = ConstrainRotationDeltaToIntegralDegrees(speedDelta);
            _input.OnHeld(() => _camera.Rotation += constrainedDelta, b => b.Buttons.X, Keys.D9);
            _input.OnHeld(() => _camera.Rotation -= constrainedDelta, b => b.Buttons.Y, Keys.D0);
        }

        private void HandleZoom(float delta)
        {
            var speedDelta = ScaleZoomDeltaByZoomSpeed(_camera.ZoomSpeed, delta);
            var zoomScaledDelta = ScaleZoomDeltaByZoomValue(_camera.Zoom, speedDelta);
            var constrainedDelta = ConstrainZoomDeltaToIntegralPercentage(_camera.ZoomSpeed, zoomScaledDelta);
            _input.OnHeld(() => _camera.Zoom -= constrainedDelta, b => b.Buttons.LeftShoulder, Keys.OemComma);
            _input.OnHeld(() => _camera.Zoom += constrainedDelta, b => b.Buttons.RightShoulder, Keys.OemPeriod);
            _input.OnScrollWheelChanged(diff => _camera.Zoom += constrainedDelta * diff);
        }

        private void HandleMovement(float delta)
        {
            var speedDelta = ScaleMovementDeltaByMovementSpeed(_camera.MovementSpeed, delta);
            var scaledDelta = ScaleDownMovementDeltaByZoomValue(speedDelta, _camera.Zoom);
            var moveDelta = ConstrainMovementDeltaToIntegralValue(scaledDelta);
            _input.OnHeld(() => _camera.Move(0, -moveDelta), b => b.DPad.Up, Keys.Up, Keys.W);
            _input.OnHeld(() => _camera.Move(0, moveDelta), b => b.DPad.Down, Keys.Down, Keys.S);
            _input.OnHeld(() => _camera.Move(-moveDelta, 0), b => b.DPad.Left, Keys.Left, Keys.A);
            _input.OnHeld(() => _camera.Move(moveDelta, 0), b => b.DPad.Right, Keys.Right, Keys.D);
        }

        private static float ScaleMovementDeltaByMovementSpeed(float movementSpeed, float delta)
        {
            return movementSpeed * delta;
        }

        private static float ScaleDownMovementDeltaByZoomValue(float delta, float zoom)
        {
            return delta / zoom;
        }

        private static float ConstrainMovementDeltaToIntegralValue(float scaledMoveDelta)
        {
            var integralDelta = Math.Round(scaledMoveDelta);
            return (float)(integralDelta < 1 ? 1 : integralDelta);
        }

        private static float ScaleZoomDeltaByZoomSpeed(float sensitivity, float delta)
        {
            return sensitivity * delta;
        }

        private static float ScaleZoomDeltaByZoomValue(float zoom, float delta)
        {
            return zoom * delta;
        }

        private static float ConstrainZoomDeltaToIntegralPercentage(float baseSensitivity, float rawDelta)
        {
            var deltaInIntegralPercentage = Math.Round(rawDelta, 2);
            return (float)(deltaInIntegralPercentage < baseSensitivity ? baseSensitivity : deltaInIntegralPercentage);
        }

        private static float ScaleRotationDeltaByRotationSpeed(float sensitivity, float delta)
        {
            return sensitivity * delta;
        }

        private static float ConstrainRotationDeltaToIntegralDegrees(float rawDelta)
        {
            var rawDeltaInDegrees = MathHelper.ToDegrees(rawDelta);
            var deltaInIntegralDegrees = Math.Round(rawDeltaInDegrees);
            var deltaAsRadians = MathHelper.ToRadians((float)deltaInIntegralDegrees);
            return deltaAsRadians;
        }

        protected override void OnEnabledChanged(object sender, EventArgs args)
        {
            _camera?.Reset();
            base.OnEnabledChanged(sender, args);
        }
    }
}
