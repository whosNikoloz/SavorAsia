using SavorAsia.Models.Menu;

namespace SavorAsia.Models
{
    public class ItemViewModel
    {
        public List<NoodlesModel> noodles { get; set; }
        public List<RamenModel> ramen { get; set; }
        public List<DrinkModel> drink { get; set; }
        public List<RiceModel> rice { get; set; }
    }
}
