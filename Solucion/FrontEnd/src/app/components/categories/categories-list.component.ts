import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { CategoriesService } from '../../services/categories.service';
import { Category } from '../../models/category.model';

@Component({
  standalone: true,
  selector: 'app-categories-list',
  imports: [CommonModule, RouterLink],
  template: `
  <div class="d-flex justify-content-between align-items-center mb-3">
    <h2 class="m-0">Categorías</h2>
    <a routerLink="/categories/new" class="btn btn-primary">Nueva Categoría</a>
  </div>
  <div class="card shadow-sm">
    <div class="card-body p-0">
      <div class="table-responsive">
        <table class="table table-hover align-middle m-0">
          <thead class="table-light">
            <tr>
              <th style="width: 100px;">ID</th>
              <th>Nombre</th>
              <th style="width: 200px;">Acciones</th>
            </tr>
          </thead>
          <tbody>
            <tr *ngFor="let c of categories">
              <td>{{ c.categoryID }}</td>
              <td>{{ c.name }}</td>
              <td>
                <div class="table-actions">
                  <a [routerLink]="['/categories', c.categoryID, 'edit']" class="btn btn-sm btn-outline-secondary">Editar</a>
                  <button class="btn btn-sm btn-outline-danger" (click)="onDelete(c)">Eliminar</button>
                </div>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  </div>
  `
})
export class CategoriesListComponent implements OnInit {
  categories: Category[] = [];
  loading = false;

  constructor(private svc: CategoriesService) {}

  ngOnInit(): void {
    this.fetch();
  }

  fetch() {
    this.loading = true;
    this.svc.getAll().subscribe({
      next: data => { this.categories = data; this.loading = false; },
      error: _ => { this.loading = false; alert('Error cargando categorías'); }
    });
  }

  onDelete(c: Category) {
    if (!confirm(`¿Eliminar categoría "${c.name}"?`)) return;
    this.svc.delete(c.categoryID).subscribe({
      next: () => this.fetch(),
      error: _ => alert('No se pudo eliminar.')
    });
  }
}
