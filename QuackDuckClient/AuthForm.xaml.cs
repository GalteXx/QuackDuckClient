using System.Windows;
using VkNet.Model;

namespace QuackDuckClient
{
    /// <summary>
    /// Interaction logic for AuthForm.xaml
    /// </summary>
    public partial class AuthForm : Window
	{
		public AuthorizationResult Auth { get; set; }

		public AuthForm()
		{
			InitializeComponent();
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			Close();
		}
	}
}
