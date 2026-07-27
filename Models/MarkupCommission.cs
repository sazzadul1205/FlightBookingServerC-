namespace FlightBooking.Models
{
    public class MarkupCommission
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public string? AirlineCode { get; set; }

        public string MarkupType { get; set; }
        public string CommissionType { get; set; }
        public decimal Markup { get; set; }
        public decimal Commission { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}