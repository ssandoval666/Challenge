import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { ProductsService } from '../../services/products.service';
import { Product } from '../../models/product.model';

@Component({
  standalone: true,
  selector: 'app-products-list',
  imports: [CommonModule, RouterLink, FormsModule],
  template: `
  <div class="d-flex justify-content-between align-items-center mb-3">
    <h2 class="m-0">Productos</h2>
    <a routerLink="/products/new" class="btn btn-primary">Nuevo Producto</a>
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
              <th style="width: 80px;">ID</th>
              <th>Nombre</th>
              <th>Descripción</th>
              <th>Categorías</th>
              <th style="width: 230px;">Acciones</th>
            </tr>
          </thead>
          <tbody>
            <tr *ngFor="let p of filteredProducts()">
              <td>{{ p.productID }}</td>
              <td>{{ p.name }}</td>
              <td class="text-muted">{{ p.description }}</td>
              <td>
                <span *ngFor="let pc of p.productCategories" class="badge text-bg-secondary me-1">
                  {{ pc.category?.name || pc.categoryID }}
                </span>
              </td>
              <td>
                <div class="table-actions">
                  <a [routerLink]="['/products', p.productID, 'edit']" class="btn btn-sm btn-outline-secondary">Editar</a>
                  <button class="btn btn-sm btn-outline-danger" (click)="onDelete(p)">Eliminar</button>
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
export class ProductsListComponent implements OnInit {
  products: Product[] = [];
  loading = false;

  searchName = '';

  constructor(private svc: ProductsService) {}

  ngOnInit(): void {
    this.fetch();
  }

  fetch() {
    this.loading = true;
    this.svc.getAll().subscribe({
      next: data => { this.products = data; this.loading = false; },
      error: _ => { this.loading = false; alert('Error cargando productos'); }
    });
  }

  private normalize(str: string): string {
    return (str ?? '').toString()
      .normalize('NFD')
      .replace(/[\u0300-\u036f]/g, '') // quita acentos
      .toLowerCase();
  }

  filteredProducts(): Product[] {
    const nameTerm = this.normalize(this.searchName.trim());
    return this.products.filter(p =>
      !nameTerm || this.normalize(p.name ?? '').includes(nameTerm)
    );
  }

  clearFilters() {
    this.searchName = '';
  }

  onDelete(p: Product) {
    if (!confirm(`¿Eliminar producto "${p.name}"?`)) return;
    this.svc.delete(p.productID).subscribe({
      next: () => this.fetch(),
      error: _ => alert('No se pudo eliminar.')
    });
  }
}
