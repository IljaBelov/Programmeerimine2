using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace autorentimineProjekt.ToDoApi.Application.Common.Paging
{
    [ExcludeFromCodeCoverage]
    public class PagedResult<T> : PagedResultBase
    {
        public IList<T> Results { get; set; }

        public PagedResult()
        {
            Results = new List<T>();
        }
    }
}