Console.WriteLine("Hello, World!");
// non-nullable bug warning
 
// This is how the legacy system declared region — no indication it could be empty 
//string region = null; // Compiler warning CS8600 
//Console.WriteLine(region.ToUpper());  // Compiler warning CS8602 


// Legacy implementation — the bug that caused the audit failure 
/*double grantPerStudent = 1999.99; 
double totalAllocation = grantPerStudent * 100_000; 
Console.WriteLine($"Total allocated (double): {totalAllocation}"); 
*/
// invalid inputs checking

var s=new Student { Id = "S2", Name = "", Age = 20, GPA = 3.0m };         
 new Student { Id = "S3", Name = "Test", Age = 12, GPA = 3.0m };   
new Student { Id = "S4", Name = "Test", Age = 20, GPA = 5.0m };  

Console.WriteLine($"Student: {s.Name}, GPA: {s.GPA}");
  

