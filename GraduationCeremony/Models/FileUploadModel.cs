namespace GraduationCeremony.Models
{
    using Microsoft.AspNetCore.Http;
    using System.ComponentModel.DataAnnotations;

    //https://medium.com/c-sharp-progarmming/convert-excel-to-data-table-in-asp-net-core-using-ep-plus-b59533e162b3
    public class FileUploadModel
    {
        [Required(ErrorMessage = "Please select file")]
        public IFormFile File { get; set; }
    }


}
