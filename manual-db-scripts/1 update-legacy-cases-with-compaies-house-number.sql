/*
This script will update existing cases in the concerns database by importing the companies house number from
the master trust table.
It should be run manually when deploying version 19 of the app to an environment that has concerns
cases created a prior version already (otherwise it's not needed)

The script relies on the fact that the academies database contains the master data and the concerns database schema
If separate databases are used then this script won't work.
*/

/* Get some stats on how many cases don't need updating, and how many do. Just for the benefit of the reader */
Select count(id) as [Cases with companies house number already]
from [concerns].[ConcernsCase] (nolock)
Where [concerns].[ConcernsCase].[TrustUkprn] IS NOT NULL
	AND [concerns].[ConcernsCase].[TrustCompaniesHouseNumber] IS NOT NULL


Select count(id) as [Expected Number of Rows Updated]
from [concerns].[ConcernsCase] (nolock)
Where [concerns].[ConcernsCase].[TrustUkprn] IS NOT NULL
	AND [concerns].[ConcernsCase].[TrustCompaniesHouseNumber] IS NULL

/* Run this to get the ids of the actual concerns cases that will be updated,
with the master trust companies house number that will be applied.

This is outputted for the reader but also used as the input to the final section

*/
DECLARE @UpdatesToApply TABLE(ConcernsCase_Id INT, TrustUkPrn NVARCHAR(12), MasterTrust_UKPRN VARCHAR(MAX), MasterTrust_CompaniesHouseNumber VARCHAR(MAX))
INSERT INTO @UpdatesToApply
SELECT
	cc.[Id] AS ConcernsCase_Id,
	cc.[TrustUkprn]AS ConcernsCase_TrustUkPrn,
	mt.[UKPRN] AS MasterTrust_UKPRN,
	mt.[Companies House Number] AS MasterTrust_CompaniesHouseNumber
from [concerns].[ConcernsCase] cc (nolock)
LEFT JOIN [mstr].[Trust] mt (nolock) ON mt.UKPRN = cc.TrustUkprn
Where cc.[TrustUkprn] IS NOT NULL
	AND cc.[TrustCompaniesHouseNumber] IS NULL

Select * From @UpdatesToApply


/* The actual update. Do this (with the previous section) in a transaction and commit / roll back appropriately */
--BEGIN TRANSACTION

--IF NOT EXISTS (SELECT * FROM concerns.__EfMigrationsHistory Where MIgrationId = '20230321162132_TrustCompaniesHouseNumber')
--BEGIN
--   ROLLBACK
--   RAISERROR('It looks like no database migration has been applied to create a companies house number on the concerns case table. Make sure this has been done before running this script', 17, -1)
--END

--	UPDATE [concerns].[ConcernsCase]
--	SET [TrustCompaniesHouseNumber] = updatesToApply.MasterTrust_CompaniesHouseNumber
--	FROM @UpdatesToApply as updatesToApply
--	WHERE [Id] = updatesToApply.ConcernsCase_Id

--	SELECT *
--	From [concerns].[ConcernsCase] WHERE ID IN (SELECT ConcernsCase_Id from @UpdatesToApply)
--COMMIT
