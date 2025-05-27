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

        //getting all properties in the filter object
        var filterProperties = filter.GetType().GetProperties();

        //lets create the x=> part of the lambda
        ParameterExpression parameter = Expression.Parameter(typeof(T), "x");
        Expression expression = Expression.Constant(true);

        foreach (var property in filterProperties)
        {
             //we get the property of the database model that has the same name as the property of the filter
            var modelProperty = typeof(T).GetProperty(property.Name);
            
            //we check what property is not present in the model but it exists on the filter and we skip it
            //Why? Maybe there are some properties that are not directly stored in the database but they are calculated in a specific way (smth we will handle later)
            if (modelProperty == null)
            {
                continue;
            }

            //we get the value of the filter property at runtime
            //the second parameter is for indexed properties ex. arrays. Since we are dealing with regular properties pass null
            var propertyValue = property.GetValue(filter, null);

            //checking the case where the property type is Datetime
            if (property.PropertyType == typeof(DateTime))
            {
                //sometimes the datetime is not passed as null(even if nullable), but as the default value of DateTime.MinValue
                //DateTime.MinValue == 01/01/0001 00:00:00
               if (propertyValue == null || (propertyValue.HasValue && (DateTime)propertyValue.Value == DateTime.MinValue))
                    continue;

                //here is created the part x=>x.ModelProperty  that takes the exact property we are searching for in the object
                var memberExpression = Expression.Property(parameter, modelProperty);

                //here we get the value that should be compared
                var filterDate = ((DateTime)propertyValue!);
                   
                //here the expression is finalized with the check == filterDate,the value of the filter, with the type filterDate.Date to not check the time
                BinaryExpression dateEqualityExpression = Expression.Equal(
                    Expression.Property(memberExpression, nameof(DateTime.Date)),
                    Expression.Constant(filterDate.Date)
                );
                //this adds the condition to the filter expression we are creating with an &&. 
                //So the expresion turns in what we had in that moment && the date check
                expression = Expression.AndAlso(expression, dateEqualityExpression);

            }
            else if (propertyValue != null)
            {
                //Here we add the checks for other property types
                var propertyType = property.PropertyType;

                var memberExpression = Expression.Property(parameter, property.Name);

                var filterValue = Expression.Constant(propertyValue);

                BinaryExpression equalityExpression = null;

                // for properties we cannot use the toupper and trim methods the same as with constants. We have to get them and then make a call.
                if (propertyType == typeof(string) && propertyValue is string stringValue && !string.IsNullOrEmpty(stringValue)
                    && memberExpression.Type == typeof(String))
                {
                    propertyValue = stringValue.Trim().ToUpper();

                    var toUpperMethod = typeof(string).GetMethod("ToUpper", Type.EmptyTypes);
                    var trimMethod = typeof(string).GetMethod("Trim", Type.EmptyTypes);

                    var toUpperCall = Expression.Call(memberExpression, toUpperMethod!);
                    var trimCall = Expression.Call(toUpperCall, trimMethod!);

                    //creating the expression
                    equalityExpression = Expression.Equal(trimCall, Expression.Constant(propertyValue));
                    expression = Expression.AndAlso(expression, equalityExpression);
                }
                else
                {
                    //creating the expression
                    equalityExpression = Expression.Equal(memberExpression, filterValue);
                    expression = Expression.AndAlso(expression, equalityExpression);
                }
            }
        }

        //check if there is an additional filter
        if (additionalFilter != null)
        { 
            //attach with && through AndAlso to the created expression
            var additionalFilterExpression = Expression.Invoke(Expression.Constant(additionalFilter), parameter);
            expression = Expression.AndAlso(expression, additionalFilterExpression);
        }

        var lambda = Expression.Lambda<Func<T, bool>>(expression, parameter);

        var modifiedLambda = ModifyLambda(lambda);

        return query.Where(modifiedLambda);
    }

    //this method adds the x. before the properties that are in the database model, because we dont need it with constants
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
