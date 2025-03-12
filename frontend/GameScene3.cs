using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Threading.Tasks;

public class GameScene3 : GameSceneBase
{
    public GameScene3(GraphicsDevice graphicsDevice, Texture2D tileTexture) 
        : base(graphicsDevice, tileTexture, "LEVEL3") { }

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
