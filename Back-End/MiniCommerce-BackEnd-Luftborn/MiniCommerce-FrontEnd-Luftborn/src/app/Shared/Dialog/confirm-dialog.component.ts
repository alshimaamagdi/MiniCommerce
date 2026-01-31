import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-confirm-modal',
  standalone: true,
  imports: [CommonModule], // <-- مهم عشان *ngIf و [ngClass]
  templateUrl: './confirm-dialog.component.html',
  styleUrls: ['./confirm-dialog.component.scss'],
})
export class ConfirmModalComponent {
  @Input() id: string = 'confirmModal'; // لازم يكون موجود
  @Input() title: string = 'Confirm Delete';
  @Input() message: string = 'Are you sure?';
  @Input() onConfirm!: () => void;

  isOpen = false;

  open() {
    this.isOpen = true;
  }

  hide() {
    this.isOpen = false;
  }

  confirm() {
    if (this.onConfirm) this.onConfirm();
    this.hide();
  }
}
