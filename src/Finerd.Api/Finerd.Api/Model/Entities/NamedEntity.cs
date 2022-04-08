using System.ComponentModel.DataAnnotations;

namespace Finerd.Api.Model.Entities
{
    public class NamedEntity: EntityKey
    {
        [Required]
        public string Name { get; set; }
    }
}
