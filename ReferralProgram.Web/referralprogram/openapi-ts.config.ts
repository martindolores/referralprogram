import { defineConfig } from "@hey-api/openapi-ts";

export default defineConfig({
  input: "https://localhost:7294/swagger/v1/swagger.json",
  output: {
    path: "src/api",
    format: "prettier",
  },
  plugins: [
    "@hey-api/typescript",
    "@hey-api/sdk",
    {
      name: "@hey-api/sdk",
      asClass: true,
    },
    "@tanstack/react-query",
  ],
  client: "@hey-api/client-fetch",
});
