
using StudentCourseManagement.Models;
using StudentCourseManagement.Repositories;

namespace StudentCourseManagement.Services;

public class AuditLogService
{
    private readonly AuditLogRepository _repository = new();

    public List<AuditLog> GetAllAuditLogs()
        => _repository.GetAllAuditLogs();

    public string AddAuditLog(
        string entityName, int entityId, string action, string? details)
    {
        if (string.IsNullOrWhiteSpace(entityName) ||
            string.IsNullOrWhiteSpace(action))
            return "Entity name and action are required.";

        if (entityId <= 0)
            return "Entity ID must be positive.";

        _repository.AddAuditLog(
            entityName.Trim(), entityId, action.Trim(), details);

        return "Audit log added.";
    }
}
