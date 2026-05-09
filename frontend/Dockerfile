# syntax=docker/dockerfile:1.7

# Build stage
FROM node:22-alpine AS build
WORKDIR /app

COPY package*.json ./
RUN npm ci

COPY . ./

# Empty default — bake in relative URLs so nginx can proxy /api to the backend.
# Override at build time with --build-arg VITE_API_URL=https://your.api when
# the frontend is hosted separately.
ARG VITE_API_URL=
ENV VITE_API_URL=${VITE_API_URL}

RUN npm run build

# Runtime stage
FROM nginx:alpine
COPY --from=build /app/dist /usr/share/nginx/html
COPY nginx.conf /etc/nginx/conf.d/default.conf

EXPOSE 80
