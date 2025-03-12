using Microsoft.VisualBasic.Devices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Input;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private Texture2D _tileTexture;
    
    private GameSceneBase[] scenes;
    private int currentSceneIndex = 0;
    private float timer = 0;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        _tileTexture = Content.Load<Texture2D>("tileset");

        scenes = new GameSceneBase[]
        {
            new GameScene1(GraphicsDevice, _tileTexture),
            new GameScene2(GraphicsDevice, _tileTexture),
            new GameScene3(GraphicsDevice, _tileTexture)
        };

        // Load levels asynchronously
        Task.Run(async () =>
        {
            foreach (var scene in scenes)
            {
                await ((dynamic)scene).Initialize();
            }
        });
    }

    protected override void Update(GameTime gameTime)
    {
        timer += (float)gameTime.ElapsedGameTime.TotalSeconds;

        if (timer >= 5f) // Switch level every 5 seconds
        {
            currentSceneIndex = (currentSceneIndex + 1) % scenes.Length;
            timer = 0;
        }

        scenes[currentSceneIndex]?.Update(gameTime);
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);
        scenes[currentSceneIndex]?.Draw();
        base.Draw(gameTime);
    }
}
