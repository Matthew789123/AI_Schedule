using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Projekt.Models;
using Microsoft.Extensions.Caching.Memory;
using System.IO;

namespace Projekt.Pages
{
    public class IndexModel : PageModel
    {
        public List<Przedmiot> przedmioty = new List<Przedmiot>();
        public List<Nauczyciel> nauczyciele = new List<Nauczyciel>();
        public List<PN> pn = new List<PN>();
        public List<Sala> sale = new List<Sala>();
        public string[] part;

        public IMemoryCache cache;

        public IndexModel(IMemoryCache _cache)
        {
            cache = _cache;
        }


        public IActionResult OnGet()
        {
            //------------------------------------------------------WCZYTYWANIE DANYCH------------------------------------------------------//
            using (StreamReader str = new StreamReader("Dane.txt"))
            {
                int liczba = Convert.ToInt32(str.ReadLine());
                for (int i = 0; i < liczba; i++)
                {
                    przedmioty.Add(new Przedmiot(str.ReadLine()));
                }
                liczba = Convert.ToInt32(str.ReadLine());
                for (int i = 0; i < liczba; i++)
                {
                    part = str.ReadLine().Split(' ');
                    nauczyciele.Add(new Nauczyciel(part[0], part[1]));
                    for (int j = 2; j < part.Length; j += 2)
                    {
                        pn.Add(new PN(nauczyciele.Last(), przedmioty.Find(x => x.nazwa == part[j]), Convert.ToInt32(part[j + 1])));
                    }
                }
                liczba = Convert.ToInt32(str.ReadLine());
                List<Przedmiot> p;
                for (int i = 0; i < liczba; i++)
                {
                    part = str.ReadLine().Split(' ');
                    p = new List<Przedmiot>();
                    for (int j = 1; j < part.Length; j++)
                    {
                        p.Add(przedmioty.Find(x => x.nazwa == part[j]));
                    }
                    sale.Add(new Sala(Convert.ToInt32(part[0]), p));
                }
            }
            //------------------------------------------------------WCZYTYWANIE DANYCH------------------------------------------------------//
            //--------------------------------------------------GENEROWANIE LOSOWEGO PLANU--------------------------------------------------//
            List<PN> kopia = new List<PN>();
            kopia = PN.CopyList(pn);
            Random rnd;
            int numer;
            int y = 0, x = 0;
            bool wolnaSala = false;
            bool wolnyNauczyciel = false;
            while (kopia.Count != 0)
            {
                rnd = new Random();
                numer = rnd.Next() % kopia.Count;
                y = rnd.Next() % 5;
                x = rnd.Next() % 14;
                wolnaSala = false;
                wolnyNauczyciel = false;
                while (!wolnaSala || !wolnyNauczyciel)
                {
                    wolnaSala = false;
                    wolnyNauczyciel = false;
                    if (kopia[numer].n.plan[y, x] == null && Godziny(kopia[numer].n, y) < 8)
                    {
                        wolnyNauczyciel = true;
                        foreach (var s in sale)
                        {
                            if (s.zajetosc[y, x] == false && s.przedmioty.Contains(kopia[numer].p) == true)
                            {
                                wolnaSala = true;

                                s.zajetosc[y, x] = true;
                                s.n[y, x] = new PN(nauczyciele.Find(x => x.imie == kopia[numer].n.imie), kopia[numer].p, 0);
                                nauczyciele.Find(x => x.imie == kopia[numer].n.imie).plan[y, x] = new Kafelek(kopia[numer].p, s);
                                kopia[numer].godziny -= 1;
                                if (kopia[numer].godziny == 0) kopia.RemoveAt(numer);
                                break;
                            }
                        }

                    }
                    x = (x + 1) % 14;
                    if (x == 0) break;
                }
                x = 0;
                y = 0;
            }
            //-----------------KONIEC---------------------------GENEROWANIE LOSOWEGO PLANU-----------------------KONIEC--------------------//
            int pkt = 0;
            foreach (var n in nauczyciele) n.punkty = n.SumujPkt2();
            foreach (var n in nauczyciele) pkt += n.punkty;

            cache.Remove("punkty");
            cache.Remove("xd");
            cache.Remove("sale");

            cache.Set("punkty", pkt);
            cache.Set("xd", nauczyciele);
            cache.Set("sale", sale);
            return RedirectToPage("Plan");
        }

        public int Godziny(Nauczyciel nauczyciel, int dzien)
        {
            int licznik = 0;
            for (int i = 0; i < 14; i++)
            {
                if (nauczyciel.plan[dzien, i] != null) licznik++;
            }
            return licznik;
        }


    }

}
