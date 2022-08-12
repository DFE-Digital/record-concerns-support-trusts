

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace Concerns.Data.Models
{
    public partial class GlobalOptionSetMetadata
    {
        public string OptionSetName { get; set; }
        public int Option { get; set; }
        public bool IsUserLocalizedLabel { get; set; }
        public int LocalizedLabelLanguageCode { get; set; }
        public string LocalizedLabel { get; set; }
    }
}
