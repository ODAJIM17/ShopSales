using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Sales.Shared.Entities
{
    public class City
    {
        public int Id { get; set; }

        [Display(Name = "Scity")]
        [Required(ErrorMessage = "Field {0} is required")]
        [MaxLength(50, ErrorMessage = "Field {0} allows max {1} characters")]
        public string Name { get; set; } = null!;
        public int StateId { get; set; }

        public State? State { get; set; }
    }
}
