using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BookByte.Models.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [DisplayName("Category Name")]
        public string Name { get; set; }
        [DisplayName("Display Order")]
        [Range(1,100)] //this will be used by asp-validation-for in cshtml file to throw validation error
        //[Range(1.100,ErrorMessage="Add custome error message here")]
        public int  DisplayOrder { get; set; }
    }
}
