Name: Braden Caleb Perumal
Student Number: ST10287165
Table of Contents:
1) Introduction
2) Requirements
3) How to Apply
4) Application Overview
5) Architecture
6) Functionality
7) Non-Functional Requirements
8) Change Log
9) FAQ's
10) How to Use
11) Licensing
12) Plugins
13) Credits
14) GitHub Link
15) Admin Login Credentials
16) References

1) Introduction:
ClaimEase CMCS is an ASP.NET Core web application designed to streamline the process of managing claims for independent contractors (IC), such as lecturers. This enhanced version introduces functional capabilities for claim submission, document management, and status tracking, allowing admins to manage and update claims efficiently.

2) Requirements:
.NET 8.0 or higher
Microsoft SQL Server (for database management)
Visual Studio 2022 or higher.


<configuration>
    <startup> 
        <supportedRuntime version="v4.0" sku=".NETFramework,Version=v4.6" />
    </startup>
</configuration>

3) How to Apply:
-  Clone the repository or download the source code.
- Open the solution file in Microsoft Visual Studio.
- Ensure that all necessary NuGet packages are restored.
- Build the solution to ensure all dependencies are resolved.
- Run the application to launch the local development server.


4) Application Overview:
Purpose: ClaimEase CMCS is built to simplify and automate the claims management process for lecturers and administrators, providing a user-friendly interface for claim submission and review.
5) Architecture:
The ClaimEase CMCS application follows the Model-View-Controller (MVC) design pattern:
- Models: Define the data structure for claims, users, and related entities.
- Views: Provide the user interface components that interact with the data and display information.
- Controllers: Handle the flow of data between the model and the view, processing user input and updating the model accordingly.

6) Functionality:
- Claim Submission: Lecturers can submit their claims at any time with a click of a button.
- Document Upload: Lecturers can upload supporting documents for their claims.
- Claim Status Management: Admins can view, update, and manage claims, ensuring that all submissions are processed promptly.



7) Non-Functional Requirements:
- Security: Role-based authentication and secure user session management ensure data protection and privacy.
- Performance: Optimized for efficient data retrieval and minimal response times.
- Scalability: Designed to accommodate a growing number of users and claims without compromising performance.
- Reliability: Ensures consistent functionality across various devices and browsers.
- Usability: Simple, intuitive design with clear navigation for both lecturers and administrators.


8) Change Log:
- New Features Added in Part 3:
- Admin Dashboard Enhancements:
-	Improved navigation and added functionalities like "Generate Reports" and "View Claims.
-	Dynamic filtering for claims based on date ranges and statuses.

- Claim Report Generation:
- Enhanced report generation for both approved and rejected claims.
- Added detailed summaries, including gross pay, overtime hours, and tax deductions.

-Lecturer Dashboard Updates:
-Refined claim submission form with real-time validations.
-Enabled file uploads and secure handling of supporting documents.
-Improved "View Claim Status" and historical data viewing capabilities.

-Profile Management:
-Lecturers and admins can update their profile details, reset passwords, and enable two-factor authentication for added security.

-Security Updates:
-Strengthened data encryption and added measures to secure sensitive user information.
-Enforced role-based access control for both lecturers and admins.
-Performance Enhancements:
-Optimized database queries and page load times for improved system responsiveness.
-Enhanced scalability to accommodate a higher number of users and claims.

9) FAQ's:
- Q1: How do I submit a claim?
Navigate to the "Submit Claim" page, enter your claim details, and upload any supporting documents. Click "Submit" to send your claim for processing.
-Q2: How can I track the status of my claim?
Go to the "View Claims" section to see the status of all your submitted claims.
-Q3: What should I do if my claim is rejected?
Review the feedback provided and make any necessary adjustments before resubmitting your claim.
-Q4: How do I reset my password?
Contact your admin or use the "Forgot Password" feature on the login page.
-Q5: Can I update my profile information?
Yes, you can update your profile information in the "Manage Profile" section.

10) How to Use:
Starting the Application:
-Open Microsoft Visual Studio.
-Navigate to the directory where you cloned or downloaded the repository.
-Open the solution file in Visual Studio.
-Build the solution by selecting “Build” from the top menu, then “Build Solution.”
-Run the application by pressing F5 or selecting “Start Debugging” from the “Debug” menu.

11) Licensing:
ClaimEase CMCS is licensed under the MIT License, which allows for open usage, modification, and distribution with appropriate credit given to the original author.

12) Plugins:
The application does not use external plugins beyond the necessary .NET packages and Bootstrap for styling.

13) Credits:
This project was developed and tested by Braden Caleb Perumal (ST10287165).

14) GitHub Link:
https://github.com/VCWVL/prog6212-poe-CalebPerumal28.git

15) Admin Login Credentials:
Admin Email: admin@cmcs.com
Admin Password: Admin123@#

16) References: 
BroCode, n.d. C# Full Course for free. [Online] Available at: https://www.youtube.com/watch?v=wxznTygnRfQ&t=10600s&ab_channel=BroCode
BROCODE, n.d. C# tutorial for beginners. [Online] Available at: https://www.youtube.com/watch?v=r3CExhZgZV8&list=PLZPZq0r_RZOPNy28FDBys3GVP2LiaIyP_&ab_channel=BroCode
Christensen, M., n.d. Creating a List of Lists in C#. [Online] Available at: https://stackoverflow.com/questions/12628222/creating-a-list-of-lists-in-c-sharp
GeeksforGeeks, n.d. C# | Constructors. [Online] Available at: https://www.geeksforgeeks.org/c-sharp-constructors/
Slayden, G., n.d. How to convert emoticons to its UTF-32/escaped unicode?. [Online] Available at: https://stackoverflow.com/questions/44728740/how-to-convert-emoticons-to-its-utf-32-escaped-unicode

