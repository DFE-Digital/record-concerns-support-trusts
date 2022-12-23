namespace ConcernsCaseWork.Attributes
{
	[System.AttributeUsage(System.AttributeTargets.Assembly, Inherited = false, AllowMultiple = false)]
	sealed class BuildTimeAttribute : System.Attribute
	{
		public string BuildTime { get; }
		public BuildTimeAttribute(string buildDateTime)
		{
			this.BuildTime = buildDateTime;
		}
	}
}