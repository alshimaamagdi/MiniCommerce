import { Injectable } from '@angular/core';
import { ApiService } from './api.service';
import { ProductUrls } from '../constants/api-urls';
import type { Product, CreateProductDto, UpdateProductDto } from '../../models/product.model';
import { map, Observable } from 'rxjs';

/** Product API – URLs equal to backend ProductsController (api/products). */
@Injectable({ providedIn: 'root' })
export class ProductService {
  constructor(private api: ApiService) {}

  /** GET api/products */
  getAll(): Observable<Product[]> {
    return this.api.get<Product[]>(ProductUrls.getAll);
  }

  /** GET api/products/{id} */
  getById(id: string): Observable<Product> {
    return this.api.get<Product>(ProductUrls.getById(id));
  }

  /** GET api/products/by-category/{categoryId} */
  getByCategoryId(categoryId: string): Observable<Product[]> {
    return this.api.get<Product[]>(ProductUrls.getByCategoryId(categoryId));
  }

  /** POST api/products */
  create(dto: CreateProductDto): Observable<Product> {
    return this.api.post<Product>(ProductUrls.create, dto);
  }

  /** PUT api/products/{id} */
  update(id: string, dto: UpdateProductDto): Observable<Product> {
    return this.api.put<Product>(ProductUrls.update(id), dto);
  }

  /** DELETE api/products/{id} */
  delete(id: string): Observable<void> {
    return this.api.delete(ProductUrls.delete(id)).pipe(map(() => undefined));
  }
}
