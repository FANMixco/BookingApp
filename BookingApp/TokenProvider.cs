using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BookingApp.DB.Classes.DB;
using BookingApp.Filters.Authorization;
using Microsoft.IdentityModel.Tokens;

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

        public static string LoginUser(Users user)
        {
            try
            {
                //If it's registered user, check user password stored in Database 
                //For demo, password is not hashed. Simple string comparison 
                //In real, password would be hashed and stored in DB. Before comparing, hash the password
                //Authentication successful, Issue Token with user credentials
                //Provide the security key which was given in the JWToken configuration in Startup.cs
                //Generate Token for user 
                var JWToken = new JwtSecurityToken(
                    issuer: Startup.ADDRESS,
                    audience: Startup.ADDRESS,
                    claims: GetUserClaims(user),
                    notBefore: new DateTimeOffset(DateTime.Now).DateTime,
                    expires: new DateTimeOffset(DateTime.Now.AddDays(1)).DateTime,
                    //Using HS256 Algorithm to encrypt Token
                    signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Startup.JWT_KEY)),
                                        SecurityAlgorithms.HmacSha256Signature)
                );

                //return token
                return new JwtSecurityTokenHandler().WriteToken(JWToken);
            }
            catch
            {
                return null;
            }
        }
    }
}
