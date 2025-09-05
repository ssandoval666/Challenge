import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormArray, FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { ProductsService } from '../../services/products.service';
import { CategoriesService } from '../../services/categories.service';
import { Product } from '../../models/product.model';
import { Category } from '../../models/category.model';

@Component({
  standalone: true,
  selector: 'app-product-form',
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  template: `
  <div class="card shadow-sm">
    <div class="card-body">
      <h2 class="h4 mb-3">{{ isEdit ? 'Editar Producto' : 'Nuevo Producto' }}</h2>
      <form [formGroup]="form" (ngSubmit)="onSubmit()" class="row g-3">
        <div class="col-md-6">
          <label class="form-label">Nombre</label>
          <input class="form-control" formControlName="name" placeholder="Nombre del producto">
          <div *ngIf="form.controls['name'].invalid && form.controls['name'].touched" class="text-danger small">
            El nombre es obligatorio.
          </div>
        </div>
        <div class="col-md-6">
          <label class="form-label">Imagen (URL)</label>
          <input class="form-control" formControlName="image" placeholder="https://...">
        </div>
        <div class="col-12">
          <label class="form-label">Descripción</label>
          <textarea class="form-control" formControlName="description" rows="3"></textarea>
        </div>

        <div class="col-12">
          <label class="form-label">Categorías</label>
          <div class="d-flex gap-2 align-items-start flex-wrap">
            <div *ngFor="let c of categories" class="form-check me-3">
              <input class="form-check-input" type="checkbox"
                [checked]="isCategorySelected(c.categoryID)"
                (change)="toggleCategory(c.categoryID, $event.target?.checked)"
                [id]="'cat-' + c.categoryID">
              <label class="form-check-label" [for]="'cat-' + c.categoryID">{{ c.name }}</label>
            </div>
          </div>
          <div class="form-text">Seleccioná una o más categorías para el producto.</div>
        </div>

        <div class="col-12 form-actions">
          <button class="btn btn-primary" [disabled]="form.invalid">Guardar</button>
          <a routerLink="/products" class="btn btn-light">Cancelar</a>
        </div>
      </form>
    </div>
  </div>
  `
})
export class ProductFormComponent implements OnInit {
  form = this.fb.group({
    productID: [0],
    name: ['', Validators.required],
    description: [''],
    image: [''],
    productCategories: this.fb.array([] as {productID:number, categoryID:number}[])
  });
  isEdit = false;
  categories: Category[] = [];

  get productCategoriesFA() { return this.form.get('productCategories') as FormArray; }

  constructor(
    private fb: FormBuilder,
    private route: ActivatedRoute,
    private router: Router,
    private productsSvc: ProductsService,
    private categoriesSvc: CategoriesService
  ) {}

  ngOnInit(): void {
    // Load categories first
    this.categoriesSvc.getAll().subscribe({
      next: cats => {
        this.categories = cats;
        const id = Number(this.route.snapshot.paramMap.get('id'));
        if (id) {
          this.isEdit = true;
          this.productsSvc.getById(id).subscribe({
            next: (p: Product) => {
              this.form.patchValue({
                productID: p.productID,
                name: p.name,
                description: p.description,
                image: p.image || ''
              });
              this.productCategoriesFA.clear();
              (p.productCategories || []).forEach(pc => {
                this.productCategoriesFA.push(this.fb.group({ productID: [p.productID], categoryID: [pc.categoryID] }));
              });
            },
            error: _ => alert('Error cargando el producto')
          });
        }
      },
      error: _ => alert('Error cargando categorías')
    });
  }

  isCategorySelected(categoryID: number): boolean {
    return this.productCategoriesFA.value?.some((pc: any) => pc.categoryID === categoryID);
  }

  toggleCategory(categoryID: number, checked?: boolean) {
    if (checked) {
      this.productCategoriesFA.push(this.fb.group({
        productID: [this.form.value.productID || 0],
        categoryID: [categoryID]
      }));
    } else {
      const idx = this.productCategoriesFA.value.findIndex((pc: any) => pc.categoryID === categoryID);
      if (idx > -1) this.productCategoriesFA.removeAt(idx);
    }
  }

  onSubmit() {
    const value = this.form.value as any;

    // Normalize payload to match your JSON shape
    const payload = {
      productID: value.productID || 0,
      name: value.name,
      description: value.description || '',
      image: value.image || null,
      productCategories: (value.productCategories || []).map((pc: any) => ({
        productID: value.productID || 0,
        product: null,
        categoryID: pc.categoryID,
        category: null
      }))
    } as Product;

    if (this.isEdit && payload.productID) {
      this.productsSvc.update(payload.productID, payload).subscribe({
        next: () => this.router.navigate(['/products']),
        error: _ => alert('No se pudo actualizar')
      });
    } else {
      this.productsSvc.create(payload).subscribe({
        next: () => this.router.navigate(['/products']),
        error: _ => alert('No se pudo crear')
      });
    }
  }
}
