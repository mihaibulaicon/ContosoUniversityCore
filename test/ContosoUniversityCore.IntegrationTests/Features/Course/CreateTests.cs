﻿namespace ContosoUniversityCore.IntegrationTests.Features.Course
{
    using System;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using ContosoUniversityCore.Features.Course;
    using Domain;
    using Shouldly;

    public class CreateTests
    {
        public async Task Should_create_new_course(ContainerFixture fixture)
        {
            var admin = new Instructor
            {
                FirstMidName = "George",
                LastName = "Costanza",
                HireDate = DateTime.Today,
            };
            await fixture.ExecuteDbContextAsync(async db =>
            {
                db.Instructors.Add(admin);
                await db.SaveChangesAsync();
            });

            var dept = new Department
            {
                Name = "History",
                Administrator = admin,
                Budget = 123m,
                StartDate = DateTime.Today
            };

            await fixture.ExecuteDbContextAsync(async db =>
            {
                db.Departments.Add(dept);
                await db.SaveChangesAsync();
            });
            var command = new Create.Command
            {
                Credits = 4,
                Department = dept,
                Number = 1234,
                Title = "English 101"
            };
            await fixture.SendAsync(command);

            await fixture.ExecuteDbContextAsync(async db =>
            {
                var created = await db.Courses.Where(c => c.CourseID == command.Number).SingleOrDefaultAsync();

                created.ShouldNotBeNull();
                created.DepartmentID.ShouldBe(dept.DepartmentID);
                created.Credits.ShouldBe(command.Credits);
                created.Title.ShouldBe(command.Title);
            });
        }
    }
}