using System;
using System.Collections.Generic;
using System.Linq;

namespace ChatMap.Extensions
{
    public class MergeExecutor<TLeft, TRight, TKey>
    {
        private readonly IEnumerable<TLeft> _left;
        private readonly IEnumerable<TRight> _right;
        private readonly Func<TLeft, TKey> _leftKey;
        private readonly Func<TRight, TKey> _rightKey;

        private Action<TRight> _create;
        private Action<TLeft, TRight> _update;
        private Action<TLeft> _delete;

        private Func<TLeft, TRight, bool> _isChanged;

        private Action<IReadOnlyList<TRight>> _creatyMany;
        private Action<IReadOnlyList<TLeft>> _deleteMany;

        public MergeExecutor(IEnumerable<TLeft> left, IEnumerable<TRight> right, Func<TLeft, TKey> leftKey, Func<TRight, TKey> rightKey)
        {
            _left = left;
            _right = right;
            _leftKey = leftKey;
            _rightKey = rightKey;
        }

        public MergeExecutor<TLeft, TRight, TKey> IsChanged(Func<TLeft, TRight, bool> isChanged)
        {
            _isChanged = isChanged;

            return this;
        }

        public MergeExecutor<TLeft, TRight, TKey> Update(Action<TLeft, TRight> update)
        {
            _update = update;

            return this;
        }

        public MergeExecutor<TLeft, TRight, TKey> Create(Action<TRight> create)
        {
            _create = create;

            return this;
        }

        public MergeExecutor<TLeft, TRight, TKey> CreateMany(Action<IReadOnlyList<TRight>> createMany)
        {
            _creatyMany = createMany;

            return this;
        }

        public MergeExecutor<TLeft, TRight, TKey> Delete(Action<TLeft> delete)
        {
            _delete = delete;

            return this;
        }

        public MergeExecutor<TLeft, TRight, TKey> DeleteMany(Action<IReadOnlyList<TLeft>> deleteMany)
        {
            _deleteMany = deleteMany;

            return this;
        }

        public void Execute()
        {
            var joinResult = EnumerableExtensions.FullOuterGroupJoin(_left, _right, _leftKey, _rightKey,
                (leftResults, rightResults) =>
                    new
                    {
                        Entities = leftResults,
                        Models = rightResults
                    });

            List<TLeft> toDelete = null;
            if (_deleteMany != null)
                toDelete = new List<TLeft>();

            List<TRight> toCreate = null;
            if (_creatyMany != null)
                toCreate = new List<TRight>();

            foreach (var pair in joinResult)
            {
                var hasEntity = pair.Entities.TryFirst(out var entity);
                if (hasEntity)
                {
                    var hasModel = pair.Models.TryFirst(out var model);

                    if (hasModel)
                    {
                        if (_isChanged == null || _isChanged(entity, model))
                            _update?.Invoke(entity, model);
                    }
                    else
                    {
                        if (_deleteMany != null)
                            toDelete.Add(entity);

                        _delete?.Invoke(entity);
                    }
                }
                else
                {
                    foreach (var pairModel in pair.Models)
                    {
                        if (_creatyMany != null)
                            toCreate.Add(pairModel);

                        _create?.Invoke(pairModel);
                    }
                }
            }

            _creatyMany?.Invoke(toCreate);
            _deleteMany?.Invoke(toDelete);
        }
    }
}