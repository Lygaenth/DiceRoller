using DiceRoller.Core.Apis;
using DiceRollerServer.Hubs;
using DiceRollerServer.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddSignalR();

builder.Services.AddSingleton<IPartyService, PartyService>();
builder.Services.AddTransient<IRollService, RollService>();
builder.Services.AddTransient<IMapService, MapService>();
builder.Services.AddSingleton<RollHub>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Error");
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();
app.MapHub<RollHub>("/rollHub");

app.Run();
