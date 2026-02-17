CREATE FUNCTION dbo.Greatest
   (  @Value1  sql_variant,
      @Value2  sql_variant
   )
   RETURNS sql_variant
AS
   BEGIN
      
      DECLARE @ReturnValue sql_variant



      DECLARE @TempTable table
         (  RowID      int  IDENTITY,
            TempColumn sql_variant
         )


      INSERT INTO @TempTable VALUES ( @Value1 )
      INSERT INTO @TempTable VALUES ( @Value2 ) 

      SELECT @ReturnValue = max( TempColumn )
      FROM @TempTable



      RETURN @ReturnValue



   END
GO
