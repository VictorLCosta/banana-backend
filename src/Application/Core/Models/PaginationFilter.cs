namespace Application.Core.Models;

public class PaginationFilter
{
    public int PageNumber { get; set; }

    public int PageSize { get; set; } = int.MaxValue;
}
