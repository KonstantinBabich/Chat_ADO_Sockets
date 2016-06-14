using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using ChatMessageDLL;
using NATUPNPLib;

namespace Chat
{
	internal enum LoginResult
	{
		CannotConnect = -1, Success, UserAlreadyConnected, AuthFail
	}

	public class ServerConnection
	{
		private Socket socket;
		public bool ConnectionState { get; private set; }
		//private const string ip = "128.0.174.199";  //IP адрес сервера
		private const string ip = "localhost";
		private int DefaultPort = 12305;	//Порт приложения
		private string username;	//Имя пользователя
		private IStaticPortMappingCollection mappings;	//Для открытия портов

		/// <summary>
		/// Подключение к серверу
		/// </summary>
		public bool Connect()
		{
			if (ConnectionState)
				return false;

			socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

			try
			{
				socket.Connect(ip, DefaultPort);
				ConnectionState = true;
			}
			catch (Exception exc)
			{
				return false;
			}
			return true;
		}

		/// <summary>
		/// Открыть порт на роутере
		/// </summary>
		private bool ForwardPort()
		{
			try
			{
				socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

				socket.Connect(ip, DefaultPort);
				socket.Close();
				return true;
			}
			catch
			{
				string host = Dns.GetHostName();
				IPAddress ip = Dns.GetHostByName(host).AddressList[0];
				UPnPNAT upnpnat = new UPnPNAT();
				mappings = upnpnat.StaticPortMappingCollection;
				try
				{
					mappings.Add(DefaultPort, "TCP", DefaultPort, ip.ToString(), true, "Chat port");
				}
				catch
				{
					return false;
				}
			}
			
			return true;
		}

		/// <summary>
		/// Закрыть открытый порт
		/// </summary>
		public void ClosePort()
		{
			mappings.Remove(DefaultPort, "TCP");
		}

		/// <summary>
		/// Отправить сообщение серверу
		/// </summary>
		/// <param name="message">Сообщение</param>
		public void SendMessage(IMessage message)
		{
			if (!ConnectionState)
				return;

			BinaryFormatter bf = new BinaryFormatter();
			MemoryStream stream = new MemoryStream();
			bf.Serialize(stream, message);
			socket.Send(stream.ToArray());
		}

		/// <summary>
		/// Залогиниться
		/// </summary>
		/// <param name="name">Имя пользователя</param>
		/// <param name="pass">Пароль</param>
		/// <returns></returns>
		internal LoginResult LogIn(string name, string pass)
		{
			if (!ConnectionState)
				if (!Connect())
					return LoginResult.CannotConnect;

			LoginResult result = LoginResult.AuthFail;
			username = name;

			SqlCommand cmd = new SqlCommand();
			cmd.CommandType = CommandType.StoredProcedure;
			cmd.CommandText = "GetUserInfoByUsername";
			SqlParameter pName = new SqlParameter("@name", SqlDbType.VarChar, 20);
			SqlParameter pPass = new SqlParameter("@pass", SqlDbType.VarBinary, 64);
			SqlParameter pSalt = new SqlParameter("@salt", SqlDbType.VarChar, 40);
			pName.Direction = ParameterDirection.Input;
			pPass.Direction = ParameterDirection.Output;
			pSalt.Direction = ParameterDirection.Output;

			pName.Value = name;
			cmd.Parameters.AddRange(new[] { pName, pPass, pSalt });

			ServerMessage message = new ServerMessage(name, cmd);
			SendMessage(message);

			byte[] bytes = new byte[2048];
			while (true)
			{
				socket.Receive(bytes);

				MemoryStream stream = new MemoryStream(bytes);
				BinaryFormatter bf = new BinaryFormatter();
				message = (bf.Deserialize(stream) as ServerMessage);
				break;
			}

			pPass = message.Command.Parameters.Find(ob => ob.Name == "@pass").ToSqlParameter();
			pSalt = message.Command.Parameters.Find(ob => ob.Name == "@salt").ToSqlParameter();
			cmd.Parameters.Clear();

			SqlCommand cmdLogin = new SqlCommand
			{
				CommandText = "LogInInfo",
				CommandType = CommandType.StoredProcedure
			};
			cmdLogin.Parameters.Add(pName);

			if (message.Command.CommandText == "Deny")
			{
				cmdLogin.Parameters.Add(new SqlParameter("@isSuccess", SqlDbType.Int) { Value = 2 });
				SendMessage(message);
				return LoginResult.UserAlreadyConnected;
			}

			if (pSalt.Value.ToString().Length >= 40)
			{
				byte[] hash;
				byte[] data = Encoding.UTF8.GetBytes(pass + pSalt.Value);

				using (SHA512 shaM = new SHA512Managed())
				{
					hash = shaM.ComputeHash(data);
				}

				if (CompareHash(hash, pPass.Value as byte[]))
				{
					cmdLogin.Parameters.Add(new SqlParameter("@isSuccess", SqlDbType.Int) {Value = 1});
					result = LoginResult.Success;
				}
				else
				{
					cmdLogin.Parameters.Add(new SqlParameter("@isSuccess", SqlDbType.Int) {Value = 0});
				}

				message = new ServerMessage(name, cmdLogin);
				SendMessage(message);
			}

			return result;
		}

		/// <summary>
		/// Регистрация нового пользователя
		/// </summary>
		/// <param name="name">Имя пользователя</param>
		/// <param name="password">Пароль</param>
		/// <returns></returns>
		public bool Registration(string name, string password)
		{
			if (!ConnectionState)
			{
				Connect();
			}

			username = name;

			SqlCommand cmd = new SqlCommand();
			cmd.CommandType = CommandType.StoredProcedure;
			cmd.CommandText = "AddNewUser";
			SqlParameter pName = new SqlParameter("@name", SqlDbType.VarChar, 20);
			SqlParameter pPass = new SqlParameter("@pass", SqlDbType.VarBinary, 64);
			SqlParameter pSalt = new SqlParameter("@salt", SqlDbType.VarChar, 40);
			pName.Direction = ParameterDirection.Input;
			pPass.Direction = ParameterDirection.Input;
			pSalt.Direction = ParameterDirection.Input;
			pName.Value = username;
			pSalt.Value = GetRandomSalt(40);

			byte[] hash;
			byte[] data = Encoding.UTF8.GetBytes(password + pSalt.Value);

			using (SHA512 shaM = new SHA512Managed())
			{
				hash = shaM.ComputeHash(data);
			}
			pPass.Value = hash;

			cmd.Parameters.AddRange(new[] { pName, pPass, pSalt });

			ServerMessage msg = new ServerMessage("", cmd);
			SendMessage(msg);

			byte[] bytes = new byte[2048];
			while (true)
			{
				if (socket.Receive(bytes) < 1)
					continue;

				MemoryStream stream = new MemoryStream(bytes);
				BinaryFormatter bf = new BinaryFormatter();
				msg = (bf.Deserialize(stream) as ServerMessage);
				break;
			}

			if (msg.Command.CommandText == "LoginTaken")
			{
				MessageBox.Show(Chat.Resources.Language.LoginTaken, Chat.Resources.Language.ErrorCaption,
					MessageBoxButton.OK, MessageBoxImage.Error);
				return false;
			}

			MessageBox.Show(Chat.Resources.Language.RegComplete, Chat.Resources.Language.OkCaption,
				MessageBoxButton.OK, MessageBoxImage.Information);
			return true;
		}

		/// <summary>
		/// Сравнение двух хешей
		/// </summary>
		/// <param name="first">Первый массив</param>
		/// <param name="second">Второй массив</param>
		/// <returns></returns>
		private bool CompareHash(byte[] first, byte[] second)
		{
			if (first.Length != second.Length)
				return false;

			for (int i = 0; i < first.Length; i++)
			{
				if (first[i] != second[i])
					return false;
			}

			return true;
		}

		/// <summary>
		/// Закрыть подключение
		/// </summary>
		public void CloseConnection()
		{
			if (socket == null)
				return;

			ServerMessage msg = new ServerMessage(username, new SqlCommand("CloseConnection") { CommandType = CommandType.StoredProcedure} );
			SendMessage(msg);
			//socket.Disconnect(true);
			ConnectionState = false;
		}

		/// <summary>
		/// Получить случайную "соль"
		/// </summary>
		/// <param name="length"></param>
		/// <returns></returns>
		private string GetRandomSalt(int length)
		{
			string result = "";

			Random rand = new Random();
			for (int i = 0; i < length; i++)
			{
				char c = Convert.ToChar(rand.Next(48, 122));
				if (c == '\\' || c == '\'')
				{
					i--;
					continue;
				}
				result += c;
			}

			return result;
		}

		/// <summary>
		/// Получение сообщение от сервера
		/// </summary>
		/// <returns></returns>
		public IMessage MessagesReciever()
		{
			while (ConnectionState)
			{
				try
				{
					byte[] bytes = new byte[2048];
					if (socket.Receive(bytes) < 1)
						continue;

					MemoryStream stream = new MemoryStream(bytes);
					BinaryFormatter bf = new BinaryFormatter();

					var msg = (bf.Deserialize(stream) as IMessage);
					return msg;
				}
				catch (Exception)
				{
				}
			}

			return null;
		}

		/// <summary>
		/// Получение списка активных пользователей
		/// </summary>
		public void GetOnlineUsers()
		{
			ServerMessage msg = new ServerMessage()
			{
				From = username,
				Command = new MyCommand("GetOnlineUsers")
			};
			SendMessage(msg);
		}
	}
}
