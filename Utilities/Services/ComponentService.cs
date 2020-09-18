using Microsoft.Xna.Framework;
using System;

namespace Utilities.Services
{
    public abstract class ComponentService : GameComponent
    {
        protected ComponentService(Game game, Type serviceType) : base(game)
        {
            if (!serviceType.IsInstanceOfType(this)) throw new InvalidOperationException($"This service must be an instance of {serviceType.Name}.");
            Game.Services.AddService(serviceType, this);
            Game.Components.Add(this);
        }
    }
}
