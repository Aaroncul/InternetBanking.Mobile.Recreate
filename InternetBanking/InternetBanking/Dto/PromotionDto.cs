using System;

namespace InternetBanking.Dto
{
    public class PromotionDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public string ActionUri { get; set; }
        public bool Active { get; set; }
        public DateTime Created { get; set; }
        public DateTime? ActiveFrom { get; set; }
        public DateTime? ActiveTo { get; set; }
    }
}
