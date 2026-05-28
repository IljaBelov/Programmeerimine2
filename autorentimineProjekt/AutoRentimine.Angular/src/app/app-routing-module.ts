import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { Cars } from './components/cars/cars';
import { Rental } from './components/rental/rental';

const routes: Routes = [
  { path: 'cars', component: Cars },
  { path: 'rental', component: Rental },
  { path: '', redirectTo: '/cars', pathMatch: 'full' }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
