
import { CommonModule } from '@angular/common';
import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { GalleryItem, GalleryModule, ImageItem } from 'ng-gallery';
import { TabDirective, TabsModule, TabsetComponent } from 'ngx-bootstrap/tabs';
import { TimeagoModule } from 'ngx-timeago';
import { take } from 'rxjs/operators';
import { Member } from 'src/app/_models/member';
import { Message } from 'src/app/_models/message';
import { User } from 'src/app/_models/user';
import { AccountService } from 'src/app/_services/account.service';
import { MessageService } from 'src/app/_services/message.service';
import { PresenceService } from 'src/app/_services/presence.service';
import { MemberMessagesComponent } from '../member-messages/member-messages.component';

@Component({
  standalone: true,
  selector: 'app-member-detail',
  templateUrl: './member-detail.component.html',
  styleUrls: ['./member-detail.component.css'],
  imports: [GalleryModule, TabsModule, CommonModule, TimeagoModule, MemberMessagesComponent]
})
export class MemberDetailComponent implements OnInit, OnDestroy {
  @ViewChild('memberTabs', {static:true}) memberTabs?:TabsetComponent;
  member:Member = {} as Member;
  activeTab?: TabDirective;
  messages: Message[] = [];
  user: User;
  images: GalleryItem[] = [];

  constructor(public presence: PresenceService
    , private route:ActivatedRoute
    , private messageService: MessageService
    , private accountService: AccountService
    , private router: Router) {
     this.accountService.currentUser$.pipe(take(1)).subscribe({
      next:user => {
        if(user) this.user = user;
      }
     });
  }

  ngOnInit(): void {
    this.route.data.subscribe(data => {
      this.member = data.member;
    });

    this.route.queryParams.subscribe(params => {
      params.tab ? this.selectTab(params.tab) : this.selectTab(0)
    })

    this.getImages();
  }

  getImages(){
    const imageUrls = [];
    if(!this.member) return;
    for (const photo of this.member.photos){
      this.images.push(new ImageItem({src: photo?.url, thumb: photo?.url}))
    }
  }

  loadMessages(){
    this.messageService.getMessageThread(this.member.userName).subscribe({
        next: messages => this.messages = messages
      });
  }

  selectTab(tabId:number){
    if (this.memberTabs) {
      this.memberTabs.tabs[tabId].active = true;
    }
  }

  onTabActivaded(data: TabDirective){
    this.activeTab = data;
    if(this.activeTab.heading === "Messages" && this.messages.length === 0){
      this.messageService.createHubConnection(this.user, this.member.userName);
    } else {
      this.messageService.stopHubConnection();
    }
  }

  ngOnDestroy(): void {
    this.messageService.stopHubConnection();
  }
}
