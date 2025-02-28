using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonogameUserInterface.Manager;
using System.Collections.Generic;


namespace MonogameUserInterface.Screens
{
    public class MainMenu : GameScreen
    {
        private int selectedOption = 0;
        private List<string> menuOptions = new() 
        { 
            "Retrieve User",
            "Retrieve All Users",
            "Add Player", 
            "Edit/Delete Player",
            "Exit" 
        };
        private SpriteFont font;
        public MainMenu(GraphicsDevice graphicsDevice, ContentManager content) : base(graphicsDevice, content) { }
        public override void LoadContent()
        {
            font = Content.Load<SpriteFont>("Font");
        }

        public override void Update(GameTime gameTime)
        {
            KeyboardState keyState = Keyboard.GetState();

            if (keyState.IsKeyDown(Keys.Down))
            {
                selectedOption = (selectedOption + 1) % menuOptions.Count;
            }
            if (keyState.IsKeyDown(Keys.Up))
            {
                selectedOption = (selectedOption - 1 + menuOptions.Count) % menuOptions.Count;
            }
            if (keyState.IsKeyDown(Keys.Enter))
            {
                switch (selectedOption)
                {
                    case 0: ScreenManager.ChangeScreen(new RetrieveUserMenu(GraphicsDevice, Content)); break;
                    case 1: ScreenManager.ChangeScreen(new RetrieveAllUsersMenu(GraphicsDevice, Content)); break;
                    case 2: ScreenManager.ChangeScreen(new AddPlayerMenu(GraphicsDevice, Content)); break;
                    case 3: ScreenManager.ChangeScreen(new EditDeleteUserMenu(GraphicsDevice, Content)); break;
                    case 4: Game1.ExitGame(); break;
                }
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            
            spriteBatch.Begin();
            spriteBatch.DrawString(font, "Main Menu", new Vector2(100, 50), Color.White);

            for (int i = 0; i < menuOptions.Count; i++)
            {
                Color color = (i == selectedOption) ? Color.Yellow : Color.White;
                spriteBatch.DrawString(font, menuOptions[i], new Vector2(100, 100 + i * 50), color);
            }

            spriteBatch.End();
        }

    }
}
