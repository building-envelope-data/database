using HotChocolate.Types;

namespace Database.GraphQl.Publications
{
    public sealed class PublicationType
      : ObjectType<Data.Publication>
    {
        protected override void Configure(
            IObjectTypeDescriptor<Data.Publication> descriptor
            )
        {
        }
    }
}