using System;

namespace StarterKit.Hangfire.Web.Jobs
{
    public class CustomRecurringJob : IRecurringJob
    {
        public Guid GenerateGuid()
        {
            return Guid.NewGuid();
        }

        public void Dispose()
        {
        }
    }

    public interface IRecurringJob : IDisposable
    {
        Guid GenerateGuid();
    }
}