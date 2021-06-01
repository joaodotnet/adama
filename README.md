# Description
This is a monolithic e-commerce application using ASP.NET 5

# Patterns / Technologies using
* Clean Architecture by (ardalis)
* EF Repository Pattern;
* Specification Pattern;
* (Some) DDD Patterns
*	Razor Pages;
* Entity Framework Core;
*	Microsoft AspNet Core Identity
*	AutoMapper;

# Additional
It also implements authentication against Sage One API using an custom refresh tokens mechanism

# How to run on development enviroment
1. Install Docker 
2. Pull Mariadb image on docker and run it
* `docker pull mariadb:latest`
* `docker run --name=mariadb -e MYSQL_ROOT_PASSWORD=1234 -d -p 3306:3306 mariadb:latest`
3. Create databases
* `docker exec -it mariadb bash -l`
* `mysql -u root -p`
* `CREATE database js_damashopweb CHARACTER SET utf8mb4;`
* `CREATE database js_identity CHARACTER SET utf8mb4;`
* `exit;`
4. Add User Secrets files to both Web projects and write connection strings
* `{
    "ConnectionStrings:DamaShopConnection": "server=127.0.0.1;uid=root;pwd=1234;database=js_damashopweb",
    "ConnectionStrings:IdentityConnection": "server=127.0.0.1;uid=root;pwd=1234;database=js_identity"
}`
5. Run Migrations on src\Infrastructure folder
* `dotnet ef database update -c DamaContext -s ..\Web\DamaWeb\`
* `dotnet ef database update -c AppIdentityDbContext -s ..\Web\DamaWeb\`
6. Run in src\Web\DamaWeb or src\Web\Backoffice folder
* `dotnet run`
7. For Backoffice use bo@damanojornal.com with Pass@word1