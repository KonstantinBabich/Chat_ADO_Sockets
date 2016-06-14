using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ChatMessageDLL;
using MessageBox = System.Windows.MessageBox;

namespace Chat
{
	/// <summary>
	/// Логика взаимодействия для RegistrationWindow.xaml
	/// </summary>
	public partial class RegistrationWindow : Window
	{
		private bool isClosing;
		private ServerConnection server;

		public RegistrationWindow(ref ServerConnection _server)
		{
			InitializeComponent();

			CheckCurrentLang();
			isClosing = false;
			PassBox1.MaxLength = 30;
			PassBox2.MaxLength = 30;
			server = _server;
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

		private void PassBox1_LostFocus(object sender, RoutedEventArgs e)
		{
			PassBox_LostFocus(sender as PasswordBox);
		}

		private void PassBox_LostFocus(PasswordBox pb)
		{
			if (isClosing)
				return;

			if (pb.Password.Length < 6)
			{
				MessageBox.Show(Chat.Resources.Language.ShortPassError, Chat.Resources.Language.ErrorCaption, 
					MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		private void PassBox2_LostFocus(object sender, RoutedEventArgs e)
		{
			PassBox_LostFocus(sender as PasswordBox);
		}

		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			isClosing = true;
		}

		private void RegButton_Click(object sender, RoutedEventArgs e)
		{
			Registration();
		}

		private void Registration()
		{
			if (!ValidationFields())
				return;

			if (server.Registration(LoginBox.Text, PassBox1.Password))
			{
				(Owner as LoginWindow).LoginBox.Text = LoginBox.Text;
				(Owner as LoginWindow).PassBox.Password = PassBox1.Password;
				Close();
			}
		}

		private bool ValidationFields()
		{
			if (PassBox1.Password.Length < 6 || PassBox2.Password.Length < 6)
			{
				MessageBox.Show(Chat.Resources.Language.ShortPassError, Chat.Resources.Language.ErrorCaption,
					MessageBoxButton.OK, MessageBoxImage.Error);
				return false;
			}

			if (PassBox1.Password != PassBox2.Password)
			{
				MessageBox.Show(Chat.Resources.Language.PassNoMatch, Chat.Resources.Language.ErrorCaption,
					MessageBoxButton.OK, MessageBoxImage.Error);
				return false;
			}

			if (LoginBox.Text.Length < 3)
			{
				MessageBox.Show(Chat.Resources.Language.ShortLoginError, Chat.Resources.Language.ErrorCaption,
					MessageBoxButton.OK, MessageBoxImage.Error);
				return false;
			}

			return true;
		}

		private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
			{
				Registration();
			}
		}
	}
}
