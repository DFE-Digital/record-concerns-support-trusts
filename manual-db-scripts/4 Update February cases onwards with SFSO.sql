SET XACT_ABORT ON;
BEGIN TRAN

DECLARE @DateFrom VARCHAR(10);
SET @DateFrom = '2023-02-20';


UPDATE [sip].[concerns].[ConcernsCase]
SET DivisionId = 1
WHERE CreatedAt >= CONVERT(DATETIME, @DateFrom);


ROLLBACK TRAN
--COMMIT TRAN