namespace ProductManagement.Shared.Dtos;

public class PaginatedResult<T>(
    IEnumerable<T> items, 
    int totalCount, 
    int pageNumber, 
    int pageSize)
{
    public IEnumerable<T> Items => items;
    public int TotalCount => totalCount;
    public int PageNumber => pageNumber;
    public int PageSize => pageSize;
    public int TotalPages
    {
        get
        {
            var totalPages = (int)Math.Ceiling((double)TotalCount / PageSize);
            return totalPages < 1 ? 1 : totalPages;
        }
    }
}