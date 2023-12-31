
import { CommonModule } from '@angular/common';
import { ChangeDetectionStrategy, Component, Input, OnInit, ViewChild } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import { TimeagoModule } from 'ngx-timeago';
import { Message } from 'src/app/_models/message';
import { MessageService } from 'src/app/_services/message.service';

@Component({
  changeDetection:ChangeDetectionStrategy.OnPush,
  selector: 'app-member-messages',
  templateUrl: './member-messages.component.html',
  styleUrls: ['./member-messages.component.css'],
  standalone: true,
  imports:[CommonModule, FormsModule, TimeagoModule]
})
export class MemberMessagesComponent{
  @ViewChild('messageForm') messageForm: NgForm;
  @Input() messages: Message[];
  @Input() username: string;
  messageContent: string;
  loading = false;

  constructor(public messageService: MessageService) { }

  sendMessage() {
    this.loading = true;
    this.messageService.sendMessages(this.username, this.messageContent).then(() => {
      this.messageForm.reset();
    }).finally(() => this.loading = false);
  }
}
