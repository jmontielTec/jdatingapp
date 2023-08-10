export interface Message
{
  id: number;
  senderId: number;
  senderuserName: string;
  senderPhotoUlr: string;
  recipientId: number;
  recipientUserName: string;
  recipientPhotoUrl: string;
  content: string;
  dateRead?: Date;
  messageSent: Date;
}
