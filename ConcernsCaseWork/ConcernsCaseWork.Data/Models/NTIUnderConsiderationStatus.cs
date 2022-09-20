using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConcernsCaseWork.Data.Models
{
    [Table("NTIUnderConsiderationStatus", Schema = "concerns")]
    public class NTIUnderConsiderationStatus
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}