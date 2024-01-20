namespace RabbitmqWatermark.Services
{
    public class productImageCreatedEvent //RabbitMq'a gönderilecek mesajlar "Text Body"(Emir kipleriyl adlandırılır) ve "Event"(geçmiş zaman kipiyle) olarak 2 tipe ayrılır. Nesne oluşturulacağında, resim, pdf vs kaydedileceğinde bunlar text veya binary olarak gönderilir. Ancak biz burda resmi normal yükleyip üzerindeki yazıyı rabbitMq'dan çekeceğimiz için Event kullanacağız. 
    {
        public string ImageName { get; set; }
        public string ImageUrl { get; set; }
    }
}
