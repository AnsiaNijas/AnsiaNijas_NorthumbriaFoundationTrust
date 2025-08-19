AnsiaNijas_NorthumbriaFoundationTrust

This project contains an  Automation Test Suite  for testing the  Search functionality  of the [Northumbria NHS website](https://www.northumbria.nhs.uk/).  

   📖 User Story
As a Northumbria foundation user,  
I want to be able to perform a search for information regarding  Quality and safety ,  
So that I can view the relevant section on the website.

---

   🛠️ Tech Stack
-  Language:  C  (.NET 8)  
-  Framework:  Reqnroll (Cucumber BDD with Gherkin)  
-  Automation Tool:  Playwright  
-  Test Runner:  NUnit  
-  Browsers:  Chrome & Firefox  

---

   🚀 How to Run

    Prerequisites
- Install [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)  
- Install [Node.js](https://nodejs.org/)  
- Install Playwright browsers:  
  
  pwsh bin/Debug/net8.0/playwright.ps1 install

How to run from command prompt

1. Navigate to the project folder run from powershell

cd AnsiaNijas_NorthumbriaFoundationTrust

2. Restore dependencies

dotnet restore

3. Build the project

dotnet build

4. Run tests in a single browser

Run in Chrome (default):

dotnet test


Run in Firefox:

dotnet test -- TestRunParameters.Parameter(name="Browser", value="firefox")

Run tests in cross-browser mode
cd ..
.\run-crossbrowser.ps1

