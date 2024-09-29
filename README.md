# Cooking Blog with Recipe Recommendation System

## Application Suitability
---
Complex applications with several distinct components that can be built, deployed, and scaled are best suited for microservices. This method guarantees scalability and flexibility.

_Reasons why this approach is suitable for my idea:_
* **User Authentication:** A separate service to handle user registration, login, and profile management.
* **Recipe Blog Management:** A service that handles adding, editing, retrieving recipes and comment them.
* **Future expansion**: I may include a Recommendation System by the use of external API to gather relevant recipes, which can scale independently from the core blog and authentication functionalities.

_Real-World Example:_
1. Microservices are used by Facebook's architecture to manage several elements such as messaging and authentication. 
2. Amazon uses a specialized microservice to process shipping orders.
3. Spotify uses microservice for user account management.

CookingMaster - cooking blog platform built with microservices, allowing users to share recipes, vote, get real-time feedback, and receive personalized recipe recommendations.
1. User Management Service (C#): Handles user authentication and profiles.
2. Cooking Blog Service (C# with WebSockets): Manages recipe posting, voting, and live feedback.
3. Recipe Recommendation Service (Python): Provides personalized recipe suggestions based on user interactions.
4. Gateway Service (Java): Routes requests and manages service discovery.
5. Service Discovery (Java): Registers and discovers microservices dynamically.

## Service Boundaries
---
**User Service:**
1. Handles user registration and login.
2. Handles user profile management via CRUD operations.
   
**Cooking Blog Service:**
1. Manages recipes via CRUD operations.
2. Interacts with the user service to retrieve author information.
3. Manages real-time comments using WebSockets.
   
**Gateway:**
1. Entry point for external HTTP requests.
2. Handles routing, security, and load balancing.
   
**Service Discovery:**
1. Manages service registration and discovery using gRPC.

_**System Architecture Diagram:**_

![PAD (1) drawio](https://github.com/user-attachments/assets/5f1601d3-5736-4f82-81a3-13857ad35ed2)


## Technology Stack and Communication Patterns
---
**Technology Stac**

> _**User Service:**_
> 
> C#, .NET Core Web API, EntityFrameworkCode + AspNetCore Identity ( for authentification )
> 
> PostgreSQL

> _**Cooking Blog Service:**_
> 
> C#, .NET Core Web API, WebSocket
> 
> PostgreSQL

> _**Gateway:**_
> 
> Java, Spring Boot, Spring Cloud Gateway

> _**Service Discovery:**_
> 
> Java, Spring Boot, Spring Cloud (Eureka)

**Communication Patterns**

> _**RESTful APIs:**_
> 
> For HTTP communication between external clients and services.

> _**WebSocket:**_
> 
> For real-time comment functionality.

> _**gRPC:**_
> 
> For communication between Gateway and internal services.

## Data Management
---
**Database:**

Each microservice will have its own database for data isolation and independence. I will use PostgreSQL for both microservices.
User Service Database - will store user credentials and profile related data.
Cooking Blog Service Database - will store recipe details such as title, description, author id, and comments.

**API Endpoints and Data Format:**
---
**User Service Endpoints**
1. `POST` /register ( Registers a new user ):
  * Request (JSON):
  ```json
  {
    "email": "string",
    "password": "string"
  }
  ```
  * Response (JSON):
    
  `201` Created:
  ```json
  {
    "message": "User successfully registered."
  }
  ```
  `400` Bad Request:
  ```json
  {
    "message": "Validation error."
  }
  ```
2. `POST` /login ( Authenticates a user and provides an access Token ):
  * Request (JSON):
  ```json
  {
    "email": "string",
    "password": "string"
  }
  ```
  * Response (JSON):
  
  `200` OK:
  ```json
   {
      "token": "string"
   }
  ```
  `401` Unauthorized:
  ```json
    {
      "message": " Invalid credentials."
    }
  ```
3. `PUT` /profile/edit/{id} ( Edits user profile ):
  * Request (query parameters + access Token in Header + JSON)
  ```json
    {
    "username": "string",
    "email": "string",
    "password": "string"
    }
  ```
  * Response (JSON):
    
  `200` OK:
  ```json
   {
      "message": "Profile updated."
   }
  ```
  `401` Unauthorized:
  ```json
    {
      "message": " Invalid JWT token."
    }
  ```
4. `GET` /profile/{id} (Fetches user profile):
  * Request (query parameters + jwt token in Header)
  * Response (JSON):
  
  `200` OK:
  ```json
    {
    "username": "string",
    "email": "string"
    }
  ```
  `401` Unauthorized:
  ```json
    {
      "message": " Invalid JWT token."
    }
  ```
**Cooking Blog Service Endpoints**
1. `GET` /recipes ( Retrieves all recipes ):
  * Request (jwt token in Header)
  * Response (JSON):
    
  `200` OK:
  ```json
  [
    {
      "id": "int",
      "title": "string",
      "author": {
        "username": "johndoe",
        "bio": "string",
        "email": "string"
      },
      "description": "string"
    }
  ]
  ```
  `401` Unauthorized:
  ```json
    {
      "message": " Invalid JWT token."
    }
  ```
2. `GET` /recipes/{id} ( Retrieves a specific recipe ):
  * Request (query parameters + jwt token in Header)
  * Response (JSON):
  
  `200` OK:
  ```json
    {
      "id": "int",
      "title": "string",
      "author": {
        "username": "johndoe",
        "bio": "string",
        "email": "string"
      },
      "description": "string"
    }
  ```
  `401` Unauthorized:
  ```json
    {
      "message": " Invalid JWT token."
    }
  ```
  `404` Not Found:
  ```json
    {
      "message": " Recipe not found."
    }
  ```
3. `POST` /recipes/post/{ownerId} ( Adds a new recipe ):
  * Request (query parameters + jwt token in Header + JSON):
  ```json
  {
    "title": "string",
    "description": "Layered pasta with meat sauce"
  }
  ```
  * Response (JSON):
  
  `201` Created:
  ```json
    {
      "message": " Recipe added."
    }
  ```
  `401` Unauthorized:
  ```json
    {
      "message": " Invalid JWT token."
    }
  ```
4. `PUT` /recipes/edit/{recipeId} ( Edits an existing recipe ):
  * Request (query parameters + jwt token in Header + JSON):
  ```json
  {
    "title": "string",
    "description": "string"
  }
  ```
  * Response (JSON):
  
  `200` OK:
  ```json
    {
      "id": "int",
      "title": "string",
      "description": "string"
    }
  ```
  `401` Unauthorized:
  ```json
    {
      "message": " Invalid JWT token."
    }
  ```
  `404` Not Found:
  ```json
    {
      "message": " Recipe not found."
    }
  ```

5. WebSocket /recipe/comment/{recipeId} (Opens a WebSocket connection for real-time commenting on a recipe ):
  * Request (JWT token as a query parameter + JSON):
    The client will open a WebSocket connection to /comment in the Cooking Blog Service, passing the JWT token as a query parameter
    (e.g., ws://cookingblog.com/comment?token=JWT-TOKEN).
  ```json
  {
    "comment": "string"
  }
  ```

## Deployment and Scaling
---
Because Docker is being used to containerize both services, consistency can be maintained throughout development, testing, and production environments.
Incorporates load balancing into each service to split requests among instances fairly, increasing responsiveness and dependability.
Endpoints are used to continuously monitor the health and performance of services, and automatic scaling is used to keep performance levels at their best.
