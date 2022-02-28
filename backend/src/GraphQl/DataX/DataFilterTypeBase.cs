using System;
using System.Linq;
using Database.GraphQl.Filters;
using HotChocolate.Configuration;
using HotChocolate.Data.Filters;
using HotChocolate.Types;
using HotChocolate.Types.Descriptors.Definitions;
using static HotChocolate.Internal.FieldInitHelper;

namespace Database.GraphQl.DataX
{
    public class DataFilterTypeBase<TData>
      : FilterInputType<TData>
      where TData : Data.IData
    {
        protected override void Configure(
          IFilterInputTypeDescriptor<TData> descriptor
          )
        {
            descriptor.BindFieldsExplicitly();
            descriptor.Field(x => x.Locale);
            descriptor.Field(x => x.Name);
            descriptor.Field(x => x.Description);
            descriptor.Field(x => x.ComponentId);
            descriptor.Field(x => x.CreatorId);
            descriptor.Field(x => x.CreatedAt);
            descriptor.Field(x => x.AppliedMethod);
            descriptor.Field(x => x.Approvals);
            descriptor.Field(x => x.Resources);
            descriptor.Field(x => x.Warnings);
        }

        // Inspired by https://github.com/ChilliCream/hotchocolate/blob/1319a4e3841beaec3bfbb282a909d2d1441f9c52/src/HotChocolate/Data/src/Data/Filters/FilterInputType.cs#L74
        // protected override FieldCollection<InputField> OnCompleteFields(
        //     ITypeCompletionContext context,
        //     InputObjectTypeDefinition definition)
        // {
        //     var fieldCollection = base.OnCompleteFields(context, definition);
        //     var index = 0;
        //     var fields = new InputField[fieldCollection.Count + 1];
        //     if (definition is FilterInputTypeDefinition { UseAnd: true, UseOr: true } def)
        //     {
        //         fields[index] = new NotField(context.DescriptorContext, index, def.Scope);
        //         index++;
        //     }
        //     foreach (var field in fieldCollection)
        //     {
        //         fields[index] = field;
        //         index++;
        //     }
        //     if (fields.Length > index)
        //     {
        //         Array.Resize(ref fields, index);
        //     }
        //     return CompleteFields(context, this, fields);
        // }
    }
}
