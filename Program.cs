using GestionEventiMagazzino.Data;
using GestionEventiMagazzino.Models;
using GestionEventiMagazzino.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication; 

var builder = WebApplication.CreateBuilder(args);

// Configurazione per salvare le chiavi crittografiche in una cartella fissa sul server
builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(@"C:\GestionEventiMagazzino\Keys"))
    .SetApplicationName("GestionEventiMagazzino");

// servizi mvc
builder.Services.AddControllersWithViews();

//servizi di signalR
builder.Services.AddSignalR();

// Collegamento al database SQL Server
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

//inserimento nel programma dell'hashing. 
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

//rende disponibile il servizio all'applicazione
builder.Services.AddScoped<PasswordService>();

//rende disponibile il servizio di creazione user
builder.Services.AddScoped<UserService>();

//rende disponibile il servizio di creazione eventi
builder.Services.AddScoped<EventoService>();

// Autenticazione tramite cookie
builder.Services.AddAuthentication(
    CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Login/Index";
        options.AccessDeniedPath = "/Login/AccessDenied";

        options.Cookie.Name = "GestionEventiMagazzino.Auth";
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.Cookie.SameSite = SameSiteMode.Lax;

        options.ExpireTimeSpan = TimeSpan.FromHours(12);
        options.SlidingExpiration = false; //in questo modo, se l'utente non fa logout, dopo 12 ore dovrà rifare il login. Se fosse true, invece, ogni volta che l'utente fa una richiesta, il tempo di scadenza del cookie si resetta e quindi non scade mai.

        // Controllo che l'utente sia ancora attivo: qualora venisse eliminato
        options.Events = new CookieAuthenticationEvents
        {
            OnValidatePrincipal = async context =>
            {
                var username = context.Principal?.Identity?.Name;

                var db = context.HttpContext.RequestServices
                    .GetRequiredService<ApplicationDbContext>();

                bool utenteAttivo = await db.Users
                    .AsNoTracking()
                    .AnyAsync(u => u.username == username
                                && !u.isDeactivated);

                if (!utenteAttivo)
                {
                    // Invalido l'autenticazione
                    context.RejectPrincipal();

                    // Elimino il cookie di autenticazione
                    await context.HttpContext.SignOutAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme);

                }
            }
        };
        });


// Gestione autorizzazioni
builder.Services.AddAuthorization();

var app = builder.Build();

// pipeline HTTP
if (!app.Environment.IsDevelopment())
{
    
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication(); //grazie a questo metodo asp.net identifica l'utente tramite cookie
app.UseAuthorization();

app.MapStaticAssets();
app.UseStaticFiles(); //questo consente di far vedere un'immagine a tutti i pc fuori dalla rete locale

app.MapControllerRoute( //ROTTA INIZIALE
    name: "default",
    pattern: "{controller=Login}/{action=Index}/{id?}") //indicando login, apparirà sempre come prima schermata
    .WithStaticAssets();

app.MapHub<EventiHub>("/eventiHub"); //eventiHub è l'indirizzo al quale i browser si collegheranno

//CREAZIONE DEL PRIMO AMMINISTARTORE: FATTO
//using (var scope = app.Services.CreateScope())
//{
//    var context = scope.ServiceProvider
//        .GetRequiredService<ApplicationDbContext>();

//    var userService = scope.ServiceProvider
//        .GetRequiredService<UserService>();

//    await userService.CreateUserAsync(
//            username: "FedeAdmin",
//            password: "Federico",
//            email: "fpjuniorsoft.it@gmail.com",
//            isAdmin: true,
//            note: "Primo amministratore"
//        );
//}

    app.Run();