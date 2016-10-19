using System;
using System.Net;
using RestSharp;
using ShangZhu.Authentication;
using ShangZhu.Responses;

namespace ShangZhu
{
    internal class Configius : IConfigius
    {
        private readonly IRestClient _restClient;
        private readonly IAccessTokenProvider _accessTokenProvider;
        private string _accessToken;

        public Configius(IAccessTokenProvider accessTokenProvider)
        {
            _restClient = new RestClient(Constants.ConfigiusBaseUrl);
            _accessTokenProvider = accessTokenProvider;
        }

        public string Get(string key)
        {
            IRestRequest request = new RestRequest("v1/application-settings/{key}", Method.GET);
            request.AddParameter("key", key);

            IRestResponse<GetApplicationSettingResponse> response = Execute<GetApplicationSettingResponse>(request);

            return response?.Data?.Setting?.Value;
        }

        public T Get<T>(string key)
        {
            string value = Get(key);

            if (String.IsNullOrEmpty(value))
            {
                return default(T);
            }

            T typedValue = (T)Convert.ChangeType(value, typeof(T));

            if (typedValue == null)
            {
                return default(T);
            }

            return typedValue;
        }

        private IRestResponse<T> Execute<T>(IRestRequest request) where T : new ()
        {
            request.AddHeader("Authorization", $"Bearer {GetAccessToken()}");

            IRestResponse<T> response = _restClient.Execute<T>(request);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                request.AddHeader("Authorization", $"Bearer {GetAccessToken(true)}");
                response = _restClient.Execute<T>(request);

                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    throw new UnauthorizedAccessException("ShangZhu cannot connect to Configius with given client credentials.");
                }
            }

            return response;
        }

        private string GetAccessToken(bool refresh = false)
        {
            if (refresh || String.IsNullOrEmpty(_accessToken))
            {
                _accessToken = _accessTokenProvider.GetAccessToken();
            }

            return _accessToken;
        }
    }
}