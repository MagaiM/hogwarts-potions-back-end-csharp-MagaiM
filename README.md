# hogwarts-potions-back-end-csharp-MagaiM

We are helping the wizards at Hogwarts with administrative tasks.

## Table of Contents

- [Introduction](#introduction)
- [Features](#features)
- [Prerequisites](#prerequisites)
- [Installation](#installation)
- [Usage](#usage)

## Introduction

This project is to upgrade and improve an older project. 
I used this project to learn about:
- ASP.NET
- Entity Framework Core
- Identity Framework
- JWTs

## Features

You can: 
- Create or destroy rooms
- Administer or expell students
- Login / out
- Move students between rooms 
- Brew potions
- Create new potion recipes

## Prerequisites

- [.NET Core 6.0.16](https://dotnet.microsoft.com/download/dotnet/6.0)
- [ASP.NET Core 6.0.16](https://dotnet.microsoft.com/download/dotnet/6.0)
- [Microsoft.AspNetCore.Authentication.JwtBearer 6.0.11](https://www.nuget.org/packages/Microsoft.AspNetCore.Authentication.JwtBearer)
- [Microsoft.AspNetCore.Identity.EntityFrameworkCore 6.0.11](https://www.nuget.org/packages/Microsoft.AspNetCore.Identity.EntityFrameworkCore)
- [Microsoft.AspNetCore.Mvc.NewtonsoftJson 6.0.6](https://www.nuget.org/packages/Microsoft.AspNetCore.Mvc.NewtonsoftJson)
- [Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation 6.0.6](https://www.nuget.org/packages/Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation)
- [Microsoft.EntityFrameworkCore 7.0.0](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore)
- [Microsoft.EntityFrameworkCore.Relational 7.0.0](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore.Relational)
- [Microsoft.EntityFrameworkCore.SqlServer 7.0.0](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore.SqlServer)
- [Microsoft.EntityFrameworkCore.Tools 7.0.0](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore.Tools)
- [Microsoft.Extensions.Identity.Core 6.0.11](https://www.nuget.org/packages/Microsoft.Extensions.Identity.Core)
- [Microsoft.Extensions.Identity.Stores 6.0.11](https://www.nuget.org/packages/Microsoft.Extensions.Identity.Stores)
- [Microsoft.Net.Compilers.Toolset 4.4.0](https://www.nuget.org/packages/Microsoft.Net.Compilers.Toolset)
- [Newtonsoft.Json 13.0.1](https://www.nuget.org/packages/Newtonsoft.Json)

## Installation

To run this project, make sure you have the [.NET Core 6.0.16](https://dotnet.microsoft.com/download/dotnet/6.0) and [ASP.NET Core 6.0.16](https://dotnet.microsoft.com/download/dotnet/6.0) installed!

Follow these steps to set up the project:

1. Install .NET Core 6.0.16 by visiting the [.NET downloads page](https://dotnet.microsoft.com/download/dotnet/6.0) and selecting the appropriate installer for your operating system.

2. Clone the repository to your local machine using the following command:

   ```bash
   git clone https://github.com/MagaiM/hogwarts-potions-back-end-csharp-MagaiM.git
   ```
   
 3. Navigate to the project directory:
   ```bash
   cd hogwarts-potions-back-end-csharp-MagaiM
   ```
   
 4. Use the .NET CLI to restore the project dependencies:
  ```bash
  dotnet restore
  ```
  
 5. Build the project:
  ```bash
  dotnet build
  ```
  
 6. Once the build process is complete, you can run the project using the following command:
  ```bash
  dotnet run
  ```
  The API will start running locally on http://localhost:5000.
  
  Note: It's recommended to set up and configure your database connection string in the appsettings.json file before running the project.

## Usage

For an example frontend application using this API, check out this project: https://github.com/MagaiM/hogwarts-potions-frontend-angular-MagaiM

Once you downloaded installed and run the api as seen in the [Installation](#installation) you can send requests to end points.

To interact with the API, you can use the following endpoints:

- **Retrieve all students:**
  ```
  GET /student
  ```
  This endpoint retrieves a list of all students.

- **Retrieve a specific student:**
  ```
  GET /student/{id}
  ```
  Replace `{id}` with the ID of the desired student to retrieve their details.
  
- **Register a new student:**
  ```
  POST /student/register
  ```
  Use this endpoint to register a new student. Send the required student information in the request body.
  
- **Register a new admin:**
  ```
  POST /admin/register
  ```
   Use this endpoint to register a new admin. Send the required admin information in the request body.
   
- **Login:**
  ```
  POST /login
  ```
  Use this endpoint to login. Send the required student/admin information in the request body.
  
- **Logout:**
  ```
  DELETE /logout
  ```
  The active user's JWT is exterminated.
   
- **Assign student to a room:**
  ```
  POST /student/add-room/{id}
  ```
  Use this endpoint to assign a student to a room. Replace `{id}` with the ID of the desired student. Send the required room information in the request body.
  
- **Update a student's information:**
  ```
  PUT /student/{id}
  ```
  Replace `{id}` with the ID of the desired student to update their details. Send the required student information in the request body.
  
- **Expell a student:**
  ```
  DELETE /student/{id}
  ```
  Replace `{id}` with the ID of the desired student to expell.
  
- **Get a usable room for a student:**
  ```
  GET /student/select-room/{id}
  ```
  Replace `{id}` with the ID of the student to get valid rooms.
  
Make sure to include the necessary headers and any required authentication or authorization tokens when making requests to the API.
