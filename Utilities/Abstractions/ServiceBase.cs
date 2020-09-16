using Microsoft.Xna.Framework;
using System;

namespace Utilities.Abstractions
{
    public abstract class ServiceBase
    {
        protected Game Game { get; }
        protected ServiceBase(Game game, Type serviceType)
        {
            Game = game ?? throw new ArgumentNullException(nameof(game));
            if (!serviceType.IsInstanceOfType(this)) throw new InvalidOperationException($"This service must be an instance of {serviceType.Name}.");
            Game.Services.AddService(serviceType, this);
        }
    }
}
