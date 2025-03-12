using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Content;
using System.Net;
using System.Linq;


public class LevelEditorScreen : GameScreen
{
    private const int TILE_SIZE = 16;
    private Texture2D tileSheet;
    private List<Rectangle> tileRects = new();
    private Dictionary<Point, int> placedTiles = new();
    private MouseState previousMouse;
    private KeyboardState previousKeyboard;
    private HttpClient client = new();
    private int levelWidth = 16; 
    private int levelHeight = 16; 

    
    private int selectedTileIndex = 0;
    private int tilesPerRow;
    private int tilesPerColumn;

    private string levelName = "";
    private bool isNamingLevel = true;
    private string message = "Enter Level Name: ";

    public LevelEditorScreen(GraphicsDevice graphicsDevice, ContentManager content) : base(graphicsDevice, content) { }

    public override void LoadContent()
    {
        tileSheet = Content.Load<Texture2D>("tileset"); 
        tilesPerRow = tileSheet.Width / TILE_SIZE;
        tilesPerColumn = tileSheet.Height / TILE_SIZE;

        for (int y = 0; y < tilesPerColumn; y++)
        {
            for (int x = 0; x < tilesPerRow; x++)
            {
                tileRects.Add(new Rectangle(x * TILE_SIZE, y * TILE_SIZE, TILE_SIZE, TILE_SIZE));
            }
        }
       


    }

    public override void Update(GameTime gameTime)
    {
        KeyboardState keyboard = Keyboard.GetState();
        MouseState mouse = Mouse.GetState();
        Point gridPos = new((int)(mouse.X / TILE_SIZE), (int)(mouse.Y / TILE_SIZE));

        if (isNamingLevel)
        {
            HandleTextInput(keyboard);
            if (keyboard.IsKeyDown(Keys.Enter) && !previousKeyboard.IsKeyDown(Keys.Enter))
            {
                isNamingLevel = false; 
                message = $"Level Name: {levelName} (Press S to Save)";
            }
        }
        else
        {
            if ((keyboard.IsKeyDown(Keys.Right) || keyboard.IsKeyDown(Keys.D)) && !previousKeyboard.IsKeyDown(Keys.Right) && !previousKeyboard.IsKeyDown(Keys.D))
            {
                selectedTileIndex = (selectedTileIndex + 1) % tileRects.Count;
            }

            if ((keyboard.IsKeyDown(Keys.Left) || keyboard.IsKeyDown(Keys.A)) && !previousKeyboard.IsKeyDown(Keys.Left) && !previousKeyboard.IsKeyDown(Keys.A))
            {
                selectedTileIndex = (selectedTileIndex - 1 + tileRects.Count) % tileRects.Count;
            }

            if (mouse.LeftButton == ButtonState.Pressed)
            {
                placedTiles[gridPos] = selectedTileIndex;
            }

            if (mouse.RightButton == ButtonState.Pressed )
            {
                placedTiles.Remove(gridPos);
            }

            if (keyboard.IsKeyDown(Keys.S) && !previousKeyboard.IsKeyDown(Keys.S))
            {
                Task.Run(SaveLevel);
            }

            if (keyboard.IsKeyDown(Keys.L) && !previousKeyboard.IsKeyDown(Keys.L))
            {
                Task.Run(LoadLevel);
            }
        }

        previousMouse = mouse;
        previousKeyboard = keyboard;
    }

    private void HandleTextInput(KeyboardState keyboard)
    {
        foreach (Keys key in keyboard.GetPressedKeys())
        {
            if (!previousKeyboard.IsKeyDown(key))
            {
                if (key == Keys.Back && levelName.Length > 0)
                {
                    levelName = levelName[..^1];
                }
                else if (key == Keys.Space)
                {
                    levelName += " ";
                }
                else if (key >= Keys.A && key <= Keys.Z)
                {
                    levelName += key.ToString();
                }
                else if (key >= Keys.D0 && key <= Keys.D9)
                {
                    levelName += key.ToString().Replace("D", "");
                }
            }
        }
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        GraphicsDevice.Clear(Color.Black);
        spriteBatch.Begin();

        foreach (var tile in placedTiles)
        {
            spriteBatch.Draw(tileSheet, new Rectangle(tile.Key.X * TILE_SIZE, tile.Key.Y * TILE_SIZE, TILE_SIZE, TILE_SIZE),
                             tileRects[tile.Value], Color.White);
        }

        spriteBatch.DrawString(Content.Load<SpriteFont>("Font"), message, new Vector2(20, 20), Color.Yellow);
        spriteBatch.DrawString(Content.Load<SpriteFont>("Font"), "Press S to Save, L to Load", new Vector2(20, 50), Color.White);
        spriteBatch.DrawString(Content.Load<SpriteFont>("Font"), "Use A/D or Left/Right to Change Tiles", new Vector2(20, 80), Color.White);
        spriteBatch.DrawString(Content.Load<SpriteFont>("Font"), $"Selected Tile: {selectedTileIndex}", new Vector2(20, 110), Color.Cyan);
        
        spriteBatch.End();
    }

    private async Task SaveLevel()
        {
            if (string.IsNullOrWhiteSpace(levelName))
            {
                message = "Level name is required!";
                return;
            }

            var levelData = new
            {
                levelName = levelName, 
                width = levelWidth,
                height = levelHeight,
                PlacedTiles = placedTiles.Select(tile => new {
                    X = tile.Key.X,
                    Y = tile.Key.Y,
                    TileIndex = tile.Value
                }).ToList()
            };

            var content = new StringContent(JsonSerializer.Serialize(levelData), Encoding.UTF8, "application/json");
            var response = await client.PostAsync("http://localhost:3000/levels", content);

            message = response.IsSuccessStatusCode ? "Level saved successfully!" : "Failed to save level.";
        }



    private async Task LoadLevel()
        {
            var response = await client.GetAsync("http://localhost:3000/levels");
            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();
                LevelData loadedLevel = JsonSerializer.Deserialize<LevelData>(json);

                placedTiles.Clear();
                foreach (var tile in loadedLevel.PlacedTiles)
                {
                    placedTiles[new Point(tile.X, tile.Y)] = tile.TileIndex;
                }

                levelName = loadedLevel.LevelName ?? "Unnamed Level";
                message = $"Loaded Level: {levelName}";
            }
            else
            {
                message = "No saved levels found.";
            }
        }
}

public class LevelData
{
    public string LevelName { get; set; }
    public List<TileData> PlacedTiles { get; set; } = new();
}

public class TileData
{
    public int X { get; set; }
    public int Y { get; set; }
    public int TileIndex { get; set; }
}


