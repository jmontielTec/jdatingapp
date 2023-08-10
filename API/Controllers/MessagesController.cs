using API.Helpers;
using Microsoft.AspNetCore.Authorization;

namespace API.Controllers;

[Authorize]
public class MessagesController : BaseApiController
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public MessagesController(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    [HttpPost]
    public async Task<ActionResult> CreateMessage(CreateMessageDto createMessageDto)
    {
        var username = User.GetUsername();
        if(username == createMessageDto.RecipientUserName.ToLower())
            return BadRequest("You cannot send messages to yourself");

        var sender = await _unitOfWork.UserRepository.GetUserByUsernameAsync(username);
        var recipient = await _unitOfWork.UserRepository.GetUserByUsernameAsync(createMessageDto.RecipientUserName);

        if(recipient == null) return NotFound();

        var message = new Message
        {
            Sender = sender,
            Recipient = recipient,
            SenderUserName = sender.UserName,
            RecipientUserName = recipient.UserName,
            Content = createMessageDto.Content
        };

        _unitOfWork.MessageRepository.AddMessage(message);

        if(await _unitOfWork.Complete()) return Ok(_mapper.Map<MessageDto>(message));

        return BadRequest("Failed to send message");
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessagesForUser([FromQuery] MessageParams messageParams)
    {
        messageParams.UserName = User.GetUsername();

        var messages = await _unitOfWork.MessageRepository.GetMessagesForUser(messageParams);

        Response.AddPaginationHeader(messages.CurrentPage, messages.PageSize, messages.TotalCount, messages.TotalPages);

        return messages;
    }

    [HttpGet("thread/{username}")]
    public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessageThread(string username)
    {
        var currentUserName = User.GetUsername();

        return Ok(await _unitOfWork.MessageRepository.GetMessageThread(currentUserName, username));
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteMessage(int id)
    {
        var username = User.GetUsername();
        var message = await _unitOfWork.MessageRepository.GetMessage(id);

        if(message.Sender.UserName != username && message.Recipient.UserName != username)
            return Unauthorized();

        if(message.Sender.UserName == username) message.SenderDeleted = true;

        if(message.Recipient.UserName == username) message.RecipientDeleted = true;

        if(message.SenderDeleted && message.RecipientDeleted) 
            _unitOfWork.MessageRepository.DeleteMessage(message);

        if(await _unitOfWork.Complete()) return Ok();

        return BadRequest("Problem deleting the message");
    }

}