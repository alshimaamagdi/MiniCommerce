export interface Product {
  id: string;
  name: string;
  description: string | null;
  price: number;
  stockQuantity: number;
  categoryId: string;
  categoryName: string | null;
  createdAt: string;
}

export interface CreateProductDto {
  name: string;
  description: string | null;
  price: number;
  stockQuantity: number;
  categoryId: string;
}

export interface UpdateProductDto {
  name: string;
  description: string | null;
  price: number;
  stockQuantity: number;
  categoryId: string;
}
