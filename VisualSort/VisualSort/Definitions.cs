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
    // Uma estrutura que contém uma lista de inteiros - usada como saída das funções de busca
    //   CUIDADO: não usar = para trocar o valor de uma ListInt, use Assign!
    public class ListInt
    {
        private List<Int64> List;
        public ListInt()
        {
            List = new List<Int64>();
        }
        public int AddValue(Int64 Value)
        {
            int i = List.IndexOf(Value);
            if (i == -1)
            {
                List.Add(Value);
                return List.Count - 1;
            }
            else
                return i;
        }
        public bool RemoveValue(Int64 Value)
        {
            return List.Remove(Value);
        }
        public void RemoveAt(int Index)
        {
            List.RemoveAt(Index);
        }
        public Int64 ValueAt(int Index)
        {
            if (Index < List.Count)
                return this.List[Index];
            return -1;
        }
        public bool SetValueAt(int Index, Int64 Value)
        {
            if (!this.Contains(Value))
            {
                if (Index < List.Count)
                {
                    this.List[Index] = Value;
                    return true;
                }
                else
                    return false;
            }
            else
                return false;
        }
        public bool Contains(params Int64[] Values)
        {
            foreach (Int64 i in Values)
                if (!List.Contains(i))
                    return false;
            return true;
        }
        public void Assign(ListInt Value)
        {
            List.Clear();
            for (int i = 0; i < Value.List.Count; i++)
                List.Add(Value.List[i]);
        }
        public void Concat(ListInt Value)
        {
            for (int i = 0; i < Value.List.Count; i++)
                AddValue(Value.List[i]);
        }
        public override string ToString()
        {
            string temp = new String(' ', 0);
            for (int i = 0; i < List.Count - 1; i++)
                temp += List[i] + ", ";
            return temp + List[List.Count - 1];
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
        public TFPessoa Recupera(BPos Posição)
        {
            return new TFPessoa();
        }
    }
    public class TBlocoPeriódicos
    {
        public TFPeriódico Recupera(BPos Posição)
        {
            return new TFPeriódico();
        }
    }
    public class TBlocoConferências
    {
        public TFConferência Recupera(BPos Posição)
        {
            return new TFConferência();
        }
    }
    public class TBlocoInstituições
    {
        public TFInstituição Recupera(BPos Posição)
        {
            return new TFInstituição();
        }
    }
    public class TBlocoProduções
    {
        public TFProdBibliográfica Recupera(BPos Posição)
        {
            return new TFProdBibliográfica();
        }
    }

    /// DEFINIÇÃO DAS CLASSES QUE TERÃO INFORMAÇÕES CRUCIAIS SOMENTE */
    ///   * ESSA SERÁ A BASE DE DADOS QUE O PROGRAMA SE ORGANIZARÁ */
    ///   * ESSES DADOS SERÃO DEIXADOS NA MEMÓRIA E NUNCA SAIRÃO DE LÁ */
    ///   

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
    // Uma ligação - liga dois Ponteiros de Nodos
    public class TLigação
    {
        public TPNodo[] Elementos;      // Elementos ligados (geralmente 2)                
        public int Peso;                // Caracteriza o Peso da ligação (Pessoa->Produção pode valer mais que Pessoa->Conferência)
        public bool Exist;              // Booleano que diz se existe - desconsiderar ligação se false
        // Constructor
        public TLigação(int Peso, params TPNodo[] Elementos)
        {
            this.Elementos = new TPNodo[Elementos.Length];
            int i = 0;
            foreach (TPNodo E in Elementos)
            {
                this.Elementos[i] = E;
                i++;
            }
            this.Peso = Peso;
            this.Exist = true;
        }
        // Retorna true se o elemento está sendo linkado nessa ligação
        public bool Contains(TPNodo Elemento)
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

    // Lista de Ligações
    public class TLigaçãoList
    {
        public List<TLigação> Ligas;    // A lista de Ligações

        // Constructor
        public TLigaçãoList()
        {
            this.Ligas = new List<TLigação>();
        }
        // Nova ligação
        public int NovaLigação(int Peso, params TPNodo[] Elementos)
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
            if (i < Ligas.Count)
            {
                // A ligação ainda existe, porém, ela não será mais usada
                Ligas[i].Exist = false;
                Ligas[i].Elementos = null;
                Ligas[i].Peso = 0;
            }
        }
        // Retorna a 1ª ligação que possui todos os elementos de entrada (e Peso igual, se não for -1) - caso não ache, retorna -1
        public int ProcuraLigação(int Peso, params TPNodo[] Elementos)
        {
            int i = 0;
            while (i < Ligas.Count)
            {
                bool Contem = true;
                foreach (TPNodo E in Elementos)
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
        // Retorna todas as ligações que possuem os elementos de entrada (e Peso, se (Peso != -1))
        public ListInt ProcuraLigações(int Peso, params TPNodo[] Elementos)
        {
            ListInt Temp = new ListInt();
            int i = 0;
            while (i < Ligas.Count)
            {
                bool Contem = true;
                foreach (TPNodo E in Elementos)
                {
                    if (!Ligas[i].Contains(E))
                        Contem = false;
                }
                if ((Elementos.Length == Ligas[i].Elementos.Length) && (Contem == true) &&
                    ((Ligas[i].Peso == Peso) || (Peso == -1)))
                    Temp.AddValue(i);
                i++;
            }
            return Temp;
        }
    }

    // Manage uma lista de índices de ligações - cada nodo terá uma dessas
    public class TLigaçãoPList
    {
        private List<Int64> Ligas;      // Lista de 'ponteiros'
        public TPNodo Parent;           // A informação sobre quem é o pai

        // Constructor
        public TLigaçãoPList(TPNodo Parent)
        {
            Ligas = new List<Int64>();
            this.Parent = Parent;
        }
        // Adiciona uma nova ligação
        public int NovaLigação(TPNodo Elemento, int Peso)
        {
            // Procura nesse set de ligações
            int Índice = ProcuraLigação(Peso, Elemento, Parent);
            // Se achou, retorna
            if (Índice > -1)
                return Índice;

            // Se não, segue para o resto das ligações
            // Procura em todas as ligações uma ligação dessas (não deve ter, mas pra ter certeza)
            Índice = Program.mLigações.ProcuraLigação(Peso, Parent, Elemento);
            // Se não existe, cria
            if (Índice == -1)
                Índice = Program.mLigações.NovaLigação(Peso, Parent, Elemento);

            // Cria essa ligação nesse set de ligações
            Ligas.Add(Índice);

            // Retorna o índice
            return Ligas.Count - 1;
        }
        // Procura uma ligação dentro dessas ligações (desconsidera Peso se Pes ==-1)
        public int ProcuraLigação(int Peso, params TPNodo[] Elementos)
        {
            int i = 0;
            while (i < Ligas.Count)
            {
                bool Contem = true;
                foreach (TPNodo E in Elementos)
                {
                    if (!Program.mLigações.Ligas[(int)Ligas[i]].Contains(E))
                        Contem = false;
                }
                if ((Contem == true) && ((Program.mLigações.Ligas[(int)Ligas[i]].Peso == Peso) || (Peso == -1)))
                    return i;
                i++;
            }
            return -1;
        }
        // Procura todas as ligações com esses elementos e esse peso (se não for -1)
        public ListInt ProcuraLigações(int Peso, params TPNodo[] Elementos)
        {
            ListInt Temp = new ListInt();
            int i = 0;
            while (i < Ligas.Count)
            {
                bool Contem = true;
                foreach (TPNodo E in Elementos)
                {
                    if (!Program.mLigações.Ligas[(int)Ligas[i]].Contains(E))
                        Contem = false;
                }
                if ((Contem == true) && ((Program.mLigações.Ligas[(int)Ligas[i]].Peso == Peso) || (Peso == -1)))
                    Temp.AddValue(i);
                i++;
            }
            return Temp;
        }
    }

    // Uma definição básica para um Nodo - nome e sigla
    public abstract class TBaseNodo
    {
        public string Nome;             // Somente possui palavras suficentemente interessantes para pesquisa rápida
        public string Iniciais;         // Para ainda mais rápida pesquisa
        public BPos Data;               // Posição no disco (bloco e offset) de todos os dados
        public Int64 Índice;            // Índice de si mesmo na lista respectiva

        // Constructors
        public TBaseNodo(string Nome, Int64 Índice, BPos Data) 
        {
            this.Nome = new string(' ', 0);
            this.Iniciais = new string(' ', 0);
            foreach (string substr in Nome.Split(' '))
            {
                if (substr.Count() >= 3)
                    this.Nome = String.Concat(this.Nome, substr, " ");
                this.Iniciais = String.Concat(this.Iniciais, substr.Substring(0, 1));
            }
            this.Data = Data;
            this.Índice = Índice;
        }
        protected TBaseNodo(string Nome, BPos Data, Int64 Índice, bool AddAllWords = false)
        {
            this.Nome = new string(' ', 0);
            this.Iniciais = new string(' ', 0);
            foreach (string substr in Nome.Split(' '))
            {
                if ((substr.Count() >= 3) || (AddAllWords))
                    this.Nome = String.Concat(this.Nome, substr, " ");
                this.Iniciais = String.Concat(this.Iniciais, substr.Substring(0, 1));
            }
            this.Data = Data;
            this.Índice = Índice;
        }
        public TBaseNodo()
        {
            this.Nome = new string(' ', 0);
            this.Iniciais = new string(' ', 0);
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
                if (!Iniciais.Contains(substr.Substring(0, 1)))
                    Cont = false;
            }
            return Cont;
        }
    }
    // Uma Pessoa
    public class TPessoa: TBaseNodo
    {
        public TLigaçãoPList Pessoas; // Lista das Pessoas (Índice da Ligação)
        public TLigaçãoPList Inst;    // Lista das Instituições (Índice da Ligação)
        public TLigaçãoPList Prod;    // Lista das Produções Bibliográficas (Índice da Ligação)
        public TLigaçãoPList Conf;    // Lista das Conferências (Índice da Ligação)
        public TLigaçãoPList Per;     // Lista dos Periódicos (Índice da Ligação)

        // Constructor (Data= onde está o resto dos dados; Índice= Índice de si na lista-mestre)
        public TPessoa(string Nome, BPos Data, Int64 Índice) :base(Nome, Data, Índice, true)
        {
            // Cria todas as listas, passando a posição de si como parâmetro
            this.Pessoas = new TLigaçãoPList(ToNodo());
            this.Inst = new TLigaçãoPList(ToNodo());
            this.Prod = new TLigaçãoPList(ToNodo());
            this.Conf = new TLigaçãoPList(ToNodo());
            this.Per = new TLigaçãoPList(ToNodo());
        }
        public TPNodo ToNodo()
        {
            return new TPNodo(Índice, 0);
        }
    }
    // Um Periódico
    public class TPeriódico : TBaseNodo
    {
        public TLigaçãoPList Prod;    // Lista das Produções Bibliográficas (Índice da Ligação)

        public TPeriódico(string Nome, BPos Data, Int64 Índice)
            : base(Nome, Data, Índice, true)
        {
            Prod = new TLigaçãoPList(ToNodo());
        }
        public TPNodo ToNodo()
        {
            return new TPNodo(Índice, 1);
        }
    }
    // Uma Conferência
    public class TConferência : TBaseNodo
    {
        public TLigaçãoPList Pessoas; // Lista das Pessoas (Índice da Ligação)

        public TConferência(string Nome, BPos Data, Int64 Índice)
            : base(Nome, Data, Índice, true)
        {
            Pessoas = new TLigaçãoPList(ToNodo());
        }
        public TPNodo ToNodo()
        {
            return new TPNodo(Índice, 2);
        }
    }
    // Uma Produção Bibliográfica
    public class TProdBibliográfica : TBaseNodo
    {
        string Título;                  // Título Completo da Produção
        string ISSN;                    // ISSN
        int AnoPublicação;              // Ano da publicação
        string Natureza;                // Natureza
        public TLigaçãoPList Autores;   // Lista dos Autores (pessoas) (em ordem de importância)
        List<string> Chaves;            // Palavras-chave da Produção

        public TProdBibliográfica(string Nome, BPos Data, Int64 Índice, bool CarregarData)
            : base(Nome, Data, Índice, true)
        {
            Título = Nome;
            Autores = new TLigaçãoPList(this.ToNodo());
            Chaves = new List<string>();
            // Se deve carregar os dados pelo BPos
            if (CarregarData)
            {
                TFProdBibliográfica temp = new TFProdBibliográfica();
                temp = Program.fProduções.Recupera(Data);
                this.ISSN = temp.ISSN;
                this.AnoPublicação = temp.AnoPublicação;
                this.Natureza = temp.Natureza;
            }
        }
        public void AdicionaAutores(params int[] Elementos)
        {
            foreach (int i in Elementos)
            {
                if (!ContémAutor(i))
                    Autores.NovaLigação(new TPNodo(i, 0), -1);
            }
        }
        public bool ContémAutor(int Índice)
        {
            return (Autores.ProcuraLigação(-1, new TPNodo(Índice, 0)) > -1);
        }
        public void AdicionaChaves(params string[] Elementos)
        {
            foreach (string s in Elementos)
            {
                if (!ContémChave(s))
                    Chaves.Add(s);
            }
        }
        public bool ContémChave(string S)
        {
            return Chaves.Contains(S);
        }
        public TPNodo ToNodo()
        {
            return new TPNodo(Índice, 3);
        }
    }
    // Uma Instituição
    public class TInstituição : TBaseNodo
    {
        public TLigaçãoPList Pessoas; // Lista das Pessoas (Índice da Ligação)

        public TInstituição(string Nome, BPos Data, Int64 Índice)
            : base(Nome, Data, Índice, true)
        {
            Pessoas = new TLigaçãoPList(ToNodo());
        }
        public TPNodo ToNodo()
        {
            return new TPNodo(Índice, 4);
        }
    }

    /// Gerentes das Listas-Mestres (essas classes serão instanciadas)
    // Lista de Pessoas
    public class TPessoasList
    {
        public List<TPessoa> Pessoas;   // A lista de Pessoas

        // Constrcutor
        public TPessoasList()
        {
            this.Pessoas = new List<TPessoa>();
        }
        // Nova Pessoa
        public Int64 NovaPessoa(string Nome, BPos Data)
        {
            // Procura uma pessoa com esse nome
            Int64 Índice = ProcuraPessoa(Nome, false);
            if (Índice > -1)
                return Índice;
            // Se não havia, cria
            Pessoas.Add(null);
            Pessoas[Pessoas.Count - 1] = new TPessoa(Nome, Data, Pessoas.Count - 1);
            return Pessoas.Count - 1;
        }
        // Procura uma Pessoa por palavras-chave
        public Int64 ProcuraPessoa(string Nome, bool SomenteIgual)
        {
            return Pessoas.Find(p => p.MesmoNome(Nome, SomenteIgual)).Índice;
        }
        // Procura todas as Pessos com a palavras-chave
        public ListInt ProcuraPessoas(string Nome, bool SomenteIgual)
        {
            List<TPessoa> newList = new List<TPessoa>();
            newList = Pessoas.FindAll(p => p.MesmoNome(Nome, SomenteIgual));
            ListInt temp = new ListInt();
            for (int i = 0; i < newList.Count; i++)
                temp.AddValue(newList[i].Índice);
            return temp;
        }
    }
    // Lista de Produções Bibliográficas
    public class TProduçõesList
    {
        public List<TProdBibliográfica> Prod;   // as produções

        // Constructor
        public TProduçõesList()
        {
            Prod = new List<TProdBibliográfica>();
        }
        // Nova Produção
        public Int64 NovaProdução(string Nome, BPos Data, bool CarregarData)
        {
            // Procura uma Produção com esse nome
            Int64 Índice = ProcuraProdução(Nome, false);
            if (Índice > -1)
                return Índice;
            // Se não havia, cria
            Prod.Add(null);
            Prod[Prod.Count - 1] = new TProdBibliográfica(Nome, Data, Prod.Count - 1, CarregarData);
            return Prod.Count - 1;
        }
        // Procura uma Produção por palavras-chave
        public Int64 ProcuraProdução(string Nome, bool SomenteIgual)
        {
            return Prod.Find(p => p.MesmoNome(Nome, SomenteIgual)).Índice;
        }
        // Procura todas as Produções com a palavras-chave
        public ListInt ProcuraProduções(string Nome, bool SomenteIgual)
        {
            List<TProdBibliográfica> newList = new List<TProdBibliográfica>();
            newList = Prod.FindAll(p => p.MesmoNome(Nome, SomenteIgual));
            ListInt temp = new ListInt();
            for (int i = 0; i < newList.Count; i++)
                temp.AddValue(newList[i].Índice);
            return temp;
        }
    }
    // Lista de Periódicos
    public class TPeriódicosList
    {
        public List<TPeriódico> Perid;  // A lista de Periódicos

        // Constructor
        public TPeriódicosList()
        {
            Perid = new List<TPeriódico>();
        }
        // Novo Periódico
        public Int64 NovoPeriódico(string Nome, BPos Data)
        {
            // Procura um periódico com esse nome
            Int64 Índice = ProcuraPeriódico(Nome, false);
            if (Índice > -1)
                return Índice;
            // Se não havia, cria
            Perid.Add(null);
            Perid[Perid.Count - 1] = new TPeriódico(Nome, Data, Perid.Count - 1);
            return Perid.Count - 1;
        }
        // Procura um Periódico por palavras-chave
        public Int64 ProcuraPeriódico(string Nome, bool SomenteIgual)
        {
            return Perid.Find(p => p.MesmoNome(Nome, SomenteIgual)).Índice;
        }
        // Procura todos os Periódicos com a palavras-chave
        public ListInt ProcuraPeriódicos(string Nome, bool SomenteIgual)
        {
            List<TPeriódico> newList = new List<TPeriódico>();
            newList = Perid.FindAll(p => p.MesmoNome(Nome, SomenteIgual));
            ListInt temp = new ListInt();
            for (int i = 0; i < newList.Count; i++)
                temp.AddValue(newList[i].Índice);
            return temp;
        }
    }
    // Lista de Conferências
    public class TConferênciaList
    {
        public List<TConferência> Conf; // A lista de Conferências

        // Constructor
        public TConferênciaList()
        {
            Conf = new List<TConferência>();
        }
        // Novo Periódico
        public Int64 NovaConferência(string Nome, BPos Data)
        {
            // Procura uma conferência com esse nome
            Int64 Índice = ProcuraConferência(Nome, false);
            if (Índice > -1)
                return Índice;
            // Se não havia, cria
            Conf.Add(null);
            Conf[Conf.Count - 1] = new TConferência(Nome, Data, Conf.Count - 1);
            return Conf.Count - 1;
        }
        // Procura uma Conferência por palavras-chave
        public Int64 ProcuraConferência(string Nome, bool SomenteIgual)
        {
            return Conf.Find(p => p.MesmoNome(Nome, SomenteIgual)).Índice;
        }
        // Procura todas as Conferências com a palavras-chave
        public ListInt ProcuraConferências(string Nome, bool SomenteIgual)
        {
            List<TConferência> newList = new List<TConferência>();
            newList = Conf.FindAll(p => p.MesmoNome(Nome, SomenteIgual));
            ListInt temp = new ListInt();
            for (int i = 0; i < newList.Count; i++)
                temp.AddValue(newList[i].Índice);
            return temp;
        }
    }
    // Lista de Instituições
    public class TInstituiçãoList
    {
        public List<TInstituição> Inst; // A lista de Instituições

        // Constructor
        public TInstituiçãoList()
        {
            Inst = new List<TInstituição>();
        }
        // Novo Periódico
        public Int64 NovaInstituição(string Nome, BPos Data)
        {
            // Procura uma Instituição com esse nome
            Int64 Índice = ProcuraInstituição(Nome, false);
            if (Índice > -1)
                return Índice;
            // Se não havia, cria
            Inst.Add(null);
            Inst[Inst.Count - 1] = new TInstituição(Nome, Data, Inst.Count - 1);
            return Inst.Count - 1;
        }
        // Procura uma Instituição por palavras-chave
        public Int64 ProcuraInstituição(string Nome, bool SomenteIgual)
        {
            return Inst.Find(p => p.MesmoNome(Nome, SomenteIgual)).Índice;
        }
        // Procura todas as Instituição com a palavras-chave
        public ListInt ProcuraInstituições(string Nome, bool SomenteIgual)
        {
            List<TInstituição> newList = new List<TInstituição>();
            newList = Inst.FindAll(p => p.MesmoNome(Nome, SomenteIgual));
            ListInt temp = new ListInt();
            for (int i = 0; i < newList.Count; i++)
                temp.AddValue(newList[i].Índice);
            return temp;
        }
    }

    //testa essa:
    public class TesteEstaCristiano
    {
        string Nome;
        BPos Banana;
        TPNodo Cebola;
        Int64 Bliblu;
        List<TLigação> Listona;
        public TesteEstaCristiano()
        {    
            // coloca algumas coisas pra testar - tenha certeza que é igual
            this.Nome = new String(' ' , 0);
            this.Banana = new BPos(2, 3);
            this.Cebola = new TPNodo(3, 2);
            this.Bliblu = 33;
            this.Listona = new List<TLigação>();
            this.Listona.Add(null);
            this.Listona[0] = new TLigação(20, Cebola);
        }
        public override string ToString()
        {
            // Fiz até um ToString() pra ti pra tu poder testar
            return Nome + ", " + Banana.Bloco + ", " + Banana.Offset + ", " + Cebola.Índice + ", " + Cebola.Tipo + ", " + Bliblu + ", " + Listona.Count + ", " + Listona[0].Peso + ", " + Listona[0].Elementos[0].Índice + ", " + Listona[0].Elementos[0].Tipo;
        }
    }
    // TesteEstaCristiano bla = new TesteEstaCristiano();
    // Console.WriteLine(bla.ToString());

    CRISTIANO LEIA ISSO:
    NÃO TA COMENTADO PRA TU VER CASO TU TENTE COMPILAR
    /*ISSO TA BEM INCOMPLETO E FALTA BASTANTE
     * NÃO FAÇA NADA COM ESSE CÓDIGO, TENTA ENTENDER O QUE TA ACONTECENDO, MAS FALTA BASTANTE
     * O QUE EU VOU FAZER: 
     * 1. TERMINAR AS ESTRUTURAS DE DADOS QUE TRABALHAM EM MEMÓRIA RAM
     *   1.1 COLOCAR EM TODAS AS LISTAS-MESTRES FUNÇÕES PARA ADICIONAR LIGAÇÕES COM TUDO QUE PODE SER LIGADO
     * 2. IMPLEMENTAR FUNÇÕES DE FÁCIL ADIÇÃO E RECUPERAÇÃO DE LIGAÇÕES ENTRE TODOS OS TIPOS DE DADOS
     * 3. ADICIONAR FUNÇÕES PARA TRABALHAR COM TODO O VOLUME DE DADOS, SALVANDO NO DISCO RÍGIDO E RECUPERANDO DEPOIS
      
     * EU ACHO MELHOR TU DAR UMA OLHADA E ANOTAR O QUE TU QUER MUDAR, MAS DEIXAR TUDO COMO ESTÁ; CONVERSAMOS AMANHÃ
     * SE TU QUISER IR FAZENDO ALGO, FAZ FUNÇÕES QUE SALVE UMA INSTÂNCIA DE "TesteEstaCristiano" PARA UM BINÁRIO E CARREGUE DE VOLTA
         tem que ser essa classe inútil porque ainda tem muito a arrumar pra ter a função pronta, mas ali tem tudo que vamos precisar
     *  BTW, SE TU QUISER COMPILAR PRA VER SE TA TUDO BEM, A COISINHA QUE GIRA É TIPO UM LOADING QUE A GENTE BOTA NUMA THREAD ENQUANTO CARREGA AS COISAS
     */
    class Definitions
    {

    }
}
