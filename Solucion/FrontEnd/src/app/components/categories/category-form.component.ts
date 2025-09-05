import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { Category } from '../../models/category.model';
import { CategoriesService } from '../../services/categories.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-category-form',
  standalone: true, // 👈 importante en Angular 19
  imports: [CommonModule, ReactiveFormsModule], // 👈 habilitamos formularios reactivos
  templateUrl: './category-form.component.html'
})
export class CategoryFormComponent implements OnInit {
  form: FormGroup;
  categoryId?: number;

  constructor(
    private fb: FormBuilder,
    private service: CategoriesService,
    private router: Router,
    private route: ActivatedRoute
  ) {
    this.form = this.fb.group({
      categoryID: [null],
      name: ['', Validators.required]
    });
  }

  ngOnInit(): void {
    this.categoryId = Number(this.route.snapshot.paramMap.get('id'));
    if (this.categoryId) {
      this.service.getById(this.categoryId).subscribe({
        next: (c: Category) => {
          this.form.patchValue({
            categoryID: c.categoryID,
            name: c.name
          });
        }
      });
    }
  }

  onSubmit(): void {
    if (this.form.invalid) return;

    const category: Category = this.form.value;

    if (this.categoryId) {
      this.service.update(this.categoryId, category).subscribe({
        next: () => this.router.navigate(['/categories'])
      });
    } else {
      this.service.create(category).subscribe({
        next: () => this.router.navigate(['/categories'])
      });
    }
  }
}
