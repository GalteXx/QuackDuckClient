using System.Reflection;
using System.Windows.Controls;

namespace QuackDuckClient
{
	/// <summary>
	/// It will be a good idea to undestand what is happening here.
	/// </summary>
    public static class WebBrowserExtension
    {
		public static void SetSilent(this WebBrowser wb)
		{
			var fiComWebBrowser = typeof(WebBrowser)
				.GetField("_axIWebBrowser2",
					BindingFlags.Instance | BindingFlags.NonPublic);

			if (fiComWebBrowser == null)
			{
				return;
			}

			var objComWebBrowser = fiComWebBrowser.GetValue(wb);

			objComWebBrowser?.GetType()
				.InvokeMember("Silent",
					BindingFlags.SetProperty,
					null,
					objComWebBrowser,
					new object[]
					{
						true
					});
		}
	}
}
