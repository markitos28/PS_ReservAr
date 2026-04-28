using ReservAr.Models;
namespace ReservAr.Dtos.Events
{
    /// <summary>
    /// DTO para representar una respuesta paginada de eventos. Contiene una lista de eventos y metadatos relacionados con la paginación, como el número total de páginas, el número total de elementos, el número de página actual y el tamaño de página.
    /// </summary>
    public class PagedResponse<T>
    {
        public List<Event> Items { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public int TotalRecords { get; set; }

        public PagedResponse(List<Event> items, int count, int pageNumber, int pageSize)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
            TotalRecords = count;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            Items = items;
        }
    }
}