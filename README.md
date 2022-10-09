<br />
<p align="center">
  <a href="https://github.com/twgraham/AnQL">
    <img src="assets/images/ankles.jpg" alt="Cartoon Ankles" width="200">
  </a>

  <h3 align="center">AnQL</h3>

  <p align="center">
    Another Query Language - simple, no nonsense querying for C#/ASP.NET/TypeScript 
    <br />
    <a href="https://codecov.io/github/twgraham/AnQL">
        <img src="https://github.com/twgraham/AnQL/actions/workflows/build-test.yaml/badge.svg" />
    </a>
    <a href="https://codecov.io/github/twgraham/AnQL" > 
        <img src="https://codecov.io/github/twgraham/AnQL/branch/master/graph/badge.svg?token=FUJXCID1YL"/> 
    </a>
</p>

This project is still a work in progress.

A simple query language that can be consumed by a range of targets:

- In memory collections (C# and Typescript)
- Database ORMs (using C# Expressions)
- MongoDB

Example of the query language

```
(first_name: John or first_name: Jane) and age: > 30 and start_date: "last year"
```

Let's say we have the following model:
```csharp
public class Employee
{
  public string FirstName { get; set; }
  public string LastName { get; set; }
  public int Age { get; set; }

  [Display(Name = "start_date")]
  public DateTime CommencementDate { get; set; }
}
```

```csharp
// Create the parser
// you can selectively register properties on your class
// or customize their matching logic
AnQLParser<Employee> _employeeQueryParser = new AnQLBuilder()
  .ForFunctions<Employee>()
  .RegisterAllProperties();

// Now we can call our method with AnQL queries
// e.g.
// - first_name: John
// - age: > 30 AND start_date: "last year"
// - NOT start_date: "June 2020"
public List<Employee> QueryEmployees(string query)
{
  List<Employee> employees = LoadEmployeesFromSomewhere();
  Func<Employee, bool> employeeFilter = _employeeQueryParser.Parse(query);

  return employees.Where(employeeFilter).ToList();
}
```
