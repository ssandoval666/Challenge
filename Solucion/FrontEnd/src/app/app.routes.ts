import { Routes } from '@angular/router';

import { ProductsListComponent } from './components/products/products-list.component';
import { ProductFormComponent } from './components/products/product-form.component';
import { CategoriesListComponent } from './components/categories/categories-list.component';
import { CategoryFormComponent } from './components/categories/category-form.component';

export const APP_ROUTES: Routes = [
  { path: '', pathMatch: 'full', redirectTo: 'products' },
  { path: 'products', component: ProductsListComponent, title: 'Productos' },
  { path: 'products/new', component: ProductFormComponent, title: 'Nuevo Producto' },
  { path: 'products/:id/edit', component: ProductFormComponent, title: 'Editar Producto' },
  { path: 'categories', component: CategoriesListComponent, title: 'Categorías' },
  { path: 'categories/new', component: CategoryFormComponent, title: 'Nueva Categoría' },
  { path: 'categories/:id/edit', component: CategoryFormComponent, title: 'Editar Categoría' },
  { path: '**', redirectTo: '' }
];
