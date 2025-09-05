import { ProductCategory } from "./product-category.model";

export interface Category {
  categoryID: number;
  name: string;
  productCategories: (ProductCategory | null)[];
}