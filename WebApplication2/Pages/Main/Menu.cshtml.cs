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

        public async Task<IActionResult> OnPostCreateExcel()//�������� Excel �������
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            var assets = await _context.GetAssetsAsync();// �������� ��� ������ �� ������� ��
            using (var package = new ExcelPackage())// ������� ����� ����� Excel
            {

                var worksheet = package.Workbook.Worksheets.Add("Assets"); // ��������� ����� ����

                // ��������� ��������
                worksheet.Cells[1, 1].Value = "������������";
                worksheet.Cells[1, 2].Value = "����������� �����";
                worksheet.Cells[1, 3].Value = "����������";
                worksheet.Cells[1, 4].Value = "��� �����";
                worksheet.Cells[1, 5].Value = "�������������� ���������";
                worksheet.Cells[1, 6].Value = "���������� ���������";
                worksheet.Cells[1, 7].Value = "���� ��������� �������������";
                worksheet.Cells[1, 8].Value = "����������� ���������";

                // ������ �� ��
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


                var stream = new MemoryStream(); // ��������� Excel-����
                package.SaveAs(stream);
                stream.Position = 0;
                // ���������� Excel-���� ��� ���������
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Assets.xlsx");
            }
        }


        public async Task<IActionResult> OnPostSearchAsync(string searchString)//����� ���������
        {
            Assets = await _context.GetAssetsAsync();

            if (string.IsNullOrWhiteSpace(searchString))
            {
                // ���� ������ ������ ������, ������� ���������
                SearchMessage = "��������� ��� ������";
                return Page();
            }

            if (int.TryParse(searchString, out int inventoryNumber) && inventoryNumber != 0)
            {
                // ���� ��������� �������� �������� ������ � �� ����� ����
                var assets = await _context.GetAssetsAsync();

                // ��������� ����� Where � ������ �������
                Assets = assets
                    .Where(a => a.inventory_number == inventoryNumber)
                    .ToList();



                if (Assets.Any())
                {
                    // ���� ������� ��������������� ������, ���������� �������� � ����� ��������
                    return Page();
                }
                else
                {
                    // ���� ������ �� �������, ������� ���������
                    Assets = await _context.GetAssetsAsync();
                    SearchMessage = "�� ������� ���������";
                    return Page();
                }
            }
            else
            {
                // ���� ��������� �������� �� �������� ������ ��� ����� ����, ������ �� ������
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
            // ������� ������ BarcodeReader ��� ������������� QR-����
            var barcodeReader = new BarcodeReader();

            // ������ ����������� �� ������ � ���� ������� ������
            byte[] imageBytes;
            using (var memoryStream = new MemoryStream())
            {
                imageStream.CopyTo(memoryStream);
                imageBytes = memoryStream.ToArray();
            }

            // ���������� QR-��� � ������� ZXing
            var result = barcodeReader.Decode(imageBytes);

            // ���� ������� ���������� QR-���, ���������� ��� ����������
            if (result != null)
            {
                return result.Text;
            }
            else
            {
                // ���� QR-��� �� ������� ����������, ���������� null ��� ����������� ����������
                throw new Exception("QR-��� �� ������� ����������.");
            }
        }
    }
}
