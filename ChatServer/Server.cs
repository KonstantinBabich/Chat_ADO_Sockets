using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using ChatMessageDLL;
using NATUPNPLib;

namespace ChatServer
{
    class Server
    {
		/// <summary>
		/// Взаимодействие с базой данных
		/// </summary>
	    private DatabaseInteraction dataBase;
		/// <summary>
		/// Сокет для прослушки входящих подключений
		/// </summary>
        private Socket listener;
	    private Dictionary<string, Socket> clientsSockets; 
		/// <summary>
		/// Стандартный порт приложения
		/// </summary>
	    private const int DefaultPort = 12305;

	    private bool isServerRunning;
		private IStaticPortMappingCollection mappings;

		public Server()
        {
			dataBase = new DatabaseInteraction(false, "ChatDB");
			clientsSockets = new Dictionary<string, Socket>();
        }

		/// <summary>
		/// Старт работы сервера
		/// </summary>
	    public void Process()
	    {
			var s = dataBase.Connect();
			if (s != "true")
			{
				Console.WriteLine(s);
				Console.ReadLine();
				return;
			}

		    isServerRunning = true;

			//Если есть роутер
			if (!ForwardPort())
			{
				Console.WriteLine("Unable to forward the port. Please restart the router.");
				Console.ReadLine();
				return;
			}

			listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			// Определяем конечную точку, IPAddress.Any означает что наш сервер будет принимать входящие соединения с любых адресов
			IPEndPoint Point = new IPEndPoint(IPAddress.Any, DefaultPort);
			// Связываем сокет с конечной точкой
			listener.Bind(Point);
			// Начинаем слушать входящие соединения
			listener.Listen(10);
			Console.WriteLine("Server running now.");
			SocketAccepter();
		}

		/// <summary>
		/// Открыть порт
		/// </summary>
		private bool ForwardPort()
		{
			string host = Dns.GetHostName();
			IPAddress ip = Dns.GetHostByName(host).AddressList[0];
			UPnPNAT upnpnat = new UPnPNAT();
			mappings = upnpnat.StaticPortMappingCollection;
			try
			{
				mappings.Add(DefaultPort, "TCP", DefaultPort, ip.ToString(), true, "ChatServer port");
			}
			catch
			{
				return false;
			}

			return true;
		}

		/// <summary>
		/// Закрыть порт
		/// </summary>
		public void ClosePort()
		{
			mappings.Remove(DefaultPort, "TCP");
		}

		/// <summary>
		/// Подключение нового клиента
		/// </summary>
		private void SocketAccepter()
		{
			Thread th = new Thread(delegate ()
			{
				while (isServerRunning)
				{
					// Создаем новый сокет, по которому мы сможем обращаться клиенту
					try
					{
						Socket client = listener.Accept();
						// Начинаем принимать входящие пакеты
						Thread thh = new Thread(delegate()
						{
							ProcessConnection(client);
						});
						thh.Start();
					}
					catch (Exception) { }
				}
			});

			th.Start();
		}

		/// <summary>
		/// Обработка нового подключения
		/// </summary>
		/// <param name="client">Сокет-клиент</param>
		private void ProcessConnection(Socket client)
		{
			bool isClientRunning = true;
			string ip = IPAddress.Parse(((IPEndPoint)client.RemoteEndPoint).Address.ToString()).ToString();
            Console.WriteLine("New connection: " + ip);
	        while (isClientRunning)
	        {
		        try
		        {
			        byte[] bytes = new byte[2048];
			        client.Receive(bytes);

			        MemoryStream stream = new MemoryStream(bytes);
			        BinaryFormatter bf = new BinaryFormatter();

			        IMessage tMsg = (bf.Deserialize(stream) as IMessage);

			        if (tMsg is ServerMessage)
			        {
				        var msg = (ServerMessage) tMsg;
				        isClientRunning = ProcessServerMessage(client, msg);
			        }

			        if (tMsg is TextMessage)
			        {
				        var msg = (TextMessage) tMsg;
						ProcessTextMessage(msg);
			        }
		        }
		        catch (Exception exc)
		        {
			        isClientRunning = false;
					ClientDisconnected(client);
				}
	        }

			ClientDisconnected(client);
			Console.WriteLine("IP " + ip + " disconnected.");
		}

		private bool IsUserAlreadyConnected(string name)
		{
			foreach (string _name in clientsSockets.Keys)
			{
				if (name == _name)
					return true;
			}

			return false;
		}

		/// <summary>
		/// Обработка серверного сообщения
		/// </summary>
		/// <param name="client">Сокет-клиент</param>
		/// <param name="msg">Серверное сообщение</param>
		/// <returns></returns>
	    private bool ProcessServerMessage(Socket client, ServerMessage msg)
	    {
		    if (msg.Command.CommandText == "GetUserInfoByUsername")
		    {
				string name = msg.Command.Parameters.Find(x => x.Name == "@name").Value.ToString();
				if (IsUserAlreadyConnected(name))
				{
					msg.Command.CommandText = "Deny";
					SendMessage(client, msg);
					return false;
				}
				SqlCommand cmd = msg.Command.ToSqlCommand();
			    var reader = dataBase.ExecuteCommmand(cmd);
			    msg.Command = new MyCommand(cmd);
				SendMessage(client, msg);
				reader.Close();
			}

			if (msg.Command.CommandText == "LogInInfo")
			{
				SqlParameter pIP = new SqlParameter()
				{
					ParameterName = "@ip",
					Direction = ParameterDirection.Input,
					Value = IPAddress.Parse(((IPEndPoint)client.RemoteEndPoint).Address.ToString()).ToString()
				};
				msg.Command.Parameters.Add(new Parameter<object>(pIP));
				dataBase.ExecuteCommmand(msg.Command.ToSqlCommand()).Close();
				if ((int)msg.Command.Parameters[1].Value == 1)
					clientsSockets.Add(msg.From, client);
			}

			if (msg.Command.CommandText == "CloseConnection")
			{
				SqlCommand cmd = msg.Command.ToSqlCommand();
				cmd.Parameters.Add(new SqlParameter("@name", SqlDbType.VarChar, 20)
				{
					Direction = ParameterDirection.Input,
					Value = msg.From
				});
				cmd.Parameters.Add(new SqlParameter()
				{
					ParameterName = "@ip",
					Direction = ParameterDirection.Input,
					Value = IPAddress.Parse(((IPEndPoint) client.RemoteEndPoint).Address.ToString()).ToString()
				});
				dataBase.ExecuteCommmand(cmd).Close();
				ClientDisconnected(client);
				return false;
			}

		    if (msg.Command.CommandText == "AddNewUser")
		    {
			    SqlCommand cmd = msg.Command.ToSqlCommand();
			    try
			    {
				    dataBase.ExecuteCommmand(cmd).Close();
			    }
			    catch (SqlException)
			    {
				    msg.Command.CommandText = "LoginTaken";
			    }

				SendMessage(client, msg);
			}

			if (msg.Command.CommandText == "NewUserOnline")
			{
				SendBroadcastMessage(msg);
			}

			if (msg.Command.CommandText == "GetOnlineUsers")
			{
				foreach(string name in clientsSockets.Keys)
				{
					msg.Command.CommandText = "NewUserOnline";
					msg.From = name;
					SendMessage(client, msg);
				}
			}

		    return true;
	    }

		/// <summary>
		/// Обработка текстового сообщения
		/// </summary>
		/// <param name="msg">Текстовое сообщение</param>
	    private void ProcessTextMessage(TextMessage msg)
	    {
			SqlCommand cmd = new SqlCommand("AddNewMessage");
			cmd.CommandType = CommandType.StoredProcedure;
		    cmd.Parameters.Add(new SqlParameter("@from", SqlDbType.VarChar, 20) {Direction = ParameterDirection.Input, Value = msg.From});
			cmd.Parameters.Add(new SqlParameter("@to", SqlDbType.VarChar, 50) { Direction = ParameterDirection.Input, Value = msg.To });
			cmd.Parameters.Add(new SqlParameter("@text", SqlDbType.VarChar, 255) { Direction = ParameterDirection.Input, Value = msg.Text});
			dataBase.ExecuteCommmand(cmd).Close();

			SendBroadcastMessage(msg);
	    }

		/// <summary>
		/// Отправка сообщения определенному пользователю
		/// </summary>
		/// <param name="client">Сокет-клиент</param>
		/// <param name="msg">Сообщение</param>
	    private void SendMessage(Socket client, IMessage msg)
	    {
			var stream = new MemoryStream();
			var bf = new BinaryFormatter();
			bf.Serialize(stream, msg);
			client.Send(stream.ToArray());
		}

		/// <summary>
		/// Отправка сообщения всем подключенным пользователям
		/// </summary>
		/// <param name="msg"></param>
		private void SendBroadcastMessage(IMessage msg)
		{
			var stream = new MemoryStream();
			var bf = new BinaryFormatter();
			bf.Serialize(stream, msg);
			foreach (Socket client in clientsSockets.Values)
			{
				client.Send(stream.ToArray());
			}
		}

		/// <summary>
		/// Отключение подключенного клиента
		/// </summary>
		/// <param name="client">Сокет-клиент</param>
		private void ClientDisconnected(Socket client)
		{
			string key;
			try
			{
				client.Disconnect(true);
				key = clientsSockets.First(x => x.Value == client).Key;
				clientsSockets.Remove(key);
			} catch (Exception)
			{
				return;
			}
			ServerMessage msg = new ServerMessage();
			msg.Command = new MyCommand("UserDisconnected");
			msg.From = key;
			SendBroadcastMessage(msg);
		}
    }
}
