using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Projekt.Models
{
    public class Nauczyciel
    {
        public int punkty { get; set; }
        public string imie { get; set; }
        public string nazwisko { get; set; }
        public Kafelek[,] plan = new Kafelek[5, 14];

        public Nauczyciel(string imie, string nazwisko)
        {
            this.imie = imie;
            this.nazwisko = nazwisko;
        }
        public Nauczyciel Copy()
        {
            Nauczyciel n = new Nauczyciel(this.imie, this.nazwisko);
            Przedmiot _przedmiot;
            Sala _sala;
            List<Przedmiot> _przedmioty;
            n.plan = new Kafelek[5, 14];
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 14; j++)
                {
                    if (this.plan[i, j] == null)
                    {
                        n.plan[i, j] = null;
                    }
                    else
                    {
                        //-------------Kopia przedmiotu w kafelku-----------
                        _przedmiot = new Przedmiot(new string(this.plan[i, j].p.nazwa));
                        //-------------Przedmioty dla sali------------------
                        _przedmioty = new List<Przedmiot>();
                        foreach (var p in this.plan[i, j].s.przedmioty) _przedmioty.Add(new Przedmiot(new string(p.nazwa)));
                        //----------Kopia sali------------------------------
                        _sala = new Sala(this.plan[i, j].s.numer, _przedmioty);
                        for (int k = 0; k < 5; k++)
                        {
                            for (int l = 0; l < 14; l++)
                            {
                                _sala.zajetosc[k, l] = this.plan[i, j].s.zajetosc[k, l] == true ? true : false;
                            }
                        }
                        n.plan[i, j] = new Kafelek(_przedmiot, _sala);
                    }
                }
            }
            return n;
        }

        public int SumujPkt2()
        {
            int punkty = 0;
            int temp = 0;
            int temp2 = 0;
            int przedOkienkiem = 0;
            int okienko = 0;
            int liczbaOkienek = 0;
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 14; j++)
                {
                    //--------------------Zajecia o 8
                    if (j == 0 && plan[i, j] != null) punkty += 10;
                    //--------------------Zajecia po 18
                    if (j >= 11 && plan[i, j] != null) punkty += 10;
                    //--------------------Zajecia w dniu
                    if (temp == 0 && plan[i, j] != null)
                    {
                        punkty += 60;
                        temp = 1;
                    }
                    //--------------------Zajecia w piatek po 14
                    if (i == 4 && temp2 == 0 && j >= 6)
                    {
                        punkty += 50;
                        temp2 = 1;
                    }
                    //--------------------Okienka (nie sprawdza ile godzin okienka)
                    if (plan[i, j] != null) przedOkienkiem = 1;
                    if (przedOkienkiem == 1 && plan[i, j] == null) okienko += 1;
                    if (przedOkienkiem == 1 && okienko >= 1 && plan[i, j] != null)
                    {
                        liczbaOkienek += okienko;
                        okienko = 0;
                    }
                }
                //--------------zerowanie pomocniczych
                punkty += liczbaOkienek * 20;
                liczbaOkienek = 0;
                przedOkienkiem = 0;
                okienko = 0;
                temp = 0;
            }
            return punkty;
        }
    }
}