import { Component } from '@angular/core';

type StudentRow = { index: string; name: string; year: number; status: 'Active' | 'On leave' | 'Graduated' };

@Component({
  selector: 'app-student-list',
  templateUrl: './student-list.component.html',
  styleUrls: ['./student-list.component.css']
})
export class StudentListComponent {
  students: StudentRow[] = [
    { index: '2025/001', name: 'John Doe',   year: 3, status: 'Active' },
    { index: '2025/017', name: 'Ana Petrović', year: 2, status: 'Active' },
    { index: '2024/112', name: 'Marko Jovanov', year: 4, status: 'Graduated' },
    { index: '2025/045', name: 'Lejla Kovač',  year: 1, status: 'On leave' },
  ];
}