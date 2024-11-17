.PHONY: all build run clean

all: run 

build:
	@echo "Building the CookingBlogService"
	@cd ./CookingBlogService/CookingBlogService && dotnet restore && dotnet publish -c Development -o out
	@echo "Done"
	@echo "Building the UserService"
	@cd ./UserService/UserService && dotnet restore && dotnet publish -c Development -o out
	@echo "Done"

migrations:
	@echo "Running Migrations"
	@export PATH="$PATH:/root/.dotnet/tools"
	@echo "Migrating for UserService"
	@cd ./UserService/UserService && dotnet ef database update --no-build --configuration Development
	@echo "Done"
	@echo "Migrating for CookingBlogService"
	@cd ./CookingBlogService/CookingBlogService && dotnet ef database update --no-build --configuration Development
	@echo "Done"

run:
	@echo "Running tests"
	@dotnet test ./UserService/UnitTests/UnitTests.csproj
	@echo "Launching the containers. Press Ctrl+C to quit."
	@docker-compose up

clean:
	@echo "Removing all the artifacts"
	@rm -rfv CookingBlogService/CookingBlogService/out CookingBlogService/CookingBlogService/bin CookingBlogService/CookingBlogService/obj
	@rm -rfv UserService/UserService/out UserService/UserService/obj UserService/UserService/bin
	@echo "Done"
	@echo "Removing all the containers"
	@docker-compose down
	@echo "Done"
