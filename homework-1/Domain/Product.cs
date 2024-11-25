namespace Domain
{
    public record Product
    {
        public Product(string id, DateTime date, int sales, int stock)
        {
            ValidateId(id);
            Id = id;
            Date = date;
            Sales = sales;
            Stock = stock;
        }

        public string Id { get; }
        public DateTime Date { get; }
        public int Sales { get; }
        public int Stock { get; }

        private void ValidateId(string id)
        {
            if(id == null)
                throw new ArgumentNullException("Id");

            if (id.Length == 0)
                throw new ArgumentException("Id must be non-zero lenght");
        }
    }
}
