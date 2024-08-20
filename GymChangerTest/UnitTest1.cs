using GymChanger.Controllers;
using GymChanger.Data;
using GymChanger.Models;
using GymChanger.ViewModels;
using GymManagement.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System.Net.Mime;
using Xunit;


namespace GymChangerTest
{
    public class TestDatabaseFixture
    {
        //Подключаем тестовую БД
        private const string ConnectionString = @"Data Source=SPEEDEVIL\SQLEXPRESS;Initial Catalog=GymChangerTest;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False";

        private static readonly object _lock = new();
        private static bool _databaseInitialized;

        //В конструкторе по умолчанию инициализируем её
        public TestDatabaseFixture()
        {
            lock (_lock)
            {
                if (!_databaseInitialized)
                {
                    using (var context = CreateContext())
                    {
                        context.Database.EnsureDeleted();
                        context.Database.EnsureCreated();

                    }
                    _databaseInitialized = true;
                }
            }
        }
        public ApplicationDbContext CreateContext()
        {
            return new ApplicationDbContext(new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlServer(ConnectionString)
                .Options);
        }
    }
    //Класс для тестирования 
    [TestClass]
    public class RoleControllerTest : IClassFixture<TestDatabaseFixture>
    {
        private Mock<RoleManager<IdentityRole>> roleManager;
        private Mock<UserManager<AppUser>> userManager;

        public TestDatabaseFixture Fixture { get; }

        public RoleControllerTest()
        {
            Fixture = new TestDatabaseFixture();
        }

    }
    //Класс для тестирования Course Контроллера
    [TestClass]
    public class CourseControllerTest : IClassFixture<TestDatabaseFixture>
    {
        private Mock<IHttpContextAccessor> httpContextAccessor;
        private Mock<UserManager<AppUser>> userManager;

        public TestDatabaseFixture Fixture { get; }
        public CourseControllerTest()
        {
            Fixture = new TestDatabaseFixture();
        }
        public CourseControllerTest(TestDatabaseFixture fixture)
        {
            Fixture = fixture;
        }

        //Тестирование метода Group
        [TestMethod]
        public async Task GroupTest()
        {
            //Arrange
            httpContextAccessor = new Mock<IHttpContextAccessor>();
            userManager = new Mock<UserManager<AppUser>>(Mock.Of<IUserStore<AppUser>>(), null, null, null, null, null, null, null, null);
            using var context = Fixture.CreateContext();

            var controller = new CourseController(context, userManager.Object, httpContextAccessor.Object);

            List<Course> courses_true = new List<Course>()
            {
                new Course {CourseId = 1 },
                new Course {CourseId = 2 }
            };

            List<Course> courses_false = new List<Course>()
            {
                new Course { CourseId = 3 },
                new Course { CourseId = 2 }
            };

            List<AppUser> appUsers = new List<AppUser>()
            {
                new AppUser { Courses = courses_true, Name = "Test_User1"},
                new AppUser { Courses = courses_false, Name = "Test_User2" }
            };

            userManager.Setup(c => c.GetUsersInRoleAsync(UserRoles.User)).ReturnsAsync(appUsers);

            //Act
            var result = await controller.Group(1);
            var viewResult = result as ViewResult;
            var model = viewResult?.Model as GroupViewModel;

            //Assert
            Assert.AreEqual(1, model.Users.Count);
            Assert.AreEqual("Test_User1", model.Users.First().Name);
            
        }
        //Тестировние метода Index
        [TestMethod]
        public void IndexTest()
        {
            //Arrange
            httpContextAccessor = new Mock<IHttpContextAccessor>();
            userManager = new Mock<UserManager<AppUser>>(Mock.Of<IUserStore<AppUser>>(), null, null, null, null, null, null, null, null);
            using var context = Fixture.CreateContext();

            var controller = new CourseController(context, userManager.Object, httpContextAccessor.Object);
            List<Course> courses_true = new List<Course>()
            {
                new Course {DayStart = "123", MonthStart = "123", TimeEnd = "123", TimeStart = "123", Title = "123", TrainerId = "123" },
                new Course {DayStart = "123", MonthStart = "123", TimeEnd = "123", TimeStart = "123", Title = "123", TrainerId = "123" },
                new Course {DayStart = "123", MonthStart = "123", TimeEnd = "123", TimeStart = "123", Title = "123", TrainerId = "123"},
                new Course {DayStart = "123", MonthStart = "123", TimeEnd = "123", TimeStart = "123", Title = "123", TrainerId = "123" }

            };
            context.Courses.AddRange(courses_true);
            context.SaveChanges();
            //Act
            var result = controller.Index();
            var viewResult = result as ViewResult;
            var model = viewResult?.Model as List<Course>;

            context.Courses.RemoveRange(courses_true);
            context.SaveChanges();

            //Assert
            Assert.AreEqual(4, model.Count);
        }
        //Тестирование метода Create
        [TestMethod]
        public async Task CreateTest()
        {
            //Arrange
            httpContextAccessor = new Mock<IHttpContextAccessor>();
            userManager = new Mock<UserManager<AppUser>>(Mock.Of<IUserStore<AppUser>>(), null, null, null, null, null, null, null, null);
            using var context = Fixture.CreateContext();

            var controller = new CourseController(context, userManager.Object, httpContextAccessor.Object);

            CreateCourseViewModel viewModel = new CreateCourseViewModel()
            {
                DayStart = "Test_Date",
                MonthStart = "123",
                TimeEnd = "123",
                TimeStart = "123",
                Title = "123",
                TrainerId = "123"
            };

            //Act
            await controller.Create(viewModel);
            List<Course> courses = context.Courses.ToList();

            context.Courses.Remove(courses.First());
            context.SaveChanges();

            //Assert
            Assert.AreEqual(1, courses.Count);
        }
    }
}