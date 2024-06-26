using GuestBookRepos.Interfaces;
using GuestBookRepos.Models;
using Microsoft.EntityFrameworkCore;

namespace GuestBookRepos.Repositories
{
    public class Repository : IRepository
    {
        private readonly ApplicationDbContext _context;

        public Repository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<User> GetUserByIdAsync(int userId)
        {
            return await _context.Users.FindAsync(userId);
        }

        public async Task<User> GetUserByNameAndPasswordAsync(string name, string password)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Name == name && u.Pwd == password);
        }

        public async Task<User> GetUserByNameAsync(string name)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Name == name);
        }

        public async Task AddUserAsync(User user)
        {
            _context.Users.Add(user);
            await SaveChangesAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<List<Message>> GetAllMessagesAsync()
        {
            return await _context.Messages.Include(m => m.User).ToListAsync();
        }

        public async Task<List<Message>> GetMessagesByUserIdAsync(int userId)
        {
            return await _context.Messages.Where(m => m.Id_User == userId).Include(m => m.User).ToListAsync();
        }

        public async Task AddMessageAsync(Message message)
        {
            _context.Messages.Add(message);
            await SaveChangesAsync();
        }
    }
}

