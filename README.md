# UBC-2020 Blueprint

This document is a project blueprint for the Associated Engineering Resource Management System project. The purpose of the doucment is to provide the teams with a starting point, and some guidance on the project structure.

## Prerequisites

You'll need the following applications installed on your machine before getting started:

| Application                                                                          | Version                     | Usage            |
| ------------------------------------------------------------------------------------ | --------------------------- | ---------------- |
| [git](https://git-scm.com/downloads)                                                 | Any                         | for cloning repo |
| [Visual Studio](https://visualstudio.microsoft.com/downloads/)                       | Community Edition or Latest | for service      |
| [SQL Server Express](https://www.microsoft.com/en-ca/download/details.aspx?id=55994) | 14.0.1000.169               | for database     |
| [Node.js/NPM](https://www.npmjs.com/get-npm)                                         | Latest LTS Version          | for client       |

| Account                                                                     | Usage              |
| --------------------------------------------------------------------------- | ------------------ |
| [Microsoft Azure](https://azure.microsoft.com/en-ca/features/azure-portal/) | for authentication |

## Set-up and Configuration Instructions

1. Clone the repo [here](https://dev.azure.com/associated/AE-UBC/_git/cpsc319-2020-winter-blueprint)

2. After the repo is cloned, navigate to the ../cpsc319-2020-winter-blueprint directory

   ```bash
   cd cpsc319-2020-winter-blueprint
   ```

### Authentication

#### Registering a new application on Azure

1. Log into [Microsoft Azure](https://azure.microsoft.com/en-ca/features/azure-portal/)
2. Navigate to ['portal'](https://portal.azure.com/#home)
3. Click on 'Azure Active Directory', click on 'App registrations', click 'New Registration' to create a new application
4. Give the application a name, under 'Supported account types' select 'Accounts in this organizational directory only (... only - Single tenant), under 'Platform configuration (Optional) select 'Client Application (Web, iOS, Android, Desktop+Devices)', then hit 'Register'
5. You should be redirected to the 'Authentication' page of the app your just registered, if not then navigate to the 'Authentication' page of your app
6. Under 'Platform configurations' click on 'Add a platform'
7. In the Configure Web screen, under 'Redirect URIs enter [https://localhost:5001](https://localhost:5001), leave the 'Logout URL' blank, under 'Implicit grant' check 'Access tokens' and 'ID tokens'; hit 'Configure' to save changes
8. After saving there will be a Web dropdown under 'Platform configurations', expand the dropdown and click 'Add URI'; and add [http://localhost:3000](http://localhost:3000) and hit the 'Save' button at the top

### Configure Database and Service

#### Create local database

1. Open SQL Server Management Studio (SSMS)
2. Create a new local database instance (Right-click Databases>New Database...>give Database a name (eg. ResourceMGMT))

#### Prepare the database and populate

1. Navigate to the ../src/svc-dotnetcore3 directory

   ```bash
   cd src/svc-dotnetcore3
   ```

2. Double click on svc-dotnetcore.sln to open up the Visual Studio solution, in solution explorer you will find three projects (Database, Tests, Web.API)
3. Right click on the Database project and select 'Publish...'
4. In the Publish Database window select 'Edit...'
5. Select the 'Browse' tab and enter the server name (same as the one in SSMS), using the dropdown menu, select the database instance in (SMSS), click on 'Test Connection' and ensure the connection is successful, click 'OK' once the test connection is successful
6. In the Publish Database window click on 'Advanced...'
7. In the 'General' tab under the 'Deployment Behavior' section, 'Uncheck Block incremental deployment if data loss might occur' and click 'OK' to confirm settings
8. In the Publish Database window, click on 'Save Profile As...', enter the File name as Database.DEV.publish.xml, ensure the file path pointing at the Database project in Visual Studios before hitting 'Save'
9. In the Publish Database window click 'Publish'
10. In the Database project open up the seed.sql file (scripts>seed.sql) and copy the SQL statements
11. Navigate to (SMSS) and create a 'New Query' in your database instance
12. Paste the SQL statments from the seed.sql and execute the statements to populate the database; some duplicated usernames may not copy over, this is fine

#### Configure the service

1. In the Web.API project, create a copy of appsettings.json and name it appsettings.Development.json
2. Edit appsettings.Development.json, in the ConnectionString replace the Data Source with the server name (same server name in SMSS), replace the Initial Catalog with the database name (same as database name in SMSS), and add [http://localhost:3000](http://localhost:3000) to AllowedOrigins, also populate the AzureAd Tenant and ClientId with the ID's from the app registration on azure, and save the changes to the file

   ```json
   "ConnectionString": "Data Source=server_name; Initial Catalog=database_name; Integrated Security=True;"
   "AllowedOrigins": "http://localhost:3000",
   "AzureAd": {
     "Instance": "https://login.microsoftonline.com/",
     "TenantId": "<Directory (tenant) Id>",
     "ClientId": "<Application (client) Id>"
   }
   ```

3. Run the Web.API project (default might be IIS Express in the debugging options), click on the (green play button button dropdown and change to Web.API); to see more detail about the port settings go to the Web.API project and go to properties > launchSettings.json. Feel free to make any changes to the ports but make sure the changes are echoed in the Azure Active Directory ports as well.
4. Visit [https://localhost:5001/users/](https://localhost:5001/users/), [https://localhost:5001/projects/](https://localhost:5001/projects/), [https://localhost:5001/locations/](https://localhost:5001/locations/) you should be getting a 401 unauthorized, this is expected
5. To ensure the database is connected, navigate to the LocationsController (Web.API>Controllers>LocationControllers) and comment out the [Authorize] attribute, now run the Web.API project again and visit [https://localhost:5001/locations/](https://localhost:5001/locations/), you should be getting the locations data back; the same can be done to test for the other controllers

### Configure the Client

1. Navigate to ../cpsc319-2020-winter-blueprint/src/ui-react-client
2. Run npm install

   ```bash
   npm install
   ```

3. Open up ../ui-react-client using a text editor or IDE of your choice
4. Add an .env file to the root ui-react-client and paste in the following (fill in the application and directory id from your app in azure, fill in the production service url before pushing to production)

   ```txt
   REACT_APP_SVC_ROOT = <Your production service URL>

   REACT_APP_CLIENT_ID = <Application (Client) Id>
   REACT_APP_TENANT_ID = <Directory (Tenant) Id>
   ```

5. Make a copy of the .env file and rename it to .env.development, paste in the following (fill in the application and directory id from your app in azure)

   ```txt
   REACT_APP_SVC_ROOT = https://localhost:5001/

   REACT_APP_CLIENT_ID = <Application (Client) Id>
   REACT_APP_TENANT_ID = <Directory (Tenant) Id>
   ```

6. Run npm start to run the client application

   ```bash
   npm start
   ```

   You should be prompted with a login page, and after logging in you should have access to the application. The Users, Projects, and Locations navigation should be giving a failedError: Network Error if your service isn't running

### Run Full Stack (service and client)

1. Run the service Web.API project in Visual Studio
2. Run the front end, navigate to localhost:3000 if browser does not open page up on its own

   ```bash
   npm start
   ```

3. Will be prompted with a microsoft login / permissions page, enter the necessary login credentials (if you're already logged in then it will take you directly to the main page of the application)
4. Click on the [http://localhost:3000/users](http://localhost:3000/users), [http://localhost:3000/projects](http://localhost:3000/projects), and [http://localhost:3000/locations](http://localhost:3000/locations) navigation links and you should be getting data back on the pages

- note: the projects and users endpoints may take longer to load

### Tests

#### Service

1. Navigate to the 'Test Explorer'
2. Select test(s) to run

#### Client

1. Type npm test to run all tests
