using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonogameUserInterface.Screens;
using System.Collections.Generic;

namespace MonogameUserInterface.Manager
{
    public static class ScreenManager
    {
        private static GameScreen currentScreen;

        public static void ChangeScreen(GameScreen newScreen)
        {
            currentScreen = newScreen;
            currentScreen.LoadContent();
        }

        public static void Update(GameTime gameTime)
        {
            currentScreen?.Update(gameTime);
        }

        public static void Draw(SpriteBatch spriteBatch)
        {
            if (currentScreen != null)
            {
                currentScreen.Draw(spriteBatch);  
            }
        }
    }
}
