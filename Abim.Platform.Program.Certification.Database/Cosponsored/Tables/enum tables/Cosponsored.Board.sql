CREATE TABLE [Cosponsored].[Board](
	[BoardId]				INT IDENTITY(1,1)				NOT NULL,
	[Code]					NVARCHAR(10)					NOT NULL,
	[Name]					NVARCHAR(255)					NULL,
	[Created]				DATETIME2 DEFAULT SYSDATETIME()	NOT NULL,
	[CreatedBy]				NVARCHAR(255)					NOT NULL,
	[Modified]				DATETIME2						NULL,
	[ModifiedBy]			NVARCHAR(255)					NULL,
 CONSTRAINT [PK_Board_Id] PRIMARY KEY CLUSTERED ([BoardId] ASC)
);
GO