using System;

namespace Week5_Test
{
    class Program
    {
        static void Main(string[] args)
        {
            int scelta = -1;
            bool uscita = false;
            while (!uscita)
            {
                Console.WriteLine();
                Console.WriteLine("-----------------");
                Console.WriteLine("MENU'");
                Console.WriteLine("-----------------");
                Console.WriteLine("Inserire il codice corrispondente all'azione che si desidera compiere: ");
                Console.WriteLine("[1] --> Inserire nuova Spesa");
                Console.WriteLine("[2] --> Approvare una Spesa esistente");
                Console.WriteLine("[3] --> Cancellare una Spesa");
                Console.WriteLine("[4] --> Visualizzare l'elenco delle Spese approvate");
                Console.WriteLine("[5] --> Visualizzare l'elenco delle Spese di uno specifico Utente");
                Console.WriteLine("[6] --> Visualizzare il totale delle Spese per Categoria");
                Console.WriteLine("[7] --> Visualizzare il totale delle Spese per Categoria (ADO)");
                Console.WriteLine("[8] --> Modificare una Spesa esistente");
                Console.WriteLine("[0] --> Esci");
                while (!int.TryParse(Console.ReadLine(), out scelta))
                {
                    Console.WriteLine("Codice inserito non corretto, riprova");
                }
                switch (scelta)
                {
                    case 1:
                        EFClient.InserisciSpesa();
                        break;
                    case 2:
                        EFClient.ApprovaSpesa();
                        break;
                    case 3:
                        EFClient.CalcellaSpesa();
                        break;
                    case 4:
                        EFClient.ElencaSpeseApprovate();
                        break;
                    case 5:
                        EFClient.FiltraSpesePerUtente();
                        break;
                    case 6:
                        EFClient.TotaleSpesePerCategoria();
                        break;
                    case 7:
                        EFClient.TotaleSpesePerCategoriaADO();
                        break;
                    case 8:
                        EFClient.ModificaSpesa();
                        break;
                    case 0:
                        uscita = true;
                        break;
                    default:
                        Console.WriteLine("La scelta effettuata non corrisponde a nessuna azione!");
                        Console.WriteLine();
                        break;
                }
            }
            Console.WriteLine("====Alla prossima!====");

        }
    }
}
