using Microsoft.AspNetCore.Server.Kestrel.Core;
using WebApi;

await Host.CreateDefaultBuilder(args)
.ConfigureWebHostDefaults(webBuilder =>
{
    webBuilder.ConfigureKestrel(op => {
        op.ListenLocalhost(5001, o => o.Protocols = HttpProtocols.Http2);
        op.ListenLocalhost(5115, o => o.Protocols = HttpProtocols.Http1);
    });
    webBuilder.UseStartup<Startup>();
}).Build().RunAsync();