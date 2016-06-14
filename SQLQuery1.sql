CREATE TABLE Users(
	Username	varchar(20)		PRIMARY KEY,
	Pass		varbinary(64)	NOT NULL,
	Salt		varchar(40)		NOT NULL
)

CREATE PROC GetUserInfoByUsername
	@name varchar(20),
	@pass varbinary(64) OUTPUT,
	@salt varchar(40) OUTPUT
AS
	SELECT @pass = Pass, @salt = Salt FROM Users
	WHERE @name = Username

CREATE PROC AddNewUser
	@name varchar(20),
	@pass varbinary(64),
	@salt varchar(40)
AS
	INSERT INTO Users
	VALUES (@name, @pass, @salt)

CREATE PROC LogInInfo
	@name varchar(20),
	@ip varchar(15),
	@isSuccess bit
AS
	DECLARE @aType VARCHAR(50)
	if @isSuccess = 0
		SELECT @aType = 'Login fail'
	if @isSuccess = 1
		SELECT @aType = 'Login succesfull'
	else
		SELECT @aType = 'User already in chat'
	INSERT INTO LogTable
	VALUES (@aType, GETDATE(), @name, @ip)

CREATE PROC CloseConnection
	@name varchar(20),
	@ip varchar(15)
AS
	INSERT INTO LogTable
	VALUES ('Closed connection', GETDATE(), @name, @ip)

CREATE TABLE LogTable(
	ID INT IDENTITY(0,1) PRIMARY KEY,
	ActionType VARCHAR(50) NOT NULL,
	ActionDate DATETIME NOT NULL,
	Username varchar(20) NOT NULL,
	AdressIP varchar(15) NOT NULL
)

CREATE TABLE RoomsMessages(
	NameFrom VARCHAR(20) NOT NULL,
	NameTo VARCHAR(50) NOT NULL,
	MessageText VARCHAR(255) NOT NULL,
	MessageDate DATETIME NOT NULL
)

CREATE PROC AddNewMessage
	@from varchar(20),
	@to varchar(50),
	@text varchar(255)
AS
	INSERT INTO RoomsMessages
	VALUES (@from, @to, @text, GETDATE())