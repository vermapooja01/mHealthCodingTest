using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Linq;
using System.Threading.Tasks;
using System.IO;

namespace Demo
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            API obj = new API();
            await obj.outputdata();
        }
    }

    public class API
    {
        string baseaddress = "http://mhealthtechinterview.azurewebsites.net/api/";
        public async Task<List<Employee>> EmployeeList()
        {
            List<Employee> emplist = new List<Employee>();
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                using (var response = await httpClient.GetAsync(baseaddress + "employees"))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    emplist = JsonConvert.DeserializeObject<List<Employee>>(apiResponse);
                }
            }
            return emplist;
        }

        public async Task<List<Department>> DepartmentList()
        {
            List<Department> deptlist = new List<Department>();
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                using (var response = await httpClient.GetAsync(baseaddress + "departments"))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    deptlist = JsonConvert.DeserializeObject<List<Department>>(apiResponse);
                }
            }
            return deptlist;
        }
        public async Task<List<Todo>> TodoList()
        {
            List<Todo> todolist = new List<Todo>();
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                using (var response = await httpClient.GetAsync(baseaddress + "todos"))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    todolist = JsonConvert.DeserializeObject<List<Todo>>(apiResponse);
                }
            }
            return todolist;
        }
        public async Task outputdata()
        {
            API _api = new API();
            var empdata = await _api.EmployeeList();
            empdata = empdata.Where(x => x.BadgeNumber == null).ToList();
            var depdata = await _api.DepartmentList();
            var employee_departmet = depdata.GroupJoin(empdata, std => std.Id, s => s.DepartmentId,
                                (std, empmloyeesgroup) => new 
                                {
                                    employees = empmloyeesgroup,
                                    Departname = std.Name
                                });

            var tododata = await _api.TodoList();
            var todo_employee = tododata.GroupJoin(empdata, std => std.AssigneeId, s => s.Id,
                                (std, empmloyeesgroup) => new 
                                {
                                    employees = empmloyeesgroup,
                                    DueDate = std.DueDate
                                });
            using (TextWriter tw = new StreamWriter("SavedList.txt"))
            {
                tw.WriteLine("## Employee without badges");
                foreach (var stud in empdata)
                    tw.WriteLine(string.Format(stud.LastName + "-" + stud.LastName + "," + stud.FirstName));
                //department
                tw.WriteLine("## Employee by department");
                foreach (var item in employee_departmet)
                {
                    tw.WriteLine(item.Departname);
                    foreach (var stud in item.employees)
                        tw.WriteLine(string.Format(stud.LastName + "," + stud.FirstName));
                }
                //todo
                tw.WriteLine("## Assigned Task By Due Date");
                foreach (var item in todo_employee)
                {
                    tw.WriteLine(item.DueDate);
                    foreach (var stud in item.employees)
                        tw.WriteLine(string.Format(stud.LastName+","+ stud.FirstName+"-"+ tododata.Where(x=> x.AssigneeId==stud.Id).Select(x=> x.Description).FirstOrDefault()));
                }

            }
        }
    }

    public class Employee
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int DepartmentId { get; set; }
        public string Position { get; set; }
        public int BadgeNumber { get; set; }
        public string HiredDate { get; set; }
        public float ProductivityScore { get; set; }
    }
    public class Department
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
    public class Todo
    {
        public int Id { get; set; }
        public int AssigneeId { get; set; }
        public string Description { get; set; }
        public string DueDate { get; set; }
        
    }
}
