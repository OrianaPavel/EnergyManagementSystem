# Use an official Node runtime as a parent image
FROM node:16-alpine

WORKDIR /app

COPY package*.json ./
RUN npm install
RUN apk add --no-cache gettext

COPY . .
RUN npm run build
CMD ["npm", "start"]

