namespace SNHU_Capstone_Project.Models.Entities
{
    public class Customer
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = "";
        public Service ChosenService { get; set; }
        public DateTime Added { get; set; }

        public enum Service
        {
            None = 0,
            Brokerage = 1,
            Retirement = 2
        }
    }
}
