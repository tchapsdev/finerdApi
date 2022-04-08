#nullable disable
using Finerd.Api.Data;
using Finerd.Api.Model.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Finerd.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : GenericController<Category>
    {
        public CategoriesController(ApplicationDbContext context):base(context)
        {
        }       
    }
}
