@echo off

start cmd /k "cd src\backend\Lifelog\Peace.Lifelog.SecurityWebService && dotnet run"

start cmd /k "cd src\backend\Lifelog\Peace.Lifelog.UserManagementWebService && dotnet run"

start cmd /k "cd src\backend\Lifelog\Peace.Lifelog.LLIWebService && dotnet run"

start cmd /k "cd src\backend\Lifelog\Peace.Lifelog.PersonalNoteWebService && dotnet run"

start cmd /k "cd src\backend\Lifelog\Peace.Lifelog.MapsWebService && dotnet run"

start cmd /k "cd src\backend\Lifelog\Peace.Lifelog.LocationRecommendationWebService && dotnet run"