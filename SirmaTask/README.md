# SirmaTask - Employee Pair Analyzer
## Overview

SirmaTask is an ASP.NET Core MVC application that analyzes employee project history from a CSV file and identifies the pair of employees who have worked together on common projects for the longest total period of time.

The application allows the user to select a CSV file through a web interface. The file is validated and parsed, the employee collaboration periods are calculated, and the pair with the longest total collaboration time is displayed together with all of their common projects.

The application supports multiple date formats and multiple CSV separators. Supported date formats include `yyyy-MM-dd`, `dd-MM-yyyy`, `dd/MM/yyyy`, `MM/dd/yyyy`, `dd.MM.yyyy` and `yyyy/MM/dd`. Supported separators include comma, semicolon, pipe and tab.

The project also includes validation, centralized error handling, application logging, overlapping period handling and a responsive user interface.

For the winning employee pair, the application displays:

- Employee ID #1
- Employee ID #2
- Project ID
- Days Worked

The total number of days worked together across all common projects is also displayed.


## Features
The application provides the following functionality:

- CSV file upload through a web interface
- Employee collaboration analysis
- Identification of the employee pair with the longest total collaboration period
- Display of all common projects for the winning pair
- `NULL` DateTo values treated as the current date
- Support for multiple date formats:
  - `yyyy-MM-dd`
  - `dd-MM-yyyy`
  - `dd/MM/yyyy`
  - `MM/dd/yyyy`
  - `dd.MM.yyyy`
  - `yyyy/MM/dd`
- Support for multiple CSV separators:
  - Comma `,`
  - Semicolon `;`
  - Pipe `|`
  - Tab
- Automatic CSV separator detection
- Optional CSV header detection
- Input validation
- Centralized error handling
- Overlapping period handling
- Inclusive day calculation
- Dependency Injection
- ASP.NET Core logging
- Responsive UI using Razor Views, Bootstrap and custom CSS


## Technologies Used
### Backend
The backend is implemented using:

- C#
- .NET 8
- ASP.NET Core MVC
- Dependency Injection
- LINQ
- StreamReader
- ASP.NET Core ILogger

The backend logic is separated into dedicated services for CSV parsing and employee collaboration analysis.

### Frontend
The user interface is implemented using:

- ASP.NET Core Razor Views
- HTML
- Bootstrap
- Custom CSS
- JavaScript

Bootstrap is used mainly for responsive behavior, navigation, forms and basic UI components.

Custom CSS is used for the application-specific visual design.

A small amount of JavaScript is used for the custom file selector and for displaying the selected CSV file name.


## CSV Input Format
The application expects a CSV file containing four columns:

- **EmpID** - identifier of the employee
- **ProjectID** - identifier of the project
- **DateFrom** - date when the employee started working on the project
- **DateTo** - date when the employee stopped working on the project

The columns must appear in the following order:

**EmpID, ProjectID, DateFrom, DateTo**

Example:

EmpID,ProjectID,DateFrom,DateTo
143,12,2013-11-01,2014-01-05
218,10,2012-05-16,NULL
143,10,2009-01-01,2011-04-27