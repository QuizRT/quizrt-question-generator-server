# QuizRT Server

This project was generated with [ASP NET CORE](https://github.com/aspnet/AspNetCore).

## Development server

Run `dotnet run` inside directory QuizRTapi. Navigate to `http://localhost:5000/`.

## Running unit tests

Navigate to QuizRTapi.Tests -
Run `dotnet test` to execute the unit tests via [CIRCLE CI](https://github.com/circleci).

## QuizRT Server Description
This server feeds enormous questions and options to a Real Time Quiz Game. API is written in .NET Core, backed by mongoDB documents to store our templates, questions generated on certain category (say cricket) under a topic (say sports) on top of these templates, also stores relevant wrong answers one among is corect based on these category and topic (crickets in sports). This model uses data source as Wikidata and query language SparQL to hit there API endpoint and collects raw data later using these and provided template to frame hundred thousands of questions and join each of them with thousands of alternating options that are different every time.
Other tools used circleci and docker.
