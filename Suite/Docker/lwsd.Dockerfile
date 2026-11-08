FROM node:24
RUN git clone --depth 1 https://github.com/linkedwebstorage/lwsd.git /app
WORKDIR /app
RUN npm install --omit=dev && npm install @opentelemetry/api @opentelemetry/auto-instrumentations-node
ENV NODE_OPTIONS="--require @opentelemetry/auto-instrumentations-node/register"
ENTRYPOINT ["node", "index.js", "--host", "0.0.0.0", "--port", "3126", "--no-auth"]
