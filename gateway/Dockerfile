# Stage 1: Build the application using Maven
FROM maven:3.9.4-eclipse-temurin-17 AS build
WORKDIR /app

# Copy the Maven project files to the container
COPY pom.xml ./
COPY src ./src

# Build the project using Maven (creates a jar file)
RUN mvn clean package -DskipTests

# Stage 2: Run the application
FROM openjdk:17-jdk-slim
WORKDIR /app

# Install curl in the slim image
RUN apt-get update && apt-get install -y curl postgresql-client

# Copy the jar file from the previous build stage
COPY --from=build /app/target/*.jar /app/myapp.jar

# Expose the port (adjust if different)
EXPOSE 8080

# Command to run the jar file
CMD ["java", "-jar", "/app/myapp.jar"]
