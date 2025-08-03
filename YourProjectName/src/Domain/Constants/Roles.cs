namespace YourProjectName.Domain.Constants;

public abstract class Roles
{
    public const string Administrator = nameof(Administrator);
    public const string User = nameof(User);

    public static IReadOnlyList<string> All { get; } = new[] { Administrator, User };
}
