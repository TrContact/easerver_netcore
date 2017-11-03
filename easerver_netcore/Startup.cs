using Microsoft.AspNetCore.Builder;
using Nancy.Owin;

namespace easerver_netcore
{
    public class Startup
    {
            public void Configure(IApplicationBuilder app)
            {
                app.UseOwin(x => x.UseNancy());
            }
        }
    }
