# Architecting.Asp.Net.Core.ThirdEdition Book Digest
## Section 1: Principles and Methodologies
1. **Chapter 1**
2. **Chapter 2**
3. **Chapter 3**
4. **Chapter 4**
## Section 2: Designing with ASP.NET Core
1. **Chapter 5** *Minimal APIs*
* Top Level Statements: Now in Entry point(program.cs) we can stop using namespaces
* Minimal Hosting: Combine two files (startup & program) into single file "program.cs" this laverage to minimize the biolerplate code necessary to bootstrap the application
```C#
WebApplicationBuilder builder = WebApplication.Create(args);
webApplication app = builder.Build();
app.Run();

```
* Minimal APIs: minimal here means Lean not mean small or not enough.
[-] Serve essential features
[-] Using Minimal APIs We map routes to delegate
    - Inline Delegate (Arrow function)
    - Method Delegate 

2. **Chapter 6** *MVC*