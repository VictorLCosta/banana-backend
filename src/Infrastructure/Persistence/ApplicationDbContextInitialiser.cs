using Bogus;

using Domain.Entities;
using Domain.ValueObjects;

using Infrastructure.Authorization;
using Infrastructure.Identity;

using Microsoft.Extensions.Logging;

namespace Infrastructure.Persistence;

public class ApplicationDbContextInitialiser(
    ApplicationDbContext context,
    RoleManager<ApplicationRole> roleManager,
    UserManager<ApplicationUser> userManager,
    ILogger<ApplicationDbContextInitialiser> logger)
{
    private readonly ApplicationDbContext _context = context;
    private readonly RoleManager<ApplicationRole> _roleManager = roleManager;
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly ILogger<ApplicationDbContextInitialiser> _logger = logger;

    public async Task InitialiseAsync(CancellationToken cancellationToken)
    {
        try
        {
            if (_context.Database.GetMigrations().Any())
            {
                if ((await _context.Database.GetPendingMigrationsAsync(cancellationToken)).Any())
                {
                    await _context.Database.MigrateAsync(cancellationToken);
                }
                if (await _context.Database.CanConnectAsync(cancellationToken))
                {
                    await SeedAsync(cancellationToken);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while initialising the database.");
            throw;
        }
    }

    public async Task SeedAsync(CancellationToken cancellationToken)
    {
        try
        {
            await TrySeedAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while seeding the database.");
            throw;
        }
    }

    public async Task TrySeedAsync(CancellationToken cancellationToken)
    {
        await SeedRolesAsync();
        await SeedAdminUserAsync();

        #if DEBUG

        await SeedEntities(cancellationToken);

        #endif

        await _context.SaveChangesAsync(cancellationToken);
    }

    private async Task SeedRolesAsync()
    {
        foreach (string roleName in ApplicationRoles.DefaultRoles)
        {
            if (await _roleManager.Roles.SingleOrDefaultAsync(r => r.Name == roleName)
                is not ApplicationRole role)
            {
                // Create the role
                _logger.LogInformation("Seeding {role} Role", roleName);
                role = new ApplicationRole(roleName, $"{roleName} Role");
                await _roleManager.CreateAsync(role);
            }

            // Assign permissions
            if (roleName == ApplicationRoles.Basic)
            {
                await AssignPermissionsToRoleAsync(_context, ApplicationPermissions.Basic, role);
            }
            else if (roleName == ApplicationRoles.Admin)
            {
                await AssignPermissionsToRoleAsync(_context, ApplicationPermissions.Admin, role);
            }
        }
    }

    private async Task AssignPermissionsToRoleAsync(ApplicationDbContext dbContext, IReadOnlyList<ApplicationPermission> permissions, ApplicationRole role)
    {
        var currentClaims = await _roleManager.GetClaimsAsync(role);
        foreach (var permission in permissions)
        {
            if (!currentClaims.Any(c => c.Type == "permission" && c.Value == permission.Name))
            {
                _logger.LogInformation("Seeding {role} Permission '{permission}'", role.Name, permission.Name);
                dbContext.RoleClaims.Add(new ApplicationRoleClaim
                {
                    RoleId = role.Id,
                    ClaimType = "permission",
                    ClaimValue = permission.Name,
                    CreatedBy = "ApplicationDbContextInitialiser"
                });
                await dbContext.SaveChangesAsync();
            }
        }
    }

    private async Task SeedAdminUserAsync()
    {
        if (await _userManager.Users.FirstOrDefaultAsync(u => u.Email == "admin@gmail.com")
            is not ApplicationUser adminUser)
        {
            string adminUserName = "admin".ToLowerInvariant();
            adminUser = new ApplicationUser
            {
                FirstName = "admin".ToLowerInvariant(),
                LastName = "admin",
                Email = "admin@gmail.com",
                UserName = adminUserName,
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                NormalizedEmail = "admin@email.com".ToUpperInvariant(),
                NormalizedUserName = adminUserName.ToUpperInvariant(),
                IsActive = true
            };

            _logger.LogInformation("Seeding Default Admin User.");
            var password = new PasswordHasher<ApplicationUser>();
            adminUser.PasswordHash = password.HashPassword(adminUser, "teste1");
            await _userManager.CreateAsync(adminUser);
        }

        // Assign role to user
        if (!await _userManager.IsInRoleAsync(adminUser, ApplicationRoles.Admin))
        {
            _logger.LogInformation("Assigning Admin Role to Admin User.");
            await _userManager.AddToRoleAsync(adminUser, ApplicationRoles.Admin);
        }
    }

    private async Task SeedEntities(CancellationToken cancellationToken)
    {
        if (!await _context.Rooms.AnyAsync(cancellationToken))
        {
            _logger.LogInformation("Seeding Rooms");
            var rooms = new Faker<Room>("pt_BR")
                .RuleFor(e => e.Name, f => f.Company.CompanyName())
                .RuleFor(e => e.Capacity, f => f.Random.Short(1, 20))
                .RuleFor(e => e.Location, f => new Address
                {
                    Street = f.Address.StreetName(),
                    City = f.Address.City(),
                    State = f.Address.State(),
                    PostalCode = f.Address.ZipCode(),
                    BuildingNumber = f.Address.BuildingNumber(),
                    Neighborhood = f.Address.SecondaryAddress()
                })
                .Generate(30);

            await _context.Rooms.AddRangeAsync(rooms, cancellationToken);
        }
    }
}
