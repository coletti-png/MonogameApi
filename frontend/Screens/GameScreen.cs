using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

public abstract class GameScreen
{
    protected GraphicsDevice GraphicsDevice;
    protected ContentManager Content;
    protected SpriteFont Font;

    public GameScreen(GraphicsDevice graphicsDevice, ContentManager content)
    {
        GraphicsDevice = graphicsDevice;
        Content = content;
    }

    public virtual void LoadContent()
    {
        Font = Content.Load<SpriteFont>("Font"); 
    }

    public abstract void Update(GameTime gameTime);
    public abstract void Draw(SpriteBatch spriteBatch);
}
