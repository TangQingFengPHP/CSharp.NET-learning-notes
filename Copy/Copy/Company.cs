namespace Copy;

public class Company
{
    public long Id { get; set; }
    public List<Employee> Employees { get; set; }
}

public class Employee
{
    public string Name { get; set; }
    public Company Company { get; set; }
}