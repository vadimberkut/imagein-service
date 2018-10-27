using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Imagein.Data.Helpers
{
    /// <summary>
    /// Extends EF EntityMaterializerSource. 
    /// Performs some data transformations when geting data frm DB
    /// </summary>
    public class CustomEntityMaterializeSource : EntityMaterializerSource
    {
        private static readonly MethodInfo NormilizeDateTimeMethod = typeof(DateTimeMapper).GetMethod(nameof(DateTimeMapper.Normalize));
        private static readonly MethodInfo NormilizeNullableDateTimeMethod = typeof(DateTimeMapper).GetMethod(nameof(DateTimeMapper.NormalizeNullable));

        public override Expression CreateReadValueExpression(Expression valueBuffer, Type type, int index, IPropertyBase property)
        {
            if(type == typeof(DateTime))
            {
                return Expression.Call(
                    NormilizeDateTimeMethod,
                    base.CreateReadValueExpression(valueBuffer, type, index, property)
                );
            }

            if (type == typeof(DateTime?))
            {
                return Expression.Call(
                    NormilizeNullableDateTimeMethod,
                    base.CreateReadValueExpression(valueBuffer, type, index, property)
                );
            }

            return base.CreateReadValueExpression(valueBuffer, type, index, property);  
        }
    }
}
