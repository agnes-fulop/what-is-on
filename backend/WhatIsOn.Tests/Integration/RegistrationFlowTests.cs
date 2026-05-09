using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Shouldly;

namespace WhatIsOn.Tests.Integration;

public class RegistrationFlowTests : IClassFixture<WhatIsOnWebApplicationFactory>
{
    private static readonly Guid AwsSummitId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
    private static readonly Guid VipRoundtableId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");
    private static readonly Guid PastSummitId = Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee");

    private readonly WhatIsOnWebApplicationFactory _factory;

    public RegistrationFlowTests(WhatIsOnWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Register_Anonymous_Returns401()
    {
        var client = _factory.CreateClient();

        var response = await client.PostAsync(
            $"/api/events/{AwsSummitId}/registrations", content: null);

        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Register_RegularUser_PublicEvent_Returns201()
    {
        var client = await ClientWithFreshUserAsync();

        var response = await client.PostAsync(
            $"/api/events/{AwsSummitId}/registrations", content: null);

        response.StatusCode.ShouldBe(HttpStatusCode.Created);
    }

    [Fact]
    public async Task Register_RegularUser_VipEvent_Returns403()
    {
        var client = await ClientWithFreshUserAsync();

        var response = await client.PostAsync(
            $"/api/events/{VipRoundtableId}/registrations", content: null);

        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Register_PastEvent_Returns400()
    {
        var client = await ClientWithFreshUserAsync();

        var response = await client.PostAsync(
            $"/api/events/{PastSummitId}/registrations", content: null);

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        var body = await response.Content.ReadAsStringAsync();
        body.ShouldContain("closed");
    }

    [Fact]
    public async Task Register_Twice_SecondAttemptReturns409()
    {
        var client = await ClientWithFreshUserAsync();

        var first = await client.PostAsync($"/api/events/{AwsSummitId}/registrations", content: null);
        first.StatusCode.ShouldBe(HttpStatusCode.Created);

        var second = await client.PostAsync($"/api/events/{AwsSummitId}/registrations", content: null);
        second.StatusCode.ShouldBe(HttpStatusCode.Conflict);
    }

    /// <summary>
    /// Each test gets its own freshly-registered user so the duplicate-check
    /// across tests doesn't trip.
    /// </summary>
    private async Task<HttpClient> ClientWithFreshUserAsync()
    {
        var client = _factory.CreateClient();
        var registerResponse = await client.PostAsJsonAsync("/api/auth/register", new
        {
            email = $"reg-{Guid.NewGuid()}@example.com",
            displayName = "Test User",
            password = "test-password-123",
        });
        registerResponse.EnsureSuccessStatusCode();

        var body = await registerResponse.Content.ReadFromJsonAsync<JsonDocument>();
        var token = body!.RootElement.GetProperty("token").GetString()!;
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return client;
    }
}
