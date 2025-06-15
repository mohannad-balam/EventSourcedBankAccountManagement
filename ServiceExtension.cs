using EventSourcedBankAccountManagement.Applications.Handlers;
using EventSourcedBankAccountManagement.Infrastructure.EventStore;
using EventSourcedBankAccountManagement.Infrastructure.Projections;

namespace EventSourcedBankAccountManagement;

public static class ServiceExtension
{
    public static void RegisterEventsSourcingServices(this IServiceCollection services)
    {
        services.AddSingleton<IEventStore, InMemoryEventStore>();
        services.AddSingleton<AccountBalanceProjection>();
        services.AddSingleton<ProjectionWorker>();
        services.AddTransient<OpenAccountHandler>();
        services.AddTransient<DipositMoneyHandler>();
        services.AddTransient<WithdarwMoneyHandler>();
    }
}