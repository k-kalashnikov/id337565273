using System;

namespace SP.Messenger.Client.Shared
{
    public class VotingContractor
    {
        public string ContractorName { get; set; }
     
        public long ContractorId { get; set; }
        // ценовое предложение
        public decimal? PriceOffer { get; set; }
        //Отклонение от лучших ц.п.
        public decimal? DeviationBestPrice { get; set; }
        //Срок поставки
        public DateTime? Term { get; set; }
        //Срок поставки/отклонение
        public int? TermDeviation { get; set; }
        //Отсрочка платежа
        public int? DefermentPayment { get; set; }
        //Отсрочка платежа/отклонение
        public int? DefermentDeviation { get; set; }
    }
}