using Microsoft.Xna.Framework;
using System;
using System.Diagnostics;

namespace Utilities.Services
{
    /// <summary>
    /// Use this service base for services that will neeed to
    /// hook into Update calls
    /// </summary>
    public abstract class ComponentService : GameComponent
    {
        protected LoggerService Logger { get; }

        protected ComponentService(Game game, Type serviceType) : base(game)
        {
            if (!serviceType.IsInstanceOfType(this)) throw new InvalidOperationException($"This service must be an instance of {serviceType.Name}.");
            Game.Services.AddService(serviceType, this);
            Game.Components.Add(this);

            Logger = Game.Services.GetService<LoggerService>();
            using (var scope = Logger?.BeginScope($"{nameof(ServiceBase)} {System.Reflection.MethodBase.GetCurrentMethod().Name}"))
                Logger?.LogTrace(scope, "{2274ACBA-8C17-4C46-9A9F-3272217C75EE}", $"Finished[{Stopwatch.GetTimestamp()}]", null);
        }
    }
}
