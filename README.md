RaceDay – Event Management API

1. Project Overview

RaceDay is an event management system designed for South African running, walking and cycling events. The API allows event organisers to manage events, categories, participant enrolments and race results. Participants can view events, enrol in categories and access their results.

The project uses ASP.NET Core Web API, C#, Entity Framework Core and Microsoft SQL Server.

2. User Roles

Participant
- Register and log in.
- View available events and event categories.
- Enrol in an event category.
- View enrolments and race results.

Organiser
- Register and log in.
- Create, view, update and delete events.
- Manage event categories.
- View participant enrolments for events.
- Record and manage race results.

3. Main API Features

- User registration and login.
- User profile management.
- Event management.
- Event category management.
- Participant enrolments.
- Race result management.
- Route information.
- Weather information.

4. Technologies Used

- C#
- ASP.NET Core Web API
- Entity Framework Core
- Microsoft SQL Server
- Swagger / OpenAPI
- Git and GitHub
- GitHub Actions

5. Database Documentation

The `docs` folder contains the database design and supporting documentation:

- `RaceDay_ERD.png` – Entity Relationship Diagram.
- `RaceDay_Endpoint_Plan.xlsx` – API endpoint plan.
- `RaceDay_Database.sql` – SQL Server database creation script.

6. Continuous Integration

The project uses GitHub Actions to restore dependencies, build the solution and run automated tests when changes are pushed to or submitted against the `master` branch.

CI/CD evidence: Insert a screenshot of the successful GitHub Actions workflow here.

7. Demonstration Video

YouTube demonstration: []

The demonstration should explain the project planning documents, database design, endpoint choices and relevant implementation or testing steps.
