using System;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;

namespace ChatServer
{
	public class DatabaseInteraction
	{
		public SqlConnection Connection { get; set; }
		public SqlCommand Command { get; set; }

		private string connectionString;

		/// <param name="ExpressType">Является ли сервер Express-версией</param>
		/// <param name="databaseName">Имя базы данных</param>
		/// <returns></returns>
		public DatabaseInteraction(bool ExpressType, string databaseName)
		{
			connectionString = 
				string.Format("server=localhost{0}; integrated security=true; database={1}",
				ExpressType ? "\\SQLEXPRESS" : "", databaseName);
		}

		/// <summary>
		/// Подключение к базе данных
		/// </summary>
		public string Connect()
		{
			try
			{
				Connection = new SqlConnection(connectionString);
				Connection.Open();
			}
			catch (SqlException exc)
			{
				foreach (SqlError error in exc.Errors)
					if (error.Message.IndexOf("Cannot open database", StringComparison.Ordinal) > -1)
					{
						CreateDatabase(Connection.Database);
						return "true";
					}

				Connection = null;
				return exc.Message;
			}

			return "true";
		}

		private void CreateDatabase(string databaseName)
		{
			Connection = new SqlConnection(connectionString);
			Connection.ConnectionString = Connection.ConnectionString.Replace(databaseName, "master");
			Connection.Open();

			string query = $"CREATE DATABASE {databaseName}";
			SqlCommand cmd = new SqlCommand(query, Connection);
			cmd.ExecuteNonQuery();
			Connection.ChangeDatabase(databaseName);
			cmd.CommandText = "CREATE TABLE Users(Username varchar(20) PRIMARY KEY, Pass varbinary(64) NOT NULL, Salt varchar(40) NOT NULL)";
			cmd.ExecuteNonQuery();
			cmd.CommandText = "CREATE TABLE LogTable(ID INT IDENTITY(0, 1) PRIMARY KEY, ActionType VARCHAR(50) NOT NULL, " +
			                  "ActionDate DATETIME NOT NULL, Username varchar(20) NOT NULL, AdressIP varchar(15) NOT NULL)";
			cmd.ExecuteNonQuery();
			cmd.CommandText = "CREATE TABLE RoomsMessages(NameFrom VARCHAR(20) NOT NULL, NameTo VARCHAR(50) NOT NULL, " +
			                  "MessageText VARCHAR(255) NOT NULL, MessageDate DATETIME NOT NULL)";
			cmd.ExecuteNonQuery();
			cmd.CommandText = "CREATE PROC GetUserInfoByUsername @name varchar(20), @pass varbinary(64) OUTPUT, @salt varchar(40) OUTPUT AS " +
			                  "SELECT @pass = Pass, @salt = Salt FROM Users WHERE @name = Username";
			cmd.ExecuteNonQuery();
			cmd.CommandText = "CREATE PROC AddNewUser @name varchar(20), @pass varbinary(64), @salt varchar(40) AS " +
			                  "INSERT INTO Users VALUES(@name, @pass, @salt)";
			cmd.ExecuteNonQuery();
			cmd.CommandText = "CREATE PROC LogInInfo @name varchar(20), @ip varchar(15), @isSuccess bit AS DECLARE @aType VARCHAR(50)" +
			                  " if @isSuccess = 0 SELECT @aType = 'Login fail' else if @isSuccess = 1 SELECT @aType = 'Login succesfull' " +
			                  "else SELECT @aType = 'User already in chat' INSERT INTO LogTable VALUES (@aType, GETDATE(), @name, @ip)";
			cmd.ExecuteNonQuery();
			cmd.CommandText = "CREATE PROC CloseConnection @name varchar(20), @ip varchar(15) AS " +
			                  "INSERT INTO LogTable VALUES ('Closed connection', GETDATE(), @name, @ip)";
			cmd.ExecuteNonQuery();
			cmd.CommandText = "CREATE PROC AddNewMessage @from varchar(20), @to varchar(50), @text varchar(255) AS " +
			                  "INSERT INTO RoomsMessages VALUES(@from, @to, @text, GETDATE())";
			cmd.ExecuteNonQuery();
		}

		public void Close()
		{
			Connection.Close();
		}

		/// <summary>
		/// Процедура аутентификации пользователя
		/// </summary>
		/// <param name="login">Логин</param>
		/// <param name="password">Пароль</param>
		/// <param name="targPass">Хэш пароля</param>
		/// <param name="salt">'Соль'</param>
		/// <returns>0 - успешная аутентификация, 1 - нет подключения к БД, 2 - Не верный пароль</returns>
		public int LogIn(string login, string password, byte[] targPass, string salt)
		{
			if (Connection.State != ConnectionState.Open)
				return 1; // Нет подключения к БД

			byte[] inpPass = Encoding.UTF8.GetBytes(password + salt);

			using (SHA512 shaM = new SHA512Managed())
			{
				inpPass = shaM.ComputeHash(inpPass);
			}

			if (inpPass == targPass)
				return 0; // Аутентификация успешна

			return 2; // Не верный пароль
		}

		/// <summary>
		/// Выполнение SQL-запроса
		/// </summary>
		/// <param name="query">SQL-запрос</param>
		/// <returns></returns>
		public SqlDataReader ExecuteQuery(string query)
		{
			Command = new SqlCommand(query, Connection);
			return Command.ExecuteReader();
		}

		/// <summary>
		/// Выполнение SQL-команды
		/// </summary>
		/// <param name="cmd">SQL-команда</param>
		/// <returns></returns>
		public SqlDataReader ExecuteCommmand(SqlCommand cmd)
		{
			Command = cmd;
			Command.Connection = Connection;
			var reader = Command.ExecuteReader();
			return reader;
		}
	}
}
