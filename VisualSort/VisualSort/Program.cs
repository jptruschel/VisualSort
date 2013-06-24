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
        public static TBlocoPessoas fPessoas = new TBlocoPessoas();
        public static TBlocoPeriódicos fPeriódicos = new TBlocoPeriódicos();
        public static TBlocoConferências fConferências = new TBlocoConferências();
        public static TBlocoInstituições fInstituições = new TBlocoInstituições();
        public static TBlocoProduções fProduções = new TBlocoProduções();
        /* LISTAS DE TODOS OS DADOS UTILIZADOS (listas-mestre) */
        public static TBigList mPessoas = new TBigList(0);
        public static TBigList mPeridódicos = new TBigList(1);
        public static TBigList mConferências = new TBigList(2);
        public static TBigList mProduções = new TBigList(3);
        public static TBigList mInstituições = new TBigList(4);
        // Retorna o Nodo, dado um Ponteiro para Nodo
        public static TInfoNodo GetNodoFromLists(TPNodo Nodo)
        {
            switch (Nodo.Tipo)
            {
                case 0:
                    return Program.mPessoas[(int)Nodo.Índice];
                case 1:
                    return Program.mPeridódicos[(int)Nodo.Índice];
                case 2:
                    return Program.mConferências[(int)Nodo.Índice];
                case 3:
                    return Program.mProduções[(int)Nodo.Índice];
                case 4:
                    return Program.mInstituições[(int)Nodo.Índice];
                default:
                    return null;
            }
        }

        /// O modo de Visualização Selecionado
        ///   0= Nodo (usando somente MaxNodos[0]
        ///   1= Pessoas
        ///   2= Periódicos
        ///   3= Conferências
        ///   4= Produção (usando MaxNodos[Artigos.Count])
        ///   5= Instituições (usando MaxNodos[Ins.Count])
        /// Por default = 0;
        public static int ViewMode = 0;
        public static int maxNodoSelecionado = 0;

        /* OUTRAS VARIÁVEIS GLOBAIS - DESCULPE, MARA ABEL */
        public static TInfoNodo NodoSelecionado, NodoMouse;

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

