using GymChanger.Data;
using GymChanger.Models;
using GymChanger.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;

namespace GymManagement.Controllers
{
    public class CourseController : Controller
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public CourseController(ApplicationDbContext context, UserManager<AppUser> userManager, IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }


        public async Task<IActionResult> Group(int id)
        {
            var users = await _userManager.GetUsersInRoleAsync(UserRoles.User);
            var usersWithCourse = new List<AppUser>();
            _context.Users.Include(X => X.Courses).ToList();

            foreach (var user in users)
            {
                if (user.Courses == null)
                    continue;
                if (user.Courses.Any(c => c.CourseId == id))
                {
                    usersWithCourse.Add(user);
                }
            }
            var groupViewModel = new GroupViewModel
            {
                Users = usersWithCourse
            };
            return View(groupViewModel);
        }
        public IActionResult Index()
        {
            var courses = _context.Courses.ToList();
            return View(courses);
        }

        [HttpPost]
        public async Task<IActionResult> AssignCourseToUser(string userId, int courseId)
        {
            var user = await _userManager.Users.Include(u => u.Courses).FirstOrDefaultAsync(u => u.Id == userId);
            var course = await _context.Courses.FirstOrDefaultAsync(i => i.CourseId == courseId);

            if (course == null)
            {
                return View();
            }

            if (user != null && !await _userManager.IsInRoleAsync(user, UserRoles.Trainer))
            {
                user.Courses.Add(course);
                await _userManager.UpdateAsync(user);

                return RedirectToAction("Index");
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RemoveCourseFromUser(string userId, int courseId)
        {
            var user = await _userManager.Users.Include(u => u.Courses).FirstOrDefaultAsync(u => u.Id == userId);
            var course = await _context.Courses.FirstOrDefaultAsync(i => i.CourseId == courseId);

            if (course == null)
            {
                return View("Ошибка");
            }
            if (user != null && !await _userManager.IsInRoleAsync(user, UserRoles.Trainer))
            {
                if (user.Courses == null)
                {
                    return View("У пользователя нет курсов");
                }
                user.Courses.Remove(course);
                await _userManager.UpdateAsync(user);

                return RedirectToAction("Index");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RemoveCourseFromUserRedirect(string userId, int courseId)
        {
            var user = await _userManager.Users.Include(u => u.Courses).FirstOrDefaultAsync(u => u.Id == userId);
            var course = await _context.Courses.FirstOrDefaultAsync(i => i.CourseId == courseId);

            if (course == null)
            {
                return View("Ошибка");
            }
            if (user != null && !await _userManager.IsInRoleAsync(user, UserRoles.Trainer))
            {
                if (user.Courses == null)
                {
                    return View("У пользователя нет курсов");
                }
                user.Courses.Remove(course);
                await _userManager.UpdateAsync(user);

                return RedirectToAction("CheckMyCourses");
            }
            return View();
        }

        public async Task<IActionResult> Create()
        {
            var trainersFromRole = await _userManager.GetUsersInRoleAsync(UserRoles.Trainer);
            var trainers = trainersFromRole.Select(t => new
            {
                Id = t.Id,
                FullName = $"{t.Name} {t.Surname}"
            }).ToList();

            var selectedList = new SelectList(trainers, "Id", "FullName");
            ViewBag.TrainerList = selectedList;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateCourseViewModel CreateCourseViewModel)
        {
            if (ModelState.IsValid)
            {
                var course = new Course
                {
                    Title = CreateCourseViewModel.Title,
                    DayStart = CreateCourseViewModel.DayStart,
                    MonthStart = CreateCourseViewModel.MonthStart,
                    TimeStart = CreateCourseViewModel.TimeStart,
                    TimeEnd = CreateCourseViewModel.TimeEnd,
                    TrainerId = CreateCourseViewModel.TrainerId,
                };

                _context.Add(course);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index");
            }
            return View(CreateCourseViewModel);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var trainersFromRole = await _userManager.GetUsersInRoleAsync(UserRoles.Trainer);
            var trainers = trainersFromRole.Select(t => new
            {
                Id = t.Id,
                FullName = $"{t.Name} {t.Surname}"
            }).ToList();

            var selectedList = new SelectList(trainers, "Id", "FullName");
            ViewBag.TrainerList = selectedList;

            var course = await _context.Courses.FirstOrDefaultAsync(i => i.CourseId == id);
            if (course == null) return View("Ошибка");
            var editCourseViewModel = new EditCourseViewModel
            {
                Title = course.Title,
                DayStart = course.DayStart,
                MonthStart = course.MonthStart,
                TimeStart = course.TimeStart,
                TimeEnd = course.TimeEnd,
                TrainerId = course.TrainerId
            };
            return View(editCourseViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, EditCourseViewModel editCourseViewModel)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Не удалось изменить курс");
                return View("Edit", editCourseViewModel);
            }
            var userCourse = await _context.Courses.AsNoTracking().FirstOrDefaultAsync(i => i.CourseId == id); ;
            if (userCourse != null)
            {
                var course = new Course
                {
                    CourseId = id,
                    Title = editCourseViewModel.Title,
                    DayStart = editCourseViewModel.DayStart,
                    MonthStart = editCourseViewModel.MonthStart,
                    TimeStart = editCourseViewModel.TimeStart,
                    TimeEnd = editCourseViewModel.TimeEnd,
                    TrainerId = editCourseViewModel.TrainerId,
                };
                _context.Update(course);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            else
            {
                return View(editCourseViewModel);
            }

        }

        public async Task<IActionResult> CheckMyCourses()
        {
            var currentUser = _httpContextAccessor.HttpContext.User;

            var userId = currentUser.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = _userManager.FindByIdAsync(userId).Result;

            var userRoles = await _userManager.GetRolesAsync(user);
            if ((userRoles.Contains(UserRoles.Admin) || userRoles.Contains(UserRoles.User)) && !userRoles.Contains(UserRoles.Trainer))
            {
                _context.Users.Include(X => X.Courses).ToList();
                if (user != null)
                {
                    if (user.Courses != null)
                    {
                        IEnumerable<Course> courses = user.Courses;
                        return View(courses);
                    }
                }
                return View();
            }
            else if (userRoles.Contains(UserRoles.Trainer))
            {
                if (user != null)
                {
                    IEnumerable<Course> courses = _context.Courses.Where(c => c.TrainerId == user.Id);
                    return View(courses);
                }
                return View();
            }
            else
                return View();
        }

    }
}
