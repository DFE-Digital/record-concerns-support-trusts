SET XACT_ABORT ON;
BEGIN TRAN

UPDATE [concerns].[ConcernsCase]
SET DivisionId = 1;

ROLLBACK TRAN
--COMMIT TRAN