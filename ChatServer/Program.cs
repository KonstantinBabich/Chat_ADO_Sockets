using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using ChatMessageDLL;
using System.Runtime.InteropServices;


namespace ChatServer
{
	class Program
	{
		static private Server server;
		static ConsoleEventDelegate handler;   // Keeps it from getting garbage collected
											   // Pinvoke
		private delegate bool ConsoleEventDelegate(int eventType);
		[DllImport("kernel32.dll", SetLastError = true)]
		private static extern bool SetConsoleCtrlHandler(ConsoleEventDelegate callback, bool add);

		static void Main(string[] args)
		{
			handler = new ConsoleEventDelegate(ConsoleEventCallback);
			SetConsoleCtrlHandler(handler, true);

			server = new Server();
			server.Process();
		}

		static bool ConsoleEventCallback(int eventType)
		{
			if (eventType == 2)
				server.ClosePort();
			return false;
		}
	}
}
