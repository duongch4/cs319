CREATE TABLE [dbo].[Users]
(
	[Id] [int] NOT NULL IDENTITY(1,1),
	[FirstName] [nvarchar](50) NOT NULL,
	[LastName] [nvarchar](50) NOT NULL,
	[Username] [nvarchar](50) NOT NULL,
	[LocationId] [int] NOT NULL,
	[IsAdmin] [bit] NOT NULL,
	[IsManager] [bit] NOT NULL,
	CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED ([Id]),
	CONSTRAINT [UK_Users_Username] UNIQUE ([Username]),
	CONSTRAINT [FK_Users_Locations] FOREIGN KEY ([LocationId]) REFERENCES [Locations]([Id]) ON DELETE NO ACTION
)