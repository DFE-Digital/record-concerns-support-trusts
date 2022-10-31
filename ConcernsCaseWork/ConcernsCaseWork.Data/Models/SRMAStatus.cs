using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConcernsCaseWork.Data.Models
{
    [Table("SRMAStatus", Schema = "concerns")]
    public class SRMAStatus
    {
        [Key]
        public int Id { get; set; }
        public long? Urn { get; set; }
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}