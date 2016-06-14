using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatMessageDLL
{
	/// <summary>
	/// Интерфейс сообщения
	/// </summary>
	public interface IMessage
	{
		string From { get; set; }
		DateTime Date { get; set; }
	}
}
