RaceDay API Usage

Overview

RaceDay exposes HTTP endpoints for managing users, events, categories, enrolments and results, as well as route and weather information.

 HTTP Methods

- GET: Retrieves resources.
- POST: Creates a resource or submits information.
- PUT: Updates an existing resource.
- PATCH: Updates selected fields when supported.
- DELETE: Removes a resource when supported.

Testing with Swagger

1. Run the API project in Visual Studio.
2. Open the Swagger interface in the browser.
3. Expand a controller to view its available endpoints.
4. Select an endpoint and choose **Try it out**.
5. Enter the required parameters or request body.
6. Execute the request and review the HTTP status code and response.

Use valid IDs and request data when testing endpoints. The available operations and required fields depend on each controller's implementation.
