using LMS.Controllers;
using LMS.Models.LMSModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace UnitTests
{
    public class UnitTest1
    {
        Team58LMSContext MakeMockDB()
        {
            var optionsBuilder = new DbContextOptionsBuilder<Team58LMSContext>();
            optionsBuilder.UseInMemoryDatabase("mock_db").UseApplicationServiceProvider(NewServiceProvider());
            Team58LMSContext db = new Team58LMSContext(optionsBuilder.Options);

            Departments d = new Departments();
            d.Name = "School of Computing";
            d.Subject = "CS";
            Departments d2 = new Departments();
            d.Name = "School of Music";
            d.Subject = "MUSC";
            Departments d3 = new Departments();
            d.Name = "School of Mathematics";
            d.Subject = "MATH";

            db.Departments.Add(d);
            db.Departments.Add(d2);
            db.Departments.Add(d3);
            db.SaveChanges();

            return db;
        }

        [Fact]
        public void TestGetDepartments()
        {
            CommonController c = new CommonController();
            c.UseLMSContext(MakeMockDB());

            var allDepartmentsResult = c.GetDepartments() as JsonResult;
            dynamic values = allDepartmentsResult.Value;
            Assert.Equal(3, values.Length);
            var x = values[0];
        }



        private static ServiceProvider NewServiceProvider()
        {
            var serviceProvider = new ServiceCollection().AddEntityFrameworkInMemoryDatabase().BuildServiceProvider();
            return serviceProvider;
        }
    }
}
