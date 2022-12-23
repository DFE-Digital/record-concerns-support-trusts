namespace ConcernsCaseWork.Attributes;

[System.AttributeUsage(System.AttributeTargets.Assembly, Inherited = false, AllowMultiple = false)]
sealed class CustomBuildMessageAttribute : System.Attribute
{
	public string CustomBuildMessage { get; }
	public CustomBuildMessageAttribute(string customBuildMessage)
	{
		this.CustomBuildMessage = customBuildMessage;
	}
}