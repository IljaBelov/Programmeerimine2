import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { Cars } from './components/cars/cars';

const routes: Routes = [
  { path: 'cars', component: Cars },
  { path: '', redirectTo: '/cars', pathMatch: 'full' }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
