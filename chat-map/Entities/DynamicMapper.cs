using ChatMap.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace ChatMap.Entities
{
    public class DynamicMapper
    {
        private readonly PropertyInfo RowIndexerMemberInfo = typeof(DataRow).GetProperties()
            .Where(p =>
            {
                ParameterInfo[] indexParameters = p.GetIndexParameters();
                return indexParameters.Length == 1 && indexParameters[0].ParameterType == typeof(string);
            })
            .Single();
        private readonly MethodInfo IsNullMethodInfo = typeof(DataRow).GetMethod("IsNull", new[] { typeof(string) });

        private readonly Dictionary<Type, Delegate> _mappers = new Dictionary<Type, Delegate>();
        private readonly Dictionary<Type, Dictionary<PropertyInfo, string>> _propertyColumns = new Dictionary<Type, Dictionary<PropertyInfo, string>>();
        private readonly Dictionary<Type, Dictionary<PropertyInfo, Delegate>> _propertyDelegates = new Dictionary<Type, Dictionary<PropertyInfo, Delegate>>();

        public Func<DataRow, TEntity> GetMapper<TEntity>()
        {
            Delegate mapper = _mappers.GetOrAdd(typeof(TEntity), entityType => CreateMapper<TEntity>(entityType));

            return (Func<DataRow, TEntity>)mapper;
        }

        public IEnumerable<TEntity> Map<TEntity>(DataTable table)
        {
            Func<DataRow, TEntity> mapper = GetMapper<TEntity>();

            return table.Rows.Cast<DataRow>().Select(row => mapper(row));
        }

        private Func<DataRow, TEntity> CreateMapper<TEntity>(Type entityType)
        {
            ParameterExpression rowParameter = Expression.Parameter(typeof(DataRow), "row");

            Dictionary<PropertyInfo, string> propertyColumns = GetPropertyColumns<TEntity>();

            MemberInitExpression expressionBody = Expression.MemberInit(Expression.New(entityType),
                propertyColumns.Select(kvp => Expression.Bind(kvp.Key, CreateGetTableValueExpression(rowParameter, kvp.Value, kvp.Key.PropertyType)))
                );

            Expression<Func<DataRow, TEntity>> lambda = Expression.Lambda<Func<DataRow, TEntity>>(expressionBody, rowParameter);

            return lambda.Compile();
        }

        private Expression CreateGetTableValueExpression(ParameterExpression rowParameter, string columnName, Type propertyType)
        {
            ConstantExpression columnNameExpression = Expression.Constant(columnName);
            ConstantExpression nullStringExpression = Expression.Constant(propertyType.IsValueType ? Activator.CreateInstance(propertyType) : null, propertyType);

            return Expression.Condition(
                Expression.Call(rowParameter, IsNullMethodInfo, columnNameExpression),
                nullStringExpression,
                Expression.Convert(Expression.MakeIndex(rowParameter, RowIndexerMemberInfo, new[] { columnNameExpression }), propertyType)
                );
        }

        private Dictionary<PropertyInfo, string> GetPropertyColumns<TEntity>()
        {
            return _propertyColumns.GetOrAdd(typeof(TEntity), entityType => entityType.GetProperties().Select(
                prop =>
                {
                    //if (prop.PropertyType != typeof(string))
                    //    throw new ArgumentException($"The entitie's {entityType.FullName} property {prop.Name} is not of string type.");

                    string columnName = prop.GetCustomAttribute<ColumnAttribute>()?.Name ?? prop.Name;

                    return new KeyValuePair<PropertyInfo, string>(prop, columnName);
                }).ToDictionary(kvp => kvp.Key, kvp => kvp.Value)
            );
        }

        public string GetColumnName<TEntity>(Expression<Func<TEntity, string>> property)
        {
            PropertyInfo propertyInfo = (PropertyInfo)((MemberExpression)property.Body).Member;
            Dictionary<PropertyInfo, string> propertyColumns = GetPropertyColumns<TEntity>();

            return propertyColumns[propertyInfo];
        }

        public Func<TEntity, string> GetGetPropertyDelegate<TEntity>(Expression<Func<TEntity, string>> property)
        {
            PropertyInfo experssionPropertyInfo = (PropertyInfo)((MemberExpression)property.Body).Member;

            Dictionary<PropertyInfo, Delegate> propertyDelegates = _propertyDelegates.GetOrAdd(typeof(TEntity), entityType => new Dictionary<PropertyInfo, Delegate>());
            return (Func<TEntity, string>)propertyDelegates.GetOrAdd(experssionPropertyInfo, propertyInfo => property.Compile());
        }
    }
}
