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

            Logger.Debug($"ShangZhu fetching key:{key} from Configius server.");
            IRestResponse<List<Setting>> response = Execute<List<Setting>>(request);

            Logger.Debug($"Getting data from response.");
            List<Setting> settings = response.Data;

            if (settings == null || !settings.Any())
            {
                Logger.Debug($"Response data is null. MEans no key found. Throwing exception.");
                throw new ConfigurationErrorsException($"ShangZhu cannot find key:{key} for application:{_appId} at environment:{_environment}.");
            }

            Logger.Debug($"Key found. Returning value:{settings.First().Value}");
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
            if (String.IsNullOrEmpty(_accessToken))
            {
                Logger.Debug($"No AccessToken found. Refreshing.");
                RefreshAccessToken();
            }

            Logger.Debug($"Injecting access token to request.");
            InjectAccessToken(request);

            Logger.Debug($"Calling server.");
            IRestResponse<T> response = _restClient.Execute<T>(request);
            Logger.Debug($"Server returned statusCode:{response.StatusCode}");

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                Logger.Debug($"Unauthorized. Refreshing access token.");
                RefreshAccessToken();

                Logger.Debug($"Injecting access token to old request.");
                InjectAccessToken(request);

                Logger.Debug($"Calling for server for key.");
                response = _restClient.Execute<T>(request);
                Logger.Debug($"Server returned response:{response.Content} with statusCode:{response.StatusCode} for key.");

                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    Logger.Debug($"Unauthorized after token refresh. Throwing exception.");
                    throw new UnauthorizedAccessException("ShangZhu cannot connect to Configius with given appId and appSecret.");
                }
            }

            return response;
        }

        private void InjectAccessToken(IRestRequest request)
        {
            Logger.Debug($"Checking if auth header exists.");
            Parameter authHeader = request.Parameters.FirstOrDefault(p => p.Type == ParameterType.HttpHeader && p.Name == "Authorization");

            if (authHeader != null)
            {
                Logger.Debug($"Auth header found. Replacing value.");
                authHeader.Value = $"Bearer {_accessToken}";
                Logger.Debug($"New Auth Header Value:{authHeader.Value}");
            }
            else
            {
                Logger.Debug($"Auth header not found. Creating new.");
                request.AddHeader("Authorization", $"Bearer {_accessToken}");
            }
        }

        private void RefreshAccessToken()
        {
            _accessToken = _accessTokenProvider.GetAccessToken();
        }
    }
}