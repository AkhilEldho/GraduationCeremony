using System.ComponentModel.DataAnnotations;

namespace GraduationCeremony.ViewModel
{
    //AddRoleViewModel class
    public class AddRoleViewModel
    {
        public string Id { get; set; }

        [Display(Name = "Role")]
        [Required(ErrorMessage = "Role Name is required")]
        public string RoleName { get; set; }

        public List<string> Users { get; set; } = new List<string>();
    }
}
