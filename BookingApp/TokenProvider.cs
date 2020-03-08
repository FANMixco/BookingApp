using System.Collections.Generic;
using System.Security.Claims;
using BookingApp.DB.Classes.DB;
using BookingApp.Filters.Authorization;

namespace BookingApp
{
    public class TokenProvider
    {
        public static IEnumerable<Claim> GetUserClaims(Users user)
        {
            return new Claim[]
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim("USERID", user.UserId.ToString()),
                new Claim("EMAILID", user.Email),
                new Claim("ROLES", user.Role == 0 ? Roles.ADMIN : Roles.CUSTOMER)
            };
        }
    }
}
