namespace InventoryValet;
    public static class QueryExtensions
{
    public static PagedResult<T> GetPaged<T>(this IQueryable<T> query,
                                             int page, int pageSize) where T : class
    {
        if (page <= 0) throw new ArgumentException("Page number should be greater than zero.");
        if (pageSize <= 0) throw new ArgumentException("Page size should be greater than zero.");

        var result = new PagedResult<T>();
        result.CurrentPage = page;
        result.PageSize = pageSize;
        result.RowCount = query.Count();

        var pageCount = (double)result.RowCount / pageSize;
        result.PageCount = (int)Math.Ceiling(pageCount);

        var skip = (page - 1) * pageSize;
        result.Results = query.Skip(skip).Take(pageSize).ToList();

        return result;
    }
}

public class PagedResult<T> where T : class
{
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
    public int RowCount { get; set; }
    public int PageCount { get; set; }
    public List<T> Results { get; set; }

    public PagedResult()
    {
        Results = new List<T>();
    }
}
