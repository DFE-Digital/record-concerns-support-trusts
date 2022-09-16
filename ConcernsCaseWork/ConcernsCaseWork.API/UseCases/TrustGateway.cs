using ConcernsCaseWork.API.Extensions;
using System.Text.RegularExpressions;

namespace ConcernsCaseWork.API.UseCases
{
    public class TrustGateway : ITrustGateway
    {
        private readonly LegacyTramsDbContext _dbContext;

        public TrustGateway(LegacyTramsDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Group GetGroupByUkPrn(string ukPrn)
        {
            return _dbContext.Group.FirstOrDefault(g => g.Ukprn == ukPrn);
        }

        public Trust GetIfdTrustByGroupId(string groupId)
        {
            return _dbContext.Trust.FirstOrDefault(t => t.TrustRef == groupId);
        }

        public Trust GetIfdTrustByRID(string RID)
        {
            return _dbContext.Trust.FirstOrDefault(x => x.Rid.Equals(RID));
        }

        public IQueryable<Trust> GetIfdTrustsByTrustRef(string[] trustRefs)
        {
            var predicate = PredicateBuilder.False<Trust>();
            foreach (var trustRef in trustRefs)
            {
                predicate = predicate.Or(t => t.TrustRef == trustRef);
            }

            return _dbContext.Trust.Where(predicate);
        }

        public IList<Group> SearchGroups(int page, int count, string groupName, string ukPrn, string companiesHouseNumber)
        {
            if (groupName == null && ukPrn == null && companiesHouseNumber == null)
            {
                return _dbContext.Group.OrderBy(group => group.GroupUid).Skip((page - 1) * count).Take(count).ToList();
            }

            return _dbContext.Group
                .Where(g => (
                    (g.GroupName.Contains(groupName) ||
                     g.Ukprn.Contains(ukPrn) ||
                     g.CompaniesHouseNumber.Contains(companiesHouseNumber))
                    && (
                        g.GroupType == "Single-academy trust" ||
                        g.GroupType == "Multi-academy trust"
                    )
                ))
                .OrderBy(group => group.GroupUid)
                .Skip((page - 1) * count).Take(count).ToList();
        }
    }
}