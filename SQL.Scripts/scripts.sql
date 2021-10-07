USE [LSDB]
GO
/****** Object:  UserDefinedTableType [dbo].[LogDatasType]    Script Date: 10/8/2021 1:05:20 AM ******/
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
/****** Object:  Table [dbo].[ApiKeys]    Script Date: 10/8/2021 1:05:21 AM ******/
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
/****** Object:  Table [dbo].[ApplicationInstances]    Script Date: 10/8/2021 1:05:21 AM ******/
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
/****** Object:  Table [dbo].[Applications]    Script Date: 10/8/2021 1:05:21 AM ******/
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
/****** Object:  Table [dbo].[LogDatas]    Script Date: 10/8/2021 1:05:21 AM ******/
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
/****** Object:  Index [IX_ApplicationId]    Script Date: 10/8/2021 1:05:21 AM ******/
CREATE NONCLUSTERED INDEX [IX_ApplicationId] ON [dbo].[LogDatas]
(
	[ApplicationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_ApplicationId_LogLevel]    Script Date: 10/8/2021 1:05:21 AM ******/
CREATE NONCLUSTERED INDEX [IX_ApplicationId_LogLevel] ON [dbo].[LogDatas]
(
	[ApplicationId] ASC,
	[LogLevel] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_ApplicationId_Timestamp]    Script Date: 10/8/2021 1:05:21 AM ******/
CREATE NONCLUSTERED INDEX [IX_ApplicationId_Timestamp] ON [dbo].[LogDatas]
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
