import { CommonModule } from '@angular/common';
import { HttpClientModule, HttpClient } from '@angular/common/http';
import { Component, inject, OnInit } from '@angular/core';

@Component({
  selector: 'app-data-display',
  standalone: true,
  imports: [CommonModule, HttpClientModule],
  templateUrl: './data-display.component.html',
  styleUrls: ['./data-display.component.css']
})
export class DataDisplayComponent implements OnInit {
  httpClient = inject(HttpClient);
  data: any = [];

  ngOnInit(): void { this.fetchData(); }

  fetchData() {
    this.httpClient
      .get('https://localhost:44389/api/ApplicationUser/GetAll', { withCredentials: true })
      .subscribe((data: any) => {
        console.log(data);
        this.data = data;
      });
  }
}