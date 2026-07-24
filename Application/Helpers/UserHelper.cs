using Domain.Constants;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;

namespace Application.Helpers
{
    public static class UserHelper
    {
        private static readonly string[] HigherAuthorityRoles = [UserRoles.Admin,UserRoles.Manager];
        public static bool IsHigherAuthority(UserRole roleName)
        {
            return HigherAuthorityRoles.Contains(roleName.ToString());
        }

        public static bool IsDirectReport(AppUser manager,AppUser subordinate)
        {
            return subordinate.ReportsToUserId == manager.Id;
        }

        public static bool IsCreator(AppUser appUser,IAuditEntity entity)
        {
            return entity.CreatedBy==appUser.Id;
        }

        public static List<AppUser> GetDirectReports(AppUser manager,List<AppUser> allUsers)
        {
            return allUsers.Where(x=>x.ReportsToUserId==manager.Id).ToList();
        }
    }
}