using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LoveBeeApi.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        public IAuthRepository Auth { get; private set; }

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            Auth = new AuthRepository(context);
        }

        public void Complete()
        {
            _context.SaveChangesAsync();
        }
    }
}
