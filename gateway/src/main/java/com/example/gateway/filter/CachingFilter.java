package com.example.gateway.filter;

import org.reactivestreams.Publisher;
import org.springframework.cache.CacheManager;
import org.springframework.core.io.buffer.DataBuffer;
import org.springframework.core.io.buffer.DataBufferUtils;
import org.springframework.http.HttpHeaders;
import org.springframework.http.HttpStatus;
import org.springframework.http.server.reactive.ServerHttpResponseDecorator;
import org.springframework.stereotype.Component;
import org.springframework.web.server.ServerWebExchange;
import org.springframework.web.server.WebFilter;
import org.springframework.web.server.WebFilterChain;
import reactor.core.publisher.Flux;
import reactor.core.publisher.Mono;

@Component
public class CachingFilter implements WebFilter {

    private final CacheManager cacheManager;

    public CachingFilter(CacheManager cacheManager) {
        this.cacheManager = cacheManager;
    }

    @Override
    public Mono<Void> filter(ServerWebExchange exchange, WebFilterChain chain) {
        String path = exchange.getRequest().getURI().getPath();

        if (path.startsWith("/user/profile") || path.startsWith("/recipe")) {
            String cacheKey = path;

            String cachedResponse = cacheManager.getCache("gateway-cache").get(cacheKey, String.class);
            if (cachedResponse != null) {
                exchange.getResponse().getHeaders().add(HttpHeaders.CONTENT_TYPE, "application/json");
                exchange.getResponse().setStatusCode(HttpStatus.OK);

                byte[] bytes = cachedResponse.getBytes();
                DataBuffer buffer = exchange.getResponse().bufferFactory().wrap(bytes);
                return exchange.getResponse().writeWith(Flux.just(buffer));
            }

            ServerHttpResponseDecorator responseDecorator = new ServerHttpResponseDecorator(exchange.getResponse()) {
                @Override
                public Mono<Void> writeWith(Publisher<? extends DataBuffer> body) {
                    Flux<? extends DataBuffer> fluxBody = Flux.from(body);

                    return super.writeWith(fluxBody.map(dataBuffer -> {
                        byte[] content = new byte[dataBuffer.readableByteCount()];
                        dataBuffer.read(content);
                        DataBufferUtils.release(dataBuffer);

                        cacheManager.getCache("gateway-cache").put(cacheKey, new String(content));
                        return bufferFactory().wrap(content);
                    }));
                }
            };

            return chain.filter(exchange.mutate().response(responseDecorator).build());
        }

        return chain.filter(exchange);
    }
}