
using System.Linq.Expressions;
using Backend.Models;

namespace Backend.Models.ClientViewModels
{
    public class ClientSearchViewModel
    {
        public string? name { get; set; }
        public ICollection<int>? lineOfBusinessIds { get; set; }
        public int? page { get; set; }
        public int? pageSize { get; set; }
        public Expression<Func<Client, object>>? orderBy { get; set; }

    }
}