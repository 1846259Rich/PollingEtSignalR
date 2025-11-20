using labo.signalr.api.Data;
using labo.signalr.api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace labo.signalr.api.Hubs
{
    public class TaskHub : Hub
    {
        private readonly ApplicationDbContext _context;

        public TaskHub(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task GetAllTask()
        {
            List<UselessTask> tasks = await _context.UselessTasks.ToListAsync();
            await Clients.All.SendAsync("GetTaskResponse", tasks);
        }

        public async Task AddTask(string taskText)
        {
            UselessTask uselessTask = new UselessTask()
            {
                Completed = false,
                Text = taskText
            };
            _context.UselessTasks.Add(uselessTask);
            await _context.SaveChangesAsync();

            await GetAllTask();
        }

        public async Task CompleteTask(int id)
        {
            UselessTask? task = await _context.FindAsync<UselessTask>(id);
            if (task != null)
            {
                task.Completed = true;
                await _context.SaveChangesAsync();
                await Clients.Caller.SendAsync("ErrorResponse", "Error");
            }
            await GetAllTask();
        }
    }
}
