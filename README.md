# Library Management System (LMS)

This project is a Library Management System (LMS) that consists of a **frontend** built with React, TypeScript, and Vite, and a **backend** built with ASP.NET Core. The system allows users to manage books, borrow requests, and other library-related functionalities.

---

## Table of Contents
###  Normal User Actions: 
1. Borrowing Books:
2. - Allow users to borrow up to 5 books in one request.
     - Limit to 3 borrowing requests per month.
     - Display the status of each request (Approved/Rejected/Waiting). 
3. View Borrowed Books:
  -Provide a list of books the user has borrowed with their statuses. 
### Super User Actions: 
1. Manage Categories: - View, add, update, and delete categories. 
2. Manage Books: - View, add, update, and delete books. 
3. Manage Borrowing Requests: - View all borrowing requests. - Approve or reject borrowing requests.
### Advanced feature
- Token-Based Authentication/Authorization (refresh token)
- send email when change request status
- filter book by category id, search book by title, paging
---

## Frontend

The frontend is built using **React**, **TypeScript**, and **Vite**. It provides a user-friendly interface for managing books and borrow requests.
## How to run application

```bash
git clone https://github.com/nguyen-bi-rain/R2EMIDASS.git
cd R2EMIDASS
```
### Frontend Setup

1. Navigate to the `frontend` directory:
   ```sh
   cd frontend
   ```

2. Install dependencies:
   ```sh
   npm install
   ```

3. Start the development server:
   ```sh
   npm run dev
   ```



### Frontend Scripts

- **`npm run dev`**: Start the development server.
- **`npm run build`**: Build the project for production.
- **`npm run preview`**: Preview the production build.
- **`npm run lint`**: Run ESLint to check for code quality issues.

---

## Backend

The backend is built using **ASP.NET Core** and provides RESTful APIs for managing the library system.


### Backend Setup

1. Navigate to the `backend/LMS` directory:
   ```sh
   cd backend/LMS
   ```

2. Set up the environment variables:
   - Copy the `.env.example` file to `.env`:
     ```sh
     cp .env.example .env
     ```
   - Update the `.env` file with your database connection string and other configurations.

3. Restore dependencies:
   ```sh
   dotnet restore
   ```

4. Run database migrations:
   ```sh
   dotnet ef database update
   ```

5. Start the application:
   ```sh
   dotnet run
   ```


### Backend Scripts

- **`dotnet restore`**: Restore NuGet packages.
- **`dotnet build`**: Build the project.
- **`dotnet run`**: Run the application.
- **`dotnet test`**: Run unit tests.

---

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.
