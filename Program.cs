using System;

namespace GameOfLife
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var game = new GOLEngine())
                game.Run();
        }
    }
}
