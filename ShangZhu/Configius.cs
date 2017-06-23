using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using RestSharp;
using ShangZhu.Authentication;
using ShangZhu.Dtos;

namespace ShangZhu
{
    internal class Configius : IConfigius
    {
        private readonly string _appId;
        private readonly string _environment;
        private readonly IRestClient _restClient;
        private readonly IAccessTokenProvider _accessTokenProvider;
        private string _accessToken;

        public Configius(string baseUrl, IAccessTokenProvider accessTokenProvider, string appId, string environment)
        {
            _restClient = new RestClient(baseUrl);
            _accessTokenProvider = accessTokenProvider;
            _environment = environment;
            _appId = appId;
        }

        public string Get(string key)
        {
            IRestRequest request = new RestRequest("settings", Method.GET);
            request.AddQueryParameter("appId", _appId);
            request.AddQueryParameter("environment", _environment);
            request.AddQueryParameter("key", key);

            IRestResponse<List<Setting>> response = Execute<List<Setting>>(request);

            List<Setting> settings = response.Data;

            if (settings == null || !settings.Any())
            {
                throw new ConfigurationErrorsException($"ShangZhu cannot find key:{key} for application:{_appId} at environment:{_environment}.");
            }

            return settings.First().Value;
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
                    throw new UnauthorizedAccessException("ShangZhu cannot connect to Configius with given appId and appSecret.");
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