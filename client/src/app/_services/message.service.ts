import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { BehaviorSubject } from 'rxjs';
import { take } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { Group } from '../_models/group';
import { Message } from '../_models/message';
import { User } from '../_models/user';
import { BusyService } from './busy.service';
import { getPaginationHeaders, getPaginationResult } from './paginationHelper';

@Injectable({
  providedIn: 'root'
})
export class MessageService {

  baserUrl = environment.apiUrl;
  hubUrl = environment.hubUrl;
  private hubConnection:HubConnection;
  private messageThreadSource = new BehaviorSubject<Message[]>([]);
  messageThread$ = this.messageThreadSource.asObservable();

  constructor(private http: HttpClient, private busyService: BusyService) { }


  createHubConnection(user: User, otherUsername : string) {
    this.busyService.busy();
    this.hubConnection = new HubConnectionBuilder()
    .withUrl(this.hubUrl + 'messages?user=' + otherUsername,{
      accessTokenFactory: () => user.token
    })
    .withAutomaticReconnect()
    .build();

    this.hubConnection
    .start()
    .catch(error => console.log(error))
    .finally(() => this.busyService.idle())

    this.hubConnection.on('ReceiveMessageThread', messages => {
      this.messageThreadSource.next(messages);
    });

    this.hubConnection.on('NewMessage', message => {
      this.messageThread$.pipe(take(1)).subscribe(messages => {
        this.messageThreadSource.next([...messages, message]);
      })
    })

    this.hubConnection.on('UpdatedGroup', (group:Group) => {
      if(group.connections.some(x => x.userName === otherUsername)){
        this.messageThread$.pipe(take(1)).subscribe(messages => {
          messages.forEach(message =>{
            if(!message.dateRead)
            {
              message.dateRead = new Date(Date.now())
            }
          })
          this.messageThreadSource.next([...messages]);
        });
      }
    })
  }

  stopHubConnection(){
    if(this.hubConnection){
      this.messageThreadSource.next([]);
      this.hubConnection.stop().catch(error => console.log(error));
    }
  }

  getMessages(pageNumber, pageSize, container){
    let params = getPaginationHeaders(pageNumber, pageSize);
    params = params.append('Container', container);
    return getPaginationResult<Message[]>(this.baserUrl + 'messages', params, this.http);
  }

  getMessageThread(username: string){
    return this.http.get<Message[]>(this.baserUrl + 'messages/thread/' + username);
  }

  async sendMessages(username: string, content:string) {
    return this.hubConnection.invoke('SendMessage',{recipientUserName: username, content})
    .catch(error => console.log(error));
  }

  deleteMessage(id: number){
    return this.http.delete(this.baserUrl + 'messages/'+ id);
  }
}
