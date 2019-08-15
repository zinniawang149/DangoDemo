using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DangoAPI.Helpers
{
    public class PagedList<T>:List<T>
    {
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }

        public PagedList(List<T> items, int count, int pageNumber, int pageSize)
        {
            TotalCount = count;
            PageSize = pageSize;
            CurrentPage = pageNumber;
            TotalPages = (int)Math.Ceiling(count / (double)PageSize);
            this.AddRange(items);
        }
        public static async Task<PagedList<T>> CreatAsync(IQueryable<T> source, int pageNumer, int pageSize) {
            int count = await source.CountAsync();
            List<T> items = await source.Skip(pageSize * (pageNumer - 1)).Take(pageSize).ToListAsync();
            return new PagedList<T>(items, count, pageNumer, pageSize);
        }
    }
}
