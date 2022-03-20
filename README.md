This is a news portal application with basic CRUD options, Authentication and Authorization. Backend soultion is using .NET Core (C#) and Entity framework Core for working with the database where I used CODE FIRST principle. 
Front-end solution is done using design patters.

Prerequisites
You will need:

Microsoft SQL Server Management Studio 2017 (or up) to run the migrations.

Visual Studio 2019 to run the application.

Running the app

On first app run test data will be inserted into tables, so you can test it right away.


Run the app in Visual Studio 2019
Ctrl + F5
Now you can test the application on the UI side!

Overall functionalities
This application has two kinds of users. A public users (registrated and unregistrated readers) and an Administrator user(journalists and editors).
There are multiple restrictions for administration users that are made so the creative process of publishing is following real life rules in journalism. 
(For example for a article to be public it has to be written by a journalist, but it goes public (on front page) only after editor approves it.)


The public unregisterd user has the possibility to read and to search for news, also they can answer the surveys without any kind of authentication or authorization.
The public registerd user has to be authenticated, and it has the posibility to write comments, send tips to the news portal as well as to read and search the content.

The administration users (journalist and editor) has the possibility to perform the same actions as the public user plus he should be able to add/edit content that will be published on the portal, and editor has haccess to the reporting module.
