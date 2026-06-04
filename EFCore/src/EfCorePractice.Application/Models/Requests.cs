using System.ComponentModel.DataAnnotations;
using EfCorePractice.Domain.Entities;

namespace EfCorePractice.Application.Models;

public record UserCreateRequest(
    [Required][MaxLength(50)] string Username,
    [Required][EmailAddress][MaxLength(100)] string Email,
    [Range(1, 150)] int Age,
    UserContactProfile? Contact = null);

public record UserUpdateEmailRequest(
    [Required][EmailAddress][MaxLength(100)] string Email);

public record UserSearchRequest(
    string? Keyword,
    string? Status,
    int? MinAge);

public record CreateUserWithOrderRequest(
    [Required] UserCreateRequest User,
    [Required][MaxLength(50)] string OrderNo,
    [Range(0.01, 999999)] decimal Amount);
