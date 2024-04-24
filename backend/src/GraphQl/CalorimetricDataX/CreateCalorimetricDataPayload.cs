using Database.Data;

namespace Database.GraphQl.CalorimetricDataX;

public sealed class CreateCalorimetricDataPayload
    : CalorimetricDataPayload<CreateCalorimetricDataError>
{
    public CreateCalorimetricDataPayload(
        CalorimetricData calorimetricData
    )
        : base(calorimetricData)
    {
    }

    public CreateCalorimetricDataPayload(
        CreateCalorimetricDataError error
    )
        : base(error)
    {
    }
}