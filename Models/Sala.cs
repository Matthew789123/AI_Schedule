using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace Projekt.Models
{
    public class Sala
    {
        public int numer { get; set; }
        public List<Przedmiot> przedmioty = new List<Przedmiot>();
        public Boolean[,] zajetosc = new Boolean[5, 14];
        public PN[,] n = new PN[5, 14];

        public Sala(int numer, List<Przedmiot> przedmioty)
        {
            this.numer = numer;
            this.przedmioty = przedmioty;
        }
        public Sala() { }

        public Sala Kopia()
        {
            int _numer = this.numer;
            List<Przedmiot> _przedmioty = przedmioty;
            Boolean[,] _zajetosc = new Boolean[5, 14];
            for(int i=0; i<5; i++)
            {
                for (int j = 0; j < 14; j++)
                {
                    if (this.zajetosc[i, j] == false) _zajetosc[i, j] = false;
                    else _zajetosc[i, j] = true;
                }
            }

            PN[,] _n = new PN[5, 14];

            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 14; j++)
                {
                    if (n[i, j] != null) _n[i, j] = n[i, j].Copy();
                }
            }

            Sala nowaSala = new Sala();
            nowaSala.n = _n;
            nowaSala.numer = _numer;
            nowaSala.przedmioty = _przedmioty;
            nowaSala.zajetosc = _zajetosc;
            return nowaSala;
        }
    }
}
