SET XACT_ABORT ON;
BEGIN TRAN

DECLARE @DateFrom VARCHAR(10);
SET @DateFrom = '2023-02-20 00:00:00';



UPDATE [concerns].[ConcernsCase]
SET DivisionId = 1
WHERE CreatedAt >= CONVERT(DATETIME, @DateFrom);


ROLLBACK TRAN
--COMMIT TRAN