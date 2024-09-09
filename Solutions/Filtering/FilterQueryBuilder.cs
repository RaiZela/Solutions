using System.Linq.Expressions;

namespace Solutions.Filtering;

public static class FilterQueryBuilder
{
    //SHEMBULL additionalFilter: Expression<Func<T, bool>> additionalFilter  = ((T p) => p.CreatedDate.Date == filter.SignedDate.Date);
    public static IQueryable<T> ApplyFilter<T>(IQueryable<T> query, object filter, Expression<Func<T, bool>> additionalFilter = null)
    {
        if (filter == null)
        {
            return query;
        }

        var filterProperties = filter.GetType().GetProperties();

        ParameterExpression parameter = Expression.Parameter(typeof(T), "x");
        Expression expression = Expression.Constant(true);

        foreach (var property in filterProperties)
        {
            var modelProperty = typeof(T).GetProperty(property.Name);
            if (modelProperty == null)
            {
                continue;
            }

            var propertyValue = property.GetValue(filter, null);

            if (property.PropertyType == typeof(DateTime))
            {
                if ((DateTime)propertyValue! == DateTime.MinValue)
                    continue;

                var memberExpression = Expression.Property(parameter, modelProperty);
                var filterDate = ((DateTime)propertyValue!);


                BinaryExpression dateEqualityExpression = Expression.Equal(
                    Expression.Property(memberExpression, nameof(DateTime.Date)),
                    Expression.Constant(filterDate.Date)
                );

                expression = Expression.AndAlso(expression, dateEqualityExpression);

            }
            else if (propertyValue != null)
            {
                var propertyType = property.PropertyType;

                var memberExpression = Expression.Property(parameter, property.Name);

                var filterValue = Expression.Constant(propertyValue);

                BinaryExpression equalityExpression = null;

                if (propertyType == typeof(string) && propertyValue is string stringValue && !string.IsNullOrEmpty(stringValue)
                    && memberExpression.Type == typeof(String))
                {
                    propertyValue = stringValue.Trim().ToUpper();

                    var toUpperMethod = typeof(string).GetMethod("ToUpper", Type.EmptyTypes);
                    var trimMethod = typeof(string).GetMethod("Trim", Type.EmptyTypes);

                    var toUpperCall = Expression.Call(memberExpression, toUpperMethod!);
                    var trimCall = Expression.Call(toUpperCall, trimMethod!);


                    equalityExpression = Expression.Equal(trimCall, Expression.Constant(propertyValue));
                    expression = Expression.AndAlso(expression, equalityExpression);
                }
                else
                {

                    equalityExpression = Expression.Equal(memberExpression, filterValue);
                    expression = Expression.AndAlso(expression, equalityExpression);
                }
            }
        }

        if (additionalFilter != null)
        {
            var additionalFilterExpression = Expression.Invoke(Expression.Constant(additionalFilter), parameter);
            expression = Expression.AndAlso(expression, additionalFilterExpression);
        }

        var lambda = Expression.Lambda<Func<T, bool>>(expression, parameter);

        var modifiedLambda = ModifyLambda(lambda);

        return query.Where(modifiedLambda);
    }

    static Expression<Func<T, bool>> ModifyLambda<T>(Expression<Func<T, bool>> originalLambda)
    {
        if (IsTrueConstant(originalLambda))
        {
            return Expression.Lambda<Func<T, bool>>(Expression.Constant(false), originalLambda.Parameters);
        }

        return originalLambda;
    }

    static bool IsTrueConstant<T>(Expression<Func<T, bool>> lambda)
    {
        // Check if the lambda body is a constant expression with value true
        if (lambda.Body is ConstantExpression constant && constant.Value is bool constantValue)
        {
            return constantValue == true;
        }

        return false;
    }
}