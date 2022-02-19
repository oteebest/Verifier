using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Verifier.Application.Interfaces.Services;
using Verifier.Shared.Enums;
using Verifier.Shared.Models.Request.HttpService;

namespace Verifier.Infrastructure.Services
{
    public class HttpService : IHttpService
    {
        private readonly HttpClient _httpClient;
        public HttpService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<HttpResponseMessage> PostAsync(string baseUrl, string endpoint, PostRequest request)
        {
            Uri baseUri = new Uri(baseUrl);
            _httpClient.BaseAddress = baseUri;
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.ConnectionClose = true;
          
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, endpoint);

            if (request.HttpServiceAuthenticationType == HttpServiceAuthenticationType.Basic)
            {
                if (string.IsNullOrEmpty(request.BasicAuthenticationUserName))
                {
                    throw new ArgumentException("user name not supplied for authentication");
                }

                if (string.IsNullOrEmpty(request.BasicAuthenticationPassword))
                {
                    throw new ArgumentException("password not supplied for authentication");
                }

                var authenticationString = $"{request.BasicAuthenticationUserName}:{request.BasicAuthenticationPassword}";
                var base64EncodedAuthenticationString = Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(authenticationString));
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Basic", base64EncodedAuthenticationString);
            }
            else if (request.HttpServiceAuthenticationType == HttpServiceAuthenticationType.BearerToken)
            {
                if (string.IsNullOrEmpty(request.Token))
                {
                    throw new ArgumentException("Token not supplied authentication");
                }

                requestMessage.Headers.Authorization = new AuthenticationHeaderValue($"Bearer {request.Token}");
            }

            if (request.ContentParmeters.Count > 0)
            {

                if (request.HttpContentType == HttpContentType.FormUrlEncoded)
                {
                    var content = new FormUrlEncodedContent(request.ContentParmeters);
                    requestMessage.Content = content;
                }
                else
                {
                    var parameters = JsonSerializer.Serialize(request.ContentParmeters);
                    var content = new StringContent(parameters);
                    requestMessage.Content = content;
                }
                
            }

            return await _httpClient.SendAsync(requestMessage);
        }
    }
}
