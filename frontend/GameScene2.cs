using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Threading.Tasks;

public class GameScene2 : GameSceneBase
{
    public GameScene2(GraphicsDevice graphicsDevice, Texture2D tileTexture) 
        : base(graphicsDevice, tileTexture, "LEVEL2") { }

    public async Task Initialize()
    {
        await LoadLevel();
    }

    public override void Update(GameTime gameTime)
    {
         if (Keyboard.GetState().IsKeyDown(Keys.Escape))
        {
            System.Environment.Exit(0);
        }
    }

    public override void Draw()
    {
        base.Draw();
    }
}
