using FreeCourse.Shared.Dtos;
using FreeCourse.Web.Models;
using FreeCourse.Web.Services.Interfaces;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;

namespace FreeCourse.Web.Services
{
    public class IdentityService : IIdentityService
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor  _httpContextAccessor;
        private readonly ClientSettings _clientSettings;
        private readonly ServiceApiSettings _serviceApiSettings;

        public IdentityService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor, IOptions<ClientSettings> clientSettings, IOptions<ServiceApiSettings> serviceApiSettings)
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;

            _clientSettings = clientSettings.Value;
            _serviceApiSettings = serviceApiSettings.Value;
        }
        
        public async Task<TokenResponse> GetAccessTokenByRefreshToken()
        {
            // Var olan tum Endpointler bu sinifa gelecek
            var disco = await _httpClient.GetDiscoveryDocumentAsync(
                new DiscoveryDocumentRequest
                {
                    Address = _serviceApiSettings.IdentityBaseUri,
                    Policy = new DiscoveryPolicy { RequireHttps = false }
                });

            if (disco.IsError)
            {
                throw disco.Exception;
            }

            var refreshToken = await _httpContextAccessor.HttpContext.GetTokenAsync(
                OpenIdConnectParameterNames.RefreshToken);

            RefreshTokenRequest refreshTokenRequest = new()
            {
                ClientId = _clientSettings.WebClientForUser.ClientId,
                ClientSecret = _clientSettings.WebClientForUser.ClientSecret,
                RefreshToken = refreshToken,
                Address=disco.TokenEndpoint
            };

            var token = await _httpClient.RequestRefreshTokenAsync(refreshTokenRequest);

            if (token.IsError)
            {
                return null;
            }

            // Yeni bir token olustur
            var authenticationTokens = new List<AuthenticationToken>()
            {
                new AuthenticationToken{ Name=OpenIdConnectParameterNames.AccessToken,
                    Value=token.AccessToken},
                new AuthenticationToken{ Name=OpenIdConnectParameterNames.RefreshToken,
                    Value=token.RefreshToken},
                new AuthenticationToken { Name=OpenIdConnectParameterNames.ExpiresIn,
                    Value=DateTime.Now.AddSeconds(token.ExpiresIn).ToString("o",CultureInfo.InvariantCulture)}
            };

            // Eldeki authentication properties alinir
            var authenticationResult = await _httpContextAccessor.HttpContext.AuthenticateAsync();
            var properties = authenticationResult.Properties;
            properties.StoreTokens(authenticationTokens);

            // UserInfo Endpointine tekrar gidilmedi.Cookie icinde kullanici ile ilgili Claim ler var
            // Bu Claimler tekrar kullanilabilir

            await _httpContextAccessor.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                authenticationResult.Principal, properties);

            return token;
        }

        public async Task RevokeRefreshToken()
        {
            // Var olan tum Endpointler bu sinifa gelecek
            var disco = await _httpClient.GetDiscoveryDocumentAsync(
                new DiscoveryDocumentRequest
                {
                    Address = _serviceApiSettings.IdentityBaseUri,
                    Policy = new DiscoveryPolicy { RequireHttps = false }
                });

            if (disco.IsError)
            {
                throw disco.Exception;
            }

            var refreshToken = await _httpContextAccessor.HttpContext.GetTokenAsync(
                OpenIdConnectParameterNames.RefreshToken);

            TokenRevocationRequest tokenRevocationRequest = new()
            {
                ClientId = _clientSettings.WebClientForUser.ClientId,
                ClientSecret = _clientSettings.WebClientForUser.ClientSecret,
                Address=disco.RevocationEndpoint,
                Token = refreshToken,
                TokenTypeHint = "refresh_token"
            };

            await _httpClient.RevokeTokenAsync(tokenRevocationRequest);
        }

        public async Task<Response<bool>> SignIn(SigninInput signinInput)
        {

            // Var olan tum Endpointler bu sinifa gelecek
            var disco = await _httpClient.GetDiscoveryDocumentAsync(
                new DiscoveryDocumentRequest
                {
                    Address = _serviceApiSettings.IdentityBaseUri,
                    Policy = new DiscoveryPolicy { RequireHttps = false}
                });
            
            if (disco.IsError)
            {
                throw disco.Exception;
            }

            // password token create et
            var passwordTokenRequest = new PasswordTokenRequest
            {
                ClientId = _clientSettings.WebClientForUser.ClientId,
                ClientSecret = _clientSettings.WebClientForUser.ClientSecret,
                UserName = signinInput.Email,
                Password = signinInput.Password,
                Address=disco.TokenEndpoint
            };

            // create edilen passwordtokenrequest gonderilir
            var token = await _httpClient.RequestPasswordTokenAsync(passwordTokenRequest);

            if (token.IsError)
            {
                var responseContent = await token.HttpResponse.Content.ReadAsStringAsync();
                var errorDto = JsonSerializer.Deserialize<ErrorDto>( responseContent, 
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                return Response<bool>.Fail(errorDto.Errors, 400);
            }

            // userInfoRequest create edilir
            var userInfoRequest = new UserInfoRequest
            {
                Token = token.AccessToken,
                Address=disco.UserInfoEndpoint
            };

            // userInfoRequest kullanilarak userInfo create edilir
            var userInfo = await _httpClient.GetUserInfoAsync(userInfoRequest);

            if (userInfo.IsError)
            {
                throw userInfo.Exception;
            }

            // userInfo Claimsler kullanilarak ClaimsIdentity create edilir
            ClaimsIdentity claimsIdentity = new ClaimsIdentity(userInfo.Claims, CookieAuthenticationDefaults.AuthenticationScheme, "name", "roles");

            // claimsIdentity kullanilarak ClaimsPrincipal create edilir
            ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            // tokenler Cookie altina save edilir
            var authenticationProperties = new AuthenticationProperties();

            authenticationProperties.StoreTokens(new List<AuthenticationToken>()
            {
                new AuthenticationToken{ Name=OpenIdConnectParameterNames.AccessToken,
                    Value=token.AccessToken},
                new AuthenticationToken{ Name=OpenIdConnectParameterNames.RefreshToken,
                    Value=token.RefreshToken},
                new AuthenticationToken { Name=OpenIdConnectParameterNames.ExpiresIn,
                    Value=DateTime.Now.AddSeconds(token.ExpiresIn).ToString("o",CultureInfo.InvariantCulture)}
            });

            // Kullanici Remember secmis ise cookie kalici halde save edilir
            authenticationProperties.IsPersistent = signinInput.IsRemember;

            // Signin yapilir
            await _httpContextAccessor.HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme, 
                claimsPrincipal,
                authenticationProperties);

            return Response<bool>.Success(200);
        }
    }
}
