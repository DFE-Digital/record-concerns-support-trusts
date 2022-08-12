namespace Concerns.Data.Models
{ 
    public partial class Account
        {
            public Guid Id { get; set; }
            public string SipUrn { get; set; }
            public string Name { get; set; }
            public int SipLocalAuthorityNumber { get; set; }
        }
}