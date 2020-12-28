using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Projekt.Models;
using Microsoft.Extensions.Caching.Memory;

namespace Projekt.Pages
{
    public class OptymalizacjaPlanuModel : BaseModel
    {
        public List<Nauczyciel> nauczyciele;
        public List<Sala> sale;

        public IMemoryCache cache;
        public string[] godziny { get; set; }

        public OptymalizacjaPlanuModel(IMemoryCache _cache)
        {
            cache = _cache;
        }

        public IActionResult OnPostHandler(string returnUrl, int nr, int iteracje)
        {
            nauczyciele = (List<Nauczyciel>)cache.Get("xd");
            sale = (List<Sala>)cache.Get("sale");
            int pkt = 0;
            for (int i = 0; i < iteracje; i++)
            {
                nauczyciele = Mutacja_v2(nauczyciele, sale, 200);
            }
            foreach (var n in nauczyciele) n.punkty = n.SumujPkt2();
            foreach (var n in nauczyciele) pkt += n.punkty;

            cache.Remove("xd");
            cache.Set("xd", nauczyciele);
            cache.Remove("punkty");
            cache.Set("punkty", pkt);
            ViewData["nr"] = nr;
            return RedirectToPage(returnUrl, new { nr = nr });
        }

        public List<Nauczyciel> Mutacja_v2(List<Nauczyciel> nauczyciele, List<Sala> saleOrginal, int szansaSzansaSzansa_SzansaNaSukces)
        {
            Random random = new Random(); ;
            int rnd;
            int dzien;
            int godzina;
            int indexSali = -1;
            int indexSali2 = -1;
            int staraSala;
            Kafelek pomocniczy;
            PN pom;
            //------------Głęboka kopia listy nauczycieli-------------------
            List<Nauczyciel> kopiaNauczyciele = new List<Nauczyciel>();
            foreach (var n in nauczyciele) kopiaNauczyciele.Add(n.Copy());
            //--------------------------------------------------------------
            //------------Głęboka kopia listy sal---------------------------
            List<Sala> sale = new List<Sala>();
            foreach (var s in saleOrginal) sale.Add(s.Kopia()); 
            //--------------------------------------------------------------
            //----------Dla każdego nauczyciela(dzialamy na kopii)--------------
            for (int i = 0; i < kopiaNauczyciele.Count(); i++)
            {
                //-----Dla każdego kafelka w planie nauczyciela------
                for (int j = 0; j < 5; j++)
                {
                    for (int k = 0; k < 14; k++)
                    {
                        //-----------Losu losu(czy wykonujemy mutacje)----------
                        rnd = random.Next() % szansaSzansaSzansa_SzansaNaSukces;
                        if (rnd == 0)
                        {
                            dzien = random.Next() % 5;
                            godzina = random.Next() % 14;
                            //-----Losu Losu(z którym kafelkiem zamieniamy)-------
                            while (j == dzien && k == godzina)
                            {
                                dzien = random.Next() % 5;
                                godzina = random.Next() % 14;
                            }
                            //-----------------------------Cztery przypadki-----------------------------
                            //----------------------------Oba kafelki puste----------------------------
                            if (kopiaNauczyciele[i].plan[j, k] == null && kopiaNauczyciele[i].plan[dzien, godzina] == null) return nauczyciele;
                            //--------------------------Pierwszy kafelek pusty--------------------------
                            else if (kopiaNauczyciele[i].plan[j, k] == null && kopiaNauczyciele[i].plan[dzien, godzina] != null)
                            {
                                //Szukamy pustej sali w dniu i godzinie z kafelka pierwszego dla przedmiotu z drugiego kafelka
                                for (int m = 0; m < sale.Count(); m++)
                                {
                                    if (sale[m].przedmioty.Exists(x => x.nazwa == kopiaNauczyciele[i].plan[dzien, godzina].p.nazwa) && sale[m].zajetosc[j, k] == false)
                                    {
                                        indexSali = m;
                                        break;
                                    }
                                }
                                //------------------------Nie znaleźliśmy pustej sali 
                                if (indexSali == -1) return nauczyciele;
                                //-------------------------Zajmujemy znalezioną sale
                                sale[indexSali].zajetosc[j, k] = true;
                                //-------------------------Zwalniamy sale z kafelka drugiego
                                staraSala = sale.FindIndex(x => x.numer == kopiaNauczyciele[i].plan[dzien, godzina].s.numer);
                                sale[staraSala].zajetosc[dzien, godzina] = false;
                                //-------------------------Ustawiam PN
                                sale[indexSali].n[j, k] = new PN(kopiaNauczyciele[i], kopiaNauczyciele[i].plan[dzien,godzina].p, 0);
                                sale[staraSala].n[dzien, godzina] = null;
                                //-------------------------zamieniamy kafelki

                                kopiaNauczyciele[i].plan[j, k] = kopiaNauczyciele[i].plan[dzien, godzina].Kopia();
                                kopiaNauczyciele[i].plan[dzien, godzina] = null;

                                //--------------------------Aktualizuje sale w kafelku pierwszym
                                kopiaNauczyciele[i].plan[j, k].s = sale[indexSali];//!!!!!!!!!!!!!!!!!!!!!!!!!Potencjalny błąd!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

                            }


                            //-----------------------------Drugi Kafelek pusty--------------------------
                            else if (kopiaNauczyciele[i].plan[j, k] != null && kopiaNauczyciele[i].plan[dzien, godzina] == null)
                            {

                                //Szukamy pustej sali w dniu i godzinie z kafelka drugiego dla przedmiotu z pierwszego kafelka
                                for (int m = 0; m < sale.Count(); m++)
                                {
                                    if (sale[m].przedmioty.Exists(x => x.nazwa == kopiaNauczyciele[i].plan[j, k].p.nazwa) && sale[m].zajetosc[dzien, godzina] == false)
                                    {
                                        indexSali = m;
                                        break;
                                    }
                                }

                                //------------------------Nie znaleźliśmy pustej sali 
                                if (indexSali == -1) return nauczyciele;
                                //-------------------------Zajmujemy znalezioną sale
                                sale[indexSali].zajetosc[dzien, godzina] = true;
                                //-------------------------Zwalniamy sale z kafelka pierwszego
                                staraSala = sale.FindIndex(x => x.numer == kopiaNauczyciele[i].plan[j, k].s.numer);
                                sale[staraSala].zajetosc[j, k] = false;
                                //-------------------------Ustawiam PN
                                sale[indexSali].n[dzien, godzina] = new PN(kopiaNauczyciele[i], kopiaNauczyciele[i].plan[j,k].p, 0);
                                sale[staraSala].n[j, k] = null;
                                //-------------------------zamieniamy kafelki

                                kopiaNauczyciele[i].plan[dzien, godzina] = kopiaNauczyciele[i].plan[j, k].Kopia();
                                kopiaNauczyciele[i].plan[j, k] = null;

                                //--------------------------Aktualizuje sale w kafelku drugim
                                kopiaNauczyciele[i].plan[dzien, godzina].s = sale[indexSali];//!!!!!!!!!!!!!!!!!!!!!!!!!Potencjalny błąd!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

                            }
                            //-----------------------------Żaden kafelek nie jest pusty------------------
                            else if (kopiaNauczyciele[i].plan[j, k] != null && kopiaNauczyciele[i].plan[dzien, godzina] != null)
                            {
                                //Szukamy pustej sali w dniu i godzinie z kafelka pierwszego dla przedmiotu z drugiego kafelka
                                for (int m = 0; m < sale.Count(); m++)
                                {
                                    if (sale[m].przedmioty.Exists(x => x.nazwa == kopiaNauczyciele[i].plan[dzien, godzina].p.nazwa) && sale[m].zajetosc[j, k] == false)
                                    {
                                        indexSali = m;
                                        break;
                                    }
                                }
                                //Szukamy pustej sali w dniu i godzinie z kafelka drugiego dla przedmiotu z pierwszego kafelka
                                for (int m = 0; m < sale.Count(); m++)
                                {
                                    if (sale[m].przedmioty.Exists(x => x.nazwa == kopiaNauczyciele[i].plan[j, k].p.nazwa) && sale[m].zajetosc[dzien, godzina] == false)
                                    {
                                        indexSali2 = m;
                                        break;
                                    }
                                }
                                // Jeśli nie znaleźliśmy sali chociaż jednego z przedmiotów to nic nie robimy
                                if (indexSali == -1 || indexSali2 == -1) return nauczyciele;
                                //-------------------------Zwalniamy stare sale
                                staraSala = sale.FindIndex(x => x.numer == kopiaNauczyciele[i].plan[j, k].s.numer);
                                sale[staraSala].zajetosc[j, k] = false;
                                sale[staraSala].n[j, k] = null;

                                staraSala = sale.FindIndex(x => x.numer == kopiaNauczyciele[i].plan[dzien, godzina].s.numer);
                                sale[staraSala].zajetosc[dzien, godzina] = false;
                                sale[staraSala].n[dzien, godzina] = null;
                                //-------------------------Zajmujemy nowe sale
                                sale[indexSali].zajetosc[j, k] = true;
                                sale[indexSali].n[j, k] = new PN(kopiaNauczyciele[i], kopiaNauczyciele[i].plan[dzien, godzina].p, 0);

                                sale[indexSali2].zajetosc[dzien, godzina] = true;
                                sale[indexSali2].n[dzien, godzina] = new PN(kopiaNauczyciele[i], kopiaNauczyciele[i].plan[j, k].p, 0);

                                // Zamieniamy kafelki
                                pomocniczy = kopiaNauczyciele[i].plan[dzien, godzina].Kopia();
                                kopiaNauczyciele[i].plan[dzien, godzina] = kopiaNauczyciele[i].plan[j, k].Kopia();
                                kopiaNauczyciele[i].plan[j, k] = pomocniczy;
                            }
                        }
                        indexSali = -1;
                        indexSali2 = -1;
                    }
                }
            }
            //---------------obliczanie punktacji dla nowego planu-------
            foreach (var n in kopiaNauczyciele) n.punkty = n.SumujPkt2();
            int punktyOginalu = 0;
            int punktyKopii = 0;
            //Sumowanie punków dla wszystkich planów
            foreach (var n in nauczyciele) punktyOginalu += n.punkty;
            foreach (var n in kopiaNauczyciele) punktyKopii += n.punkty;
            //---------------Zwracamy liste nauczycieli z lepszym planem--------
            if (punktyOginalu <= punktyKopii)
                return nauczyciele;
            else
            {
                
                cache.Remove("sale");
                cache.Set("sale", sale);
                return kopiaNauczyciele;
            }

        }
    }
}