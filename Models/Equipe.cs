using System;
using System.Collections.Generic;
using System.IO;
using E_Players_AspNETCore.Interfaces;

namespace E_Players_AspNETCore.Models
{
    public class Equipe : EPlayersBase , IEquipe
    {
        public int IdEquipe { get; set; }
        public string Nome { get; set; }
        public string Imagem { get; set; }

        private const string PATH = "Database/equipe.csv";

        /// <summary>
        /// Método construtor que cria os arquivos e pastas caso não existam
        /// </summary>
        public Equipe()
        {
            CreateFolderAndFile(PATH);
        }

        /// <summary>
        /// Adiciona uma Equipe ao CSV
        /// </summary>
        /// <param name="e"></param>
        public void Create(Equipe e)
        {
            string[] linha = { PrepararLinha(e) };
            File.AppendAllLines(PATH, linha);
        }

        /// <summary>
        /// Prepara a linha para a estrutura do objeto Equipe
        /// </summary>
        /// <param name="e">Objeto "Equipe"</param>
        /// <returns>Retorna a linha em formato de .csv</returns>
        private string PrepararLinha(Equipe e)
        {
            return $"{e.IdEquipe};{e.Nome};{e.Imagem}";
        }

        /// <summary>
        /// Exclui uma Equipe
        /// </summary>
        /// <param name="idEquipe"></param>
        public void Delete(int idEquipe)
        {
            List<string> linhas = ReadAllLinesCSV(PATH);
            // 1;FLA;fla.png
            linhas.RemoveAll(x => x.Split(";")[0] == idEquipe.ToString());                        
            RewriteCSV(PATH, linhas);
        }

        /// <summary>
        /// Lê todos as linhas do csv
        /// </summary>
        /// <returns>Lista de Equipes</returns>
        public List<Equipe> ReadAll()
        {
            List<Equipe> equipes = new List<Equipe>();
            string[] linhas = File.ReadAllLines(PATH);

            foreach (var item in linhas)
            {
                string[] linha = item.Split(";");

                Equipe equipe = new Equipe();
                equipe.IdEquipe = Int32.Parse(linha[0]);
                equipe.Nome = linha[1];
                equipe.Imagem = linha[2];

                equipes.Add(equipe);
            }
            return equipes;
        }

        /// <summary>
        /// Altera uma Equipe
        /// </summary>
        /// <param name="e">Equipe alterada</param>
        public void Update(Equipe e)
        {
            List<string> linhas = ReadAllLinesCSV(PATH);
            linhas.RemoveAll(x => x.Split(";")[0] == e.IdEquipe.ToString());
            linhas.Add( PrepararLinha(e) );                        
            RewriteCSV(PATH, linhas); 
        }
    }
}