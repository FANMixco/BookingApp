using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace BookingApp.Models
{
    public abstract class AbstractUserDefaults
    {
        public List<SelectListItem> RolesModel { get; set; }
        public int SelectedRole { get; set; }
    }
}
