using System.ComponentModel.DataAnnotations;

namespace GraduationCeremony.ViewModel
{
    //EditRoleViewModel class
    public class EditRoleViewModel
    {
        public string Id { get; set; }

        [Display(Name = "Role")]
        [Required(ErrorMessage = "Role Name is required")]
        public string RoleName { get; set; }
        public List<string> Users { get; set; } = new();

    }
}
