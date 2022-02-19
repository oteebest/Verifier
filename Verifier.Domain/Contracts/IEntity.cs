using System;
using System.Collections.Generic;
using System.Text;

namespace Verifier.Domain.Contracts
{
    public interface IEntity<TId> : IEntity
    {
        public TId Id { get; set; }
    }

    public interface IEntity
    {

    }

   
}
