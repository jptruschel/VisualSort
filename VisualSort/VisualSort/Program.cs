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
        /// Algumas definições globais e o início do programa
        /// </summary>

        /* BLOCOS DE DADOS SALVOS NO DISCO */
        public static TBlocoPessoasHandler fPessoas = new TBlocoPessoasHandler();
        public static TBlocoArtigosHandler fArtigos = new TBlocoArtigosHandler();
        public static TBlocoLivrosHandler fLivros = new TBlocoLivrosHandler();
        public static TBlocoPeriódicosHandler fPeridódicos = new TBlocoPeriódicosHandler();
        public static TBlocoCapítulosHandler fCapítulos = new TBlocoCapítulosHandler();
        public static TBlocoConferênciasHandler fConferências = new TBlocoConferênciasHandler();
        public static TBlocoInstituiçõesHandler fInstituições = new TBlocoInstituiçõesHandler();
        /* LISTAS DE TODOS OS DADOS UTILIZADOS (listas-mestre) */
        public static TBigList mPessoas = new TBigList(0);
        public static TBigList mArtigos = new TBigList(1);
        public static TBigList mLivros = new TBigList(2);
        public static TBigList mPeridódicos = new TBigList(3);
        public static TBigList mCapítulos = new TBigList(4);
        public static TBigList mConferências = new TBigList(5);
        public static TBigList mInstituições = new TBigList(6);
        // Retorna o Nodo, dado um Ponteiro para Nodo
        // Tipo (0= Pessoa; 1= Artigo; 2= Livro; 3= Periódico; 4= Capítulo; 5= Conferência; 6= Instituição)
        public static TInfoNodo GetNodoFromLists(TPNodo Nodo)
        {
            switch (Nodo.Tipo)
            {
                case 0:
                    return Program.mPessoas[(int)Nodo.Índice];
                case 1:
                    return Program.mArtigos[(int)Nodo.Índice];
                case 2:
                    return Program.mLivros[(int)Nodo.Índice];
                case 3:
                    return Program.mPeridódicos[(int)Nodo.Índice];
                case 4:
                    return Program.mCapítulos[(int)Nodo.Índice];
                case 5:
                    return Program.mConferências[(int)Nodo.Índice];
                case 6:
                    return Program.mInstituições[(int)Nodo.Índice];
                default:
                    return null;
            }
        }

        /// O modo de Visualização Selecionado
        ///   0= Nodo (MaxNodoSelecionado: desenhado; Voltar/Avançar)
        ///   1= MaxNodos (MaxNodos criados pela pesquisa; Cada MaxNodo representa
        ///                 um elemento da pesquisa, onde, com mais zoom, é possível
        ///                 se ver as ligações internas (drawnodos com infonodos repetidos)
        /// Por default = 0;
        public static int ViewMode = 0;
        public static int maxNodoSelecionado = 0;

        /* OUTRAS VARIÁVEIS GLOBAIS - DESCULPE, MARA ABEL */
        public static TInfoNodo NodoSelecionado;
        public static TDrawNodo NodoMouse;
        public static int MaxNodoSelecionado;
        public static TDrawMaxNodo MaxNodoMouse;

        static void Main(string[] args)
        {
            // Inicializa
            fPessoas.InicializaGravação();
            fArtigos.InicializaGravação();
            fLivros.InicializaGravação();
            fPeridódicos.InicializaGravação();
            fCapítulos.InicializaGravação();
            fConferências.InicializaGravação();
            fInstituições.InicializaGravação();
            CSVBIZU.ConferenciasCSV();
            CSVBIZU.PeriodicosCSV();
            XMLBIZU.ReadFromXML("C:\\Users\\João Paulo T Ruschel\\Documents\\Visual Studio 2010\\Projects\\VisualSort\\VisualSort\\Tabelas\\krug.xml");
            fPessoas.FinalizaGravação();
            fArtigos.FinalizaGravação();
            fLivros.FinalizaGravação();
            fPeridódicos.FinalizaGravação();
            fCapítulos.FinalizaGravação();
            fConferências.FinalizaGravação();
            fInstituições.FinalizaGravação();

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

