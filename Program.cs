using Kodluyoruz_RepoGenerator.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IGithubService, GithubService>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


app.MapGet("/getRepos", (IGithubService _githubService) =>
{
    
    var resp = _githubService.GetTemplateRepoList();

    return resp;
       
      
})
.WithName("GetRepositoriesList");

app.MapGet("/getMembers", (IGithubService _githubService) =>
{
    
    var resp = _githubService.GetMemberList();
    return resp;

})
.WithName("GetMembersList");

app.MapGet("/CreateRepository", (IGithubService _githubService) =>
{
    // get members 
    var memberlist = _githubService.GetMemberList();
    if (memberlist.counter == 0)
        return "Üye bulunamadý";
    // get template repos
    var repositorylist = _githubService.GetTemplateRepoList();
    if(repositorylist.counter==0)
        return "Repository bulunamadý";

    //create repositories
    var resp = _githubService.CloneTemplateRepositoryBulk(memberlist.Data, repositorylist.Data);
    return resp;

})
.WithName("CreateRepository");

app.Run();

internal record WeatherForecast(DateTime Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}