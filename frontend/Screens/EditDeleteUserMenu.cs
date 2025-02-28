using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.VisualBasic.ApplicationServices;
using MonogameUserInterface.Manager;
using Microsoft.Xna.Framework.Content;

namespace MonogameUserInterface.Screens
{
    public class EditDeleteUserMenu : GameScreen
    {
        private List<User> users = new();
        private int selectedIndex = 0;
        private string statusMessage = "Loading users...";
        private string updateMessage = "";
        private SpriteFont font;
        private HttpClient client = new();
        
        private string[] fieldLabels = { "Screen Name", "Score" };
        private int currentField = 0; 
        private bool isEditing = false;
        private string screenNameInput = "";
        private string scoreInput = "";

        public EditDeleteUserMenu(GraphicsDevice graphicsDevice, ContentManager content) : base(graphicsDevice, content) { }
        
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
                    users = JsonConvert.DeserializeObject<List<User>>(jsonData)
                        .OrderBy(u => u.ScreenName).ToList();
                    statusMessage = users.Count > 0 ? "Use UP/DOWN to select, ENTER to edit" : "No users found.";
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

        private async Task SaveChanges()
        {
            if (users.Count == 0 || string.IsNullOrWhiteSpace(screenNameInput) || string.IsNullOrWhiteSpace(scoreInput))
            {
                updateMessage = "All fields are required!";
                return;
            }

            if (!int.TryParse(scoreInput, out int parsedScore))
            {
                updateMessage = "Score must be a number!";
                return;
            }

            var existingUser = users.FirstOrDefault(u => u.ScreenName == screenNameInput && u.playerid != users[selectedIndex].playerid);
            if (existingUser != null)
            {
                updateMessage = "Name already taken!";
                return;
            }

            var user = users[selectedIndex];
            user.ScreenName = screenNameInput;
            user.Score = parsedScore;

            string json = JsonConvert.SerializeObject(user);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PutAsync($"http://localhost:3000/player/{user.playerid}", content);
            updateMessage = response.IsSuccessStatusCode ? "User updated successfully!" : "Failed to update user.";
            await Task.Run(() => FetchAllUsers());

            isEditing = false;
        }

        private async Task DeleteUser()
        {
            var response = await client.DeleteAsync($"http://localhost:3000/player/{users[selectedIndex].playerid}");
            if (response.IsSuccessStatusCode)
            {
                updateMessage = "User deleted successfully.";
                await Task.Run(() => FetchAllUsers());
            }
            else
            {
                updateMessage = "Failed to delete user.";
            }
        }

        public override void Update(GameTime gameTime)
        {
            KeyboardState keyState = Keyboard.GetState();

            if (isEditing)
            {
                HandleTextInput(keyState);
                if (keyState.IsKeyDown(Keys.Enter))
                {
                    Task.Run(() => SaveChanges());
                }
                if (keyState.IsKeyDown(Keys.Escape))
                {
                    isEditing = false;
                    updateMessage = "";
                }
                return; 
            }

            if (keyState.IsKeyDown(Keys.Down))
            {
                selectedIndex = (selectedIndex + 1) % users.Count;
            }
            if (keyState.IsKeyDown(Keys.Up))
            {
                selectedIndex = (selectedIndex - 1 + users.Count) % users.Count;
            }
            if (keyState.IsKeyDown(Keys.Enter) && users.Count > 0)
            {
                isEditing = true;
                screenNameInput = users[selectedIndex].ScreenName;
                scoreInput = users[selectedIndex].Score.ToString();
            }
            if (keyState.IsKeyDown(Keys.D) && users.Count > 0)
            {
                Task.Run(() => DeleteUser());
            }
            if (keyState.IsKeyDown(Keys.Tab))
            {
                currentField = (currentField + 1) % fieldLabels.Length;
            }
            if (keyState.IsKeyDown(Keys.Escape))
            {
                ScreenManager.ChangeScreen(new MainMenu(GraphicsDevice, Content));
            }
        }

        private void HandleTextInput(KeyboardState keyState)
        {
            string inputText = GetTextInput(keyState);
            if (!string.IsNullOrEmpty(inputText))
            {
                if (currentField == 0)
                    screenNameInput += inputText;
                else if (currentField == 1)
                    scoreInput += inputText;
            }

            if (keyState.IsKeyDown(Keys.Back))
            {
                if (currentField == 0 && screenNameInput.Length > 0)
                    screenNameInput = screenNameInput[..^1];
                else if (currentField == 1 && scoreInput.Length > 0)
                    scoreInput = scoreInput[..^1];
            }
        }

        private string GetTextInput(KeyboardState keyState)
        {
            foreach (Keys key in keyState.GetPressedKeys())
            {
                if (key >= Keys.A && key <= Keys.Z)
                    return key.ToString();
                if (key >= Keys.D0 && key <= Keys.D9)
                    return key.ToString().Replace("D", "");
                if (key == Keys.Space)
                    return " ";
            }
            return "";
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin();
            spriteBatch.DrawString(font, "Edit/Delete Users (ENTER - Edit, D - Delete)", new Vector2(50, 50), Color.White);
            spriteBatch.DrawString(font, "TAB - Switch Fields", new Vector2(50, 80), Color.White);

            spriteBatch.DrawString(font, statusMessage, new Vector2(50, 100), Color.Red);

            for (int i = 0; i < users.Count; i++)
            {
                Color color = (i == selectedIndex) ? Color.Yellow : Color.White;
                spriteBatch.DrawString(font, $"{users[i].ScreenName} - {users[i].Score}", new Vector2(50, 150 + i * 30), color);
            }

            spriteBatch.DrawString(font, "Press ESC to return to the main menu", new Vector2(50, 500), Color.Yellow);
            spriteBatch.DrawString(font, updateMessage, new Vector2(50, 550), Color.Green);

            if (isEditing)
            {
                spriteBatch.DrawString(font, $"Editing: {users[selectedIndex].ScreenName}", new Vector2(50, 400), Color.Cyan);
                spriteBatch.DrawString(font, $"Screen Name: {(currentField == 0 ? "> " : "")}{screenNameInput}", new Vector2(50, 430), currentField == 0 ? Color.Yellow : Color.White);
                spriteBatch.DrawString(font, $"Score: {(currentField == 1 ? "> " : "")}{scoreInput}", new Vector2(50, 460), currentField == 1 ? Color.Yellow : Color.White);
                spriteBatch.DrawString(font, "Press ENTER to save, ESC to cancel, TAB to switch fields", new Vector2(50, 490), Color.Gray);
            }

            spriteBatch.End();
        }
}

    
}
