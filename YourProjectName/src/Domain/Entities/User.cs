using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using YourProjectName.Domain.Common;
using YourProjectName.Domain.Constants;

namespace YourProjectName.Domain.Entities;

[Index(nameof(Username), Name = "IX_User_Username", IsUnique = true)]
public class User : BaseEntity
{
    private User() { }

    public User(string username, string role)
    {
      if (string.IsNullOrWhiteSpace(username))
        throw new ArgumentException("The username is required.", nameof(username));

      if (!Roles.All.Contains(role))
        throw new ArgumentException($"The role '{role}' is not recognized.", nameof(role));

      Username = username;
      Role = role;
    }

    [Required]
    public string Username { get; private set; } = default!;

    [Required]
    public string Role { get; private set; } = default!;
}

