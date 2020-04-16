using LMS.Models.LMSModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LMS.Controllers
{
    [Authorize(Roles = "Student")]
    public class StudentController : CommonController
    {

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Catalog()
        {
            return View();
        }

        public IActionResult Class(string subject, string num, string season, string year)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            return View();
        }

        public IActionResult Assignment(string subject, string num, string season, string year, string cat, string aname)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            ViewData["cat"] = cat;
            ViewData["aname"] = aname;
            return View();
        }


        public IActionResult ClassListings(string subject, string num)
        {
            System.Diagnostics.Debug.WriteLine(subject + num);
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            return View();
        }


        /*******Begin code to modify********/

        /// <summary>
        /// Returns a JSON array of the classes the given student is enrolled in.
        /// Each object in the array should have the following fields:
        /// "subject" - The subject abbreviation of the class (such as "CS")
        /// "number" - The course number (such as 5530)
        /// "name" - The course name
        /// "season" - The season part of the semester
        /// "year" - The year part of the semester
        /// "grade" - The grade earned in the class, or "--" if one hasn't been assigned
        /// </summary>
        /// <param name="uid">The uid of the student</param>
        /// <returns>The JSON array</returns>
        public IActionResult GetMyClasses(string uid)
        {
            var query = from s in db.Students
                        join e in db.Enrolled
                        on s.UId equals e.UId
                        join c in db.Classes
                        on e.ClassId equals c.ClassId
                        join co in db.Courses
                        on c.CourseId equals co.CourseId
                        where s.UId == uid
                        select new
                        {
                            subject = co.Subject,
                            number = co.Num,
                            name = co.Name,
                            season = c.Season,
                            year = c.Year,
                            grade = e.Grade
                        };
            return Json(query.ToArray());
        }

        /// <summary>
        /// Returns a JSON array of all the assignments in the given class that the given student is enrolled in.
        /// Each object in the array should have the following fields:
        /// "aname" - The assignment name
        /// "cname" - The category name that the assignment belongs to
        /// "due" - The due Date/Time
        /// "score" - The score earned by the student, or null if the student has not submitted to this assignment.
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="uid"></param>
        /// <returns>The JSON array</returns>
        public IActionResult GetAssignmentsInClass(string subject, int num, string season, int year, string uid)
        {
            var query = from co in db.Courses
                        join c in db.Classes
                        on co.CourseId equals c.CourseId
                        join ac in db.AssignmentCat
                        on c.ClassId equals ac.ClassId
                        join a in db.Assignments
                        on ac.CatId equals a.CatId
                        join s in db.Submission.Where(m => m.UId == uid)
                        on a.AssId equals s.AssId
                        into join1
                        from j1 in join1.DefaultIfEmpty()
                        where co.Subject == subject && co.Num == num.ToString() && c.Season == season && c.Year == year
                        select new
                        {
                            aname = a.Name,
                            cname = ac.Name,
                            due = a.Due,
                            score = j1 == null ? null : (uint?)j1.Score
                        };
            return Json(query.ToArray());
        }



        /// <summary>
        /// Adds a submission to the given assignment for the given student
        /// The submission should use the current time as its DateTime
        /// You can get the current time with DateTime.Now
        /// The score of the submission should start as 0 until a Professor grades it
        /// If a Student submits to an assignment again, it should replace the submission contents
        /// and the submission time (the score should remain the same).
        /// Does *not* automatically reject late submissions.
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class</param>
        /// <param name="asgname">The new assignment name</param>
        /// <param name="uid">The student submitting the assignment</param>
        /// <param name="contents">The text contents of the student's submission</param>
        /// <returns>A JSON object containing {success = true/false}.</returns>
        public IActionResult SubmitAssignmentText(string subject, int num, string season, int year,
          string category, string asgname, string uid, string contents)
        {
            try
            {
                uint assId = (from co in db.Courses
                              join c in db.Classes
                              on co.CourseId equals c.CourseId
                              join ac in db.AssignmentCat
                              on c.ClassId equals ac.ClassId
                              join a in db.Assignments
                              on ac.CatId equals a.CatId
                              where co.Subject == subject && co.Num == num.ToString() && c.Season == season && c.Year == year
                              && ac.Name == category && a.Name == asgname
                              select a.AssId).First();
                Submission submission = (from s in db.Submission
                                         where s.AssId == assId && s.UId == uid
                                         select s).FirstOrDefault();
                if (submission == null)
                {
                    Submission newSubmission = new Submission()
                    {
                        AssId = assId,
                        UId = uid,
                        Score = 0,
                        Contents = contents,
                        Time = DateTime.Now
                    };
                    db.Submission.Add(newSubmission);
                }
                else
                {
                    submission.Contents = contents;
                    submission.Time = DateTime.Now;
                }
                db.SaveChanges();
            }
            catch
            {
                return Json(new { success = false });
            }
            return Json(new { success = true });
        }


        /// <summary>
        /// Enrolls a student in a class.
        /// </summary>
        /// <param name="subject">The department subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester</param>
        /// <param name="year">The year part of the semester</param>
        /// <param name="uid">The uid of the student</param>
        /// <returns>A JSON object containing {success = {true/false},
        /// false if the student is already enrolled in the Class.</returns>
        public IActionResult Enroll(string subject, int num, string season, int year, string uid)
        {
            try
            {
                uint classId = (from c in db.Classes
                                join co in db.Courses
                                on c.CourseId equals co.CourseId
                                where co.Subject == subject && co.Num == num.ToString() && c.Season == season && c.Year == year
                                select c.ClassId).First();
                Enrolled newE = new Enrolled()
                {
                    UId = uid,
                    ClassId = classId,
                    Grade = "--"
                };
                db.Enrolled.Add(newE);
                db.SaveChanges();
            }
            catch
            {
                return Json(new { success = false });

            }
            return Json(new { success = true });
        }



        /// <summary>
        /// Calculates a student's GPA
        /// A student's GPA is determined by the grade-point representation of the average grade in all their classes.
        /// Assume all classes are 4 credit hours.
        /// If a student does not have a grade in a class ("--"), that class is not counted in the average.
        /// If a student does not have any grades, they have a GPA of 0.0.
        /// Otherwise, the point-value of a letter grade is determined by the table on this page:
        /// https://advising.utah.edu/academic-standards/gpa-calculator-new.php
        /// </summary>
        /// <param name="uid">The uid of the student</param>
        /// <returns>A JSON object containing a single field called "gpa" with the number value</returns>
        public IActionResult GetGPA(string uid)
        {
            var query = from e in db.Enrolled
                        where e.UId == uid
                        select e.Grade;
            double totalGradePoints = 0;
            int totalHours = 0;
            foreach (string grade in query)
            {
                if (!grade.Equals("--"))
                {
                    double gradePoint = 0;
                    switch (grade)
                    {
                        case "A":
                            gradePoint = 4.0;
                            break;
                        case "A-":
                            gradePoint = 3.7;
                            break;
                        case "B+":
                            gradePoint = 3.3;
                            break;
                        case "B":
                            gradePoint = 3.0;
                            break;
                        case "B-":
                            gradePoint = 2.7;
                            break;
                        case "C+":
                            gradePoint = 2.3;
                            break;
                        case "C":
                            gradePoint = 2.0;
                            break;
                        case "C-":
                            gradePoint = 1.7;
                            break;
                        case "D+":
                            gradePoint = 1.3;
                            break;
                        case "D":
                            gradePoint = 1.0;
                            break;
                        case "D-":
                            gradePoint = 0.7;
                            break;
                        case "E":
                            gradePoint = 0.0;
                            break;
                    }
                    totalGradePoints += 4 * gradePoint;
                    totalHours += 4;
                }
            }
            double gpa = 0;
            if (totalHours != 0)
            {
                gpa = totalGradePoints / totalHours;
            }
            return Json(new { gpa = gpa });
        }

        /*******End code to modify********/

    }
}