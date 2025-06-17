# CentraliaStore Configuration Information

## Instructions
- Clone the CentraliaStore project from GitHub using Visual Studio.
- Create a fork of the repository on GitHub:
- Navigate to the CentraliaStore repo.
- Click "Fork" (top-right).
- Make sure you're based on the development branch. (See Note Below)
- In Visual Studio, create a new branch off of your fork (e.g., feature/my-update).

> 
> ⚠️ In Visual Studio, unclick "Clone only the master branch" so all branches are available.
> 

## Seeding the Admin
Paste the following JSON into the secrets.json file:

```json
{
  "Accounts:AdminEmail": "admin@centraliastore.com",
  "Accounts:AdminPassword": "Admin123!",
  "Accounts:TestUserEmail": "test@centraliastore.com",
  "Accounts:TestUserPassword": "Test123!"
}
```

## Seeding the Database
- In the Package Manager Console, run:

```console
Update-Database
```

## Making Your Changes

### Responding to Issues
See this link for the [issue tracker](https://github.com/CCAppDevs/CentraliaStore/issues).
- Select the issue
- On your own fork resolve the issue
  - Make sure your commit focuses on this issue.
  - Make sure to mark your commit message with the following

  ##### Example
  > fix: Fixed a bug in seed data

- Finish your code, and submit a pull request.

### When Changing the Database
If changes involve the database (e.g., seed data), run the following in your manage package console or terminal

```console
Add-Migration YourMigrationName
Update-Database
```

- Make a commit
- Push your changes to your fork on GitHub.


### Creating a Pull Request
Ensure all code entering the repository is comparing your fork to the development branch. All code missing this requirement will be denied until fixed.
