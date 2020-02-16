CREATE TABLE [dbo].[OutOfOffice](
    [ResourceId][int] NOT NULL,
    [FromDate] DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    [ToDate] DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    [Reason] nvarchar(100)
  CONSTRAINT [FK_OutOfOffice_ResourceId] FOREIGN KEY ([ResourceId]) REFERENCES [Users]([Id])
  CONSTRAINT [PK_OutOfOffice] PRIMARY KEY ([ResourceId], [FromDate], [ToDate])
)