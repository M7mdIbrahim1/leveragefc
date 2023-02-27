using System;
using System.Linq.Expressions;

namespace Backend.Helper
{
   

    public static class ExpressionExtensions
    {
        public static string AsPath(this LambdaExpression expression)
        {
            if (expression == null) return null;

            var exp = expression.Body;
            TryParsePath(exp, out string path);
            return path;
        }
        private static bool TryParsePath(Expression expression, out string path)
        {
            path = null;
            var withoutConvert = RemoveConvert(expression);

            if (withoutConvert is MemberExpression memberExpression)
            {
                var thisPart = memberExpression.Member.Name;
                if (!TryParsePath(memberExpression.Expression, out string parentPart))
                {
                    return false;
                }
                path = parentPart == null ? thisPart : (parentPart + "." + thisPart);
            }
            else if (withoutConvert is MethodCallExpression callExpression)
            {
                if (callExpression.Method.Name == "Select"
                    && callExpression.Arguments.Count == 2)
                {
                    if (!TryParsePath(callExpression.Arguments[0], out string parentPart))
                    {
                        return false;
                    }
                    if (parentPart != null)
                    {
                        if (callExpression.Arguments[1] is LambdaExpression subExpression)
                        {
                            if (!TryParsePath(subExpression.Body, out string thisPart))
                            {
                                return false;
                            }
                            if (thisPart != null)
                            {
                                path = parentPart + "." + thisPart;
                                return true;
                            }
                        }
                    }
                }
                else if (callExpression.Method.Name == "Where")
                {
                    throw new NotSupportedException("Filtering an Include expression is not supported");
                }
                else if (callExpression.Method.Name == "OrderBy" || callExpression.Method.Name == "OrderByDescending")
                {
                    throw new NotSupportedException("Ordering an Include expression is not supported");
                }
                return false;
            }

            return true;
        }

        private static Expression RemoveConvert(Expression expression)
        {
            while (expression.NodeType == ExpressionType.Convert
                   || expression.NodeType == ExpressionType.ConvertChecked)
            {
                expression = ((UnaryExpression)expression).Operand;
            }

            return expression;
        }
    }
}