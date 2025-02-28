using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.VisualBasic.ApplicationServices;
using MonogameUserInterface.Manager;
using Microsoft.Xna.Framework.Content;
using System;

namespace MonogameUserInterface.Screens
{
    public class RetrieveAllUsersMenu : GameScreen
    {
        private List<User> users = new();
        private string statusMessage = "Loading...";
        private SpriteFont font;
        private HttpClient client = new();
        private int selectedIndex = 0;
        private const int maxVisibleUsers = 10; 
        private int scrollOffset = 0;

        public RetrieveAllUsersMenu(GraphicsDevice graphicsDevice, ContentManager content) : base(graphicsDevice, content) { }

        public override void LoadContent()
        {
            font = Content.Load<SpriteFont>("Font");
            Task.Run(() => FetchAllUsers());
        }

        private async Task FetchAllUsers()
        {
            try
            {
                var response = await client.GetAsync("http://localhost:3000/player");
                if (response.IsSuccessStatusCode)
                {
                    string jsonData = await response.Content.ReadAsStringAsync();
                    users = JsonConvert.DeserializeObject<List<User>>(jsonData).OrderBy(u => u.ScreenName).ToList();

                    statusMessage = users.Count > 0 ? "" : "No users found.";
                    selectedIndex = 0; 
                    scrollOffset = 0;
                }
                else
                {
                    statusMessage = "Failed to fetch users.";
                }
            }
            catch
            {
                statusMessage = "Error retrieving users.";
            }
        }

        public override void Update(GameTime gameTime)
        {
            KeyboardState keyState = Keyboard.GetState();

            if (keyState.IsKeyDown(Keys.Down) && users.Count > 0)
            {
                selectedIndex = Math.Min(selectedIndex + 1, users.Count - 1);

                if (selectedIndex >= scrollOffset + maxVisibleUsers)
                {
                    scrollOffset++;
                }
            }

            if (keyState.IsKeyDown(Keys.Up) && users.Count > 0)
            {
                selectedIndex = Math.Max(selectedIndex - 1, 0);

                if (selectedIndex < scrollOffset)
                {
                    scrollOffset--;
                }
            }

            if (keyState.IsKeyDown(Keys.Escape))
            {
                ScreenManager.ChangeScreen(new MainMenu(GraphicsDevice, Content));
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin();

            spriteBatch.DrawString(font, "All Users (Use UP/DOWN to scroll, ESC to return):", new Vector2(50, 50), Color.White);

            if (!string.IsNullOrEmpty(statusMessage))
            {
                spriteBatch.DrawString(font, statusMessage, new Vector2(50, 100), Color.Red);
            }
            else
            {
                for (int i = 0; i < Math.Min(users.Count - scrollOffset, maxVisibleUsers); i++)
                {
                    int userIndex = scrollOffset + i;
                    Color color = (userIndex == selectedIndex) ? Color.Yellow : Color.Cyan;
                    spriteBatch.DrawString(font, $"{users[userIndex].ScreenName} - {users[userIndex].Score}", new Vector2(50, 150 + i * 30), color);
                }
            }

            spriteBatch.End();
        }
    }

}
