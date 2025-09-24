using AutoInsightAPI.Dtos.Common;

namespace AutoInsightAPI.Dtos
{
  public class PagedResponseDto<T> : HateoasResourceDto
  {
      public int PageNumber { get; set; }
      public int PageSize { get; set; }
      public int TotalPages { get; set; }
      public int TotalRecords { get; set; }

      public List<T> Data { get; set; } = new();
  }
}
