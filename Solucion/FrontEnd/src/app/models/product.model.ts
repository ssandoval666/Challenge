import type { ProductCategory } from './product-category.model';

export interface Product {
  productID: number;
  name: string;
  description: string;
  image: string | null;
  productCategories: ProductCategory[];
}
