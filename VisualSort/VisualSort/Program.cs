using System;

namespace VisualSort
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (MainForm game = new MainForm())
            {
                game.IsMouseVisible = true;
                game.Window.Title = "Visual Sort";
                game.Run();
            }
        }
    }
#endif
}

