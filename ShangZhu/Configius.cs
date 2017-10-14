using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using RestSharp;
using ShangZhu.Authentication;
using ShangZhu.Dtos;
using ShangZhu.Logging;

namespace ShangZhu
{
    internal class Configius : IConfigius
    {
        private static readonly ILog Logger = LogProvider.GetCurrentClassLogger();

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
            Logger.Debug($"ShangZhu getting key:{key}.");
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
            InjectAccessToken(request);

            IRestResponse<T> response = _restClient.Execute<T>(request);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                RefreshAccessToken();
                InjectAccessToken(request);
                response = _restClient.Execute<T>(request);

                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    throw new UnauthorizedAccessException("ShangZhu cannot connect to Configius with given appId and appSecret.");
                }
            }

            if (response.StatusCode != HttpStatusCode.OK)
            {
                Logger.Error($"Configius returned an invalid status code:{response.StatusCode}. {response.Content}");
            }

            return response;
        }

        private void InjectAccessToken(IRestRequest request)
        {
            Parameter authHeader = request.Parameters.FirstOrDefault(p => p.Type == ParameterType.HttpHeader && p.Name == "Authorization");

            if (authHeader != null)
            {
                authHeader.Value = $"Bearer {_accessToken}";
            }
            else
            {
                request.AddHeader("Authorization", $"Bearer {_accessToken}");
            }
        }

        private void RefreshAccessToken()
        {
            _accessToken = _accessTokenProvider.GetAccessToken();
        }
    }
}