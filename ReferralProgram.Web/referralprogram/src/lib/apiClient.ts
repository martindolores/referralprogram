import { createClient } from "@hey-api/client-fetch";

// Configure the API client
// This will be used by the generated SDK after running `npm run codegen`
export const apiClient = createClient({
  baseUrl: import.meta.env.VITE_API_URL || "", // Uses Vite proxy in development
});

// Export for use in components
export default apiClient;
