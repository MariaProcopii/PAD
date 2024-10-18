package com.example.gateway;

import io.github.resilience4j.springboot3.bulkhead.autoconfigure.ThreadPoolBulkheadProperties;
import org.springframework.boot.SpringApplication;
import org.springframework.boot.autoconfigure.SpringBootApplication;
import org.springframework.cloud.client.discovery.EnableDiscoveryClient;
import org.springframework.cloud.gateway.route.RouteLocator;
import org.springframework.cloud.gateway.route.builder.RouteLocatorBuilder;
import org.springframework.context.annotation.Bean;
import org.springframework.context.annotation.Primary;
import org.springframework.http.HttpMethod;
import org.springframework.http.HttpStatus;
import org.springframework.util.unit.DataSize;

import java.time.Duration;

@SpringBootApplication
@EnableDiscoveryClient
public class GatewayApplication {

	public static void main(String[] args) {
		SpringApplication.run(GatewayApplication.class, args);
	}

//	@Bean
//	public RouteLocator routes(RouteLocatorBuilder builder) {
//		String uriCookingBlogService = "http://localhost:5096";
//		String uriUserService = "http://localhost:5236";
//		return builder.routes()
//				.route("caching-cooking-blog-service", r -> r
//						.path("/recipe/**")
//						.filters(f -> f
//								.localResponseCache(Duration.ofMinutes(30), DataSize.parse("500MB")))
//						.uri(uriCookingBlogService))
//				.route("caching-user-service", r -> r
//						.path("/user/profile/**")
//						.filters(f -> f
//								.localResponseCache(Duration.ofMinutes(30), DataSize.parse("500MB")))
//						.uri(uriUserService))
////				.route("cooking-blog-service", r -> r
////						.path("/recipe/**")
////						.filters(f -> f
////								.circuitBreaker(config -> config
////										.setName("myServiceCircuitBreaker")
////										.setFallbackUri("forward:/fallback"))
////								.retry(config -> config
////										.setRetries(3)
////										.setStatuses(HttpStatus.INTERNAL_SERVER_ERROR)
////										.setMethods(HttpMethod.GET, HttpMethod.POST)))
////						.uri(uriCookingBlogService))
////				.route("user-service", r -> r
////						.path("/user/**")
////						.filters(f -> f
////								.circuitBreaker(config -> config
////										.setName("myServiceCircuitBreaker")
////										.setFallbackUri("forward:/fallback"))
////								.retry(config -> config
////										.setRetries(3)
////										.setStatuses(HttpStatus.INTERNAL_SERVER_ERROR)
////										.setMethods(HttpMethod.GET, HttpMethod.POST)))
////						.uri(uriUserService))
//				.build();
//	}

	@Bean
	@Primary
	public ThreadPoolBulkheadProperties threadPoolBulkheadProperties() {
		return new ThreadPoolBulkheadProperties();
	}
}
