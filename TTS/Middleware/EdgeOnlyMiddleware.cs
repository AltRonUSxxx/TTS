namespace TTS.Middleware
{
    public class EdgeOnlyMiddleware
    {
        private readonly RequestDelegate _next;
        public EdgeOnlyMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var path = context.Request.Path.Value?.ToLower() ?? "";
            if(path.StartsWith("/swagger"))
            {
                await _next(context);
                return;
            }

            var userAgent = context.Request.Headers.UserAgent.ToString();
            bool isEdge = userAgent.Contains("Edg/");
            if(!isEdge)
            {
                context.Response.ContentType = "text/html; charset=utf-8;";
                await context.Response.WriteAsync("""
                                        <!DOCTYPE html>
                    <html lang="en">
                    <head>
                        <meta charset="utf-8" />
                        <title> Require Microsoft Edge</edge>
                        <link rel="stylesheet" href="~/lib/bootstrap/dist/css/bootstrap.min.css" />
                        <link rel="stylesheet" href="~/css/site.css" asp-append-version="true" />
                        <link rel="stylesheet" href="~/TTS.styles.css" asp-append-version="true" />
                    </head>
                    <body>
                        <div class="container mt-5">
                            <div class="alert alert-warning">
                                <h1>Use Microsoft Edge for this site</h1>
                                <p>This project specialy requires Microsoft Edge</p>
                            </div>
                        </div>
                        
                    </body>
                    </html>
                    
                    """);

                return;
            }
            await _next(context);
        }
    }
}
