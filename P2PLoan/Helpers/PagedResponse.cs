using System;

namespace P2PLoan.Helpers;

public class PagedResponse<T>
{

    public T Items { get; set; }
    public int TotalItems { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }

}
