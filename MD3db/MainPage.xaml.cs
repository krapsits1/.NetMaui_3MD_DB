using MD3db;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using System.Text.Json;

namespace MD3db
{
    public partial class MainPage : ContentPage
    {
        //Lapas inicializācija
        public MainPage()
        {
            try
            {
                InitializeComponent();
                LoadData().ConfigureAwait(false);

            }
            catch (Exception ex)
            {
                DisplayAlert("Error", $"An error occurred while initializing the page: {ex.Message}", "OK");
            }
        }
        //Datu ielādē no funckijām
        private async Task LoadData()
        {
            try
            {
                await LoadTeachers();
                await LoadStudents();
                await LoadCourses();
                await LoadAssignments();
                await LoadSubmissions();
                await LoadSubmissionPickers();


            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"An error occurred while initializing the page: {ex.Message}", "OK");
            }
        }

        //Funckijas,kas aktivizē, testa datu izstrādi
        private async void OnCreateTestDataClicked(object sender, EventArgs e)
        {
            try
            {
                await CreateTestData();
                await DisplayAlert("Success", "Test data created successfully.", "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"An error occurred while creating test data: {ex.Message}", "OK");
            }
        }

        // Metode, kas izstrādā testa datus
        private async Task CreateTestData()
        {
            try
            {
                var db = await DatabaseService.GetDatabaseAsync();

                var teacher = new Teacher { Name = "Inta", Surname = "Busule", Gender = "Female", ContractDate = DateTime.Now };
                await db.InsertAsync(teacher);

                var student = new Student { Name = "Emīls", Surname = "Vētra", Gender = "Male", StudentIdNumber = "ev23058" };
                await db.InsertAsync(student);

                var course = new Course { Name = ".Net", TeacherId = teacher.Id };
                await db.InsertAsync(course);

                var assignment = new Assignment { DeadLine = (DateTime.Now.AddDays(7)), Description = "Homework 3 DB", CourseId = course.Id };
                await db.InsertAsync(assignment);

                var submission = new Submission { SubmissionTime = DateTime.Now, Score = 95.5, AssignmentId = assignment.Id, StudentId = student.Id };
                await db.InsertAsync(submission);

                await LoadData();

            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"An error occurred while creating test data: {ex.Message}", "OK");
                throw;
            }
        }



        // Metode, kas ielādē Teachers datus
        private async Task LoadTeachers()
        {
            try
            {
                var db = await DatabaseService.GetDatabaseAsync();
                var teachers = await db.Table<Teacher>().ToListAsync();

                if (teachers.Count > 0)
                {
                    TeachersCollectionView.ItemsSource = teachers;
                }
                else
                {
                    await DisplayAlert("No Data", "No teachers found.", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"An error occurred while loading teachers: {ex.Message}", "OK");
            }
        }

        //Metodes, kas ielādē Students datus
        private async Task LoadStudents()
        {
            try
            {
                var db = await DatabaseService.GetDatabaseAsync();
                var students = await db.Table<Student>().ToListAsync();

                if (students.Count > 0)
                {
                    StudentsCollectionView.ItemsSource = students;
                }
                else
                {
                    await DisplayAlert("No Data", "No students found.", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"An error occurred while loading students: {ex.Message}", "OK");
            }
        }

        //Metode, kas ļauj izveidot jaunu studentu
        private async void OnCreateStudentClicked(object sender, EventArgs e)
        {
            //Dabū ievades laukus no UI elementiem
            string name = StudentNameEntry.Text;
            string surname = StudentSurnameEntry.Text;
            string gender = StudentGenderPicker.SelectedItem?.ToString();
            string studentIdNumber = StudentIdEntry.Text;

            //Validējam ievades laukus
            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(surname) || string.IsNullOrWhiteSpace(gender) || string.IsNullOrWhiteSpace(studentIdNumber))
            {
                await DisplayAlert("Error", "Please fill in all fields.", "OK");
                return;
            }

            // Izveido jaunu studentu
            var newStudent = new Student
            {
                Name = name,
                Surname = surname,
                Gender = gender,
                StudentIdNumber = studentIdNumber
            };

            // Saglabā jauno studentu datubāzē
            var db = await DatabaseService.GetDatabaseAsync();
            await db.InsertAsync(newStudent);

            // Atjauno studentu sarakstu
            await LoadStudents();

            // Notīram ievades laukus
            StudentNameEntry.Text = string.Empty;
            StudentSurnameEntry.Text = string.Empty;
            StudentGenderPicker.SelectedIndex = -1;
            StudentIdEntry.Text = string.Empty;
            await LoadSubmissionPickers();

            await DisplayAlert("Success", "Student created successfully.", "OK");
        }

        //metode, kas ielādē kursus
        private async Task LoadCourses()
        {
            try
            {


                var db = await DatabaseService.GetDatabaseAsync();


                var courses = await db.Table<Course>().ToListAsync();
                var teachers = await db.Table<Teacher>().ToListAsync();

                CoursePicker.ItemsSource = courses;
                CoursePicker.ItemDisplayBinding = new Binding("Name");

 
                if (teachers.Count == 0)
                {
                    await DisplayAlert("No Data", "No teachers found.", "OK");
                    return;
                }


                if (courses.Count == 0)
                {
                    await DisplayAlert("No Data", "No courses found.", "OK");
                    return;
                }

                // Sajoino kursus ar skolotājiem, lai iegūtu kursu nosaukumus un skolotāju vārdus
                var courseTeacherData = from course in courses
                                        join teacher in teachers on course.TeacherId equals teacher.Id
                                        select new
                                        {
                                            CourseName = course.Name,
                                            TeacherName = teacher.Name
                                        };

                // ieliek kursus CollectionView
                CoursesListView.ItemsSource = courseTeacherData.ToList();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"An error occurred while loading courses: {ex.Message}", "OK");
            }
        }

        //Metode, kas ielādē assignments
        private async Task LoadAssignments()
        {
            try
            {
                var db = await DatabaseService.GetDatabaseAsync();

                var assignments = await db.Table<Assignment>().ToListAsync();


                var courses = await db.Table<Course>().ToListAsync();


                var assignmentsWithCourseNames = assignments.Select(a =>
                {
                    a.CourseName = courses.FirstOrDefault(c => c.Id == a.CourseId)?.Name;
                    return a;
                }).ToList();

                AssignmentsListView.ItemsSource = assignmentsWithCourseNames;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"An error occurred while loading assignments: {ex.Message}", "OK");
            }
        }

        //metode, kas dabūt kursus sinhroni
        private async Task<List<Course>> GetCoursesAsync()
        {
            var db = await DatabaseService.GetDatabaseAsync();
            return await db.Table<Course>().ToListAsync();
        }

        //metode, kas ļauj izveidot jaunu Assignment
        private async void OnCreateAssignmentClicked(object sender, EventArgs e)
        {
            //Dabū datus no UI elementiem
            string description = AssignmentDescriptionEntry.Text;
            DateTime? deadline = AssignmentDeadlinePicker.Date;
            var selectedCourse = CoursePicker.SelectedItem as Course;

            //validē ievades laukus
            if (string.IsNullOrWhiteSpace(description) || !deadline.HasValue || selectedCourse == null)
            {
                await DisplayAlert("Error", "Please fill in all fields.", "OK");
                return;
            }

            // Izveido jaunu assignment
            var newAssignment = new Assignment
            {
                Description = description,
                DeadLine = deadline.Value,
                CourseId = selectedCourse.Id,
                CourseName = selectedCourse.Name
            };

            // saglabā jauno assignment datubāzē
            var db = await DatabaseService.GetDatabaseAsync();
            await db.InsertAsync(newAssignment);

            //atjuano assignment sarakstu
            await LoadAssignments();

            // attīra ievades laukus
            AssignmentDescriptionEntry.Text = string.Empty;
            AssignmentDeadlinePicker.Date = DateTime.Now;
            CoursePicker.SelectedIndex = -1;
            await LoadSubmissionPickers();
            await DisplayAlert("Success", "Assignment created successfully.", "OK");
        }



        //Metode, kas ļauj mainīts assignment datus
        private async void OnEditAssignmentClicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            var assignment = button?.CommandParameter as Assignment;
            if (assignment != null)
            {
                //izveido dialogu, kurā varēs mainīt assignment datus
                var descriptionEntry = new Entry { Text = assignment.Description, Placeholder = "Description", Margin = new Thickness(10) };
                var deadlineEntry = new DatePicker { Date = assignment.DeadLine, Margin = new Thickness(10) };
                var coursePicker = new Picker { Title = "Select Course", Margin = new Thickness(10) };

                
                var db = await DatabaseService.GetDatabaseAsync();
                var courses = await db.Table<Course>().ToListAsync();
                foreach (var course in courses)
                {
                    coursePicker.Items.Add(course.Name);
                }
                coursePicker.SelectedItem = courses.FirstOrDefault(c => c.Id == assignment.CourseId)?.Name;

                // Izveido stack layout, kurā ievieto ievades laukus un pogas
                var stackLayout = new StackLayout
                {
                    Children = { descriptionEntry, deadlineEntry, coursePicker }
                };

                var modalPage = new ContentPage
                {
                    Content = new StackLayout
                    {
                        Children =
                {
                    new Label { Text = "Edit Assignment", FontAttributes = FontAttributes.Bold, HorizontalOptions = LayoutOptions.Center },
                    stackLayout,
                    new Button
                    {
                        Text = "Save",

                        TextColor = Colors.Black,
                        Margin = new Thickness(10),
                        BackgroundColor = Colors.LightGreen, 
                        Command = new Command(async () =>
                        {
                            assignment.Description = descriptionEntry.Text;
                            assignment.DeadLine = deadlineEntry.Date;
                            var selectedCourse = courses.FirstOrDefault(c => c.Name == coursePicker.SelectedItem?.ToString());
                            if (selectedCourse != null)
                            {
                                assignment.CourseId = selectedCourse.Id;
                                assignment.CourseName = selectedCourse.Name;
                            }

                            await db.UpdateAsync(assignment);

                            await LoadAssignments();

                            await Application.Current.MainPage.Navigation.PopModalAsync();
                        })
                    },
                    new Button
                    {
                        Text = "Cancel",
                        Margin = new Thickness(10),
                        TextColor = Colors.Black,

                        BackgroundColor = Colors.LightPink, // Set the background color

                        Command = new Command(async () =>
                        {
                            await Application.Current.MainPage.Navigation.PopModalAsync();
                        })
                    }
                }
                    }
                };

                //parāda modalPage
                await Application.Current.MainPage.Navigation.PushModalAsync(modalPage);
            }
        }

        //Metode, kas ļauj dzēst assignment 
        private async void OnDeleteAssignmentClicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            var assignment = button?.CommandParameter as Assignment;
            if (assignment != null)
            {
               
                var result = await DisplayAlert("Delete", $"Are you sure you want to delete assignment: {assignment.Description}?", "Yes", "No");
                if (result)
                {
                    //izdēž assignment no datubāzes
                    var db = await DatabaseService.GetDatabaseAsync();
                    await db.DeleteAsync(assignment);
                    await LoadAssignments();
                }
            }
        }

        //Metode, kas ļauj labot submission datus
        private async void OnEditSubmissionClicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            var submission = button?.CommandParameter as Submission;
            if (submission != null)
            {
                // izveido dialogu, kurā varēs mainīt submission datus
                var scoreEntry = new Entry { Text = submission.Score.ToString(), Placeholder = "Score", Margin = new Thickness(10) };
                var submissionTimeEntry = new DatePicker { Date = submission.SubmissionTime, Margin = new Thickness(10) };
                var assignmentPicker = new Picker { Title = "Select Assignment", Margin = new Thickness(10) };
                var studentPicker = new Picker { Title = "Select Student", Margin = new Thickness(10) };

                var db = await DatabaseService.GetDatabaseAsync();
                var assignments = await db.Table<Assignment>().ToListAsync();
                var students = await db.Table<Student>().ToListAsync();

                foreach (var assignment in assignments)
                {
                    assignmentPicker.Items.Add(assignment.Description);
                }
                assignmentPicker.SelectedItem = assignments.FirstOrDefault(a => a.Id == submission.AssignmentId)?.Description;

                foreach (var student in students)
                {
                    studentPicker.Items.Add(student.Name);
                }
                studentPicker.SelectedItem = students.FirstOrDefault(s => s.Id == submission.StudentId)?.Name;

                // izveido stack ar ievades laukiem un pogām
                var stackLayout = new StackLayout
                {
                    Children = { scoreEntry, submissionTimeEntry, assignmentPicker, studentPicker }
                };

                var modalPage = new ContentPage
                {
                    Content = new StackLayout
                    {
                        Children =
                {
                    new Label { Text = "Edit Submission", FontAttributes = FontAttributes.Bold, HorizontalOptions = LayoutOptions.Center },
                    stackLayout,
                    new Button
                    {
                        Text = "Save",
                        TextColor = Colors.Black,
                        Margin = new Thickness(10),
                        BackgroundColor = Colors.LightGreen,
                        Command = new Command(async () =>
                        {
                            submission.Score = double.Parse(scoreEntry.Text);
                            submission.SubmissionTime = submissionTimeEntry.Date;
                            var selectedAssignment = assignments.FirstOrDefault(a => a.Description == assignmentPicker.SelectedItem?.ToString());
                            if (selectedAssignment != null)
                            {
                                submission.AssignmentId = selectedAssignment.Id;
                            }
                            var selectedStudent = students.FirstOrDefault(s => s.Name == studentPicker.SelectedItem?.ToString());
                            if (selectedStudent != null)
                            {
                                submission.StudentId = selectedStudent.Id;
                            }

                            await db.UpdateAsync(submission);
                            await LoadSubmissions();
                            await Application.Current.MainPage.Navigation.PopModalAsync();
                        })
                    },
                    new Button
                    {
                        Text = "Cancel",
                        Margin = new Thickness(10),
                        TextColor = Colors.Black,
                        BackgroundColor = Colors.LightPink,
                        Command = new Command(async () =>
                        {
                            await Application.Current.MainPage.Navigation.PopModalAsync();
                        })
                    }
                }
                    }
                };

                // parāda modalPage
                await Application.Current.MainPage.Navigation.PushModalAsync(modalPage);
            }
        }

        //metode, kas ļauj dzēst submission
        private async void OnDeleteSubmissionClicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            var submission = button?.CommandParameter as Submission;
            if (submission != null)
            {
                var result = await DisplayAlert("Delete", $"Are you sure you want to delete submission with score: {submission.Score}?", "Yes", "No");
                if (result)
                {
                    var db = await DatabaseService.GetDatabaseAsync();
                    await db.DeleteAsync(submission);
                    await LoadSubmissions();
                }
            }
        }

        //metode, kas ielādē submissions
        private async Task LoadSubmissions()
        {
            try
            {
                var db = await DatabaseService.GetDatabaseAsync();
                var submissions = await db.Table<Submission>().ToListAsync();
                var assignments = await db.Table<Assignment>().ToListAsync();
                var students = await db.Table<Student>().ToListAsync();

                var submissionsWithDetails = submissions.Select(s =>
                {
                    s.AssignmentDescription = assignments.FirstOrDefault(a => a.Id == s.AssignmentId)?.Description;
                    s.StudentName = students.FirstOrDefault(st => st.Id == s.StudentId)?.FullName;
                    return s;
                }).ToList();

                SubmissionListView.ItemsSource = submissionsWithDetails;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"An error occurred while loading submissions: {ex.Message}", "OK");
            }
        }

        //metode, kas ielādē submission pickerus
        private async Task LoadSubmissionPickers()
        {
            try
            {
                var assignments = await GetAssignmentsAsync();
                SubmissionAssignmentPicker.ItemsSource = assignments;
                SubmissionAssignmentPicker.ItemDisplayBinding = new Binding("Description");

                var students = await GetStudentsAsync();
                SubmissionStudentPicker.ItemsSource = students;
                SubmissionStudentPicker.ItemDisplayBinding = new Binding("FullName");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"An error occurred while loading submission pickers: {ex.Message}", "OK");
            }
        }

        //metode, kas dabū assignmentus sinhroni
        private async Task<List<Assignment>> GetAssignmentsAsync()
        {
            var db = await DatabaseService.GetDatabaseAsync();
            return await db.Table<Assignment>().ToListAsync();
        }

        //metode, kas dabū studentus sinhroni
        private async Task<List<Student>> GetStudentsAsync()
        {
            var db = await DatabaseService.GetDatabaseAsync();
            return await db.Table<Student>().ToListAsync();
        }

        //metode, kas ļauj izveidot jaunu submission
        private async void OnCreateSubmissionClicked(object sender, EventArgs e)
        {
            var selectedAssignment = SubmissionAssignmentPicker.SelectedItem as Assignment;
            var selectedStudent = SubmissionStudentPicker.SelectedItem as Student;
            DateTime? submissionDate = SubmissionDatePicker.Date;
            double score;


            if (selectedAssignment == null || selectedStudent == null || !submissionDate.HasValue || !double.TryParse(SubmissionScoreEntry.Text, out score))
            {
                await DisplayAlert("Error", "Please fill in all fields correctly.", "OK");
                return;
            }

            var newSubmission = new Submission
            {
                AssignmentId = selectedAssignment.Id,
                StudentId = selectedStudent.Id,
                SubmissionTime = submissionDate.Value,
                Score = score
            };

      
            var db = await DatabaseService.GetDatabaseAsync();
            await db.InsertAsync(newSubmission);

            await LoadSubmissions();

  
            SubmissionAssignmentPicker.SelectedIndex = -1;
            SubmissionStudentPicker.SelectedIndex = -1;
            SubmissionDatePicker.Date = DateTime.Now;
            SubmissionScoreEntry.Text = string.Empty;

            await DisplayAlert("Success", "Submission created successfully.", "OK");
        }
        //metode, kas ļauj saglabāt datus JSON failā
        private async void OnSaveDataButtonClicked(object sender, EventArgs e)
        {
            string directory = @"C:\Temp";
            string fileName = "DatabaseData.json";
            string filePath = Path.Combine(directory, fileName);

            try
            {
                var db = await DatabaseService.GetDatabaseAsync();

                var teachers = await db.Table<Teacher>().ToListAsync();
                var students = await db.Table<Student>().ToListAsync();
                var courses = await db.Table<Course>().ToListAsync();
                var assignments = await db.Table<Assignment>().ToListAsync();
                var submissions = await db.Table<Submission>().ToListAsync();

                var data = new
                {
                    Teachers = teachers,
                    Students = students,
                    Courses = courses,
                    Assignments = assignments,
                    Submissions = submissions
                };

                string jsonData = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });

                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                await File.WriteAllTextAsync(filePath, jsonData);

                await DisplayAlert("Success", $"Data has been saved to {filePath}", "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"An error occurred while saving data: {ex.Message}", "OK");
            }
        }
        //metode, kas ļauj ielādēt datus no JSON faila
        private async void OnLoadDataButtonClicked(object sender, EventArgs e)
        {
            try
            {
                var result = await FilePicker.PickAsync(new PickOptions
                {
                    PickerTitle = "Please select a JSON file",
                    
                });

                if (result == null)
                    return; // User canceled the file picking

                string filePath = result.FullPath;
                string jsonData = await File.ReadAllTextAsync(filePath);

                var data = JsonSerializer.Deserialize<DatabaseData>(jsonData);

                if (data == null)
                {
                    await DisplayAlert("Error", "Failed to deserialize the JSON data.", "OK");
                    return;
                }

                var db = await DatabaseService.GetDatabaseAsync();

                await db.DeleteAllAsync<Teacher>();
                await db.DeleteAllAsync<Student>();
                await db.DeleteAllAsync<Course>();
                await db.DeleteAllAsync<Assignment>();
                await db.DeleteAllAsync<Submission>();

                await db.InsertAllAsync(data.Teachers);
                await db.InsertAllAsync(data.Students);
                await db.InsertAllAsync(data.Courses);
                await db.InsertAllAsync(data.Assignments);
                await db.InsertAllAsync(data.Submissions);

                await DisplayAlert("Success", "Data has been loaded from the JSON file.", "OK");

                // Refresh the UI
                await LoadData();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"An error occurred while loading data: {ex.Message}", "OK");
            }
        }

        private class DatabaseData
        {
            public List<Teacher> Teachers { get; set; }
            public List<Student> Students { get; set; }
            public List<Course> Courses { get; set; }
            public List<Assignment> Assignments { get; set; }
            public List<Submission> Submissions { get; set; }
        }
    }
}
