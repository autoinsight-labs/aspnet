using AutoInsightAPI.Dtos;
using AutoInsightAPI.Models;
using AutoMapper;

namespace AutoInsightAPI.Profiles
{
  public class AddressProfile: Profile
  {
    public AddressProfile()
    {
      CreateMap<Address, AddressDto>();
      CreateMap<AddressDto, Address>()
       .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember is not null));
    }
  }
}
