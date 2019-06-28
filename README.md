# BangazonAPI

Welcome to the Bangazon Platform API

## Getting Started

These instructions will get you a copy of the project up and running on your local machine

### Installing

First, you'll need to clone down the repo into a directory. Open your terminal and enter

```
git clone git@github.com:grumpy-rainbows/BangazonAPI.git
```

You'll need to create a database and also, add data. We used Azure, we recommend using either Azure or SSMS and to run the script to create the database [Official Banagazon SQL](./bangazon.sql).

The next thing to do is add data to each of the tables. We've provided you with the following script [Bangazon Data SQL](./data.sql).


After that, open up your editor which we prefer to be Visual Studio, through the terminal with the command

```
cd BangazonAPI/BangazonAPI
```

```
start BangazonApi.csproj
```

Now, you'll be taken to Visual Studio with the project opened up. The next thing you'll want to do it start the application and Run the program. On the upper middle part of the toolbar, you'll see a green arrow and `IIS Express`, click on the dropdown button and choose `BangazonAPI`. This will run the program and automatically open up a window to see our API. If for some chance you are not directed, simply go to your browser and enter in 

```
http://localhost:5000
```