using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using OfficeOpenXml;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WebApplication2.Data;
using WebApplication2.Data.Models;
using Microsoft.AspNetCore.Http;
using System.Drawing;
using System;
using QRCoder;
using ZXing;



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
        public async Task<IActionResult> OnPostDecodeQrCodeAsync(IFormFile qrCodeImage)
        {
            if (qrCodeImage == null || qrCodeImage.Length == 0)
            {
                // Handle invalid input
                return BadRequest("No image uploaded");
            }

            // Check if the uploaded file is an image
            if (!qrCodeImage.ContentType.StartsWith("image/"))
            {
                return BadRequest("Uploaded file is not an image");
            }

            try
            {
                // Read the uploaded image into a MemoryStream
                using (var memoryStream = new MemoryStream())
                {
                    await qrCodeImage.CopyToAsync(memoryStream);

                    // Decode the QR code from the image
                    string decodedText = DecodeQrCode(memoryStream);

                    // Try parsing the decoded text as an integer (inventory number)
                    if (!int.TryParse(decodedText, out int inventoryNumber))
                    {
                        // If parsing fails, return an error
                        return BadRequest("Decoded text is not a valid inventory number");
                    }

                    // Now use the inventory number to search for the asset
                    Assets = await _context.GetAssetsAsync();
                    var asset = Assets.FirstOrDefault(a => a.inventory_number == inventoryNumber);

                    if (asset != null)
                    {
                        // If the asset is found, return the page with the asset details
                        return Page();
                    }
                    else
                    {
                        // If the asset is not found, return the page with a message
                        Assets = await _context.GetAssetsAsync();
                        SearchMessage = "Asset not found";
                        return Page();
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle any errors that occur during the process
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }


        private string DecodeQrCode(Stream imageStream)
        {
            // Создаем объект BarcodeReader для декодирования QR-кода
            var barcodeReader = new BarcodeReader();

            // Читаем изображение из потока в виде массива байтов
            byte[] imageBytes;
            using (var memoryStream = new MemoryStream())
            {
                imageStream.CopyTo(memoryStream);
                imageBytes = memoryStream.ToArray();
            }

            // Декодируем QR-код с помощью ZXing
            var result = barcodeReader.Decode(imageBytes);

            // Если удалось распознать QR-код, возвращаем его содержимое
            if (result != null)
            {
                return result.Text;
            }
            else
            {
                // Если QR-код не удалось распознать, возвращаем null или выбрасываем исключение
                throw new Exception("QR-код не удалось распознать.");
            }
        }
    }
}
