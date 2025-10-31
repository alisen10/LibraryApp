# 📚 Library App

This project is a **Library Management System** built using **ASP.NET Core MVC** and **PostgreSQL**.  
It provides different interfaces and functionalities based on user roles:  
**Library User**, **Library Officer**, and **System Admin**.

---

## 🚀 Features

### 👤 Authentication & Security
- Users can **register** and **log in** to the system.
- Passwords are securely stored using **encryption**.
- All users can update their passwords via the **Profile Page**.

---

## 👥 User Roles

### 📘 Library User
- Can **browse** books by category and **search** by name on the Home page.  
- On the **Books** page, can **borrow** available books.  
- On the **Profile** page, can view their **borrowed** and **returned** books.

---

### 🧾 Library Officer
- Has the same **Home page** as Library User.  
- On the **Books** page:
  - Can **add** or **remove** books.
  - Can **increase or decrease** book quantities.
  - Can **loan** a book to a user.
- On the **Users** page:
  - Can **change a user's type** (e.g., from *Library User* to *Library Officer* or vice versa).
- On the **Borrowed Books** page:
  - Can **view all borrowed books** and their statuses.

---

### 🛠️ System Admin
- Has **full visibility** across the system.
- Can **manage user roles** (promote/demote users).
- Cannot directly interact with books (view-only access).

---

## 🧠 Architecture Overview

The project follows a **Layered Architecture** with four layers:

- **Business:** Handles business logic and services.  
- **DataAccess:** Manages repositories and database operations.  
- **Models:** Defines entity classes.  
- **Web:** Contains MVC controllers, views, and UI components.


