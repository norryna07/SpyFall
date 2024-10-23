![](banner.png)
# SpyFall - [website](http://spyfall.nodnor.com)

## Table of contents
1. [Description](#description)
2. [Technologies](#technologies)
3. [MVC Architecture](#mvc-architecture)
4. [Database](#database)
5. [Usage](#usage)
6. [License](#license)

## Description
**SpyFall** is an MVC web application designed to provide users with a straightforward interface for scanning ports on a specified IP address. Upon initiating a scan, the application identifies open ports and returns detailed information about the services running on those ports. Additionally, SpyFall includes functionality that allows users to exploit certain services, providing an educational tool for understanding network security vulnerabilities.

## Technologies
- **Frontend**: 
  - HTML
  - CSS
  - JavaScript
  - Bootstrap
- **Backend**:
  - C#
  - .NET Core
  - ASP.NET
- **Database**:
  - Microsoft SQL 
  - PostgreSQL

## MVC Architecture

### Models
#### Common Service
- this is a table from the database that will contain the well known services for each port
- for example: `22 - ssh` will be a pair from this class

#### Service Verif
- also a table from the database that will contain
  - the request format for a series of services
    - the first characters
    - the whole message
    - the length of it
  - the response format 
    - what should the response contains
    - with what the response should start
    - the minim length of the response

#### Port Scan Result
- this class will be used to store the result after scanning the ports
- will contain information about the **number port**, the **status of the port** and **the common service open of this port**

#### Deep Scan Result
- this class will be used to store the result of the deep scanning
- will contain information about the **port number**, the **service tested** and **the status of the service**

#### DNS Pair
- if an open DNS port is found we can use this server to make DNS requests
- this class will store the results of these requests

### Views
#### Index
- the main page where the user can **provide** the IP address and the port range and **receive** the scan results

#### DeepScan
- the page where the result of a deep scan will pe displayed

#### DNS
- an interactive page where the user can make DNS request to the port discovered and see the results

#### Privacy
- the page where usage information are displayed
- and where the Responsibility Disclaimer can be seen

### Controller
- a main controller that will handle all the actions from the site
- will be the interface between the **views** and the **services** where the scanning is done

## Database
- the information about the common services and the service verification messages will be stored in a database
- the scripts for **Microsoft SQL** can be find [here](./DatabaseStrips/)
- the scripts for **PostgreSQL** can be find [here](./DatabaseScripsPostgres/)

## Usage
- this application can be access using this website: `spyfall.nodnor.com`
- to run this application locally
  - create a local database using PostgreSQL (or Microsoft SQL)
    - instruction for setup a PostgreSQL server: [on Ubuntu](https://ubuntu.com/server/docs/install-and-configure-postgresql) or [on Windows](https://www.pgadmin.org/)
    - run the scripts in order:  **creates.sql**, **ServiceVerif-inserts.sql**, **CommonServices-inserts.sql** and **CommonServices-updates.sql**
    ```postgres
    \i path/to/a/sql/script
    ```
  - install dotnet on your machine - [from here](https://dotnet.microsoft.com/en-us/download)
  - before running the application go to `appsettings.json` and add in the string connection
    - the name of the database
    - the username
    - the password
  - to run this application 
    - go to in the [SpyFall folder](./SpyFall_project/)
    ```bash
    dotnet run
    ```
    - it possible to need to run this command using **sudo** because the application listen on port 80

## License
This project is licensed under the [MIT License](LICENSE).