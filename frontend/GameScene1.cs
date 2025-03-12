using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Threading.Tasks;

public class GameScene1 : GameSceneBase
{
    public GameScene1(GraphicsDevice graphicsDevice, Texture2D tileTexture) 
        : base(graphicsDevice, tileTexture, "TYLER") { }

    public async Task Initialize()
    {
        await LoadLevel();
    }

    public override void Update(GameTime gameTime)
    {
        // Add specific logic if needed
    }

    public override void Draw()
    {
        base.Draw();
    }
}
