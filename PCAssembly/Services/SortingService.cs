namespace PCAssembly.Services
{
    public class SortingService
    {
        public static IQueryable<Assembly> SortAssemblies(IQueryable<Assembly> query, string sortColumn, string sortDirection)
        {
            return sortColumn switch
            {
                "AssemblyName" => sortDirection == "asc" ? query.OrderBy(a => a.AssemblyName) : query.OrderByDescending(a => a.AssemblyName),
                "Avgrating" => sortDirection == "asc" ? query.OrderBy(a => a.Avgrating) : query.OrderByDescending(a => a.Avgrating),
                "User" => sortDirection == "asc" ? query.OrderBy(a => a.User.UserName) : query.OrderByDescending(a => a.User.UserName),
                _ => query.OrderBy(a => a.AssemblyName) // По умолчанию сортируем по имени сборки
            };
        }

        public static IQueryable<Component> SortComponents(IQueryable<Component> query, string sortColumn, string sortDirection)
        {
            return sortColumn switch
            {
                "Name" => sortDirection == "asc" ? query.OrderBy(c => c.Name) : query.OrderByDescending(c => c.Name),
                "Description" => sortDirection == "asc" ? query.OrderBy(c => c.Description) : query.OrderByDescending(c => c.Description),
                "Price" => sortDirection == "asc" ? query.OrderBy(c => c.Price) : query.OrderByDescending(c => c.Price),
                "Type" => sortDirection == "asc" ? query.OrderBy(c => c.TypeComponents.TypeName) : query.OrderByDescending(c => c.TypeComponents.TypeName),
                _ => query
            };
        }

        public static IQueryable<Review> ReviewsSorting(IQueryable<Review> reviewsQuery, string sortOrder, string sortDirection)
        {
            switch (sortOrder)
            {
                case "ReviewText":
                    reviewsQuery = sortDirection == "asc" ? reviewsQuery.OrderBy(r => r.ReviewText) : reviewsQuery.OrderByDescending(r => r.ReviewText);
                    break;
                case "Rating":
                    reviewsQuery = sortDirection == "asc" ? reviewsQuery.OrderBy(r => r.Rating) : reviewsQuery.OrderByDescending(r => r.Rating);
                    break;
                case "Assembly":
                    reviewsQuery = sortDirection == "asc" ? reviewsQuery.OrderBy(r => r.Assembly.AssemblyName) : reviewsQuery.OrderByDescending(r => r.Assembly.AssemblyName);
                    break;
                default:
                    reviewsQuery = reviewsQuery.OrderBy(r => r.ReviewText); // По умолчанию сортировка по тексту отзыва
                    break;
            }
            return reviewsQuery;
        }
    }
}
