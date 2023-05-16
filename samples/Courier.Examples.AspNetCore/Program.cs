using Courier;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddAuthentication();
builder.Services.AddAuthorization();
builder.Services.AddCourier();
var app = builder.Build();
app.UseAuthentication();
app.UseAuthorization();
app.UseCourier();
app.Run();