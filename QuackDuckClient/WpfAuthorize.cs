using JetBrains.Annotations;
using System.Threading.Tasks;
using VkNet.Abstractions.Authorization;
using VkNet.Abstractions.Core;
using VkNet.Enums.SafetyEnums;
using VkNet.Model;
using VkNet.Utils;
using Url = Flurl.Url;

namespace QuackDuckClient
{
	/// <summary>
	/// Class
	/// </summary>
	// Attribute tells IDE to not raise warnings becuase this class is unused. (Used by external library)
    [UsedImplicitly]
    public class WpfAuthorize : IAuthorizationFlow
    {
        private readonly IVkApiVersionManager _versionManager;
        private IApiAuthParams _authParams;

        public WpfAuthorize(IVkApiVersionManager versionManager)
        {
            _versionManager = versionManager;
        }

		/// <summary>
		/// Uses page as browser and provides VK authorization window for user
		/// </summary>
		/// <returns>Authorization result data</returns>
        public Task<AuthorizationResult> AuthorizeAsync()
        {
			// Call dialog with auth form
			var dlg = new AuthForm();

			// Using window as webbrowser and navigating to url (first argument)
			dlg.WebBrowser.Navigate(
				CreateAuthorizeUrl(_authParams.ApplicationId, _authParams.Settings.ToUInt64(), Display.Mobile, "123456"),
				null,
				null,
				"User-Agent: CustomUserAgent");

			dlg.WebBrowser.Navigated += (sender, args) =>
			{
				// Disables js in browser
				dlg.WebBrowser.SetSilent();
				var result = VkAuthorization2.From(args.Uri.AbsoluteUri); // Gets information from result URL by parcing it

				if (!result.IsAuthorized)
					return;

				// Parce result data into dlg.Auth
				dlg.Auth = new AuthorizationResult
				{
					AccessToken = result.AccessToken,
					ExpiresIn = result.ExpiresIn,
					UserId = result.UserId,
				};

				dlg.Close();
			};

			dlg.ShowDialog();

			return Task.FromResult(dlg.Auth);
		}

		public void SetAuthorizationParams(IApiAuthParams authorizationParams)
		{
			_authParams = authorizationParams;
		}

		/// <summary>
		/// Generates url with parameters
		/// </summary>
		/// <returns>Implicit flow authorization URL with parameters</returns>
		public Url CreateAuthorizeUrl()
		{
			var url = new Url("https://oauth.vk.com/authorize")
				.SetQueryParam("client_id", _authParams.ApplicationId) // App id 
				// Do not change. Url set due to the terms of service of VK for standalone apps
				.SetQueryParam("redirect_uri", "https://oauth.vk.com/blank.html") 
				// Do not change. Display variable sets the version of front-end of auth page
				.SetQueryParam("display", Display.Mobile) 
				// Scope defines VK user's information accessible in app
				.SetQueryParam("scope", _authParams.Settings.ToUInt64())
				// Defines what will be the response after OAuth2
				.SetQueryParam("response_type", "token")
				.SetQueryParam("v", _versionManager.Version)
				// If 1, ask for authorize everytime. When 0, app should pass this step 
				.SetQueryParam("revoke", "1");

			return url;
		}

		/// <summary>
		/// Translates Url to Uri
		/// </summary>
		/// <returns>Uri</returns>
		public Url CreateAuthorizeUrl(ulong clientId, ulong scope, Display display, string state)
		{
			return CreateAuthorizeUrl().ToUri();
		}
	}
}
