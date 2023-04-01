using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Dtos;

namespace API.Helpers
{
    public class Pagination<T> where T : class
    {
        public IReadOnlyList<T> Data { get; set;}
        public int Count { get; set;}
        public int PageIndex { get;set; }
        public int PageSize { get;set; }
        public string Search {get;set;}
        public Pagination(IReadOnlyList<T> data, int count, int pageIndex, int pageSize)
        {
            Data = data;
            Count = count;
            PageIndex = pageIndex;
            PageSize = pageSize;

        }

        // public int TotalPages => (int)Math.Ceiling(decimal.Divide(Count, PageSize));
        // public bool HasPreviousPage => PageIndex > 1;
        // public bool HasNextPage => PageIndex < TotalPages;
        // public int? NextPageNumber => HasNextPage ? PageIndex + 1 : (int?)null;
        // public int? PreviousPageNumber => HasPreviousPage ? PageIndex - 1 : (int?)null;
    }

}