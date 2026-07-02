using Microsoft.EntityFrameworkCore;

namespace ECommerce.Domain.Abstractions.Interceptors;

public interface IAuditInterceptor
{
    void ApplyAudit(DbContext context);
}
