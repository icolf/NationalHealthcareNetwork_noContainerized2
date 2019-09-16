using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Organizations.Api.Helpers
{
    public class PageList<T> : List<T>
    {
        public int CurrentPage { get; private set; }

        public int TotalPages { get; private set; }

        public int PageSize { get; private set; }

        public int TotalCount { get; private set; }

        public bool HasPrevious => (CurrentPage>1);

        public bool HasNext => (CurrentPage < TotalPages);

        public PageList(List<T> items, int currentPage, int pageSize, int totalCount)
        {
            TotalCount = totalCount;
            PageSize = pageSize;
            CurrentPage = currentPage;
            TotalPages = (int) Math.Ceiling(totalCount / (double) pageSize);
            AddRange(items);
        }

        public static async Task<PageList<T>> Create(IQueryable<T> source, int currentPage, int pageSize)
        {
            var count = await source.CountAsync();
            var items = await source.Skip((currentPage - 1) * pageSize).Take(pageSize).ToListAsync();
            return new PageList<T>(items, currentPage, pageSize, count);
        }
    }
}
