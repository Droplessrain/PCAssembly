using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PCAssembly
{
    public class Component
    {
        public int ComponentId { get; set; }

        [Required(ErrorMessage = "The TypeComponents field is required.")]
        public int TypeComponentsId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        public string Description { get; set; }

        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }

      
        public virtual TypeComponent TypeComponents { get; set; }
    }
}
