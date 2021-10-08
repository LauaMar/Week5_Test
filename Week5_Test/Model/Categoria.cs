using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Week5_Test.Model
{
    public class Categoria
    {
        public int Id { get; set; }
        public string Descrizione { get; set; } //nel testo questa proprietà era definita come categoria,
                                                //ma non posso darle lo stesso nome della classe

        public IList<Spesa> Spese { get; set; }

    }
}
