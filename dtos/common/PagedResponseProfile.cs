using AutoInsightAPI.Models;
using AutoInsightAPI.Dtos;
using AutoMapper;

namespace AutoInsightAPI.Repositories
{
  public class PagedResponseProfile : Profile
  {
     public PagedResponseProfile()
     {
         CreateMap(typeof(PagedResponse<>), typeof(PagedResponseDTO<>));
     }
  }
}
