# Angular ABM + .NET 8 (Standalone, Angular 19)

App Angular (standalone) con Bootstrap 5 para ABM de **Products** y **Categories** contra la API `.NET 8` en `https://localhost:7047/Api`.

## Requisitos
- Node.js 18+
- Angular CLI 19 (`npm i -g @angular/cli@^19`)

## Instalación
```bash
npm install
ng serve -o
```
La app corre en `http://localhost:4200`.

## Estructura de Rutas
- `/products` (listado)
- `/products/new` (alta)
- `/products/:id/edit` (edición)
- `/categories` (listado)
- `/categories/new` (alta)
- `/categories/:id/edit` (edición)

## Configuración API
Si tu base cambia, edita `src/app/services/api.config.ts`.
