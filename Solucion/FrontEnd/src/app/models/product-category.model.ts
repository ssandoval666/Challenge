import type { Category } from './category.model';
import type { Product } from './product.model';

export interface ProductCategory {
  productID: number;
  categoryID: number;
  product?: Product | null;
  category?: Category | null;
}
