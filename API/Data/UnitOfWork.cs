namespace API.Data;

public class UnitOfWork : IUnitOfWork
{
    private readonly DataContext context;
    private readonly IMapper mapper;
    public UnitOfWork(DataContext context, IMapper mapper)
    {
        this.mapper = mapper;
        this.context = context;

    }
    public IUserRepository UserRepository => new UserRepository(this.context, this.mapper);

    public IMessageRepository MessageRepository => new MessageRepository(this.context, this.mapper);

    public ILikesRepository LikesRepository => new LikesRepository(this.context);

    public async Task<bool> Complete()
    {
        return await this.context.SaveChangesAsync() > 0;
    }

    public bool HasChanges()
    {
        return this.context.ChangeTracker.HasChanges();
    }
}
