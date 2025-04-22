using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession();



bool CustomLifetimeValidator(DateTime? notBefore, DateTime? expires, SecurityToken securityToken, TokenValidationParameters validationParameters)
{
    if (expires == null)
        return false;
    return expires > DateTime.UtcNow;
}

builder.Services.AddAuthorization();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(options => {
    options.TokenValidationParameters = new TokenValidationParameters{
        ValidateIssuer = true,
        ValidIssuer = AuthOptions.ISSUER,
        ValidateAudience = true,
        ValidAudience = AuthOptions.AUDIENCE,
        ValidateIssuerSigningKey = true,
        ValidateLifetime = true,
        LifetimeValidator = CustomLifetimeValidator,
        IssuerSigningKey = AuthOptions.GetKey()
    };
});

var app = builder.Build();
IDBManager db = app.Services.GetRequiredService<IDBManager>();
app.UseDefaultFiles();
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();

if (!db.ConnectToDB("/home/kali/Documents/WEBB/web3/Users.db")){
    Console.WriteLine("connection failure");
    return;
}

AuthRequests Au = new AuthRequests(db);

app.MapPost("/login", ([FromForm]string login, [FromForm]string password) => Au.Login(login,password)).DisableAntiforgery();

app.MapPost("/signup", ([FromForm]string login, [FromForm]string password) => Au.SignUp(login, password)).DisableAntiforgery();

app.MapPost("/changePassword", ([FromForm]string login, [FromForm]string oldPassword, [FromForm]string newPassword) => Au.ChangePass(login,oldPassword,newPassword)).DisableAntiforgery();

app.MapPost("/DeleteUser", ([FromForm]string login, [FromForm]string password) => Au.DelUser(login,password)).DisableAntiforgery();

LinkedList<int> list = new LinkedList<int>();

app.MapPost("/sort/RandArray", [Authorize] ([FromForm]string len, [FromForm]string min, [FromForm]string max) => {
    int len1 = int.Parse(len);
    int min1 = int.Parse(min);
    int max1 = int.Parse(max);
    if (len1<=0){
        return Results.BadRequest("Length must be greater than 0");
    }
    list.Clear();
    Random random = new Random();
    for (int i = 0; i < len1; i++){
        list.Add(random.Next(min1,max1));
    }
    return Results.Ok(list);
}).DisableAntiforgery();

app.MapPost("/sort/SortArray", [Authorize] () => {
    if (list == null || list.Count == 0){
        return Results.BadRequest("No array stored on the server." );
    }
    list.Sort();
    return Results.Ok("Array sorted successfully.");
        
});

app.MapPost("/sort/ClearArray", [Authorize] () => {
    list.Clear();
    return Results.Ok("List vanished");
});

app.MapPost("/sort/RemoveElement", [Authorize] ([FromForm]string itemToRemove) => {
    int item = int.Parse(itemToRemove);
    if (list.Remove(item)){
        return Results.Ok($"Element {itemToRemove} deleted.");
    }
    return Results.BadRequest($"Element {itemToRemove} not found.");
}).DisableAntiforgery();

app.MapPost("/sort/AddElements", [Authorize] ([FromForm]string addItem) => {
    int[] array = addItem.Split(',').Select(x => int.Parse(x)).ToArray();
    for (int i = 0; i < array.Length; i++){
        list.Add(array[i]);
    }
    return Results.Ok($"Element {addItem} added.");
}).DisableAntiforgery();

app.MapGet("/sort/ShowList", [Authorize] () => {
    if (list == null || list.Count == 0){
        return Results.BadRequest("No array stored on the server." );
    }
    return Results.Ok(list);
});

app.Run();
public class AuthOptions{
    public const string ISSUER = "WEBB";
    public const string AUDIENCE = "WEBBAudience";
    public static SymmetricSecurityKey GetKey() {
        return new SymmetricSecurityKey(Encoding.UTF8.GetBytes("AsFarAsIUnderstendICanWriteHereAnythingIWantButIMastMakeSureTheLenghtIsValid"));
    }
}
public class AuthRequests{
    private readonly IDBManager database;
    public AuthRequests(IDBManager datab){
        database = datab;
    }
    public IResult Login(string login, string password){
    if (!database.CheckUser(login,password))
        return Results.Unauthorized();
    var jwt = new JwtSecurityToken(
        issuer: AuthOptions.ISSUER,
        audience: AuthOptions.AUDIENCE,
        expires: DateTime.UtcNow.AddMinutes(10),
        signingCredentials: new SigningCredentials(
            AuthOptions.GetKey(), SecurityAlgorithms.HmacSha256
        )
    );

    var encodedToken = new JwtSecurityTokenHandler().WriteToken(jwt);

    var response = new {
        access_token = encodedToken,
        username = login
    };

    return Results.Ok(response);
    }
    public IResult SignUp(string login, string password){
        if (database.AddUser(login,password))
            return Results.Ok("User " + login + " registered succesfuly");
        return Results.Problem("Failed to register user " + login);
    }
    public IResult ChangePass(string login, string oldPassword, string newPassword){
    if (!database.CheckUser(login,oldPassword))
        return Results.Unauthorized();
    if (oldPassword == newPassword)
        return Results.BadRequest("New password must be different from the old one" );
    if (database.ChangePassword(login,newPassword))
        return Results.Ok("Password changed succesfuly");
    return Results.Problem("Failed to change password");
    }
    public IResult DelUser(string login, string password){
    if (!database.CheckUser(login,password))
        return Results.Unauthorized();
    if (database.DeleteUser(login))
        return Results.Ok("User deleted succesfuly");
    return Results.Problem("Failed to delete User");
    }
}