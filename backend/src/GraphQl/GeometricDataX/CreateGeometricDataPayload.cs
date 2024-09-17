using Database.Data;

namespace Database.GraphQl.GeometricDataX;

public sealed class CreateGeometricDataPayload
    : GeometricDataPayload<CreateGeometricDataError>
{
    public CreateGeometricDataPayload(
        GeometricData geometricData
    )
        : base(geometricData)
    {
    }

    public CreateGeometricDataPayload(
        CreateGeometricDataError error
    )
        : base(error)
    {
    }
}