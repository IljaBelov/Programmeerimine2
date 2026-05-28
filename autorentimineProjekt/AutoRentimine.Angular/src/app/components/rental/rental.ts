import { Component, OnInit, OnDestroy } from '@angular/core';
import { CarService, CarDto } from '../../services/car';

@Component({
  selector: 'app-rental',
  standalone: false,
  templateUrl: './rental.html',
  styleUrl: './rental.css',
})
export class Rental implements OnInit, OnDestroy {
  screen: string = 'selection';
  availableCars: CarDto[] = [];
  selectedCar: CarDto | null = null;
  errorMessage: string = '';

  carInfo: string = '';
  timeText: string = '00:00:00';
  kmText: string = '0.00 км';
  finalPrice: string = '';
  receiptDetails: string = '';

  private startTime: Date = new Date();
  private kilometersTraveled: number = 0;
  private timer: any = null;

  constructor(private carService: CarService) {}

  ngOnInit(): void {
    this.loadAvailableCars();
  }

  ngOnDestroy(): void {
    if (this.timer) clearInterval(this.timer);
  }

  loadAvailableCars(): void {
    this.errorMessage = '';
    this.carService.getCars().subscribe({
      next: (cars) => {
        this.availableCars = cars.filter(c => c.status?.trim().toLowerCase() === 'free');
      },
      error: (err) => this.errorMessage = 'Ошибка загрузки: ' + err.message
    });
  }

  startRental(car: CarDto): void {
    this.carService.createBooking(car.id).subscribe({
      next: () => {
        this.selectedCar = car;
        this.startTime = new Date();
        this.kilometersTraveled = 0;
        this.carInfo = `Вы арендовали: ${car.mark} ${car.model} [${car.registrationNumber}]`;
        this.timeText = '00:00:00';
        this.kmText = '0.00 км';
        this.screen = 'active';

        this.timer = setInterval(() => {
          const elapsed = new Date().getTime() - this.startTime.getTime();
          this.kilometersTraveled += 50 / 3600;

          const h = Math.floor(elapsed / 3600000).toString().padStart(2, '0');
          const m = Math.floor((elapsed % 3600000) / 60000).toString().padStart(2, '0');
          const s = Math.floor((elapsed % 60000) / 1000).toString().padStart(2, '0');

          this.timeText = `${h}:${m}:${s}`;
          this.kmText = `${this.kilometersTraveled.toFixed(2)} км`;
        }, 1000);
      },
      error: (err) => this.errorMessage = 'Не удалось арендовать: ' + err.message
    });
  }

  endRental(): void {
    if (this.timer) { clearInterval(this.timer); this.timer = null; }

    this.carService.cancelBooking(this.selectedCar!.id, this.kilometersTraveled).subscribe({
      next: (price) => {
        const elapsed = new Date().getTime() - this.startTime.getTime();
        const h = Math.floor(elapsed / 3600000).toString().padStart(2, '0');
        const m = Math.floor((elapsed % 3600000) / 60000).toString().padStart(2, '0');
        const s = Math.floor((elapsed % 60000) / 1000).toString().padStart(2, '0');

        this.finalPrice = `${price.toFixed(2)} EUR`;
        this.receiptDetails = `Автомобиль: ${this.selectedCar!.mark} ${this.selectedCar!.model}\n` +
                              `Пройдено: ${this.kilometersTraveled.toFixed(2)} км\n` +
                              `Время: ${h}:${m}:${s}`;
        this.screen = 'result';
      },
      error: (err) => {
        this.errorMessage = 'Ошибка завершения: ' + err.message;
        this.timer = setInterval(() => {}, 1000);
      }
    });
  }

  backToMenu(): void {
    this.screen = 'selection';
    this.selectedCar = null;
    this.loadAvailableCars();
  }
}
