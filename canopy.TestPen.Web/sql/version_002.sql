USE [CanopyTestPen]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

--Stories
CREATE TABLE [dbo].[Stories](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[RunId] [int] NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[Description] [nvarchar](2000) NOT NULL
 CONSTRAINT [PK_Stories] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] 

GO

--Add TestedBy to Scenarios
ALTER TABLE Scenarios ADD TestedBy VARCHAR(100)

--Add IsActive to Runs
ALTER TABLE Runs ADD IsActive BIT NOT NULL DEFAULT 1

--Add ClaimedBy to Case
ALTER TABLE Cases ADD ClaimedBy VARCHAR(100)

--Add UpdateDate to Scenarios
ALTER TABLE Scenarios ADD UpdateDate DATETIME

--PassFailSkipLog
CREATE TABLE [dbo].[PassFailSkipLog](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ScenarioId] [int] NOT NULL,
	[ChangeBy] [nvarchar](100) NOT NULL,
	[Description] [nvarchar](2000) NOT NULL
 CONSTRAINT [PK_PassFailSkipLog] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] 

GO