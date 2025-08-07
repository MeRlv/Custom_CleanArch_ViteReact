// src/Web/Endpoints/SecurityEndpoints.cs
using System;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using YourProjectName.Application.Common.Interfaces;
using YourProjectName.Application.Common.Models;
using YourProjectName.Application.Users.Queries.GetUserByUsername;
using YourProjectName.Domain.Entities;
using YourProjectName.Web.Infrastructure;

namespace YourProjectName.Web.Endpoints;

public class SecurityEndpoints : EndpointGroupBase
{
    public override string? GroupName => "security";

    public override void Map(RouteGroupBuilder group)
    {
        group.MapPost("/login", Login)
            .WithName("LoginUser")
            .AllowAnonymous()
            .Produces<AuthResponse>(200)
            .Produces(400);

        group.MapPost("/refresh", Refresh)
            .WithName("RefreshToken")
            .AllowAnonymous()
            .Produces<AuthResponse>(200)
            .Produces(401);
    }

    // DTOs
    public record LoginRequest(string Username, string Password);
    public record RefreshRequest(User User, string RefreshToken);
    public record AuthResponse(User User, TokenModel AccessToken, TokenModel RefreshToken);

    public async Task<IResult> Login(
        ISender sender,
        ITokenService tokenService,
        [FromBody] LoginRequest request)
    {
        try
        {
            // Maybe log the user with his password
            if (string.IsNullOrWhiteSpace(request.Password))
            {
                return TypedResults.BadRequest(new { Message = "Password is required." });
            }

            // Ensure user exists via the query
            var user = await sender.Send(new GetUserByUsernameQuery(request.Username));
            // Generate tokens
            var access = tokenService.GenerateAccessTokenFromUsername(request.Username);
            var refresh = tokenService.GenerateRefreshTokenFromUsername(request.Username);

            return TypedResults.Ok(new AuthResponse(user, access, refresh));
        }
        catch (Exception ex)
        {
            return HandleException(ex);
        }
    }

    public async Task<IResult> Refresh(
        ITokenService tokenService,
        [FromBody] RefreshRequest request)
    {
        try
        {
            // Consume the refresh token and generate new tokens
            var access = tokenService.ConsumeRefreshToken(request.User.Username, request.RefreshToken);
            var refresh = tokenService.GenerateRefreshTokenFromUsername(request.User.Username);

            // To satisfy async/await requirement
            await Task.CompletedTask;

            return TypedResults.Ok(new AuthResponse(request.User, access, refresh));
        }
        catch (Exception ex)
        {
            return HandleException(ex);
        }
    }
}
