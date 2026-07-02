using Microsoft.EntityFrameworkCore;

namespace ECommerce.Domain.Abstractions.Interceptors;

public interface ISoftDeleteInterceptor
{
    void ApplySoftDelete(DbContext context);
}
