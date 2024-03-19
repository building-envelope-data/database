using HotChocolate.Types;

namespace Database.GraphQl.Numerations
{
    public sealed class NumerationType
      : ObjectType<Data.Numeration>
    {
        protected override void Configure(
            IObjectTypeDescriptor<Data.Numeration> descriptor
            )
        {
        }
    }
}