docs: add detailed setup instructions for frontend and backend in README



FRONTEND

\- Go to the frontend folder

\- Run `npm install`

\- Run `npm run dev`

\- Make sure your localhost runs under http://localhost:3000/ for testing purposes

\- Go to the `.env` file and make sure the API URL runs under https://localhost:7216/ or change it when necessary



BACKEND

\- Go to `backend\\MentalHealthCheckinApi`

\- Open the `MentalHealthCheckinApi.sln` in Visual Studio

\- Check `appsettings.json` and make sure you have the correct database details; you can change the username/password when necessary

\- Go to \*\*Tools > NuGet Package Manager > Package Manager Console\*\*

\- Run `dotnet ef migrations add InitialCreate` to create a table in the database

\- Run the app; it will automatically migrate and seed the data

\- Make sure the API runs under https://localhost:7216/
- Try to open https://localhost:7216/swagger/index.html



LOGIN

Manage Account - has permission to edit and to view all the check-ins

&nbsp;Username: bob

&nbsp;Password: password123



Employee Account - only permission is creating self check-ins

&nbsp;Username: alice

&nbsp;Password: password123

