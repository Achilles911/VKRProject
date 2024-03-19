namespace WebApplication2.Data.Models
{
    public class Assets
    {
        public int id { get; set; }//ключ номер
        public string object_name { get; set; }//имя и тип 
        public int inventory_number { get; set; }// инвентарный номер
        public int quantity { get; set; }//kol-vo
        public int year_introduction { get; set; }//год ввода в эксплуатацию
        public decimal initial_cost { get; set; }//первоначальная стоимость
        public decimal residual_value { get; set; }//остаточная стоимость
        public int useful_life { get; set; }//срок полезного использования
        public string technical_condition { get; set; }//техническое состояние

    }
}
