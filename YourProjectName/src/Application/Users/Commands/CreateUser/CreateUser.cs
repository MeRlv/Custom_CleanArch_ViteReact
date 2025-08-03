using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using YourProjectName.Application.Common.Interfaces;
using YourProjectName.Domain.Constants;
using YourProjectName.Domain.Entities;

namespace YourProjectName.Application.Users.Commands.CreateUser
{
    // Query that encapsulates the data needed to create a user
    public record CreateUserCommand(string Username, string Role) : IRequest<User>;

    // FluentValidation validator for the command
    public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
    {
        public CreateUserCommandValidator()
        {
            RuleFor(x => x.Username)
                .NotEmpty().WithMessage("Username is required.")
                .MaximumLength(100).WithMessage("Username must be at most 100 characters.");

            RuleFor(x => x.Role)
                .NotEmpty().WithMessage("Role is required.")
                .Must(r => Roles.All.Contains(r))
                .WithMessage(x => $"Role '{x.Role}' is not recognized. Allowed: {string.Join(", ", Roles.All)}");
        }
    }

    // The command handler that processes the CreateUserCommand
    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, User>
    {
        private readonly IApplicationDbContext _context;

        public CreateUserCommandHandler(IApplicationDbContext context)
            => _context = context;

        public async Task<User> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            // Vérifier l'unicité du username
            var exists = await _context.Users
                .AsNoTracking()
                .AnyAsync(u => u.Username == request.Username, cancellationToken);

            if (exists)
                throw new ValidationException($"Username '{request.Username}' already exists.");

            // Instancier la nouvelle entité (vérifications de domaine dans son constructeur)
            var user = new User(request.Username, request.Role);

            // Persister
            _context.Users.Add(user);
            await _context.SaveChangesAsync(cancellationToken);

            return user;
        }
    }
}
