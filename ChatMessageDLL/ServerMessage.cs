using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatMessageDLL
{
	/// <summary>
	/// Класс сообщения, с командой или информацией для сервера
	/// </summary>
	[Serializable]
	public class ServerMessage : IMessage
	{
		public string From { get; set; }
		public MyCommand Command { get; set; }
		public DateTime Date { get; set; }

		public ServerMessage(string from, SqlCommand cmd)
		{
			From = from;
			Command = new MyCommand(cmd);
			Date = DateTime.Now;
		}

		public ServerMessage() { }
	}
}
