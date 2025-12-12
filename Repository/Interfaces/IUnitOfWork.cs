using System;
using Corno.Concept.Portal.Windsor.Context;

namespace Corno.Concept.Portal.Repository.Interfaces;

public interface IUnitOfWork : IDisposable
{
    BaseDbContext DbContext { get; }

    void Save();
}