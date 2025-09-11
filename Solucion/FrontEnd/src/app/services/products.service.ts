import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, map, tap } from 'rxjs';
import { Product } from '../models/product.model';
import { API_BASE } from './api.config';

@Injectable({
  providedIn: 'root'
})
export class ProductsService {
  private apiUrl = `${API_BASE}/Products`;

  constructor(private http: HttpClient) {}

getAll(): Observable<Product[]> {
  return this.http.get<Product[]>(this.apiUrl).pipe(
    tap(res => {
        console.log('📥 JSON recibido desde API:', res);
      }),
    map(products =>
      (products || []).map(p => ({
        ...p,
        productCategories: (p.productCategories || [])
          .filter(pc => pc !== null)
          .map(pc => ({
            ...pc!,
            product: null, // 🔴 evitar ciclos
            category: pc?.category
              ? {
                  ...pc.category,
                  productCategories: [] // 🔴 vaciamos para no traer nulls
                }
              : null
          }))
      }))
    )
  );
}

getById(id: number): Observable<Product> {
  return this.http.get<Product>(`${this.apiUrl}/${id}`).pipe(
    map(p => ({
      ...p,
      productCategories: (p.productCategories || [])
        .filter(pc => pc !== null)
        .map(pc => ({
          ...pc!,
          product: null,
          category: pc?.category
            ? {
                ...pc.category,
                productCategories: []
              }
            : null
        }))
    }))
  );
}

 create(product: Product): Observable<Product> {
  const dto = {
    productID: product.productID,
    name: product.name,
    description: product.description,
    image: product.image,
    categoryIDs: product.productCategories.map(pc => pc.categoryID)
  };

  return this.http.post<Product>(this.apiUrl, dto);
}

  update(id: number, product: Product): Observable<void> {
    const dto = {
    productID: product.productID,
    name: product.name,
    description: product.description,
    image: product.image,
    categoryIDs: product.productCategories.map(pc => pc.categoryID)
  };
  
    console.log(`${this.apiUrl}/${id}`);
    return this.http.put<void>(`${this.apiUrl}/${id}`, dto);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
