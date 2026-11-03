export interface LoginRequest {
  email: string;
  password: string;
}

export interface RegisterRequest {
  fullName: string;
  email: string;
  password: string;
}

export interface AuthResponse {
  message: string;
  token?: string;
}

export interface JwtPayload {
  nameid: string;
  unique_name: string;
  email: string;
  role: string;
  exp: number;
}

