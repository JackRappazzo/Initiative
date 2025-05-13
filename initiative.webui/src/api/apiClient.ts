// src/api/apiClient.ts
import axios, { AxiosInstance, AxiosResponse, InternalAxiosRequestConfig } from "axios";

export class ApiClient {
  private client: AxiosInstance;

  constructor() {
    this.client = axios.create({
      baseURL: "https://localhost:7034/api", // Replace with your API base URL
      headers: {
        "Content-Type": "application/json",
      },
    });

    // Request Interceptor for adding token to every request
    this.client.interceptors.request.use(this.addAuthHeader);

    // Response Interceptor to handle errors globally
    this.client.interceptors.response.use(this.handleResponse, this.handleError);
  }

   private addAuthHeader = (config: InternalAxiosRequestConfig): InternalAxiosRequestConfig => {
        const token = localStorage.getItem("token");
        if (token && config.headers) {
        config.headers.Authorization = `Bearer ${token}`;
        }
        return config;
  };

  private handleResponse(response: AxiosResponse): AxiosResponse {
    return response;
  }

  private handleError(error: any): Promise<any> {
    
    if (error.response && error.response.status === 301) {
      // Handle unauthorized error, maybe redirect to login
      console.error("Unauthorized request", error);
    } else {
      // Handle other types of errors
      console.error("API Error", error);
    }
    return Promise.reject(error);
  }

  public post<T>(url: string, data: any): Promise<T> {
    return this.client.post(url, data, {
      headers: {'Content-Type': 'application/json' },
      withCredentials: true,
    });
  }

  public get<T>(url: string): Promise<T> {
    return this.client.get(url);
  }

  // Add other HTTP methods as needed (put, delete, etc.)
}

export const apiClient = new ApiClient();