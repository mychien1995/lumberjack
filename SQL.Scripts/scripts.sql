USE [LSDB]
GO
/****** Object:  UserDefinedTableType [dbo].[LogDatasType]    Script Date: 10/9/2021 1:20:39 PM ******/
CREATE TYPE [dbo].[LogDatasType] AS TABLE(
	[Id] [uniqueidentifier] NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[Timestamp] [bigint] NULL,
	[LogLevel] [int] NOT NULL,
	[Namespace] [varchar](500) NULL,
	[Message] [nvarchar](max) NULL,
	[Request] [varchar](2000) NULL,
	[RequestContext] [nvarchar](max) NULL,
	[ApplicationId] [uniqueidentifier] NULL,
	[Instance] [varchar](500) NULL
)
GO
/****** Object:  Table [dbo].[ApiKeys]    Script Date: 10/9/2021 1:20:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ApiKeys](
	[Id] [uniqueidentifier] NOT NULL,
	[KeyValue] [nvarchar](max) NULL,
	[ApplicationId] [uniqueidentifier] NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_ApiKeyId] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ApplicationInstances]    Script Date: 10/9/2021 1:20:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ApplicationInstances](
	[Id] [uniqueidentifier] NOT NULL,
	[InstanceName] [varchar](500) NULL,
	[ApplicationId] [uniqueidentifier] NULL,
 CONSTRAINT [PK_ApplicationInstanceId] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Applications]    Script Date: 10/9/2021 1:20:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Applications](
	[Id] [uniqueidentifier] NOT NULL,
	[ApplicationName] [varchar](500) NULL,
	[ApplicationCode] [varchar](20) NULL,
	[SortOrder] [int] NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_ApplicationId] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[ApplicationCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[LogDatas]    Script Date: 10/9/2021 1:20:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LogDatas](
	[Id] [uniqueidentifier] NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[Timestamp] [bigint] NULL,
	[LogLevel] [int] NOT NULL,
	[Namespace] [varchar](500) NULL,
	[Message] [nvarchar](max) NULL,
	[Request] [varchar](2000) NULL,
	[RequestContext] [nvarchar](max) NULL,
	[ApplicationId] [uniqueidentifier] NULL,
	[Instance] [varchar](500) NULL,
 CONSTRAINT [PK_LogId] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[LogDatas_2]    Script Date: 10/9/2021 1:20:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LogDatas_2](
	[Id] [uniqueidentifier] NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[Timestamp] [bigint] NULL,
	[LogLevel] [int] NOT NULL,
	[Namespace] [varchar](500) NULL,
	[Message] [nvarchar](max) NULL,
	[Request] [varchar](2000) NULL,
	[RequestContext] [nvarchar](max) NULL,
	[ApplicationId] [uniqueidentifier] NULL,
	[Instance] [varchar](500) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Shards]    Script Date: 10/9/2021 1:20:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Shards](
	[ShardId] [uniqueidentifier] NOT NULL,
	[Number] [int] NULL,
	[TableName] [varchar](200) NULL,
	[CreatedDate] [bigint] NULL,
	[MaximumSize] [bigint] NULL,
	[IsCurrent] [bit] NOT NULL,
 CONSTRAINT [PK_ShardId] PRIMARY KEY CLUSTERED 
(
	[ShardId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Index [IX_ApplicationId]    Script Date: 10/9/2021 1:20:39 PM ******/
CREATE NONCLUSTERED INDEX [IX_ApplicationId] ON [dbo].[LogDatas]
(
	[ApplicationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_ApplicationId_LogLevel]    Script Date: 10/9/2021 1:20:39 PM ******/
CREATE NONCLUSTERED INDEX [IX_ApplicationId_LogLevel] ON [dbo].[LogDatas]
(
	[ApplicationId] ASC,
	[LogLevel] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_ApplicationId_Timestamp]    Script Date: 10/9/2021 1:20:39 PM ******/
CREATE NONCLUSTERED INDEX [IX_ApplicationId_Timestamp] ON [dbo].[LogDatas]
(
	[ApplicationId] ASC,
	[Timestamp] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_ApplicationId]    Script Date: 10/9/2021 1:20:39 PM ******/
CREATE NONCLUSTERED INDEX [IX_ApplicationId] ON [dbo].[LogDatas_2]
(
	[ApplicationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_ApplicationId_LogLevel]    Script Date: 10/9/2021 1:20:39 PM ******/
CREATE NONCLUSTERED INDEX [IX_ApplicationId_LogLevel] ON [dbo].[LogDatas_2]
(
	[ApplicationId] ASC,
	[LogLevel] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_ApplicationId_Timestamp]    Script Date: 10/9/2021 1:20:39 PM ******/
CREATE NONCLUSTERED INDEX [IX_ApplicationId_Timestamp] ON [dbo].[LogDatas_2]
(
	[ApplicationId] ASC,
	[Timestamp] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE [dbo].[ApiKeys] ADD  DEFAULT (newid()) FOR [Id]
GO
ALTER TABLE [dbo].[ApiKeys] ADD  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[ApplicationInstances] ADD  DEFAULT (newid()) FOR [Id]
GO
ALTER TABLE [dbo].[Applications] ADD  DEFAULT (newid()) FOR [Id]
GO
ALTER TABLE [dbo].[Applications] ADD  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[LogDatas] ADD  DEFAULT (newid()) FOR [Id]
GO
ALTER TABLE [dbo].[LogDatas_2] ADD  DEFAULT (newid()) FOR [Id]
GO
ALTER TABLE [dbo].[ApiKeys]  WITH CHECK ADD  CONSTRAINT [FK_ApiKey_Application] FOREIGN KEY([ApplicationId])
REFERENCES [dbo].[Applications] ([Id])
GO
ALTER TABLE [dbo].[ApiKeys] CHECK CONSTRAINT [FK_ApiKey_Application]
GO
ALTER TABLE [dbo].[ApplicationInstances]  WITH CHECK ADD  CONSTRAINT [FK_Instance_Application] FOREIGN KEY([ApplicationId])
REFERENCES [dbo].[Applications] ([Id])
GO
ALTER TABLE [dbo].[ApplicationInstances] CHECK CONSTRAINT [FK_Instance_Application]
GO
/****** Object:  StoredProcedure [dbo].[AutoScale]    Script Date: 10/9/2021 1:20:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROCEDURE [dbo].[AutoScale]
(
	@currentTime bigint,
	@maximumSize bigint,
	@buffer bigint
)
AS
BEGIN
	DECLARE @newTableName varchar(200);
	DECLARE @currentTable varchar(200);
	DECLARE @currentTableNumber varchar(200);
	DECLARE @currentSize bigint;
	IF(EXISTS(SELECT TOP 1 * FROM dbo.Shards))
	BEGIN
		SET @currentTable = (SELECT TableName FROM dbo.Shards WHERE IsCurrent = 1);
		SET @currentTableNumber = (SELECT Number FROM dbo.Shards WHERE IsCurrent = 1);
		DECLARE @getSizeQuery nvarchar(MAX);
		SET @getSizeQuery = 'SELECT @currentSizeOUT = COUNT(*) FROM ' + @currentTable
		exec sp_executesql @getSizeQuery, N'@currentSizeOUT bigint OUTPUT', @currentSizeOUT = @currentSize OUTPUT;
		IF(@currentSize > @maximumSize - @buffer)
		BEGIN
			SET @newTableName = 'dbo.LogDatas_' + CAST((@currentTableNumber + 1) AS varchar(20));
			EXEC dbo.CreateLogTable @TableName = @newTableName;
			UPDATE Shards SET IsCurrent = 0;
			INSERT INTO Shards (ShardId, Number, TableName, MaximumSize, CreatedDate, IsCurrent)
			VALUES(NEWID(), (@currentTableNumber + 1), @newTableName, @maximumSize, @currentTime, 1);
		END
	END
	ELSE
	BEGIN
		SET @newTableName = 'dbo.LogDatas_1';
		EXEC dbo.CreateLogTable @TableName = @newTableName;
		INSERT INTO Shards (ShardId, Number, TableName, MaximumSize, CreatedDate, IsCurrent)
		VALUES(NEWID(), 1, @newTableName, @maximumSize, @currentTime, 1);
	END
END
GO
/****** Object:  StoredProcedure [dbo].[CreateLogTable]    Script Date: 10/9/2021 1:20:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROCEDURE [dbo].[CreateLogTable]
(
	@TableName varchar(200)
)
AS
BEGIN
	DECLARE @createTableQuery nvarchar(MAX);
	DECLARE @createIndexQuery nvarchar(MAX);
	SET @createTableQuery = 'CREATE TABLE {0}(
	[Id] [uniqueidentifier] primary key default newid(),
	[CreateDate] [datetime] NOT NULL,
	[Timestamp] [bigint] NULL,
	[LogLevel] [int] NOT NULL,
	[Namespace] [varchar](500) NULL,
	[Message] [nvarchar](max) NULL,
	[Request] [varchar](2000) NULL,
	[RequestContext] [nvarchar](max) NULL,
	[ApplicationId] [uniqueidentifier] NULL,
	[Instance] [varchar](500) NULL);';

	SET @createIndexQuery = 'CREATE NONCLUSTERED INDEX [IX_ApplicationId] ON {0}
	(
		[ApplicationId] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	
	CREATE NONCLUSTERED INDEX [IX_ApplicationId_LogLevel] ON {0}
	(
		[ApplicationId] ASC,
		[LogLevel] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	
	CREATE NONCLUSTERED INDEX [IX_ApplicationId_Timestamp] ON {0}
	(
		[ApplicationId] ASC,
		[Timestamp] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	';
	
	SET @createTableQuery = REPLACE(@createTableQuery, '{0}', @TableName);
	SET @createIndexQuery = REPLACE(@createIndexQuery, '{0}', @TableName);

	exec sp_executesql  @createTableQuery
	exec sp_executesql  @createIndexQuery
END
GO
/****** Object:  StoredProcedure [dbo].[QueryLogs]    Script Date: 10/9/2021 1:20:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROCEDURE [dbo].[QueryLogs]
(
	@TableName varchar(200),
	@Text nvarchar(MAX),
	@ApplicationId uniqueidentifier,
	@LogLevel int,
	@StartTime bigint,
	@EndTime bigint,
	@Skip int,
	@Take int
)
AS
BEGIN
	IF(@Skip IS NULL)
		SET @Skip = 0;
	IF(@Take IS NULL)
		SET @Take = 100;
	DECLARE @query nvarchar(MAX);
	DECLARE @countQuery nvarchar(MAX);
	DECLARE @queryPart nvarchar(MAX);
	DECLARE @params nvarchar(MAX);
	SET @params =  N'@Text nvarchar(MAX),
	@ApplicationId uniqueidentifier,
	@LogLevel int,
	@StartTime bigint,
	@EndTime bigint';

	SET @queryPart = '(@Text IS NULL OR LEN(@Text) = 0 OR Message LIKE ''%' + @Text+ '%'') AND
	(@ApplicationId IS NULL OR ApplicationId = @ApplicationId) AND
	(@LogLevel IS NULL OR LogLevel = @LogLevel) AND
	(@StartTime IS NULL OR Timestamp >= @StartTime) AND
	(@EndTime IS NULL OR Timestamp <= @EndTime)';

	SET @countQuery = 'SELECT COUNT(*) AS Total FROM ' + @TableName + ' WHERE ' + @queryPart;
	SET @query = 'SELECT * FROM ' + @TableName + ' WHERE ' + @queryPart + 
	' ORDER BY Timestamp DESC OFFSET ' + CAST(@Skip AS varchar(20)) + ' ROWS FETCH NEXT ' + CAST(@Take AS varchar(20)) + ' ROWS ONLY';

	exec sp_executesql @countQuery, @params, @Text, @ApplicationId, @LogLevel, @StartTime, @EndTime
	exec sp_executesql @query, @params, @Text, @ApplicationId, @LogLevel, @StartTime, @EndTime
END
GO
