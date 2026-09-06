namespace Test;

[TestClass]
public sealed class ConditionalRequestTests
{
    private static readonly MyWebApplication app = new();
    public required TestContext TestContext { get; set; }

    [TestMethod]
    public async Task IfModifiedSince()
    {
        var createRequest = new HttpRequestMessage(HttpMethod.Post, "");
        createRequest.Headers.Add("Slug", "/x1");
        createRequest.Content = new StringContent("Hello World", System.Text.Encoding.UTF8, "text/plain");

        var createResponse = await app.Client.SendAsync(createRequest, TestContext.CancellationToken);
        createResponse.Should().Be201Created();
        createResponse.Should().HaveHeader("Location");
        var createdLocation = createResponse.Headers.Location;

        var getResponse = await app.Client.GetAsync(createdLocation, TestContext.CancellationToken);
        getResponse.Should().Be200Ok();
        getResponse.Should().HaveHeader("Last-Modified");
        var lastModified = getResponse.Content.Headers.LastModified;

        var modifiedAtRequest = new HttpRequestMessage(HttpMethod.Get, createdLocation);
        modifiedAtRequest.Headers.IfModifiedSince = lastModified;
        var modifiedAtResponse = await app.Client.SendAsync(modifiedAtRequest, TestContext.CancellationToken);
        modifiedAtResponse.Should().Be304NotModified();

        var modifiedBeforeRequest = new HttpRequestMessage(HttpMethod.Get, createdLocation);
        modifiedBeforeRequest.Headers.IfModifiedSince = lastModified - TimeSpan.FromSeconds(1);
        var modifiedBeforeResponse = await app.Client.SendAsync(modifiedBeforeRequest, TestContext.CancellationToken);
        modifiedBeforeResponse.Should().Be200Ok();

        // Otherwise we're too fast for one second date header resolution.
        await Task.Delay(2000, TestContext.CancellationToken);

        var modifiedAfterRequest = new HttpRequestMessage(HttpMethod.Get, createdLocation);
        modifiedAfterRequest.Headers.IfModifiedSince = lastModified + TimeSpan.FromSeconds(1);
        var modifiedAfterResponse = await app.Client.SendAsync(modifiedAfterRequest, TestContext.CancellationToken);
        modifiedAfterResponse.Should().Be304NotModified();
    }

    [TestMethod]
    public async Task IfUnmodifiedSince()
    {
        var createRequest = new HttpRequestMessage(HttpMethod.Post, "");
        createRequest.Headers.Add("Slug", "/x2");
        createRequest.Content = new StringContent("Hello World", System.Text.Encoding.UTF8, "text/plain");

        var createResponse = await app.Client.SendAsync(createRequest, TestContext.CancellationToken);
        createResponse.Should().Be201Created();
        createResponse.Should().HaveHeader("Location");
        var createdLocation = createResponse.Headers.Location;

        var getResponse = await app.Client.GetAsync(createdLocation, TestContext.CancellationToken);
        getResponse.Should().Be200Ok();
        getResponse.Should().HaveHeader("Last-Modified");
        var lastModified = getResponse.Content.Headers.LastModified;

        var modifiedAtRequest = new HttpRequestMessage(HttpMethod.Get, createdLocation);
        modifiedAtRequest.Headers.IfUnmodifiedSince = lastModified;
        var modifiedAtResponse = await app.Client.SendAsync(modifiedAtRequest, TestContext.CancellationToken);
        modifiedAtResponse.Should().Be200Ok();

        var modifiedBeforeRequest = new HttpRequestMessage(HttpMethod.Get, createdLocation);
        modifiedBeforeRequest.Headers.IfUnmodifiedSince = lastModified - TimeSpan.FromSeconds(1);
        var modifiedBeforeResponse = await app.Client.SendAsync(modifiedBeforeRequest, TestContext.CancellationToken);
        modifiedBeforeResponse.Should().Be412PreconditionFailed();

        // Otherwise we're too fast for one second date header resolution.
        await Task.Delay(2000, TestContext.CancellationToken);

        var modifiedAfterRequest = new HttpRequestMessage(HttpMethod.Get, createdLocation);
        modifiedAfterRequest.Headers.IfUnmodifiedSince = lastModified + TimeSpan.FromSeconds(1);
        var modifiedAfterResponse = await app.Client.SendAsync(modifiedAfterRequest, TestContext.CancellationToken);
        modifiedAfterResponse.Should().Be200Ok();
    }
}
