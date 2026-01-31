
export const API_CATEGORIES = '/api/categories';
export const API_PRODUCTS = '/api/products';

export const CategoryUrls = {
  getAll: API_CATEGORIES,
  getById: (id: string) => `${API_CATEGORIES}/${id}`,
  create: API_CATEGORIES,
  update: (id: string) => `${API_CATEGORIES}/${id}`,
  delete: (id: string) => `${API_CATEGORIES}/${id}`,
} as const;

export const ProductUrls = {
  getAll: API_PRODUCTS,
  getById: (id: string) => `${API_PRODUCTS}/${id}`,
  getByCategoryId: (categoryId: string) => `${API_PRODUCTS}/by-category/${categoryId}`,
  create: API_PRODUCTS,
  update: (id: string) => `${API_PRODUCTS}/${id}`,
  delete: (id: string) => `${API_PRODUCTS}/${id}`,
} as const;
