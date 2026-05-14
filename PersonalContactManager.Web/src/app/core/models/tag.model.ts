export interface Tag {
  id: string;
  name: string;
  color: string | null;
}

export interface CreateTagRequest {
  name: string;
  color?: string;
}

export interface UpdateTagRequest {
  name: string;
  color?: string;
}
