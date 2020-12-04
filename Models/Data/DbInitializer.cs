using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace API.Models.Data
{
    public class DbInitializer
    {
        public static void Initialize(GeneralDbContext context)
        {
            context.Database.Migrate();

            if (!context.Users.Any(i => i.LoginName == "admin"))
            {
                context.Users.Add(new User()
                { LoginName = "admin", Name = "admin", Password = CryptoHelper.Crypto.HashPassword("admin"), IsAdmin = true });
                context.SaveChanges();
            }
        }
    }
}