using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConcernsCaseWork.Data.Models
{
    [Table("NoticeToImproveConditionType", Schema = "concerns")]
    public class NoticeToImproveConditionType
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int DisplayOrder { get; set; }
    }
}