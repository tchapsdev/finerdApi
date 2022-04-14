﻿using AutoMapper;
using Finerd.Api.Hubs;
using Finerd.Api.Model.Entities;
using Finerd.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Net.Http.Headers;

namespace Finerd.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionsController : BaseApiController
    {
        private readonly ITransactionService TransactionService;
        private readonly ILogger<HeathController> _logger;
        private readonly IMapper _mapper;
        private readonly IHubContext<NotificationHub, INotificationClient> _notificationHubContext;
        public TransactionsController(ITransactionService TransactionService, ILogger<HeathController> logger, IMapper mapper, IHubContext<NotificationHub, INotificationClient> notificationHubContext)
        {
            this.TransactionService = TransactionService;
            _logger = logger;
            _mapper = mapper;
            _notificationHubContext = notificationHubContext;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var result = await TransactionService.Get(UserID);
            if (!result.Success)
            {
                return UnprocessableEntity(result);
            }
            return Ok(_mapper.Map<IList<TransactionDto>>(result.Transactions));
        }


        [HttpPost]
        public async Task<IActionResult> Post(TransactionDto transaction)
        {
            var model = _mapper.Map<Transaction>(transaction);
            
            var result = await TransactionService.Save(model, UserID);
            if (!result.Success)
            {
                return UnprocessableEntity(result);
            }
            var photo = Upload(Request, model.Id);
            if (string.IsNullOrEmpty(photo))
            {
                result.Transaction.Photo = photo;
                await TransactionService.Save(result.Transaction, UserID);
            }
            return Ok(_mapper.Map<TransactionDto>(result.Transaction));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, TransactionDto transaction)
        {
            if (id != transaction.Id)
                return BadRequest();

            var model = _mapper.Map<Transaction>(transaction);
            var result = await TransactionService.Save(model, UserID);
            if (!result.Success)
            {
                return UnprocessableEntity(result);
            }
            var photo = Upload(Request, model.Id);
            if (string.IsNullOrEmpty(photo))
            {
                result.Transaction.Photo = photo;
                await TransactionService.Save(result.Transaction, UserID);
            }
            return Ok(_mapper.Map<TransactionDto>(result.Transaction));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await TransactionService.Delete(id, UserID);
            if (!result.Success)
            {
                return UnprocessableEntity(result);
            }
            return Ok(result.Id);
        }


        string Upload(HttpRequest request, int transactionId)
        {
            try
            {
                var file = request.Form.Files[0];
                var folderName = Path.Combine("Resources", "Images");
                var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                if (file.Length > 0)
                {
                    var fileName = transactionId + ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                    var fullPath = Path.Combine(pathToSave, fileName);
                    var dbPath = Path.Combine(folderName, fileName);
                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }
                    return fileName;
                }                
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }
            return "";
        }

    }
}
