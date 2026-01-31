import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink, ActivatedRoute } from '@angular/router';
import { CategoryService } from '../../../core/services/category.service';
import type { CreateCategoryDto } from '../../../models/category.model';

@Component({
  selector: 'app-category-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './category-form.component.html',
  styleUrl: './category-form.component.scss',
})
export class CategoryFormComponent implements OnInit {
  form!: FormGroup;
  id: string | null = null;
  loading = false;
  error: string | null = null;

  constructor(
    private fb: FormBuilder,
    private categoryService: CategoryService,
    private router: Router,
    private route: ActivatedRoute
  ) {}

  ngOnInit(): void {
    this.id = this.route.snapshot.paramMap.get('id');
    this.form = this.fb.group({
      name: ['', [Validators.required, Validators.maxLength(200)]],
      description: [''],
    });
    if (this.id) {
      this.categoryService.getById(this.id).subscribe({
        next: (c) => this.form.patchValue({ name: c.name, description: c.description ?? '' }),
        error: () => (this.error = 'Category not found'),
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
    const dto: CreateCategoryDto = {
      name: this.form.value.name,
      description: this.form.value.description || null,
    };
    const req = this.id
      ? this.categoryService.update(this.id, dto)
      : this.categoryService.create(dto);
    req.subscribe({
      next: () => this.router.navigate(['/categories']),
      error: (err) => {
        this.error = err?.error?.message ?? err?.message ?? 'Save failed';
        this.loading = false;
      },
    });
  }
}
