using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using YourProjectName.Application.Common.Interfaces;
using YourProjectName.Application.Common.Models;
using YourProjectName.Domain.Constants;
using YourProjectName.Domain.Entities;
using YourProjectName.Infrastructure.Data;

namespace YourProjectName.Infrastructure.Identity
{
  public class IdentityService : IIdentityService
  {
    private readonly ApplicationDbContext _context;

    public IdentityService(ApplicationDbContext context)
    {
      _context = context;
    }

    public async Task<string?> GetUserNameAsync(int userId)
    {
      var user = await _context.Users
        .AsNoTracking()
        .FirstOrDefaultAsync(u => u.Id == userId);

      return user?.Username;
    }

    public async Task<bool> IsInRoleAsync(int userId, string role)
    {
      var user = await _context.Users
        .AsNoTracking()
        .FirstOrDefaultAsync(u => u.Id == userId);

      return user is not null
        && string.Equals(user.Role, role, StringComparison.OrdinalIgnoreCase);
    }

    public async Task<bool> AuthorizeAsync(int userId, string policyName)
    {
      // Retrieve the user
      var user = await _context.Users
        .AsNoTracking()
        .FirstOrDefaultAsync(u => u.Id == userId);

      if (user is null)
        return false;

      // Apply business logic for each policy
      return policyName switch
      {
        Policies.CanPurge => string.Equals(user.Role, Roles.Administrator, StringComparison.OrdinalIgnoreCase),
        // Add other policies here if needed:
        // Policies.CanEdit   => user.Role == Roles.Editor,
        _ => false
      };
    }

    public async Task<(Result Result, int UserId)> CreateUserAsync(string userName)
    {
      // Basic validation
      if (string.IsNullOrWhiteSpace(userName))
        return (Result.Failure(new[] { "Username is required." }), 0);

      // Check for uniqueness
      var exists = await _context.Users
        .AsNoTracking()
        .AnyAsync(u => u.Username == userName);
      if (exists)
        return (Result.Failure(new[] { "Username already exists." }), 0);

      // Create user with default role
      var user = new User(userName, Roles.User);
      _context.Users.Add(user);
      await _context.SaveChangesAsync();

      return (Result.Success(), user.Id);
    }

    public async Task<Result> DeleteUserAsync(int userId)
    {
      var user = await _context.Users.FindAsync(userId);
      if (user is null)
        return Result.Failure(new[] { "User not found." });

      _context.Users.Remove(user);
      await _context.SaveChangesAsync();

      return Result.Success();
    }
  }
}
