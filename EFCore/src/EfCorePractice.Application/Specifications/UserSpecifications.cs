using EfCorePractice.Domain.Entities;

namespace EfCorePractice.Application.Specifications;

public static class UserSpecifications
{
    public static IQueryable<User> ApplySearch(
        this IQueryable<User> query,
        string? keyword,
        string? status,
        int? minAge)
    {
        if (!string.IsNullOrWhiteSpace(keyword))
        {
            query = query.Where(u =>
                u.Username.Contains(keyword) || u.Email.Contains(keyword));
        }

        if (!string.IsNullOrWhiteSpace(status))
        {
            query = query.Where(u => u.Status == status);
        }

        if (minAge.HasValue)
        {
            query = query.Where(u => u.Age >= minAge.Value);
        }

        return query;
    }
}
