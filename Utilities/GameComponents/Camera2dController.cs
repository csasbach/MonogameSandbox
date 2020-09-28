using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Diagnostics;
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

        public bool MoveEnabled { get; set; } = true;
        public Vector2 Position
        {
            get => _camera.Position;
            set => _camera.Position = MoveEnabled ? value : Position;
        }
        public bool RotationEnabled { get; set; } = true;
        public float Rotation
        {
            get => _camera.Rotation;
            set => _camera.Rotation = RotationEnabled ? value : Rotation;
        }
        public bool ZoomEnabled { get; set; } = true;
        public float MaxZoom
        {
            get => _camera.MaxZoom;
            set => _camera.MaxZoom = value;
        }
        public float MinZoom
        {
            get => _camera.MinZoom;
            set => _camera.MinZoom = value;
        }
        public float Zoom
        {
            get => _camera.Zoom;
            set => _camera.Zoom = ZoomEnabled ? value : Zoom;
        }
        public bool ResetEnabled { get; set; } = true;
        public Matrix Transform => _camera.Transform;

        public Camera2dController(Game game, GraphicsDeviceManager graphicsDeviceManager, IInputService inputService) : base(game, typeof(ICameraService))
        {
            using (var scope = Logger?.BeginScope($"{nameof(Camera2dController)} {System.Reflection.MethodBase.GetCurrentMethod().Name}"))
            {
                Logger?.LogTrace(scope, "{EF9CE49A-FAEE-4CF0-89B9-101A99BCBCD0}", $"Started [{Stopwatch.GetTimestamp()}]", null);

                _graphicsDeviceManager = graphicsDeviceManager;
                _input = inputService;
                Enabled = false;

                Logger?.LogTrace(scope, "{38754872-0D05-415E-B821-013F55E25BE9}", $"Finished [{Stopwatch.GetTimestamp()}]", null);
            }
        }

        public Camera2dController(Game game, GraphicsDeviceManager graphicsDeviceManager, ICameraOptions cameraOptions, IInputService inputService) : base(game, typeof(ICameraService))
        {
            using (var scope = Logger?.BeginScope($"{nameof(Camera2dController)} {System.Reflection.MethodBase.GetCurrentMethod().Name} (with {nameof(cameraOptions)})"))
            {
                Logger?.LogTrace(scope, "{37176C0D-E134-49BA-AB89-ECD9284DF072}", $"Started [{Stopwatch.GetTimestamp()}]", null);

                _graphicsDeviceManager = graphicsDeviceManager;
                _cameraOptions = cameraOptions;
                _input = inputService;
                Enabled = false;

                Logger?.LogTrace(scope, "{1BED4B9E-71D9-47E8-AE52-24C9BE6380B0}", $"Finished [{Stopwatch.GetTimestamp()}]", null);
            }
        }

        public void Reset()
        {
            if (ResetEnabled) _camera.Reset();
        }

        public void FullReset()
        {
            _camera.FullReset();
        }

        public void SetStartParameters(CameraStartParameters cameraStartParameters)
        {
            Position = cameraStartParameters.Position ?? Position;
            Rotation = cameraStartParameters.Rotation ?? Rotation;
            Zoom = cameraStartParameters.Zoom ?? Zoom;
        }

        public override void Initialize()
        {
            using (var scope = Logger?.BeginScope($"{nameof(Camera2dController)} {System.Reflection.MethodBase.GetCurrentMethod().Name}"))
            {
                Logger?.LogTrace(scope, "{F0005F3C-A6AB-4F2D-8316-C922B299624B}", $"Started [{Stopwatch.GetTimestamp()}]", null);

                _camera = _cameraOptions is null ?
                    new Camera2d(_graphicsDeviceManager.GraphicsDevice.Viewport) :
                    new Camera2d(_graphicsDeviceManager.GraphicsDevice.Viewport, _cameraOptions);

                Logger?.LogTrace(scope, "{83579F22-0071-4A58-B544-5C7AF36E61F1}", $"Finished Override [{Stopwatch.GetTimestamp()}]", null);

                base.Initialize();

                Logger?.LogTrace(scope, "{D8D820C9-681E-42D9-9B35-83FF1F960B44}", $"Finished Base [{Stopwatch.GetTimestamp()}]", null);
            }
        }

        public override void Update(GameTime gameTime)
        {
            float deltaT = gameTime.DeltaT();

            if (ResetEnabled) HandleReset();
            if (RotationEnabled) HandleRotation(deltaT);
            if (ZoomEnabled) HandleZoom(deltaT);
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
            _input.OnHeld(() => Rotation += constrainedDelta, b => b.Buttons.X, Keys.D9);
            _input.OnHeld(() => Rotation -= constrainedDelta, b => b.Buttons.Y, Keys.D0);
        }

        private void HandleZoom(float delta)
        {
            var speedDelta = ScaleZoomDeltaByZoomSpeed(_camera.ZoomSpeed, delta);
            var zoomScaledDelta = ScaleZoomDeltaByZoomValue(_camera.Zoom, speedDelta);
            var constrainedDelta = ConstrainZoomDeltaToIntegralPercentage(_camera.ZoomSpeed, zoomScaledDelta);
            _input.OnHeld(() => Zoom -= constrainedDelta, b => b.Buttons.LeftShoulder, Keys.OemComma);
            _input.OnHeld(() => Zoom += constrainedDelta, b => b.Buttons.RightShoulder, Keys.OemPeriod);
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
            MoveEnabled = true;
            RotationEnabled = true;
            ZoomEnabled = true;
            ResetEnabled = true;
            _camera?.FullReset();
            base.OnEnabledChanged(sender, args);
        }
    }
}
