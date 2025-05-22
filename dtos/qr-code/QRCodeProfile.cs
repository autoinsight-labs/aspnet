using AutoInsightAPI.Dtos;
using AutoInsightAPI.Models;
using AutoMapper;

namespace AutoInsightAPI.Profiles
{
  public class QRCodeProfile: Profile
  {
    public QRCodeProfile()
    {
      CreateMap<QRCode, QRCodeDTO>();
      CreateMap<QRCodeDTO, QRCode>()
       .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember is not null));
    }
  }
}
