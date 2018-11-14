# FROM microsoft/dotnet:sdk AS build-env
# WORKDIR /app

# # Copy csproj and restore as distinct layers
# COPY *.csproj ./
# RUN dotnet restore

# # # RUN dotnet build
# # EXPOSE 80/tcp
# # FROM microsoft/mssql-server-linux
# # RUN dotnet ef database update
# # RUN dotnet run --server.urls http://*:80

# # Copy everything else and build
# COPY . ./
# RUN dotnet publish -c Release -o out

# # Build runtime image
# FROM microsoft/dotnet:aspnetcore-runtime
# WORKDIR /app
# COPY --from=build-env /app/out .
# ENTRYPOINT ["dotnet", "QuizRTapi.dll"]

#############

# # FROM microsoft/aspnetcore-build:lts

FROM microsoft/dotnet
COPY . /app
WORKDIR /app
RUN ["dotnet", "restore"]
RUN ["dotnet", "build"]
# EXPOSE 80/tcp
RUN chmod +x ./entrypoint.sh
CMD /bin/bash ./entrypoint.sh

##########################

# FROM microsoft/dotnet:runtime

# WORKDIR /dotnetapp
# COPY out .
# ENTRYPOINT ["dotnet", "QuizRTapi.dll"]