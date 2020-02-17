CREATE TABLE [dbo].[Locations]
(
	[Id] [int] NOT NULL IDENTITY(1,1),
	[Province] [nvarchar](100) NOT NULL,
	[City] [nvarchar](100) NOT NULL
		CONSTRAINT [PK_Locations] PRIMARY KEY CLUSTERED ([Id])
)