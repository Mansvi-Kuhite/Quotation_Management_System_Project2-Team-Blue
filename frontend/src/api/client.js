import axios from "axios";

const explicitBase = import.meta.env.VITE_API_BASE_URL;
const candidateBaseUrls = explicitBase
  ? [explicitBase]
  : [
      // "/api",
      "http://localhost:5000/api",
      "http://localhost:5001/api",
      // "http://127.0.0.1:5001/api",
      // "http://localhost:5283/api",
      // "http://127.0.0.1:5283/api"
    ];

let activeBaseIndex = 0;

const apiClient = axios.create({
  baseURL: candidateBaseUrls[activeBaseIndex],
  timeout: 30000
});

apiClient.interceptors.request.use((config) => {
  if (!explicitBase) {
    config.baseURL = candidateBaseUrls[activeBaseIndex];
  }
  const token = localStorage.getItem("qms_token");
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

apiClient.interceptors.response.use(
  (response) => response,
  async (error) => {
    const originalRequest = error.config || {};
    const isNetworkError = !error.response;
    const currentAttempt = Number(originalRequest.__candidateAttempt || 0);
    const canRetry =
      isNetworkError && !explicitBase && currentAttempt < candidateBaseUrls.length - 1;

    if (canRetry) {
      const nextAttempt = currentAttempt + 1;
      activeBaseIndex = nextAttempt;
      originalRequest.__candidateAttempt = nextAttempt;
      originalRequest.baseURL = candidateBaseUrls[nextAttempt];
      return apiClient(originalRequest);
    }

    if (isNetworkError && !explicitBase) {
      error.userHint = `Cannot reach backend. Tried: ${candidateBaseUrls.join(" , ")}`;
    }

    return Promise.reject(error);
  }
);

export default apiClient;
