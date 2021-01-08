CodeName_RestApi

This repository provides a web api for using the words database .NET Standard package.
The rest api is built using target framework of .NET 5.0 with .NET Core Web API template of Visual Studio.

Exposed Features Requests:

(CardController - Only Supports English Words for Now)
GET CardAtIndex
GET CardCount
GET RandomCards
POST Card
PUT UpdateCard
DELETE Card

Dependencies:

WordsDatabase .NET Standard DLL (including MongoDriver)
ASP.NET Core