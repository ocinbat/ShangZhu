using System;
using System.Net;
using RestSharp;
using RestSharp.Authenticators;

namespace ShangZhu.Authentication
{
    internal class AccessTokenProvider : IAccessTokenProvider
    {
        private readonly string _appId;
        private readonly string _appSecret;
        private readonly IRestClient _restClient;

        public AccessTokenProvider(string appId, string appSecret)
        {
            _appId = appId;
            _appSecret = appSecret;
            _restClient = new RestClient(Constants.ConfigiusBaseUrl);
            _restClient.Authenticator = new HttpBasicAuthenticator(appId, appSecret);
        }

        public string GetAccessToken()
        {
            IRestRequest request = new RestRequest("token", Method.POST);
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddHeader("Accept", "application/json");

            request.AddParameter("grant_type", "client_credentials");

            IRestResponse<AuthenticationResponse> response = _restClient.Execute<AuthenticationResponse>(request);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                return response.Data.AccessToken;
            }

            return String.Empty;
        }
    }
}