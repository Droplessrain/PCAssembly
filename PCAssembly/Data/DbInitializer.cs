using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using PCAssembly.Data;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PCAssembly
{
    public static class DbInitializer
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            using var context = serviceProvider.GetRequiredService<ComputerAssemblyContext>();

            // Применяем миграции
            await context.Database.EnsureCreatedAsync();

            // Проверяем, если база данных уже заполнена
            if (context.Components.Any() || context.TypeComponents.Any())
            {
                return;
            }

            // Создаем роли
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            await CreateRoleAsync(roleManager, "Admin");
            await CreateRoleAsync(roleManager, "User");

            // Создаем пользователя
            var userManager = serviceProvider.GetRequiredService<UserManager<User>>();
            var user = new User
            {
                UserName = "exampleUser",
                Email = "user@example.com",
                EmailConfirmed = true
            };

            if (await userManager.FindByEmailAsync(user.Email) == null)
            {
                await userManager.CreateAsync(user, "Password123!");
                await userManager.AddToRoleAsync(user, "User");
            }

            // Создаем типы компонентов
            var typeComponents = new[]
            {
                new TypeComponent { TypeName = "Processor" },
                new TypeComponent { TypeName = "Graphics Card" },
                new TypeComponent { TypeName = "Motherboard" },
                new TypeComponent { TypeName = "RAM" }
            };

            await context.TypeComponents.AddRangeAsync(typeComponents);
            await context.SaveChangesAsync();

            // Создаем компоненты
            var components = new[]
            {
                new Component { Name = "Intel Core i9", Description = "High-end processor", Price = 500.00m, TypeComponentsId = typeComponents[0].TypeComponentsId },
                new Component { Name = "NVIDIA RTX 3080", Description = "High-end graphics card", Price = 800.00m, TypeComponentsId = typeComponents[1].TypeComponentsId },
                new Component { Name = "ASUS ROG Motherboard", Description = "Gaming motherboard", Price = 250.00m, TypeComponentsId = typeComponents[2].TypeComponentsId },
                new Component { Name = "Corsair Vengeance 16GB", Description = "High-speed RAM", Price = 120.00m, TypeComponentsId = typeComponents[3].TypeComponentsId }
            };

            await context.Components.AddRangeAsync(components);
            await context.SaveChangesAsync();

            // Создаем сборки
            var assembly = new Assembly
            {
                UserId = user.Id,
                AssemblyName = "Gaming PC",
                Avgrating = 5
            };

            await context.Assemblies.AddAsync(assembly);
            await context.SaveChangesAsync();

            // Связываем сборку с компонентами
            var assemblyComponents = new[]
            {
                new AssemblyComponent { AssemblyId = assembly.AssemblyId, ComponentId = components[0].ComponentId },
                new AssemblyComponent { AssemblyId = assembly.AssemblyId, ComponentId = components[1].ComponentId },
                new AssemblyComponent { AssemblyId = assembly.AssemblyId, ComponentId = components[2].ComponentId },
                new AssemblyComponent { AssemblyId = assembly.AssemblyId, ComponentId = components[3].ComponentId }
            };

            await context.AssemblyComponents.AddRangeAsync(assemblyComponents);
            await context.SaveChangesAsync();

            // Создаем отзыв
            var review = new Review
            {
                AssemblyId = assembly.AssemblyId,
                UserId = user.Id,
                ReviewText = "This is an amazing PC build!",
                Rating = 5
            };

            await context.Reviews.AddAsync(review);
            await context.SaveChangesAsync();

            Console.WriteLine("База данных успешно инициализирована!");
        }

        private static async Task CreateRoleAsync(RoleManager<IdentityRole> roleManager, string roleName)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }
    }
}
