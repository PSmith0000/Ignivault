Ignivault 🔑
Ignivault is a secure, full-stack zero-knowledge password and file vault built with .NET, Blazor WebAssembly, and Syncfusion components. The core security principle is that all user data is encrypted and decrypted exclusively on the client-side, ensuring the server never has access to user secrets.

Core Concept: Zero-Knowledge Architecture
The security of this vault relies on a zero-knowledge model. The user's master password is never sent to the server. Instead, it is combined with a unique salt (fetched from the server) to derive a master encryption key directly in the user's browser using the Web Crypto API. This key is used to encrypt and decrypt vault items with AES-GCM and is discarded when the session ends. The server only ever stores opaque, encrypted blobs of data.

✨ Features
Secure User Authentication:

Registration and a multi-step login flow.

Two-Factor Authentication (2FA) with TOTP and single-use recovery codes.

Password reset via secure email links (using SendGrid).

Vault Management Dashboard:

A central dashboard with Syncfusion Charts and Grids to view all vault items.

Items are categorized into tabs for Credentials, Notes, and Files.

Client-side search and date-range filtering.

Client-Side Cryptography:

Full CRUD (Create, Read, Update, Delete) for all vault items with end-to-end encryption.

Secure, streaming uploads and downloads of large (up to 100MB) encrypted files.

A standalone utility page for decrypting downloaded files.

Role-Based Access Control (RBAC):

A clear distinction between User and Admin roles, enforced by JWT claims.

Admin-Only Features:

A Reports Page to view application-wide statistics.

A User Management Page to view, lock, unlock, and manage roles.

A detailed audit trail of all significant user actions.

💻 Technology Stack
Area	Technology
Backend	ASP.NET Core Web API (.NET 9)
Frontend	Blazor WebAssembly (.NET 9)
UI Components	Syncfusion Blazor
Database	Entity Framework Core & SQL Server
Authentication	ASP.NET Core Identity & JWT
Email	SendGrid
Deployment	Microsoft Azure (App Service & SQL Database)

🚀 Getting Started
Prerequisites
.NET 9 SDK

Visual Studio 2022 or later

SQL Server (LocalDB, Express, or full version)

Configuration
Clone the repository.

In the ignivault.WebAPI project, rename appsettings.Example.json to appsettings.Development.json.

Fill in the required values:

ConnectionStrings:DefaultConnection: Your local SQL server connection string.

Jwt:Key: A long, random, secret string for signing JWTs.

SendGrid:ApiKey: Your SendGrid API key.

AdminUsers: Configure at least one default admin account.

In the ignivault.App project's wwwroot folder, rename appsettings.Example.json to appsettings.Development.json.

Fill in the required values:

ApiBaseAddress: The local URL of your Web API (e.g., https://localhost:7269).

SyncfusionLicenseKey: Your Syncfusion license key.

Running the Application
Open the solution (.sln) in Visual Studio.

Open the Package Manager Console.

Set the "Default project" to ignivault.WebAPI.

Run the command Update-Database. This will create the database and run all migrations.

Set ignivault.WebAPI as the startup project and run the application (F5).

☁️ Deployment
The application is designed to be deployed to Microsoft Azure. The process involves:

Creating an Azure SQL Database.

Creating an Azure App Service.

Publishing the ignivault.WebAPI project from Visual Studio.

Configuring all secrets (Connection String, JWT Key, SendGrid Key) in the App Service Configuration for a secure production environment.

📜 License
This project is licensed under the MIT License.
