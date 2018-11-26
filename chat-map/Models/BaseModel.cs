using System.Collections.Generic;

namespace ChatMap.Models
{
    public abstract class BaseModel<TId> : IBaseModel<TId>
    {
        public TId Id { get; set; }
    }

    public abstract class BaseModel: BaseModel<int>, IBaseModel
    {
    }
}
