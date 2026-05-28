import { Component, OnInit } from '@angular/core';
import { CarService, CarDto } from '../../services/car';

@Component({
  selector: 'app-cars',
  standalone: false,
  templateUrl: './cars.html',
  styleUrl: './cars.css',
})
export class Cars implements OnInit {
  cars: CarDto[] = [];
  errorMessage: string = '';
  formCar: CarDto = this.emptycar();
  showForm: boolean = false;

  constructor(private carService: CarService) {}

  ngOnInit(): void {
    this.loadCars();
  }

  loadCars(): void {
    this.carService.getCars().subscribe({
      next: (data) => this.cars = data,
      error: (err) => this.errorMessage = 'Ошибка загрузки: ' + err.message
    });
  }

  editCar(car: CarDto): void {
    this.formCar = { ...car };
    this.showForm = true;
  }

  newCar(): void {
    this.formCar = this.emptycar();
    this.showForm = true;
  }

  saveCar(): void {
    this.carService.saveCar(this.formCar).subscribe({
      next: () => {
        this.loadCars();
        this.showForm = false;
        this.formCar = this.emptycar();
      },
      error: (err) => this.errorMessage = 'Ошибка сохранения: ' + err.message
    });
  }

  deleteCar(id: number): void {
    this.carService.deleteCar(id).subscribe({
      next: () => this.loadCars(),
      error: (err) => this.errorMessage = 'Ошибка удаления: ' + err.message
    });
  }

  cancel(): void {
    this.showForm = false;
    this.formCar = this.emptycar();
  }

  emptycar(): CarDto {
    return { id: 0, mark: '', model: '', registrationNumber: '', dailyRate: 0, status: 'free' };
  }
}
