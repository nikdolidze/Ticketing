﻿using Ticketing.Core.Domain.Entities;

namespace Ticketing.Core.Application.Interfaces.Repositories;

public interface ICartRepository : IRepository<Cart, Guid>
{
}