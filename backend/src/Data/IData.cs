using System;
using System.Collections.Generic;

namespace Database.Data;

public interface IData
{
    Guid ComponentId { get; }
    string? Name { get; }
    string? Description { get; }
    string[] Warnings { get; }
    Guid CreatorId { get; }
    DateTime CreatedAt { get; }
    AppliedMethod AppliedMethod { get; }
    ICollection<DataApproval> Approvals { get; }

    ICollection<GetHttpsResource> Resources { get; }

    // ResponseApproval Approval { get; }
    string Locale { get; }
}