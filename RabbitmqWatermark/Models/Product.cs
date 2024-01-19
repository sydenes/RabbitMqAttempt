using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RabbitmqWatermark.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        [StringLength(50)]
        public string Name { get; set; }
        [StringLength(500)]
        public string PictureUrl { get; set; }
        [Column(TypeName="decimal(18,2)")]
        public decimal Price { get; set; }
        [Range(1, 100)]
        public int Stock { get; set; }
    }
}
