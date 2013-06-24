using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VisualSort
{
    // Um vetor de 2 dimensões de inteiros
    public struct BPos
    {
        public Int64 Bloco, Offset;
        public BPos(Int64 Bloco, Int64 Offset) 
        {
            this.Bloco = Bloco;
            this.Offset = Offset;
        }
        public BPos(int Bloco, int Offset)
        {
            this.Bloco = Bloco;
            this.Offset = Offset;
        }
        public static BPos operator +(BPos p1, BPos p2)
        {
            return new BPos(p1.Bloco + p2.Bloco, p1.Offset + p2.Offset);
        }
        public static BPos operator +(BPos p1, int offset)
        {
            return new BPos(p1.Bloco, p1.Offset + offset);
        }
        public static BPos operator -(BPos p1, BPos p2)
        {
            return new BPos(p1.Bloco - p2.Bloco, p1.Offset - p2.Offset);
        }
        public static BPos operator -(BPos p1, int offset)
        {
            return new BPos(p1.Bloco, p1.Offset - offset);
        }
        public static bool operator ==(BPos E1, BPos E2)
        {
            if (E1 == null)
                if (E2 == null)
                    return true;
                else
                    return false;
            if (E2 == null)
                    return false;
            return ((E1.Bloco == E2.Bloco) && (E1.Offset == E2.Offset));
        }
        public static bool operator !=(BPos E1, BPos E2)
        {
            if (E1 == null)
                if (E2 == null)
                    return false;
                else
                    return true;
            if (E2 == null)
                return true;
            return !((E1.Bloco == E2.Bloco) && (E1.Offset == E2.Offset));
        }
        public override bool Equals(System.Object obj)
        {
            return false;
        }
        public override int GetHashCode()
        {
            return (int)(Bloco ^ Offset);
        }
        public override string ToString()
        {
            return "Bloco: " + Bloco + "; Offset: " + Offset;
        }
    }
    // Uma estrutura que contém uma lista de inteiros - usada como saída das funções de busca
    //   CUIDADO: não usar = para trocar o valor de uma ListInt, use Copy!
    public class ListInt : List<Int64>
    {
        public ListInt() : base()
        {
        }
        public int AddValue(Int64 Value)
        {
            int i = IndexOf(Value);
            if (i == -1)
            {
                base.Add(Value);
                return Count - 1;
            }
            else
                return i;
        }
        public override string ToString()
        {
            string temp = new String(' ', 0);
            for (int i = 0; i < Count - 1; i++)
                temp += this[i] + ", ";
            return temp + this[Count - 1];
        }
    }

    /// DEFINIÇÃO DAS CLASSES QUE TERÃO TODOS OS DADOS (GUARDADO EM DISCO)
    ///   * ESSA SERÁ A BASE DE DADOS DE TUDO QUE O PROGRAMA TERÁ  
    ///   * ESSES DADOS SERÃO DEIXADOS NO DISCO E CARREGADOS CONFORME NECESSÁRIO 
    // Pessoas
    public struct TFPessoa
    {
        public string NomeCompleto;     // Nome Completo (na Ordem Correta)
        public string País;             // País de nascimento
        public string TextoResumo;      // Resumo da pessoa
    }
    // Um Periódico
    public struct TFPeriódico
    {
        public long ISSN1, ISSN2;       // Códigos de ISSN
        public string Nome;             // Nome do periódico
        public string Qualis;           // Qualis
    }
    // Uma conferência
    public struct TFConferência
    {
        public string Sigla, Nome;      // Nome (e Sigla)
        public string Caráter;          // Caráter da conferência - INTERNACIONAL ou NACIONAL
        public string Qualis;           // Qualis
    }
    // Uma Produção Bibliográfica
    public struct TFProdBibliográfica
    {
        public string Título;           // Título
        public string ISSN;             // ISSN relativo
        public int Periodico;           // Índice na Lista de Periódicos
        public string MeioDivulgação;   // Meio de Divulgação da Produção
        public string Idioma;           // Idioma da Produção
        public int AnoPublicação;       // Ano da Publicação
        public string Natureza;         // Natureza (COMPLETO, ESTENDIDO, RESUMO)
    }
    // Uma Instituição
    public struct TFInstituição
    {
        public string Nome;             // Nome da instituição
    }

    // Blocos - incompletos
    public class TBlocoPessoas
    {
        public BPos Adiciona(TFPessoa Elemento)
        {
            BPos Pos = new BPos(-1, -1);

            return Pos;
        }
        public TFPessoa Recupera(BPos Posição)
        {
            return new TFPessoa();
        }
    }
    public class TBlocoPeriódicos
    {
        public BPos Adiciona(TFPessoa Elemento)
        {
            BPos Pos = new BPos(-1, -1);

            return Pos;
        }
        public TFPeriódico Recupera(BPos Posição)
        {
            return new TFPeriódico();
        }
    }
    public class TBlocoConferências
    {
        public BPos Adiciona(TFPessoa Elemento)
        {
            BPos Pos = new BPos(-1, -1);

            return Pos;
        }
        public TFConferência Recupera(BPos Posição)
        {
            return new TFConferência();
        }
    }
    public class TBlocoInstituições
    {
        public BPos Adiciona(TFPessoa Elemento)
        {
            BPos Pos = new BPos(-1, -1);

            return Pos;
        }
        public TFInstituição Recupera(BPos Posição)
        {
            return new TFInstituição();
        }
    }
    public class TBlocoProduções
    {
        public BPos Adiciona(TFPessoa Elemento)
        {
            BPos Pos = new BPos(-1, -1);

            return Pos;
        }
        public TFProdBibliográfica Recupera(BPos Posição)
        {
            return new TFProdBibliográfica();
        }
    }

    /// DEFINIÇÃO DAS CLASSES QUE TERÃO INFORMAÇÕES CRUCIAIS SOMENTE */
    ///   * ESSA SERÁ A BASE DE DADOS QUE O PROGRAMA SE ORGANIZARÁ */
    ///   * ESSES DADOS SERÃO DEIXADOS NA MEMÓRIA E NUNCA SAIRÃO DE LÁ */  

    // Um tipo super-básico de ponteiro para um Nodo - contém somente o índice na lista-mestre respectiva
    public struct TPNodo
    {
        public Int64 Índice;            // Índice na correspondente lista
        public int Tipo;                // Tipo (0= pessoa; 1= Periódico; 2= Conferência; 3= Produção; 4= Instituição)
        
        // Constructor
        public TPNodo(Int64 Índice, int Tipo)
        {
            this.Índice = Índice;
            this.Tipo = Tipo;
        }
        // Override de Operadores
        public static bool operator ==(TPNodo E1, TPNodo E2)
        {
            if ((E1 == null) || (E2 == null)) return false;
            return ((E1.Tipo == E2.Tipo) && (E1.Índice == E2.Índice));
        }
        public static bool operator !=(TPNodo E1, TPNodo E2)
        {
            if ((E1 == null) || (E2 == null)) return true;
            return !((E1.Tipo == E2.Tipo) && (E1.Índice == E2.Índice));
        }
        public override bool Equals(System.Object obj)
        {
            return false; 
        }
        public override int GetHashCode()
        {
            return (int) (Índice ^ Tipo);
        }
    }

    // Um nodo de informação
    public class TInfoNodo : TDrawNodo
    {
        public List<TPNodo> Ligações;   // As ligações do elemento com todos os outros
        public string Nome;             // Somente possui palavras suficentemente interessantes para pesquisa rápida
        public string Iniciais;         // Para ainda mais rápida pesquisa
        public BPos Data;               // Posição no disco (bloco e offset) de todos os dados
        public TPNodo Nodo;             // Informação sobre que tipo é e onde está na lista principal

        // Constructors
        public TInfoNodo(string Nome, BPos Data) : base() 
        {
            foreach (string substr in Nome.Split(' '))
            {
                if (substr.Count() >= 3)
                    this.Nome = String.Concat(this.Nome, substr, " ");
                this.Iniciais = String.Concat(this.Iniciais, substr.Substring(0, 1));
            }
            if ((this.Nome == null) || (this.Nome.Length == 0))
                this.Nome = Nome;
            this.Data = Data;
            this.Nodo = new TPNodo(-1, -1);
            this.Ligações = new List<TPNodo>();
            InitializeLines();
        }

        public TInfoNodo()
        {
            this.Data.Bloco = -1;
            this.Data.Offset = -1;
        }
        // Função que verifica se o nome de entrada é o mesmo (ou seja, se todas as palavras de Value estão em Nome)
        public bool MesmoNome(string Value, bool ForceEqual)
        {
            // TODO - TERMINAR
            bool Cont = true;
            foreach (string substr in Value.Split(' '))
            {
                if (!Nome.Contains(substr))
                    Cont = false;
                if (substr.Length >= 1)
                    if (!Iniciais.Contains(substr.Substring(0, 1)))
                        Cont = false;
            }
            return Cont;
        }
        // Funções para Adicionar (e verificar) Ligações com outros Nodos
        public void AdicionaLigaçãoCom(params TPNodo[] Elementos)
        {
            // Para cada elemento
            foreach (TPNodo Nodo in Elementos)
            {
                // Procura uma ligação com esses elementos e esse peso
                int Índice = Ligações.IndexOf(Nodo);
                if (Índice == -1)
                {
                    Console.WriteLine("a");
                    // Se não havia, cria
                    Ligações.Add(Nodo);
                    NewLine(Ligações.Count - 1);
                }
            }
        }
        // Deleta uma ligação
        public void DeletaLigaçãoCom(params TPNodo[] Elementos)
        {
            foreach (TPNodo Nodo in Elementos)
            {
                int Índice = Ligações.IndexOf(Nodo);
                if (Índice > -1)
                {
                    Ligações.RemoveAt(Índice);
                    AppGraphics.DPrimitives.SetDrawability(Lines[Índice], false);
                }
            }
        }
        public bool ContémLigaçãoCom(params TPNodo[] Elementos)
        {
            bool Contém = true;
            foreach (TPNodo Nodo in Elementos)
            {
                Contém = (Contém && Ligações.Contains(Nodo));
            }
            return Contém;
        }
    }

    /// Gerente das Listas-Mestres
    public class TBigList : List<TInfoNodo>
    {
        public int Tipo;
        // Constrcutor
        public TBigList(int Tipo) : base()
        {
            this.Tipo = Tipo;
        }
        // Cria novos elementos
        public Int64 NovoNodo(params TInfoNodo[] Elementos)
        {
            foreach (TInfoNodo Nodo in Elementos)
            {
                if (ProcuraNodo(Nodo.Nome, true).Count == 0)
                {
                    Add(Nodo);
                    this[this.Count - 1].Nodo = new TPNodo(this.Count-1, Tipo);
                }
            }
            return Count - 1;
        }
        // Funções para procurar todos os Elementos com dado Critério
        public List<TInfoNodo> ProcuraNodo(string Nome, bool SomenteIgual)
        {
            return this.FindAll(p => p.MesmoNome(Nome, SomenteIgual));
        }
        public List<TInfoNodo> ProcuraNodo(int Tipo)
        {
            return this.FindAll(p => (p.Nodo.Tipo == Tipo));
        }
        public List<TInfoNodo> ProcuraNodo(bool Drawable)
        {
            return this.FindAll(p => (p.Drawable == Drawable));
        }
        public TInfoNodo ProcuraNodoSelecionado()
        {
            return this.Find(p => (p.Selected == true));
        }
        public List<TInfoNodo> ProcuraNodo(BPos Data)
        {
            return this.FindAll(p => (p.Data == Data));
        }
        public List<TInfoNodo> ProcuraNodo(Predicate<TInfoNodo> match)
        {
            return this.FindAll(match);
        }
    }

    // classe de teste
    public class TesteEstaCristiano
    {
        string Nome;
        BPos Banana;
        TPNodo Cebola;
        Int64 Bliblu;
        TBigList Listona;
        public TesteEstaCristiano()
        {    
            // coloca algumas coisas pra testar - tenha certeza que é igual
            this.Nome = new String(' ' , 0);
            this.Banana = new BPos(2, 3);
            this.Cebola = new TPNodo(3, 2);
            this.Bliblu = 33;
            this.Listona = new TBigList(3);
            this.Listona.NovoNodo(new TInfoNodo("Bliblu", Banana));
        }
        public override string ToString()
        {
            // Fiz até um ToString() pra ti pra tu poder testar
            return Nome + ", " + Banana.Bloco + ", " + Banana.Offset + ", " + Cebola.Índice + ", " + Cebola.Tipo + ", " + Bliblu + ", " + Listona.Count + ", " + Listona[0].Nome + ", " + Listona[0].Ligações.Count + ", " + Listona[0].Nodo.Tipo;
        }
    }
    // TesteEstaCristiano bla = new TesteEstaCristiano();
    // Console.WriteLine(bla.ToString());
}
