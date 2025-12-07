namespace HairStudio.Services.DTOs.Services
{
    public class ServicesByGenderDTO
    {
        public List<ServiceSummaryDTO> MaleServices { get; set; } = new List<ServiceSummaryDTO>();
        public List<ServiceSummaryDTO> FemaleServices { get; set; } = new List<ServiceSummaryDTO>();
    }
}
