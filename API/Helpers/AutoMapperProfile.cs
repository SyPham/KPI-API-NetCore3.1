using API.Dtos;
using AutoMapper;
using Models.EF;

namespace API.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<User, UserForRegisterDto>();
            CreateMap<UserForRegisterDto, User>();

            //CreateMap<UserAccount, UserModel>();
            //CreateMap<RegisterModel, UserAccount>();
            //CreateMap<UpdateModel, UserAccount>();
        }
    }
}