using ConcernsCaseWork.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConcernsCaseWork.Data.Configurations.NoticeToImprove;

public class NoticeToImproveConditionConfiguration : IEntityTypeConfiguration<NoticeToImproveCondition>
{
	public void Configure(EntityTypeBuilder<NoticeToImproveCondition> builder)
	{
		builder.ToTable("NoticeToImproveCondition", "concerns");

		builder.HasKey(e => e.Id);
		
		var createdAt = new DateTime(2022, 08, 17);

		var financialManagementConditionTypeId = 1;
		var governanceConditionTypeId = 2;
		var complianceConditionTypeId = 3;
		var safeguardingConditionTypeId = 4;
		var fraudAndIrregularityConditionTypeId = 5;
		var standardConditionTypeId = 6;
		var additionalFinancialSupportConditionTypeId = 7;

		builder.HasData(
			new NoticeToImproveCondition[]
			{
				new NoticeToImproveCondition{ Id = 1, Name = "Audit and risk committee", CreatedAt = createdAt, UpdatedAt = createdAt, ConditionTypeId = financialManagementConditionTypeId, DisplayOrder = 1 },
				new NoticeToImproveCondition{ Id = 2, Name = "Internal audit findings", CreatedAt = createdAt, UpdatedAt = createdAt, ConditionTypeId = financialManagementConditionTypeId, DisplayOrder = 2 },
				new NoticeToImproveCondition{ Id = 3, Name = "Trust financial plan", CreatedAt = createdAt, UpdatedAt = createdAt, ConditionTypeId = financialManagementConditionTypeId, DisplayOrder = 3  },
				new NoticeToImproveCondition{ Id = 4, Name = "Financial management and governance review", CreatedAt = createdAt, UpdatedAt = createdAt, ConditionTypeId = financialManagementConditionTypeId, DisplayOrder = 4 },
				new NoticeToImproveCondition{ Id = 5, Name = "Financial systems & controls and internal scrutiny", CreatedAt = createdAt, UpdatedAt = createdAt, ConditionTypeId = financialManagementConditionTypeId, DisplayOrder = 5 },
				new NoticeToImproveCondition{ Id = 6, Name = "Integrated curriculum and financial planning", CreatedAt = createdAt, UpdatedAt = createdAt, ConditionTypeId = financialManagementConditionTypeId, DisplayOrder = 6 },
				new NoticeToImproveCondition{ Id = 7, Name = "Monthly management accounts", CreatedAt = createdAt, UpdatedAt = createdAt, ConditionTypeId = financialManagementConditionTypeId, DisplayOrder = 7 },
				new NoticeToImproveCondition{ Id = 8, Name = "National deals for schools", CreatedAt = createdAt, UpdatedAt = createdAt, ConditionTypeId = financialManagementConditionTypeId, DisplayOrder = 8 },
				new NoticeToImproveCondition{ Id = 9, Name = "School resource management", CreatedAt = createdAt, UpdatedAt = createdAt, ConditionTypeId = financialManagementConditionTypeId, DisplayOrder = 9 },

				new NoticeToImproveCondition{ Id = 10, Name = "Academy ambassadors", CreatedAt = createdAt, UpdatedAt = createdAt, ConditionTypeId = governanceConditionTypeId, DisplayOrder = 1 },
				new NoticeToImproveCondition{ Id = 11, Name = "Academy transfer", CreatedAt = createdAt, UpdatedAt = createdAt, ConditionTypeId = governanceConditionTypeId, DisplayOrder = 2 },
				new NoticeToImproveCondition{ Id = 12, Name = "Action plan", CreatedAt = createdAt, UpdatedAt = createdAt, ConditionTypeId = governanceConditionTypeId, DisplayOrder = 3 },
				new NoticeToImproveCondition{ Id = 13, Name = "AGM of members", CreatedAt = createdAt, UpdatedAt = createdAt, ConditionTypeId = governanceConditionTypeId, DisplayOrder = 4 },
				new NoticeToImproveCondition{ Id = 14, Name = "Board meetings", CreatedAt = createdAt, UpdatedAt = createdAt, ConditionTypeId = governanceConditionTypeId, DisplayOrder = 5 },
				new NoticeToImproveCondition{ Id = 15, Name = "Independant review of governance", CreatedAt = createdAt, UpdatedAt = createdAt, ConditionTypeId = governanceConditionTypeId, DisplayOrder = 6 },
				new NoticeToImproveCondition{ Id = 16, Name = "Lines of accountability", CreatedAt = createdAt, UpdatedAt = createdAt, ConditionTypeId = governanceConditionTypeId, DisplayOrder = 7 },
				new NoticeToImproveCondition{ Id = 17, Name = "Providing sufficient challenge", CreatedAt = createdAt, UpdatedAt = createdAt, ConditionTypeId = governanceConditionTypeId, DisplayOrder = 8 },
				new NoticeToImproveCondition{ Id = 18, Name = "Scheme of delegation", CreatedAt = createdAt, UpdatedAt = createdAt, ConditionTypeId = governanceConditionTypeId, DisplayOrder = 9 },
				new NoticeToImproveCondition{ Id = 19, Name = "School improvement", CreatedAt = createdAt, UpdatedAt = createdAt, ConditionTypeId = governanceConditionTypeId, DisplayOrder = 10 },
				new NoticeToImproveCondition{ Id = 20, Name = "Strengthen governance", CreatedAt = createdAt, UpdatedAt = createdAt, ConditionTypeId = governanceConditionTypeId, DisplayOrder = 11 },

				new NoticeToImproveCondition{ Id = 21, Name = "Admissions", CreatedAt = createdAt, UpdatedAt = createdAt, ConditionTypeId = complianceConditionTypeId, DisplayOrder = 1 },
				new NoticeToImproveCondition{ Id = 22, Name = "Excessive executive payments (high pay)", CreatedAt = createdAt, UpdatedAt = createdAt, ConditionTypeId = complianceConditionTypeId, DisplayOrder = 2 },
				new NoticeToImproveCondition{ Id = 23, Name = "Publishing requirements (compliance with)", CreatedAt = createdAt, UpdatedAt = createdAt, ConditionTypeId = complianceConditionTypeId, DisplayOrder = 3 },
				new NoticeToImproveCondition{ Id = 24, Name = "Related party Transactions (RPTs)", CreatedAt = createdAt, UpdatedAt = createdAt, ConditionTypeId = complianceConditionTypeId, DisplayOrder = 4 },

				new NoticeToImproveCondition{ Id = 25, Name = "Review and update safeguarding policies", CreatedAt = createdAt, UpdatedAt = createdAt, ConditionTypeId = safeguardingConditionTypeId, DisplayOrder = 1 },
				new NoticeToImproveCondition{ Id = 26, Name = "Commission external review of safeguarding", CreatedAt = createdAt, UpdatedAt = createdAt, ConditionTypeId = safeguardingConditionTypeId, DisplayOrder = 2 },
				new NoticeToImproveCondition{ Id = 27, Name = "Appoint trustee with leadership responsibility for safeguarding", CreatedAt = createdAt, UpdatedAt = createdAt, ConditionTypeId = safeguardingConditionTypeId, DisplayOrder = 3 },
				new NoticeToImproveCondition{ Id = 28, Name = "Safeguarding recruitment process", CreatedAt = createdAt, UpdatedAt = createdAt, ConditionTypeId = safeguardingConditionTypeId, DisplayOrder = 4 },

				new NoticeToImproveCondition{ Id = 29, Name = "Novel, contentious, and/or repercussive transactions", CreatedAt = createdAt, UpdatedAt = createdAt, ConditionTypeId = fraudAndIrregularityConditionTypeId, DisplayOrder = 1 },
				new NoticeToImproveCondition{ Id = 30, Name = "Off-payroll payments", CreatedAt = createdAt, UpdatedAt = createdAt, ConditionTypeId = fraudAndIrregularityConditionTypeId, DisplayOrder = 2 },
				new NoticeToImproveCondition{ Id = 31, Name = "Procurement policy", CreatedAt = createdAt, UpdatedAt = createdAt, ConditionTypeId = fraudAndIrregularityConditionTypeId, DisplayOrder = 3 },
				new NoticeToImproveCondition{ Id = 32, Name = "Register of interests", CreatedAt = createdAt, UpdatedAt = createdAt, ConditionTypeId = fraudAndIrregularityConditionTypeId, DisplayOrder = 4 },

				new NoticeToImproveCondition{ Id = 33, Name = "Financial returns", CreatedAt = createdAt, UpdatedAt = createdAt, ConditionTypeId = standardConditionTypeId, DisplayOrder = 1 },
				new NoticeToImproveCondition{ Id = 34, Name = "Delegated freedoms", CreatedAt = createdAt, UpdatedAt = createdAt, ConditionTypeId = standardConditionTypeId, DisplayOrder = 2 },
				new NoticeToImproveCondition{ Id = 35, Name = "Trustee contact details", CreatedAt = createdAt, UpdatedAt = createdAt, ConditionTypeId = standardConditionTypeId, DisplayOrder = 3 },

				new NoticeToImproveCondition{ Id = 36, Name = "Review of board and executive team capability", CreatedAt = createdAt, UpdatedAt = createdAt, ConditionTypeId = additionalFinancialSupportConditionTypeId, DisplayOrder = 1 },
				new NoticeToImproveCondition{ Id = 37, Name = "Academy transfer (lower risk)", CreatedAt = createdAt, UpdatedAt = createdAt, ConditionTypeId = additionalFinancialSupportConditionTypeId, DisplayOrder = 2 },
				new NoticeToImproveCondition{ Id = 38, Name = "Move to latest model funding agreement", CreatedAt = createdAt, UpdatedAt = createdAt, ConditionTypeId = additionalFinancialSupportConditionTypeId, DisplayOrder = 3 },
				new NoticeToImproveCondition{ Id = 39, Name = "Qualified Floating Charge (QFC)", CreatedAt = createdAt, UpdatedAt = createdAt, ConditionTypeId = additionalFinancialSupportConditionTypeId, DisplayOrder = 4 }
			});
	}
}