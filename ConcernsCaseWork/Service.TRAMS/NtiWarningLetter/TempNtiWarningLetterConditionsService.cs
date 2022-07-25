using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.TRAMS.NtiWarningLetter
{
	public class TempNtiWarningLetterConditionsService : INtiWarningLetterConditionsService
	{
		public async Task<ICollection<NtiWarningLetterConditionDto>> GetAllConditionsAsync()
		{
			var types = new NtiWarningLetterConditionTypeDto[]
			{
				new NtiWarningLetterConditionTypeDto
				{
					Id = 1,
					Name = "Financial Management conditions",
					DisplayOrder = 1
				},
				new NtiWarningLetterConditionTypeDto
				{
					Id = 2,
					Name = "Governance conditions",
					DisplayOrder = 2
				},
				new NtiWarningLetterConditionTypeDto
				{
					Id = 3,
					Name = "Compliance conditions",
					DisplayOrder = 3
				},
				new NtiWarningLetterConditionTypeDto
				{
					Id = 4,
					Name = "Standard conditions (mandatory)",
					DisplayOrder = 4
				}
			};

			var conditions = new List<NtiWarningLetterConditionDto>
			{
				new NtiWarningLetterConditionDto()
				{
					Id = 1,
					Name = "Trust financial plan",
					Type = types[0],
				}
			};

			conditions.AddRange(Enumerable.Range(1, 4).Select(i =>
				new NtiWarningLetterConditionDto
				{
					Id = i,
					Name = $"{types[1].Name} Condition {i}",
					Type = types[1],
				}
			));

			conditions.AddRange(Enumerable.Range(1, 1).Select(i =>
				new NtiWarningLetterConditionDto
				{
					Id = i,
					Name = $"{types[2].Name} Condition {i}",
					Type = types[2],
				}
			));

			conditions.AddRange(Enumerable.Range(1, 1).Select(i =>
				new NtiWarningLetterConditionDto
				{
					Id = i,
					Name = $"{types[3].Name} Condition {i}",
					Type = types[3],
				}
			));

			return await Task.FromResult(conditions);
		}
	}
}
