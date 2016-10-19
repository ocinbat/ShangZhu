using RestSharp;
using RestSharp.Authenticators;
using ShangZhu.Authentication;

namespace ShangZhu
{
    public class ShangZhu
    {
        public static IConfigius Connect(string appId, string appSecret)
        {
            IAccessTokenProvider accessTokenProvider = new AccessTokenProvider(appId, appSecret);

            return new Configius(accessTokenProvider);
        }
    }
}
