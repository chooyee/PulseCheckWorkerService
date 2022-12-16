using EasyCronJob.Core;
using Job;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting.WindowsServices;
using Factory;
using PulseCheckWorkerService.Services;
using Global;

LoggerService.InitLogService();

var options = new WebApplicationOptions
{
    Args = args,
    ContentRootPath = WindowsServiceHelpers.IsWindowsService()
                                     ? AppContext.BaseDirectory : default
};

var builder = WebApplication.CreateBuilder(options);
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.ApplyResulation<SyncJob>(options =>
{
    options.CronExpression = GlobalEnv.Instance.CronJob;
    options.TimeZoneInfo = TimeZoneInfo.Local;
    options.CronFormat = Cronos.CronFormat.Standard;
});
//builder.Services.AddRazorPages();
builder.Services.AddHostedService<PulseService>();
builder.Host.UseWindowsService();

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
//app.UseStaticFiles();
//app.UseRouting();
//app.MapRazorPages();
await app.RunAsync();
