using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Projekt.Models
{
    public class PN
    {
        public Nauczyciel n { get; set; }
        public Przedmiot p { get; set; }
        public int godziny { get; set; }

        public PN(Nauczyciel n, Przedmiot p, int godziny)
        {
            this.n = n;
            this.p = p;
            this.godziny = godziny;
        }

        public PN Copy()
        {
            PN cp = (PN)this.MemberwiseClone();
            return cp;
        }

        static public List<PN> CopyList(List<PN> list)
        {
            List<PN> copy = new List<PN>();
            foreach (var p in list)
            {
                copy.Add(p.Copy());
            }
            return copy;
        }
    }
}