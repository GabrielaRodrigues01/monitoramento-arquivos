using MonitoramentoArquivos.Api.Workers;
using MonitoramentoArquivos.Application.Contracts;
using MonitoramentoArquivos.Application.Services;
using MonitoramentoArquivos.Infrastructure.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new()
    {
        Title = "Monitoramento de Arquivos",
        Version = "v1",
        Description = "API para ingestão, monitoramento e auditoria de arquivos financeiros"
    });
});

builder.Services.Configure<FileIngestionOptions>(
    builder.Configuration.GetSection("FileIngestion"));

builder.Services.AddHostedService<FolderMonitorHostedService>();

builder.Services.AddScoped<IFileReceiptParser, FileReceiptLineParser>();
builder.Services.AddScoped<FileReceiptIngestionService>();

builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Monitoramento de Arquivos v1");
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
