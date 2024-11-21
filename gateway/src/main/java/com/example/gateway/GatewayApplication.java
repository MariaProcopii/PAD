package com.example.gateway;

import io.github.resilience4j.springboot3.bulkhead.autoconfigure.ThreadPoolBulkheadProperties;
import org.springframework.boot.SpringApplication;
import org.springframework.boot.autoconfigure.SpringBootApplication;
import org.springframework.boot.autoconfigure.http.HttpMessageConverters;
import org.springframework.cache.annotation.EnableCaching;
import org.springframework.cloud.client.discovery.EnableDiscoveryClient;
import org.springframework.cloud.openfeign.EnableFeignClients;
import org.springframework.context.annotation.Bean;
import org.springframework.context.annotation.Primary;
import org.springframework.http.converter.json.MappingJackson2HttpMessageConverter;

@SpringBootApplication
@EnableDiscoveryClient
@EnableCaching
@EnableFeignClients
public class GatewayApplication {

	public static void main(String[] args) {
		SpringApplication.run(GatewayApplication.class, args);
	}

	@Bean
	@Primary
	public ThreadPoolBulkheadProperties threadPoolBulkheadProperties() {
		return new ThreadPoolBulkheadProperties();
	}

	@Bean
	public HttpMessageConverters httpMessageConverters() {
		return new HttpMessageConverters(new MappingJackson2HttpMessageConverter());
	}
}
