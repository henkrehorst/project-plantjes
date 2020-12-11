using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

//inspired by: https://docs.microsoft.com/en-us/aspnet/core/data/ef-mvc/sort-filter-page?view=aspnetcore-5.0#add-paging-links
namespace project_c.Helpers
{
    public class PaginatedResponse<T>
    {
        public int PageIndex { get; private set; }
        public int TotalPages { get; private set; }
        
        public int Count { get; private set; }

        public List<T> Items { get; private set; }

        public PaginatedResponse(List<T> items, int count, int pageIndex, int pageSize)
        {
            PageIndex = pageIndex;
            TotalPages = (int) Math.Ceiling(count / (double) pageSize);
            RightPagingNumbers = CalculateRightPagingNumbers();
            LeftPagingNumbers = CalculateLeftPagingNumbers();
            Items = items;
            Count = count;
        }

        public bool HasMultiplePages
        {
            get { return TotalPages > 1; }
        }

        public bool HasPreviousPage
        {
            get { return (PageIndex > 1); }
        }

        public bool HasNextPage
        {
            get { return (PageIndex < TotalPages); }
        }

        public bool HiddenPages
        {
            get { return TotalPages > 6; }
        }

        public int[] LeftPagingNumbers { get; private set; }
        
        private int[] CalculateLeftPagingNumbers()
        {
            if (HiddenPages)
            {
                if (PageIndex < TotalPages / 2)
                {
                    if (PageIndex == 1) return Enumerable.Range(1, 3).ToArray();
                    return Enumerable.Range(PageIndex - 1, 3).ToArray();
                }

                if (PageIndex == 4) return Enumerable.Range(1, 2).ToArray();
                return Enumerable.Range(1, 3).ToArray();
            }

            return Enumerable.Range(1, TotalPages).ToArray();
        }

        public int[] RightPagingNumbers { get; private set; }

        private int[] CalculateRightPagingNumbers()
        {
            if (HiddenPages)
            {
                if ((double) PageIndex >= (double) TotalPages / 2.0)
                {
                    if (PageIndex == TotalPages) return Enumerable.Range(PageIndex - 2, 3).ToArray();
                    return Enumerable.Range(PageIndex - 1, 3).ToArray();
                }

                return Enumerable.Range(TotalPages - 2, 3).ToArray();
            }

            return new int[] { };
        }

        public static async Task<PaginatedResponse<T>> CreateAsync(IQueryable<T> source, int pageIndex, int pageSize)
        {
            var count = await source.CountAsync();
            var items = await source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
            return new PaginatedResponse<T>(items, count, pageIndex, pageSize);
        }
    }
}