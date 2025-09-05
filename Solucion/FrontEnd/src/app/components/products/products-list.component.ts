import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { ProductsService } from '../../services/products.service';
import { Product } from '../../models/product.model';

@Component({
  standalone: true,
  selector: 'app-products-list',
  imports: [CommonModule, RouterLink],
  template: `
  <div class="d-flex justify-content-between align-items-center mb-3">
    <h2 class="m-0">Productos</h2>
    <a routerLink="/products/new" class="btn btn-primary">Nuevo Producto</a>
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
            <tr *ngFor="let p of products">
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

  onDelete(p: Product) {
    if (!confirm(`¿Eliminar producto "${p.name}"?`)) return;
    this.svc.delete(p.productID).subscribe({
      next: () => this.fetch(),
      error: _ => alert('No se pudo eliminar.')
    });
  }
}
