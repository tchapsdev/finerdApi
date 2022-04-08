#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Finerd.Api.Data;
using Finerd.Api.Model;
using Finerd.Api.Model.Entities;

namespace Finerd.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionTypesController : GenericController<TransactionType>
    {
        public TransactionTypesController(ApplicationDbContext context) : base(context)
        {
        }
    }

}
