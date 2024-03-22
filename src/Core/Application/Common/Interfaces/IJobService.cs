using System.Linq.Expressions;

namespace TD.WebApi.Application.Common.Interfaces;

public interface IJobService : ITransientService
{
    string Enqueue(Expression<Action> methodCall);

    string Enqueue(Expression<Func<Task>> methodCall);

    string Enqueue<T>(Expression<Action<T>> methodCall);

    string Enqueue<T>(Expression<Func<T, Task>> methodCall);

    string Schedule(Expression<Action> methodCall, TimeSpan delay);

    string Schedule(Expression<Func<Task>> methodCall, TimeSpan delay);

    string Schedule(Expression<Action> methodCall, DateTimeOffset enqueueAt);

    string Schedule(Expression<Func<Task>> methodCall, DateTimeOffset enqueueAt);

    string Schedule<T>(Expression<Action<T>> methodCall, TimeSpan delay);

    string Schedule<T>(Expression<Func<T, Task>> methodCall, TimeSpan delay);

    string Schedule<T>(Expression<Action<T>> methodCall, DateTimeOffset enqueueAt);

    string Schedule<T>(Expression<Func<T, Task>> methodCall, DateTimeOffset enqueueAt);

    bool Delete(string jobId);

    bool Delete(string jobId, string fromState);

    bool Requeue(string jobId);

    bool Requeue(string jobId, string fromState);


    void Recurring(string jobName, Expression<Action> methodCall, Func<string> cronExpression);

    void Recurring(string jobName, Expression<Func<Task>> methodCall, Func<string> cronExpression);
    void Recurring<T>(string jobName, Expression<Action<T>> methodCall, Func<string> cronExpression);
    void Recurring<T>(string jobName, Expression<Func<T, Task>> methodCall, Func<string> cronExpression);

    void Recurring(string jobName, Expression<Action> methodCall, string cronExpression);

    void Recurring(string jobName, Expression<Func<Task>> methodCall, string cronExpression);
    void Recurring<T>(string jobName, Expression<Action<T>> methodCall, string cronExpression);
    void Recurring<T>(string jobName, Expression<Func<T, Task>> methodCall, string cronExpression);

    void RecurringRemoveIfExists(string jobId);

    void AddOrUpdate(string id, Expression<Func<Task>> methodCall, Func<string> cron, TimeZoneInfo timeZone, string queue);

    void AddOrUpdate<T>(string id, Expression<Func<T, Task>> methodCall, Func<string> cron, TimeZoneInfo timeZone, string queue);
    void AddOrUpdate<T>(string id, Expression<Func<T, Task>> methodCall, string cron, TimeZoneInfo timeZone, string queue);


    void AddOrUpdate(string id, Expression<Action> methodCall, Func<string> cron, TimeZoneInfo timeZone, string queue);

    void AddOrUpdate<T>(string id, Expression<Action<T>> methodCall, Func<string> cron, TimeZoneInfo timeZone, string queue);
}