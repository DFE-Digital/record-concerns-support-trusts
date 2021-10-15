namespace ConcernsCaseWork.Models
{
	/// <summary>
	/// Frontend model classes used only for UI rendering
	/// </summary>
	public sealed class EstablishmentModel
	{
		public string Urn { get; }
		
		public string LocalAuthorityCode { get; }
		
		public string LocalAuthorityName { get; }
		
		public string EstablishmentNumber { get; }
		
		public string EstablishmentName { get; }

		public string HeadteacherTitle { get; }

		public string HeadteacherFirstName { get; }

		public string HeadteacherLastName { get; }

		public string HeadteacherFullName { get { return string.Format("{0} {1} {2}", this.HeadteacherTitle, this.HeadteacherFirstName, this.HeadteacherLastName); } }

		public EstablishmentTypeModel EstablishmentType { get; }

		private string SchoolWebsite { get; }

		public string EstablishmentWebsite { get { return string.Format("http://{0}", this.SchoolWebsite); } }

		public EstablishmentModel(string urn, string localAuthorityCode, string localAuthorityName,
			string establishmentNumber, string establishmentName, string headteacherTitle, string headteacherFirstName,  string headteacherLastName, EstablishmentTypeModel establishmentType, string schoolWebsite) => 
			(Urn, LocalAuthorityCode, LocalAuthorityName, EstablishmentNumber, EstablishmentName, HeadteacherTitle, HeadteacherFirstName, HeadteacherLastName, EstablishmentType, SchoolWebsite) = 
			(urn, localAuthorityCode, localAuthorityName, establishmentNumber, establishmentName, headteacherTitle, headteacherFirstName, headteacherLastName, establishmentType, schoolWebsite);
	}
}