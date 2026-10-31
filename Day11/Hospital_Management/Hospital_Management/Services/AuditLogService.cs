
using HospitalAppointmentManagement.Models;
using HospitalAppointmentManagement.Repositories;

namespace HospitalAppointmentManagement.Services;

public class AuditLogService
{
    private readonly AuditLogRepository _auditLogRepository;

    public AuditLogService(AuditLogRepository auditLogRepository)
    {
        _auditLogRepository = auditLogRepository;
    }

    // Validate and save an audit log
    public async Task LogAsync(
        string entityName,
        int entityId,
        string action,
        string? details = null)
    {
        if (string.IsNullOrWhiteSpace(entityName))
        {
            throw new ArgumentException(
                "Entity name cannot be empty.");
        }

        if (entityId <= 0)
        {
            throw new ArgumentException(
                "Entity ID must be greater than zero.");
        }

        if (string.IsNullOrWhiteSpace(action))
        {
            throw new ArgumentException(
                "Action cannot be empty.");
        }

        AuditLog auditLog = new AuditLog
        {
            EntityName = entityName,
            EntityId = entityId,
            Action = action,
            Details = details
        };

        await _auditLogRepository.LogAsync(auditLog);
    }

    // Retrieve all audit logs
    public async Task<List<AuditLog>> GetAllAsync()
    {
        return await _auditLogRepository.GetAllAsync();
    }
}
