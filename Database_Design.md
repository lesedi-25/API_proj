RaceDay Database Design

 Overview

RaceDay uses a relational database to store information about users, events, categories, enrolments, results, routes and weather.

 Main Entities

- Users: Stores user details and identifies Participants and Organisers.
- Events: Stores event information and links events to their organisers.
- Categories: Stores categories associated with events.
- Enrolments: Records participants entering events and categories.
- Results: Stores participant performance and finishing information.
- Routes: Stores route information associated with events.
- Weather: Stores weather information associated with events.

Relationships

Foreign keys connect related records across the database. Events reference their organisers, categories reference events, and enrolments reference participants, events and categories. Results reference enrolments, while routes and weather records reference events.

Primary keys uniquely identify records. Foreign keys help maintain referential integrity between related tables.

The database design supports the main RaceDay event management workflow.
