namespace AI_Service.Core.DTOs
{
    public record NotificationEventDto(
        Guid UserId,
        Guid RequestId,
        string Message,
        DateTime CreatedAt
    );
}
