using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;

namespace VisualSort
{
    // Constantes utilizadas pelo Programa inteiro
    public static class Constants
    {
        public static int NodosPorBloco = 128;
        public static int BlocoBufferSize = 32;
        public static string blocPessoasFileName = "pess_bloco";
        public static string blocPeriódicosFileName = "peri_bloco";
        public static string blocConferênciasFileName = "conf_bloco";
        public static string blocCapsFileName = "caps_bloco";
        public static string blocLivrosFileName = "livr_bloco";
        public static string blocArtigosFileName = "arti_bloco";
        public static string blocInstituiçõesFileName = "inst_bloco";
        public static string CSVFileNamePeriódicos = "C:\\Users\\João Paulo T Ruschel\\Documents\\Visual Studio 2010\\Projects\\VisualSort\\VisualSort\\Tabelas\\periodicos.csv";
        public static string CSVFileNameConverências = "C:\\Users\\João Paulo T Ruschel\\Documents\\Visual Studio 2010\\Projects\\VisualSort\\VisualSort\\Tabelas\\conferencias.csv";
    }

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

    // CLASSE QUE CARREGA DOS ARQUIVOS CSV DE CONFERENCIAS E PERIÓDICOS
    //   TODAS AS INFORMAÇÔES COMPETENTES AO PROGRAMA
    public static class CSVBIZU
    {
        public static void PeriodicosCSV()
        {
            StreamReader reader = new StreamReader(File.Open(Constants.CSVFileNamePeriódicos, FileMode.Open),Encoding.ASCII);
            reader.ReadLine(); //Lê a linha que contém informações inúteis
            foreach (string s in reader.ReadToEnd().Split('\n'))
            {
                TFPeriódico periódico = new TFPeriódico();
                string[] aux = s.Split(';');
                if (aux.Length >= 3)
                {
                    foreach (string issns in aux[0].Split('-'))
                        periódico.ISSN = periódico.ISSN + issns;
                    periódico.Nome = aux[1];
                    periódico.Qualis = aux[2];
                    //Console.WriteLine(periódico.ToString());
                    int Índice = (int)Program.mPeridódicos.NovoNodo(periódico.Nome);
                    Program.mPeridódicos[Índice].Data = Program.fPeridódicos.AdicionaInformação(periódico);
                }
            }

            reader.Close();
        }

        public static void ConferenciasCSV()
        {
            StreamReader reader = new StreamReader(File.Open(Constants.CSVFileNameConverências, FileMode.Open), Encoding.ASCII);
            reader.ReadLine();//Lê a linha que contém informações inúteis
            foreach (string s in reader.ReadToEnd().Split('\n'))
            {
                TFConferência conferencia= new TFConferência();
                string[] aux = s.Split(';');
                if (aux.Length >= 4)
                {
                    conferencia.Sigla = aux[0];
                    conferencia.Nome = aux[1];
                    conferencia.Caráter = aux[2];
                    conferencia.Qualis = aux[3];
                    //Console.WriteLine(conferencia.ToString());
                    int Índice = (int)Program.mConferências.NovoNodo(conferencia.Nome);
                    Program.mConferências[Índice].Data = Program.fConferências.AdicionaInformação(conferencia);
                }
            }

            reader.Close();
        }

    }

    // CLASSE QUE CARREGA DE UM ARQUIVO XML (NOME DADO POR PARAMETRO) 
    //   TODAS AS INFORMAÇÔES COMPETENTES AO PROGRAMA(PESSOA< ARTIGOS, NOMES, ETC)
    public static class XMLBIZU
    {
        public static void ReadFromXML(string arquivo)
        {
            TFPessoa Pessoa = new TFPessoa();
            int Índice = -1;

            XmlTextReader reader = new XmlTextReader(arquivo);

            while (reader.Read())
            {
                if (reader.Name == "DADOS-GERAIS")
                {
                    Pessoa.NomeCompleto = reader.GetAttribute("NOME-COMPLETO");
                    Console.WriteLine(Pessoa.NomeCompleto);
                    Índice = (int)Program.mPessoas.NovoNodo(Pessoa.NomeCompleto);
                    Pessoa.País = reader.GetAttribute("PAIS-DE-NASCIMENTO");
                }
                else if (reader.Name == "RESUMO-CV")
                {
                    Pessoa.TextoResumo = reader.GetAttribute("TEXTO-RESUMO-CV-RH");
                }
                else if (reader.Name=="ARTIGO-PUBLICADO")
                {
                    TFArtigo artigo = new TFArtigo();
                    int ÍndiceArtigo = -1;
                    List<int> Autores = new List<int>();

                    XmlReader PArtigo = reader.ReadSubtree();
                    while (PArtigo.Read())
                    {
                        Autores.Clear();
                        if (PArtigo.Name == "DADOS-BASICOS-DO-ARTIGO")
                        {
                            artigo.Natureza = PArtigo.GetAttribute("NATUREZA");
                            artigo.Idioma = PArtigo.GetAttribute("IDIOMA");
                            artigo.AnoPublicação = int.Parse(PArtigo.GetAttribute("ANO-DO-ARTIGO"));
                            artigo.Título = PArtigo.GetAttribute("TITULO-DO-ARTIGO");
                            artigo.MeioDivulgação = PArtigo.GetAttribute("MEIO-DE-DIVULGACAO");
                            ÍndiceArtigo = (int)Program.mArtigos.NovoNodo(artigo.Título);
                        }
                        else if (PArtigo.Name == "DETALHAMENTO-DO-ARTIGO")
                        {
                            artigo.ISSN = PArtigo.GetAttribute("ISSN");
                            artigo.Periodico = PArtigo.GetAttribute("TITULO-DO-PERIODICO-OU-REVISTA");
                        }
                        else if (PArtigo.Name == "PALAVRAS-CHAVE")
                        {
                            artigo.PalavrasChave = new List<string>();
                            for (int i = 1; i <= PArtigo.AttributeCount; i++)
                                artigo.PalavrasChave.Add(PArtigo.GetAttribute("PALAVRA-CHAVE-" + i));
                        }
                        else if (PArtigo.Name == "AUTORES")
                        {
                            if(PArtigo.GetAttribute("NOME-COMPLETO-DO-AUTOR")!=Pessoa.NomeCompleto)
                            {
                                string NomeCompleto = PArtigo.GetAttribute("NOME-COMPLETO-DO-AUTOR");
                                int indicep = (int)Program.mPessoas.NovoNodo(NomeCompleto);
                                Program.mArtigos[ÍndiceArtigo].AdicionaLigaçãoCom(new TPNodo(indicep, 0));
                                Autores.Add(indicep);
                            }
                        }
                        if (ÍndiceArtigo > -1)
                        {
                            Program.mArtigos[ÍndiceArtigo].Data = Program.fArtigos.AdicionaInformação(artigo);
                            Program.mPessoas[Índice].AdicionaLigaçãoCom(Program.mArtigos[ÍndiceArtigo].Nodo);
                            foreach (int i in Autores)
                                Program.mPessoas[i].AdicionaLigaçãoCom(Program.mArtigos[ÍndiceArtigo].Nodo);
                        }
                    }
                    Console.WriteLine(artigo.ToString());
                }
                else if (reader.Name == "LIVROS-E-CAPITULOS")
                {
                   XmlReader PLivrosCaps = reader.ReadSubtree();

                   while(PLivrosCaps.Read())
                    {
                        if(PLivrosCaps.Name=="CAPITULO-DE-LIVRO-PUBLICADO")
                        {
                            TFCap cap = new TFCap();
                            int ÍndiceCapítulo = -1;
                            List<int> Autores = new List<int>();
                            XmlReader Pcap = PLivrosCaps.ReadSubtree();

                            Pcap.ReadToFollowing("DADOS-BASICOS-DO-CAPITULO");
                            cap.Idioma = Pcap.GetAttribute("IDIOMA");
                            cap.MeioDivulgação = Pcap.GetAttribute("MEIO-DE-DIVULGACAO");
                            cap.Título = Pcap.GetAttribute("TITULO-DO-CAPITULO-DO-LIVRO");
                            cap.AnoPublicação = int.Parse(Pcap.GetAttribute("ANO"));
                            Pcap.ReadToFollowing("DETALHAMENTO-DO-CAPITULO");
                            cap.ISBN = Pcap.GetAttribute("ISBN");
                            cap.Livro = Pcap.GetAttribute("TITULO-DO-LIVRO");
                            ÍndiceCapítulo = (int)Program.mCapítulos.NovoNodo(cap.Título);

                            while (Pcap.ReadToFollowing("AUTORES"))
                            {
                                string ordem = Pcap.GetAttribute("ORDEM-DE-AUTORIA");

                                string NomeCompleto = Pcap.GetAttribute("NOME-COMPLETO");

                                int indicep = (int)Program.mPessoas.NovoNodo(NomeCompleto);
                                Program.mCapítulos[ÍndiceCapítulo].AdicionaLigaçãoCom(new TPNodo(indicep, 0));
                                Autores.Add(indicep);
                            }
                                Pcap.ReadToFollowing("PALAVRAS-CHAVE");
                                cap.PalavrasChave = new List<string>();
                                for (int i = 1; i <= Pcap.AttributeCount; i++)
                                    cap.PalavrasChave.Add(Pcap.GetAttribute("PALAVRA-CHAVE-" + i));
                                
                            while(Pcap.Read());
                            if (ÍndiceCapítulo > -1)
                            {
                                Program.mCapítulos[ÍndiceCapítulo].Data = Program.fCapítulos.AdicionaInformação(cap);
                                Program.mPessoas[Índice].AdicionaLigaçãoCom(Program.mCapítulos[ÍndiceCapítulo].Nodo);
                                foreach (int i in Autores)
                                    Program.mPessoas[i].AdicionaLigaçãoCom(Program.mCapítulos[ÍndiceCapítulo].Nodo);
                            }
                        }
                    }
                    while (PLivrosCaps.Read());
                }
                if (Índice > -1)
                    Program.mPessoas[Índice].Data = Program.fPessoas.AdicionaInformação(Pessoa);
            }
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

        public override string ToString()
        {
            return "NomeCompleto: "+this.NomeCompleto + "País: "+this.País + "\nTextoResumo:\n  "+this.TextoResumo;
        }

        public void GravaBinário(string arquivo)
        {
            BinaryWriter writer = new BinaryWriter(File.Open(arquivo, FileMode.Append));
            writer.Write(NomeCompleto);
            writer.Write(País);
            writer.Write(TextoResumo);
            writer.Close();
        }

        public void LeBinario(string arquivo)
        {
            BinaryReader reader = new BinaryReader(File.Open(arquivo, FileMode.Open));
            this.NomeCompleto = reader.ReadString();
            this.País = reader.ReadString();
            this.TextoResumo = reader.ReadString();
            reader.Close();
        }
    }
    // Um Periódico
    public struct TFPeriódico
    {
        public string ISSN;       // Códigos de ISSN
        public string Nome;             // Nome do periódico
        public string Qualis;           // Qualis
        public override string ToString()
        {
            return "ISSN :" + this.ISSN + "\nNome :" + this.Nome + "\nQUALIS :" + this.Qualis;
        }
    }
    // Uma conferência
    public struct TFConferência
    {
        public string Sigla, Nome;      // Nome (e Sigla)
        public string Caráter;          // Caráter da conferência - INTERNACIONAL ou NACIONAL
        public string Qualis;           // Qualis

        public override string ToString()
        {
            return "Sigla :" + this.Sigla + "\nNome :" + this.Nome + "\nCaráter :" + this.Caráter + "\nQUALIS :" + this.Qualis;
        }
    }
    //Um Capitulo de Livro
    public struct TFCap
    {
        public string Título;           // Título
        public string ISBN;             // ISBN relativo
        public string Idioma;           // Idioma da Produção
        public int AnoPublicação;       // Ano da Publicação
        public string Natureza;         // Natureza (COMPLETO, ESTENDIDO, RESUMO)
        public string Livro;             // Livro ao qual pertence
        public string MeioDivulgação;
        public List<string> PalavrasChave; //Palavras chave
    }
    //Um Livro
    public struct TFLivro
    {
        public string Título;           // Título
        public string ISBN;             // ISBN relativo
        public string Idioma;           // Idioma da Produção
        public int AnoPublicação;       // Ano da Publicação
        public string MeioDivulgação;
        public List<string> PalavrasChave; //Palavras chave
    }
    // Uma Produção Bibliográfica
    public struct TFArtigo
    {
        public string Título;              // Título
        public string ISSN;             // ISSN relativo
        public string Periodico;        // Nome do periódico
        public string MeioDivulgação;   // Meio de Divulgação da Produção
        public string Idioma;           // Idioma da Produção
        public int AnoPublicação;       // Ano da Publicação
        public string Natureza;         // Natureza (COMPLETO, ESTENDIDO, RESUMO)
        public List<string> PalavrasChave; //Palavras chave

        public override string ToString()
        {
            string aux = "\nTítulo: " + this.Título + "\nISSN :" + this.ISSN + "\nPeriodico :" + this.Periodico + "\nMeio de Divulgação" + this.MeioDivulgação + "\nIdioma :" + this.Idioma + "\nAno de Publicação :" + this.AnoPublicação.ToString() + "\nNatureza :" + this.Natureza + "\nPalavras-Chave :";
            if (this.PalavrasChave != null)
                foreach (string s in this.PalavrasChave)
                    aux += s + ",";
            aux.TrimEnd(' ', ',');
            return aux;
        }
        public void GravaBinário(string arquivo)
        {
            BinaryWriter writer = new BinaryWriter(File.Open(arquivo, FileMode.Append));
            writer.Write(this.Título);
            writer.Write(this.ISSN);
            writer.Write(this.Periodico);
            writer.Write(this.MeioDivulgação);
            writer.Write(this.Idioma);
            writer.Write(this.AnoPublicação);
            writer.Write(this.Natureza);
            writer.Write(this.PalavrasChave.Count);
            for(int i=0;i<this.PalavrasChave.Count;i++)
                writer.Write(this.PalavrasChave[i]);
            writer.Close();
        }

        public void LeBinario(string arquivo)
        {
            BinaryReader reader = new BinaryReader(File.Open(arquivo, FileMode.Open));
            this.Título = reader.ReadString();
            this.ISSN = reader.ReadString();
            this.Periodico = reader.ReadString();
            this.MeioDivulgação=reader.ReadString();
            this.Idioma=reader.ReadString();
            this.AnoPublicação=reader.ReadInt32();
            this.Natureza=reader.ReadString();
            int aux = reader.ReadInt32();
            for (int i = 0; i < aux; i++)
                this.PalavrasChave.Add(reader.ReadString());
            reader.Close();
        }
    }
    // Uma Instituição
    public struct TFInstituição
    {
        public string Nome;             // Nome da instituição
    }

    // Classes que fazem a ponte RAM <-> Disco
    // Blocos das Pessoas
    public class TBlocoPessoasHandler
    {
        private TFPessoa[] BlocoAtual;
        private TFPessoa[][] BlocoBuffer;
        protected int blocoCount;
        protected int uÍndiceNoBloco;

        public TBlocoPessoasHandler()
        {
            BlocoAtual = null;
            BlocoBuffer = null;
            blocoCount = 0;
            uÍndiceNoBloco = 0;
        }
        public void InicializaGravação()
        {
            BlocoAtual = new TFPessoa[Constants.NodosPorBloco];
        }
        public void FinalizaGravação()
        {
            if (uÍndiceNoBloco > 0)
                SalvarBloco(Constants.blocPessoasFileName + blocoCount + ".blc");
            BlocoAtual = null;
            blocoCount++;
        }
        public BPos AdicionaInformação(TFPessoa Informação)
        {
            if (BlocoAtual != null)
            {
                BlocoAtual[uÍndiceNoBloco] = Informação;
                uÍndiceNoBloco++;
                BPos bPosAdicionado = new BPos(blocoCount, uÍndiceNoBloco);
                if (uÍndiceNoBloco >= Constants.NodosPorBloco)
                {
                    SalvarBloco(Constants.blocPessoasFileName + blocoCount + ".blc");
                    blocoCount++;
                    uÍndiceNoBloco = 0;
                    BlocoAtual = null;
                    BlocoAtual = new TFPessoa[Constants.NodosPorBloco];
                }
                return bPosAdicionado;
            }
            return new BPos(-1, -1);
        }
        public TFPessoa RetornaInformação(BPos Posição)
        {
            TFPessoa Pessoa = new TFPessoa();
            // carrega do binário tudo
            return Pessoa;
        }
        private void SalvarBloco(string FileName)
        {
            // salva todo o BlocoAtual no disco
        }
        private void CarregarBuffer()
        {
            BlocoBuffer = new TFPessoa[Constants.BlocoBufferSize][];
            for (int i = 0; i < Constants.BlocoBufferSize; i++)
                BlocoBuffer[i] = new TFPessoa[Constants.NodosPorBloco];
        }
        private void DescarregaBuffer()
        {
            BlocoBuffer = null;
        }
    }
    // Blocos dos Artigos
    public class TBlocoArtigosHandler
    {
        private TFArtigo[] BlocoAtual;
        private TFArtigo[][] BlocoBuffer;
        protected int blocoCount;
        protected int uÍndiceNoBloco;

        public TBlocoArtigosHandler()
        {
            BlocoAtual = null;
            BlocoBuffer = null;
            blocoCount = 0;
            uÍndiceNoBloco = 0;
        }
        public void InicializaGravação()
        {
            BlocoAtual = new TFArtigo[Constants.NodosPorBloco];
        }
        public void FinalizaGravação()
        {
            if (uÍndiceNoBloco > 0)
                SalvarBloco(Constants.blocArtigosFileName + blocoCount + ".blc");
            blocoCount++;
            BlocoAtual = null;
        }
        public BPos AdicionaInformação(TFArtigo Informação)
        {
            BlocoAtual[uÍndiceNoBloco] = Informação;
            uÍndiceNoBloco++;
            BPos bPosAdicionado = new BPos(blocoCount, uÍndiceNoBloco);
            if (uÍndiceNoBloco >= Constants.NodosPorBloco)
            {
                SalvarBloco(Constants.blocArtigosFileName + blocoCount + ".blc");
                blocoCount++;
                uÍndiceNoBloco = 0;
                BlocoAtual = null;
                BlocoAtual = new TFArtigo[Constants.NodosPorBloco];
            }
            return bPosAdicionado;
        }
        public TFArtigo RetornaInformação(BPos Posição)
        {
            TFArtigo Artigo = new TFArtigo();
            // carrega do binário tudo
            return Artigo;
        }
        private void SalvarBloco(string FileName)
        {
            // salva todo o BlocoAtual no disco
        }
        private void CarregarBuffer()
        {
            BlocoBuffer = new TFArtigo[Constants.BlocoBufferSize][];
            for (int i = 0; i < Constants.BlocoBufferSize; i++)
                BlocoBuffer[i] = new TFArtigo[Constants.NodosPorBloco];
        }
        private void DescarregaBuffer()
        {
            BlocoBuffer = null;
        }
    }
    // Blocos dos Livros
    public class TBlocoLivrosHandler
    {
        private TFLivro[] BlocoAtual;
        private TFLivro[][] BlocoBuffer;
        protected int blocoCount;
        protected int uÍndiceNoBloco;

        public TBlocoLivrosHandler()
        {
            BlocoAtual = null;
            BlocoBuffer = null;
            blocoCount = 0;
            uÍndiceNoBloco = 0;
        }
        public void InicializaGravação()
        {
            BlocoAtual = new TFLivro[Constants.NodosPorBloco];
        }
        public void FinalizaGravação()
        {
            if (uÍndiceNoBloco > 0)
                SalvarBloco(Constants.blocLivrosFileName + blocoCount + ".blc");
            blocoCount++;
            BlocoAtual = null;
        }
        public BPos AdicionaInformação(TFLivro Informação)
        {
            BlocoAtual[uÍndiceNoBloco] = Informação;
            uÍndiceNoBloco++;
            BPos bPosAdicionado = new BPos(blocoCount, uÍndiceNoBloco);
            if (uÍndiceNoBloco >= Constants.NodosPorBloco)
            {
                SalvarBloco(Constants.blocLivrosFileName + blocoCount + ".blc");
                blocoCount++;
                uÍndiceNoBloco = 0;
                BlocoAtual = null;
                BlocoAtual = new TFLivro[Constants.NodosPorBloco];
            }
            return bPosAdicionado;
        }
        public TFLivro RetornaInformação(BPos Posição)
        {
            TFLivro Livro = new TFLivro();
            // carrega do binário tudo
            return Livro;
        }
        private void SalvarBloco(string FileName)
        {
            // salva todo o BlocoAtual no disco
        }
        private void CarregarBuffer()
        {
            BlocoBuffer = new TFLivro[Constants.BlocoBufferSize][];
            for (int i = 0; i < Constants.BlocoBufferSize; i++)
                BlocoBuffer[i] = new TFLivro[Constants.NodosPorBloco];
        }
        private void DescarregaBuffer()
        {
            BlocoBuffer = null;
        }
    }
    // Blocos dos Periódicos
    public class TBlocoPeriódicosHandler
    {
        private TFPeriódico[] BlocoAtual;
        private TFPeriódico[][] BlocoBuffer;
        protected int blocoCount;
        protected int uÍndiceNoBloco;

        public TBlocoPeriódicosHandler()
        {
            BlocoAtual = null;
            BlocoBuffer = null;
            blocoCount = 0;
            uÍndiceNoBloco = 0;
        }
        public void InicializaGravação()
        {
            BlocoAtual = new TFPeriódico[Constants.NodosPorBloco];
        }
        public void FinalizaGravação()
        {
            if (uÍndiceNoBloco > 0)
                SalvarBloco(Constants.blocPeriódicosFileName + blocoCount + ".blc");
            blocoCount++;
            BlocoAtual = null;
        }
        public BPos AdicionaInformação(TFPeriódico Informação)
        {
            BlocoAtual[uÍndiceNoBloco] = Informação;
            uÍndiceNoBloco++;
            BPos bPosAdicionado = new BPos(blocoCount, uÍndiceNoBloco);
            if (uÍndiceNoBloco >= Constants.NodosPorBloco)
            {
                SalvarBloco(Constants.blocPeriódicosFileName + blocoCount + ".blc");
                blocoCount++;
                uÍndiceNoBloco = 0;
                BlocoAtual = null;
                BlocoAtual = new TFPeriódico[Constants.NodosPorBloco];
            }
            return bPosAdicionado;
        }
        public TFPeriódico RetornaInformação(BPos Posição)
        {
            TFPeriódico Periódico = new TFPeriódico();
            // carrega do binário tudo
            return Periódico;
        }
        private void SalvarBloco(string FileName)
        {
            // salva todo o BlocoAtual no disco
        }
        private void CarregarBuffer()
        {
            BlocoBuffer = new TFPeriódico[Constants.BlocoBufferSize][];
            for (int i = 0; i < Constants.BlocoBufferSize; i++)
                BlocoBuffer[i] = new TFPeriódico[Constants.NodosPorBloco];
        }
        private void DescarregaBuffer()
        {
            BlocoBuffer = null;
        }
    }
    // Blocos dos Capítulos
    public class TBlocoCapítulosHandler
    {
        private TFCap[] BlocoAtual;
        private TFCap[][] BlocoBuffer;
        protected int blocoCount;
        protected int uÍndiceNoBloco;

        public TBlocoCapítulosHandler()
        {
            BlocoAtual = null;
            BlocoBuffer = null;
            blocoCount = 0;
            uÍndiceNoBloco = 0;
        }
        public void InicializaGravação()
        {
            BlocoAtual = new TFCap[Constants.NodosPorBloco];
        }
        public void FinalizaGravação()
        {
            if (uÍndiceNoBloco > 0)
                SalvarBloco(Constants.blocCapsFileName + blocoCount + ".blc");
            blocoCount++;
            BlocoAtual = null;
        }
        public BPos AdicionaInformação(TFCap Informação)
        {
            BlocoAtual[uÍndiceNoBloco] = Informação;
            uÍndiceNoBloco++;
            BPos bPosAdicionado = new BPos(blocoCount, uÍndiceNoBloco);
            if (uÍndiceNoBloco >= Constants.NodosPorBloco)
            {
                SalvarBloco(Constants.blocCapsFileName + blocoCount + ".blc");
                blocoCount++;
                uÍndiceNoBloco = 0;
                BlocoAtual = null;
                BlocoAtual = new TFCap[Constants.NodosPorBloco];
            }
            return bPosAdicionado;
        }
        public TFCap RetornaInformação(BPos Posição)
        {
            TFCap Capítulo = new TFCap();
            // carrega do binário tudo
            return Capítulo;
        }
        private void SalvarBloco(string FileName)
        {
            // salva todo o BlocoAtual no disco
        }
        private void CarregarBuffer()
        {
            BlocoBuffer = new TFCap[Constants.BlocoBufferSize][];
            for (int i = 0; i < Constants.BlocoBufferSize; i++)
                BlocoBuffer[i] = new TFCap[Constants.NodosPorBloco];
        }
        private void DescarregaBuffer()
        {
            BlocoBuffer = null;
        }
    }
    // Blocos das Conferências
    public class TBlocoConferênciasHandler
    {
        private TFConferência[] BlocoAtual;
        private TFConferência[][] BlocoBuffer;
        protected int blocoCount;
        protected int uÍndiceNoBloco;

        public TBlocoConferênciasHandler()
        {
            BlocoAtual = null;
            BlocoBuffer = null;
            blocoCount = 0;
            uÍndiceNoBloco = 0;
        }
        public void InicializaGravação()
        {
            BlocoAtual = new TFConferência[Constants.NodosPorBloco];
        }
        public void FinalizaGravação()
        {
            if (uÍndiceNoBloco > 0)
                SalvarBloco(Constants.blocConferênciasFileName + blocoCount + ".blc");
            blocoCount++;
            BlocoAtual = null;
        }
        public BPos AdicionaInformação(TFConferência Informação)
        {
            BlocoAtual[uÍndiceNoBloco] = Informação;
            uÍndiceNoBloco++;
            BPos bPosAdicionado = new BPos(blocoCount, uÍndiceNoBloco);
            if (uÍndiceNoBloco >= Constants.NodosPorBloco)
            {
                SalvarBloco(Constants.blocConferênciasFileName + blocoCount + ".blc");
                blocoCount++;
                uÍndiceNoBloco = 0;
                BlocoAtual = null;
                BlocoAtual = new TFConferência[Constants.NodosPorBloco];
            }
            return bPosAdicionado;
        }
        public TFConferência RetornaInformação(BPos Posição)
        {
            TFConferência Conferência = new TFConferência();
            // carrega do binário tudo
            return Conferência;
        }
        private void SalvarBloco(string FileName)
        {
            // salva todo o BlocoAtual no disco
        }
        private void CarregarBuffer()
        {
            BlocoBuffer = new TFConferência[Constants.BlocoBufferSize][];
            for (int i = 0; i < Constants.BlocoBufferSize; i++)
                BlocoBuffer[i] = new TFConferência[Constants.NodosPorBloco];
        }
        private void DescarregaBuffer()
        {
            BlocoBuffer = null;
        }
    }
    // Blocos das Instituições
    public class TBlocoInstituiçõesHandler
    {
        private TFInstituição[] BlocoAtual;
        private TFInstituição[][] BlocoBuffer;
        protected int blocoCount;
        protected int uÍndiceNoBloco;

        public TBlocoInstituiçõesHandler()
        {
            BlocoAtual = null;
            BlocoBuffer = null;
            blocoCount = 0;
            uÍndiceNoBloco = 0;
        }
        public void InicializaGravação()
        {
            BlocoAtual = new TFInstituição[Constants.NodosPorBloco];
        }
        public void FinalizaGravação()
        {
            if (uÍndiceNoBloco > 0)
                SalvarBloco(Constants.blocInstituiçõesFileName + blocoCount + ".blc");
            blocoCount++;
            BlocoAtual = null;
        }
        public BPos AdicionaInformação(TFInstituição Informação)
        {
            BlocoAtual[uÍndiceNoBloco] = Informação;
            uÍndiceNoBloco++;
            BPos bPosAdicionado = new BPos(blocoCount, uÍndiceNoBloco);
            if (uÍndiceNoBloco >= Constants.NodosPorBloco)
            {
                SalvarBloco(Constants.blocInstituiçõesFileName + blocoCount + ".blc");
                blocoCount++;
                uÍndiceNoBloco = 0;
                BlocoAtual = null;
                BlocoAtual = new TFInstituição[Constants.NodosPorBloco];
            }
            return bPosAdicionado;
        }
        public TFInstituição RetornaInformação(BPos Posição)
        {
            TFInstituição Instiuição = new TFInstituição();
            // carrega do binário tudo
            return Instiuição;
        }
        private void SalvarBloco(string FileName)
        {
            // salva todo o BlocoAtual no disco
        }
        private void CarregarBuffer()
        {
            BlocoBuffer = new TFInstituição[Constants.BlocoBufferSize][];
            for (int i = 0; i < Constants.BlocoBufferSize; i++)
                BlocoBuffer[i] = new TFInstituição[Constants.NodosPorBloco];
        }
        private void DescarregaBuffer()
        {
            BlocoBuffer = null;
        }
    }

    /// DEFINIÇÃO DAS CLASSES QUE TERÃO INFORMAÇÕES CRUCIAIS SOMENTE */
    ///   * ESSA SERÁ A BASE DE DADOS QUE O PROGRAMA SE ORGANIZARÁ */
    ///   * ESSES DADOS SERÃO DEIXADOS NA MEMÓRIA E NUNCA SAIRÃO DE LÁ */  

    // Um tipo super-básico de ponteiro para um Nodo - contém somente o índice na lista-mestre respectiva
    public struct TPNodo
    {
        public Int64 Índice;            // Índice na correspondente lista
        public int Tipo;                // Tipo (0= Pessoa; 1= Artigo; 2= Livro; 3= Periódico; 4= Capítulo; 5= Conferência; 6= Instituição)
        
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
    public class TInfoNodo
    {
        public List<TPNodo> Ligações;   // As ligações do elemento com todos os outros
        public string Nome;             // Somente possui palavras suficentemente interessantes para pesquisa rápida
        public string Iniciais;         // Para ainda mais rápida pesquisa
        public BPos Data;               // Posição no disco (bloco e offset) de todos os dados
        public TPNodo Nodo;             // Informação sobre que tipo é e onde está na lista principal
        public TDrawNodo DrawNodo;      // Nodo de desenho (ponteiro)

        // Constructors
        public TInfoNodo(string Nome, BPos Data) : base() 
        {
            /*foreach (string substr in Nome.Split(' '))
            {
                if (substr.Count() >= 3)
                    this.Nome = String.Concat(this.Nome, substr, " ");
                this.Iniciais = String.Concat(this.Iniciais, substr.Substring(0, 1));
            }
            if ((this.Nome == null) || (this.Nome.Length == 0))*/
                this.Nome = Nome;
            this.Data = Data;
            this.Nodo = new TPNodo(-1, -1);
            this.Ligações = new List<TPNodo>();
            DrawNodo = null;
        }

        public TInfoNodo()
        {
            this.Data.Bloco = -1;
            this.Data.Offset = -1;
            DrawNodo = null;
        }
        // Função que verifica se o nome de entrada é o mesmo (ou seja, se todas as palavras de Value estão em Nome)
        public bool MesmoNome(string Value, bool ForceEqual)
        {
            if (Nome == null)
                return false;
            // TODO - TERMINAR
            bool Cont = true;
            int sCount = 0;
            if (Value != null)
            {
                if (!ForceEqual)
                {
                    foreach (string substr in Value.Split(' '))
                    {
                        if (!Nome.Contains(substr))
                        {
                            Cont = false;
                            sCount++;
                        }
                        //if (substr.Length >= 1)
                        //    if (!Iniciais.Contains(substr.Substring(0, 1)))
                        //        Cont = false;
                    }
                }
                else
                    if (Value == Nome)
                        Cont = true;
                    else
                        Cont = false;
                return Cont;
                //return (Value == this.Nome);
            }
            return false;
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
                    // Se não havia, cria
                    Ligações.Add(Nodo);
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
        // Cria um novo elemento dado somente um nome
        public int NovoNodo(string Nome)
        {
            List<TInfoNodo> Lista = ProcuraNodo(Nome, true);
            if (Lista.Count == 0)
            {
                Add(new TInfoNodo(Nome, new BPos(-1, -1)));
                this[this.Count - 1].Nodo = new TPNodo(this.Count - 1, Tipo);
                return this.Count - 1;
            }
            else
            {
                return (int)Lista[0].Nodo.Índice;
            }
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
        public List<TInfoNodo> ProcuraNodo(BPos Data)
        {
            return this.FindAll(p => (p.Data == Data));
        }
        public List<TInfoNodo> ProcuraNodo(Predicate<TInfoNodo> match)
        {
            return this.FindAll(match);
        }
    }
}
