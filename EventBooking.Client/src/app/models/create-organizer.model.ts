export interface CreateOrganizerRequest {
  fullName: string;
  email: string;
  password: string;
}

export interface CreateOrganizerResponse {
  userId: number;
  fullName: string;
  email: string;
  role: string;
  token: string;
}
