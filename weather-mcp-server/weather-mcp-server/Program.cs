using mcp_shared.ChatGptBot.Ioc;
using ModelContextProtocol.Protocol;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services
    .AddMcpServer().WithHttpTransport(o=> /* required for open ai mco calls */ o.Stateless=true)
    .WithToolsFromAssembly()
    .WithPromptsFromAssembly()
    .WithListResourcesHandler(async (ctx, ct) =>
    {
        return new ListResourcesResult
        {
            Resources =
            [
                new Resource { Name = "Direct Text Resource", Description = "A direct text resource", MimeType = "text/plain", Uri = "file:///c:/Temp" },
            ]
        };
    })
      .WithReadResourceHandler(async (ctx, ct) =>
      {
          var uri = ctx.Params?.Uri;

          if (uri == "file:///c:/Temp")
          {
              Uri uris = new Uri(uri);
              string windowsPath = uris.LocalPath;
              var contents = new List<TextResourceContents>();
              Directory.GetFiles(windowsPath).ToList().ForEach(f =>
              {
                  contents.Add(new TextResourceContents
                  {
                      Text = File.ReadAllText(f),
                      MimeType = "text/plain",
                      Uri = f,
                  });   
              });   
              return new ReadResourceResult
              {
                  Contents = contents.Cast<ResourceContents>().ToList()
              };
          }
          return new ReadResourceResult
          {

          };
      });
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddHttpClient();

builder.Services.RegisterByConvention<Program>();
builder.Services.AddHttpLogging(logging =>
{
    logging.LoggingFields = Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.RequestBody |
    Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.ResponseBody |
    Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.ResponseHeaders|
    Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.ResponseStatusCode|
    Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.RequestHeaders ;
    logging.RequestBodyLogLimit = 4096;
    logging.ResponseHeaders.Add("mcp-session-id");
    logging.RequestHeaders.Add(" mcp-session-id");
    logging.ResponseBodyLogLimit = 4096;
    logging.CombineLogs = true;
});

var app = builder.Build();
app.UseHttpLogging();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
#if !DEBUG
app.UseHttpsRedirection();
#endif
app.UseAuthorization();

app.MapControllers();

app.MapMcp("mcp");

app.Run();
