﻿namespace ConcernsCasework.Service.Nti
{
	public interface INtiConditionsService
	{
		Task<ICollection<NtiConditionDto>> GetAllConditionsAsync();
	}
}
