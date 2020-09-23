using Microsoft.Xna.Framework;
using System;
using System.Diagnostics;

namespace Utilities.Services
{
    /// <summary>
    /// Use this base for services that do not need to hook
    /// into Update and Draw calls
    /// </summary>
    public abstract class ServiceBase
    {
        protected LoggerService Logger { get; }
        protected Game Game { get; }

        protected ServiceBase(Game game, Type serviceType)
        {
            Game = game ?? throw new ArgumentNullException(nameof(game));
            if (!serviceType.IsInstanceOfType(this)) throw new InvalidOperationException($"This service must be an instance of {serviceType.Name}.");
            Game.Services.AddService(serviceType, this);

            Logger = Game.Services.GetService<LoggerService>();
            using (var scope = Logger?.BeginScope($"{nameof(ServiceBase)} {System.Reflection.MethodBase.GetCurrentMethod().Name}"))
                Logger?.LogTrace(scope, "{1A89352F-B366-4107-936B-B232E4709752}", $"Finished[{Stopwatch.GetTimestamp()}]", null);
        }
    }
}
