namespace Backend.Models
{
    public class BaseEntity
    {
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string CreatorUserId { get; set; }
        public string LastUpdateUserId { get; set; }
        public int ChangeSequenceNumber { get; set; }
        public bool IsDeleted { get; set; }
        public bool? IsActive { get; set; }
    }
}


