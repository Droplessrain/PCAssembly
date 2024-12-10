using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PCAssembly;
using PCAssembly.Data;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("ComputerAssemblyContextConnection")
    ?? throw new InvalidOperationException("������ ����������� 'ComputerAssemblyContextConnection' �� �������.");

builder.Services.AddDbContext<ComputerAssemblyContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddIdentity<User, IdentityRole>(options =>
{
    // ��������� ���������� � ������
    options.Password.RequireDigit = true; // ��������� ���� �� ���� �����
    options.Password.RequiredLength = 6; // ����������� ����� ������
    options.Password.RequireNonAlphanumeric = false; // ��������� ������������ ����������� �������
    options.Password.RequireUppercase = true; // ��������� ���� �� ���� ��������� �����
    options.Password.RequireLowercase = true; // ��������� ���� �� ���� �������� �����

    // ��������� ���������� � ������� ������
    options.User.RequireUniqueEmail = true; // ��������� ���������� email

    // ��������� �����
    options.SignIn.RequireConfirmedAccount = false; // �� ��������� ������������� ������� ������
    options.SignIn.RequireConfirmedEmail = false;  // �� ��������� ������������� email
    options.SignIn.RequireConfirmedPhoneNumber = false; // �� ��������� ������������� ��������
})
.AddRoles<IdentityRole>() 
.AddEntityFrameworkStores<ComputerAssemblyContext>()
.AddDefaultTokenProviders();

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddLogging(config =>
{
    config.AddConsole();
    config.AddDebug();
});


var app = builder.Build();

// ��������� ��������� HTTP-��������
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication(); // ��������� ��������������
app.UseAuthorization();  // ��������� �����������

app.MapControllerRoute(
    name: "admin",
    pattern: "Admin",
    defaults: new { controller = "Admin", action = "Index" });

app.MapControllerRoute(
    name: "user",
    pattern: "User",
    defaults: new { controller = "User", action = "Profile" });

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


app.MapRazorPages();

// ������������� ����� � ��������������
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        Console.WriteLine("������ ������������� ���� ������...");
        //await DbInitializer.InitializeAsync(services); // ������������� ���� ������
        await InitializeRolesAndAdminAsync(services);  // ������������� ����� � ��������������
        Console.WriteLine("������������� ���� ������ ���������.");
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "������ ��� ������������� ���� ������: {Message}", ex.Message);
    }
}

app.Run();

// ����� ��� ������������� ����� � ��������������
async Task InitializeRolesAndAdminAsync(IServiceProvider serviceProvider)
{
    var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = serviceProvider.GetRequiredService<UserManager<User>>();

    // ����, ������� ����� �������
    string[] roles = { "Admin", "User" };

    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }

    // �������� ��������������
    var adminEmail = "admin@example.com";
    var adminPassword = "Admin@123";

    if (await userManager.FindByEmailAsync(adminEmail) == null)
    {
        var admin = new User { UserName = adminEmail, Email = adminEmail, EmailConfirmed = true };
        var result = await userManager.CreateAsync(admin, adminPassword);
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(admin, "Admin");
        }
    }

    // �������� �������� ������������ (�������������)
    var userEmail = "user@example.com";
    var userPassword = "User@123";

    if (await userManager.FindByEmailAsync(userEmail) == null)
    {
        var user = new User { UserName = userEmail, Email = userEmail };
        var result = await userManager.CreateAsync(user, userPassword);
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(user, "User");
        }
    }
}
