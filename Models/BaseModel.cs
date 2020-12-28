using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Projekt.Models
{
    public class BaseModel : PageModel
    {
        public string returnUrl { get; set; }
        public int nr { get; set; }
        public int iteracje { get; set; }
    }
}
