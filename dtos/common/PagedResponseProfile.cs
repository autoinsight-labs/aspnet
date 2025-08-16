using AutoInsightAPI.Models;
using AutoInsightAPI.Dtos;
using AutoMapper;

namespace AutoInsightAPI.Profiles
{
  public class PagedResponseProfile : Profile
  {
     public PagedResponseProfile()
     {
         CreateMap(typeof(PagedResponse<>), typeof(PagedResponseDto<>));
     }
  }
}
