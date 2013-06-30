using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

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
        public static string CSVFileNamePeriódicos = "periodicos.csv";
        public static string CSVFileNameConverências = "conferencias.csv";
        public static int MaxSelectedNodeSizeAlwaysText = 100;
        public static int MaxSelectedNodeLinesAlwaysText = 3;
        public static string DiretorioRaiz = "tabelas\\";
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

        public void GravaBPos(BinaryWriter writer)
        {
            writer.Write(this.Bloco);
            writer.Write(this.Offset);
        }
        public static BPos LeBPos(BinaryReader reader)
        {
            BPos aux = new BPos();
            aux.Bloco = reader.ReadInt64();
            aux.Offset = reader.ReadInt64();
            return aux;
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
            StreamReader reader = new StreamReader(File.Open(Constants.DiretorioRaiz+Constants.CSVFileNamePeriódicos, FileMode.Open),Encoding.GetEncoding("iso-8859-1"));
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
                    int Índice = (int)Program.mPeriódicos.NovoNodo(periódico.Nome);
                    Program.mPeriódicos[Índice].Data = Program.fPeridódicos.AdicionaInformação(periódico);
                }
            }

            reader.Close();
        }

        public static void ConferenciasCSV()
        {
            StreamReader reader = new StreamReader(File.Open(Constants.DiretorioRaiz+Constants.CSVFileNameConverências, FileMode.Open), Encoding.GetEncoding("iso-8859-1"));
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

            //XmlTextReader reader = new XmlTextReader(arquivo);
          //  reader. = Encoding.UTF32; //ISO-8859-1
            XmlReader reader = XmlReader.Create(new StreamReader(arquivo, Encoding.GetEncoding("UTF-8")));
            while (reader.Read())
            {
                if (reader.Name == "DADOS-GERAIS" && reader.AttributeCount > 0)
                {
                    Pessoa.NomeCompleto = reader.GetAttribute("NOME-COMPLETO");
                    Console.WriteLine(Pessoa.NomeCompleto);
                    Índice = (int)Program.mPessoas.NovoNodo(Pessoa.NomeCompleto);
                    Pessoa.País = reader.GetAttribute("PAIS-DE-NASCIMENTO");
                }
                else if (reader.Name == "RESUMO-CV" && reader.AttributeCount > 0)
                {
                    Pessoa.TextoResumo = reader.GetAttribute("TEXTO-RESUMO-CV-RH");
                }
                else if (reader.Name=="ARTIGO-PUBLICADO")
                {
                    TFArtigo artigo = new TFArtigo();
                    int ÍndiceArtigo = -1;
                    artigo.Tipo = 0;
                    List<int> Autores = new List<int>();
                    List<int> Periódicos = new List<int>();
                    artigo.PalavrasChave = new List<string>();
                    XmlReader PArtigo = reader.ReadSubtree();
                    while (PArtigo.Read())
                    {
                        if (PArtigo.Name == "DADOS-BASICOS-DO-ARTIGO" && reader.AttributeCount > 0)
                        {
                            artigo.Natureza = PArtigo.GetAttribute("NATUREZA");
                            artigo.Idioma = PArtigo.GetAttribute("IDIOMA");
                            artigo.AnoPublicação = int.Parse(PArtigo.GetAttribute("ANO-DO-ARTIGO"));
                            artigo.Título = PArtigo.GetAttribute("TITULO-DO-ARTIGO");
                            artigo.MeioDivulgação = PArtigo.GetAttribute("MEIO-DE-DIVULGACAO");
                            ÍndiceArtigo = (int)Program.mArtigos.NovoNodo(artigo.Título);
                        }
                        else if (PArtigo.Name == "DETALHAMENTO-DO-ARTIGO" && reader.AttributeCount > 0)
                        {
                            artigo.ISSN = PArtigo.GetAttribute("ISSN");
                            artigo.PeriodicoOuConferencia = PArtigo.GetAttribute("TITULO-DO-PERIODICO-OU-REVISTA");
                            int indiceper = (int)Program.mPeriódicos.NovoNodo(artigo.PeriodicoOuConferencia,artigo.ISSN);
                            Program.mArtigos[ÍndiceArtigo].AdicionaLigaçãoCom(new TPNodo(indiceper, 5));
                            Periódicos.Add(indiceper);
                            
                        }
                        else if (PArtigo.Name == "PALAVRAS-CHAVE" && reader.AttributeCount>0)
                        {
                            artigo.PalavrasChave = new List<string>();
                            for (int i = 1; i <= PArtigo.AttributeCount; i++)
                                artigo.PalavrasChave.Add(PArtigo.GetAttribute("PALAVRA-CHAVE-" + i));
                        }
                        else if (PArtigo.Name == "AUTORES" && reader.AttributeCount > 0)
                        {
                            if(Pessoa.NomeCompleto!=string.Empty)
                            {
                                string NomeCompleto = PArtigo.GetAttribute("NOME-COMPLETO-DO-AUTOR");
                                int indicep = (int)Program.mPessoas.NovoNodo(NomeCompleto);
                                Program.mArtigos[ÍndiceArtigo].AdicionaLigaçãoCom(new TPNodo(indicep, 0));
                                Autores.Add(indicep);
                            }
                        } 
                    }
                    if (ÍndiceArtigo > -1)
                    {
                        Program.mArtigos[ÍndiceArtigo].Data = Program.fArtigos.AdicionaInformação(artigo);
                        Program.mPessoas[Índice].AdicionaLigaçãoCom(Program.mArtigos[ÍndiceArtigo].Nodo);
                        Program.mArtigos[ÍndiceArtigo].AdicionaLigaçãoCom(Program.mPessoas[Índice].Nodo);
                        foreach (int i in Autores)
                            Program.mPessoas[i].AdicionaLigaçãoCom(Program.mArtigos[ÍndiceArtigo].Nodo);
                        foreach (int i in Periódicos)
                            Program.mPeriódicos[i].AdicionaLigaçãoCom(Program.mArtigos[ÍndiceArtigo].Nodo);
                    }
                }
                else if (reader.Name == "LIVROS-E-CAPITULOS")
                {
                   XmlReader PLivrosCaps = reader.ReadSubtree();

                   while(PLivrosCaps.Read())
                    {
                        if (PLivrosCaps.Name == "CAPITULO-DE-LIVRO-PUBLICADO" && reader.AttributeCount > 0)
                        {
                            TFCap cap = new TFCap();
                            int ÍndiceCapítulo = -1;
                            List<int> Autores = new List<int>();
                            cap.PalavrasChave = new List<string>();
                            XmlReader Pcap = PLivrosCaps.ReadSubtree();
                            cap.PalavrasChave = new List<string>();
                            Pcap.ReadToFollowing("DADOS-BASICOS-DO-CAPITULO");
                            cap.Idioma = Pcap.GetAttribute("IDIOMA");
                            cap.MeioDivulgação = Pcap.GetAttribute("MEIO-DE-DIVULGACAO");
                            cap.Título = Pcap.GetAttribute("TITULO-DO-CAPITULO-DO-LIVRO");
                            cap.AnoPublicação = int.Parse(Pcap.GetAttribute("ANO"));
                            Pcap.ReadToFollowing("DETALHAMENTO-DO-CAPITULO");
                            cap.ISBN = Pcap.GetAttribute("ISBN");
                            cap.Livro = Pcap.GetAttribute("TITULO-DO-LIVRO");
                            ÍndiceCapítulo = (int)Program.mCapítulos.NovoNodo(cap.Título);

                            while (Pcap.ReadToFollowing("AUTORES") && reader.AttributeCount > 0)
                            {
                                string ordem = Pcap.GetAttribute("ORDEM-DE-AUTORIA");

                                string NomeCompleto = Pcap.GetAttribute("NOME-COMPLETO-DO-AUTOR");
                                if (NomeCompleto != null && NomeCompleto != string.Empty)
                                {
                                    int indicep = (int)Program.mPessoas.NovoNodo(NomeCompleto);
                                    Program.mCapítulos[ÍndiceCapítulo].AdicionaLigaçãoCom(new TPNodo(indicep, 0));
                                    Autores.Add(indicep);
                                }
                            }
                                Pcap.ReadToFollowing("PALAVRAS-CHAVE");
                                if (Pcap.AttributeCount > 0)
                                {
                                    for (int i = 1; i <= Pcap.AttributeCount; i++)
                                        cap.PalavrasChave.Add(Pcap.GetAttribute("PALAVRA-CHAVE-" + i));
                                }
                            while (Pcap.Read()) ;
                           if (ÍndiceCapítulo > -1)
                            {
                                Program.mCapítulos[ÍndiceCapítulo].Data = Program.fCapítulos.AdicionaInformação(cap);
                                Program.mPessoas[Índice].AdicionaLigaçãoCom(Program.mCapítulos[ÍndiceCapítulo].Nodo);
                                foreach (int i in Autores)
                                    if (i != Índice)
                                        Program.mPessoas[i].AdicionaLigaçãoCom(Program.mCapítulos[ÍndiceCapítulo].Nodo);
                            }
                        }

                    }
                    while (PLivrosCaps.Read());
                }
                else if (reader.Name == "TRABALHO-EM-EVENTOS")
                {
                        TFArtigo artigo = new TFArtigo();
                        int ÍndiceTrab = -1;
                        artigo.Tipo = 1;
                        List<int> Autorest = new List<int>();
                        List<int> Conferencias = new List<int>();
                        artigo.PalavrasChave = new List<string>();
                        XmlReader PTrab = reader.ReadSubtree();
                        while (PTrab.Read())
                        {
                            if (PTrab.Name == "DADOS-BASICOS-DO-TRABALHO" && reader.AttributeCount > 0)
                            {
                                artigo.Natureza = PTrab.GetAttribute("NATUREZA");
                                artigo.Idioma = PTrab.GetAttribute("IDIOMA");
                                artigo.AnoPublicação = int.Parse(PTrab.GetAttribute("ANO-DO-TRABALHO"));
                                artigo.Título = PTrab.GetAttribute("TITULO-DO-TRABALHO");
                                artigo.MeioDivulgação = PTrab.GetAttribute("MEIO-DE-DIVULGACAO");
                                ÍndiceTrab = (int)Program.mArtigos.NovoNodo(artigo.Título);
                            }
                            else if (PTrab.Name == "DETALHAMENTO-DO-TRABALHO" && reader.AttributeCount > 0)
                            {
                                artigo.ISSN = "i";
                                artigo.PeriodicoOuConferencia = PTrab.GetAttribute("NOME-DO-EVENTO");
                                int indicec = (int)Program.mConferências.NovoNodo(artigo.PeriodicoOuConferencia);
                                Program.mArtigos[ÍndiceTrab].AdicionaLigaçãoCom(new TPNodo(indicec, 5));
                                Conferencias.Add(indicec);
                            }
                            else if (PTrab.Name == "PALAVRAS-CHAVE" && reader.AttributeCount > 0)
                            {
                                artigo.PalavrasChave = new List<string>();
                                for (int i = 1; i <= PTrab.AttributeCount; i++)
                                    artigo.PalavrasChave.Add(PTrab.GetAttribute("PALAVRA-CHAVE-" + i));
                            }
                            else if (PTrab.Name == "AUTORES" && reader.AttributeCount > 0)
                            {
                                if (Pessoa.NomeCompleto != string.Empty)
                                {
                                    string NomeCompleto = PTrab.GetAttribute("NOME-COMPLETO-DO-AUTOR");
                                    int indicep = (int)Program.mPessoas.NovoNodo(NomeCompleto);
                                    Program.mArtigos[ÍndiceTrab].AdicionaLigaçãoCom(new TPNodo(indicep, 0));
                                    Autorest.Add(indicep);
                                }
                            }
                    }
                        if (ÍndiceTrab > -1)
                        {
                            Program.mArtigos[ÍndiceTrab].Data = Program.fArtigos.AdicionaInformação(artigo);
                            Program.mPessoas[Índice].AdicionaLigaçãoCom(Program.mArtigos[ÍndiceTrab].Nodo);
                            foreach (int i in Autorest)
                                Program.mPessoas[i].AdicionaLigaçãoCom(Program.mArtigos[ÍndiceTrab].Nodo);
                            foreach (int i in Conferencias)
                                Program.mConferências[i].AdicionaLigaçãoCom(Program.mArtigos[ÍndiceTrab].Nodo);
                        }
                }
            }
            if (Índice > -1)
                Program.mPessoas[Índice].Data = Program.fPessoas.AdicionaInformação(Pessoa);
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
            return "NomeCompleto: " + this.NomeCompleto + "País: " + this.País + "\nTextoResumo:\n  " + this.TextoResumo;
        }

        public static void GravaBinário(BinaryWriter writer, TFPessoa pessoa)
        {
            writer.Write(pessoa.NomeCompleto);
            writer.Write(pessoa.País);
            writer.Write(pessoa.TextoResumo);
        }

        public static TFPessoa LeBinario(BinaryReader reader)
        {
            TFPessoa aux = new TFPessoa();
            aux.NomeCompleto = reader.ReadString();
            aux.País = reader.ReadString();
            aux.TextoResumo = reader.ReadString();
            return aux;
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

        public static void GravaBinário(BinaryWriter writer, TFPeriódico periódico)
        {
            writer.Write(periódico.ISSN);
            writer.Write(periódico.Nome);
            writer.Write(periódico.Qualis);
        }

        public static TFPeriódico LeBinario(BinaryReader reader)
        {
            TFPeriódico aux = new TFPeriódico();
            aux.ISSN = reader.ReadString();
            aux.Nome = reader.ReadString();
            aux.Qualis = reader.ReadString();
            return aux;
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

        public static void GravaBinário(BinaryWriter writer, TFConferência conferencia)
        {
            writer.Write(conferencia.Sigla);
            writer.Write(conferencia.Nome);
            writer.Write(conferencia.Caráter);
            writer.Write(conferencia.Qualis);
        }

        public static TFConferência LeBinario(BinaryReader reader)
        {
            TFConferência aux = new TFConferência();
            aux.Sigla = reader.ReadString();
            aux.Nome = reader.ReadString();
            aux.Caráter = reader.ReadString();
            aux.Qualis = reader.ReadString();
            return aux;
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

        public static void GravaBinário(BinaryWriter writer, TFCap cap)
        {
            writer.Write(cap.Título);
            writer.Write(cap.ISBN);
            writer.Write(cap.Idioma);
            writer.Write(cap.AnoPublicação);
            if (cap.Natureza == null)
                writer.Write("");
            else
                writer.Write(cap.Natureza);
            writer.Write(cap.Livro);
            writer.Write(cap.MeioDivulgação);
            writer.Write(cap.PalavrasChave.Count);
            for (int i = 0; i < cap.PalavrasChave.Count; i++)
                writer.Write(cap.PalavrasChave[i]);
        }

        public static TFCap LeBinario(BinaryReader reader)
        {
            TFCap cap = new TFCap();
            cap.Título = reader.ReadString();
            cap.ISBN = reader.ReadString();
            cap.Idioma = reader.ReadString();
            cap.AnoPublicação = reader.ReadInt32();
            cap.Natureza = reader.ReadString();
            cap.Livro = reader.ReadString();
            cap.MeioDivulgação = reader.ReadString();
            int aux = reader.ReadInt32();
            for (int i = 0; i < aux; i++)
                cap.PalavrasChave.Add(reader.ReadString());
            return cap;
        }
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

        public static void GravaBinário(BinaryWriter writer, TFLivro livro)
        {
            writer.Write(livro.Título);
            writer.Write(livro.ISBN);
            writer.Write(livro.Idioma);
            writer.Write(livro.AnoPublicação);
            writer.Write(livro.MeioDivulgação);
            writer.Write(livro.PalavrasChave.Count);
            for (int i = 0; i < livro.PalavrasChave.Count; i++)
                writer.Write(livro.PalavrasChave[i]);
        }

        public static TFLivro LeBinario(BinaryReader reader)
        {
            TFLivro livro = new TFLivro();
            livro.Título = reader.ReadString();
            livro.ISBN = reader.ReadString();
            livro.Idioma = reader.ReadString();
            livro.AnoPublicação = reader.ReadInt32();
            livro.MeioDivulgação = reader.ReadString();
            int aux = reader.ReadInt32();
            for (int i = 0; i < aux; i++)
                livro.PalavrasChave.Add(reader.ReadString());
            return livro;
        }
    }
    // Uma Produção Bibliográfica
    public struct TFArtigo
    {
        public byte Tipo;                  //1 = Periodico; 2 = Conferencia   
        public string Título;              // Título
        public string ISSN;             // ISSN relativo
        public string PeriodicoOuConferencia;        // Nome do periódico ou conferencia
        public string MeioDivulgação;   // Meio de Divulgação da Produção
        public string Idioma;           // Idioma da Produção
        public int AnoPublicação;       // Ano da Publicação
        public string Natureza;         // Natureza (COMPLETO, ESTENDIDO, RESUMO)
        public List<string> PalavrasChave; //Palavras chave

        public override string ToString()
        {
            string aux;
            if (Tipo == 0)
                aux = "Tipo : Periódico";
            else
                aux = "Tipo : Conferencia";
            aux += "\nTítulo: " + this.Título + "\nISSN :" + this.ISSN + "\nPeriodico :" + this.PeriodicoOuConferencia + "\nMeio de Divulgação" + this.MeioDivulgação + "\nIdioma :" + this.Idioma + "\nAno de Publicação :" + this.AnoPublicação.ToString() + "\nNatureza :" + this.Natureza + "\nPalavras-Chave :";
            if (this.PalavrasChave != null)
            {
                foreach (string s in this.PalavrasChave)
                    aux += s + ",";
            }
            return aux;
        }

        public static void GravaBinário(BinaryWriter writer, TFArtigo artigo)
        {
            writer.Write(artigo.Tipo);
            writer.Write(artigo.Título);
            writer.Write(artigo.ISSN);
            writer.Write(artigo.PeriodicoOuConferencia);
            writer.Write(artigo.MeioDivulgação);
            writer.Write(artigo.Idioma);
            writer.Write(artigo.AnoPublicação);
            writer.Write(artigo.Natureza);
            writer.Write(artigo.PalavrasChave.Count);
            for (int i = 0; i < artigo.PalavrasChave.Count; i++)
                writer.Write(artigo.PalavrasChave[i]);
        }

        public static TFArtigo LeBinario(BinaryReader reader)
        {
            TFArtigo artigo = new TFArtigo();
            artigo.Tipo = reader.ReadByte();
            artigo.Título = reader.ReadString();
            artigo.ISSN = reader.ReadString();
            artigo.PeriodicoOuConferencia = reader.ReadString();
            artigo.MeioDivulgação = reader.ReadString();
            artigo.Idioma = reader.ReadString();
            artigo.AnoPublicação = reader.ReadInt32();
            artigo.Natureza = reader.ReadString();
            int aux = reader.ReadInt32();
            for (int i = 0; i < aux; i++)
                artigo.PalavrasChave.Add(reader.ReadString());
            return artigo;
        }
    }

    // Classes que fazem a ponte RAM <-> Disco
    // Blocos das Pessoas
    public class TBlocoPessoasHandler
    {
        private TFPessoa[] BlocoAtual;
        protected int blocoCount;
        protected int uÍndiceNoBloco;

        public TBlocoPessoasHandler()
        {
            BlocoAtual = null;
            blocoCount = 0;
            uÍndiceNoBloco = 0;
        }

        public static TBlocoPessoasHandler LeIndices(string arquivo)
        {
            BinaryReader reader = new BinaryReader(File.Open(arquivo, FileMode.Open));
            TBlocoPessoasHandler handler = new TBlocoPessoasHandler(); 
            handler.blocoCount = reader.ReadInt32();
            handler.uÍndiceNoBloco = reader.ReadInt32();
            reader.Close();
            return handler;
        }

        public void GravaIndices(string arquivo)
        {
            BinaryWriter writer = new BinaryWriter(File.Open(arquivo, FileMode.Create));
            writer.Write(this.blocoCount);
            writer.Write(this.uÍndiceNoBloco);
            writer.Close();
        }

        public void InicializaGravação()
        {
            BlocoAtual = new TFPessoa[Constants.NodosPorBloco];
        }
        public void FinalizaGravação()
        {
            if (uÍndiceNoBloco > 0)
                SalvarBloco(Constants.DiretorioRaiz+Constants.blocPessoasFileName + blocoCount + ".blc");
            BlocoAtual = null;
            blocoCount++;
        }
        public BPos AdicionaInformação(TFPessoa Informação)
        {
            if (BlocoAtual != null)
            {
                BlocoAtual[uÍndiceNoBloco] = Informação;
                BPos bPosAdicionado = new BPos(blocoCount, uÍndiceNoBloco);
                uÍndiceNoBloco++;
                if (uÍndiceNoBloco >= Constants.NodosPorBloco)
                {
                    SalvarBloco(Constants.DiretorioRaiz+Constants.blocPessoasFileName + blocoCount + ".blc");
                    blocoCount++;
                    uÍndiceNoBloco = 0;
                    BlocoAtual = null;
                    BlocoAtual = new TFPessoa[Constants.NodosPorBloco];
                }
                return bPosAdicionado;
            }
            return new BPos(-1, -1);
        }

        public TFPessoa GetPessoa(BPos pos) //pode jogar excessão
        {
            TFPessoa aux = new TFPessoa();
            int i;

            BinaryReader reader = new BinaryReader(File.Open(Constants.DiretorioRaiz+Constants.blocPessoasFileName + pos.Bloco.ToString() + ".blc", FileMode.Open));
            for (i = 0; i < pos.Offset; i++)
                TFPessoa.LeBinario(reader);
            aux = TFPessoa.LeBinario(reader);
            reader.Close();

            return aux;
        }

        protected void SalvarBloco(string FileName)
        {
            BinaryWriter writer = new BinaryWriter(File.Open(FileName, FileMode.OpenOrCreate));
            for (int i = 0; i < uÍndiceNoBloco; i++)
            {
                TFPessoa.GravaBinário(writer, BlocoAtual[i]);
            }
            writer.Close();
            // salva todo o BlocoAtual no disco
        }
    }
    // Blocos dos Artigos
    public class TBlocoArtigosHandler
    {
        private TFArtigo[] BlocoAtual;
        protected int blocoCount;
        protected int uÍndiceNoBloco;

        public TBlocoArtigosHandler()
        {
            BlocoAtual = null;
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
                SalvarBloco(Constants.DiretorioRaiz+Constants.blocArtigosFileName + blocoCount + ".blc");
            BlocoAtual = null;
            blocoCount++;
        }

        public static TBlocoArtigosHandler LeIndices(string arquivo)
        {
            BinaryReader reader = new BinaryReader(File.Open(arquivo, FileMode.Open));
            TBlocoArtigosHandler handler = new TBlocoArtigosHandler();
            handler.blocoCount = reader.ReadInt32();
            handler.uÍndiceNoBloco = reader.ReadInt32();
            reader.Close();
            return handler;
        }

        public void GravaIndices(string arquivo)
        {
            BinaryWriter writer = new BinaryWriter(File.Open(arquivo, FileMode.Create));
            writer.Write(this.blocoCount);
            writer.Write(this.uÍndiceNoBloco);
            writer.Close();
        }

        public BPos AdicionaInformação(TFArtigo Informação)
        {
            if (BlocoAtual != null)
            {
                BlocoAtual[uÍndiceNoBloco] = Informação;
                BPos bPosAdicionado = new BPos(blocoCount, uÍndiceNoBloco);
                uÍndiceNoBloco++;
                if (uÍndiceNoBloco >= Constants.NodosPorBloco)
                {
                    SalvarBloco(Constants.DiretorioRaiz+Constants.blocArtigosFileName + blocoCount + ".blc");
                    blocoCount++;
                    uÍndiceNoBloco = 0;
                    BlocoAtual = null;
                    BlocoAtual = new TFArtigo[Constants.NodosPorBloco];
                }
                return bPosAdicionado;
            }
            return new BPos(-1, -1);
        }

        public TFArtigo GetArtigo(BPos pos) //pode jogar excessão
        {
            TFArtigo aux = new TFArtigo();
            int i;

            BinaryReader reader = new BinaryReader(File.Open(Constants.DiretorioRaiz + Constants.blocArtigosFileName + pos.Bloco.ToString() + ".blc", FileMode.Open));
            for (i = 0; i < pos.Offset; i++)
                TFArtigo.LeBinario(reader);
            aux = TFArtigo.LeBinario(reader);
            reader.Close();

            return aux;
        }

        protected void SalvarBloco(string FileName)
        {
            BinaryWriter writer = new BinaryWriter(File.Open(FileName, FileMode.OpenOrCreate));
            for (int i = 0; i < uÍndiceNoBloco; i++)
            {
                TFArtigo.GravaBinário(writer, BlocoAtual[i]);
            }
            writer.Close();
            // salva todo o BlocoAtual no disco
        }

    }
    // Blocos dos Livros
    public class TBlocoLivrosHandler
    {
        private TFLivro[] BlocoAtual;
        protected int blocoCount;
        protected int uÍndiceNoBloco;

        public TBlocoLivrosHandler()
        {
            BlocoAtual = null;
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
                SalvarBloco(Constants.DiretorioRaiz+Constants.blocLivrosFileName + blocoCount + ".blc");
            BlocoAtual = null;
            blocoCount++;
        }

        public static TBlocoLivrosHandler LeIndices(string arquivo)
        {
            BinaryReader reader = new BinaryReader(File.Open(arquivo, FileMode.Open));
            TBlocoLivrosHandler handler = new TBlocoLivrosHandler();
            handler.blocoCount = reader.ReadInt32();
            handler.uÍndiceNoBloco = reader.ReadInt32();
            reader.Close();
            return handler;
        }

        public void GravaIndices(string arquivo)
        {
            BinaryWriter writer = new BinaryWriter(File.Open(arquivo, FileMode.Create));
            writer.Write(this.blocoCount);
            writer.Write(this.uÍndiceNoBloco);
            writer.Close();
        }

        public BPos AdicionaInformação(TFLivro Informação)
        {
            if (BlocoAtual != null)
            {
                BlocoAtual[uÍndiceNoBloco] = Informação;
                BPos bPosAdicionado = new BPos(blocoCount, uÍndiceNoBloco);
                uÍndiceNoBloco++;
                if (uÍndiceNoBloco >= Constants.NodosPorBloco)
                {
                    SalvarBloco(Constants.DiretorioRaiz+Constants.blocLivrosFileName + blocoCount + ".blc");
                    blocoCount++;
                    uÍndiceNoBloco = 0;
                    BlocoAtual = null;
                    BlocoAtual = new TFLivro[Constants.NodosPorBloco];
                }
                return bPosAdicionado;
            }
            return new BPos(-1, -1);
        }

        public TFLivro GetLivro(BPos pos) //pode jogar excessão
        {
            TFLivro aux = new TFLivro();
            int i;

            BinaryReader reader = new BinaryReader(File.Open(Constants.DiretorioRaiz + Constants.blocLivrosFileName + pos.Bloco.ToString() + ".blc", FileMode.Open));
            for (i = 0; i < pos.Offset; i++)
                TFLivro.LeBinario(reader);
            aux = TFLivro.LeBinario(reader);
            reader.Close();

            return aux;
        }

        protected void SalvarBloco(string FileName)
        {
            BinaryWriter writer = new BinaryWriter(File.Open(FileName, FileMode.OpenOrCreate));
            for (int i = 0; i < uÍndiceNoBloco; i++)
            {
                TFLivro.GravaBinário(writer, BlocoAtual[i]);
            }
            writer.Close();
            // salva todo o BlocoAtual no disco
        }
    }
    // Blocos dos Periódicos
    public class TBlocoPeriódicosHandler
    {
       private TFPeriódico[] BlocoAtual;
        protected int blocoCount;
        protected int uÍndiceNoBloco;

        public TBlocoPeriódicosHandler()
        {
            BlocoAtual = null;
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
                SalvarBloco(Constants.DiretorioRaiz+Constants.blocPeriódicosFileName + blocoCount + ".blc");
            BlocoAtual = null;
            blocoCount++;
        }

        public static TBlocoPeriódicosHandler LeIndices(string arquivo)
        {
            BinaryReader reader = new BinaryReader(File.Open(arquivo, FileMode.Open));
            TBlocoPeriódicosHandler handler = new TBlocoPeriódicosHandler();
            handler.blocoCount = reader.ReadInt32();
            handler.uÍndiceNoBloco = reader.ReadInt32();
            reader.Close();
            return handler;
        }

        public void GravaIndices(string arquivo)
        {
            BinaryWriter writer = new BinaryWriter(File.Open(arquivo, FileMode.Create));
            writer.Write(this.blocoCount);
            writer.Write(this.uÍndiceNoBloco);
            writer.Close();
        }

        public BPos AdicionaInformação(TFPeriódico Informação)
        {
            if (BlocoAtual != null)
            {
                BlocoAtual[uÍndiceNoBloco] = Informação;
                BPos bPosAdicionado = new BPos(blocoCount, uÍndiceNoBloco);
                uÍndiceNoBloco++;
                if (uÍndiceNoBloco >= Constants.NodosPorBloco)
                {
                    SalvarBloco(Constants.DiretorioRaiz+Constants.blocPeriódicosFileName + blocoCount + ".blc");
                    blocoCount++;
                    uÍndiceNoBloco = 0;
                    BlocoAtual = null;
                    BlocoAtual = new TFPeriódico[Constants.NodosPorBloco];
                }
                return bPosAdicionado;
            }
            return new BPos(-1, -1);
        }

        public TFPeriódico GetPeriódico(BPos pos) //pode jogar excessão
        {
            TFPeriódico aux = new TFPeriódico();
            int i;

            BinaryReader reader = new BinaryReader(File.Open(Constants.DiretorioRaiz + Constants.blocPeriódicosFileName + pos.Bloco.ToString() + ".blc", FileMode.Open));
            for (i = 0; i < pos.Offset; i++)
                TFPeriódico.LeBinario(reader);
            aux = TFPeriódico.LeBinario(reader);
            reader.Close();

            return aux;
        }

        protected void SalvarBloco(string FileName)
        {
            BinaryWriter writer = new BinaryWriter(File.Open(FileName, FileMode.OpenOrCreate));
            for (int i = 0; i < uÍndiceNoBloco; i++)
            {
                TFPeriódico.GravaBinário(writer, BlocoAtual[i]);
            }
            writer.Close();
            // salva todo o BlocoAtual no disco
        }

    }
    // Blocos dos Capítulos
    public class TBlocoCapítulosHandler
    {
       private TFCap[] BlocoAtual;
        protected int blocoCount;
        protected int uÍndiceNoBloco;

        public TBlocoCapítulosHandler()
        {
            BlocoAtual = null;
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
                SalvarBloco(Constants.DiretorioRaiz+Constants.blocCapsFileName + blocoCount + ".blc");
            BlocoAtual = null;
            blocoCount++;
        }

        public static TBlocoCapítulosHandler LeIndices(string arquivo)
        {
            BinaryReader reader = new BinaryReader(File.Open(arquivo, FileMode.Open));
            TBlocoCapítulosHandler handler = new TBlocoCapítulosHandler();
            handler.blocoCount = reader.ReadInt32();
            handler.uÍndiceNoBloco = reader.ReadInt32();
            reader.Close();
            return handler;
        }

        public void GravaIndices(string arquivo)
        {
            BinaryWriter writer = new BinaryWriter(File.Open(arquivo, FileMode.Create));
            writer.Write(this.blocoCount);
            writer.Write(this.uÍndiceNoBloco);
            writer.Close();
        }

        public BPos AdicionaInformação(TFCap Informação)
        {
            if (BlocoAtual != null)
            {
                BlocoAtual[uÍndiceNoBloco] = Informação;
                BPos bPosAdicionado = new BPos(blocoCount, uÍndiceNoBloco);
                uÍndiceNoBloco++;
                if (uÍndiceNoBloco >= Constants.NodosPorBloco)
                {
                    SalvarBloco(Constants.DiretorioRaiz+Constants.blocCapsFileName + blocoCount + ".blc");
                    blocoCount++;
                    uÍndiceNoBloco = 0;
                    BlocoAtual = null;
                    BlocoAtual = new TFCap[Constants.NodosPorBloco];
                }
                return bPosAdicionado;
            }
            return new BPos(-1, -1);
        }

        public TFCap GetCapítulo(BPos pos) //pode jogar excessão
        {
            TFCap aux = new TFCap();
            int i;

            BinaryReader reader = new BinaryReader(File.Open(Constants.DiretorioRaiz + Constants.blocCapsFileName + pos.Bloco.ToString() + ".blc", FileMode.Open));
            for (i = 0; i < pos.Offset; i++)
                TFCap.LeBinario(reader);
            aux = TFCap.LeBinario(reader);
            reader.Close();

            return aux;
        }

        protected void SalvarBloco(string FileName)
        {
            BinaryWriter writer = new BinaryWriter(File.Open(FileName, FileMode.OpenOrCreate));
            for (int i = 0; i < uÍndiceNoBloco; i++)
            {
                TFCap.GravaBinário(writer, BlocoAtual[i]);
            }
            writer.Close();
            // salva todo o BlocoAtual no disco
        }

    }
    // Blocos das Conferências
    public class TBlocoConferênciasHandler
    {
       private TFConferência[] BlocoAtual;
        protected int blocoCount;
        protected int uÍndiceNoBloco;

        public TBlocoConferênciasHandler()
        {
            BlocoAtual = null;
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
                SalvarBloco(Constants.DiretorioRaiz+Constants.blocConferênciasFileName + blocoCount + ".blc");
            BlocoAtual = null;
            blocoCount++;
        }

        public static TBlocoConferênciasHandler LeIndices(string arquivo)
        {
            BinaryReader reader = new BinaryReader(File.Open(arquivo, FileMode.Open));
            TBlocoConferênciasHandler handler = new TBlocoConferênciasHandler();
            handler.blocoCount = reader.ReadInt32();
            handler.uÍndiceNoBloco = reader.ReadInt32();
            reader.Close();
            return handler;
        }

        public void GravaIndices(string arquivo)
        {
            BinaryWriter writer = new BinaryWriter(File.Open(arquivo, FileMode.Create));
            writer.Write(this.blocoCount);
            writer.Write(this.uÍndiceNoBloco);
            writer.Close();
        }

        public BPos AdicionaInformação(TFConferência Informação)
        {
            if (BlocoAtual != null)
            {
                BlocoAtual[uÍndiceNoBloco] = Informação;
                BPos bPosAdicionado = new BPos(blocoCount, uÍndiceNoBloco);
                uÍndiceNoBloco++;
                if (uÍndiceNoBloco >= Constants.NodosPorBloco)
                {
                    SalvarBloco(Constants.DiretorioRaiz+Constants.blocConferênciasFileName + blocoCount + ".blc");
                    blocoCount++;
                    uÍndiceNoBloco = 0;
                    BlocoAtual = null;
                    BlocoAtual = new TFConferência[Constants.NodosPorBloco];
                }
                return bPosAdicionado;
            }
            return new BPos(-1, -1);
        }

        public TFConferência GetConferência(BPos pos) //pode jogar excessão
        {
            TFConferência aux = new TFConferência();
            int i;

            BinaryReader reader = new BinaryReader(File.Open(Constants.DiretorioRaiz + Constants.blocConferênciasFileName + pos.Bloco.ToString() + ".blc", FileMode.Open));
            for (i = 0; i < pos.Offset; i++)
                TFConferência.LeBinario(reader);
            aux = TFConferência.LeBinario(reader);
            reader.Close();

            return aux;
        }

        protected void SalvarBloco(string FileName)
        {
            BinaryWriter writer = new BinaryWriter(File.Open(FileName, FileMode.OpenOrCreate));
            for (int i = 0; i < uÍndiceNoBloco; i++)
            {
                TFConferência.GravaBinário(writer, BlocoAtual[i]);
            }
            writer.Close();
            // salva todo o BlocoAtual no disco
        }
    }

    /// DEFINIÇÃO DAS CLASSES QUE TERÃO INFORMAÇÕES CRUCIAIS SOMENTE */
    ///   * ESSA SERÁ A BASE DE DADOS QUE O PROGRAMA SE ORGANIZARÁ */
    ///   * ESSES DADOS SERÃO DEIXADOS NA MEMÓRIA E NUNCA SAIRÃO DE LÁ */  

    // Um tipo super-básico de ponteiro para um Nodo - contém somente o índice na lista-mestre respectiva
    public struct TPNodo
    {
        public Int64 Índice;            // Índice na correspondente lista
        public int Tipo;                // Tipo (0= Pessoa; 1= Artigo; 2= Livro; 3= Periódico; 4= Capítulo; 5= Conferência)
        
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

        public void GravaTPNodo(BinaryWriter writer)
        {
            writer.Write(this.Índice);
            writer.Write(this.Tipo);
        }

        public static TPNodo LeTPNodo(BinaryReader reader)
        {
            TPNodo PNodo = new TPNodo();
            PNodo.Índice = reader.ReadInt64();
            PNodo.Tipo = reader.ReadInt32();
            return PNodo;
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
        public string ISSN;
        public string NomeNormalizado;
        public BPos Data;               // Posição no disco (bloco e offset) de todos os dados
        public TPNodo Nodo;             // Informação sobre que tipo é e onde está na lista principal
        public TDrawNodo DrawNodo;      // Nodo de desenho (ponteiro)
        public bool Pesquisado;

        // Constructors
        public TInfoNodo(string Nome, BPos Data) : base() 
        {
            this.Nome = Nome;
            this.NomeNormalizado = StringFunctions.CustomNormalize(Nome);
            this.Data = Data;
            this.Nodo = new TPNodo(-1, -1);
            this.Ligações = new List<TPNodo>();
            DrawNodo = null;
            Pesquisado = false;
        }
        // Constructors
        public TInfoNodo(string Nome,string ISSN, BPos Data)
            : base()
        {
            this.Nome = Nome;
            this.NomeNormalizado = StringFunctions.CustomNormalize(Nome);
            this.Data = Data;
            this.ISSN = ISSN;
            this.Nodo = new TPNodo(-1, -1);
            this.Ligações = new List<TPNodo>();
            DrawNodo = null;
            Pesquisado = false;
        }

        public void GravaTInfoNodo(BinaryWriter writer)
        {
            writer.Write(this.Ligações.Count);
            for (int i = 0; i < this.Ligações.Count; i++)
                this.Ligações[i].GravaTPNodo(writer);
            writer.Write(this.Nome);
            writer.Write(this.NomeNormalizado);
            if (this.ISSN != null)
                writer.Write(this.ISSN);
            else
                writer.Write(string.Empty);
            this.Data.GravaBPos(writer);
            this.Nodo.GravaTPNodo(writer);

        }

        public static TInfoNodo LeTInfoNodo(BinaryReader reader)
        {
            TInfoNodo INodo = new TInfoNodo();
            INodo.Ligações = new List<TPNodo>();
            int aux = reader.ReadInt32();
            for(int i = 0;i<aux;i++)
                INodo.Ligações.Add(TPNodo.LeTPNodo(reader));
            INodo.Nome = reader.ReadString();
            INodo.NomeNormalizado = reader.ReadString();
            INodo.ISSN = reader.ReadString();
            INodo.Data = BPos.LeBPos(reader);
            INodo.Nodo = TPNodo.LeTPNodo(reader);
            INodo.Pesquisado = false;
            return INodo;
        }

        public TInfoNodo()
        {
            this.Data.Bloco = -1;
            this.Data.Offset = -1;
            DrawNodo = null;
        }

        public bool MesmoISSN(string ISSN)
        {
            if (this.ISSN == "i" || this.ISSN == null)
                return false;
            if (ISSN != null)
            {
                if (this.ISSN == ISSN)
                    return true;
                else
                    return false;
            }
            else
                return false;


        }
        // Função que verifica se o nome de entrada é o mesmo (ou seja, se todas as palavras de Value estão em Nome)
        public bool MesmoNome(string Value, bool ForceEqual, bool UseDistance, bool useNormalizedName)
        {
            if (Nome == null)
                return false;
            // TODO - TERMINAR
            if (Value != null)
            {
                string sNome, sValue;
                if (useNormalizedName)
                    sNome = NomeNormalizado.ToLower();
                else
                    sNome = Nome.ToLower();
                sValue = Value.ToLower();
                if (ForceEqual)
                    return StringFunctions.CompareStr(Value, Nome, UseDistance, 2);
                else
                {
                    if (sValue == sNome)
                        return true;
                    bool Cont = true;
                    int sCount = 0;
                    foreach (string substr in sValue.Split(' '))
                    {
                        if (!sNome.Contains(substr))
                        {
                            Cont = false;
                            sCount++;
                        }
                    }
                    if (Cont)
                        return true;
                    string[] vsplits = sValue.Split(' ');
                    string[] nsplits = sNome.Split(' ');
                    bool ContainsUpper = true;
                    int UpperCount = 0;
                    // Se o nome for o mesmo
                    foreach (char c in sValue)
                        if (char.IsUpper(c))
                        {
                            if (!sNome.Contains(c))
                                ContainsUpper = false;
                            else
                                UpperCount++;
                        }
                    // Se a última ou 1ª palavra existir e o número de maísuculas for o mesmo de letras, true
                    if (sNome.Contains(vsplits[vsplits.Length - 1]))
                    {
                        if (vsplits.Length == UpperCount)
                            return true;
                    }
                    if (sNome.Contains(vsplits[0]))
                    {
                        if (vsplits.Length == UpperCount)
                            return true;
                    }
                }

                return false;
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
                // Procura uma ligação com esses elementos
                int Índice = -1;
                for (int i = 0; i < Ligações.Count; i++)
                {
                    if ((Ligações[i].Índice == Nodo.Índice) &&
                        (Ligações[i].Tipo == Nodo.Tipo))
                    {
                        Índice = i;
                    }
                }
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
                int Índice = -1;
                for (int i = 0; i < Ligações.Count; i++)
                {
                    if ((Ligações[i].Índice == Nodo.Índice) &&
                        (Ligações[i].Tipo == Nodo.Tipo))
                    {
                        Índice = i;
                    }
                }
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
                if (ProcuraNodo(Nodo.Nome, true, false, false).Count == 0)
                {
                    Add(Nodo);
                    this[this.Count - 1].Nodo = new TPNodo(this.Count-1, Tipo);
                }
            }
            return Count - 1;
        }
        // Cria um novo elemento dado somente um nome e um ISSN
        public int NovoNodo(string Nome,string ISSN)
        {
            List<TInfoNodo> Lista = ProcuraNodoISSN(ISSN, true);
            if (Lista.Count == 0)
            {
                Add(new TInfoNodo(Nome,ISSN, new BPos(-1, -1)));
                this[this.Count - 1].Nodo = new TPNodo(this.Count - 1, Tipo);
                return this.Count - 1;
            }
            else
            {
                return (int)Lista[0].Nodo.Índice;
            }
        }
        // Cria um novo elemento dado somente um nome
        public int NovoNodo(string Nome)
        {
            List<TInfoNodo> Lista = ProcuraNodo(Nome, true, false, false);
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

        public void GravaTBigList(string arquivo)
        {
            BinaryWriter writer = new BinaryWriter(File.Open(arquivo, FileMode.Create));
            writer.Write(Tipo);
            writer.Write(this.Count);
            for (int i = 0; i < this.Count; i++)
                this[i].GravaTInfoNodo(writer);
            writer.Close();
        }
         public static TBigList LeTBigList(string arquivo)
         {
             BinaryReader reader = new BinaryReader(File.Open(arquivo, FileMode.Open));
             TBigList BigList = new TBigList(reader.ReadInt32());
             int aux = reader.ReadInt32();
             for (int i = 0; i < aux; i++)
                 BigList.Add(TInfoNodo.LeTInfoNodo(reader));
             reader.Close();
                 return BigList;
         }

         // Funções para procurar todos os Elementos com dado Critério
         public List<TInfoNodo> ProcuraNodo(string Nome, bool SomenteIgual, bool useDistance, bool useNormalizedName)
        {
            return this.FindAll(p => p.MesmoNome(Nome, SomenteIgual, useDistance, useNormalizedName));
        }
        public List<TInfoNodo> ProcuraNodoISSN(string ISSN, bool SomenteIgual)
        {
            return this.FindAll(p => p.MesmoISSN(ISSN));
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

    public static class StringFunctions
    {
        public static int EditionDistance(string s1, string s2)
        {
            int[,] matrix = new int[s1.Length+1,s2.Length+1];
            int equals = 0;
            for (int i = 0; i < s1.Length + 1; i++)
                matrix[i, 0] = i;
            for (int i = 0; i < s2.Length+1; i++)
                matrix[0, i] = i;
            

            for (int j = 1; j < s2.Length + 1; j++)
                for (int i = 1; i < s1.Length +1; i++)
                {
                    if (s1[i - 1] != s2[j - 1])
                        equals = 1;
                    else
                        equals = 0;
                    matrix[i, j] = Math.Min(Math.Min(matrix[i - 1, j]+1, matrix[i, j - 1]+1), (matrix[i - 1, j - 1] + equals));
                }
            return matrix[s1.Length, s2.Length];
        }

        public static string CustomNormalize(string s)
        {
            string aux = string.Empty;
            string entrada = s.ToLower();
            for (int i = 0; i < s.Length; i++)
            {
                if (entrada[i] == 'ã')
                    aux += 'a';
                else if (entrada[i] == 'ç')
                    aux += 'c';
                else if (entrada[i] == 'ó')
                    aux += 'o';
                else if (entrada[i] == 'á')
                    aux += 'a';
                else if (entrada[i] == 'à')
                    aux += 'a';
                else if (entrada[i] == 'é')
                    aux += 'e';
                else if (entrada[i] == 'í')
                    aux += 'i';
                else if (entrada[i] == 'ô')
                    aux += 'o';
                else if (entrada[i] == 'ê')
                    aux += 'e';
                else if (entrada[i] == 'ü')
                    aux += 'u';
                else if (entrada[i] == 'Ã')
                    aux += 'A';
                else if (entrada[i] == 'Ç')
                    aux += 'C';
                else if (entrada[i] == 'Ó')
                    aux += 'O';
                else if (entrada[i] == 'Á')
                    aux += 'A';
                else if (entrada[i] == 'À')
                    aux += 'A';
                else if (entrada[i] == 'É')
                    aux += 'E';
                else if (entrada[i] == 'Í')
                    aux += 'I';
                else if (entrada[i] == 'Ô')
                    aux += 'O';
                else if (entrada[i] == 'Ê')
                    aux += 'E';
                else if (entrada[i] == 'Ü')
                    aux += 'U';
                else if (entrada[i] == ' ')
                    aux += ' ';
                else
                {
                    if(char.IsLetterOrDigit(entrada[i]))
                        aux += entrada[i];
                }
            }
            return aux;
        }

        public static bool CompareStr(string s1, string s2, bool useDistance, int Threshold)
        {
            if (useDistance)
                return (EditionDistance(s1, s2) <= Threshold);
            else
                return(s1 == s2);
        }
    }
}
