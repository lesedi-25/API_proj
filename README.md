# RaceDay – Event Management System

## 1. Project Overview

RaceDay is an event management system designed for South African running, walking and cycling events.

The system allows event organisers to create and manage sporting events, categories, participant enrolments and race results. Participants can browse upcoming events, enrol in event categories, manage their profiles and view their results.

The project consists of two main components:

- **ASP.NET Core Web API:** Handles the system's data and database operations.
- **ASP.NET Core MVC Website:** Provides a user-friendly website through which participants and organisers interact with the system.

## 2. User Roles

### Participant

Participants can:

- Register and log in.
- View upcoming running, walking and cycling events.
- Filter events by event type.
- View event details, banners and available categories.
- Enrol in an event category.
- View their enrolments and enrolment status.
- View their race results.
- Update their profile details.
- Upload a profile picture.

### Organiser

Organisers can:

- Register and log in.
- Access an organiser dashboard.
- Create, view, update and delete events.
- Upload and replace event banner images.
- Manage event categories.
- View participant enrolments.
- Manage enrolment statuses.
- Record and manage race results.

## 3. Main System Features

- User registration, login and logout.
- Role-based navigation for Participants and Organisers.
- Session-based access to protected website pages.
- Event management.
- Event category management.
- Participant enrolments.
- Enrolment status management.
- Race result management.
- Participant profile management.
- Profile picture uploads.
- Event banner uploads and display.
- API integration between the MVC website and Web API.
- Responsive website layout using Bootstrap.

## 4. Technologies Used

- C#
- ASP.NET Core Web API
- ASP.NET Core MVC
- Entity Framework Core
- Microsoft SQL Server
- Swagger / OpenAPI
- HTML and CSS
- Bootstrap
- JavaScript
- Git and GitHub
- GitHub Actions

## 5. Database Documentation

The `docs` folder contains the database design and supporting documentation:

- `RaceDay_ERD.png` – Entity Relationship Diagram.
- `RaceDay_Endpoint_Plan.xlsx` – API endpoint plan.
- `RaceDay_Database.sql` – SQL Server database creation script.

## 6. Image Uploads

The MVC website uses the local `wwwroot` folder to store uploaded images.

- Participant profile pictures are stored in `wwwroot/images/profiles`.
- Event banner images are stored in `wwwroot/images/events`.

Images are uploaded and displayed by the MVC website without using Azure storage.

## 7. Running the Project

### Requirements

- Visual Studio 2022.
- .NET 8 SDK.
- Microsoft SQL Server.
- SQL Server Management Studio (SSMS).

### Steps

1. Clone the GitHub repository.
2. Open `API_proj.sln` in Visual Studio.
3. Configure the database connection string in the API project's `appsettings.json`.
4. Ensure the database is created and the required migrations have been applied.
5. Configure the MVC project's API base URL in its `appsettings.json`.
6. Set the API project and `RaceDay.Web` as startup projects so they run together.
7. Run the solution.

The API Swagger page is available at:

`http://localhost:5226/swagger`

The MVC website is available at:

`http://localhost:5122/`

## 8. Continuous Integration

The project uses GitHub Actions to build the solution automatically when changes are pushed to the repository.

The workflow helps check that the project builds successfully.

A screenshot of the successful GitHub Actions workflow should be included as evidence.

## 9. Demonstration Videos

### Part 2 – API and Database Planning

YouTube demonstration:

https://youtu.be/5DOIIFe3xcE

This demonstration covers the project planning documents, database design, endpoint plan and SQL database script.

### Part 3 – MVC Website

YouTube demonstration:

(youtube vid placeholder)

The Part 3 demonstration should show the Participant and Organiser workflows, including event management, enrolments, categories, results and image uploads.

## 10. GitHub Repository

Repository: https://github.com/lesedi-25/API_proj

Part 3 branch: `Part3`
