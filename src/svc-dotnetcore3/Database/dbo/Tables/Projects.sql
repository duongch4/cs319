CREATE TABLE [dbo].[Projects]
(
	[Id] [int] NOT NULL IDENTITY(1,1),
	[Number] [nvarchar](50) NOT NULL,
	[Title] [nvarchar](255) NOT NULL,
	[LocationId] [int] NOT NULL,
	[CreatedAt] DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
	[UpdatedAt] DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
	[ManagerId] [int] NOT NULL,
	[ProjectStartDate] DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
	[ProjectEndDate] DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
	CONSTRAINT [PK_Projects] PRIMARY KEY CLUSTERED ([Id]),
	CONSTRAINT [FK_Projects_Locations] FOREIGN KEY ([LocationId]) REFERENCES [Locations]([Id]),
	CONSTRAINT [FK_Projects_User] FOREIGN KEY ([ManagerId]) REFERENCES [Users]([Id]),
	CONSTRAINT [UK_Projects_Number] UNIQUE ([Number])
)