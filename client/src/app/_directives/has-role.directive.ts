import { Directive, Input, OnInit, TemplateRef, ViewContainerRef } from '@angular/core';
import { take } from 'rxjs/operators';
import { User } from '../_models/user';
import { AccountService } from '../_services/account.service';

@Directive({
  selector: '[appHasRole]'
})
export class HasRoleDirective implements OnInit {
  @Input() appHasRole: String[] = [];

  user?: User | null;

  constructor(private viewContainerRef: ViewContainerRef,
    private templateRaf: TemplateRef<any>,
     private accountService: AccountService) {
       this.accountService.currentUser$.pipe(take(1)).subscribe(user => {
        this.user = user;
       })
  }

  ngOnInit(): void {
    //clear view if no roles
    if(!this.user?.roles || this.user == null){
      this.viewContainerRef.clear();
      return;
    }

    if(this.user?.roles.some(r => this.appHasRole.includes(r))){
      this.viewContainerRef.createEmbeddedView(this.templateRaf);
    }
    else {
      this.viewContainerRef.clear();
    }
  }

}
