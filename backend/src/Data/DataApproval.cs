using System;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using Database.Data;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Data
{
    [Owned]
    public sealed class DataApproval
    : IApproval
    {
        public DateTime Timestamp { get; private set; }
        public string Signature { get; private set; }
        public string KeyFingerprint { get; private set; }
        public string Query { get; private set; }
        public string Response { get; private set; }
        public Guid ApproverId { get; private set; }
        public Publication? Publication { get; set; }
        public Standard? Standard { get; set; }
        [NotMapped]
        public IReference? Statement
        {
            get => Standard is not null ? Standard : Publication;
            // set {if(value is Standard)  Standard = (Standard)value; else Publication = (Publication)value;}
        }

        public DataApproval(
          DateTime timestamp,
          string signature,
          string keyFingerprint,
          string query,
          string response,
          Guid approverId,
          Publication publication,
          Standard standard
          
        )
        {
            Timestamp = timestamp;
            Signature = signature;
            KeyFingerprint = keyFingerprint;
            Query = query;
            Response = response;
            ApproverId = approverId;
            Publication = publication;
            Standard = standard;
        }
    }
}
