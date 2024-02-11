﻿using PrimarySchool;

var applicationContext = new ApplicationDbContext();

applicationContext.Students.Add(new Student {
    Id = 1,
    DisplayName = "Joe"
});

applicationContext.SaveChanges();

var joe = applicationContext.Students.Find(1);
Console.WriteLine($"Joe was registered on: {joe.RegistrationDate}");
