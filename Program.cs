using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using Microsoft.Extensions.Azure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddScoped<IUserClaimsPrincipalFactory<IdentityUser>, CustomClaimsPrincipalFactory>();
builder.Services.AddControllersWithViews();
builder.Services.AddAzureClients(clientBuilder =>
{
    clientBuilder.AddBlobServiceClient(builder.Configuration["BlobStorageSettings:ConnectionString:blob"]!, preferMsi: true);
    clientBuilder.AddQueueServiceClient(builder.Configuration["BlobStorageSettings:ConnectionString:queue"]!, preferMsi: true);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    var roles = new[] { "Admin", "User", "Owner" , "ManageUserRoles",


"CreateDoctor",
"CreateClinicalRecord",
"CreateClinic",
"CreatePatient",
"ViewDoctor",
"ViewClinicalRecord",
"ViewClinic",
"ViewPatient",
"DeleteDoctor",
"DeleteClinicalRecord",
"DeleteClinic",
"DeletePatient"



    };

    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
            await roleManager.CreateAsync(new IdentityRole(role));

    }
}
//create admin acc with admin roles 

using (var scope = app.Services.CreateScope())
{
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
    string email = "test123@admin.com";
    string password = "Jeffery123$";
    if (await userManager.FindByEmailAsync(email) == null)
    {
        var user = new IdentityUser();
        user.Email = email;
        user.UserName = email;
        user.EmailConfirmed = true;
        await userManager.CreateAsync(user, password);

        await userManager.AddToRoleAsync(user, "Admin");
    }


}


app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();




app.Run();
