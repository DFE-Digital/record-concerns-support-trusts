﻿namespace ConcernsCaseWork.Data.Models
{
	public class NTIWarningLetterConditionType
    {
	    public int Id { get; set; }
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int DisplayOrder { get; set; }
    }
}