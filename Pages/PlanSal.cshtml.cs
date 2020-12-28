using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Caching.Memory;
using Projekt.Models;

namespace Projekt.Pages
{
    public class PlanSalModel : BaseModel
    {
        public List<Nauczyciel> nauczyciele;
        public List<Sala> sale;
        public IMemoryCache cache;
        public string[] godziny { get; set; }
        public int punkty { get; set; }

        public PlanSalModel(IMemoryCache _cache)
        {
            cache = _cache;
        }

        public void OnGet(string nr)
        {
            punkty = (int)cache.Get("punkty");
            ViewData["returnUrl"] = "PlanSal";
            if (nr != null) ViewData["nr"] = nr;
            nauczyciele = (List<Nauczyciel>)cache.Get("xd");
            sale = (List<Sala>)cache.Get("sale");
            
            godziny = new string[] { "8:15-9:00", "9:15-10:00", "10:15-11:00", "11:15-12:00", "12:15-13:00", "13:15-14:00", "14:15-15:00", "15:05-15:50", "16:00-16:45", "16:50-17:35", "17:40-18:25", "18:30-19:15", "19:20-20:05", "20:10-20:55" };
        }

        public void OnPost(int nr)
        {
            punkty = (int)cache.Get("punkty");
            ViewData["returnUrl"] = "PlanSal";
            ViewData["nr"] = nr;
            nauczyciele = (List<Nauczyciel>)cache.Get("xd");
            sale = (List<Sala>)cache.Get("sale");
            
            godziny = new string[] { "8:15-9:00", "9:15-10:00", "10:15-11:00", "11:15-12:00", "12:15-13:00", "13:15-14:00", "14:15-15:00", "15:05-15:50", "16:00-16:45", "16:50-17:35", "17:40-18:25", "18:30-19:15", "19:20-20:05", "20:10-20:55" };
        }
    }
}