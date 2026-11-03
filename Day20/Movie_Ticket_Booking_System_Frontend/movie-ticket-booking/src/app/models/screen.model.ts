export interface Screen {
  screenId: number;
  theatreId: number;
  theatreName: string;
  screenName: string;
}

export interface CreateScreenRequest {
  theatreId: number;
  screenName: string;
}

export interface UpdateScreenRequest extends CreateScreenRequest {}

