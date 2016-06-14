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
	/// Класс, хранящий данные SQL-параметра
	/// </summary>
	/// <typeparam name="T"></typeparam>
	[Serializable]
	public class Parameter<T>
	{
		public string Name { get; }
		public SqlDbType DbType { get; }
		public int Size { get; }
		public ParameterDirection Direction { get; }
		public T Value { get; }

		public Parameter(string name, SqlDbType dbType, object value, int size, ParameterDirection direction)
		{
			Name = name;
			DbType = dbType;
			Value = (T)value;
			Size = size;
			Direction = direction;
		}

		public Parameter(SqlParameter param)
		{
			Name = param.ParameterName;
			DbType = param.SqlDbType;
			Value = (T)param.Value;
			Size = param.Size;
			Direction = param.Direction;
		}

		/// <summary>
		/// Возвращает SQL-параметр
		/// </summary>
		/// <returns></returns>
		public SqlParameter ToSqlParameter()
		{
			SqlParameter param = new SqlParameter(Name, DbType, Size)
			{
				Direction = Direction
			};

			param.Value = Value;

			return param;
		}
	}
}
