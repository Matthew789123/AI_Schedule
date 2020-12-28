using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Projekt.Models
{
    public class Przedmiot
    {
        public string nazwa { get; set; }

        public Przedmiot(string nazwa)
        {
            this.nazwa = nazwa;

        }
    }
}
