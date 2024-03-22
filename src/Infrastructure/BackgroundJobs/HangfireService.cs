using System.Linq.Expressions;
using TD.WebApi.Application.Common.Interfaces;
using Hangfire;

namespace TD.WebApi.Infrastructure.BackgroundJobs;

public class HangfireService : IJobService
{
    public bool Delete(string jobId) =>
        BackgroundJob.Delete(jobId);

    public bool Delete(string jobId, string fromState) =>
        BackgroundJob.Delete(jobId, fromState);

    public string Enqueue(Expression<Func<Task>> methodCall) =>
        BackgroundJob.Enqueue(methodCall);

    public string Enqueue<T>(Expression<Action<T>> methodCall) =>
        BackgroundJob.Enqueue(methodCall);

    public string Enqueue(Expression<Action> methodCall) =>
        BackgroundJob.Enqueue(methodCall);

    public string Enqueue<T>(Expression<Func<T, Task>> methodCall) =>
        BackgroundJob.Enqueue(methodCall);

    public bool Requeue(string jobId) =>
        BackgroundJob.Requeue(jobId);

    public bool Requeue(string jobId, string fromState) =>
        BackgroundJob.Requeue(jobId, fromState);

    public string Schedule(Expression<Action> methodCall, TimeSpan delay) =>
        BackgroundJob.Schedule(methodCall, delay);

    public string Schedule(Expression<Func<Task>> methodCall, TimeSpan delay) =>
        BackgroundJob.Schedule(methodCall, delay);

    public string Schedule(Expression<Action> methodCall, DateTimeOffset enqueueAt) =>
        BackgroundJob.Schedule(methodCall, enqueueAt);

    public string Schedule(Expression<Func<Task>> methodCall, DateTimeOffset enqueueAt) =>
        BackgroundJob.Schedule(methodCall, enqueueAt);

    public string Schedule<T>(Expression<Action<T>> methodCall, TimeSpan delay) =>
        BackgroundJob.Schedule(methodCall, delay);

    public string Schedule<T>(Expression<Func<T, Task>> methodCall, TimeSpan delay) =>
        BackgroundJob.Schedule(methodCall, delay);

    public string Schedule<T>(Expression<Action<T>> methodCall, DateTimeOffset enqueueAt) =>
        BackgroundJob.Schedule(methodCall, enqueueAt);

    public string Schedule<T>(Expression<Func<T, Task>> methodCall, DateTimeOffset enqueueAt) =>
        BackgroundJob.Schedule(methodCall, enqueueAt);


    public void Recurring<T>(string jobName, Expression<Func<T, Task>> methodCall, Func<string> cronExpression) =>
        RecurringJob.AddOrUpdate(jobName, methodCall, cronExpression: cronExpression);

    public void Recurring(string jobName, Expression<Action> methodCall, Func<string> cronExpression) =>
        RecurringJob.AddOrUpdate(jobName, methodCall, cronExpression: cronExpression);

    public void Recurring(string jobName, Expression<Func<Task>> methodCall, Func<string> cronExpression) =>
        RecurringJob.AddOrUpdate(jobName, methodCall, cronExpression: cronExpression);

    public void Recurring<T>(string jobName, Expression<Action<T>> methodCall, Func<string> cronExpression) =>
        RecurringJob.AddOrUpdate(jobName, methodCall, cronExpression: cronExpression);


    public void Recurring<T>(string jobName, Expression<Func<T, Task>> methodCall, string cronExpression) =>
           RecurringJob.AddOrUpdate(methodCall, cronExpression: cronExpression);

    public void Recurring(string jobName, Expression<Action> methodCall, string cronExpression) =>
        RecurringJob.AddOrUpdate(jobName, methodCall, cronExpression: cronExpression);

    public void Recurring(string jobName, Expression<Func<Task>> methodCall, string cronExpression) =>
        RecurringJob.AddOrUpdate(jobName, methodCall, cronExpression: cronExpression);

    public void Recurring<T>(string jobName, Expression<Action<T>> methodCall, string cronExpression) =>
        RecurringJob.AddOrUpdate(jobName, methodCall, cronExpression: cronExpression);

    public void RecurringRemoveIfExists(string jobId) =>
    RecurringJob.RemoveIfExists(jobId);

    public void AddOrUpdate(string id, Expression<Func<Task>> methodCall, Func<string> cron, TimeZoneInfo timeZone, string queue) =>
        RecurringJob.AddOrUpdate(id, methodCall, cron, timeZone, queue);

    public void AddOrUpdate<T>(string id, Expression<Func<T, Task>> methodCall, Func<string> cron, TimeZoneInfo timeZone, string queue) =>
        RecurringJob.AddOrUpdate(id, methodCall, cron, timeZone, queue);

    public void AddOrUpdate<T>(string id, Expression<Func<T, Task>> methodCall, string cron, TimeZoneInfo timeZone, string queue) =>
        RecurringJob.AddOrUpdate(id, methodCall, cron, timeZone, queue);

    public void AddOrUpdate(string id, Expression<Action> methodCall, Func<string> cron, TimeZoneInfo timeZone, string queue) =>
        RecurringJob.AddOrUpdate(id, methodCall, cron, timeZone, queue);

    public void AddOrUpdate<T>(string id, Expression<Action<T>> methodCall, Func<string> cron, TimeZoneInfo timeZone, string queue) =>
        RecurringJob.AddOrUpdate(id, methodCall, cron, timeZone, queue);
}