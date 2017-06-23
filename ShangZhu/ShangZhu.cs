using ShangZhu.Authentication;

namespace ShangZhu
{
    public class ShangZhu
    {
        public static IConfigius Connect(string appId, string appSecret, string environment)
        {
            return Connect(appId, appSecret, environment, Constants.ConfigiusBaseUrl);
        }

        public static IConfigius Connect(string appId, string appSecret, string environment, string configiusBaseUrl)
        {
            IAccessTokenProvider accessTokenProvider = new AccessTokenProvider(configiusBaseUrl, appId, appSecret);

            return new Configius(configiusBaseUrl, accessTokenProvider, appId, environment);
        }
    }
}
