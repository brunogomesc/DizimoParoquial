using Microsoft.AspNetCore.Mvc.Rendering;

namespace DizimoParoquial.Utils
{
    public class AmountPages
    {

        public static List<SelectListItem> GetAmountPageInput()
        {

            List<SelectListItem> items =
            [
                new SelectListItem { Text = "10", Value = "10", Selected = true },
                new SelectListItem { Text = "15", Value = "15" },
                new SelectListItem { Text = "25", Value = "25"},
                new SelectListItem { Text = "50", Value = "50" },
            ];

            return items;

        }

    }
}
