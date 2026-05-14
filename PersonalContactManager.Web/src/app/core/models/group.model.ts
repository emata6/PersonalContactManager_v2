export interface Group {
  id: string;
  name: string;
  description: string | null;
}

export interface CreateGroupRequest {
  name: string;
  description?: string;
}

export interface UpdateGroupRequest {
  name: string;
  description?: string;
}
