USE [HPTLight]
GO

/****** Object:  Table [dbo].[GameInfo]    Script Date: 2025-08-16 16:27:40 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GameInfo]') AND type in (N'U'))
DROP TABLE [dbo].[GameInfo]
GO

CREATE TABLE [dbo].[GameInfo](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ATGGameId] [nvarchar](50) NOT NULL,
	[Code] [nvarchar](50) NOT NULL,
	[Date] [date] NOT NULL,
	[Track1] [int] NOT NULL,
	[Track2] [int] NULL
) ON [PRIMARY]
GO


/****** Object:  Table [dbo].[GameData]    Script Date: 2025-08-16 16:26:52 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GameData]') AND type in (N'U'))
DROP TABLE [dbo].[GameData]
GO

CREATE TABLE [dbo].[GameData](
	[GameId] [int] NOT NULL,
	[Status] [nvarchar](50) NOT NULL,
	[Timestamp] [datetime] NOT NULL,
	[Version] [bigint] NULL,
	[JsonData] [nvarchar](max) NOT NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO




