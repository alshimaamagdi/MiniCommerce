import { Injectable } from '@angular/core';
import { ApiService } from './api.service';
import { CategoryUrls } from '../constants/api-urls';
import type { Category, CreateCategoryDto, UpdateCategoryDto } from '../../models/category.model';
import { map, Observable } from 'rxjs';

/** Category API – URLs equal to backend CategoriesController (api/categories). */
@Injectable({ providedIn: 'root' })
export class CategoryService {
  constructor(private api: ApiService) {}

  /** GET api/categories */
  getAll(): Observable<Category[]> {
    return this.api.get<Category[]>(CategoryUrls.getAll);
  }

  /** GET api/categories/{id} */
  getById(id: string): Observable<Category> {
    return this.api.get<Category>(CategoryUrls.getById(id));
  }

  /** POST api/categories */
  create(dto: CreateCategoryDto): Observable<Category> {
    return this.api.post<Category>(CategoryUrls.create, dto);
  }

  /** PUT api/categories/{id} */
  update(id: string, dto: UpdateCategoryDto): Observable<Category> {
    return this.api.put<Category>(CategoryUrls.update(id), dto);
  }

  /** DELETE api/categories/{id} */
  delete(id: string): Observable<void> {
    return this.api.delete(CategoryUrls.delete(id)).pipe(map(() => undefined));
  }
}
