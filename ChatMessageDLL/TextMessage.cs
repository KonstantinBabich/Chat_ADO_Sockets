using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatMessageDLL
{
	/// <summary>
	/// Класс текстового сообщения
	/// </summary>
	[Serializable]
	public class TextMessage: IMessage
	{
		public string From { get; set; }
		public string To { get; set; }
		public bool IsPrivate { get; set; }
		public string Text { get; set; }
		public DateTime Date { get; set; }

		public TextMessage(string from, string to, bool isPrivate, string text)
		{
			From = from;
			To = to;
			IsPrivate = isPrivate;
			Text = text;
			Date = DateTime.Now;
		}
	}
}
