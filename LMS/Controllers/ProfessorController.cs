﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LMS.Models.LMSModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LMS.Controllers
{
    [Authorize(Roles = "Professor")]
    public class ProfessorController : CommonController
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Students(string subject, string num, string season, string year)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
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

        public IActionResult Categories(string subject, string num, string season, string year)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            return View();
        }

        public IActionResult CatAssignments(string subject, string num, string season, string year, string cat)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            ViewData["cat"] = cat;
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

        public IActionResult Submissions(string subject, string num, string season, string year, string cat, string aname)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            ViewData["cat"] = cat;
            ViewData["aname"] = aname;
            return View();
        }

        public IActionResult Grade(string subject, string num, string season, string year, string cat, string aname, string uid)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            ViewData["cat"] = cat;
            ViewData["aname"] = aname;
            ViewData["uid"] = uid;
            return View();
        }

        /*******Begin code to modify********/


        /// <summary>
        /// Returns a JSON array of all the students in a class.
        /// Each object in the array should have the following fields:
        /// "fname" - first name
        /// "lname" - last name
        /// "uid" - user ID
        /// "dob" - date of birth
        /// "grade" - the student's grade in this class
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <returns>The JSON array</returns>
        public IActionResult GetStudentsInClass(string subject, int num, string season, int year)
        {
            var query = from s in db.Students
                        join e in db.Enrolled
                        on s.UId equals e.UId
                        join c in db.Classes
                        on e.ClassId equals c.ClassId
                        join co in db.Courses
                        on c.CourseId equals co.CourseId
                        join d in db.Departments
                        on co.Subject equals d.Subject
                        where d.Subject == subject && co.Num == num.ToString() && c.Season == season && c.Year == year
                        select new
                        {
                            fname = s.FName,
                            lname = s.LName,
                            uid = s.UId,
                            dob = s.Dob,
                            grade = e.Grade
                        };
            return Json(query.ToArray());
        }



        /// <summary>
        /// Returns a JSON array with all the assignments in an assignment category for a class.
        /// If the "category" parameter is null, return all assignments in the class.
        /// Each object in the array should have the following fields:
        /// "aname" - The assignment name
        /// "cname" - The assignment category name.
        /// "due" - The due DateTime
        /// "submissions" - The number of submissions to the assignment
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class, 
        /// or null to return assignments from all categories</param>
        /// <returns>The JSON array</returns>
        public IActionResult GetAssignmentsInCategory(string subject, int num, string season, int year, string category)
        {
            var query = from d in db.Departments
                        join co in db.Courses
                        on d.Subject equals co.Subject
                        join c in db.Classes
                        on co.CourseId equals c.CourseId
                        join ac in db.AssignmentCat
                        on c.ClassId equals ac.ClassId
                        join a in db.Assignments
                        on ac.CatId equals a.CatId
                        into join1
                        from j1 in join1
                        where d.Subject == subject && co.Num == num.ToString() && c.Season == season && c.Year == year
                        && (category == null || (category != null && ac.Name == category))
                        select new
                        {
                            aname = j1.Name,
                            cname = ac.Name,
                            due = j1.Due,
                            submissions = (from j1 in join1
                                           join s in db.Submission
                                           on j1.AssId equals s.AssId
                                           select s).Count()
                        };
            return Json(query.ToArray());
        }


        /// <summary>
        /// Returns a JSON array of the assignment categories for a certain class.
        /// Each object in the array should have the folling fields:
        /// "name" - The category name
        /// "weight" - The category weight
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class</param>
        /// <returns>The JSON array</returns>
        public IActionResult GetAssignmentCategories(string subject, int num, string season, int year)
        {
            var query = from d in db.Departments
                        join co in db.Courses
                        on d.Subject equals co.Subject
                        join c in db.Classes
                        on co.CourseId equals c.CourseId
                        join ac in db.AssignmentCat
                        on c.ClassId equals ac.ClassId
                        where d.Subject == subject && co.Num == num.ToString() && c.Season == season && c.Year == year
                        select new
                        {
                            name = ac.Name,
                            weight = ac.Weight
                        };
            return Json(query.ToArray());
        }

        /// <summary>
        /// Creates a new assignment category for the specified class.
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The new category name</param>
        /// <param name="catweight">The new category weight</param>
        /// <returns>A JSON object containing {success = true/false},
        ///	false if an assignment category with the same name already exists in the same class.</returns>
        public IActionResult CreateAssignmentCategory(string subject, int num, string season, int year, string category, int catweight)
        {
            try
            {
                uint classId = (from d in db.Departments
                                join co in db.Courses
                                on d.Subject equals co.Subject
                                join c in db.Classes
                                on co.CourseId equals c.CourseId
                                where d.Subject == subject && co.Num == num.ToString() && c.Season == season && c.Year == year
                                select c.ClassId).First();
                AssignmentCat newCat = new AssignmentCat()
                {
                    Name = category,
                    ClassId = classId,
                    Weight = (uint)catweight
                };
                db.AssignmentCat.Add(newCat);
                db.SaveChanges();
            }
            catch
            {
                return Json(new { success = false });
            }
            return Json(new { success = true });
        }

        /// <summary>
        /// Creates a new assignment for the given class and category.
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class</param>
        /// <param name="asgname">The new assignment name</param>
        /// <param name="asgpoints">The max point value for the new assignment</param>
        /// <param name="asgdue">The due DateTime for the new assignment</param>
        /// <param name="asgcontents">The contents of the new assignment</param>
        /// <returns>A JSON object containing success = true/false,
        /// false if an assignment with the same name already exists in the same assignment category.</returns>
        public IActionResult CreateAssignment(string subject, int num, string season, int year, string category, string asgname, int asgpoints, DateTime asgdue, string asgcontents)
        {
            try
            {
                uint classId = (from co in db.Courses
                                join c in db.Classes
                                on co.CourseId equals c.CourseId
                                where co.Subject == subject && co.Num == num.ToString() && c.Season == season && c.Year == year
                                select c.ClassId).First();
                uint catId = (from ac in db.AssignmentCat
                              where ac.Name == category && ac.ClassId == classId
                              select ac.CatId).First();
                Assignments newAsg = new Assignments()
                {
                    Name = asgname,
                    CatId = catId,
                    Contents = asgcontents,
                    Due = asgdue,
                    Points = (uint)asgpoints
                };
                db.Assignments.Add(newAsg);
                db.SaveChanges();
                var students = from e in db.Enrolled
                               where e.ClassId == classId
                               select e;
                foreach (var student in students)
                {
                    UpdateGrade(student.UId, classId);
                }
            }
            catch
            {
                return Json(new { success = false });
            }
            return Json(new { success = true });
        }


        /// <summary>
        /// Gets a JSON array of all the submissions to a certain assignment.
        /// Each object in the array should have the following fields:
        /// "fname" - first name
        /// "lname" - last name
        /// "uid" - user ID
        /// "time" - DateTime of the submission
        /// "score" - The score given to the submission
        /// 
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class</param>
        /// <param name="asgname">The name of the assignment</param>
        /// <returns>The JSON array</returns>
        public IActionResult GetSubmissionsToAssignment(string subject, int num, string season, int year, string category, string asgname)
        {
            var query = from co in db.Courses
                        join c in db.Classes
                        on co.CourseId equals c.CourseId
                        join ac in db.AssignmentCat
                        on c.ClassId equals ac.ClassId
                        join a in db.Assignments
                        on ac.CatId equals a.CatId
                        join s in db.Submission
                        on a.AssId equals s.AssId
                        join st in db.Students
                        on s.UId equals st.UId
                        where co.Subject == subject && co.Num == num.ToString() && c.Year == year && c.Season == season
                        && ac.Name == category && a.Name == asgname
                        select new
                        {
                            fname = st.FName,
                            lname = st.LName,
                            uid = st.UId,
                            time = s.Time,
                            score = s.Score
                        };
            return Json(query.ToArray());
        }


        /// <summary>
        /// Set the score of an assignment submission
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class</param>
        /// <param name="asgname">The name of the assignment</param>
        /// <param name="uid">The uid of the student who's submission is being graded</param>
        /// <param name="score">The new score for the submission</param>
        /// <returns>A JSON object containing success = true/false</returns>
        public IActionResult GradeSubmission(string subject, int num, string season, int year, string category, string asgname, string uid, int score)
        {
            try
            {
                uint classId = (from co in db.Courses
                                join c in db.Classes
                                on co.CourseId equals c.CourseId
                                where co.Subject == subject && co.Num == num.ToString() && c.Season == season && c.Year == year
                                select c.ClassId).First();
                Submission submission = (from ac in db.AssignmentCat
                                         join a in db.Assignments
                                         on ac.CatId equals a.CatId
                                         join s in db.Submission
                                         on a.AssId equals s.AssId
                                         where ac.ClassId == classId && ac.Name == category && a.Name == asgname && s.UId == uid
                                         select s).First();
                submission.Score = (uint)score;
                db.SaveChanges();
                UpdateGrade(uid, classId);
            }
            catch
            {
                return Json(new { success = false });
            }
            return Json(new { success = true });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uid">The uid of the student</param>
        private void UpdateGrade(string uId, uint classId)
        {
            var query = from ac in db.AssignmentCat.Where(m => m.ClassId == classId)
                        join a in db.Assignments
                        on ac.CatId equals a.CatId into join1
                        from j1 in join1.DefaultIfEmpty()
                        join s in db.Submission.Where(n => n.UId == uId)
                        on j1.AssId equals s.AssId into join2
                        from j2 in join2.DefaultIfEmpty()
                        orderby ac.CatId
                        select new
                        {
                            categoryId = ac.CatId,
                            categoryWeight = ac.Weight,
                            assignmentPoints = j1 == null ? null : (uint?)j1.Points,
                            submissionScore = j2 == null ? null : (uint?)j2.Score
                        };
            double total = 0;
            uint weightTotal = 0;
            uint currentId = query.First().categoryId;
            uint currentWeight = query.First().categoryWeight;
            uint totalMaxPoints = 0;
            uint totalScoreEarned = 0;
            foreach (var o in query)
            {
                if (o.assignmentPoints == null)
                {
                    continue;
                }
                if (currentId != o.categoryId)
                {
                    if (totalMaxPoints != 0)
                    {
                        double categoryPercentage = (double)totalScoreEarned / (double)totalMaxPoints;
                        double categoryScaledTotal = categoryPercentage * (double)currentWeight;
                        weightTotal += currentWeight;
                        total += categoryScaledTotal;
                    }

                    currentId = o.categoryId;
                    currentWeight = o.categoryWeight;
                    totalMaxPoints = 0;
                    totalScoreEarned = 0;
                }
                totalMaxPoints += (uint)o.assignmentPoints;
                if (o.submissionScore == null)
                {
                    totalScoreEarned += 0;
                }
                else
                {
                    totalScoreEarned += (uint)o.submissionScore;
                }
            }
            if (totalMaxPoints != 0)
            {
                double categoryPercentage = (double)totalScoreEarned / (double)totalMaxPoints;
                double categoryScaledTotal = categoryPercentage * (double)currentWeight;
                weightTotal += currentWeight;
                total += categoryScaledTotal;
            }
            double scalingFactor = 100.0 / (double)weightTotal;
            total *= scalingFactor;
            Enrolled enrolled = (from e in db.Enrolled
                                 where e.UId == uId && e.ClassId == classId
                                 select e).First();
            if (total >= 93)
            {
                enrolled.Grade = "A";
            }
            else if (total >= 90)
            {
                enrolled.Grade = "A-";
            }
            else if (total >= 87)
            {
                enrolled.Grade = "B+";
            }
            else if (total >= 83)
            {
                enrolled.Grade = "B";
            }
            else if (total >= 80)
            {
                enrolled.Grade = "B-";
            }
            else if (total >= 77)
            {
                enrolled.Grade = "C+";
            }
            else if (total >= 73)
            {
                enrolled.Grade = "C";
            }
            else if (total >= 70)
            {
                enrolled.Grade = "C-";
            }
            else if (total >= 67)
            {
                enrolled.Grade = "D+";
            }
            else if (total >= 63)
            {
                enrolled.Grade = "D";
            }
            else if (total >= 60)
            {
                enrolled.Grade = "D-";
            }
            else
            {
                enrolled.Grade = "E";
            }
            db.SaveChanges();
        }


        /// <summary>
        /// Returns a JSON array of the classes taught by the specified professor
        /// Each object in the array should have the following fields:
        /// "subject" - The subject abbreviation of the class (such as "CS")
        /// "number" - The course number (such as 5530)
        /// "name" - The course name
        /// "season" - The season part of the semester in which the class is taught
        /// "year" - The year part of the semester in which the class is taught
        /// </summary>
        /// <param name="uid">The professor's uid</param>
        /// <returns>The JSON array</returns>
        public IActionResult GetMyClasses(string uid)
        {
            var query = from c in db.Classes
                        join co in db.Courses
                        on c.CourseId equals co.CourseId
                        join d in db.Departments
                        on co.Subject equals d.Subject
                        where c.UId == uid
                        select new
                        {
                            subject = d.Subject,
                            number = co.Num,
                            name = co.Name,
                            season = c.Season,
                            year = c.Year
                        };
            return Json(query.ToArray());
        }


        /*******End code to modify********/

    }
}