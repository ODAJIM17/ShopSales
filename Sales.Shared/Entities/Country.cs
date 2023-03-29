using System.ComponentModel.DataAnnotations;

namespace Sales.Shared.Entities
{
    public class Country
    {
        public int Id { get; set; }

        [Display(Name = "Country Name")]
        [Required(ErrorMessage = "Field {0} is required")]
        [MaxLength(100, ErrorMessage = "Field {0} allows max {1} characters")]
        public string Name { get; set; } = null!;

        public ICollection<State>? States { get; set; }
        public int StatesNumber => States == null ? 0 : States.Count;
    }
}
