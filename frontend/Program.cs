public static class Program
    {
        static void Main()
        {
            //using (var game = new LevelEditor()) 
            using (var game = new Game1()) 
            {
                game.Run(); 
            }
        }
    }