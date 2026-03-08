using Microsoft.AspNetCore.Identity;
using Tashyeed.Infrastructure;
using Tashyeed.Infrastructure.Identity;
using Tashyeed.Shared.Interfaces;
using Tashyeed.Web.Helper.Seeding;
using Tashyeed.Web.Modules.Accounting.Services;
using Tashyeed.Web.Modules.CustodyModule.Services;
using Tashyeed.Web.Modules.Expenses.Services;
using Tashyeed.Web.Modules.Procurement.Services;
using Tashyeed.Web.Modules.ProjectAssignment.Services;
using Tashyeed.Web.Modules.Projects.Services;
using Tashyeed.Web.Modules.UserManagement.Services;
using Tashyeed.Web.Modules.Workers.Services;

public partial class Program
{
    private async static Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllersWithViews()
            .AddRazorOptions(options =>
            {
                options.ViewLocationFormats.Add("/Modules/{1}/Views/{0}.cshtml");
                options.ViewLocationFormats.Add("/Views/Shared/{0}.cshtml");
                options.ViewLocationFormats.Add("/Modules/CustodyModule/Views/{0}.cshtml");
            });

        builder.Services.AddInfrastructure(builder.Configuration);
        builder.Services.AddAutoMapper(typeof(Program).Assembly);
        builder.Services.AddScoped<IProjectService, ProjectService>();
        builder.Services.AddScoped<IUserManagementService, UserManagementService>();
        builder.Services.AddScoped<IProjectAssignmentService, ProjectAssignmentService>();
        builder.Services.AddScoped<ICustodyService, CustodyService>();
        builder.Services.AddScoped<IProjectAssignmentChecker, ProjectAssignmentChecker>();
        builder.Services.AddScoped<IExpenseService, ExpenseService>();
        builder.Services.AddScoped<IProcurementService, ProcurementService>();
        builder.Services.AddScoped<IWorkerService, WorkerService>();
        builder.Services.AddScoped<IReportService, ReportService>();


        var app = builder.Build();

        using (var scope = app.Services.CreateScope())
        {
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            await IdentitySeeder.SeedAsync(roleManager, userManager);
        }

        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapStaticAssets();
        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}")
            .WithStaticAssets();

        app.Run();
    }
}