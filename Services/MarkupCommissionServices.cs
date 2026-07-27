using FlightBooking.Models;

namespace FlightBooking.Services
{
    // DTO for updates - defined in same file
    public class MarkupCommissionUpdate
    {
        public int? UserId { get; set; }
        public string? AirlineCode { get; set; }
        public string? MarkupType { get; set; }
        public string? CommissionType { get; set; }
        public decimal? Markup { get; set; }
        public decimal? Commission { get; set; }
        public bool? IsActive { get; set; }
    }

    public static class MarkupCommissionServices
    {
        static List<MarkupCommission> MarkupCommissions { get; }

        static int nextId = 2;

        // Static constructor - FIXED: Using decimal values
        static MarkupCommissionServices()
        {
            MarkupCommissions = new List<MarkupCommission>
            {
                new MarkupCommission
                {
                    Id = 1,
                    UserId = 1,
                    AirlineCode = "AA",
                    MarkupType = "Percentage",
                    CommissionType = "Flat",
                    Markup = 10m,     
                    Commission = 200m,
                    IsActive = true,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                }
            };
        }

        // Get all users
        public static List<MarkupCommission> GetAll() => MarkupCommissions;

        // Get User by Id
        public static MarkupCommission? Get(int id)
        {
            return MarkupCommissions.FirstOrDefault(u => u.Id == id);
        }

        // Add a New MarkupCommission
        public static void Add(MarkupCommission markupCommission)
        {
            if (markupCommission == null)
                throw new ArgumentNullException(nameof(markupCommission));

            // Required fields - FIXED: Proper decimal checks
            if (markupCommission.UserId == 0)
                throw new ArgumentException("User ID is required");
            if (string.IsNullOrWhiteSpace(markupCommission.MarkupType))
                throw new ArgumentException("Markup Type is required");
            if (string.IsNullOrWhiteSpace(markupCommission.CommissionType))
                throw new ArgumentException("Commission Type is required");
            if (markupCommission.Markup <= 0)
                throw new ArgumentException("Markup must be greater than 0");
            if (markupCommission.Commission <= 0)
                throw new ArgumentException("Commission must be greater than 0");

            // Check for duplicates
            if (string.IsNullOrWhiteSpace(markupCommission.AirlineCode))
            {
                bool globalExists = MarkupCommissions.Any(m => string.IsNullOrWhiteSpace(m.AirlineCode));
                if (globalExists)
                    throw new InvalidOperationException("A global commission setting already exists. Only one global entry is allowed.");
            }
            else
            {
                bool airlineCodeExists = MarkupCommissions.Any(m => m.AirlineCode == markupCommission.AirlineCode);
                if (airlineCodeExists)
                    throw new InvalidOperationException(
                        $"A commission is already set for airline code '{markupCommission.AirlineCode}'");
            }

            markupCommission.Id = nextId++;
            markupCommission.CreatedAt = DateTime.Now;
            markupCommission.UpdatedAt = DateTime.Now;
            MarkupCommissions.Add(markupCommission);
        }

        // Update a MarkupCommission - FIXED: Using MarkupCommissionUpdate DTO
        public static void Update(int id, MarkupCommissionUpdate updateDto)
        {
            var index = MarkupCommissions.FindIndex(u => u.Id == id);
            if (index == -1)
                throw new KeyNotFoundException($"MarkupCommission with ID {id} not found");

            var existing = MarkupCommissions[index];

            // Store the original AirlineCode for comparison
            var originalAirlineCode = existing.AirlineCode;

            // Determine the new AirlineCode (if being updated)
            string newAirlineCode = updateDto.AirlineCode ?? originalAirlineCode;

            // VALIDATION: Check for duplicates BEFORE updating
            if (string.IsNullOrWhiteSpace(newAirlineCode))
            {
                bool anotherGlobalExists = MarkupCommissions
                    .Any(m => m.Id != id && string.IsNullOrWhiteSpace(m.AirlineCode));

                if (anotherGlobalExists)
                    throw new InvalidOperationException("A global commission setting already exists. Only one global entry is allowed.");
            }
            else
            {
                bool airlineCodeExists = MarkupCommissions
                    .Any(m => m.Id != id && m.AirlineCode == newAirlineCode);

                if (airlineCodeExists)
                    throw new InvalidOperationException(
                        $"A commission is already set for airline code '{newAirlineCode}'");
            }

            // Update only the fields that were provided - FIXED: Proper null checks
            if (updateDto.UserId.HasValue)
                existing.UserId = updateDto.UserId.Value;

            if (updateDto.AirlineCode != null)
                existing.AirlineCode = updateDto.AirlineCode;

            if (updateDto.MarkupType != null)
                existing.MarkupType = updateDto.MarkupType;

            if (updateDto.CommissionType != null)
                existing.CommissionType = updateDto.CommissionType;

            if (updateDto.Markup.HasValue)
                existing.Markup = updateDto.Markup.Value;

            if (updateDto.Commission.HasValue)
                existing.Commission = updateDto.Commission.Value;

            if (updateDto.IsActive.HasValue)
                existing.IsActive = updateDto.IsActive.Value;

            // Always update the timestamp
            existing.UpdatedAt = DateTime.Now;

            MarkupCommissions[index] = existing;
        }

        // Delete a User
        public static void Delete(int id)
        {
            var markupCommission = Get(id);
            if (markupCommission is null)
                throw new KeyNotFoundException($"MarkupCommission with ID {id} not found");
            MarkupCommissions.Remove(markupCommission);
        }
    }
}