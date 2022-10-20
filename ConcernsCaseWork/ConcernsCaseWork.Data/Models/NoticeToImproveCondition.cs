using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConcernsCaseWork.Data.Models
{
    [Table("NoticeToImproveCondition", Schema = "sdd")]
    public class NoticeToImproveCondition
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int ConditionTypeId { get; set; }
        public int DisplayOrder { get; set; }

        public virtual NoticeToImproveConditionType ConditionType { get; set; }
        public virtual ICollection<NoticeToImproveConditionMapping> NoticeToImproveConditionsMapping { get; set; }
    }
}