# Stage 1: Build the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

COPY . .

# Restore and build the project
RUN dotnet restore
RUN dotnet publish -c Development -o out

RUN dotnet tool install --global dotnet-ef

ENV PATH="$PATH:/root/.dotnet/tools"

CMD ["export", "PATH=$PATH:/root/.dotnet/tools"]

CMD ["dotnet", "ef", "database", "update", "--no-build"]


FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# built files from the build stage
COPY --from=build /app/out .

EXPOSE ${PORT} 

#  entry point for the container
ENTRYPOINT ["dotnet", "CookingBlogService.dll"]