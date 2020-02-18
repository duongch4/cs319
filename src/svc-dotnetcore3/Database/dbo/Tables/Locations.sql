CREATE TABLE [dbo].[Locations]
(
	[Id] [int] NOT NULL IDENTITY(1,1),
	[Province] [nvarchar](20) NULL,
	[City] [nvarchar](100) NOT NULL
		CONSTRAINT [PK_Locations] PRIMARY KEY CLUSTERED ([Id]),
	CONSTRAINT [UK_Locations_City] UNIQUE ([City])
)