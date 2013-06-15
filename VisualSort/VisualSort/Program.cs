using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VisualSort
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
       
        /* LISTAS DE TODOS OS DADOS UTILIZADOS (listas-mestre) */
        public static TLigaçãoList Ligações = new TLigaçãoList();
        public static List<TPessoa> Pessoas = new List<TPessoa>();
        public static List<TPeriódico> Peridódicos = new List<TPeriódico>();
        public static List<TConferência> Conferências = new List<TConferência>();
        public static List<TProdBibliográfica> Produções = new List<TProdBibliográfica>();
        public static List<TInstituição> Instituições = new List<TInstituição>();

        static void Main(string[] args)
        {
            using (MainForm game = new MainForm())
            {
                game.IsMouseVisible = true;
                game.Window.Title = "VisualSort";
                game.Run();
            }
        }
    }
#endif
}

