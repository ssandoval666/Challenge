import { Component } from '@angular/core';
import { RouterLink, RouterOutlet } from '@angular/router';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, RouterOutlet, RouterLink],
  template: `
  <nav class="navbar navbar-expand-lg navbar-dark bg-dark rounded-3 mb-3 px-3">
    <a class="navbar-brand" href="#">ABM Angular + .NET 8</a>
    <div class="collapse navbar-collapse show">
      <ul class="navbar-nav me-auto mb-2 mb-lg-0">
        <li class="nav-item"><a routerLink="/products" class="nav-link">Productos</a></li>
        <li class="nav-item"><a routerLink="/categories" class="nav-link">Categorías</a></li>
      </ul>
      <span class="navbar-text text-white-50 small">Bootstrap 5 • Standalone</span>
    </div>
  </nav>
  <div class="container">
    <router-outlet></router-outlet>
  </div>
  `
})
export class AppComponent {}
