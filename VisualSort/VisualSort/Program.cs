using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

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
        public static TPessoasList mPessoas = new TPessoasList();
        public static TPeriódicosList mPeridódicos = new TPeriódicosList();
        public static TConferênciaList mConferências = new TConferênciaList();
        public static TProduçõesList mProduções = new TProduçõesList();
        public static TInstituiçãoList mInstituições = new TInstituiçãoList();


        /* TEXTURAS USADAS GLOBALMENTE */
        public static Texture2D NodoTex;
        public static Texture2D[] LoadingTexture;


        static void Main(string[] args)
        {
            using (Renderer game = new Renderer())
            {
                game.IsMouseVisible = true;
                game.Window.Title = "VisualSort";
                game.Run();
            }
        }
    }
#endif
}

