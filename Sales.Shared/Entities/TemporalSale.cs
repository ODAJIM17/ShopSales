using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Sales.Shared.Entities
{
    public class TemporalSale
    {
        public int Id { get; set; }

        public User? User { get; set; }

        public string? UserId { get; set; }

        public Product? Product { get; set; }

        public int ProductId { get; set; }

        [DisplayFormat(DataFormatString = "{0:N2}")]
        [Display(Name = "Qty")]
        [Required(ErrorMessage = "{0} is required.")]
        public float Quantity { get; set; }

        [DataType(DataType.MultilineText)]
        [Display(Name = "Coments")]
        public string? Remarks { get; set; }

        public decimal Value => Product == null ? 0 : Product.Price * (decimal)Quantity;

    }
}
