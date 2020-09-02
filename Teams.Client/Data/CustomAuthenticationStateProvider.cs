using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity;
using Blazored.LocalStorage;
using Teams.Models.Models;

namespace Teams.Client.Data
{
    public class CustomAuthenticationStateProvider : AuthenticationStateProvider
    {
        public ILocalStorageService _localStorageService { get; }
        private readonly HttpClient _httpClient;
        private StateManagenment _state;
        private TeamsService _service;

        public CustomAuthenticationStateProvider(ILocalStorageService localStorageService,
            HttpClient httpClient,StateManagenment state, TeamsService service)
        {
            //throw new Exception("CustomAuthenticationStateProviderException");
            _localStorageService = localStorageService;
            _httpClient = httpClient;
            _state = state;
            _service = service;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var accessToken = await _localStorageService.GetItemAsync<string>("accessToken");

            ClaimsIdentity identity = new ClaimsIdentity();
            var claimsPrincipal = new ClaimsPrincipal();
            if (accessToken != null && accessToken != string.Empty)
            {
                AuthenticateResponseModel user = await _service.GetUserByAccessTokenAsync(accessToken);
                if (user != null)
                {
                    _state.userState = user;
                    identity = GetClaimsIdentity(user);
                    claimsPrincipal = new ClaimsPrincipal(identity);
                }
            }
            return await Task.FromResult(new AuthenticationState(claimsPrincipal));
        }

        public async Task MarkUserAsAuthenticated(AuthenticateResponseModel user)
        {
            await _localStorageService.SetItemAsync("accessToken", user.Token);
            await _localStorageService.SetItemAsync("refreshToken", user.Token);
            _state.userState = user;
            var identity = GetClaimsIdentity(user);

            var claimsPrincipal = new ClaimsPrincipal(identity);

            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(claimsPrincipal)));
        }

        public async Task MarkUserAsLoggedOut()
        {
            await _localStorageService.RemoveItemAsync("refreshToken");
            await _localStorageService.RemoveItemAsync("accessToken");
            _state.userState = null;
            var identity = new ClaimsIdentity();

            var user = new ClaimsPrincipal(identity);

            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
        }

        private ClaimsIdentity GetClaimsIdentity(AuthenticateResponseModel user)
        {
            var claimsIdentity = new ClaimsIdentity();

            if (user.Username != null)
            {
                claimsIdentity = new ClaimsIdentity(new[]
                                {
                                    new Claim(ClaimTypes.Name, user.Username)
                                }, "apiauth_type");
            }

            return claimsIdentity;
        }
    }
}