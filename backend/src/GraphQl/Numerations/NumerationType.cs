using Database.Data;
using HotChocolate.Types;

namespace Database.GraphQl.Numerations;

public sealed class NumerationType
    : ObjectType<Numeration>
{
    protected override void Configure(
        IObjectTypeDescriptor<Numeration> descriptor
    )
    {
    }
}