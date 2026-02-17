CREATE TABLE [Cosponsored].[BoardContact](
	[BoardContactId]		INT	IDENTITY(1,1)				NOT NULL,
	[BoardId]				INT								NOT NULL,
	[ContactName]			NVARCHAR(255)					NULL,
	[Email]					NVARCHAR(255)					NOT NULL,
	[Created]				DATETIME2 DEFAULT SYSDATETIME()	NOT NULL,
	[CreatedBy]				NVARCHAR(255)					NOT NULL,
	[Modified]				DATETIME2						NULL,
	[ModifiedBy]			NVARCHAR(255)					NULL,
 CONSTRAINT [PK_BoardContact_Id] PRIMARY KEY CLUSTERED ([BoardContactId] ASC),
 CONSTRAINT [FK_BoardContanct_BoardId]	FOREIGN KEY	([BoardId])	REFERENCES	[Cosponsored].[Board] ([BoardId])
);
GO