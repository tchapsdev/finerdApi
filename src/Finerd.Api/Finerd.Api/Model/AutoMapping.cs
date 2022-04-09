using AutoMapper;
using Finerd.Api.Model.Entities;

namespace Finerd.Api.Model
{
    public class AutoMapping : Profile
    {
        public AutoMapping()
        {
            CreateMap<User, UserDto>(); // means you want to map from User to UserDTO
            CreateMap<UserDto, User>(); // means you want to map from UserDto to User

            CreateMap<Transaction, TransactionDto>(); // means you want to map from Transaction to TransactionDto
            CreateMap<TransactionDto, Transaction>(); // means you want to map from TransactionDto to Transaction
        }
    }
}
