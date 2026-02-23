
CREATE PROCEDURE dbo.usp_get_certificate_exam_list
AS
BEGIN

SELECT DISTINCT c.Name AS ExamName, c.CertificationGuid FROM dbo.Certification c
INNER JOIN dbo.Source s ON s.SourceId = c.SourceId
WHERE s.Code = 'ABIM'
ORDER BY c.Name;

END