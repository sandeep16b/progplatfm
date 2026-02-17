CREATE FUNCTION Cosponsored.RandomSSN
(
)
RETURNS INT
AS
BEGIN
	
	DECLARE @RandomSSN INT

	SELECT @RandomSSN = floor(1000 + (SELECT Value FROM Cosponsored.GenRandomNum) * 8999)

	RETURN @RandomSSN
	
END