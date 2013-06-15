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
        List<int> ProduçõesBib; // Índices de todas as Produções (na lista-mestre)
    }

    // Blocos
    public class TBlocoPessoas
    {
        TFPessoa[] Bloco;
        int Count;
        public TBlocoPessoas()
        {
            Bloco = new TFPessoa[512];
            Count = 0;
        }
        public void AdicionaPessoa()
        {

        }
    }

    /// DEFINIÇÃO DAS CLASSES QUE TERÃO INFORMAÇÕES CRUCIAIS SOMENTE */
    ///   * ESSA SERÁ A BASE DE DADOS QUE O PROGRAMA SE ORGANIZARÁ */
    ///   * ESSES DADOS SERÃO DEIXADOS NA MEMÓRIA E NUNCA SAIRÃO DE LÁ */
    ///   

    // Um tipo super-básico de ponteiro para um Nodo - contém somente o índice na lista-mestre respectiva
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

    // Uma ligação - liga dois Ponteiros de Nodos
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
        public TBaseNodo(string Nome, BPos Data) 
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
        }
        protected TBaseNodo(string Nome, BPos Data, bool AddAllWords = false)
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
        }
        public TBaseNodo()
        {
            this.Nome = new string(' ', 0);
            this.Iniciais = new string(' ', 0);
            this.Data.Bloco = -1;
            this.Data.Offset = -1;
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
        public TPessoa(string Nome, BPos Data, int Índice) :base(Nome, Data, true)
        {
            // Cria todas as listas, passando a posição de si como parâmetro
            this.Pessoas = new TLigaçãoPList(new TBEl(Índice, 0));
            this.Inst = new TLigaçãoPList(new TBEl(Índice, 0));
            this.Prod = new TLigaçãoPList(new TBEl(Índice, 0));
            this.Conf = new TLigaçãoPList(new TBEl(Índice, 0));
            this.Per = new TLigaçãoPList(new TBEl(Índice, 0));
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


    //testa essa:
    class TesteEstaCristiano
    {
        string Nome;
        BPos Banana;
        TBEl Cebola;
        Int64 Bliblu;
        List<TLigação> Listona;
        public TesteEstaCristiano()
        {    
            // coloca algumas coisas pra testar - tenha certeza que é igual
            this.Nome = new String(' ' , 0);
            this.Banana = new BPos(2, 3);
            this.Cebola = new TBEl(3, 2);
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
     * O QUE EU VOU FAZER AMANHÃ (SÁBADO:)
     * 1. REVERIFICAR ESSE ESQUEMA DE LINKS E LIGAÇÕES
     * 2. ADICIONAR CLASSES ESPECIAIS QUE VÃO FICAR RESPONSÁVEIS POR CADA UMA DAS LISTAS-MESTRE DE CADA TIPO
     * 3. CADA CLASSE ESPECIAL VAI TER FUNÇÕES DE ADICIONAR NOVOS ELEMENTOS E PROCURAR
     * 4. CADA FUNÇÃO DE ADICIONAR VAI TER CERTEZA QUE ELE NÃO EXISTE AINDA UTILIZANDO FUNÇÕES DE COMPARAÇÕES DE STRING DE VÁRIOS TIPOS (INICIAIS, SOBRENOME, ETC)
     * 5. AS FUNÇÕES DE PROCURAR TAMBÉM VÃO USAR ESSAS FUNÇÕES DE COMPARAÇÃO DE STRINGS
     * 6. ADICIONAR FUNÇÕES PARA TRABALHAR COM TODO O VOLUME DE DADOS, SALVANDO NO DISCO RÍGIDO E RECUPERANDO DEPOIS
     * EU ACHO MELHOR TU DAR UMA OLHADA E ANOTAR O QUE TU QUER MUDAR, MAS DEIXAR TUDO COMO ESTÁ; CONVERSAMOS AMANHÃ
     * SE TU QUISER IR FAZENDO ALGO, FAZ FUNÇÕES QUE SALVE UMA INSTÂNCIA DE "TesteEstaCristiano" PARA UM BINÁRIO E CARREGUE DE VOLTA
         tem que ser essa classe inútil porque ainda tem muito a arrumar pra ter a função pronta, mas ali tem tudo que vamos precisar
     *  BTW, SE TU QUISER COMPILAR PRA VER SE TA TUDO BEM, A COISINHA QUE GIRA É TIPO UM LOADING QUE A GENTE BOTA NUMA THREAD ENQUANTO CARREGA AS COISAS
     */
    class Definitions
    {

    }
}
