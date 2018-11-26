using System.Collections.Generic;

namespace ChatMap.Models
{
    public interface IBaseModel<TId>
    {
        TId Id { get; }
    }

    public interface IBaseModel : IBaseModel<int>
    {
    }
}
