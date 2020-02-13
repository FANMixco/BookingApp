using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace BookingApp.Models
{
    public class UserUpdateModel : AbstractUserDefaults
    {
        public int ID { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }

        public UserUpdateModel()
        {
            RolesModel = new List<SelectListItem>()
            {
                new SelectListItem { Value = "0", Text = "Admin" },
                new SelectListItem { Value = "1", Text = "Reserver" }
            };

            SelectedRole = 1;
        }
    }
}
