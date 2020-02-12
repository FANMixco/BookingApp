using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace BookingApp.Models
{
    public class UserCreateModel : AbstractUserDefaults
    {
        public UserCreateModel()
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
