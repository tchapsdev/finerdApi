using Finerd.Api.Model.Entities;
using System.ComponentModel.DataAnnotations;

namespace Finerd.Api.Model
{
    public class UserDto: EntityKey
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
    }
}
