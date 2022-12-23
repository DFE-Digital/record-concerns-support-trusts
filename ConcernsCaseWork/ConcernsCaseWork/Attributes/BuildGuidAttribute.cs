namespace ConcernsCaseWork.Attributes;

[System.AttributeUsage(System.AttributeTargets.Assembly, Inherited = false, AllowMultiple = false)]
sealed class BuildGuidAttribute : System.Attribute
{
	public string BuildGuid { get; }
	public BuildGuidAttribute(string buildGuid)
	{
		this.BuildGuid = buildGuid;
	}
}