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
       
        /* BLOCOS DE DADOS SALVOS NO DISCO */
        public static TBlocoPessoas fPessoas = new TBlocoPessoas();
        public static TBlocoPeriódicos fPeriódicos = new TBlocoPeriódicos();
        public static TBlocoConferências fConferências = new TBlocoConferências();
        public static TBlocoInstituições fInstituições = new TBlocoInstituições();
        public static TBlocoProduções fProduções = new TBlocoProduções();
        /* LISTAS DE TODOS OS DADOS UTILIZADOS (listas-mestre) */
        public static TLigaçãoList mLigações = new TLigaçãoList();
        public static TPessoasList mPessoas = new TPessoasList();
        public static TPeriódicosList mPeridódicos = new TPeriódicosList();
        public static TConferênciaList mConferências = new TConferênciaList();
        public static TProduçõesList mProduções = new TProduçõesList();
        public static TInstituiçãoList mInstituições = new TInstituiçãoList();


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

