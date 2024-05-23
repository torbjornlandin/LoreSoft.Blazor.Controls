using System.Linq.Expressions;
using System.Reflection;

using LoreSoft.Blazor.Controls.Extensions;

using Microsoft.AspNetCore.Components;

namespace LoreSoft.Blazor.Controls;

public class QueryBuilderField<TItem> : ComponentBase
{
    [CascadingParameter(Name = "QueryBuilder")]
    protected QueryBuilder<TItem> QueryBuilder { get; set; }

    [Parameter]
    public Expression<Func<TItem, object>> Field { get; set; }

    [Parameter]
    public List<string> Operators { get; set; }

    [Parameter]
    public string InputType { get; set; }

    [Parameter]
    public string Title { get; set; }


    [Parameter]
    public RenderFragment<QueryFilter> ValueTemplate { get; set; }

    [Parameter]
    public RenderFragment<QueryFilter> OperatorTemplate { get; set; }


    public string Name { get; set; }

    public Type Type { get; set; }


    public List<string> CurrentOperators { get; set; }

    public string CurrentInputType { get; set; }

    public string CurrentTitle { get; set; }


    protected override void OnInitialized()
    {
        if (QueryBuilder == null)
            throw new InvalidOperationException("QueryColumn must be child of QueryBuilder");

        if (Field == null)
            throw new InvalidOperationException("QueryColumn Property parameter is required");

        // register with parent grid
        QueryBuilder.AddField(this);
    }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        UpdateProperty();
        UpdateOperators();
        UpdateInputType();
        UpdateTitle();
    }


    private void UpdateTitle()
    {
        if (Title.HasValue())
        {
            CurrentTitle = Title;
            return;
        }

        CurrentTitle = Name.ToTitle();
    }

    private void UpdateInputType()
    {
        if (InputType.HasValue())
        {
            CurrentInputType = InputType;
            return;
        }

        CurrentInputType = GetInputType(Type);
    }

    private void UpdateOperators()
    {
        if (Operators != null && Operators.Count > 0)
        {
            CurrentOperators = Operators;
            return;
        }

        CurrentOperators = [QueryOperators.Equal, QueryOperators.NotEqual];

        if (Type == typeof(string))
        {
            CurrentOperators.Add(QueryOperators.Contains);
            CurrentOperators.Add(QueryOperators.NotContains);

            CurrentOperators.Add(QueryOperators.StartsWith);
            CurrentOperators.Add(QueryOperators.EndsWith);
        }
        else if (IsComparableType(Type))
        {
            CurrentOperators.Add(QueryOperators.GreaterThan);
            CurrentOperators.Add(QueryOperators.GreaterThanOrEqual);

            CurrentOperators.Add(QueryOperators.LessThan);
            CurrentOperators.Add(QueryOperators.LessThanOrEqual);
        }

        CurrentOperators.Add(QueryOperators.IsNull);
        CurrentOperators.Add(QueryOperators.IsNotNull);
    }

    private void UpdateProperty()
    {
        MemberInfo memberInfo = null;

        if (Field?.Body is MemberExpression memberExpression)
            memberInfo = memberExpression.Member;
        else if (Field?.Body is UnaryExpression { Operand: MemberExpression memberOperand })
            memberInfo = memberOperand.Member;

        if (memberInfo is PropertyInfo propertyInfo)
        {
            Name = propertyInfo.Name;
            Type = propertyInfo.PropertyType;
        }
        else if (memberInfo is FieldInfo fieldInfo)
        {
            Name = fieldInfo.Name;
            Type = fieldInfo.FieldType;
        }
        else
        {
            Name = memberInfo.Name;
        }
    }


    private bool IsComparableType(Type TargetType)
    {
        if (TargetType == typeof(int))
            return true;
        if (TargetType == typeof(long))
            return true;
        if (TargetType == typeof(short))
            return true;
        if (TargetType == typeof(float))
            return true;
        if (TargetType == typeof(double))
            return true;
        if (TargetType == typeof(decimal))
            return true;
        if (TargetType == typeof(DateTime))
            return true;
        if (TargetType == typeof(DateTimeOffset))
            return true;
        if (TargetType == typeof(DateOnly))
            return true;
        if (TargetType == typeof(TimeOnly))
            return true;
        if (TargetType == typeof(TimeSpan))
            return true;

        return false;
    }

    private string GetInputType(Type TargetType)
    {
        if (TargetType == typeof(int))
            return "number";
        if (TargetType == typeof(long))
            return "number";
        if (TargetType == typeof(short))
            return "number";
        if (TargetType == typeof(float))
            return "number";
        if (TargetType == typeof(double))
            return "number";
        if (TargetType == typeof(decimal))
            return "number";
        if (TargetType == typeof(DateTime))
            return "date";
        if (TargetType == typeof(DateTimeOffset))
            return "date";
        if (TargetType == typeof(DateOnly))
            return "date";
        if (TargetType == typeof(TimeOnly))
            return "time";
        if (TargetType == typeof(TimeSpan))
            return "time";

        return "text";
    }
}
