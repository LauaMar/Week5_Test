using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Week5_Test.Model;

namespace Week5_Test
{
    public class EFClient
    {
        internal static void InserisciSpesa()
        {
            using GestioneSpeseContext ctx = new GestioneSpeseContext();
            Spesa newSpesa = new Spesa();
            Console.WriteLine("----Inserisci nuova Spesa----");
            Console.WriteLine();

            //richiesta dati all'utente
            DateTime data = new();
            Console.WriteLine("Inserisci la data in cui hai effettuato la spesa (Il termine ultimo per caricare le spese è un anno):");
            while (!(DateTime.TryParse(Console.ReadLine(), out data) && data <= DateTime.Now && data >= DateTime.Today.AddYears(-1)))
                Console.WriteLine("Data inserita non corretta");
            newSpesa.Data = data;

            int catId = -1;
            Console.WriteLine("Inserisci la categoria in cui rientra la spesa:");
            foreach(var cat in ctx.Categorie)
                Console.WriteLine($"[{cat.Id}] --> {cat.Descrizione}");
            while (!(int.TryParse(Console.ReadLine(), out catId) && catId>0))
                Console.WriteLine("Impossibile inserire un ID minore di zero");
            var categoriaScelta = ctx.Categorie.SingleOrDefault(
                c => c.Id == catId);
            if (categoriaScelta != null)
                newSpesa.CategoriaId = catId;
            else
            {
                Console.WriteLine("Nessuna categoria corrispondente all'ID inserito!");
                return;
            }

            Console.WriteLine("Inserisci la descrizione della spesa:");
            newSpesa.Descrizione= CheckString();

            Console.WriteLine("Inserisci l'Utente:");
            newSpesa.Utente = CheckString();

            decimal importo = -1;
            Console.WriteLine("Inserisci l'importo della spesa:");
            while (!(decimal.TryParse(Console.ReadLine(), out importo) && importo > 0))
                Console.WriteLine("Impossibile inserire un importo minore di zero");
            newSpesa.Importo = importo;

            newSpesa.Approvato = false;

            ctx.Spese.Add(newSpesa);
            ctx.SaveChanges();
            Console.WriteLine("Spesa inserita con successo!");
        }

        internal static void ApprovaSpesa()
        {
            using GestioneSpeseContext ctx = new GestioneSpeseContext();

            Console.WriteLine("Le spese in attesa di approvazione sono:");
            StampaSpese(s => s.Approvato == false);
            Console.WriteLine();
            Console.WriteLine("Inserisci l'ID della spesa da approvare:");
            Spesa spesaDaApprovare = CheckId(ctx);
            if (spesaDaApprovare != null && spesaDaApprovare.Approvato == false)
            {
                spesaDaApprovare.Approvato = true;
                ctx.SaveChanges();
                Console.WriteLine("Spesa approvata con successo!");
            }
            else
                Console.WriteLine("Id non trovato");
        }

        internal static void CalcellaSpesa()
        {
            using GestioneSpeseContext ctx = new GestioneSpeseContext();
            Console.WriteLine("Le spese presenti sono:");
            StampaSpese();
            Console.WriteLine("Inserisci l'ID della spesa da eliminare:");
           Spesa spesaDaRimuovere= CheckId(ctx);
            if (spesaDaRimuovere != null)
            {
                ctx.Spese.Remove(spesaDaRimuovere);
                ctx.SaveChanges();
                Console.WriteLine("Spesa cancellata con successo!");
            }
            else
            {
                Console.WriteLine("Id non trovato");
                return;
            }
                
        }

        internal static void ElencaSpeseApprovate()
        {
            using GestioneSpeseContext ctx = new GestioneSpeseContext();

            var item = ctx.Spese.FirstOrDefault(s => s.Approvato == true);
            if (item != null)
            {
                Console.WriteLine("====Elenco spese approvate====");
                StampaSpese(s => s.Approvato == true);
                Console.WriteLine();
            }
            else
                Console.WriteLine("Al momento non sono presenti spese approvate!");
        }

        internal static void FiltraSpesePerUtente()
        {
            using GestioneSpeseContext ctx = new GestioneSpeseContext();

            Console.WriteLine("Inserisci l'Utente:");
            string utente = CheckString();
            var item = ctx.Spese.FirstOrDefault(s => s.Utente == utente);
            if (item != null)
            {
                Console.WriteLine($"==== Elenco spese dell'utente {utente}====");
                StampaSpese(s => s.Utente == utente);
                Console.WriteLine();
            }
            else
                Console.WriteLine($"Nessuna spesa trovata a nome {utente}");
        }

        internal static void TotaleSpesePerCategoria()
        {
            using GestioneSpeseContext ctx = new GestioneSpeseContext();
            Console.WriteLine("====Totale spese per categoria====");
            Console.WriteLine(new String('-', 50));
            Console.WriteLine("{0,-25} {1, 20}", "Categoria", "Totale Spese");
            Console.WriteLine(new String('-', 50));

            foreach (var cat in ctx.Categorie.Include(c => c.Spese))
                Console.WriteLine("{0,-25} {1, 20}",
                        cat.Descrizione, cat.Spese.Sum(s => s.Importo));
            Console.WriteLine(new String('-', 50));
            Console.WriteLine();
        }

        internal static void TotaleSpesePerCategoriaADO()
        {
            IConfigurationRoot config = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json")
                    .Build();
            string connectionString = config.GetConnectionString("TestWeek5");

            using SqlConnection connection = new SqlConnection(connectionString);

            try
            {
                connection.Open();
                SqlCommand selectCommand = connection.CreateCommand();
                selectCommand.CommandType = System.Data.CommandType.Text;
                selectCommand.CommandText = "SELECT c.Descrizione as [Categoria], sum(s.Importo) as [Totale spese] FROM Spese s " +
                    "RIGHT JOIN Categorie c ON c.Id = s.CategoriaId GROUP BY c.Descrizione";

                SqlDataReader reader = selectCommand.ExecuteReader();

                Console.WriteLine("====Totale spese per categoria====");
                Console.WriteLine(new String('-', 50));
                Console.WriteLine("{0,-25} {1, 20}", "Categoria", "Totale Spese");
                Console.WriteLine(new String('-', 50));
                while (reader.Read())
                {
                    if(reader["Totale spese"]==DBNull.Value)
                        Console.WriteLine("{0,-25} {1, 20}", reader["Categoria"], "0");
                    else
                    Console.WriteLine("{0,-25} {1, 20}", reader["Categoria"], reader["Totale spese"]);
                }

                Console.WriteLine(new String('-', 50));
            }
            catch (Exception ex)
            {
                Console.Write($"Errore: {ex.Message}");
            }
            finally
            {
                connection.Close();
            }
        }

        internal static void ModificaSpesa()
        {
            using GestioneSpeseContext ctx = new GestioneSpeseContext();

            Console.WriteLine("Le spese presenti sono:");
            StampaSpese();
            Console.WriteLine("Inserisci l'ID della spesa da modificare:");
            Spesa spesaDaModificare = CheckId(ctx);
            if (spesaDaModificare == null)
            {
                Console.WriteLine("Id non trovato");
                return;
            }

            int catId = -1;
            Console.WriteLine("Inserisci la categoria in cui rientra la spesa:");
            foreach (var cat in ctx.Categorie)
                Console.WriteLine($"[{cat.Id}] --> {cat.Descrizione}");
            while (!(int.TryParse(Console.ReadLine(), out catId) && catId > 0))
                Console.WriteLine("Impossibile inserire un ID minore di zero");
            var categoriaScelta = ctx.Categorie.SingleOrDefault(
                c => c.Id == catId);
            if (categoriaScelta != null)
                spesaDaModificare.CategoriaId = catId;
            else
            {
                Console.WriteLine("Nessuna categoria corrispondente all'ID inserito!");
                return;
            }

            Console.WriteLine("Inserisci la descrizione della spesa:");
            spesaDaModificare.Descrizione = CheckString();

            decimal importo = -1;
            Console.WriteLine("Inserisci l'importo della spesa:");
            while (!(decimal.TryParse(Console.ReadLine(), out importo) && importo > 0))
                Console.WriteLine("Impossibile inserire un importo minore di zero");
            spesaDaModificare.Importo = importo;

            if(spesaDaModificare.Approvato==true) //una volta modificate, le spese già approvate necessitano nuovamente di approvazione
                 spesaDaModificare.Approvato = false;


            Console.WriteLine("Spesa modificata con successo!");
            ctx.SaveChanges();
            
        }


        #region Support Methods

        public static void StampaSpese(Func<Spesa, bool> filter = null)
        {
            using GestioneSpeseContext ctx = new GestioneSpeseContext();

            IEnumerable<Spesa> listaSpese;
            if (filter != null)
                listaSpese = ctx.Spese
                    .Include(s => s.Categoria)
                    .Where(filter);
            else
                listaSpese = ctx.Spese
                   .Include(s => s.Categoria);


            Console.WriteLine();
            Console.WriteLine(new String('-', 140));
            Console.WriteLine("{0,-5}{1,-40}{2,-20}{3,-25}{4,-20}{5,-15}{6,-10}", "ID", "Descrizione", "Utente", "Data", "Categoria",
                "Importo", "Approvato");
            Console.WriteLine(new String('-', 140));
            foreach (Spesa s in listaSpese)
            {
                Console.WriteLine("{0,-5}{1,-40}{2,-20}{3,-25}{4,-20}{5,-15}{6,-10}",
                    s.Id, s.Descrizione, s.Utente, s.Data.ToString("dd-MMM-yyyy HH:mm"), s.Categoria.Descrizione, s.Importo,
                    (s.Approvato? "YES" : "NO"));
            }
            Console.WriteLine(new String('-', 140));
        }

        private static Spesa CheckId(GestioneSpeseContext ctx)
        {
            int idScelto = -1;
            while (!(int.TryParse(Console.ReadLine(), out idScelto) && idScelto > 0))
                Console.WriteLine("Id inserito non valido, inserirne un altro");

            var spesa = ctx.Spese.Find(idScelto);
            if (spesa == null)
            {
                return null;
            }
            return spesa;
        }

        public static string CheckString()
        {
            string des = Console.ReadLine();
            while (string.IsNullOrEmpty(des))
            {
                Console.WriteLine("Impossibile inserire una stringa vuota, riprova");
                des = Console.ReadLine();
            }
            return des;
        }

        #endregion

    }
}
