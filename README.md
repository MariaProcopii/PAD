# Cooking Blog with Recipe Recommendation System

## Theory
* **Circuit Breaker:** A design pattern that prevents repeated failures in a system by temporarily halting requests, allowing recovery time after multiple reroute attempts.

* **High Availability:** Ensures a service remains consistently operational and accessible to users, reducing downtime.

* **Logging and Monitoring with ELK, Prometheus, and Grafana:** Tools for collecting, visualizing, and analyzing logs and metrics from services, helping monitor performance and diagnose issues.

* **Two-Phase Commit:** A protocol that coordinates changes across multiple databases in two steps, ensuring data consistency.

* **Consistent Hashing for Caching:** A technique to distribute cache data across servers effectively, preventing overload on any one server.

* **Cache High Availability:** Strategies that ensure continuous access to cached data, even if some cache instances fail.

* **Saga Pattern for Long-Running Transactions:** A method for managing transactions across services by coordinating long-running steps to maintain data consistency without using two-phase commits.

* **Database Redundancy and Replication:** The creation of database copies to prevent data loss and ensure quick recovery during failures.

* **Data Warehouse and ETL:** A centralized repository for data from multiple sources, kept up-to-date through ETL (Extract, Transform, Load) processes.

## Application Suitability
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
### User Service:
* Handles user registration and login.
* Handles user profile management via CRUD operations.
   
### Cooking Blog Service:
* Manages recipes via CRUD operations.
* Interacts with the user service to retrieve author information.
* Manages real-time comments using WebSockets.
   
### Gateway:
* Entry point for external HTTP requests.
* Handles routing, security, and load balancing.
   
### Service Discovery:
* Manages service registration and discovery using gRPC.

### System Architecture Diagram:

![PAD (1) drawio](https://github.com/user-attachments/assets/9ab86adb-9c0c-49f5-817a-a0521cb04c60)


## Technology Stack and Communication Patterns
### Technology Stack

_**User Service:**_
* C# 
* .NET Core Web API 
* Eureka Client 
* OpenAPI (Swagger) 
* EntityFrameworkCore + AspNetCore Identity 
* AspNetCore Auth JwtBearer
* PostgreSQL

_**Cooking Blog Service:**_
* C# 
* .NET Core Web API 
* EntityFrameworkCore + AspNetCore Identity ( for authentification ) 
* Eureka Client 
* Redis Client 
* Websocket Client 
* OpenAPI (Swagger)
* PostgreSQL

_**Gateway:**_
* Java, 
* Spring Boot 
* Spring Cloud Gateway 
* Eureka Client 
* Spring Cloud Starter Circuitbreaker Resilience4j 
* Spring Cloud Loadbalancer 
* Spring Boot Starter Test 
* Caffeine (Redis Client)

_**Service Discovery:**_
* Java 
* Spring Boot 
* Spring Cloud (Eureka Server) 
* Spring Boot Starter Test

### Communication Patterns

_**RESTful APIs:**_

For HTTP communication between external clients and services.

_**WebSocket:**_

For real-time comment functionality.

_**gRPC:**_

For communication between microservices.

## Data Management

### Database:

Each microservice will have its own database for data isolation and independence. I will use PostgreSQL for both microservices.
User Service Database - will store user credentials and profile related data.
Cooking Blog Service Database - will store recipe details such as title, description, author id, and comments.

## API Endpoints and Data Format:
### Gateway

1. `GET` /actuator/health (Get the gateway and services availability):
   
    **Response (JSON):**

      `200` OK:
      ```json
      {
        "status": "UP",
        "components": {
          "circuitBreakers": {
            "status": "UNKNOWN"
          },
          "discoveryComposite": {
            "status": "UP",
            "components": {
              "discoveryClient": {
                "status": "UP",
                "details": {
                  "services": [
                    "gateway",
                    "cooking-blog-service",
                    "user-service"
                  ]
                }
              },
              "eureka": {
                "description": "Remote status from Eureka server",
                "status": "UP",
                "details": {
                  "applications": {
                    "GATEWAY": 1,
                    "COOKING-BLOG-SERVICE": 3,
                    "USER-SERVICE": 3
                  }
                }
              }
            }
          },
          "diskSpace": {
            "status": "UP",
            "details": {
              "total": 99637788672,
              "free": 30259027968,
              "threshold": 10485760,
              "path": "/app/.",
              "exists": true
            }
          },
          "ping": {
            "status": "UP"
          },
          "reactiveDiscoveryClients": {
            "status": "UP",
            "components": {
              "Simple Reactive Discovery Client": {
                "status": "UP",
                "details": {
                  "services": []
                }
              },
              "Spring Cloud Eureka Reactive Discovery Client": {
                "status": "UP",
                "details": {
                  "services": [
                    "gateway",
                    "cooking-blog-service",
                    "user-service"
                  ]
                }
              }
            }
          },
          "refreshScope": {
              "status": "UP"
          }
        }
      }
      ```

### Service Discovery

1. `GET` /actuator/health (Get the service discovery availability)

    **Response (JSON):**

      `200` OK:
      ```json
      {
        "status": "UP"
      } 
      ```

### User Service Endpoints

1. `POST` /register (Registers a new user):

    **Request (JSON):**
      ```json
      {
        "email": "string",
        "password": "string"
      }
      ```

    **Response (JSON):**
       
      `200` Ok

      ---
      `400` Bad Request:
      ```json
      {
        "type": "https://tools.ietf.org/html/rfc9110#section-15.5.1",
        "title": "One or more validation errors occurred.",
        "status": 400,
        "errors": {
          "DuplicateUserName": [
            "Username 'irina@gmail.com' is already taken."
          ]
        }
      } 
      ```

2. `POST` /login (Authenticates a user and provides an access Token):

    **Request (JSON):**
      ```json
      {
        "email": "string",
        "password": "string"
      }
      ```

    **Response (JSON):**
      
      `200` OK:
      ```json
      {
        "tokenType": "Bearer",
        "accessToken": "CfDJ8GH0t9zz71xHuyQqndCtnZpENCGpXC5-D61sqFOC-SzLEKMWko4L5qaKcFqXialpMhuYuWltUwNTKmAMwImH0lsLyPZfd8eedBoTvB8DoIBnsNJWMVtgwDwZqiBhjP02ohIyltGHB44Ben_AKO7W8QkLvCRDGKo8tYqz4YYmx-hojT_OgAO-YhTU408rJDz2GL-g7NOEgE19HIZc2j-4lvjPGK0jIN5dvOAVnNsacT0W3RJO88S7FNzxXOSkTe1-UCDxD0tyuHBxn6Cdf70fmikYLxsJrb8Hg_GKp48API9UENSjtSLFTt77f1yfab6DvsvWoMWC3FTPsyFzKcRNFj-ObHHuUhomBhQgrHumb-0SwbCFWaeXcjF_E2Igrc0A7WPNS3kQmp1DHWonDzyTjKRxdP2X1KTHoG_YBsWmFElmwrbBPT0M53MyGmsokrmwXLjXbYgXDHVpHNF4wpmG_pLBHKQi2bGvaqVhcmODWEszclTOSiNI0uofoYIE6osnd1P_bbN6VRbE7cMYK5la6eanKXInldg8wfreWngKjlmdMXcRFMAmgZuB8JrI7F5QadHUnPnUUCIURiqMrSUCDBd8vmA_YNxYZVI9ORouhFCJoN-Wj40cAzEXgTBiPfC-lyM60nSdwhX-5_jpU6_IEW7xKGdyZzTl1m2-FUr7pMvKYuau0GMh0u1x1JP9Aj7mOw",
        "expiresIn": 3600,
        "refreshToken": "CfDJ8GH0t9zz71xHuyQqndCtnZoC_jK7Qbs5nmJzodPZu71xfGaul6BgaWbPVkkorWMe0D27EkfbK9EDOWVLnFg8Z1qir64slUW1FDM2t3LSgydh0do1iHynIIPjXZjHcJqADE0NZD-WUfm86LP2CZvzw8aVczGNFbyRpNZCJXCr-NxKJ1_NvP5SPfd1DLOIdbDgXpWMpqDtye0lvk3uXUE8D3QPGe0uH5UyeUyFXjqIh-XrMzjl0pfIcIqS2Wbb2mkJ-rTl3kU2SN4VbLhGGr-nGayuxI9v3ol2CDgyi2GdzH88VoeYiUt9jM5B0O8Xa_0BJsJyZnAZYl0dTG0Dc4aisMOMjvGj4ATQkwpQYNqmq1BRUpSPXcHlxpuP_stBS5mymj8m_WgGC5LUT7Wit_7wXdWUFd7KZqZgtY2wv_whsSiRE8u__jNVhFbl2Sf0DMyo-xZtGB-ySVw5uCRD1hkvsd8Uv1rBYAezvaocWrGPm_VKNolYbjGMLpPuOmCllNbBkPm-O2Vbr2iW6mN541aHToE09_Fw1EC0hVSa0Y7y5Pdda8ms_fM6orZS9IugTxmW5Xh8Qnu7NgnzHe46K5qq7p_lx0Kbmu1tGVDDIXIrwsQD9m9yiqQolObhoq0e81rOtgD7YCNoIgmDDLkYSxQBk71bqGnxl4TXWwljx6ESI88moW1S-ZNabvG1e1V5CSWP1g"
      }
 
      ```
      ---
      `401` Unauthorized:
      ```json
      {
        "type": "https://tools.ietf.org/html/rfc9110#section-15.5.2",
        "title": "Unauthorized",
        "status": 401,
        "detail": "Failed"
      } 
      ```

3. `PUT` /user/profile/edit/{id} (Edits user profile):

    **Request (query parameters + access Token in Header + JSON):**
    ```json
    {
      "username": "string",
      "email": "string",
      "password": "string"
    }
    ```
    **Response (JSON):**
      
      `200` OK:
      ```json
      {
        "message": "User information updated successfully."
      }
      ```
      ---
      `401` Unauthorized:
      ```json
      {
        "message": " Invalid JWT token."
      }
      ```
      ---
      `400` Bad Request
      ```json
      [
        {
          "code": null,
          "description": "User not found"
        }
      ]
      ```

4. `GET` /user/profile/{id} (Fetches user profile):

    **Request (query parameters + jwt token in Header)**

    **Response (JSON):**
    
      `200` OK:
      ```json
      {
        "username": "string",
        "email": "string"
      }
      ```
      ---
      `401` Unauthorized:
      ```json
      {
        "message": " Invalid JWT token."
      }
      ```

5. `GET` /user/health (Get service health):

    **Response (JSON):**

      `200` OK:
      ```json
      {
        "status": "healthy"
      } 
      ```

### Cooking Blog Service Endpoints

1. `GET` /recipe (Retrieves all recipes):

    **Request (jwt token in Header)**

    **Response (JSON):**
    
    `200` OK:
    ```json
    [
      {
        "title": "NreRecipe",
        "description": "Some useful info",
        "username": "Irinav2",
        "email": "irina@gmail.com"
      }
    ]
    ```
    ---
    `401` Unauthorized:
    ```json
    {
      "message": " Invalid JWT token."
    }
    ```

2. `GET` /recipes/{id} (Retrieves a specific recipe):

    **Request (query parameters + jwt token in Header)**

    **Response (JSON):**
  
    `200` OK:
    ```json
   {
      "id": 1,
      "title": "NreRecipe",
      "author": {
        "username": "Irinav2",
        "email": "irina@gmail.com"
      },
      "description": "Some useful info"
    } 
    ```
    ---
    `401` Unauthorized:
    ```json
    {
      "message": " Invalid JWT token."
    }
    ```
    ---
    `404` Not Found:
    ```json
    {
      "message": " Recipe not found."
    }
    ```

3. `POST` /recipes/post/{ownerId} (Adds a new recipe):

    **Request (query parameters + jwt token in Header + JSON):**
    ```json
    {
      "title": "string",
      "description": "Layered pasta with meat sauce"
    }
    ```

    **Response (JSON):**

    `201` Created:
    ```json
    {
      "message": " Recipe added."
    }
    ```
    ---
    `401` Unauthorized:
    ```json
    {
      "message": " Invalid JWT token."
    }
    ```

4. `PUT` /recipes/edit/{recipeId} (Edits an existing recipe):

    **Request (query parameters + jwt token in Header + JSON):**
    ```json
    {
      "title": "string",
      "description": "string"
    }
    ```

    **Response (JSON):**
    
    `200` OK:
    ```json
    {
      "id": "int",
      "title": "string",
      "description": "string",
      "ownerId": "111ad015-c066-4548-8978-54af75a71c31" 
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

5. `GET` /blog/health (Get the blog status)

    **Response (JSON):**

      `200` OK:
      ```json
      {
        "status": "healthy"
      }
      ```

6. `WebSocket` /ws/connect?room={name} (Opens a WebSocket connection for real-time commenting on a recipe ):

    **Request (JWT token as a query parameter + JSON):**
      The client will open a WebSocket connection to /comment in the Cooking Blog Service. Then a full-duplex communication channel is opened.
    ```
    Message
    > I love this receipe! :3
    ```

## Scaling
The microservices are deployed using docker compose, that takes care of the container orchestration. There are several instances of each microservice, created by the docker compose at the build stage. In the `docker-compose.yml` all the needed configuration for each container is described, i.e. the exposed/mapped port, environment variables, etc.

## Deployment

#### 1. Clone the project
```
git clone git@github.com:MariaProcopii/PAD.git
cd PAD
```

#### 2. Launch the project
There is a `makefile`, that have several helper-commands, such as:
* build -- compiles the binaries of the microservices
* migrations -- seeding of the fresh databases
* run -- runs tests and launches the project
* clean -- removes the compiles binaries (if any) and kills the containers

To launch the project just run the following command.
```
make
```

#### 3. Testing
Firtly, register a new user, in order to create a user. The login to receive the access token. With that token, the user is able to request any available endpoint. Please note that the TTL of the token is small for the demostration purposes, so there is a need of repeated logins.