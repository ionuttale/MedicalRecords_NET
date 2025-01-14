using MySql.Data.MySqlClient;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Register the MySQL connection string as a service
builder.Services.AddTransient<MySqlConnection>(_ =>
    new MySqlConnection(builder.Configuration.GetConnectionString("MySQLConnection")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapControllerRoute(
    name: "patients",
    pattern:"{controller=Patients}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapControllerRoute(
    name:"medicines",
    pattern:"{controller=Medicines}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapControllerRoute(
    name:"purchases",
    pattern:"{controller=Purchases}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
