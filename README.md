<h1 align="center">Ignivault 🔑</h1>

<p align="center">
  A secure, full-stack <strong>zero-knowledge</strong> password and file vault built with .NET, Blazor WebAssembly, and Syncfusion components.
</p>
<div>
  <h1 align="center">Doxygen Docs</h1>
  <p align="center">
  https://psmith0000.github.io/ignivault-docs/
  </p>
</div>
## Core Concept: Zero-Knowledge Architecture

The security of this vault relies on a zero-knowledge model. The user's master password is never sent to the server. Instead, it is combined with a unique salt to derive a master encryption key directly in the user's browser using the Web Crypto API. This key is used to encrypt and decrypt vault items with **AES-GCM**. The server only ever stores opaque, encrypted blobs of data.

---
## ✨ Features

- **Secure User Authentication:**
    - Registration and a multi-step login flow.
    - **Two-Factor Authentication (2FA)** with TOTP and recovery codes.
    - Password reset via secure email links.
- **Vault Management Dashboard:**
    - A central dashboard with **Syncfusion Charts and Grids**.
    - Client-side search and date-range filtering.
- **Client-Side Cryptography:**
    - Full CRUD (Create, Read, Update, Delete) with end-to-end encryption.
    - Secure, streaming uploads and downloads of large files.
    - A standalone utility page for decrypting downloaded files.
- **Role-Based Access Control (RBAC):**
    - A clear distinction between `User` and `Admin` roles.
- **Admin-Only Features:**
    - A **Reports Page** for application-wide statistics.
    - A **User Management Page** to view, lock, unlock, and manage roles.
    - A detailed audit trail of significant user actions.

---
## 💻 Technology Stack

<table>
  <thead>
    <tr>
      <th>Area</th>
      <th>Technology</th>
    </tr>
  </thead>
  <tbody>
    <tr>
      <td><strong>Backend</strong></td>
      <td>ASP.NET Core Web API (.NET 9)</td>
    </tr>
    <tr>
      <td><strong>Frontend</strong></td>
      <td>Blazor WebAssembly (.NET 9)</td>
    </tr>
    <tr>
      <td><strong>UI Components</strong></td>
      <td>Syncfusion Blazor</td>
    </tr>
    <tr>
      <td><strong>Database</strong></td>
      <td>Entity Framework Core & SQL Server</td>
    </tr>
    <tr>
      <td><strong>Authentication</strong></td>
      <td>ASP.NET Core Identity & JWT</td>
    </tr>
     <tr>
      <td><strong>Email</strong></td>
      <td>SendGrid</td>
    </tr>
    <tr>
      <td><strong>Deployment</strong></td>
      <td>Microsoft Azure</td>
    </tr>
  </tbody>
</table>

---
## 🚀 Getting Started

<details>
  <summary><strong>Click to view setup instructions</strong></summary>
  
  ### Prerequisites
  - .NET 9 SDK
  - Visual Studio 2022 or later
  - SQL Server (LocalDB, Express, or full version)

  ### Configuration
  1. Clone the repository.
  2. In the `ignivault.WebAPI` project, rename `appsettings.Example.json` to `appsettings.Development.json`.
  3. Fill in the required values:
      - `ConnectionStrings:DefaultConnection`
      - `Jwt:Key`
      - `SendGrid:ApiKey`
      - `AdminUsers`
  4. In the `ignivault.App` project's `wwwroot` folder, rename `appsettings.Example.json` to `appsettings.Development.json`.
  5. Fill in the required values:
      - `ApiBaseAddress`
      - `SyncfusionLicenseKey`

  ### Running the Application
  1. Open the solution (`.sln`) in Visual Studio.
  2. In the **Package Manager Console**, set the "Default project" to `ignivault.WebAPI`.
  3. Run the command `Update-Database`.
  4. Set `ignivault.WebAPI` as the startup project and run the application (F5).

</details>

---
## ☁️ Deployment

The application is designed to be deployed to **Microsoft Azure**. The process involves creating an **Azure SQL Database** and an **Azure App Service**, and configuring all production secrets in the App Service Configuration.

---
## 📜 License

This project is licensed under the MIT License.
