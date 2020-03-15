CREATE TABLE [dbo].[Locations]
(
	[Id] [int] NOT NULL IDENTITY(1,1),
	[Province] [nvarchar](100),
	[City] [nvarchar](100) NOT NULL,
	CONSTRAINT [PK_Locations] PRIMARY KEY CLUSTERED ([Id]),
	CONSTRAINT [UK_City_Province] UNIQUE ([City], [Province])
)