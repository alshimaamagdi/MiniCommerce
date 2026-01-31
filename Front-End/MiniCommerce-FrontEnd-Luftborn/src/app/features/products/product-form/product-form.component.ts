import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink, ActivatedRoute } from '@angular/router';
import { ProductService } from '../../../core/services/product.service';
import { CategoryService } from '../../../core/services/category.service';
import type { CreateProductDto } from '../../../models/product.model';
import type { Category } from '../../../models/category.model';

@Component({
  selector: 'app-product-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './product-form.component.html',
  styleUrl: './product-form.component.scss',
})
export class ProductFormComponent implements OnInit {
  form!: FormGroup;
  id: string | null = null;
  categories: Category[] = [];
  loading = false;
  error: string | null = null;

  constructor(
    private fb: FormBuilder,
    private productService: ProductService,
    private categoryService: CategoryService,
    private router: Router,
    private route: ActivatedRoute
  ) {}

  ngOnInit(): void {
    this.id = this.route.snapshot.paramMap.get('id');
    this.form = this.fb.group({
      name: ['', [Validators.required, Validators.maxLength(200)]],
      description: [''],
      price: [0, [Validators.required, Validators.min(0)]],
      stockQuantity: [0, [Validators.required, Validators.min(0)]],
      categoryId: ['', Validators.required],
    });
    this.categoryService.getAll().subscribe({
      next: (list) => (this.categories = list),
      error: () => (this.error = 'Failed to load categories'),
    });
    if (this.id) {
      this.productService.getById(this.id).subscribe({
        next: (p) =>
          this.form.patchValue({
            name: p.name,
            description: p.description ?? '',
            price: p.price,
            stockQuantity: p.stockQuantity,
            categoryId: p.categoryId,
          }),
        error: () => (this.error = 'Product not found'),
      });
    }
  }

  get isEdit(): boolean {
    return this.id != null;
  }

  submit(): void {
    if (this.form.invalid || this.loading) return;
    this.loading = true;
    this.error = null;
    const dto: CreateProductDto = {
      name: this.form.value.name,
      description: this.form.value.description || null,
      price: Number(this.form.value.price),
      stockQuantity: Number(this.form.value.stockQuantity),
      categoryId: this.form.value.categoryId,
    };
    const req = this.id
      ? this.productService.update(this.id, dto)
      : this.productService.create(dto);
    req.subscribe({
      next: () => this.router.navigate(['/products']),
      error: (err) => {
        this.error = err?.error?.message ?? err?.message ?? 'Save failed';
        this.loading = false;
      },
    });
  }
}
