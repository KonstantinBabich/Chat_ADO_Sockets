using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;


namespace Chat
{
	/// <summary>
	/// Логика взаимодействия для LoginWindow.xaml
	/// </summary>
	public partial class LoginWindow : Window
	{
		private ServerConnection server;
		private bool isLoginSuccess = false;

		public LoginWindow()
		{
			System.Threading.Thread.CurrentThread.CurrentUICulture = 
				CultureInfo.GetCultureInfo(Properties.Settings.Default.Language.IetfLanguageTag);

			InitializeComponent();
			CheckCurrentLang();

			SetLoginData();

			server = new ServerConnection();
		}

		private void SetLoginData()
		{
			FileInfo file = new FileInfo("data.b");
			try
			{
				using (BinaryReader br = new BinaryReader(file.Open(FileMode.Open, FileAccess.Read)))
				{
					bool flag = br.ReadBoolean();
					if (!flag)
						return;

					keepLoginBox.IsChecked = true;
					LoginBox.Text = br.ReadString();
					PassBox.Password = br.ReadString();
				}
			}
			catch (Exception exc)
			{
				
			}
		}

		private void SaveLoginData()
		{
			FileInfo file = new FileInfo("data.b");
			try
			{
				using (BinaryWriter bw = new BinaryWriter(file.Open(FileMode.Create, FileAccess.Write)))
				{
					bw.Write(keepLoginBox.IsChecked.Value);
					bw.Write(LoginBox.Text);
					bw.Write(PassBox.Password);
				}
			}
			catch (Exception exc)
			{

			}
		}

		private void EnglishMI_Click(object sender, RoutedEventArgs e)
		{
			SwitchLang("en");
		}

		private void RussianMI_Click(object sender, RoutedEventArgs e)
		{
			SwitchLang("ru");
		}

		private void UkrainianMI_Click(object sender, RoutedEventArgs e)
		{
			SwitchLang("uk");
		}

		private void GermanMI_Click(object sender, RoutedEventArgs e)
		{
			SwitchLang("de");
		}

		private void SwitchLang(string newLang)
		{
			Properties.Settings.Default.Language = new CultureInfo(newLang);
			Properties.Settings.Default.Save();

			EnglishMI.IsChecked = newLang == "en";
			UkrainianMI.IsChecked = newLang == "uk";
			RussianMI.IsChecked = newLang == "ru";
			GermanMI.IsChecked = newLang == "de";

			var answer = MessageBox.Show(Chat.Resources.Language.ChangeLanguageRequest, Chat.Resources.Language.WarningCaption,
				MessageBoxButton.YesNo, MessageBoxImage.Question);
			if (answer == MessageBoxResult.Yes)
			{
				System.Windows.Forms.Application.Restart();
				System.Windows.Application.Current.Shutdown();
			}
		}

		private void CheckCurrentLang()
		{
			switch (Properties.Settings.Default.Language.IetfLanguageTag)
			{
				case "ru":
					RussianMI.IsChecked = true;
					break;
				case "uk":
					UkrainianMI.IsChecked = true;
					break;
				case "de":
					GermanMI.IsChecked = true;
					break;
				default:
					EnglishMI.IsChecked = true;
					break;
			}
		}

		private void LoginButton_Click(object sender, RoutedEventArgs e)
		{
			Login();
		}

		private void Login()
		{
			if (LoginBox.Text.Length < 3)
			{
				MessageBox.Show(Chat.Resources.Language.ShortLoginError, Chat.Resources.Language.ErrorCaption);
				return;
			}
			if (PassBox.Password.Length < 6)
			{
				MessageBox.Show(Chat.Resources.Language.PassError, Chat.Resources.Language.ErrorCaption);
				return;
			}

            switch (server.LogIn(LoginBox.Text, PassBox.Password))
			{
				case LoginResult.AuthFail:
					MessageBox.Show(Chat.Resources.Language.LoginFail, Chat.Resources.Language.ErrorCaption);
					return;
				case LoginResult.UserAlreadyConnected:
					MessageBox.Show(Chat.Resources.Language.LoginFailAlreadyConnected, Chat.Resources.Language.ErrorCaption);
					return;
				case LoginResult.CannotConnect:
					MessageBox.Show(Chat.Resources.Language.ConnectionError, Chat.Resources.Language.ErrorCaption);
					return;
			}

			MainWindow window = new MainWindow(ref server, LoginBox.Text);
			window.Show();
			isLoginSuccess = true;
			Close();
		}

		private void RegistrationButton_Click(object sender, RoutedEventArgs e)
		{
			RegistrationWindow window = new RegistrationWindow(ref server) {Owner = this};
			window.ShowDialog();
		}

		private void Window_Closed(object sender, EventArgs e)
		{
			if (!isLoginSuccess)
				server.CloseConnection();

			SaveLoginData();
		}

		private void Window_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
			{
				Login();
			}
		}
	}
}
