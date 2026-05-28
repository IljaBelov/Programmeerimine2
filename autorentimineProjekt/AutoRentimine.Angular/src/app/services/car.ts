import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface CarDto {
  id: number;
  mark: string;
  model: string;
  registrationNumber: string;
  dailyRate: number;
  status: string;
}

@Injectable({
  providedIn: 'root',
})
export class CarService {
  private apiUrl = 'http://localhost:55005/api/cars';
 private bookingsUrl = 'http://localhost:55005/api/bookings';

  constructor(private http: HttpClient) {}

  getCars(): Observable<CarDto[]> {
    return this.http.get<CarDto[]>(this.apiUrl);
  }

  deleteCar(id: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/${id}`);
  }

  saveCar(car: CarDto): Observable<any> {
    if (car.id === 0) {
      return this.http.post(this.apiUrl, car);
    } else {
      return this.http.put(`${this.apiUrl}/${car.id}`, car);
    }
  }
  createBooking(carId: number): Observable<number> {
    return this.http.post<number>(this.bookingsUrl, { carId });
  }

  cancelBooking(carId: number, kilometers: number): Observable<number> {
    return this.http.post<number>(`${this.bookingsUrl}/cancel`, { carId, kilometers });
  }
}
