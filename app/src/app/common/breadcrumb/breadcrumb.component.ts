import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { BreadcrumbService } from '../../core';

@Component({
  selector: 'app-breadcrumb',
  templateUrl: './breadcrumb.component.html',
  styleUrls: ['./breadcrumb.component.scss']
})
export class BreadcrumbComponent implements OnInit {

  pageName: any[];
  breadcrums: any[] = [{ label: 'Global Bill Payment', data: '/' },];
  constructor(private router: Router, private breadcrumbService: BreadcrumbService) { }

  ngOnInit() {
    
    this.breadcrumbService.currentName.subscribe(name => {
      this.pageName = name;
      if (this.pageName) {
        this.breadcrums = [];
        this.pageName.forEach(ele => {
          this.breadcrums.push({
            label: ele.label,
            url: ele.data,
            Optionalparam: ele.Optionalparam ? ele.Optionalparam[0] : null,
            isAbsolute: ele.isAbsolute
          });
        });
      }
    });
  }


  // routeNavigate(obj) {
  //   if (obj.Optionalparam != undefined) {
  //     this.router.navigate([obj.url, obj.Optionalparam]);
  //   } else {
  //     this.router.navigate([obj.url]);
  //   }
  // }

}
