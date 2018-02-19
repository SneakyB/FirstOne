using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;


namespace FirstOne.Models
{
    public class Client
    {
        public int ClientID { get; set; }

        public String Name { get; set; }

        public String Surname { get; set; }

        public String City { get; set; }

        public String Program { get; set; }

        [NotMapped]
        public IEnumerable<SelectListItem> Programs { get; set; }

    }
}
