using FreeCourse.Web.Exceptions;
using FreeCourse.Web.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace FreeCourse.Web.Handler
{
    public class ResourceOwnerPasswordTokenHandler:DelegatingHandler
    {
        private readonly IHttpContextAccessor _htpContextAccessor;
        private readonly IIdentityService _identityService;
        private readonly ILogger<ResourceOwnerPasswordTokenHandler> _logger;

        public ResourceOwnerPasswordTokenHandler(IHttpContextAccessor htpContextAccessor,
            IIdentityService identityService,
            ILogger<ResourceOwnerPasswordTokenHandler> logger)
        {
            _htpContextAccessor = htpContextAccessor;
            _identityService= identityService;
            _logger=logger;
        }

        protected async override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var accessToken = await _htpContextAccessor.HttpContext.GetTokenAsync(
                OpenIdConnectParameterNames.AccessToken);

            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            var response = await base.SendAsync(request, cancellationToken);

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                var tokenResponse = await _identityService.GetAccessTokenByRefreshToken();

                if (tokenResponse != null)
                {
                    request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", tokenResponse.AccessToken);
                    response = await base.SendAsync(request, cancellationToken);
                }
            }

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                throw new UnAuthorizedException();
            }

            return response;
        }
    }
}
