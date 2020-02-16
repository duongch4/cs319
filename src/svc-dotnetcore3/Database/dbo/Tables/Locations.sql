CREATE TABLE [dbo].[Locations]
(
	[Id] [int] NOT NULL IDENTITY(1,1),
	[Code] [nvarchar](10) NOT NULL,
	[Name] [nvarchar](100) NOT NULL
		CONSTRAINT [PK_Locations] PRIMARY KEY CLUSTERED ([Id]),
	CONSTRAINT [UK_Locations_Code] UNIQUE ([Code]),
	CONSTRAINT [UK_Locations_Name] UNIQUE ([Name])
)
