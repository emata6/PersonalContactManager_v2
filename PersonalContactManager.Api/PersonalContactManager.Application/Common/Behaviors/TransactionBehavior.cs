using MediatR;
using PersonalContactManager.Application.Common.Interfaces;
using PersonalContactManager.Domain.Interfaces;

namespace PersonalContactManager.Application.Common.Behaviors;

public sealed class TransactionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IUnitOfWork _unitOfWork;

    public TransactionBehavior(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (request is not ICommand && request is not ICommand<TResponse>)
            return await next(cancellationToken);

        var response = await next(cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return response;
    }
}
