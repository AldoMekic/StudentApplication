import { Component } from '@angular/core';

type SubjectRow = { code: string; name: string; ects: number; semester: number };

@Component({
  selector: 'app-subject-list',
  templateUrl: './subject-list.component.html',
  styleUrls: ['./subject-list.component.css']
})
export class SubjectListComponent {
  subjects: SubjectRow[] = [
    { code: 'ALG101', name: 'Algorithms',        ects: 8, semester: 3 },
    { code: 'DB201',  name: 'Databases',         ects: 7, semester: 4 },
    { code: 'DSP310', name: 'Digital Signal Processing', ects: 6, semester: 5 },
    { code: 'AI420',  name: 'AI & IoT',          ects: 6, semester: 6 },
  ];
}