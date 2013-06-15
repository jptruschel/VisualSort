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
        public override string ToString()
        {
            return "Bloco: " + Bloco + "; Offset: " + Offset;
        }
    }

    /// DEFINIÇÃO DAS CLASSES QUE TERÃO TODOS OS DADOS (GUARDADO EM DISCO)
    ///   * ESSA SERÁ A BASE DE DADOS DE TUDO QUE O PROGRAMA TERÁ  
    ///   * ESSES DADOS SERÃO DEIXADOS NO DISCO E CARREGADOS CONFORME NECESSÁRIO 
    // Um Periódico
    public class TFPeriódico
    {
        long ISSN1, ISSN2;      // Códigos de ISSN
        string Nome;            // Nome do periódico
        string Qualis;          // Qualis
    }
    // Uma conferência
    public class TFConferência
    {
        string Sigla, Nome;     // Nome (e Sigla)
        string Caráter;         // Caráter da conferência - INTERNACIONAL ou NACIONAL
        string Qualis;          // Qualis
    }
    // Uma Produção Bibliográfica
    public class TFProdBibliográfica
    {
        string Título;          // Título
        string ISSN;            // ISSN relativo
        int Periodico;          // Índice na Lista de Periódicos
        string MeioDivulgação;  // Meio de Divulgação da Produção
        string Idioma;          // Idioma da Produção
        int AnoPublicação;      // Ano da Publicação
        string Natureza;        // Natureza (COMPLETO, ESTENDIDO, RESUMO)
    }
    // Uma Instituição
    public class TFInstituição
    {
        string Nome;            // Nome da instituição
    }
    // Pessoas
    public class TFPessoa
    {
        string NomeCompleto;    // Nome Completo (na Ordem Correta)
        List<string> CNomes;    // Nomes Curto Gerados a partir do nome completo (eg: "Leandro Krug Wives"; "Wives LK"; "Wives, LK"; "LK Wives")
        string País;            // País de nascimento
        string TextoResumo;     // Resumo da pessoa
        List<int> ProduçõesBib; // Índices de todas as Produções
    }

    /// DEFINIÇÃO DAS CLASSES QUE TERÃO INFORMAÇÕES CRUCIAIS SOMENTE */
    ///   * ESSA SERÁ A BASE DE DADOS QUE O PROGRAMA SE ORGANIZARÁ */
    ///   * ESSES DADOS SERÃO DEIXADOS NA MEMÓRIA E NUNCA SAIRÃO DE LÁ */
    ///   

    // Um tipo super-básico de Elemento - contém somente o índice na lista respectiva
    public struct TBEl
    {
        public Int64 Índice;            // Índice na correspondente lista
        public int Tipo;                // Tipo (0= pessoa; 1= Periódico; 2= Conferência; 3= Produção; 4= Instituição)
        
        // Constructor
        public TBEl(int Índice, int Tipo)
        {
            this.Índice = Índice;
            this.Tipo = Tipo;
        }
        // Override de Operadores
        public static bool operator ==(TBEl E1, TBEl E2)
        {
            if ((E1 == null) || (E2 == null)) return false;
            return ((E1.Tipo == E2.Tipo) && (E1.Índice == E2.Índice));
        }
        public static bool operator !=(TBEl E1, TBEl E2)
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

    // Uma ligação - liga dois ElementosBásicos (por índices das listas-mestre existentes)
    public class TLigação
    {
        public TBEl[] Elementos;        // Elementos ligados (geralmente 2)                
        public int Peso;                // Caracteriza o Peso da ligação (Pessoa->Produção pode valer mais que Pessoa->Conferência)
        public bool Exist;              // Booleano que diz se existe - desconsiderar ligação se false
        // Constructor
        public TLigação(int Peso, params TBEl[] Elementos)
        {
            this.Elementos = new TBEl[Elementos.Length];
            int i = 0;
            foreach (TBEl E in Elementos)
            {
                this.Elementos[i] = E;
                i++;
            }
            this.Peso = Peso;
            this.Exist = true;
        }
        // Retorna true se o elemento está sendo linkado nessa ligação
        public bool Contains(TBEl Elemento)
        {
            int i = 0;
            while (i < this.Elementos.Length)
            {
                if (this.Elementos[i] == Elemento)
                    return true;
                i++;
            }
            return false;
        }
    }
    // Manage uma lista de Ligações
    public class TLigaçãoList
    {
        public List<TLigação> Ligas;        // A lista de Ligações

        // Constructor
        public TLigaçãoList()
        {
            Ligas = new List<TLigação>();
        }
        // Nova ligação
        public int NovaLigação(int Peso, params TBEl[] Elementos)
        {
            // Procura uma ligação com esses elementos e esse peso
            int Índice = ProcuraLigação(Peso, Elementos);
            if (Índice > -1)
                return Índice;
            // Se não havia, cria
            Ligas.Add(null);
            Ligas[Ligas.Count - 1] = new TLigação(Peso, Elementos);
            return Ligas.Count - 1;
        }
        // Deleta uma ligação
        public void DeletaLigação(int i)
        {
            // A ligação ainda existe, porém, ela não será mais usada
            Ligas[i].Exist = false;
            Ligas[i].Elementos = null;
            Ligas[i].Peso = 0;
        }
        // Retorna a 1ª ligação que possui todos os elementos de entrada (e Peso igual, se não for -1) - caso não ache, retorna -1
        public int ProcuraLigação(int Peso, params TBEl[] Elementos)
        {
            int i = 0;
            while (i < Ligas.Count)
            {
                bool Contem = true;
                foreach (TBEl E in Elementos)
                {
                    if (!Ligas[i].Contains(E))
                        Contem = false;
                }
                if ((Elementos.Length == Ligas[i].Elementos.Length) && (Contem == true) && 
                    ((Ligas[i].Peso == Peso) || (Peso == -1)))
                    return i;
                i++;
            }
            return -1;
        }
    }

    // Manage uma lista de índices de ligações
    public class TLigaçãoPList
    {
        private List<Int64> Ligas;          // Lista de 'ponteiros'
        public TBEl Parent;                 // A informação sobre quem é o pai

        // Constructor
        public TLigaçãoPList(TBEl Parent)
        {
            Ligas = new List<Int64>();
            this.Parent = Parent;
        }
        // Adiciona uma nova ligação
        public int NovaLigação(TBEl Elemento, int Peso)
        {
            // Procura nesse set de ligações
            int Índice = ProcuraLigação(Peso, Elemento, Parent);
            // Se achou, retorna
            if (Índice > -1)
                return Índice;

            // Se não, segue para o resto das ligações
            // Procura em todas as ligações uma ligação dessas (não deve ter, mas pra ter certeza)
            Índice = Program.Ligações.ProcuraLigação(Peso, Parent, Elemento);
            // Se não existe, cria
            if (Índice == -1)
                Índice = Program.Ligações.NovaLigação(Peso, Parent, Elemento);

            // Cria essa ligação nesse set de ligações
            Ligas.Add(Índice);

            // Retorna o índice
            return Ligas.Count - 1;
        }
        // Procura uma ligação dentro dessas ligações
        public int ProcuraLigação(int Peso, params TBEl[] Elementos)
        {
            int i = 0;
            while (i < Ligas.Count)
            {
                bool Contem = true;
                foreach (TBEl E in Elementos)
                {
                    if (!Program.Ligações.Ligas[(int)Ligas[i]].Contains(E))
                        Contem = false;
                }
                if ((Contem == true) && ((Program.Ligações.Ligas[(int)Ligas[i]].Peso == Peso) || (Peso == -1)))
                    return i;
                i++;
            }
            return -1;
        }
    }

    // Uma definição básica para um Nodo - nome e sigla
    public abstract class TBaseNodo
    {
        public string Nome;             // Somente possui palavras suficentemente interessantes para pesquisa rápida
        public string Iniciais;         // Para ainda mais rápida pesquisa
        public BPos Data;               // Posição no disco (bloco e offset) de todos os dados

        // Constructors
        public TBaseNodo(string Nome) 
        {
            this.Nome = new string(' ', 0);
            this.Iniciais = new string(' ', 0);
            foreach (string substr in Nome.Split(' '))
            {
                if (substr.Count() >= 3)
                    this.Nome = String.Concat(this.Nome, substr, " ");
                this.Iniciais = String.Concat(this.Iniciais, substr.Substring(0, 1));
            }
        }
        protected TBaseNodo(string Nome, bool AddAllWords = false)
        {
            this.Nome = new string(' ', 0);
            this.Iniciais = new string(' ', 0);
            foreach (string substr in Nome.Split(' '))
            {
                if ((substr.Count() >= 3) || (AddAllWords))
                    this.Nome = String.Concat(this.Nome, substr, " ");
                this.Iniciais = String.Concat(this.Iniciais, substr.Substring(0, 1));
            }
        }
        public TBaseNodo()
        {
            this.Nome = new string(' ', 0);
            this.Iniciais = new string(' ', 0);
        }
    }
    // Uma Pessoa
    public class TPessoa: TBaseNodo
    {
        public List<List<int>> Pessoas; // As pessoas que estão conectadas a essa (1ª dimensão: Ordem de atrelamento; 2ª dimensão: Índice da Ligação)
        public List<List<int>> Inst;    // Lista das Instituições (mesma lógica das Pessoas)
        public List<List<int>> Prod;    // Lista das Produções Bibliográficas (mesma lógica das Pessoas)
        public List<List<int>> Conf;    // Lista das Conferências (mesma lógica das Pessoas)
        public List<List<int>> Per;     // Lista dos Periódicos (mesma lógica das Pessoas)

        // Constructor
        public TPessoa(string Nome, BPos Data) :base(Nome, true)
        {
            this.Pessoas = new List<List<int>>();
            this.Inst = new List<List<int>>();
            this.Prod = new List<List<int>>();
            this.Conf = new List<List<int>>();
            this.Per = new List<List<int>>();
            this.Data = Data;
        }
    }
    // Um Periódico
    class TPeriódico : TBaseNodo
    {
        
    }
    // Uma Conferência
    class TConferência : TBaseNodo
    {
        
    }
    // Uma Produção Bibliográfica
    class TProdBibliográfica : TBaseNodo
    {
        string Título;                  // Título Completo da Produção
        string ISSN;                    // ISSN
        int AnoPublicação;              // Ano da publicação
        string Natureza;                // Natureza
        List<int> Autores;              // Índices dos Autores (em ordem de importância)
        List<string> Chaves;            // Palavras-chave da Produção
        List<int> Áreas;                // Áreas do conhecimento abrangidas pela produção
        BPos Data;                      // Posição no disco (bloco e offset) do resto dos dados desse periódico
    }
    // Uma Instituição
    class TInstituição : TBaseNodo
    {
        
    }

    class Definitions
    {

    }
}
