using Microsoft.AspNetCore.Identity;
using Planora.Domain.Entities;
using Planora.Domain.Constants;
using Planora.Domain.Enums;
using Planora.Infrastructure.Data;

namespace Planora.Infrastructure;

public static class SeedData
{
    public static async Task InitializeAsync(
        PlanoraDbContext context,
        UserManager<User> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        string[] roles = { AppRoles.Admin, AppRoles.Teacher, AppRoles.Student };
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole(role));
        }

        var adminEmail = "admin@planora.ua";
        if (await userManager.FindByEmailAsync(adminEmail) == null)
        {
            var admin = new User
            {
                UserName = adminEmail,
                Email = adminEmail,
                FullName = "System Administrator",
                EmailConfirmed = true
            };
            var result = await userManager.CreateAsync(admin, "Admin123!");
            if (result.Succeeded)
                await userManager.AddToRoleAsync(admin, AppRoles.Admin);
        }

        if (!context.TimeSlots.Any())
        {
            context.TimeSlots.AddRange(
                new TimeSlot { Number = 1, StartTime = new TimeSpan(8, 30, 0),  EndTime = new TimeSpan(9, 50, 0) },
                new TimeSlot { Number = 2, StartTime = new TimeSpan(10, 10, 0), EndTime = new TimeSpan(11, 30, 0) },
                new TimeSlot { Number = 3, StartTime = new TimeSpan(11, 50, 0), EndTime = new TimeSpan(13, 10, 0) },
                new TimeSlot { Number = 4, StartTime = new TimeSpan(13, 30, 0), EndTime = new TimeSpan(14, 50, 0) },
                new TimeSlot { Number = 5, StartTime = new TimeSpan(15, 05, 0), EndTime = new TimeSpan(16, 25, 0) },
                new TimeSlot { Number = 6, StartTime = new TimeSpan(16, 40, 0), EndTime = new TimeSpan(18, 0, 0) }
            );
            await context.SaveChangesAsync();
        }

        if (!context.Buildings.Any())
        {
            context.Buildings.AddRange(
                new Building { Name = "Головний корпус", Address = "вул. Університетська, 1" },
                new Building { Name = "Корпус Фізичного факультету", Address = "вул. Драгоманова, 50" },
                new Building { Name = "Корпус Економічного факультету", Address = "пр. Свободи, 18" },
                new Building { Name = "Корпус Історичного факультету", Address = "вул. Коперника, 15" }
            );
            await context.SaveChangesAsync();
        }

        if (!context.Classrooms.Any())
        {
            var b1 = context.Buildings.First(b => b.Name == "Головний корпус");
            var b2 = context.Buildings.First(b => b.Name == "Корпус Фізичного факультету");
            var b3 = context.Buildings.First(b => b.Name == "Корпус Економічного факультету");
            var b4 = context.Buildings.First(b => b.Name == "Корпус Історичного факультету");

            context.Classrooms.AddRange(
                new Classrooms { Number = "115", Capacity = 120, HasComputers = false, HasProjector = true,  Faculty = "ФПМІ",      BuildingId = b1.Id },
                new Classrooms { Number = "116", Capacity = 80,  HasComputers = false, HasProjector = true,  Faculty = "Мех-мат",   BuildingId = b1.Id },
                new Classrooms { Number = "117", Capacity = 60,  HasComputers = true,  HasProjector = true,  Faculty = "ФПМІ",      BuildingId = b1.Id },
                new Classrooms { Number = "118", Capacity = 30,  HasComputers = true,  HasProjector = false, Faculty = "ФПМІ",      BuildingId = b1.Id },
                new Classrooms { Number = "219", Capacity = 40,  HasComputers = true,  HasProjector = true,  Faculty = "ФПМІ",      BuildingId = b1.Id },
                new Classrooms { Number = "220", Capacity = 40,  HasComputers = false, HasProjector = false, Faculty = "Загальний", BuildingId = b1.Id },
                new Classrooms { Number = "339", Capacity = 90,  HasComputers = false, HasProjector = true,  Faculty = "Мех-мат",   BuildingId = b1.Id },
                new Classrooms { Number = "340", Capacity = 300, HasComputers = false, HasProjector = true,  Faculty = "Загальний", BuildingId = b1.Id },
                new Classrooms { Number = "101", Capacity = 100, HasComputers = false, HasProjector = false, Faculty = "Загальний", BuildingId = b1.Id },
                new Classrooms { Number = "102", Capacity = 60,  HasComputers = false, HasProjector = false, Faculty = "Загальний", BuildingId = b1.Id },
                new Classrooms { Number = "103", Capacity = 50,  HasComputers = true,  HasProjector = false, Faculty = "Загальний", BuildingId = b1.Id },

                new Classrooms { Number = "200", Capacity = 150, HasComputers = false, HasProjector = true,  Faculty = "Фізичний",  BuildingId = b2.Id },
                new Classrooms { Number = "10",  Capacity = 30,  HasComputers = true,  HasProjector = true,  Faculty = "Фізичний",  BuildingId = b2.Id },
                new Classrooms { Number = "11",  Capacity = 25,  HasComputers = true,  HasProjector = true,  Faculty = "Фізичний",  BuildingId = b2.Id },
                new Classrooms { Number = "12",  Capacity = 30,  HasComputers = false, HasProjector = false, Faculty = "Фізичний",  BuildingId = b2.Id },
                new Classrooms { Number = "13",  Capacity = 20,  HasComputers = true,  HasProjector = false, Faculty = "Фізичний",  BuildingId = b2.Id },

                new Classrooms { Number = "201", Capacity = 180, HasComputers = false, HasProjector = true,  Faculty = "Економічний", BuildingId = b3.Id },
                new Classrooms { Number = "202", Capacity = 35,  HasComputers = false, HasProjector = true,  Faculty = "Економічний", BuildingId = b3.Id },
                new Classrooms { Number = "203", Capacity = 30,  HasComputers = true,  HasProjector = false, Faculty = "Економічний", BuildingId = b3.Id },
                new Classrooms { Number = "305", Capacity = 60,  HasComputers = false, HasProjector = true,  Faculty = "Економічний", BuildingId = b3.Id },
                new Classrooms { Number = "306", Capacity = 40,  HasComputers = true,  HasProjector = true,  Faculty = "Економічний", BuildingId = b3.Id },

                new Classrooms { Number = "3",   Capacity = 40,  HasComputers = true,  HasProjector = true,  Faculty = "Історичний", BuildingId = b4.Id },
                new Classrooms { Number = "4",   Capacity = 35,  HasComputers = true,  HasProjector = true,  Faculty = "Історичний", BuildingId = b4.Id },
                new Classrooms { Number = "6",   Capacity = 25,  HasComputers = true,  HasProjector = true,  Faculty = "Історичний", BuildingId = b4.Id },
                new Classrooms { Number = "5",   Capacity = 80,  HasComputers = false, HasProjector = true,  Faculty = "Історичний", BuildingId = b4.Id }
            );
            await context.SaveChangesAsync();
        }

        if (!context.Groups.Any())
        {
            context.Groups.AddRange(
                new Groups { Name = "ПМІ-11", Faculty = "ФПМІ",  StudentCount = 28 },
                new Groups { Name = "ПМІ-12", Faculty = "ФПМІ",  StudentCount = 25 },
                new Groups { Name = "ПМА-21", Faculty = "ФПМІ",  StudentCount = 27 },
                new Groups { Name = "МТ-11",  Faculty = "Мех-мат", StudentCount = 30 },
                new Groups { Name = "ЕК-11",  Faculty = "Економічний", StudentCount = 28 },
                new Groups { Name = "ЕК-12",  Faculty = "Економічний", StudentCount = 25 },
                new Groups { Name = "ФЗ-11",  Faculty = "Фізичний",   StudentCount = 22 }
            );
            await context.SaveChangesAsync();
        }

        if (!context.Subjects.Any())
        {
            context.Subjects.AddRange(
                new Subjects { Name = "Математичний аналіз",          Type = LessonType.Lecture,  Requirements = "Проектор" },
                new Subjects { Name = "Математичний аналіз",          Type = LessonType.Practice, Requirements = null },
                
                new Subjects { Name = "Лінійна алгебра",              Type = LessonType.Lecture,  Requirements = "Проектор" },
                new Subjects { Name = "Лінійна алгебра",              Type = LessonType.Practice, Requirements = null },
                
                new Subjects { Name = "Програмування (C++)",          Type = LessonType.Lecture,  Requirements = null },
                new Subjects { Name = "Програмування (C++)",          Type = LessonType.Lab,      Requirements = "Комп'ютерний клас" },
                
                new Subjects { Name = "Алгоритми та структури даних", Type = LessonType.Lecture,  Requirements = "Проектор" },
                new Subjects { Name = "Алгоритми та структури даних", Type = LessonType.Lab,      Requirements = "Комп'ютерний клас" },
                
                new Subjects { Name = "Бази даних",                   Type = LessonType.Lecture,  Requirements = "Проектор" },
                new Subjects { Name = "Бази даних",                   Type = LessonType.Lab,      Requirements = "Комп'ютерний клас" },
                
                new Subjects { Name = "Веб-навігація",                Type = LessonType.Lecture,  Requirements = "Проектор" },
                new Subjects { Name = "Веб-навігація",                Type = LessonType.Lab,      Requirements = "Комп'ютерний клас" },

                new Subjects { Name = "Математична статистика",       Type = LessonType.Lecture,  Requirements = null },
                new Subjects { Name = "Математична статистика",       Type = LessonType.Practice, Requirements = null },
                
                new Subjects { Name = "Теоретична фізика",            Type = LessonType.Lecture,  Requirements = "Проектор" },
                new Subjects { Name = "Теоретична фізика",            Type = LessonType.Lab,      Requirements = "Фізична лабораторія" },

                new Subjects { Name = "Основи екології",              Type = LessonType.Lecture,  Requirements = null },

                new Subjects { Name = "Англійська мова",              Type = LessonType.Practice, Requirements = null },

                new Subjects { Name = "Історія України",              Type = LessonType.Lecture,  Requirements = null },
                new Subjects { Name = "Історія України",              Type = LessonType.Practice, Requirements = null },

                new Subjects { Name = "Філософія",                    Type = LessonType.Lecture,  Requirements = null },

                new Subjects { Name = "Мікроекономіка",               Type = LessonType.Lecture,  Requirements = null },
                new Subjects { Name = "Мікроекономіка",               Type = LessonType.Practice, Requirements = null },
                
                new Subjects { Name = "Макроекономіка",               Type = LessonType.Lecture,  Requirements = "Проектор" },
                new Subjects { Name = "Макроекономіка",               Type = LessonType.Practice, Requirements = null },

                new Subjects { Name = "Вища математика",              Type = LessonType.Lecture,  Requirements = null },
                new Subjects { Name = "Вища математика",              Type = LessonType.Practice, Requirements = null },

                new Subjects { Name = "Дискретна математика",         Type = LessonType.Lecture,  Requirements = null },
                new Subjects { Name = "Дискретна математика",         Type = LessonType.Practice, Requirements = null },

                new Subjects { Name = "Фізичне виховання",            Type = LessonType.Practice, Requirements = "Спортзал" },

                new Subjects { Name = "Комп'ютерні мережі",           Type = LessonType.Lecture,  Requirements = "Проектор" },
                new Subjects { Name = "Комп'ютерні мережі",           Type = LessonType.Lab,      Requirements = "Комп'ютерний клас" }
            );
            await context.SaveChangesAsync();
        }

        var teacherData = new[]
        {
            ("tkachenko@lnu.edu.ua",  "Ткаченко Віктор Анатолійович",  "ФПМІ",        "Доцент"),
            ("zavhorodnya@lnu.edu.ua","Завгородня Олена Миколаївна",   "ФПМІ",        "Професор"),
            ("panchenko@lnu.edu.ua",  "Панченко Дмитро Ігорович",      "Мех-мат",     "Старший викладач"),
            ("levytskyy@lnu.edu.ua",  "Левицький Андрій Богданович",   "Мех-мат",     "Доцент"),
            ("kravets@lnu.edu.ua",    "Кравець Марія Олександрівна",   "Економічний", "Професор"),
            ("stepanenko@lnu.edu.ua", "Степаненко Юрій Васильович",    "Економічний", "Старший викладач"),
            ("mykhaylov@lnu.edu.ua",  "Михайлов Олександр Петрович",   "Фізичний",    "Доцент"),
            ("hryhorchuk@lnu.edu.ua", "Григорчук Тетяна Степанівна",   "ФПМІ",        "Доцент"),
            ("melnychuk@lnu.edu.ua",  "Мельничук Віталій Іванович",    "ФПМІ",        "Доцент"),
            ("koval@lnu.edu.ua",      "Коваль Ірина Павлівна",         "Загальний",   "Старший викладач"),
            ("lisova@lnu.edu.ua",     "Лісова Надія Олексіївна",       "Загальний",   "Доцент"),
            ("petrenko@lnu.edu.ua",   "Петренко Ірина Михайлівна",     "Загальний",   "Старший викладач"),
            ("bojko@lnu.edu.ua",      "Бойко Василь Петрович",         "ФПМІ",        "Доцент"),
            ("oleksandra@gmail.com",  "Мельник Олександра Іванівна",   "ФПМІ",        "Старший викладач")
        };

        foreach (var (email, fullName, faculty, position) in teacherData)
        {
            if (await userManager.FindByEmailAsync(email) == null)
            {
                var teacher = new User
                {
                    UserName = email,
                    Email = email,
                    FullName = fullName,
                    Faculty = faculty,
                    Position = position,
                    EmailConfirmed = true
                };
                var pwd = email == "oleksandra@gmail.com" ? "Oleksandra123!" : "Teacher123!";
                var result = await userManager.CreateAsync(teacher, pwd);
                if (result.Succeeded)
                    await userManager.AddToRoleAsync(teacher, AppRoles.Teacher);
            }
        }

        var studentData = new[]
        {
            ("student1@lnu.edu.ua", "Мельник Олександр Ігорович", "ФПМІ",        "ПМІ-11"),
            ("student2@lnu.edu.ua", "Козак Дарина Олексіївна",   "Економічний", "ЕК-11"),
            ("student3@lnu.edu.ua", "Литвин Максим Андрійович",  "Мех-мат",     "МТ-11"),
        };

        foreach (var (email, fullName, faculty, groupName) in studentData)
        {
            if (await userManager.FindByEmailAsync(email) == null)
            {
                var groups = context.Groups.FirstOrDefault(g => g.Name == groupName);
                var student = new User
                {
                    UserName = email,
                    Email = email,
                    FullName = fullName,
                    Faculty = faculty,
                    GroupId = groups?.Id,
                    EmailConfirmed = true
                };
                var result = await userManager.CreateAsync(student, "Student123!");
                if (result.Succeeded)
                    await userManager.AddToRoleAsync(student, AppRoles.Student);
            }
        }

        if (!context.TeachingAssignments.Any())
        {
            var teachers = (await userManager.GetUsersInRoleAsync(AppRoles.Teacher)).ToList();
            var subjects = context.Subjects.ToList();
            var groups   = context.Groups.ToList();

            Subjects? S(string name, LessonType type) => subjects.FirstOrDefault(s => s.Name == name && s.Type == type);
            User? T(string email) => teachers.FirstOrDefault(t => t.Email == email);
            Groups? G(string name) => groups.FirstOrDefault(g => g.Name == name);

            var assignmentsToAdd = new List<TeachingAssignment>();

            void AddAssignment(string teacherEmail, string subjectName, LessonType type, string groupName, int hours)
            {
                var teacher = T(teacherEmail);
                var subject = S(subjectName, type);
                var group = G(groupName);

                if (teacher != null && subject != null && group != null)
                {
                    assignmentsToAdd.Add(new TeachingAssignment 
                    { 
                        TeacherId = teacher.Id, 
                        SubjectId = subject.Id, 
                        GroupId = group.Id, 
                        HoursPerWeek = hours 
                    });
                }
            }

            // Рясне генерування навантаження:
            // ПМІ-11
            AddAssignment("tkachenko@lnu.edu.ua",     "Програмування (C++)",          LessonType.Lecture,  "ПМІ-11", 2);
            AddAssignment("tkachenko@lnu.edu.ua",     "Програмування (C++)",          LessonType.Lab,      "ПМІ-11", 4);
            AddAssignment("oleksandra@gmail.com",     "Математичний аналіз",          LessonType.Lecture,  "ПМІ-11", 2);
            AddAssignment("oleksandra@gmail.com",     "Математичний аналіз",          LessonType.Practice, "ПМІ-11", 2);
            AddAssignment("koval@lnu.edu.ua",         "Англійська мова",              LessonType.Practice, "ПМІ-11", 2);
            AddAssignment("lisova@lnu.edu.ua",        "Історія України",              LessonType.Lecture,  "ПМІ-11", 2);
            AddAssignment("lisova@lnu.edu.ua",        "Історія України",              LessonType.Practice, "ПМІ-11", 2);
            AddAssignment("petrenko@lnu.edu.ua",      "Фізичне виховання",            LessonType.Practice, "ПМІ-11", 2);
            AddAssignment("zavhorodnya@lnu.edu.ua",   "Комп'ютерні мережі",           LessonType.Lecture,  "ПМІ-11", 2);
            AddAssignment("zavhorodnya@lnu.edu.ua",   "Комп'ютерні мережі",           LessonType.Lab,      "ПМІ-11", 4);

            // ПМІ-12
            AddAssignment("tkachenko@lnu.edu.ua",     "Програмування (C++)",          LessonType.Lecture,  "ПМІ-12", 2);
            AddAssignment("tkachenko@lnu.edu.ua",     "Програмування (C++)",          LessonType.Lab,      "ПМІ-12", 4);
            AddAssignment("oleksandra@gmail.com",     "Математичний аналіз",          LessonType.Lecture,  "ПМІ-12", 2);
            AddAssignment("oleksandra@gmail.com",     "Математичний аналіз",          LessonType.Practice, "ПМІ-12", 2);
            AddAssignment("koval@lnu.edu.ua",         "Англійська мова",              LessonType.Practice, "ПМІ-12", 2);
            AddAssignment("lisova@lnu.edu.ua",        "Філософія",                    LessonType.Lecture,  "ПМІ-12", 2);
            AddAssignment("petrenko@lnu.edu.ua",      "Фізичне виховання",            LessonType.Practice, "ПМІ-12", 2);
            AddAssignment("zavhorodnya@lnu.edu.ua",   "Комп'ютерні мережі",           LessonType.Lecture,  "ПМІ-12", 2);
            AddAssignment("zavhorodnya@lnu.edu.ua",   "Комп'ютерні мережі",           LessonType.Lab,      "ПМІ-12", 2);

            // ПМА-21
            AddAssignment("panchenko@lnu.edu.ua",     "Алгоритми та структури даних", LessonType.Lecture,  "ПМА-21", 2);
            AddAssignment("panchenko@lnu.edu.ua",     "Алгоритми та структури даних", LessonType.Lab,      "ПМА-21", 4);
            AddAssignment("hryhorchuk@lnu.edu.ua",    "Бази даних",                   LessonType.Lecture,  "ПМА-21", 2);
            AddAssignment("hryhorchuk@lnu.edu.ua",    "Бази даних",                   LessonType.Lab,      "ПМА-21", 4);
            AddAssignment("zavhorodnya@lnu.edu.ua",   "Веб-навігація",                LessonType.Lecture,  "ПМА-21", 2);
            AddAssignment("zavhorodnya@lnu.edu.ua",   "Веб-навігація",                LessonType.Lab,      "ПМА-21", 2);
            AddAssignment("koval@lnu.edu.ua",         "Англійська мова",              LessonType.Practice, "ПМА-21", 2);
            AddAssignment("petrenko@lnu.edu.ua",      "Фізичне виховання",            LessonType.Practice, "ПМА-21", 2);
            AddAssignment("oleksandra@gmail.com",     "Дискретна математика",         LessonType.Lecture,  "ПМА-21", 2);
            AddAssignment("bojko@lnu.edu.ua",         "Дискретна математика",         LessonType.Practice, "ПМА-21", 2);

            // МТ-11
            AddAssignment("levytskyy@lnu.edu.ua",     "Лінійна алгебра",              LessonType.Lecture,  "МТ-11", 2);
            AddAssignment("levytskyy@lnu.edu.ua",     "Лінійна алгебра",              LessonType.Practice, "МТ-11", 4);
            AddAssignment("zavhorodnya@lnu.edu.ua",   "Математичний аналіз",          LessonType.Lecture,  "МТ-11", 4);
            AddAssignment("zavhorodnya@lnu.edu.ua",   "Математичний аналіз",          LessonType.Practice, "МТ-11", 4);
            AddAssignment("panchenko@lnu.edu.ua",     "Математична статистика",       LessonType.Lecture,  "МТ-11", 2);
            AddAssignment("panchenko@lnu.edu.ua",     "Математична статистика",       LessonType.Practice, "МТ-11", 2);
            AddAssignment("lisova@lnu.edu.ua",        "Історія України",              LessonType.Lecture,  "МТ-11", 2);
            AddAssignment("petrenko@lnu.edu.ua",      "Фізичне виховання",            LessonType.Practice, "МТ-11", 2);

            // ЕК-11
            AddAssignment("stepanenko@lnu.edu.ua",    "Мікроекономіка",               LessonType.Lecture,  "ЕК-11", 2);
            AddAssignment("stepanenko@lnu.edu.ua",    "Мікроекономіка",               LessonType.Practice, "ЕК-11", 2);
            AddAssignment("kravets@lnu.edu.ua",       "Макроекономіка",               LessonType.Lecture,  "ЕК-11", 2);
            AddAssignment("kravets@lnu.edu.ua",       "Макроекономіка",               LessonType.Practice, "ЕК-11", 2);
            AddAssignment("levytskyy@lnu.edu.ua",     "Вища математика",              LessonType.Lecture,  "ЕК-11", 2);
            AddAssignment("levytskyy@lnu.edu.ua",     "Вища математика",              LessonType.Practice, "ЕК-11", 2);
            AddAssignment("koval@lnu.edu.ua",         "Англійська мова",              LessonType.Practice, "ЕК-11", 2);
            AddAssignment("petrenko@lnu.edu.ua",      "Фізичне виховання",            LessonType.Practice, "ЕК-11", 2);
            AddAssignment("lisova@lnu.edu.ua",        "Історія України",              LessonType.Lecture,  "ЕК-11", 2);
            AddAssignment("lisova@lnu.edu.ua",        "Історія України",              LessonType.Practice, "ЕК-11", 2);

            // ЕК-12
            AddAssignment("stepanenko@lnu.edu.ua",    "Мікроекономіка",               LessonType.Lecture,  "ЕК-12", 2);
            AddAssignment("stepanenko@lnu.edu.ua",    "Мікроекономіка",               LessonType.Practice, "ЕК-12", 2);
            AddAssignment("kravets@lnu.edu.ua",       "Макроекономіка",               LessonType.Lecture,  "ЕК-12", 2);
            AddAssignment("kravets@lnu.edu.ua",       "Макроекономіка",               LessonType.Practice, "ЕК-12", 2);
            AddAssignment("lisova@lnu.edu.ua",        "Філософія",                    LessonType.Lecture,  "ЕК-12", 2);
            AddAssignment("koval@lnu.edu.ua",         "Англійська мова",              LessonType.Practice, "ЕК-12", 2);
            AddAssignment("petrenko@lnu.edu.ua",      "Фізичне виховання",            LessonType.Practice, "ЕК-12", 2);
            AddAssignment("levytskyy@lnu.edu.ua",     "Вища математика",              LessonType.Lecture,  "ЕК-12", 2);
            AddAssignment("levytskyy@lnu.edu.ua",     "Вища математика",              LessonType.Practice, "ЕК-12", 2);

            // ФЗ-11
            AddAssignment("mykhaylov@lnu.edu.ua",     "Теоретична фізика",            LessonType.Lecture,  "ФЗ-11", 4);
            AddAssignment("mykhaylov@lnu.edu.ua",     "Теоретична фізика",            LessonType.Lab,      "ФЗ-11", 4);
            AddAssignment("zavhorodnya@lnu.edu.ua",   "Математичний аналіз",          LessonType.Lecture,  "ФЗ-11", 2);
            AddAssignment("zavhorodnya@lnu.edu.ua",   "Математичний аналіз",          LessonType.Practice, "ФЗ-11", 2);
            AddAssignment("oleksandra@gmail.com",     "Основи екології",              LessonType.Lecture,  "ФЗ-11", 2);
            AddAssignment("koval@lnu.edu.ua",         "Англійська мова",              LessonType.Practice, "ФЗ-11", 2);
            AddAssignment("petrenko@lnu.edu.ua",      "Фізичне виховання",            LessonType.Practice, "ФЗ-11", 2);

            context.TeachingAssignments.AddRange(assignmentsToAdd);
            await context.SaveChangesAsync();
        }

        if (!context.GroupDisciplineLists.Any())
        {
            var groups   = context.Groups.ToList();
            var subjects = context.Subjects.ToList();

            Groups G(string name)   => groups.First(g => g.Name == name);
            Subjects S(string name, LessonType type) => subjects.First(s => s.Name == name && s.Type == type);

            var gs = new List<GroupDisciplineList>();

            gs.AddRange(MakeGroupPlan(G("ПМІ-11"), S, new[] {
                ("Математичний аналіз", 2, LessonType.Lecture), ("Математичний аналіз", 2, LessonType.Practice),
                ("Програмування (C++)", 2, LessonType.Lecture), ("Програмування (C++)", 4, LessonType.Lab),
                ("Історія України", 2, LessonType.Lecture), ("Історія України", 2, LessonType.Practice),
                ("Англійська мова", 2, LessonType.Practice)
            }));

            gs.AddRange(MakeGroupPlan(G("ПМІ-12"), S, new[] {
                ("Математичний аналіз", 2, LessonType.Lecture), ("Математичний аналіз", 2, LessonType.Practice),
                ("Програмування (C++)", 2, LessonType.Lecture), ("Програмування (C++)", 4, LessonType.Lab),
                ("Філософія", 2, LessonType.Lecture),
                ("Англійська мова", 2, LessonType.Practice)
            }));

            gs.AddRange(MakeGroupPlan(G("ПМА-21"), S, new[] {
                ("Алгоритми та структури даних", 2, LessonType.Lecture), ("Алгоритми та структури даних", 4, LessonType.Lab),
                ("Бази даних", 2, LessonType.Lecture), ("Бази даних", 4, LessonType.Lab),
                ("Веб-навігація", 2, LessonType.Lecture), ("Веб-навігація", 2, LessonType.Lab),
                ("Англійська мова", 2, LessonType.Practice),
                ("Фізичне виховання", 2, LessonType.Practice),
                ("Дискретна математика", 2, LessonType.Lecture), ("Дискретна математика", 2, LessonType.Practice)
            }));

            gs.AddRange(MakeGroupPlan(G("МТ-11"), S, new[] {
                ("Лінійна алгебра", 2, LessonType.Lecture), ("Лінійна алгебра", 4, LessonType.Practice),
                ("Математичний аналіз", 4, LessonType.Lecture), ("Математичний аналіз", 4, LessonType.Practice),
                ("Математична статистика", 2, LessonType.Lecture), ("Математична статистика", 2, LessonType.Practice),
                ("Історія України", 2, LessonType.Lecture),
                ("Фізичне виховання", 2, LessonType.Practice)
            }));

            gs.AddRange(MakeGroupPlan(G("ЕК-11"), S, new[] {
                ("Мікроекономіка", 2, LessonType.Lecture), ("Мікроекономіка", 2, LessonType.Practice),
                ("Макроекономіка", 2, LessonType.Lecture), ("Макроекономіка", 2, LessonType.Practice),
                ("Вища математика", 2, LessonType.Lecture), ("Вища математика", 2, LessonType.Practice),
                ("Англійська мова", 2, LessonType.Practice),
                ("Фізичне виховання", 2, LessonType.Practice),
                ("Історія України", 2, LessonType.Lecture), ("Історія України", 2, LessonType.Practice)
            }));

            gs.AddRange(MakeGroupPlan(G("ЕК-12"), S, new[] {
                ("Мікроекономіка", 2, LessonType.Lecture), ("Мікроекономіка", 2, LessonType.Practice),
                ("Макроекономіка", 2, LessonType.Lecture), ("Макроекономіка", 2, LessonType.Practice),
                ("Філософія", 2, LessonType.Lecture),
                ("Англійська мова", 2, LessonType.Practice),
                ("Фізичне виховання", 2, LessonType.Practice),
                ("Вища математика", 2, LessonType.Lecture), ("Вища математика", 2, LessonType.Practice)
            }));

            gs.AddRange(MakeGroupPlan(G("ФЗ-11"), S, new[] {
                ("Теоретична фізика", 4, LessonType.Lecture), ("Теоретична фізика", 4, LessonType.Lab),
                ("Математичний аналіз", 2, LessonType.Lecture), ("Математичний аналіз", 2, LessonType.Practice),
                ("Основи екології", 2, LessonType.Lecture),
                ("Англійська мова", 2, LessonType.Practice),
                ("Фізичне виховання", 2, LessonType.Practice)
            }));

            context.GroupDisciplineLists.AddRange(gs);
            await context.SaveChangesAsync();
        }

        if (!context.Schedules.Any(s => s.TeacherId == context.Users.FirstOrDefault(u => u.Email == "oleksandra@gmail.com").Id && s.DayOfWeek == DayOfWeekEnum.Monday))
        {
            var user = await userManager.FindByEmailAsync("oleksandra@gmail.com");
            var subject = context.Subjects.FirstOrDefault(s => s.Name == "Математичний аналіз" && s.Type == LessonType.Lecture);
            var subjectLab = context.Subjects.FirstOrDefault(s => s.Name == "Математичний аналіз" && s.Type == LessonType.Practice);
            var group = context.Groups.FirstOrDefault(g => g.Name == "ПМІ-11");
            var classroom = context.Classrooms.FirstOrDefault();
            var timeSlot1 = context.TimeSlots.FirstOrDefault(t => t.Number == 1);
            var timeSlot2 = context.TimeSlots.FirstOrDefault(t => t.Number == 2);

            if (user != null && subject != null && subjectLab != null && group != null && classroom != null && timeSlot1 != null && timeSlot2 != null)
            {
                context.Schedules.AddRange(
                    new Schedule
                    {
                        DayOfWeek = DayOfWeekEnum.Monday,
                        WeekType = WeekType.Both,
                        TimeSlotId = timeSlot1.Id,
                        ClassroomId = classroom.Id,
                        TeacherId = user.Id,
                        SubjectId = subject.Id,
                        GroupId = group.Id,
                        IsOnline = false
                    },
                    new Schedule
                    {
                        DayOfWeek = DayOfWeekEnum.Monday,
                        WeekType = WeekType.Both,
                        TimeSlotId = timeSlot2.Id,
                        ClassroomId = classroom.Id,
                        TeacherId = user.Id,
                        SubjectId = subjectLab.Id,
                        GroupId = group.Id,
                        IsOnline = false
                    }
                );
                await context.SaveChangesAsync();
            }
        }
    }

    private static List<GroupDisciplineList> MakeGroupPlan(
        Groups groups,
        Func<string, LessonType, Subjects> subjectLookup,
        (string SubjectName, int Hours, LessonType Type)[] plan)
    {
        return plan.Select(p => 
        {
            var subj = subjectLookup(p.SubjectName, p.Type);
            return new GroupDisciplineList
            {
                GroupId = groups.Id,
                SubjectId = subj.Id,
                HoursPerWeek = p.Hours,
                LessonType = p.Type
            };
        }).ToList();
    }
}
