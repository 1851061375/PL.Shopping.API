dotnet ef migrations add InitialMigrations --project .././Migrators/Migrators.MSSQL/ --context TenantDbContext -o Migrations/Tenant
dotnet ef migrations add InitialMigrations --project .././Migrators/Migrators.MSSQL/ --context ApplicationDbContext -o Migrations/Application
dotnet ef migrations script {FROM_MIGRATION} {TO_MiGRATION} --context ApplicationDbContext -o ./script.sql
cd src/Host
dotnet build
dotnet run
https://localhost:5001/swagger/index.html
dotnet ef migrations add Fix_3 --project .././Migrators/Migrators.MSSQL/ --context ApplicationDbContext -o Migrations/Application

