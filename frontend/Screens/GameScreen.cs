using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;


namespace MonogameUserInterface.Screens
{
    public abstract class GameScreen
    {
        protected GraphicsDevice GraphicsDevice;
        protected ContentManager Content;

        public GameScreen(GraphicsDevice graphicsDevice, ContentManager content)
        {
            this.GraphicsDevice = graphicsDevice;
            this.Content = content;
        }
        
        public abstract void LoadContent();

        public abstract void Update(GameTime gameTime);

        public abstract void Draw(SpriteBatch spriteBatch);
    }
}
