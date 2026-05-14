using MediatR;

namespace PersonalContactManager.Application.Common.Interfaces;

public interface IQuery<TResponse> : IRequest<TResponse> { }
