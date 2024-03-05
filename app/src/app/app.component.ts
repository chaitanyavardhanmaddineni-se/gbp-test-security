import { Component, OnInit, ViewEncapsulation } from '@angular/core';
import { CbmsBaseUtilService, PermissionService, IRoutesInfo } from 'se-cbms-base';
import { Location } from '@angular/common';
import { Router, ActivatedRoute,NavigationEnd } from '@angular/router';
//import { Idle, DEFAULT_INTERRUPTSOURCES } from '@ng-idle/core';

import { Title } from '@angular/platform-browser';
import { filter, map } from 'rxjs/operators';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent {
  title = 'cbms-global-bill-payment';

  constructor(
    private cbmsUtil: CbmsBaseUtilService,
    private location: Location ,public router: Router,
    private activatedRoute: ActivatedRoute,
    private titleService: Title  
  )
  {
    
}
ngOnInit() {
 
  this.router.events.pipe(
      filter(event => event instanceof NavigationEnd),
    )
    .subscribe(() => {
      var rt = this.getChild(this.activatedRoute)

      rt.data.subscribe(data => {
        //console.log(data);
        this.titleService.setTitle(data.title)})
    })

}

getChild(activatedRoute: ActivatedRoute) {
  if (activatedRoute.firstChild) {
    return this.getChild(activatedRoute.firstChild);
  } else {
    return activatedRoute;
  }

}
}
