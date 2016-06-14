using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ChatMessageDLL;
using System.Windows.Media.Effects;

namespace Chat
{
	/// <summary>
	/// Логика взаимодействия для MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private ServerConnection server;
		private bool isRunning = true;
		private string username;

		public MainWindow(ref ServerConnection _server, string uname)
		{
			InitializeComponent();
			
			CheckCurrentLang();
			server = _server;
			username = uname;
			chatPanel.Orientation = Orientation.Vertical;
			usersList.Items.Add(username);

			MessageHandler();

			messageBox.Focus();
			ServerMessage msg = new ServerMessage();
			msg.From = username;
			msg.Command = new MyCommand("NewUserOnline");
			server.SendMessage(msg);

			Thread.Sleep(10);	//Костыль для след. команды
			server.GetOnlineUsers();
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

		private void textBox_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key != Key.Enter || (e.Key == Key.Enter && messageBox.Text.Length == 0))
				return;
			if (e.Key == Key.Enter && Keyboard.Modifiers == ModifierKeys.Control)
			{
				messageBox.Text += "\n";
				messageBox.CaretIndex = messageBox.Text.Length;
				return;
			}

			TextMessage msg = new TextMessage(username, "mainRoom", false, messageBox.Text);
			server.SendMessage(msg);
			messageBox.Text = "";
		}

		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			isRunning = false;
			server.CloseConnection();
		}

		private void MessageHandler()
		{
			Thread th = new Thread(delegate ()
			{
				while (isRunning)
				{
					var _msg = server.MessagesReciever();
					Dispatcher.BeginInvoke(new ThreadStart(delegate
					{
						if (_msg == null)
							return;

						if (_msg is TextMessage)
						{
							TextMessage msg = (TextMessage)_msg;
							chatPanel.Children.Add(CreateMessageBlock(msg));
							scrollViewer.ScrollToBottom();
						}

						if (_msg is ServerMessage)
						{
							ServerMessage msg = (ServerMessage)_msg;
							ProcessServerMessage(msg);
						}
					}));
				}
			});

			th.Start();
		}

		private StackPanel CreateMessageBlock(TextMessage msg)
		{
			StackPanel panel = new StackPanel()
			{
				Orientation = Orientation.Vertical,
				Margin = new Thickness(1)
			};
			Label nameLabel = new Label()
			{
				Content = msg.From,
				Margin = new Thickness(5, 0, 5, -5),
				FontSize = 10,
				FontWeight = FontWeights.SemiBold,
				Foreground = username == msg.From ? Brushes.Blue : Brushes.Black
			};
			Label timeLabel = new Label()
			{
				Content = $"{msg.Date.ToShortDateString()}\n{msg.Date.ToShortTimeString()}",
				Margin = new Thickness(1),
				FontSize = 10,
			};
			TextBlock block = new TextBlock()
			{
				Text = msg.Text,
				FontSize = 12,
				MinWidth = 100,
				MaxWidth = 300,
				TextWrapping = TextWrapping.Wrap,
				Margin = new Thickness(5),
			};
			Border borderShadow = new Border()
			{
				CornerRadius = new CornerRadius(4),
				BorderBrush = Brushes.Black,
				BorderThickness = new Thickness(1),
				HorizontalAlignment = HorizontalAlignment.Stretch,
				MinWidth = 100,
				MaxWidth = 300,
				Background = Brushes.LemonChiffon,
				Effect = new DropShadowEffect()
				{
					Color = new Color() { R = 0, G = 0, B = 0, A = 0},
					Opacity = 1,
					ShadowDepth = 2
				}
			};
			Border border = new Border()
			{
				CornerRadius = new CornerRadius(4),
				BorderBrush = Brushes.Black,
				BorderThickness = new Thickness(1),
				HorizontalAlignment = HorizontalAlignment.Stretch,
				MinWidth = 100,
				MaxWidth = 300,
			};
			border.Child = block;
            StackPanel horPanel = new StackPanel()
			{
				Orientation = Orientation.Horizontal,
				HorizontalAlignment = HorizontalAlignment.Stretch,
				Margin = new Thickness(1)
			};
			Grid grid = new Grid();
			grid.Children.Add(borderShadow);
			grid.Children.Add(border);
			horPanel.Children.Add(grid);
			horPanel.Children.Add(timeLabel);

			panel.Children.Add(nameLabel);
			panel.Children.Add(horPanel);
			return panel;
		}

		private void ProcessServerMessage(ServerMessage msg)
		{
			if (msg.Command.CommandText == "NewUserOnline")
			{
				if (msg.From == username)
					return;

				usersList.Items.Add(msg.From);
			}

			if (msg.Command.CommandText == "UserDisconnected")
			{
				usersList.Items.Remove(msg.From);
			}
		}
	}
}
