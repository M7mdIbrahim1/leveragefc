using Backend.Models.LineOfBusinessViewModels;

namespace Backend.Models.MilestoneViewModels
{
    public class MilestoneViewModel
    {

        public int? Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int? LineOfBusinessId { get; set; }
        public virtual LineOfBusinessViewModel? LineOfBusiness { get; set; }

        public bool? NeedPayment { get; set; }
        public decimal? DefaultAmountValue { get; set; }

        public int? TotalCount { get; set; }
    }
}