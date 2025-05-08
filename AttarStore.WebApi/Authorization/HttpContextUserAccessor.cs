using Microsoft.AspNetCore.Http;
using System.Security.Claims;


namespace AttarStore.WebApi.Authorization
{
    public class HttpContextUserAccessor : IUserContextAccessor
    {
        private readonly IHttpContextAccessor _http;

        public HttpContextUserAccessor(IHttpContextAccessor http) => _http = http;

        public string? ActorId => _http.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        public string? ActorRole => _http.HttpContext?.User.FindFirst(ClaimTypes.Role)?.Value;
    }
}

