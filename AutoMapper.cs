using AutoMapper;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<Yard, GetYardDTO>();
        CreateMap<YardEmployee, GetYardEmployeeDTO>();
        CreateMap<YardVehicle, GetYardVehicleDTO>();
    }
}
