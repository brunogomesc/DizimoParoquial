using Microsoft.AspNetCore.Http.HttpResults;
using System.Reflection.Emit;
using System.Xml.Linq;
using System;

namespace DizimoParoquial.Models
{
    public class TithePayer
    {

        public int TithePayerId { get; set; }

		public string Name { get; set; } = string.Empty;

        public string? Document { get; set; }

        public DateTime DateBirth { get; set; }

        public string? Email { get; set; }

        public string? PhoneNumber { get; set; }

        public string? Address { get; set; }

        public string? Number { get; set; }

        public string? ZipCode { get; set; }

        public string? Neighborhood { get; set; } 

        public string? Complement { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public byte[]? TermFile { get; set; }

        public string TermFile64Base { get; set; } = string.Empty;

        public string? Extension { get; set; }

        public int UserId { get; set; }

    }
}
