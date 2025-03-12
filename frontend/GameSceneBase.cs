using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

public abstract class GameSceneBase
{
    protected GraphicsDevice _graphicsDevice;
    protected SpriteBatch _spriteBatch;
    protected Texture2D _tileTexture;
    protected Dictionary<Point, int> _loadedTiles = new();
    protected string _levelName;
    private HttpClient _client = new();
    private string _apiUrl = "http://localhost:3000/levels/";

    public GameSceneBase(GraphicsDevice graphicsDevice, Texture2D tileTexture, string levelName)
    {
        _graphicsDevice = graphicsDevice;
        _tileTexture = tileTexture;
        _levelName = levelName;
        _spriteBatch = new SpriteBatch(_graphicsDevice);
    }

    public async Task LoadLevel()
    {
        try
        {
            var response = await _client.GetAsync(_apiUrl + _levelName);
            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();
                LevelData level = JsonSerializer.Deserialize<LevelData>(json);

                _loadedTiles.Clear();
                foreach (var tile in level.PlacedTiles)
                {
                    _loadedTiles[new Point(tile.X, tile.Y)] = tile.TileIndex;
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error loading level {_levelName}: {e.Message}");
        }
    }

    public virtual void Update(GameTime gameTime) 
    { 
        if (Keyboard.GetState().IsKeyDown(Keys.Escape))
        {
            System.Environment.Exit(0);
        }
    }

    public virtual void Draw()
    {
        _graphicsDevice.Clear(Color.Black);
        _spriteBatch.Begin();

        foreach (var tile in _loadedTiles)
        {
            Rectangle sourceRect = new Rectangle(tile.Value * 16, 0, 16, 16);
            _spriteBatch.Draw(_tileTexture, new Vector2(tile.Key.X * 16, tile.Key.Y * 16), sourceRect, Color.White);
        }

        _spriteBatch.End();
    }
}
