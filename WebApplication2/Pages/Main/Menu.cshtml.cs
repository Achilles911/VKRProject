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
        //public async Task<IActionResult> OnPostSearchByQRCodeAsync(IFormFile qrImage)
        //{
        //    if (qrImage == null || qrImage.Length == 0)
        //    {
        //        // ��������� ������, ����� ���� �� ��� ��������
        //        return Page();
        //    }

        //    try
        //    {
        //        using (var memoryStream = new MemoryStream())
        //        {
        //            await qrImage.CopyToAsync(memoryStream);
        //            memoryStream.Position = 0;

        //            var barcodeReader = new BarcodeReader();
        //            var imageBytes = memoryStream.ToArray(); // �������������� ����������� � ������ ������
        //            var result = barcodeReader.Decode(imageBytes); // ������������� ������� ������

        //            if (result != null && int.TryParse(result.Text, out int inventoryNumber))
        //            {
        //                var asset = await _context.FindAssetAsync(inventoryNumber);
        //                if (asset != null)
        //                {
        //                    // ������� ������������ �������� �� ������������ ������ �� QR-����
        //                    return RedirectToPage("/Main/MenuAsset", new { id = asset.id });
        //                }
        //                else
        //                {
        //                    // ������������ �������� �� ������� �� ������������ ������ �� QR-����
        //                    SearchMessage = $"������������ �������� � ����������� ������� {inventoryNumber} �� �������";
        //                }
        //            }
        //            else
        //            {
        //                // �� ������� ���������� QR-��� ��� ����� ������������ ������
        //                SearchMessage = "QR-��� �� �������� ��������������� ������������ ������";
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        // ��������� ���������� ��� ������������� QR-����
        //        SearchMessage = "��������� ������ ��� ��������� QR-����: " + ex.Message;
        //    }

        //    // ���������� �������� � ���������� � ���������� ������
        //    return Page();
        //}










    }
}
