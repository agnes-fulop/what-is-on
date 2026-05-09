using System.Net;
using System.Net.Http.Json;
using Shouldly;

namespace WhatIsOn.Tests.Integration;

public class AuthFlowTests : IClassFixture<WhatIsOnWebApplicationFactory>
{
    private readonly HttpClient _client;

    public AuthFlowTests(WhatIsOnWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Register_ValidInput_ReturnsTokenAndUser()
    {
        var response = await _client.PostAsJsonAsync("/api/auth/register", new
        {
            email = $"newuser-{Guid.NewGuid()}@example.com",
            displayName = "New User",
            password = "test-password-123",
        });

        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var body = await response.Content.ReadFromJsonAsync<AuthResponse>();
        body.ShouldNotBeNull();
        body.Token.ShouldNotBeNullOrWhiteSpace();
        body.User.Role.ShouldBe("Regular");
    }

    [Fact]
    public async Task Login_SeededUser_ReturnsToken()
    {
        var response = await _client.PostAsJsonAsync("/api/auth/login", new
        {
            email = "regular@example.com",
            password = "demo-password-123",
        });

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<AuthResponse>();
        body.ShouldNotBeNull();
        body.User.Email.ShouldBe("regular@example.com");
    }

    [Fact]
    public async Task Login_BadPassword_Returns401()
    {
        var response = await _client.PostAsJsonAsync("/api/auth/login", new
        {
            email = "regular@example.com",
            password = "wrong-password",
        });

        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    private record AuthResponse(string Token, DateTime ExpiresAtUtc, AuthUser User);
    private record AuthUser(Guid Id, string Email, string DisplayName, string Role);
}
