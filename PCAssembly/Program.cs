using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PCAssembly;
using PCAssembly.Data;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("ComputerAssemblyContextConnection")
    ?? throw new InvalidOperationException("Строка подключения 'ComputerAssemblyContextConnection' не найдена.");

builder.Services.AddDbContext<ComputerAssemblyContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddIdentity<User, IdentityRole>(options =>
{
    // Настройка требований к паролю
    options.Password.RequireDigit = true; // Требуется хотя бы одна цифра
    options.Password.RequiredLength = 6; // Минимальная длина пароля
    options.Password.RequireNonAlphanumeric = false; // Отключаем обязательные специальные символы
    options.Password.RequireUppercase = true; // Требуется хотя бы одна заглавная буква
    options.Password.RequireLowercase = true; // Требуется хотя бы одна строчная буква

    // Настройка требований к учетной записи
    options.User.RequireUniqueEmail = true; // Требуется уникальный email

    // Настройка входа
    options.SignIn.RequireConfirmedAccount = false; // Не требовать подтверждения учетной записи
    options.SignIn.RequireConfirmedEmail = false;  // Не требовать подтверждения email
    options.SignIn.RequireConfirmedPhoneNumber = false; // Не требовать подтверждения телефона
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

// Настройка конвейера HTTP-запросов
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication(); // Включение аутентификации
app.UseAuthorization();  // Включение авторизации

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

// Инициализация ролей и администратора
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        Console.WriteLine("Начало инициализации базы данных...");
        //await DbInitializer.InitializeAsync(services); // Инициализация базы данных
        await InitializeRolesAndAdminAsync(services);  // Инициализация ролей и администратора
        Console.WriteLine("Инициализация базы данных завершена.");
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Ошибка при инициализации базы данных: {Message}", ex.Message);
    }
}

app.Run();

// Метод для инициализации ролей и администратора
async Task InitializeRolesAndAdminAsync(IServiceProvider serviceProvider)
{
    var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = serviceProvider.GetRequiredService<UserManager<User>>();

    // Роли, которые нужно создать
    string[] roles = { "Admin", "User" };

    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }

    // Создание администратора
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

    // Создание обычного пользователя (необязательно)
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
