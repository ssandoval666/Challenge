import { Component, OnInit } from '@angular/core';
import { CategoryService } from '../../services/category.service';

@Component({
  selector: 'app-product-list',
  templateUrl: './product-list.component.html'
})
export class ProductListComponent implements OnInit {
  products: any[] = [];

  constructor(private CategoryService: CategoryService) {}

  ngOnInit() {
    this.loadProducts();
  }

  loadProducts() {
    this.CategoryService.getAll().subscribe(data => this.products = data);
  }

  delete(id: number) {
    if(confirm('Seguro de eliminar este producto?')){
      this.CategoryService.delete(id).subscribe(() => this.loadProducts());
    }
  }
}
