import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { CategoriesService } from '../../services/categories.service';
import { Category } from '../../models/category.model';

@Component({
  standalone: true,
  selector: 'app-categories-list',
  imports: [CommonModule, RouterLink, FormsModule],
  template: `
  <div class="d-flex justify-content-between align-items-center mb-3">
    <h2 class="m-0">Categorías</h2>
    <a routerLink="/categories/new" class="btn btn-primary">Nueva Categoría</a>
  </div>

  <!-- Filtro por nombre -->
  <div class="card mb-3 shadow-sm">
    <div class="card-body">
      <form class="row g-2">
        <div class="col-md-10">
          <input [(ngModel)]="searchName" name="searchName" type="text"
                 placeholder="Buscar por nombre"
                 class="form-control" />
        </div>
        <div class="col-md-2 d-grid">
          <button type="button" class="btn btn-outline-secondary" (click)="clearFilters()">Limpiar</button>
        </div>
      </form>
    </div>
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
            <tr *ngFor="let c of filteredCategories()">
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

  searchName = '';

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

  // Normaliza texto: sin acentos, en minúsculas
  private normalize(str: string): string {
    return (str ?? '').toString()
      .normalize('NFD')
      .replace(/[\u0300-\u036f]/g, '')
      .toLowerCase();
  }

  filteredCategories(): Category[] {
    const term = this.normalize(this.searchName.trim());
    return this.categories.filter(c =>
      !term || this.normalize(c.name ?? '').includes(term)
    );
  }

  clearFilters() {
    this.searchName = '';
  }

  onDelete(c: Category) {
    if (!confirm(`¿Eliminar categoría "${c.name}"?`)) return;
    this.svc.delete(c.categoryID).subscribe({
      next: () => this.fetch(),
      error: _ => alert('No se pudo eliminar.')
    });
  }
}
