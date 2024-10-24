using System;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Database.Data;

[Owned]
public sealed class DataApproval
    : IApproval
{
    public DataApproval(
        DateTime timestamp,
        string signature,
        string keyFingerprint,
        string query,
        string response,
        Guid approverId
    )
    {
        Timestamp = timestamp;
        Signature = signature;
        KeyFingerprint = keyFingerprint;
        Query = query;
        Response = response;
        ApproverId = approverId;
    }

    public Guid ApproverId { get; private set; }
    public DateTime Timestamp { get; private set; }
    public string Signature { get; private set; }
    public string KeyFingerprint { get; private set; }
    public string Query { get; private set; }
    public string Response { get; private set; }
    public Publication? Publication { get; set; }
    public Standard? Standard { get; set; }
    [NotMapped] public IReference? Statement => Standard is not null ? Standard : Publication;
}