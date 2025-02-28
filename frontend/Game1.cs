using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonogameUserInterface.Manager;
using MonogameUserInterface.Screens;

namespace MonogameUserInterface
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            IsFixedTimeStep = true;
            graphics.SynchronizeWithVerticalRetrace = true; 
            TargetElapsedTime = TimeSpan.FromSeconds(1.0 / 60.0);
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            ScreenManager.ChangeScreen(new MainMenu(GraphicsDevice, Content));
        }

        protected override void Update(GameTime gameTime)
        {
            ScreenManager.Update(gameTime);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            ScreenManager.Draw(spriteBatch);
            base.Draw(gameTime);
        }

        public static void ExitGame()
        {
            ExitGame();
        }
    }
}
