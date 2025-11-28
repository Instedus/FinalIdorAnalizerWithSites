using Site_3;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Site_3.Models;

namespace Site_3
{
    public class ScheduleService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<ScheduleService> _logger;

        public ScheduleService(IServiceProvider serviceProvider, ILogger<ScheduleService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _serviceProvider.CreateScope();
                    var context = scope.ServiceProvider.GetRequiredService<AdvancedDbContext>();
                    var now = DateTime.Now;

                    var schedules = await context.Schedules
                        .Where(s => s.LastRun < now.AddMinutes(-1))
                        .ToListAsync(stoppingToken);

                    foreach (var schedule in schedules)
                    {
                        await ExecuteSchedule(context, schedule);
                        schedule.LastRun = now;
                        await context.SaveChangesAsync(stoppingToken);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in ScheduleService");
                }

                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }

        private async Task ExecuteSchedule(AdvancedDbContext context, Schedule schedule)
        {
            try
            {
                if (schedule.JobType == "export")
                {
                    // ⚠️ Вторичная IDOR: нет проверки прав доступа, используется сохранённый UserId
                    var exportData = $"Экспорт для пользователя {schedule.UserId} от {DateTime.Now:yyyy-MM-dd}";
                    var job = new ExportJob
                    {
                        UserId = schedule.UserId, // Уязвимость!
                        Status = "completed",
                        ExportData = exportData
                    };

                    context.ExportJobs.Add(job);
                    await context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to execute schedule {schedule.Id} for user {schedule.UserId}");
            }
        }
    }
}