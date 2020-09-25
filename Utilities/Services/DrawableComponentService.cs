using Microsoft.Xna.Framework;
using System;
using System.Diagnostics;

namespace Utilities.Services
{
    /// <summary>
    /// Use this service base for services that will neeed to
    /// hook into Update and Draw calls
    /// </summary>
    public abstract class DrawableComponentService : DrawableGameComponent
    {
        private readonly Type _serviceType;

        protected LoggerService Logger { get; }

        protected DrawableComponentService(Game game, Type serviceType) : base(game)
        {
            if (!serviceType.IsInstanceOfType(this)) throw new InvalidOperationException($"This service must be an instance of {serviceType.Name}.");
            _serviceType = serviceType;

            // idempotent adds
            if (Game.Services.GetService(_serviceType) is null) Game.Services.AddService(_serviceType, this);
            if (!Game.Components.Contains(this)) Game.Components.Add(this);

            Logger = Game.Services.GetService<LoggerService>();
            using (var scope = Logger?.BeginScope($"{nameof(ServiceBase)} {System.Reflection.MethodBase.GetCurrentMethod().Name}"))
                Logger?.LogTrace(scope, "{0F2E4597-0CD6-4F6A-919D-5F9B9FB43EC6}", $"Finished[{Stopwatch.GetTimestamp()}]", null);
        }

        protected override void Dispose(bool disposing)
        {
            // idempotent removes
            if (!(Game.Services.GetService(_serviceType) is null)) Game.Services.RemoveService(_serviceType);
            if (Game.Components.Contains(this)) Game.Components.Remove(this);

            base.Dispose(disposing);
        }
    }
}
