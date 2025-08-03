// src/Web/Endpoints/SecurityEndpoints.cs
using System;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;
using YourProjectName.Application.Common.Interfaces;
using YourProjectName.Application.Common.Models;
using YourProjectName.Application.Users.Queries.GetUserByUsername;
using YourProjectName.Web.Infrastructure;

namespace YourProjectName.Web.Endpoints;

public class SecurityEndpoints : EndpointGroupBase
{
    public override string? GroupName => "/security";

    public override void Map(RouteGroupBuilder group)
    {
        group.MapPost("/login", Login)
             .AllowAnonymous()
             .Produces<Ok<AuthResponse>>()
             .Produces<BadRequest>();

        group.MapPost("/refresh", Refresh)
             .AllowAnonymous()
             .Produces<Ok<AuthResponse>>()
             .Produces<UnauthorizedHttpResult>();
    }

    // DTOs
    public record LoginRequest(string Username);
    public record RefreshRequest(string RefreshToken);
    public record AuthResponse(TokenModel AccessToken, TokenModel RefreshToken);

    // Handler for /security/login
    public async Task<Results<Ok<AuthResponse>, BadRequest>> Login(
        ISender sender,
        ITokenService tokenService,
        [AsParameters] LoginRequest request)
    {
        try
        {
            // Ensure user exists via the query
            var user = await sender.Send(new GetUserByUsernameQuery(request.Username));

            // Generate tokens
            var (access, refresh) = await tokenService.GenerateTokensAsync(request.Username);
            return TypedResults.Ok(new AuthResponse(access, refresh));
        }
        catch (KeyNotFoundException)
        {
            return TypedResults.BadRequest();
        }
    }

    // Handler for /security/refresh
    public async Task<Results<Ok<AuthResponse>, UnauthorizedHttpResult>> Refresh(
        ITokenService tokenService,
        [AsParameters] RefreshRequest request)
    {
        var tokens = await tokenService.RefreshTokensAsync(request.RefreshToken);
        if (tokens is null)
            return TypedResults.Unauthorized();

        var (newAccess, newRefresh) = tokens.Value;
        return TypedResults.Ok(new AuthResponse(newAccess, newRefresh));
    }
}
