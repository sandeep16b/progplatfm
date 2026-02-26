;with cte_data(Value,Description,Created,Modified,CreatedBy,ModifiedBy)
as (select * from (values
('Auto','Deselection made via Auto Deactivation Cease Billing Report',SYSDATETIME(), SYSDATETIME(),'System','System'),
('Self','Deselection made by ABIM Staff via Staff Portal',SYSDATETIME(), SYSDATETIME(),'System','System')
)c(Value,Description,Created,Modified,CreatedBy,ModifiedBy))
merge	dbo.DeselectionType as t
using	cte_data as s
on		1=1 and t.Value = s.Value
when matched then
	update set
	Description = s.Description
when not matched by target then
	insert(Value,Description,Created,Modified,CreatedBy,ModifiedBy)
	values(s.Value,s.Description,s.Created,s.Modified,s.CreatedBy,s.ModifiedBy);