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

        public string Email { get; set; } = string.Empty;

        public string? PhoneNumber { get; set; }

        public string Address { get; set; } = string.Empty;

        public string Number { get; set; } = string.Empty;

        public string ZipCode { get; set; } = string.Empty;

        public string Neighborhood { get; set; } = string.Empty;

        public string Complement { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public byte[]? TermFile { get; set; }

        public string TermFile64Base { get; set; } = string.Empty;

        public string? Extension { get; set; }

        public int UserId { get; set; }

    }
}
