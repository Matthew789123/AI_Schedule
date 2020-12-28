using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Projekt.Models
{
    public class Kafelek
    {
        public Przedmiot p { get; set; }
        public Sala s { get; set; }

        public Kafelek(Przedmiot p, Sala s)
        {
            this.p = p;
            this.s = s;
        }
        public Kafelek() { }
        public Kafelek Kopia()
        {
            Kafelek pomocniczy = new Kafelek();
            List<Przedmiot> _przedmioty = new List<Przedmiot>();

            foreach (var p in this.s.przedmioty) _przedmioty.Add(new Przedmiot(p.nazwa));

            Sala sala = new Sala(this.s.numer, _przedmioty);

            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 14; j++)
                {
                    sala.zajetosc[i, j] = this.s.zajetosc[i, j] == true ? true : false;
                }
            }

            pomocniczy.p = new Przedmiot(this.p.nazwa);
            pomocniczy.s = sala;
            this.ToString();
            return pomocniczy;

        }
    }
}
