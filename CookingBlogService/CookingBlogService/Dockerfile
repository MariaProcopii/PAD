# Stage 1: Build the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy the project files
COPY . .

# ENV ASPNETCORE_ENVIRONMENT=Development

# Restore and build the project
RUN dotnet restore
RUN dotnet publish -c Development -o out

# Install the dotnet-ef tool to run migrations
RUN dotnet tool install --global dotnet-ef

# Make sure the global tools path is in the PATH
ENV PATH="$PATH:/root/.dotnet/tools"

CMD ["export", "PATH=$PATH:/root/.dotnet/tools"]

# Run the migrations
CMD ["dotnet", "ef", "database", "update", "--no-build"]

# Stage 2: Run the application
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Copy the built files from the build stage
COPY --from=build /app/out .
# COPY ./out .  

# Expose the port your app runs on (e.g., 80 for ASP.NET Core)
EXPOSE ${PORT} 

# Set the entry point for the container to run the application
ENTRYPOINT ["dotnet", "CookingBlogService.dll"]