using AutoInsightAPI.Dtos;
using AutoInsightAPI.Models;
using AutoMapper;

namespace AutoInsightAPI.Profiles
{
  public class QRCodeProfile: Profile
  {
    public QRCodeProfile()
    {
      CreateMap<QRCode, QrCodeDto>();
      CreateMap<QrCodeDto, QRCode>()
       .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember is not null));
    }
  }
}
