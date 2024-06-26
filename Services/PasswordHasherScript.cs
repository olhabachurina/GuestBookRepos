using GuestBookRepos.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GuestBookRepos.Service
{
    public class PasswordHasherScript
    {
        private readonly ApplicationDbContext _context;
        private readonly IPasswordHasher<User> _passwordHasher;

        public PasswordHasherScript(ApplicationDbContext context, IPasswordHasher<User> passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }

        public async Task RehashPasswords()
        {
            var users = await _context.Users.ToListAsync();
            foreach (var user in users)
            {
                // Проверяем, является ли пароль уже захешированным
                try
                {
                    Convert.FromBase64String(user.Pwd);
                }
                catch (FormatException)
                {
                    // Если не является, хешируем пароль
                    user.Pwd = _passwordHasher.HashPassword(user, user.Pwd);
                }
            }

            await _context.SaveChangesAsync();
        }
    }
}

