using OQS.CoreWebAPI.Database;
using OQS.CoreWebAPI.Entities;


namespace OQS.CoreWebAPI.Tests.SetUp
{
    public abstract class Seeder
    {
        public static void SeedDb(ApplicationDBContext dbContext)
        {
            SeedUsers(dbContext);
         
        }

        private static void SeedUsers(ApplicationDBContext dbContext)
        {
            var users = new List<User> 
            {
                new User
                {
                    Id = Guid.Parse("00000000-0000-0000-0001-000000000001").ToString(),
                   
                    Email = "email1@email.com"
                    
                },
                new User
                {
                    Id = Guid.Parse("00000000-0000-0000-0001-000000000002").ToString(),
                    
                    Email = "email2@email.com"
                },
                new User
                {
                    Id = Guid.Parse("00000000-0000-0000-0001-000000000003").ToString(),
                 
                    Email = "emai3@email.com"
                }
            };

            dbContext.Users.AddRange(users);
            dbContext.SaveChanges();
        }

      

       
       
     

       
    }
}