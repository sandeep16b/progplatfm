CREATE TABLE [Cosponsored].[ActionType](
	[Value]					INT					NOT NULL,
	[Description]			NVARCHAR(255)					NULL,
	[Created]				DATETIME2 DEFAULT SYSDATETIME()	NOT NULL,
	[CreatedBy]				NVARCHAR(255)					NOT NULL,
	[Modified]				DATETIME2						NULL,
	[ModifiedBy]			NVARCHAR(255)					NULL,
 CONSTRAINT [PK_ActionType_Value] PRIMARY KEY CLUSTERED ([Value] ASC)
);
GO