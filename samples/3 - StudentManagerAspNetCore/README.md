# Hello world
A sample application showing triggers integrated with ASP.NET Core Razor pages

## Triggers

- Courses.AutoSignupStudents: This trigger ensures that whenever a new mandatory course gets added- or an existing course gets marked as mandatory, that all students will be signed up for that course.
- StudentCourses.BlockRemovalWhenCourseIsMandatory: This trigger rejects the deletion of courses when there are still students signed up for that course (example of a manual foreign key through triggers)
- StudentCourses.SendWelcomingEmail: This trigger will send an email to the student whenever he gets signed up for a course.
- Students.AssigRegistrationDate: This trigger assigns a registration date to newly added users
- Students.SignupToMandatoryCourses: This trigger ensures that whenever a new students gets added to the database, the same student will be signed up for all mandatory courses
- Traits.Audited.CreateAuditRecord: This trigger will store a snapshot of the changes made to a record on an Add/Update/Removal of any entity that implements the IAudited trait interface
- Traits.SoftDelete.EnsureSoftDelete: This trigger will catch any attempt at deleting a entity that imlpements the ISoftDelete trait interface and instead update that entity and set its DeletedOn property to the current DateTime

## Build and run this sample
Run `dotnet run` in the root of this project 
