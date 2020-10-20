using SP.Messenger.Persistence;
using System;

namespace SP.Messenger.Application.Test.Infrastructure
{
    public class CommandTestBase : IDisposable
    {
        private readonly MessengerDbContext _context;
        public CommandTestBase()
        {
            _context = MessengerContextFactory.Create();
        }
        public void Dispose()
        {
            MessengerContextFactory.Destroy(_context);
        }
    }
}
