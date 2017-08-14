using NCAAFootballPlayoffs.Models;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using Excel = Microsoft.Office.Interop.Excel;

namespace NCAAFootballPlayoffs.Utilities
{
    public class ExcelManager
    {

        public void testExcel()
        {
            Excel.Application xlApp = new Excel.Application();

            if (xlApp == null)
            {
                return;
            }

            Excel.Workbook xlWorkBook;
            Excel.Worksheet xlWorkSheet;

            object misValue = System.Reflection.Missing.Value;

            xlWorkBook = xlApp.Workbooks.Add(misValue);

            xlWorkSheet = (Microsoft.Office.Interop.Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);
            xlWorkSheet.Cells[1, 1] = "ID";
            xlWorkSheet.Cells[1, 2] = "Name";
            xlWorkSheet.Cells[2, 1] = "1";
            xlWorkSheet.Cells[2, 2] = "One";
            xlWorkSheet.Cells[3, 1] = "2";
            xlWorkSheet.Cells[3, 2] = "Two";

            //xlWorkSheet = (Microsoft.Office.Interop.Excel.Worksheet)xlWorkBook.Worksheets.get_Item(2);
            //xlWorkSheet.Cells[1, 1] = "Sheet 2 content";

            xlWorkBook.SaveAs("your-file-name.xls");
        }

        public MemoryStream DownloadPicks(int? seasonID = 1)
        {
            MemoryStream memStream;

            List<Game> games;
            List<UserName> usernames;

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("New Sheet");

                //worksheet.Cells[1, 1].Value = "ID";
                //worksheet.Cells[1, 2].Value = "Name";
                //worksheet.Cells[2, 1].Value = "1";
                //worksheet.Cells[2, 2].Value = "One";
                //worksheet.Cells[3, 1].Value = "2";
                //worksheet.Cells[3, 2].Value = "Two";
                using (var db = new NCAAFootballPlayoffsEntities())
                {
                    games = db.Games.Where(f => !f.Archived).ToList();
                    if (seasonID != null)
                    {
                        games = games.Where(f => f.SeasonID == seasonID).ToList();
                        usernames = db.UserNames.Where(f => f.UserPicks.Any(g => g.Game.SeasonID == seasonID)).OrderBy(h => h.UserNameText).ToList();
                    }
                    else
                    {
                        games = games.Where(f => f.SeasonID == 1).ToList();
                        usernames = db.UserNames.Where(f => f.UserPicks.Any(g => g.Game.SeasonID == 1)).OrderBy(h => h.UserNameText).ToList();
                    }


                    for (int i = 1; i <= usernames.Count; i++)
                    {
                        worksheet.Cells[1, i].Value = usernames[i - 1].UserNameText;
                    }

                    for (int i = 1; i <= games.Count; i++)
                    {

                        worksheet.Cells[i + 1, 1].Value = games[i - 1].Favorite.TeamName + " vs. " + games[i - 1].Underdog.TeamName;

                        for (int j = 1; j <= usernames.Count; j++)
                        {
                            UserPick userpick = usernames[j - 1].UserPicks.FirstOrDefault(f => f.GameID == games[i - 1].GameID);
                            if (userpick != null)
                            {
                                worksheet.Cells[i + 1, j + 1].Value = "test";
                            }
                        }
                    }
                }
                    


                worksheet.Cells[1, 1, 1, 2].Style.TextRotation = 90;
                worksheet.Cells[1, 1, 1, 2].Style.Fill.PatternType = ExcelFillStyle.Solid;
                Color colFromHex = System.Drawing.ColorTranslator.FromHtml("#B7DEE8");
                worksheet.Cells[1, 1, 1, 2].Style.Fill.BackgroundColor.SetColor(colFromHex);

                worksheet.Cells[1, 1, 1, 2].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                worksheet.Cells[1, 1, 1, 2].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                worksheet.Cells[1, 1, 1, 2].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                worksheet.Cells[1, 1, 1, 2].Style.Border.Left.Style = ExcelBorderStyle.Thin;



                memStream = new MemoryStream(package.GetAsByteArray());
            }

            return memStream;
        }
    }
}