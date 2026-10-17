RaceDay Testing Guide

Build Verification

Open the solution in Visual Studio or run `dotnet build API_proj.sln` from the repository root. Confirm that the build completes successfully.

API Testing

Use Swagger to inspect and test the available endpoints.

Recommended workflow:

1. Register a user with the Organiser role.
2. Register a user with the Participant role.
3. Create an event using a valid organiser ID.
4. Create a category for that event.
5. Enrol the participant in the event and category.
6. Record a result for the enrolment.
7. Retrieve the result to verify that it was saved.

Use the actual request fields required by each endpoint.

Continuous Integration

Check the repository's GitHub Actions tab after pushing changes. Review the workflow result and investigate any failed restore, build or test steps.
