using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Verifier.Shared.Models.Request.HttpService;

namespace Verifier.Application.Interfaces.Services
{
    public interface IHttpService
    {
        Task<HttpResponseMessage> PostAsync(string baseUrl, string endpoint, PostRequest request);
    }
}
