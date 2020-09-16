using Microsoft.Xna.Framework;
using System;

namespace Utilities.Abstractions
{
    public abstract class DrawableComponentService : DrawableGameComponent
    {
        protected DrawableComponentService(Game game, Type serviceType) : base(game)
        {
            if (!serviceType.IsInstanceOfType(this)) throw new InvalidOperationException($"This service must be an instance of {serviceType.Name}.");
            Game.Services.AddService(serviceType, this);
            Game.Components.Add(this);
        }
    }
}
