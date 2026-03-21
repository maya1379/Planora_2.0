using Microsoft.AspNetCore.Identity;
using Planora.Domain.Entities;
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

        string[] roles = { "Admin", "Teacher", "Student" };
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
                Role = UserRole.Admin,
                EmailConfirmed = true
            };
            var result = await userManager.CreateAsync(admin, "Admin123!");
            if (result.Succeeded)
                await userManager.AddToRoleAsync(admin, "Admin");
        }

        if (!context.TimeSlots.Any())
        {
            context.TimeSlots.AddRange(
                new TimeSlot { Number = 1, StartTime = new TimeSpan(8, 30, 0),  EndTime = new TimeSpan(10, 5, 0) },
                new TimeSlot { Number = 2, StartTime = new TimeSpan(10, 15, 0), EndTime = new TimeSpan(11, 50, 0) },
                new TimeSlot { Number = 3, StartTime = new TimeSpan(12, 0, 0),  EndTime = new TimeSpan(13, 35, 0) },
                new TimeSlot { Number = 4, StartTime = new TimeSpan(13, 45, 0), EndTime = new TimeSpan(15, 20, 0) },
                new TimeSlot { Number = 5, StartTime = new TimeSpan(15, 30, 0), EndTime = new TimeSpan(17, 5, 0) },
                new TimeSlot { Number = 6, StartTime = new TimeSpan(17, 15, 0), EndTime = new TimeSpan(18, 50, 0) }
            );
            await context.SaveChangesAsync();
        }

        if (!context.Buildings.Any())
        {
            context.Buildings.AddRange(
                new Building { Name = "Корпус №1 (головний)",     Address = "вул. Університетська, 1" },
                new Building { Name = "Корпус №2 (технічний)",    Address = "вул. Університетська, 3" },
                new Building { Name = "Корпус №3 (гуманітарний)", Address = "вул. Наукова, 10" },
                new Building { Name = "Корпус №4 (лабораторний)", Address = "вул. Наукова, 12" }
            );
            await context.SaveChangesAsync();
        }

        if (!context.Classrooms.Any())
        {
            var b1 = context.Buildings.First(b => b.Name.Contains("№1"));
            var b2 = context.Buildings.First(b => b.Name.Contains("№2"));
            var b3 = context.Buildings.First(b => b.Name.Contains("№3"));
            var b4 = context.Buildings.First(b => b.Name.Contains("№4"));

            context.Classrooms.AddRange(

                new Classrooms { Number = "101", Capacity = 120, HasComputers = false, HasProjector = true,  Faculty = "Загальний", BuildingId = b1.Id },
                new Classrooms { Number = "102", Capacity = 80,  HasComputers = false, HasProjector = true,  Faculty = "Загальний", BuildingId = b1.Id },
                new Classrooms { Number = "103", Capacity = 60,  HasComputers = false, HasProjector = true,  Faculty = "ФІОТ",     BuildingId = b1.Id },
                new Classrooms { Number = "104", Capacity = 40,  HasComputers = false, HasProjector = true,  Faculty = "ФІОТ",     BuildingId = b1.Id },
                new Classrooms { Number = "105", Capacity = 40,  HasComputers = false, HasProjector = false, Faculty = "ФІТ",      BuildingId = b1.Id },

                new Classrooms { Number = "201", Capacity = 30,  HasComputers = true,  HasProjector = true,  Faculty = "ФІТ",      BuildingId = b2.Id },
                new Classrooms { Number = "202", Capacity = 30,  HasComputers = true,  HasProjector = true,  Faculty = "ФІТ",      BuildingId = b2.Id },
                new Classrooms { Number = "203", Capacity = 25,  HasComputers = true,  HasProjector = true,  Faculty = "ФІОТ",     BuildingId = b2.Id },
                new Classrooms { Number = "204", Capacity = 25,  HasComputers = true,  HasProjector = false, Faculty = "ФІОТ",     BuildingId = b2.Id },
                new Classrooms { Number = "205", Capacity = 20,  HasComputers = true,  HasProjector = true,  Faculty = "ФІТ",      BuildingId = b2.Id },

                new Classrooms { Number = "301", Capacity = 35,  HasComputers = false, HasProjector = true,  Faculty = "ФГН",      BuildingId = b3.Id },
                new Classrooms { Number = "302", Capacity = 35,  HasComputers = false, HasProjector = true,  Faculty = "ФГН",      BuildingId = b3.Id },
                new Classrooms { Number = "303", Capacity = 30,  HasComputers = false, HasProjector = false, Faculty = "ФГН",      BuildingId = b3.Id },
                new Classrooms { Number = "304", Capacity = 25,  HasComputers = false, HasProjector = true,  Faculty = "ФЕ",       BuildingId = b3.Id },

                new Classrooms { Number = "401", Capacity = 20,  HasComputers = true,  HasProjector = true,  Faculty = "ФІТ",      BuildingId = b4.Id },
                new Classrooms { Number = "402", Capacity = 20,  HasComputers = true,  HasProjector = true,  Faculty = "ФІОТ",     BuildingId = b4.Id },
                new Classrooms { Number = "403", Capacity = 15,  HasComputers = true,  HasProjector = true,  Faculty = "ФІТ",      BuildingId = b4.Id },
                new Classrooms { Number = "404", Capacity = 30,  HasComputers = false, HasProjector = true,  Faculty = "ФЕ",       BuildingId = b4.Id }
            );
            await context.SaveChangesAsync();
        }

        if (!context.Groups.Any())
        {
            context.Groups.AddRange(
                new Groups { Name = "КН-11", Faculty = "ФІТ",  StudentCount = 28 },
                new Groups { Name = "КН-12", Faculty = "ФІТ",  StudentCount = 25 },
                new Groups { Name = "КН-21", Faculty = "ФІТ",  StudentCount = 27 },
                new Groups { Name = "КН-22", Faculty = "ФІТ",  StudentCount = 24 },
                new Groups { Name = "КН-31", Faculty = "ФІТ",  StudentCount = 22 },
                new Groups { Name = "КН-32", Faculty = "ФІТ",  StudentCount = 20 },
                new Groups { Name = "ІО-11", Faculty = "ФІОТ", StudentCount = 30 },
                new Groups { Name = "ІО-12", Faculty = "ФІОТ", StudentCount = 26 },
                new Groups { Name = "ІО-21", Faculty = "ФІОТ", StudentCount = 28 },
                new Groups { Name = "ІО-22", Faculty = "ФІОТ", StudentCount = 25 },
                new Groups { Name = "ЕК-11", Faculty = "ФЕ",   StudentCount = 22 },
                new Groups { Name = "ЕК-21", Faculty = "ФЕ",   StudentCount = 20 }
            );
            await context.SaveChangesAsync();
        }

        if (!context.Subjects.Any())
        {
            context.Subjects.AddRange(

                new Subjects { Name = "Вища математика",             Type = LessonType.Lecture,  Requirements = "Проектор" },
                new Subjects { Name = "Фізика",                      Type = LessonType.Lecture,  Requirements = "Проектор" },
                new Subjects { Name = "Програмування (C++)",          Type = LessonType.Lecture,  Requirements = null },
                new Subjects { Name = "Алгоритми та структури даних", Type = LessonType.Lecture,  Requirements = "Проектор" },
                new Subjects { Name = "Бази даних",                   Type = LessonType.Lecture,  Requirements = "Проектор" },
                new Subjects { Name = "Комп'ютерні мережі",          Type = LessonType.Lecture,  Requirements = null },
                new Subjects { Name = "Операційні системи",           Type = LessonType.Lecture,  Requirements = "Проектор" },
                new Subjects { Name = "Дискретна математика",         Type = LessonType.Lecture,  Requirements = null },
                new Subjects { Name = "Англійська мова",              Type = LessonType.Lecture,  Requirements = null },
                new Subjects { Name = "Філософія",                    Type = LessonType.Lecture,  Requirements = null },
                new Subjects { Name = "Економіка",                    Type = LessonType.Lecture,  Requirements = null },
                new Subjects { Name = "Цифрова електроніка",          Type = LessonType.Lecture,  Requirements = "Проектор" },

                new Subjects { Name = "Програмування (C++) — лаб.",   Type = LessonType.Lab,      Requirements = "Комп'ютерний клас" },
                new Subjects { Name = "Бази даних — лаб.",            Type = LessonType.Lab,      Requirements = "Комп'ютерний клас" },
                new Subjects { Name = "Комп'ютерні мережі — лаб.",   Type = LessonType.Lab,      Requirements = "Комп'ютерний клас" },
                new Subjects { Name = "ОС — лаб.",                    Type = LessonType.Lab,      Requirements = "Комп'ютерний клас" },
                new Subjects { Name = "Фізика — лаб.",                Type = LessonType.Lab,      Requirements = "Фізична лабораторія" },
                new Subjects { Name = "Цифрова електроніка — лаб.",   Type = LessonType.Lab,      Requirements = "Комп'ютерний клас" },

                new Subjects { Name = "Вища математика — практ.",     Type = LessonType.Practice, Requirements = null },
                new Subjects { Name = "Алгоритми — практ.",           Type = LessonType.Practice, Requirements = null },
                new Subjects { Name = "Дискретна математика — практ.",Type = LessonType.Practice, Requirements = null },
                new Subjects { Name = "Англійська мова — практ.",     Type = LessonType.Practice, Requirements = null },
                new Subjects { Name = "Економіка — практ.",           Type = LessonType.Practice, Requirements = null }
            );
            await context.SaveChangesAsync();
        }

        var teacherData = new[]
        {
            ("ivanov@planora.ua",    "Іванов Іван Іванович",          "ФІТ",  "Професор"),
            ("petrova@planora.ua",   "Петрова Ольга Миколаївна",      "ФІТ",  "Доцент"),
            ("sydorenko@planora.ua", "Сидоренко Андрій Петрович",     "ФІТ",  "Старший викладач"),
            ("kovalenko@planora.ua", "Коваленко Марія Олександрівна", "ФІОТ", "Доцент"),
            ("bondar@planora.ua",    "Бондар Сергій Вікторович",      "ФІОТ", "Професор"),
            ("marchenko@planora.ua", "Марченко Тетяна Ігорівна",      "ФІОТ", "Старший викладач"),
            ("moroz@planora.ua",     "Мороз Дмитро Анатолійович",     "ФЕ",   "Доцент"),
            ("shevchenko@planora.ua","Шевченко Наталія Василівна",    "ФГН",  "Доцент"),
            ("tkachuk@planora.ua",   "Ткачук Олег Григорович",        "ФІТ",  "Професор"),
            ("lysenko@planora.ua",   "Лисенко Віктор Павлович",       "ФЕ",   "Старший викладач"),
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
                    Role = UserRole.Teacher,
                    Faculty = faculty,
                    Position = position,
                    EmailConfirmed = true
                };
                var result = await userManager.CreateAsync(teacher, "Teacher123!");
                if (result.Succeeded)
                    await userManager.AddToRoleAsync(teacher, "Teacher");
            }
        }

        var studentData = new[]
        {
            ("student1@planora.ua", "Мельник Олександр Ігорович", "ФІТ",  "КН-31"),
            ("student2@planora.ua", "Козак Дарина Олексіївна",   "ФІОТ", "ІО-11"),
            ("student3@planora.ua", "Литвин Максим Андрійович",  "ФІТ",  "КН-11"),
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
                    Role = UserRole.Student,
                    Faculty = faculty,
                    GroupId = groups?.Id,
                    EmailConfirmed = true
                };
                var result = await userManager.CreateAsync(student, "Student123!");
                if (result.Succeeded)
                    await userManager.AddToRoleAsync(student, "Student");
            }
        }

        if (!context.TeachingAssignments.Any())
        {
            var teachers = context.Users.Where(u => u.Role == UserRole.Teacher).ToList();
            var subjects = context.Subjects.ToList();
            var groups   = context.Groups.ToList();

            Subjects? S(string name) => subjects.FirstOrDefault(s => s.Name == name);
            User? T(string email) => teachers.FirstOrDefault(t => t.Email == email);
            Groups? G(string name) => groups.FirstOrDefault(g => g.Name == name);

            var assignmentsToAdd = new List<TeachingAssignment>();

            void AddAssignment(string teacherEmail, string subjectName, string groupName, int hours)
            {
                var teacher = T(teacherEmail);
                var subject = S(subjectName);
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

            AddAssignment("ivanov@planora.ua", "Вища математика", "КН-11", 2);
            AddAssignment("ivanov@planora.ua", "Вища математика — практ.", "КН-11", 2);
            AddAssignment("petrova@planora.ua", "Програмування (C++)", "КН-11", 2);
            AddAssignment("petrova@planora.ua", "Програмування (C++) — лаб.", "КН-11", 2);
            AddAssignment("sydorenko@planora.ua", "Алгоритми та структури даних", "КН-21", 2);
            AddAssignment("sydorenko@planora.ua", "Алгоритми — практ.", "КН-21", 2);
            AddAssignment("kovalenko@planora.ua", "Бази даних", "КН-21", 2);
            AddAssignment("kovalenko@planora.ua", "Бази даних — лаб.", "КН-21", 2);
            AddAssignment("ivanov@planora.ua", "Вища математика", "ІО-11", 2);
            AddAssignment("ivanov@planora.ua", "Вища математика — практ.", "ІО-11", 2);
            AddAssignment("moroz@planora.ua", "Цифрова електроніка", "ІО-11", 2);
            AddAssignment("moroz@planora.ua", "Цифрова електроніка — лаб.", "ІО-11", 2);

            context.TeachingAssignments.AddRange(assignmentsToAdd);
            await context.SaveChangesAsync();
        }

        if (!context.GroupDisciplineLists.Any())
        {
            var groups   = context.Groups.ToList();
            var subjects = context.Subjects.ToList();

            Groups G(string name)   => groups.First(g => g.Name == name);
            Subjects S(string name) => subjects.First(s => s.Name == name);

            var gs = new List<GroupDisciplineList>();

            gs.AddRange(MakeGroupPlan(G("КН-11"), S, new[] {
                ("Вища математика", 2, LessonType.Lecture), ("Вища математика — практ.", 2, LessonType.Practice),
                ("Програмування (C++)", 2, LessonType.Lecture), ("Програмування (C++) — лаб.", 2, LessonType.Lab),
                ("Фізика", 2, LessonType.Lecture), ("Фізика — лаб.", 2, LessonType.Lab),
                ("Англійська мова", 2, LessonType.Lecture), ("Англійська мова — практ.", 2, LessonType.Practice)
            }));

            gs.AddRange(MakeGroupPlan(G("КН-12"), S, new[] {
                ("Вища математика", 2, LessonType.Lecture), ("Вища математика — практ.", 2, LessonType.Practice),
                ("Програмування (C++)", 2, LessonType.Lecture), ("Програмування (C++) — лаб.", 2, LessonType.Lab),
                ("Фізика", 2, LessonType.Lecture), ("Фізика — лаб.", 2, LessonType.Lab),
                ("Філософія", 2, LessonType.Lecture)
            }));

            gs.AddRange(MakeGroupPlan(G("КН-21"), S, new[] {
                ("Алгоритми та структури даних", 2, LessonType.Lecture), ("Алгоритми — практ.", 2, LessonType.Practice),
                ("Бази даних", 2, LessonType.Lecture), ("Бази даних — лаб.", 2, LessonType.Lab),
                ("Дискретна математика", 2, LessonType.Lecture), ("Дискретна математика — практ.", 2, LessonType.Practice),
                ("Економіка", 2, LessonType.Lecture)
            }));

            gs.AddRange(MakeGroupPlan(G("КН-22"), S, new[] {
                ("Алгоритми та структури даних", 2, LessonType.Lecture), ("Алгоритми — практ.", 2, LessonType.Practice),
                ("Бази даних", 2, LessonType.Lecture), ("Бази даних — лаб.", 2, LessonType.Lab),
                ("Англійська мова", 2, LessonType.Lecture), ("Англійська мова — практ.", 2, LessonType.Practice)
            }));

            gs.AddRange(MakeGroupPlan(G("КН-31"), S, new[] {
                ("Комп'ютерні мережі", 2, LessonType.Lecture), ("Комп'ютерні мережі — лаб.", 2, LessonType.Lab),
                ("Операційні системи", 2, LessonType.Lecture), ("ОС — лаб.", 2, LessonType.Lab),
                ("Економіка", 2, LessonType.Lecture), ("Економіка — практ.", 2, LessonType.Practice)
            }));

            gs.AddRange(MakeGroupPlan(G("КН-32"), S, new[] {
                ("Комп'ютерні мережі", 2, LessonType.Lecture), ("Комп'ютерні мережі — лаб.", 2, LessonType.Lab),
                ("Операційні системи", 2, LessonType.Lecture), ("ОС — лаб.", 2, LessonType.Lab),
                ("Філософія", 2, LessonType.Lecture)
            }));

            gs.AddRange(MakeGroupPlan(G("ІО-11"), S, new[] {
                ("Вища математика", 2, LessonType.Lecture), ("Вища математика — практ.", 2, LessonType.Practice),
                ("Програмування (C++)", 2, LessonType.Lecture), ("Програмування (C++) — лаб.", 2, LessonType.Lab),
                ("Цифрова електроніка", 2, LessonType.Lecture), ("Цифрова електроніка — лаб.", 2, LessonType.Lab)
            }));

            gs.AddRange(MakeGroupPlan(G("ІО-12"), S, new[] {
                ("Вища математика", 2, LessonType.Lecture), ("Вища математика — практ.", 2, LessonType.Practice),
                ("Програмування (C++)", 2, LessonType.Lecture), ("Програмування (C++) — лаб.", 2, LessonType.Lab),
                ("Англійська мова", 2, LessonType.Lecture), ("Англійська мова — практ.", 2, LessonType.Practice)
            }));

            gs.AddRange(MakeGroupPlan(G("ІО-21"), S, new[] {
                ("Бази даних", 2, LessonType.Lecture), ("Бази даних — лаб.", 2, LessonType.Lab),
                ("Комп'ютерні мережі", 2, LessonType.Lecture), ("Комп'ютерні мережі — лаб.", 2, LessonType.Lab),
                ("Дискретна математика", 2, LessonType.Lecture), ("Дискретна математика — практ.", 2, LessonType.Practice)
            }));

            gs.AddRange(MakeGroupPlan(G("ІО-22"), S, new[] {
                ("Операційні системи", 2, LessonType.Lecture), ("ОС — лаб.", 2, LessonType.Lab),
                ("Алгоритми та структури даних", 2, LessonType.Lecture), ("Алгоритми — практ.", 2, LessonType.Practice),
                ("Економіка", 2, LessonType.Lecture), ("Економіка — практ.", 2, LessonType.Practice)
            }));

            gs.AddRange(MakeGroupPlan(G("ЕК-11"), S, new[] {
                ("Фізика", 2, LessonType.Lecture), ("Фізика — лаб.", 2, LessonType.Lab),
                ("Цифрова електроніка", 2, LessonType.Lecture), ("Цифрова електроніка — лаб.", 2, LessonType.Lab),
                ("Вища математика", 2, LessonType.Lecture), ("Вища математика — практ.", 2, LessonType.Practice)
            }));

            gs.AddRange(MakeGroupPlan(G("ЕК-21"), S, new[] {
                ("Фізика", 2, LessonType.Lecture), ("Фізика — лаб.", 2, LessonType.Lab),
                ("Комп'ютерні мережі", 2, LessonType.Lecture), ("Комп'ютерні мережі — лаб.", 2, LessonType.Lab),
                ("Економіка", 2, LessonType.Lecture), ("Економіка — практ.", 2, LessonType.Practice)
            }));

            context.GroupDisciplineLists.AddRange(gs);
            await context.SaveChangesAsync();
        }
    }

    private static List<GroupDisciplineList> MakeGroupPlan(
        Groups groups,
        Func<string, Subjects> subjectLookup,
        (string SubjectName, int Hours, LessonType Type)[] plan)
    {
        return plan.Select(p => new GroupDisciplineList
        {
            GroupId = groups.Id,
            SubjectId = subjectLookup(p.SubjectName).Id,
            HoursPerWeek = p.Hours,
            LessonType = p.Type
        }).ToList();
    }
}
