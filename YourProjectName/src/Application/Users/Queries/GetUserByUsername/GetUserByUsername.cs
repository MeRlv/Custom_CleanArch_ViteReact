using System;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using YourProjectName.Application.Common.Interfaces;
using YourProjectName.Domain.Entities;

namespace YourProjectName.Application.Users.Queries.GetUserByUsername
{
    /// <summary>
    /// Query to retrieve a User by their username.
    /// </summary>
    public record GetUserByUsernameQuery(string Username) : IRequest<User>;

    /// <summary>
    /// Validator for GetUserByUsernameQuery.
    /// </summary>
    public class GetUserByUsernameQueryValidator : AbstractValidator<GetUserByUsernameQuery>
    {
        public GetUserByUsernameQueryValidator()
        {
            RuleFor(q => q.Username)
                .NotEmpty().WithMessage("Username is required.");
        }
    }

    /// <summary>
    /// Handler for GetUserByUsernameQuery.
    /// </summary>
    public class GetUserByUsernameQueryHandler : IRequestHandler<GetUserByUsernameQuery, User>
    {
        private readonly IApplicationDbContext _context;

        public GetUserByUsernameQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<User> Handle(GetUserByUsernameQuery request, CancellationToken cancellationToken)
        {
            var user = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Username == request.Username, cancellationToken);

            if (user is null)
            {
                throw new KeyNotFoundException($"User '{request.Username}' not found.");
            }

            return user;
        }
    }
}
