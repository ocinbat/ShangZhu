using System.Runtime.Serialization;

namespace ShangZhu.Authentication
{
    internal class AuthenticationResponse
    {
        [DataMember(Name = "access_token")]
        public string AccessToken { get; set; }
    }
}