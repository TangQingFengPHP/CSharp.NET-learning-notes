using EfCorePractice.Domain.Enums;

namespace EfCorePractice.Application.Models;

public record UserSummaryDto(long Id, string Username, string Email);

public record UserWithOrdersDto(
    long Id,
    string Username,
    string Email,
    int Age,
    string Status,
    int TenantId,
    UserContactDto Contact,
    IReadOnlyList<UserOrderItemDto> Orders);

public record UserOrderItemDto(
    long Id,
    string OrderNo,
    decimal Amount,
    OrderStatus Status,
    DateTime CreatedAt);

public record UserContactDto(string Email, string Phone, IReadOnlyList<string> Tags);

public record OrderUserDto(
    long OrderId,
    string OrderNo,
    decimal Amount,
    OrderStatus Status,
    long UserId,
    string Username,
    string Email);

public record QueryPlanDto(string LinQDescription, string Sql);

public record MigrationInfoDto(
    string DatabaseProvider,
    IReadOnlyList<string> AppliedMigrations,
    IReadOnlyList<string> PendingMigrations);

public record TransactionDemoResult(long UserId, long OrderId, string Message);

public record ChangeTrackingDemoDto(
    string InitialState,
    string AfterAttachModified,
    string AfterSave);

public record ApiError(string Message, string? Detail = null);
