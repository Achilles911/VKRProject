using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using OfficeOpenXml;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WebApplication2.Data;
using WebApplication2.Data.Models;
using ZXing;
using System;


namespace WebApplication2.Pages.Main
{
    public class MenuModel : PageModel
    {
        private readonly IApplicationContext _context;

        public MenuModel(IApplicationContext context)
        {
            _context = context;
        }

        public IList<Assets> Assets { get; set; }
        public string SearchMessage { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            Assets = await _context.GetAssetsAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostCreateExcel()//Создание Excel таблицы
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            var assets = await _context.GetAssetsAsync();// Получаем все записи из таблицы БД
            using (var package = new ExcelPackage())// Создаем новый пакет Excel
            {

                var worksheet = package.Workbook.Worksheets.Add("Assets"); // Добавляем новый лист

                // Заголовки столбцов
                worksheet.Cells[1, 1].Value = "Наименование";
                worksheet.Cells[1, 2].Value = "Инвентарный номер";
                worksheet.Cells[1, 3].Value = "Количество";
                worksheet.Cells[1, 4].Value = "Год ввода";
                worksheet.Cells[1, 5].Value = "Первоначальная стоимость";
                worksheet.Cells[1, 6].Value = "Остаточная стоимость";
                worksheet.Cells[1, 7].Value = "Срок полезного использования";
                worksheet.Cells[1, 8].Value = "Техническое состояние";

                // Данные из БД
                int row = 2;
                foreach (var asset in assets)
                {
                    worksheet.Cells[row, 1].Value = asset.object_name;
                    worksheet.Cells[row, 2].Value = asset.inventory_number;
                    worksheet.Cells[row, 3].Value = asset.quantity;
                    worksheet.Cells[row, 4].Value = asset.year_introduction;
                    worksheet.Cells[row, 5].Value = asset.initial_cost;
                    worksheet.Cells[row, 6].Value = asset.residual_value;
                    worksheet.Cells[row, 7].Value = asset.useful_life;
                    worksheet.Cells[row, 8].Value = asset.technical_condition;
                    row++;
                }


                var stream = new MemoryStream(); // Сохраняем Excel-файл
                package.SaveAs(stream);
                stream.Position = 0;
                // Возвращаем Excel-файл как результат
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Assets.xlsx");
            }
        }


        public async Task<IActionResult> OnPostSearchAsync(string searchString)//поиск предметов
        {
            Assets = await _context.GetAssetsAsync();

            if (string.IsNullOrWhiteSpace(searchString))
            {
                // Если строка поиска пустая, выводим сообщение
                SearchMessage = "Заполните эту строку";
                return Page();
            }

            if (int.TryParse(searchString, out int inventoryNumber) && inventoryNumber != 0)
            {
                // Если введенное значение является числом и не равно нулю
                var assets = await _context.GetAssetsAsync();

                // Применяем метод Where к списку активов
                Assets = assets
                    .Where(a => a.inventory_number == inventoryNumber)
                    .ToList();



                if (Assets.Any())
                {
                    // Если найдены соответствующие активы, возвращаем страницу с этими активами
                    return Page();
                }
                else
                {
                    // Если активы не найдены, выводим сообщение
                    Assets = await _context.GetAssetsAsync();
                    SearchMessage = "Не найдено предметов";
                    return Page();
                }
            }
            else
            {
                // Если введенное значение не является числом или равно нулю, ничего не делаем
                return Page();
            }
        }
        //public async Task<IActionResult> OnPostSearchByQRCodeAsync(IFormFile qrImage)
        //{
        //    if (qrImage == null || qrImage.Length == 0)
        //    {
        //        // Обработка случая, когда файл не был загружен
        //        return Page();
        //    }

        //    try
        //    {
        //        using (var memoryStream = new MemoryStream())
        //        {
        //            await qrImage.CopyToAsync(memoryStream);
        //            memoryStream.Position = 0;

        //            var barcodeReader = new BarcodeReader();
        //            var imageBytes = memoryStream.ToArray(); // Преобразование изображения в массив байтов
        //            var result = barcodeReader.Decode(imageBytes); // Декодирование массива байтов

        //            if (result != null && int.TryParse(result.Text, out int inventoryNumber))
        //            {
        //                var asset = await _context.FindAssetAsync(inventoryNumber);
        //                if (asset != null)
        //                {
        //                    // Найдена материальная ценность по инвентарному номеру из QR-кода
        //                    return RedirectToPage("/Main/MenuAsset", new { id = asset.id });
        //                }
        //                else
        //                {
        //                    // Материальная ценность не найдена по инвентарному номеру из QR-кода
        //                    SearchMessage = $"Материальная ценность с инвентарным номером {inventoryNumber} не найдена";
        //                }
        //            }
        //            else
        //            {
        //                // Не удалось распознать QR-код как число инвентарного номера
        //                SearchMessage = "QR-код не содержит действительного инвентарного номера";
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        // Обработка исключений при декодировании QR-кода
        //        SearchMessage = "Произошла ошибка при обработке QR-кода: " + ex.Message;
        //    }

        //    // Возвращаем страницу с сообщением о результате поиска
        //    return Page();
        //}










    }
}
