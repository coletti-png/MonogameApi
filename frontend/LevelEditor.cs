using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

public class LevelEditor : Game
{
    private GraphicsDeviceManager graphics;
    private SpriteBatch spriteBatch;
    private LevelEditorScreen editorScreen;

    public LevelEditor()
    {
        graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        base.Initialize();
    }

    protected override void LoadContent()
    {
        spriteBatch = new SpriteBatch(GraphicsDevice);
        editorScreen = new LevelEditorScreen(GraphicsDevice, Content);
        editorScreen.LoadContent();
    }

    protected override void Update(GameTime gameTime)
    {
        editorScreen.Update(gameTime);
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        editorScreen.Draw(spriteBatch);
        base.Draw(gameTime);
    }
}