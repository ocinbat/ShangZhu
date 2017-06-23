using System;
using System.Net;
using RestSharp;

namespace ShangZhu.Authentication
{
    internal class AccessTokenProvider : IAccessTokenProvider
    {
        private readonly string _appId;
        private readonly string _appSecret;
        private readonly IRestClient _restClient;

        public AccessTokenProvider(string baseUrl, string appId, string appSecret)
        {
            _appId = appId;
            _appSecret = appSecret;
            _restClient = new RestClient(baseUrl);
        }

        public string GetAccessToken()
        {
            IRestRequest request = new RestRequest("token", Method.POST);
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddHeader("Accept", "application/json");

            request.AddParameter("grant_type", "client_credentials");
            request.AddParameter("client_id", _appId);
            request.AddParameter("client_secret", _appSecret);

            IRestResponse<AuthenticationResponse> response = _restClient.Execute<AuthenticationResponse>(request);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                return response.Data.AccessToken;
            }

            return String.Empty;
        }
    }
}