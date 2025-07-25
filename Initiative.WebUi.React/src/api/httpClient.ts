import axios, {
  AxiosInstance,
  AxiosRequestConfig,
  AxiosResponse,
  AxiosError,
  InternalAxiosRequestConfig
} from 'axios';
import { config } from '../config/environment';

export class HttpClient {
  private static instance:HttpClient;

  private api: AxiosInstance;
  private isRefreshing: boolean = false;
  private failedQueue: Array<{
    resolve: (value?: unknown) => void;
    reject: (reason?: unknown) => void;
  }> = [];

public static GetInstance() {
  if(HttpClient.instance == null)
  {
    HttpClient.instance = new HttpClient(config.apiBaseUrl);
  }
  return HttpClient.instance;
}

  private constructor(baseURL: string) {
    this.api = axios.create({
      baseURL,
      withCredentials: true, // for refresh cookies
    });

    this.setupInterceptors();
  }

  private getAccessToken(): string | null {
    return localStorage.getItem('token');
  }

  public setAccessToken(token: string) {
    localStorage.setItem('token', token);
  }

  private clearAccessToken() {
    localStorage.removeItem('token');
  }

  private setupInterceptors() {
    // Attach JWT to request
    this.api.interceptors.request.use((config: InternalAxiosRequestConfig) => {
      const token = this.getAccessToken();
      if (token && config.headers) {
        config.headers['Authorization'] = `Bearer ${token}`;
      }
      return config;
    });

    // Handle 401 and refresh
    this.api.interceptors.response.use(
      (response: AxiosResponse) => response,
      async (error: AxiosError) => {
        const originalRequest = error.config as AxiosRequestConfig & { _retry?: boolean };

        if (error.response?.status === 401 && !originalRequest._retry) {
          if (this.isRefreshing) {
            return new Promise((resolve, reject) => {
              this.failedQueue.push({ resolve, reject });
            }).then((token) => {
              if (originalRequest.headers && token) {
                originalRequest.headers['Authorization'] = `Bearer ${token}`;
              }
              return this.api(originalRequest);
            });
          }

          originalRequest._retry = true;
          this.isRefreshing = true;

          try {
            const refreshRes = await axios.post<{ accessToken: string }>(
              `${this.api.defaults.baseURL}/admin/refresh`,
              {},
              { withCredentials: true }
            );

            const newToken = refreshRes.data.accessToken;
            this.setAccessToken(newToken);
            this.processQueue(null, newToken);

            if (originalRequest.headers) {
              originalRequest.headers['Authorization'] = `Bearer ${newToken}`;
            }

            return this.api(originalRequest);
          } catch (refreshError) {
            this.processQueue(refreshError, null);
            this.clearAccessToken();
            return Promise.reject(refreshError);
          } finally {
            this.isRefreshing = false;
          }
        }

        return Promise.reject(error);
      }
    );
  }

  private processQueue(error: unknown, token: string | null = null) {
    this.failedQueue.forEach(prom => {
      if (error) {
        prom.reject(error);
      } else {
        prom.resolve(token);
      }
    });
    this.failedQueue = [];
  }

  // Generic request methods (optional convenience)
  public async get<T>(url: string): Promise<T> {
    return (await this.api.get(url, { headers: {'Content-Type': 'application/json' , withCredentials: true}})).data;
  }

  public async post<T>(url: string, data: any): Promise<T> {
    return (await this.api.post(url, data, {
      headers: {'Content-Type': 'application/json' },
      withCredentials: true,
    })).data;
  }

  public async put<T>(url: string, data: any): Promise<T> {
    return (await this.api.put(url, data, {
      headers: {'Content-Type': 'application/json' },
      withCredentials: true,
    })).data;
  }

  // Or expose full Axios instance
  public get instance(): AxiosInstance {
    return this.api;
  }
}