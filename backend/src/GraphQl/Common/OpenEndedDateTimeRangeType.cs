using HotChocolate.Types;
using NpgsqlTypes;
using DateTime = System.DateTime;

namespace Database.GraphQl.Common;

public sealed class OpenEndedDateTimeRangeType
    : ObjectType<NpgsqlRange<DateTime>>
{
    protected override void Configure(
        IObjectTypeDescriptor<NpgsqlRange<DateTime>> descriptor
    )
    {
        descriptor.BindFieldsExplicitly();

        var suffixedName = nameof(OpenEndedDateTimeRangeType);
        descriptor.Name(suffixedName.Remove(suffixedName.Length - "Type".Length));

        descriptor
            .Field("from")
            .Type<DateTimeType>()
            .Resolve(context =>
                {
                    var range = context.Parent<NpgsqlRange<DateTime>>();
                    return range.LowerBoundInfinite
                        ? null
                        : range.LowerBound;
                }
            );

        descriptor
            .Field("until")
            .Type<DateTimeType>()
            .Resolve(context =>
                {
                    var range = context.Parent<NpgsqlRange<DateTime>>();
                    return range.UpperBoundInfinite
                        ? null
                        : range.UpperBound;
                }
            );
    }
}