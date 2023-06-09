using Rugal.Net.LocalFileManager.Extention;
using Rugal.NetCommon.Extention.Startup;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

#region Rugal Library

#region Add Common Input
builder.Services.AddCommonInputOptions();
#endregion

#region Add LocalFile
builder.Services.AddLocalFile(builder.Configuration);
#endregion

#endregion 

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
