using AutoMapper;
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
        private readonly IGenericService<Category> _categoryService;
        private readonly IGenericService<TransactionType> _transactionTypeService;
        public TransactionsController(ITransactionService TransactionService, ILogger<HeathController> logger, IMapper mapper, 
            IHubContext<NotificationHub, INotificationClient> notificationHubContext, 
            IGenericService<Category> categoryService, IGenericService<TransactionType> transactionTypeService)
        {
            this.TransactionService = TransactionService;
            _logger = logger;
            _mapper = mapper;
            _notificationHubContext = notificationHubContext;
            _categoryService = categoryService;
            _transactionTypeService = transactionTypeService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var result = await TransactionService.Get(UserID);
            if (!result.Success)
            {
                return UnprocessableEntity(result);
            }
            var modelDtos = _mapper.Map<IList<TransactionDto>>(result.Transactions);
            if (modelDtos == null)
                return Ok(new { data = new List<TransactionDto>() });
            SetTransactionDtoParameters(modelDtos);
            return Ok(modelDtos);
        }

        private void SetTransactionDtoParameters(IList<TransactionDto> modelDtos)
        {
            if (modelDtos.Any())
            {
                var categories = _categoryService.Query().ToList() ?? new List<Category>();
                var transactionTypes = _transactionTypeService.Query().ToList() ?? new List<TransactionType>();
                modelDtos.ToList().ForEach(t =>
                {
                    t.Type = transactionTypes.Where(tp => tp.Id == t.TransactionTypeId).Select(tp => tp.Name).FirstOrDefault();
                    t.Category = categories.Where(c => c.Id == t.CategoryId).Select(c => c.Name).FirstOrDefault();
                });
            }
        }

        [HttpGet("findAllByType/{typeId}")]
        public async Task<IActionResult> GetByType(string typeId)
        {
            var result = await TransactionService.Get(UserID);
            if (!result.Success)
            {
                return UnprocessableEntity(result);
            }
            var transactionType = _transactionTypeService.Query().Where(t => t.Name.ToLower() == typeId.ToLower()).FirstOrDefault();
            var modelDtos = _mapper.Map<IList<TransactionDto>>(result.Transactions.Where(t => t.TransactionTypeId == transactionType?.Id).ToList());
            if (modelDtos == null)
                return Ok(new { data = new List<TransactionDto>() });
            SetTransactionDtoParameters(modelDtos);
            return Ok(modelDtos);
        }


        [HttpPost]
        public async Task<IActionResult> Post(TransactionDto transaction)
        {
            transaction.UserId = UserID;
            transaction.Photo = "";
            transaction.Description = transaction.Description ?? "";
            var model = _mapper.Map<Transaction>(transaction);
            await SetTransactionTypeId(transaction, model);
            await SetCategoryId(transaction, model);
            //await SetPaymentMethodId(transaction, model);
           
            var result = await TransactionService.Save(model, UserID);
            if (!result.Success)
            {
                return UnprocessableEntity(result);
            }
            var photo = Upload(Request, model.Id);
            if (!string.IsNullOrEmpty(photo))
            {
                result.Transaction.Photo = photo;
                await TransactionService.Save(result.Transaction, UserID);
            }
            var modelDto = _mapper.Map<TransactionDto>(result.Transaction);
            modelDto.Type = transaction.Type;
            modelDto.Category = transaction.Category;
            return Ok(modelDto);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, TransactionDto transaction)
        {
            if (id != transaction.Id)
                return BadRequest();

            transaction.UserId = UserID;
            transaction.Photo = "";
            transaction.Description = transaction.Description ?? "";
            var model = _mapper.Map<Transaction>(transaction);
            await SetTransactionTypeId(transaction, model);
            await SetCategoryId(transaction, model);
            //await SetPaymentMethodId(transaction, model);

            var result = await TransactionService.Save(model, UserID);
            if (!result.Success)
            {
                return UnprocessableEntity(result);
            }
            var photo = Upload(Request, model.Id);
            if (!string.IsNullOrEmpty(photo))
            {
                result.Transaction.Photo = photo;
                await TransactionService.Save(result.Transaction, UserID);
            }
            var modelDto = _mapper.Map<TransactionDto>(result.Transaction);
            modelDto.Type = transaction.Type;
            modelDto.Category = transaction.Category;
            return Ok(modelDto);
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
                    var fileName = transactionId + ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName?.Trim('"');
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

        #region Parameters


        private async Task SetCategoryId(TransactionDto transaction, Transaction model)
        {
            if (model.CategoryId > 0)
                return;
            var category = _categoryService.Query().Where(t => t.Name.Trim().ToLower() == transaction.Category?.Trim().ToLower()).FirstOrDefault();
            if (category == null)
            {
                if (!string.IsNullOrEmpty(transaction.Category))
                    category = await _categoryService.Add(new Category { Name = transaction.Category.Trim() });
            }
            model.CategoryId = category != null ? category.Id : 0;
        }

        private async Task SetTransactionTypeId(TransactionDto transaction, Transaction model)
        {
            if (model.TransactionTypeId > 0)
                return;
            var transactionType = _transactionTypeService.Query().Where(t => t.Name.Trim().ToLower() == transaction.Type?.Trim().ToLower()).FirstOrDefault();
            if (transactionType == null)
            {
                if (!string.IsNullOrEmpty(transaction.Type))
                    transactionType = await _transactionTypeService.Add(new TransactionType { Name = transaction.Type.Trim() });
            }
            model.TransactionTypeId = transactionType != null ? transactionType.Id : 0;
        }
        //private async Task SetPaymentMethodId(TransactionDto transaction, Transaction model)
        //{
        //    if (model.PaymentMethodId > 0)
        //        return;
        //    var paymentMethod = _paymentMethodService.Query().Where(t => t.Name.Trim().ToLower() == transaction.PaymentMethod?.Trim().ToLower()).FirstOrDefault();
        //    if (paymentMethod == null)
        //    {
        //        if (!string.IsNullOrEmpty(transaction.PaymentMethod))
        //            paymentMethod = await _paymentMethodService.Add(new PaymentMethod { Name = transaction.PaymentMethod.Trim() });
        //    }
        //    model.PaymentMethodId = paymentMethod?.Id == null || paymentMethod.Id == 0 ? null : paymentMethod.Id;
        //}
        #endregion

    }
}
