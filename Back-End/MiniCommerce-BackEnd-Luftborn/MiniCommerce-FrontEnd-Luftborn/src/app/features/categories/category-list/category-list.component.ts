import { Component, OnInit, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { CategoryService } from '../../../core/services/category.service';
import { ToastrService } from 'ngx-toastr';
import type { Category } from '../../../models/category.model';
import { ConfirmModalComponent } from '../../../Shared/Dialog/confirm-dialog.component';

@Component({
  selector: 'app-category-list',
  standalone: true,
  imports: [CommonModule, RouterLink, ConfirmModalComponent],
  templateUrl: './category-list.component.html',
  styleUrls: ['./category-list.component.scss'],
})
export class CategoryListComponent implements OnInit {
  categories: Category[] = [];
  selectedCategory: Category | null = null;
  loading = true;
  error: string | null = null;

  @ViewChild('confirmModal') confirmModal!: ConfirmModalComponent;

  constructor(
    private categoryService: CategoryService,
    private toastr: ToastrService
  ) {}

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.loading = true;
    this.error = null;
    this.categoryService.getAll().subscribe({
      next: (data) => {
        this.categories = data;
        this.loading = false;
      },
      error: (err) => {
        this.error = err?.message ?? 'Failed to load';
        this.loading = false;
      },
    });
  }

  openDeleteModal(category: Category) {
    this.selectedCategory = category;
    this.confirmModal.onConfirm = () => this.confirmDelete();
    this.confirmModal.open();
  }

  confirmDelete() {
    if (!this.selectedCategory) return;
    this.categoryService.delete(this.selectedCategory.id).subscribe({
      next: () => {
        this.toastr.success(
          `Category "${this.selectedCategory?.name}" deleted successfully`
        );
        this.load();
      },
      error: (err) =>
        this.toastr.error(err?.message ?? 'Delete failed'),
    });
  }

  trackById(index: number, item: Category) {
    return item.id;
  }
}
