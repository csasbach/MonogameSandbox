using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameSandbox.Scenes.MainMenu;
using MonoGameSandbox.SharedComponents;
using Utilities.Abstractions;
using Utilities.Attributes;
using Utilities.DrawableGameComponents;

namespace MonoGameSandbox.Scenes.SomeOtherDemo
{
    public abstract class SceneExampleBase : Scene
    {
        private readonly IInputService _input;

        protected SceneExampleBase(Game game, SpriteBatch spriteBatch) : base(game, spriteBatch)
        {
            _input = Game.Services.GetService<IInputService>();
        }

        protected override void LoadContent()
        {
            IndependentSprites.Add(new Hud(Game, SpriteBatch));

            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            _input.OnReleased(() => GameState.SetGameState<MainMenuScene>(SpriteBatch), g => g.A, Keys.E);

            base.Update(gameTime);
        }
    }

    [Scene("Demo 01")]
    public class Demo01 : SceneExampleBase
    {
        public Demo01(Game game, SpriteBatch spriteBatch) : base(game, spriteBatch)
        {
        }
    }

    [Scene("Demo 02")]
    public class Demo02 : SceneExampleBase
    {
        public Demo02(Game game, SpriteBatch spriteBatch) : base(game, spriteBatch)
        {
        }
    }

    [Scene("Demo 03")]
    public class Demo03 : SceneExampleBase
    {
        public Demo03(Game game, SpriteBatch spriteBatch) : base(game, spriteBatch)
        {
        }
    }

    [Scene("Demo 04")]
    public class Demo04 : SceneExampleBase
    {
        public Demo04(Game game, SpriteBatch spriteBatch) : base(game, spriteBatch)
        {
        }
    }

    [Scene("Demo 05")]
    public class Demo05 : SceneExampleBase
    {
        public Demo05(Game game, SpriteBatch spriteBatch) : base(game, spriteBatch)
        {
        }
    }

    [Scene("Demo 06")]
    public class Demo06 : SceneExampleBase
    {
        public Demo06(Game game, SpriteBatch spriteBatch) : base(game, spriteBatch)
        {
        }
    }

    [Scene("Demo 07")]
    public class Demo07 : SceneExampleBase
    {
        public Demo07(Game game, SpriteBatch spriteBatch) : base(game, spriteBatch)
        {
        }
    }

    [Scene("Demo 08")]
    public class Demo08 : SceneExampleBase
    {
        public Demo08(Game game, SpriteBatch spriteBatch) : base(game, spriteBatch)
        {
        }
    }
}
