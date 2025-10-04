[Back to Home](README.md)

## Chapter 3
**Architectural Principles**

Architectural Principles delves (يتعمق) into foundational guidelines for contemporary (معاصر) software development, including the Separation of Concerns (SoC), Don’t Repeat Yourself (DRY), Keep It Simple, Stupid (KISS), and You Aren’t Gonna Need It (YAGNI) principles, culminating in a detailed exploration of The SOLID principles.
These principles help stimulate critical thinking, aid in evaluating trade-offs, and guide decision-making toward creating solutions that are flexible, resilient, testable, and maintainable
بمعنى أن هذه المبادئ تساعد في تحفيز التفكير النقدي، وتساعد في تقييم المقايضات، وتوجه اتخاذ القرارات نحو إنشاء حلول تكون مرنة، وقوية، وقابلة للاختبار، وسهلة الصيانة.

-> Trade-offs
و هي موازنة بين جوانب مختلفة من النظام، مثل الأداء، القابلية للصيانة، الأمان، والتكلفة.

1. DRY
    This principle advocate the SoC (Seperation Of Concern) princile
2. KISS
    This principle emphasizes the idea that systems work best when they are kept simple 
3. YAGNI
    Don't add functionality untill you need it    
4. SOLID: 

***SRP*** Single Resposibility Principle

***OCP*** Open/Closed Principle

***LSP*** Liskov Substitution Principle

***ISP*** Interface Segregation Principle

***DIP*** Dependency Inversion Principle

1._**SRP**_ - Single Responsibility Principle
The SRP dictates that a single class should have only one responsibility, leading to the famous quote: "There should never be more than one reason for a class to change".

  • **Goal**: Fewer responsibilities result in less code, making it easier to understand and minimizing the impact of changes on the overall system.

  • **Application**: Using mapper objects helps extract the responsibility of mapping types from the consumers, making it easier to maintain and refactor the mapping logic, thus adhering to the SRP

  • **Violation**: The anti-pattern of a God class breaks the SRP by concentrating too many responsibilities in one unit. The book addresses this by recommending the segregation of responsibilities into multiple cohesive units.



[Next Chapter](Chapter4.MD)
