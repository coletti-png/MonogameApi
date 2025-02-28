using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.VisualBasic.ApplicationServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using MonogameUserInterface.Manager;
using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;
using System.Linq;
using System;

namespace MonogameUserInterface.Screens
{
    public class RetrieveUserMenu : GameScreen
    {
        private List<User> allUsers = new();
        private List<User> filteredUsers = new();
        private int selectedIndex = 0;
        private string searchQuery = "";
        private bool isSearching = true; 
        private string statusMessage = "Loading users...";
        private string updateMessage = "";
        private SpriteFont font;
        private HttpClient client = new();
        public RetrieveUserMenu(GraphicsDevice graphicsDevice, ContentManager content) : base(graphicsDevice, content) { }

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
                    allUsers = JsonConvert.DeserializeObject<List<User>>(jsonData).OrderBy(u => u.ScreenName).ToList();
                    
                    filteredUsers = new List<User>(allUsers);
                    statusMessage = allUsers.Count > 0 ? "Start typing to search..." : "No users found.";
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

        private void FilterUsers()
        {
            if (string.IsNullOrWhiteSpace(searchQuery))
            {
                filteredUsers = new List<User>(allUsers);
            }
            else
            {
                filteredUsers = allUsers.Where(u => u.ScreenName.StartsWith(searchQuery, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            selectedIndex = 0;
        }

        public override void Update(GameTime gameTime)
        {
            KeyboardState keyState = Keyboard.GetState();

            if (isSearching)
            {
                HandleSearchInput(keyState);

                if (keyState.IsKeyDown(Keys.Enter) && filteredUsers.Count > 0)
                {
                    isSearching = false;
                }
                
                if (keyState.IsKeyDown(Keys.Escape))
                {
                    if (!string.IsNullOrEmpty(searchQuery))
                    {
                        searchQuery = "";
                        FilterUsers();
                    }
                    else
                    {
                        ScreenManager.ChangeScreen(new MainMenu(GraphicsDevice, Content));
                    }
                }

                return;
            }

            if (keyState.IsKeyDown(Keys.Down) && filteredUsers.Count > 0)
            {
                selectedIndex = (selectedIndex + 1) % filteredUsers.Count;
            }
            if (keyState.IsKeyDown(Keys.Up) && filteredUsers.Count > 0)
            {
                selectedIndex = (selectedIndex - 1 + filteredUsers.Count) % filteredUsers.Count;
            }
            if (keyState.IsKeyDown(Keys.Escape))
            {
                ScreenManager.ChangeScreen(new MainMenu(GraphicsDevice, Content));
            }
            if (keyState.IsKeyDown(Keys.Tab))
            {
                isSearching = !isSearching;
            }
            if (keyState.IsKeyDown(Keys.Enter) && filteredUsers.Count > 0)
            {
                updateMessage = $"Screen Name: {filteredUsers[selectedIndex].ScreenName}, Score: {filteredUsers[selectedIndex].Score}";
            }
        }

        private void HandleSearchInput(KeyboardState keyState)
        {
            string inputText = GetTextInput(keyState);
            if (!string.IsNullOrEmpty(inputText))
            {
                searchQuery += inputText;
                FilterUsers();
            }

            if (keyState.IsKeyDown(Keys.Back) && searchQuery.Length > 0)
            {
                searchQuery = searchQuery[..^1];
                FilterUsers();
            }
        }

        private string GetTextInput(KeyboardState keyState)
        {
            foreach (Keys key in keyState.GetPressedKeys())
            {
                if (key >= Keys.A && key <= Keys.Z)
                {
                    return key.ToString();
                }
                if (key >= Keys.D0 && key <= Keys.D9)
                {
                    return key.ToString().Replace("D", "");
                }
                if (key == Keys.Space)
                {
                    return " ";
                }
            }
            return "";
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin();
            spriteBatch.DrawString(font, "Retrieve Users (ENTER - View, TAB - Toggle Search, ESC - Back)", new Vector2(50, 50), Color.White);
            spriteBatch.DrawString(font, statusMessage, new Vector2(50, 100), Color.Red);

            spriteBatch.DrawString(font, $"Search: {(isSearching ? "> " : "")}{searchQuery}", new Vector2(50, 140), isSearching ? Color.Yellow : Color.White);

            for (int i = 0; i < filteredUsers.Count; i++)
            {
                Color color = (i == selectedIndex) ? Color.Yellow : Color.White;
                spriteBatch.DrawString(font, $"{filteredUsers[i].ScreenName} - {filteredUsers[i].Score} PTS", new Vector2(50, 180 + i * 30), color);
            }

            spriteBatch.DrawString(font, "Press ESC to return, ENTER to view, TAB to switch", new Vector2(50, 500), Color.Yellow);
            spriteBatch.DrawString(font, updateMessage, new Vector2(50, 550), Color.Green);

            spriteBatch.End();
        }
    }


}
