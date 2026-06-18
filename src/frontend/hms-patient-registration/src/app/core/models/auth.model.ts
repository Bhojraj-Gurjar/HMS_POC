export interface LoginRequest {
  username: string;
  password: string;
}

export interface LoginResponse {
  accessToken: string;
  tokenType: string;
  expiresInSeconds: number;
  username: string;
  roles: string[];
}

export interface AuthUser {
  username: string;
  roles: string[];
  token: string;
}
