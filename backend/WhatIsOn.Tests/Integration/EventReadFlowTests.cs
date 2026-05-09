using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Shouldly;

namespace WhatIsOn.Tests.Integration;

public class EventReadFlowTests : IClassFixture<WhatIsOnWebApplicationFactory>
{
    // Stable IDs from SeedData (verified by manual inspection of the seed).
    private static readonly Guid AwsSummitId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
    private static readonly Guid VipRoundtableId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");

    private readonly WhatIsOnWebApplicationFactory _factory;

    public EventReadFlowTests(WhatIsOnWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task ListEvents_ReturnsSeededCatalog()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/events");

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);
        doc.RootElement.GetArrayLength().ShouldBe(4);
    }

    [Fact]
    public async Task GetVipEvent_Anonymous_Returns401()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync($"/api/events/{VipRoundtableId}");

        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetVipEvent_RegularUser_Returns403()
    {
        var client = _factory.CreateClient();
        var token = await LoginAsync(client, "regular@example.com");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await client.GetAsync($"/api/events/{VipRoundtableId}");

        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetVipEvent_VipUser_ReturnsFullDetailWithLayout()
    {
        var client = _factory.CreateClient();
        var token = await LoginAsync(client, "vip@example.com");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await client.GetAsync($"/api/events/{VipRoundtableId}");

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);
        doc.RootElement.GetProperty("isVip").GetBoolean().ShouldBeTrue();
        doc.RootElement.GetProperty("layout").ValueKind.ShouldBe(JsonValueKind.Object);
        doc.RootElement.GetProperty("layout").GetProperty("components")
            .GetArrayLength().ShouldBeGreaterThan(0);
    }

    [Fact]
    public async Task GetPublicEvent_Anonymous_ReturnsLayoutWithEnrichedSpeakerCard()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync($"/api/events/{AwsSummitId}");

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var json = await response.Content.ReadAsStringAsync();

        // The AWS layout has SpeakerCards that should be enriched with full
        // speaker objects, not just IDs.
        json.ShouldContain("Dr. Jane Doe");
        json.ShouldContain("\"speaker\":{");
    }

    private static async Task<string> LoginAsync(HttpClient client, string email)
    {
        var response = await client.PostAsJsonAsync("/api/auth/login", new
        {
            email,
            password = "demo-password-123",
        });
        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadFromJsonAsync<JsonDocument>();
        return body!.RootElement.GetProperty("token").GetString()!;
    }
}
