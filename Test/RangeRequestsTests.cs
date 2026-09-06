using System.Net.Http.Headers;

namespace Test;

[TestClass]
public sealed class RangeRequestTests
{
    private static readonly MyWebApplication app = new();
    public required TestContext TestContext { get; set; }

    [TestMethod]
    public async Task Range()
    {
        var createRequest = new HttpRequestMessage(HttpMethod.Post, "");
        createRequest.Headers.Add("Slug", "/x3");
        createRequest.Content = new StringContent("Hello World", System.Text.Encoding.UTF8, "text/plain");

        var createResponse = await app.Client.SendAsync(createRequest, TestContext.CancellationToken);
        createResponse.Should().Be201Created();
        createResponse.Should().HaveHeader("Location");
        var createdLocation = createResponse.Headers.Location;

        var getResponse = await app.Client.GetAsync(createdLocation, TestContext.CancellationToken);
        getResponse.Should().Be200Ok();
        getResponse.Should().HaveHeader("Accept-Ranges").And.BeValue("bytes");
        var getResponseText = await (getResponse.Content as StreamContent).ReadAsStringAsync();
        getResponseText.Should().Be("Hello World");

        var rangeRequest = new HttpRequestMessage(HttpMethod.Get, createdLocation);
        rangeRequest.Headers.Range = new RangeHeaderValue(0, 4);
        var rangeRequestResponse = await app.Client.SendAsync(rangeRequest, TestContext.CancellationToken);
        rangeRequestResponse.Should().Be206PartialContent();
        var rangeRequestResponseText = await (getResponse.Content as StreamContent).ReadAsStringAsync();
        rangeRequestResponseText.Should().Be("Hello World");
    }
}
