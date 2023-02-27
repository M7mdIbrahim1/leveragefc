using System;
using Backend.Auth;
using Backend.Models.LineOfBusinessViewModels;
using Backend.Models.FileViewModels;

namespace Backend.Models.CompanyViewModels
{
    public class CompanyGroupViewModel
    {
        public int? Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int? CompanyId { get; set; }
    }
}