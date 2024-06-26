﻿using System.Globalization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using ToDoListMVC.Context;
using ToDoListMVC.Models;

namespace ToDoListMVC.Controllers;

[Authorize]
public class TaskController : Controller
{
    private readonly ILogger<TaskController> _logger;
    private readonly ApplicationDbContext _context;

    public TaskController(ILogger<TaskController> logger,
        ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult RedirectToNewTask()
    {
        return View("NewTask");
    }
    
    public IActionResult RedirectToEditTask(Tasks task)
    {
        ViewData["SelectedDate"] = task.ScheduledDate.ToString("yyyy-MM-dd");
        ViewData["Name"] = task.Name;
        return View("EditTask", task);
    }
    public IActionResult RedirectToScheduledTasks(DateTime? date)
    {
        DateTime targetDate = date ?? DateTime.Today;
        var scheduledTasks = GetTasks(date);
        
        ViewData["SelectedDate"] = targetDate.ToString("yyyy-MM-dd");
        return View("ScheduledTasks", scheduledTasks);
    }

    public async Task<IActionResult> AddTask(Tasks task)
    {
        try
        {
            task.Username = User.Identity.Name;
            task.CreatedDate = DateTime.Now;
            DateTime targetDate = task.ScheduledDate >= DateTime.Today ? task.ScheduledDate : DateTime.Today;
            task.ScheduledDate = targetDate;
            
            await SaveAsync(task);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        
        return View("Index");
    }

    public async Task<IActionResult> EditTask(Tasks task)
    {
        try
        {
            var query = _context.Tasks.AsQueryable();
            var taskToUpdate = query.Where(x => x.Id == task.Id).FirstOrDefault();
            taskToUpdate.Name = task.Name;
            taskToUpdate.ScheduledDate = task.ScheduledDate;
            await SaveAsync(taskToUpdate);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }

        return RedirectToScheduledTasks(task.ScheduledDate);
    }

    public async Task<IActionResult> DeleteTask(int id)
    {
        try
        {
            var taskToRemove = await GetSingleTask(id);
            taskToRemove.IsDeleted = true;
            await SaveAsync(taskToRemove);
            
            return RedirectToScheduledTasks(taskToRemove.ScheduledDate);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }

    }

    public async Task SaveAsync(Tasks task)
    {
        try
        {
            if (task.Id == 0)
            {
                _context.Add(task);
            }
            else
            {
                _context.Update(task);
            }

            await _context.SaveChangesAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<IActionResult> MarkAsDone(int id)
    {
        try
        {

            var taskDone = await GetSingleTask(id);
            taskDone.IsDone = true;
            await SaveAsync(taskDone);
            return RedirectToScheduledTasks(taskDone.ScheduledDate);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        
    }

    public IEnumerable<Tasks> GetTasks(DateTime? date)
    {
        DateTime targetDate = date ?? DateTime.Today;
        var query = _context.Tasks.AsQueryable();
        return query.Where(x => x.ScheduledDate == targetDate 
                                && x.Username == User.Identity.Name
                                && x.IsDeleted == false).ToList();
    }

    public async Task<Tasks> GetSingleTask(int id)
    {
        var query = _context.Tasks.AsQueryable();
        return await query.Where(x => x.Id == id).FirstOrDefaultAsync();
    }
    
}