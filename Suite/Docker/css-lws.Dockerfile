FROM node:20 AS build
ARG CSS_SHA=768de499e5e3ba19fefd0f9a705e68414aea8580
WORKDIR /app
RUN git init \
 && git fetch --depth 1 https://github.com/jeswr/CommunitySolidServer.git $CSS_SHA \
 && git checkout FETCH_HEAD
RUN npm ci --unsafe-perm && npm run build

FROM node:20
WORKDIR /app
COPY --from=build /app/package.json ./package.json
COPY --from=build /app/bin ./bin
COPY --from=build /app/config ./config
COPY --from=build /app/dist ./dist
COPY --from=build /app/node_modules ./node_modules
COPY --from=build /app/templates ./templates
RUN npm install --no-save @opentelemetry/api @opentelemetry/auto-instrumentations-node
ENV NODE_OPTIONS="--require @opentelemetry/auto-instrumentations-node/register"
COPY css-lws-open.json /config/lws-open.json
ENTRYPOINT ["node", "bin/server.js"]
