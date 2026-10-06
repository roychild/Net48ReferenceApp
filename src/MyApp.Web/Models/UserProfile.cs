using System.Collections.Generic;

namespace MyApp.Web.Models
{
    public class UserProfile
    {
        public string Name { get; set; }

        public string NameIdentifier { get; set; }

        public string Issuer { get; set; }

        public IList<ClaimInfo> Claims { get; set; } = new List<ClaimInfo>();
    }

    public class ClaimInfo
    {
        public string Type { get; set; }

        public string Value { get; set; }
    }
}
