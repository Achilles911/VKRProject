using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Linq;
using System.Threading.Tasks;
using WebApplication2.Data;
using WebApplication2.Data.Models;

namespace WebApplication2.Pages.Main
{
    public class CreateAssetModel : PageModel
    {
        private readonly ApplicationContext _context;

        public CreateAssetModel(ApplicationContext context)
        {
            _context = context;
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync(string objectName, int inventoryNumber, int quantity, int yearIntroduction, decimal initialCost, decimal residualValue, int usefulLife, string technicalCondition)
        {

            var maxId = _context.Assets.Max(a => (int?)a.id) ?? 0;
            // Увеличение максимального идентификатора на единицу
            var newId = maxId + 1;

            var asset = new Assets
            {
                id = newId,
                object_name = objectName,
                inventory_number = inventoryNumber,
                quantity = quantity,
                year_introduction = yearIntroduction,
                initial_cost = initialCost,
                residual_value = residualValue,
                useful_life = usefulLife,
                technical_condition = technicalCondition
            };

            await _context.Assets.AddAsync(asset);
            await _context.SaveChangesAsync();

            return RedirectToPage("/Main/menu");
        }
    }
}
