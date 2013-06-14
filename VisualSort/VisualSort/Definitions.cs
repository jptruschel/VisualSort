using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VisualSort
{
    // Um vetor de 2 dimensões de inteiros
    class BPos
    {
        public int Bloco, Offset;
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
    }

    /// DEFINIÇÃO DAS CLASSES QUE TERÃO TODOS OS DADOS (GUARDADO EM DISCO)
    ///   * ESSA SERÁ A BASE DE DADOS DE TUDO QUE O PROGRAMA TERÁ  
    ///   * ESSES DADOS SERÃO DEIXADOS NO DISCO E CARREGADOS CONFORME NECESSÁRIO 
    // Um Periódico
    class TFPeriódico
    {
        long ISSN1, ISSN2;      // Códigos de ISSN
        string Nome;            // Nome do periódico
        string Qualis;          // Qualis
    }
    // Uma conferência
    class TFConferência
    {
        string Sigla, Nome;     // Nome (e Sigla)
        string Caráter;         // Caráter da conferência - INTERNACIONAL ou NACIONAL
        string Qualis;          // Qualis
    }
    // Uma Produção Bibliográfica
    class TFProdBibliográfica
    {
        string Título;          // Título
        string ISSN;            // ISSN relativo
        int Periodico;          // Índice na Lista de Periódicos
        string MeioDivulgação;  // Meio de Divulgação da Produção
        string Idioma;          // Idioma da Produção
        int AnoPublicação;   // Ano da Publicação
        string Natureza;        // Natureza (COMPLETO, ESTENDIDO, RESUMO)
    }
    // Uma Instituição
    class TFInstituição
    {
        string Nome;            // Nome da instituição
    }
    // Pessoas
    class TFPessoa
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
    class TElemento
    {
        public string Nome;            // Somente possui palavras suficentemente interessantes para pesquisa rápida
        public string Sigla;           // Para ainda mais rápida pesquisa

        public TElemento(string Nome) 
        {
            this.Nome = new string(' ', 0);
            this.Sigla = new string(' ', 0);
            foreach (string substr in Nome.Split(' '))
            {
                if (substr.Count() >= 3)
                    this.Nome = String.Concat(this.Nome, substr, " ");
                this.Sigla = String.Concat(this.Sigla, substr.Substring(0, 1));
            }
        }
        public TElemento()
        {
            this.Nome = new string(' ', 0);
            this.Sigla = new string(' ', 0);
        }
    }
    class TPessoa: TElemento
    {
        string[] Nomes;         // Array pequeno (4) de nomes curtos para pesquisa rápida por nome
        List<List<int>> Pessoas;// As pessoas que estão conectadas a essa (1ª dimensão: Ordem de atrelamento; 2ª dimensão: Lista das pessoas)
        List<List<int>> Inst;   // Lista das Instituições (mesma lógica das Pessoas)
        List<List<int>> Prod;   // Lista das Produções Bibliográficas (mesma lógica das Pessoas)
        List<List<int>> Conf;   // Lista das Conferências (mesma lógica das Pessoas)
        BPos Data;              // Posição no disco (bloco e offset) de todos os dados dessa pessoa
    }
    class TPeriódico : TElemento
    {
        BPos Data;              // Posição no disco (bloco e offset) do resto dos dados desse periódico
    }
    class TConferência : TElemento
    {
        BPos Data;              // Posição no disco (bloco e offset) do resto dos dados desse periódico
    }
    class TProdBibliográfica : TElemento
    {
        string Título;          // Título Completo da Produção
        string ISSN;            // ISSN
        int AnoPublicação;      // Ano da publicação
        string Natureza;        // Natureza
        List<int> Autores;      // Índices dos Autores (em ordem de importância)
        List<string> Chaves;    // Palavras-chave da Produção
        List<int> Áreas;        // Áreas do conhecimento abrangidas pela produção
        BPos Data;              // Posição no disco (bloco e offset) do resto dos dados desse periódico
    }
    class TInstituição : TElemento
    {
        BPos Data;              // Posição no disco (bloco e offset) do resto dos dados desse periódico
    }

    class Definitions
    {

    }
}
