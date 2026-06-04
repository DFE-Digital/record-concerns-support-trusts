namespace ConcernsCaseWork.Data.Models
{
	public class TargetedTrustEngagementType
	{
		private TargetedTrustEngagementType()
		{
		}

		public TargetedTrustEngagementType(API.Contracts.TargetedTrustEngagement.TargetedTrustEngagementActivity activity, API.Contracts.TargetedTrustEngagement.TargetedTrustEngagementActivityType? activityType) : this()
		{
			if (!Enum.IsDefined(typeof(API.Contracts.TargetedTrustEngagement.TargetedTrustEngagementActivity), activity))
				throw new ArgumentOutOfRangeException(nameof(activity),
					$"{activity} value is not one of the supported engagement activity");

			if (activityType.HasValue && !Enum.IsDefined(typeof(API.Contracts.TargetedTrustEngagement.TargetedTrustEngagementActivityType), activityType))
				throw new ArgumentOutOfRangeException(nameof(activityType),
					$"{activityType} value is not one of the supported engagement activity type");

			ActivityId = activity;
			ActivityTypeId = activityType;
		}

		public int Id { get; set; }
		public API.Contracts.TargetedTrustEngagement.TargetedTrustEngagementActivity ActivityId { get; set; }
		public API.Contracts.TargetedTrustEngagement.TargetedTrustEngagementActivityType? ActivityTypeId { get; set; }
	}
}
