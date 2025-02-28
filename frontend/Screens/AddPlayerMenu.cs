using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using MonogameUserInterface.Manager;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Content;
using System;

namespace MonogameUserInterface.Screens
{
    public class AddPlayerMenu : GameScreen
    {
    private string screenName = "";
    private string firstName = "";
    private string lastName = "";
    private string score = "";

    private string[] fieldLabels = { "Screen Name", "First Name", "Last Name", "Score" };
    private int currentField = 0; 

    private string message = "Enter player details and press Enter.";
    private SpriteFont font;
    private HttpClient client = new();
    private KeyboardState previousKeyState;

    public AddPlayerMenu(GraphicsDevice graphicsDevice, ContentManager content) : base(graphicsDevice, content) { }

    public override void LoadContent()
    {
        font = Content.Load<SpriteFont>("Font");
    }

    public override void Update(GameTime gameTime)
    {
        KeyboardState keyState = Keyboard.GetState();
        HandleTextInput(keyState);

        if (keyState.IsKeyDown(Keys.Tab) && !previousKeyState.IsKeyDown(Keys.Tab))
        {
            currentField = (currentField + 1) % fieldLabels.Length;
        }

        if (keyState.IsKeyDown(Keys.Enter) && !previousKeyState.IsKeyDown(Keys.Enter))
        {
            Task.Run(() => AddPlayer());
        }

        if (keyState.IsKeyDown(Keys.Escape))
        {
            ScreenManager.ChangeScreen(new MainMenu(GraphicsDevice, Content));
        }

        previousKeyState = keyState;
    }

    private void HandleTextInput(KeyboardState keyState)
    {
        string inputText = GetTextInput(keyState);

        if (!string.IsNullOrEmpty(inputText))
        {
            switch (currentField)
            {
                case 0: screenName += inputText; break;
                case 1: firstName += inputText; break;
                case 2: lastName += inputText; break;
                case 3: score += inputText; break;
            }
        }

        if (keyState.IsKeyDown(Keys.Back) && !previousKeyState.IsKeyDown(Keys.Back))
        {
            switch (currentField)
            {
                case 0: if (screenName.Length > 0) screenName = screenName[..^1]; break;
                case 1: if (firstName.Length > 0) firstName = firstName[..^1]; break;
                case 2: if (lastName.Length > 0) lastName = lastName[..^1]; break;
                case 3: if (score.Length > 0) score = score[..^1]; break;
            }
        }
    }

    private string GetTextInput(KeyboardState keyState)
    {
        foreach (Keys key in keyState.GetPressedKeys())
        {
            if (!previousKeyState.IsKeyDown(key))
            {
                if (key >= Keys.A && key <= Keys.Z)
                {
                    return key.ToString(); // Append letters
                }
                else if (key >= Keys.D0 && key <= Keys.D9)
                {
                    return key.ToString().Replace("D", ""); // Append numbers
                }
                else if (key == Keys.Space)
                {
                    return " ";
                }
            }
        }
        return "";
    }

    private async Task AddPlayer()
{
    if (string.IsNullOrWhiteSpace(screenName) || string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName) || string.IsNullOrWhiteSpace(score))
    {
        message = "All fields are required!";
        return;
    }

    if (!int.TryParse(score, out int parsedScore))
    {
        message = "Score must be a number!";
        return;
    }

    var playerData = new
    {
        ScreenName = screenName,
        FirstName = firstName,
        LastName = lastName,
        Score = parsedScore,
        DateStartedPlaying = DateTime.UtcNow.ToString("yyyy-MM-dd")
    };

    string jsonData = JsonSerializer.Serialize(playerData);
    Console.WriteLine("Sending JSON: " + jsonData); 

    var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
    var response = await client.PostAsync("http://localhost:3000/player", content);

    string responseText = await response.Content.ReadAsStringAsync();
    Console.WriteLine("API Response: " + responseText); 

    message = response.IsSuccessStatusCode 
        ? "Player added successfully!" 
        : $"Failed to add player. API Error: {responseText}";
}

    public override void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Begin();
        spriteBatch.DrawString(font, "Enter Player Details:", new Vector2(50, 50), Color.White);

        spriteBatch.DrawString(font, "Screen Name: " + (currentField == 0 ? "> " : "") + screenName, new Vector2(50, 100), currentField == 0 ? Color.Yellow : Color.White);
        spriteBatch.DrawString(font, "First Name: " + (currentField == 1 ? "> " : "") + firstName, new Vector2(50, 140), currentField == 1 ? Color.Yellow : Color.White);
        spriteBatch.DrawString(font, "Last Name: " + (currentField == 2 ? "> " : "") + lastName, new Vector2(50, 180), currentField == 2 ? Color.Yellow : Color.White);
        spriteBatch.DrawString(font, "Score: " + (currentField == 3 ? "> " : "") + score, new Vector2(50, 220), currentField == 3 ? Color.Yellow : Color.White);

        spriteBatch.DrawString(font, message, new Vector2(50, 280), Color.Green);
        spriteBatch.DrawString(font, "Press TAB to switch fields, ENTER to submit, ESC to cancel", new Vector2(50, 320), Color.Gray);

        spriteBatch.End();
    }
}
}
