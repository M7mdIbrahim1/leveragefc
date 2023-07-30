using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models
{
    public class Milestone : BaseEntity
    {

        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }

        [ForeignKey(nameof(LineOfBusiness))]
        public int LineOfBusinessId { get; set; }
        public virtual LineOfBusiness LineOfBusiness { get; set; }

        public string Description { get; set; }
        public bool NeedPayment { get; set; }
        public decimal? DefaultAmountValue { get; set; }

        public int? Index { get; set; }



    }
}