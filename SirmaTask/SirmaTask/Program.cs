using SirmaTask.Services;
using SirmaTask.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddScoped<ICsvParserService, CsvParserService>();
builder.Services.AddScoped<IEmployeePairAnalyzer, EmployeePairAnalyzer>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Employees}/{action=Analyze}/{id?}");

app.Run();