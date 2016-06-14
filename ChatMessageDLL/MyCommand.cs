using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatMessageDLL
{
	/// <summary>
	/// Класс, хранящий необходимые данные SQL-команды
	/// </summary>
	[Serializable]
	public class MyCommand
	{
		public string CommandText { get; set; }
		public List<Parameter<object>> Parameters { get; set; }
		public CommandType ComType { get; set; }

		public MyCommand() { }

		public MyCommand(SqlCommand cmd)
		{
			CommandText = cmd.CommandText;
			ComType = cmd.CommandType;
			Parameters = new List<Parameter<object>>();
			foreach (SqlParameter parameter in cmd.Parameters)
			{
				Parameters.Add(new Parameter<object>(parameter));
			}
		}

		public MyCommand(string comText)
		{
			CommandText = comText;
		}

		/// <summary>
		/// Возвращает SQL-команду
		/// </summary>
		/// <returns></returns>
		public SqlCommand ToSqlCommand()
		{
			var cmd = new SqlCommand(CommandText) {CommandType = ComType};
			foreach (Parameter<object> parameter in Parameters)
			{
				SqlParameter param = parameter.ToSqlParameter();
				//cmd.Parameters.Add(param);
				cmd.Parameters.Add(parameter.ToSqlParameter());
			}

			return cmd;
		}
	}
}
