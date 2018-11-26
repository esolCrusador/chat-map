using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace ChatMap.Extensions
{
    public struct LambdaJoinBuilder<TLeft, TRight>
    {
        public LambdaJoinBuilder(IQueryable<TLeft> left, IQueryable<TRight> right)
        {
            _left = left;
            _right = right;
        }

        private readonly IQueryable<TLeft> _left;
        private readonly IQueryable<TRight> _right;
        public LambdaJoinBuilderWithLeftKey<TLeft, TRight, TKey> LeftKey<TKey>(Expression<Func<TLeft, TKey>> keySelector)
        {
            return new LambdaJoinBuilderWithLeftKey<TLeft, TRight, TKey>(_left, _right, keySelector);
        }
    }
    public struct LambdaJoinBuilderWithLeftKey<TLeft, TRight, TKey>
    {
        public LambdaJoinBuilderWithLeftKey(IQueryable<TLeft> left, IQueryable<TRight> right, Expression<Func<TLeft, TKey>> keySelector)
        {
            _left = left;
            _right = right;
            _keySelector = keySelector;
        }

        private readonly IQueryable<TLeft> _left;
        private readonly IQueryable<TRight> _right;
        private readonly Expression<Func<TLeft, TKey>> _keySelector;

        public LambdaJoinBuilder<TLeft, TRight, TKey> RightKey(Expression<Func<TRight, TKey>> rightKeySelector)
        {
            return new LambdaJoinBuilder<TLeft, TRight, TKey>(_left, _right, _keySelector, rightKeySelector);
        }
    }

    public struct LambdaJoinBuilder<TLeft, TRight, TKey>
    {
        public LambdaJoinBuilder(IQueryable<TLeft> left, IQueryable<TRight> right, Expression<Func<TLeft, TKey>> keySelector, Expression<Func<TRight, TKey>> rightKeySelector)
        {
            _left = left;
            _right = right;
            _leftKeySelector = keySelector;
            _rightKeySelector = rightKeySelector;
        }
        private readonly IQueryable<TLeft> _left;
        private readonly IQueryable<TRight> _right;
        private readonly Expression<Func<TLeft, TKey>> _leftKeySelector;
        private readonly Expression<Func<TRight, TKey>> _rightKeySelector;
        public IQueryable<TResult> Join<TResult>(Expression<Func<TLeft, TRight, TResult>> getResult)
        {
            return _left.Join(_right, _leftKeySelector, _rightKeySelector, getResult);
        }
        public IQueryable<TResult> LeftGroupJoin<TResult>(Expression<Func<TLeft, IEnumerable<TRight>, TResult>> getResult)
        {
            return _left.GroupJoin(_right, _leftKeySelector, _rightKeySelector, getResult);
        }
    }
}
