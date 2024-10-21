var builder = WebApplication.CreateBuilder(args);

string path = "C:\\Users\\HP\\OneDrive\\Рабочий стол\\ManagingSystem\\Main\\appsetting.json";

builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

IConfigurationRoot configuration = new ConfigurationBuilder()
.SetBasePath(Directory.GetCurrentDirectory())
.AddJsonFile(path)
.Build();

builder.Services.AddRepositories(configuration);

var app = builder.Build();

app.UseAgeCheck();
app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();
app.Run();