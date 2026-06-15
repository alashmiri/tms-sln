
import { Temporal } from "@js-temporal/polyfill";
import { isStudent, type Student } from "./models/student.model.js";
import { parseStudent } from "./models/student.model.js"; 
const student: Student = { 
id: "STU-001", 
name: "Hana Tadesse", 
enrollmentDate: Temporal.Now.instant(), 
}; 
// Try these  what does the compiler say? 
//student.id = "STU-001"; 
//console.log(student.gpa.toFixed(2)); 
console.log(student.gpa?.toFixed(2) ?? "Not yet graded"); 


//import { Student, isStudent } from "./models/student.model"; 
 function processStudent(raw: unknown) { 
  if (isStudent(raw)) { 
    const gpaDisplay = raw.gpa?.toFixed(2) ?? "Not yet graded"; 
    console.log(`Student ${raw.name}  GPA: ${gpaDisplay}`); 
  } else { 
    console.error("Invalid student data received"); 
  } 
} 

 
processStudent({ id: "STU-001", name: "Hana", gpa: 3.7 }); 
// Prints: Student Hana  GPA: 3.70 
 
processStudent(42); 
// Prints: Invalid student data received 


//import { parseStudent } from "./models/student.model"; 
console.log(parseStudent({ id: "STU-001", name: "Hana" })); 
// Prints a valid Student object 
parseStudent({ id: 42, name: "Test" }); 
// Throws: TypeError: Expected id to be a string, received number 