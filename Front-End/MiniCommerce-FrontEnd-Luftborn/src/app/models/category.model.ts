export interface Category {
  id: string;
  name: string;
  description: string | null;
  createdAt: string;
}

export interface CreateCategoryDto {
  name: string;
  description: string | null;
}

export interface UpdateCategoryDto {
  name: string;
  description: string | null;
}
