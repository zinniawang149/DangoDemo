using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DangoAPI.Helpers
{
    public class PaginationHeader
    {
        public int CurrentPage { get; set; }
        public int ItemsPerPage { get; set; }
        public int TotalItems { get; set; }
        public int TotelPages { get; set; }
        public PaginationHeader(int currentPage, int itemsPerPage, int totalItems, int totelPages)
        {
            this.CurrentPage = currentPage;
            this.ItemsPerPage = itemsPerPage;
            this.TotalItems = totalItems;
            this.TotelPages = totelPages;
        }
    }
}
