using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using Backend.Models;

namespace Backend.Models.CompanyViewModels
{
    public class CompanySearchViewModel
    {
        public string? name { get; set; }
        public string? ownerId { get; set; }
        public int? page { get; set; }
        public int? pageSize { get; set; }
        public Expression<Func<Company, object>>? orderBy { get; set; }

    }
}