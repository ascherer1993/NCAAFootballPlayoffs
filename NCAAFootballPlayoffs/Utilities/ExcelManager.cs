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

        //THIS USES EPPLUS
        // [y, x?]
        public MemoryStream DownloadPicks(int? seasonID = null)
        {
            if (seasonID == null)
            {
                seasonID = General.GetActiveSeasonID();
            }

            MemoryStream memStream;

            List<Game> games;
            List<BonusQuestion> bonusQuestions;
            List<Username> usernames;


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
                    bonusQuestions = db.BonusQuestions.Where(f => !f.Archived).ToList();

                    if (seasonID != null)
                    {
                        games = games.Where(f => f.SeasonID == seasonID).ToList();
                        bonusQuestions = bonusQuestions.Where(f => f.SeasonID == seasonID).ToList();
                        usernames = db.Usernames.Where(f => f.Approved && f.UserPicks.Any(g => g.Game.SeasonID == seasonID)).OrderBy(h => h.UsernameText).ToList();

                    }
                    else
                    {
                        games = games.Where(f => f.SeasonID == 1).ToList();
                        bonusQuestions = bonusQuestions.Where(f => f.SeasonID == 1).ToList();
                        usernames = db.Usernames.Where(f => f.Approved && f.UserPicks.Any(g => g.Game.SeasonID == 1)).OrderBy(h => h.UsernameText).ToList();
                    }


                    for (int i = 1; i <= usernames.Count; i++)
                    {
                        worksheet.Cells[1, 1 + i].Style.WrapText = true;

                        worksheet.Cells[1, 1 + i].Value = usernames[i - 1].UsernameText + "\n"
                            + usernames[i - 1].User.EmailAddress + "\n"
                            + usernames[i - 1].User.DisplayName;

                        //worksheet.Cells[1, 1 + i].Value = usernames[i - 1].UserNameText;

                        worksheet.Cells[1, 1 + i].Style.TextRotation = 90;
                        worksheet.Cells[1, 1 + i].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        Color usernameColor = System.Drawing.ColorTranslator.FromHtml("#B7DEE8");
                        worksheet.Cells[1, 1 + i].Style.Fill.BackgroundColor.SetColor(usernameColor);
                        
                        worksheet.Cells[1, 1 + i].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells[1, 1 + i].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells[1, 1 + i].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells[1, 1 + i].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    }

                    for (int i = 1; i <= games.Count; i++)
                    {
                        Game game = games[i - 1];
                        worksheet.Cells[i + 1, 1].Value = game.Favorite.TeamName + " vs. " + game.Underdog.TeamName;

                        if (game.IsBCSBowl)
                        {
                            worksheet.Cells[i + 1, 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            Color bcsBackground = System.Drawing.ColorTranslator.FromHtml("#79B979");
                            worksheet.Cells[i + 1, 1].Style.Fill.BackgroundColor.SetColor(bcsBackground);

                            worksheet.Cells[i + 1, 1].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells[i + 1, 1].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells[i + 1, 1].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells[i + 1, 1].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        }

                        for (int j = 1; j <= usernames.Count; j++)
                        {
                            Username username = usernames[j - 1];

                            if (game.IsBCSBowl)
                            {
                                worksheet.Cells[i + 1, j + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                Color bcsBackground = System.Drawing.ColorTranslator.FromHtml("#79B979");
                                worksheet.Cells[i + 1, j + 1].Style.Fill.BackgroundColor.SetColor(bcsBackground);

                                worksheet.Cells[i + 1, j + 1].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                worksheet.Cells[i + 1, j + 1].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                worksheet.Cells[i + 1, j + 1].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                worksheet.Cells[i + 1, j + 1].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                            }


                            UserPick userpick = username.UserPicks.FirstOrDefault(f => f.GameID == games[i - 1].GameID);
                            if (userpick != null)
                            {
                                if (userpick.ChosenTeamID == game.FavoriteID)
                                {
                                    worksheet.Cells[i + 1, j + 1].Value = "F";
                                }
                                else if (userpick.ChosenTeamID == game.UnderdogID)
                                {
                                    worksheet.Cells[i + 1, j + 1].Value = "U";
                                }
                                else
                                {
                                }

                                if (userpick.IsSurePick)
                                {
                                    worksheet.Cells[i + 1, j + 1].Value += "S";
                                }
                            }
                        }
                    }
                    for (int i = 1; i <= bonusQuestions.Count; i++)
                    {
                        BonusQuestion bonusQuestion = bonusQuestions[i - 1];
                        worksheet.Cells[games.Count + i + 1, 1].Value = bonusQuestion.Text;

                        for (int j = 1; j <= usernames.Count; j++)
                        {
                            Username username = usernames[j - 1];
                            UserBonusQuestionPick userBonusQuestionPick = username.UserBonusQuestionPicks.FirstOrDefault(f => f.QuestionAnswer.BonusQuestionID == bonusQuestion.BonusQuestionID);
                            if (userBonusQuestionPick != null)
                            {
                                worksheet.Cells[games.Count + i + 1, j + 1].Value = userBonusQuestionPick.QuestionAnswer.Text;
                            }
                        }
                    }
                }
                //worksheet.Cells[1, 1, 1, 2].Style.TextRotation = 90;
                //worksheet.Cells[1, 1, 1, 2].Style.Fill.PatternType = ExcelFillStyle.Solid;
                //Color colFromHex = System.Drawing.ColorTranslator.FromHtml("#B7DEE8");
                //worksheet.Cells[1, 1, 1, 2].Style.Fill.BackgroundColor.SetColor(colFromHex);

                //worksheet.Cells[1, 1, 1, 2].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                //worksheet.Cells[1, 1, 1, 2].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                //worksheet.Cells[1, 1, 1, 2].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                //worksheet.Cells[1, 1, 1, 2].Style.Border.Left.Style = ExcelBorderStyle.Thin;

                worksheet.Cells[1, 1, games.Count() + 1, 1].AutoFitColumns();
                //worksheet.Cells[1, 1, maxY, 1].
                worksheet.View.FreezePanes(2, 2);

                memStream = new MemoryStream(package.GetAsByteArray());
            }

            return memStream;
        }


        public MemoryStream DownloadBracketPicks(int seasonID, int usernameID)
        {
            MemoryStream memStream;

            Username username;
            List<Game> games;
            List<UserPick> userpicks;
            List<BonusQuestion> bonusQuestions;
            List<UserBonusQuestionPick> userBonusQuestionPicks;

            Color titleFontColor = System.Drawing.ColorTranslator.FromHtml("#0000FF");
            Color bcsBackground = System.Drawing.ColorTranslator.FromHtml("#5cb85c");
            Color gameBackground = System.Drawing.ColorTranslator.FromHtml("#A8B2CD");

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("New Sheet");
                worksheet.PrinterSettings.Orientation = eOrientation.Landscape;
                worksheet.PrinterSettings.TopMargin = .5M;
                worksheet.PrinterSettings.RightMargin = .5M;
                worksheet.PrinterSettings.BottomMargin = .5M;
                worksheet.PrinterSettings.LeftMargin = .5M;

                using (var db = new NCAAFootballPlayoffsEntities())
                {
                    Season season = db.Seasons.Find(seasonID);

                    //Get Data
                    username = db.Usernames.FirstOrDefault(f => !f.Archived && f.UsernameID == usernameID);
                    if (username == null)
                    {
                        username = new Username();
                    }
                    games = db.Games.Where(f => !f.Archived && f.SeasonID == seasonID).OrderBy(g => g.GameDatetime).ToList();
                    userpicks = username.UserPicks.Where(f => f.Game.SeasonID == seasonID).ToList();
                    bonusQuestions = db.BonusQuestions.Where(f => !f.Archived && f.SeasonID == seasonID).ToList();
                    userBonusQuestionPicks = username.UserBonusQuestionPicks.Where(f => f.QuestionAnswer.BonusQuestion.SeasonID == seasonID).ToList();

                    //Styling
                    worksheet.Cells[1, 1, (((games.Count / 2) + 1) * 2) + 1, 10].Style.Font.Size = 10;

                    worksheet.Cells[1, 1, (((games.Count / 2) + 1) * 2) + 1, 10].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells[1, 1, (((games.Count / 2) + 1) * 2) + 1, 10].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells[1, 1, (((games.Count / 2) + 1) * 2) + 1, 10].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells[1, 1, (((games.Count / 2) + 1) * 2) + 1, 10].Style.Border.Left.Style = ExcelBorderStyle.Thin;

                    worksheet.Cells[1, 5, (((games.Count / 2) + 1) * 2) + 1, 5].Style.Border.Right.Style = ExcelBorderStyle.Double;

                    worksheet.Column(1).Width = 24;
                    worksheet.Column(2).Width = 4;
                    worksheet.Column(3).Width = 7;
                    worksheet.Column(4).Width = 4;
                    worksheet.Column(5).Width = 24;
                    worksheet.Column(6).Width = 24;
                    worksheet.Column(7).Width = 4;
                    worksheet.Column(8).Width = 7;
                    worksheet.Column(9).Width = 4;
                    worksheet.Column(10).Width = 24;
                    
                    worksheet.Cells[1, 1, 1, 10].Style.Font.Color.SetColor(titleFontColor);
                    worksheet.Cells[1, 1, 1, 10].Style.Font.Bold = true;

                    for (int i = 0; i < 2; i++)
                    {
                        worksheet.Cells[1, 1 + (5 * i)].Value = "Favorite";
                        worksheet.Cells[1, 2 + (5 * i)].Value = "Pick";
                        worksheet.Cells[1, 3 + (5 * i)].Value = "Odds";
                        worksheet.Cells[1, 4 + (5 * i)].Value = "Pick";
                        worksheet.Cells[1, 5 + (5 * i)].Value = "Underdog";
                    }


                    for (int i = 0; i < games.Count; i++)
                    {
                        int yCoordinate = i;
                        int bracketColumn = 0;
                        if (i > (games.Count / 2) - 1)
                        {
                            bracketColumn = 1;
                            yCoordinate = yCoordinate - (games.Count / 2);
                        }

                        worksheet.Cells[1 + (2 * yCoordinate) + 1, 1 + (5 * bracketColumn), 1 + (2 * yCoordinate) + 1, 5 + (5 * bracketColumn)].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[1 + (2 * yCoordinate) + 1, 1 + (5 * bracketColumn), 1 + (2 * yCoordinate) + 1, 5 + (5 * bracketColumn)].Style.Fill.BackgroundColor.SetColor(gameBackground);

                        worksheet.Cells[1 + (2 * yCoordinate) + 1, 1 + (5 * bracketColumn)].Value = games[i].GameName;
                        worksheet.Cells[1 + (2 * yCoordinate) + 1, 2 + (5 * bracketColumn), 1 + (2 * yCoordinate) + 1, 5 + (5 * bracketColumn)].Merge = true;
                        worksheet.Cells[1 + (2 * yCoordinate) + 1, 1 + (5 * bracketColumn)].Style.Border.Right.Style = ExcelBorderStyle.None;
                        worksheet.Cells[1 + (2 * yCoordinate) + 1, 2 + (5 * bracketColumn)].Style.Border.Left.Style = ExcelBorderStyle.None;
                        worksheet.Cells[1 + (2 * yCoordinate) + 1, 1 + (5 * bracketColumn)].Style.Font.Bold = true;
                        if (games[i].GameDatetime != null)
                        {
                            worksheet.Cells[1 + (2 * yCoordinate) + 1, 2 + (5 * bracketColumn)].Value = ((DateTime)games[i].GameDatetime).ToString("M/d/yy h:mmtt") + " CT " + "--" + games[i].Location.City + ", " + games[i].Location.State.StateName;
                            worksheet.Cells[1 + (2 * yCoordinate) + 1, 2 + (5 * bracketColumn)].Style.Font.Size = 8;
                        }
                        
                        //Second row
                        worksheet.Cells[1 + (2 * yCoordinate) + 2, 1 + (5 * bracketColumn)].Value = games[i].Favorite.TeamName;
                        worksheet.Cells[1 + (2 * yCoordinate) + 2, 2 + (5 * bracketColumn)].Value = " ";
                        worksheet.Cells[1 + (2 * yCoordinate) + 2, 3 + (5 * bracketColumn)].Value = games[i].PointSpread;
                        worksheet.Cells[1 + (2 * yCoordinate) + 2, 3 + (5 * bracketColumn)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells[1 + (2 * yCoordinate) + 2, 4 + (5 * bracketColumn)].Value = " ";
                        worksheet.Cells[1 + (2 * yCoordinate) + 2, 5 + (5 * bracketColumn)].Value = games[i].Underdog.TeamName;
                        worksheet.Cells[1 + (2 * yCoordinate) + 2, 1 + (5 * bracketColumn)].Style.Font.Bold = true;
                        worksheet.Cells[1 + (2 * yCoordinate) + 2, 3 + (5 * bracketColumn)].Style.Font.Bold = true;
                        worksheet.Cells[1 + (2 * yCoordinate) + 2, 5 + (5 * bracketColumn)].Style.Font.Bold = true;
                        
                        if (games[i].IsBCSBowl)
                        {
                            worksheet.Cells[1 + (2 * yCoordinate) + 2, 1 + (5 * bracketColumn), 1 + (2 * yCoordinate) + 2, 5 + (5 * bracketColumn)].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            worksheet.Cells[1 + (2 * yCoordinate) + 2, 1 + (5 * bracketColumn), 1 + (2 * yCoordinate) + 2, 5 + (5 * bracketColumn)].Style.Fill.BackgroundColor.SetColor(bcsBackground);
                        }

                        int gameID = games[i].GameID;
                        UserPick userpick = username.UserPicks.FirstOrDefault(f => f.GameID == gameID);
                        if (userpick != null)
                        {
                            if (userpick.ChosenTeamID == games[i].FavoriteID)
                            {
                                worksheet.Cells[1 + (2 * yCoordinate) + 2, 2 + (5 * bracketColumn)].Value = "X";
                            }
                            else if (userpick.ChosenTeamID == games[i].UnderdogID)
                            {
                                worksheet.Cells[1 + (2 * yCoordinate) + 2, 4 + (5 * bracketColumn)].Value = "X";
                            }
                        }
                    }
                    

                    worksheet.Cells[games.Count + (games.Count % 2 == 0 ? 3 : 4), 5].Value = "Bonus Questions";
                    worksheet.Cells[games.Count + (games.Count % 2 == 0 ? 3 : 4), 5].Style.Font.Bold = true;
                    worksheet.Cells[games.Count + (games.Count % 2 == 0 ? 3 : 4), 5].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[games.Count + (games.Count % 2 == 0 ? 3 : 4), 5].Style.Fill.BackgroundColor.SetColor(bcsBackground);

                    worksheet.Cells[games.Count + (games.Count % 2 == 0 ? 3 : 4), 5, games.Count + (games.Count % 2 == 0 ? 3 : 4), 10].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells[games.Count + (games.Count % 2 == 0 ? 3 : 4), 5, games.Count + (games.Count % 2 == 0 ? 3 : 4), 10].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells[games.Count + (games.Count % 2 == 0 ? 3 : 4), 5, games.Count + (games.Count % 2 == 0 ? 3 : 4), 10].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells[games.Count + (games.Count % 2 == 0 ? 3 : 4), 5, games.Count + (games.Count % 2 == 0 ? 3 : 4), 10].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells[games.Count + (games.Count % 2 == 0 ? 3 : 4), 5, games.Count + (games.Count % 2 == 0 ? 3 : 4), 9].Merge = true;
                                                                              
                    worksheet.Cells[games.Count + (games.Count % 2 == 0 ? 3 : 4), 10].Value = "Question Answers";
                    worksheet.Cells[games.Count + (games.Count % 2 == 0 ? 3 : 4), 10].Style.Font.Bold = true;
                    worksheet.Cells[games.Count + (games.Count % 2 == 0 ? 3 : 4), 10].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[games.Count + (games.Count % 2 == 0 ? 3 : 4), 10].Style.Fill.BackgroundColor.SetColor(bcsBackground);
                                                                 
                    worksheet.Cells[games.Count + (games.Count % 2 == 0 ? 3 : 4), 5, games.Count + (games.Count % 2 == 0 ? 3 : 4), 9].Merge = true;

                    for (int i = 0; i < bonusQuestions.Count; i++)
                    {
                        //worksheet.Cells[1, 1, ((games.Count / 2) * 2) + 1, 10].Style.Font.Size = 10;

                        worksheet.Cells[games.Count + (games.Count % 2 == 0 ? 4 : 5) + i, 5, games.Count + (games.Count % 2 == 0 ? 4 : 5) + i, 10].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells[games.Count + (games.Count % 2 == 0 ? 4 : 5) + i, 5, games.Count + (games.Count % 2 == 0 ? 4 : 5) + i, 10].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells[games.Count + (games.Count % 2 == 0 ? 4 : 5) + i, 5, games.Count + (games.Count % 2 == 0 ? 4 : 5) + i, 10].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells[games.Count + (games.Count % 2 == 0 ? 4 : 5) + i, 5, games.Count + (games.Count % 2 == 0 ? 4 : 5) + i, 10].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        

                        worksheet.Cells[games.Count + (games.Count % 2 == 0 ? 4 : 5) + i, 5].Value = bonusQuestions[i].Text;

                        List<QuestionAnswer> questionAnswers = bonusQuestions[i].QuestionAnswers.ToList();
                        QuestionAnswer questionAnswer = questionAnswers.FirstOrDefault(f => userBonusQuestionPicks.Any(g => g.SelectedAnswerID == f.QuestionAnswerID));
                        if (questionAnswer != null)
                        {
                                worksheet.Cells[games.Count + (games.Count % 2 == 0 ? 4 : 5) + i, 10].Value = questionAnswer.Text;
                        }
                        worksheet.Cells[games.Count + (games.Count % 2 == 0 ? 4 : 5) + i, 5, games.Count + (games.Count % 2 == 0 ? 4 : 5) + i, 10].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[games.Count + (games.Count % 2 == 0 ? 4 : 5) + i, 5, games.Count + (games.Count % 2 == 0 ? 4 : 5) + i, 10].Style.Fill.BackgroundColor.SetColor(bcsBackground);

                        worksheet.Cells[games.Count + (games.Count % 2 == 0 ? 4 : 5) + i, 5, games.Count + (games.Count % 2 == 0 ? 4 : 5) + i, 9].Merge = true;

                    }

                    List<UserPick> surePicks = userpicks.Where(f => f.IsSurePick).ToList();
                    
                    worksheet.Cells[games.Count + (games.Count % 2 == 0 ? 3 : 4) + 0, 1].Value = "Bracket Name";
                    worksheet.Cells[games.Count + (games.Count % 2 == 0 ? 3 : 4) + 1, 1].Value = "Real Name";
                    worksheet.Cells[games.Count + (games.Count % 2 == 0 ? 3 : 4) + 2, 1].Value = "Email";
                    worksheet.Cells[games.Count + (games.Count % 2 == 0 ? 3 : 4) + 3, 1].Value = "Season";
                    worksheet.Cells[games.Count + (games.Count % 2 == 0 ? 3 : 4) + 4, 1].Value = "Sure Pick 1";
                    worksheet.Cells[games.Count + (games.Count % 2 == 0 ? 3 : 4) + 5, 1].Value = "Sure Pick 2";
                    worksheet.Cells[games.Count + (games.Count % 2 == 0 ? 3 : 4) + 6, 1].Value = "Sure Pick 3";
                    worksheet.Cells[games.Count + (games.Count % 2 == 0 ? 3 : 4) + 0, 1].Style.Font.Bold = true;
                    worksheet.Cells[games.Count + (games.Count % 2 == 0 ? 3 : 4) + 1, 1].Style.Font.Bold = true;
                    worksheet.Cells[games.Count + (games.Count % 2 == 0 ? 3 : 4) + 2, 1].Style.Font.Bold = true;
                    worksheet.Cells[games.Count + (games.Count % 2 == 0 ? 3 : 4) + 3, 1].Style.Font.Bold = true;
                    worksheet.Cells[games.Count + (games.Count % 2 == 0 ? 3 : 4) + 4, 1].Style.Font.Bold = true;
                    worksheet.Cells[games.Count + (games.Count % 2 == 0 ? 3 : 4) + 5, 1].Style.Font.Bold = true;
                    worksheet.Cells[games.Count + (games.Count % 2 == 0 ? 3 : 4) + 6, 1].Style.Font.Bold = true;

                    if (username.User != null)
                    {
                        worksheet.Cells[games.Count + (games.Count % 2 == 0 ? 3 : 4) + 0, 2].Value = username.UsernameText;
                        worksheet.Cells[games.Count + (games.Count % 2 == 0 ? 3 : 4) + 1, 2].Value = username.User.DisplayName;
                        worksheet.Cells[games.Count + (games.Count % 2 == 0 ? 3 : 4) + 2, 2].Value = username.User.EmailAddress;
                    }
                    worksheet.Cells[games.Count + (games.Count % 2 == 0 ? 3 : 4) + 3, 2].Value = season.SeasonYear.ToString();
                    worksheet.Cells[games.Count + (games.Count % 2 == 0 ? 3 : 4) + 4, 2].Value = surePicks.Count >= 1 ? surePicks[0].Team.TeamName : null;
                    worksheet.Cells[games.Count + (games.Count % 2 == 0 ? 3 : 4) + 5, 2].Value = surePicks.Count >= 2 ? surePicks[1].Team.TeamName : null;
                    worksheet.Cells[games.Count + (games.Count % 2 == 0 ? 3 : 4) + 6, 2].Value = surePicks.Count >= 3 ? surePicks[2].Team.TeamName : null;

                    worksheet.Cells[games.Count + (games.Count % 2 == 0 ? 3 : 4) + 0, 2, games.Count + (games.Count % 2 == 0 ? 3 : 4) + 0, 4].Merge = true;
                    worksheet.Cells[games.Count + (games.Count % 2 == 0 ? 3 : 4) + 1, 2, games.Count + (games.Count % 2 == 0 ? 3 : 4) + 1, 4].Merge = true;
                    worksheet.Cells[games.Count + (games.Count % 2 == 0 ? 3 : 4) + 2, 2, games.Count + (games.Count % 2 == 0 ? 3 : 4) + 2, 4].Merge = true;
                    worksheet.Cells[games.Count + (games.Count % 2 == 0 ? 3 : 4) + 3, 2, games.Count + (games.Count % 2 == 0 ? 3 : 4) + 3, 4].Merge = true;
                    worksheet.Cells[games.Count + (games.Count % 2 == 0 ? 3 : 4) + 4, 2, games.Count + (games.Count % 2 == 0 ? 3 : 4) + 4, 4].Merge = true;
                    worksheet.Cells[games.Count + (games.Count % 2 == 0 ? 3 : 4) + 5, 2, games.Count + (games.Count % 2 == 0 ? 3 : 4) + 5, 4].Merge = true;
                    worksheet.Cells[games.Count + (games.Count % 2 == 0 ? 3 : 4) + 6, 2, games.Count + (games.Count % 2 == 0 ? 3 : 4) + 6, 4].Merge = true;

                    worksheet.Cells[games.Count + (games.Count % 2 == 0 ? 3 : 4) + 0, 1, games.Count + (games.Count % 2 == 0 ? 3 : 4) + 6, 4].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells[games.Count + (games.Count % 2 == 0 ? 3 : 4) + 0, 1, games.Count + (games.Count % 2 == 0 ? 3 : 4) + 6, 4].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells[games.Count + (games.Count % 2 == 0 ? 3 : 4) + 0, 1, games.Count + (games.Count % 2 == 0 ? 3 : 4) + 6, 4].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells[games.Count + (games.Count % 2 == 0 ? 3 : 4) + 0, 1, games.Count + (games.Count % 2 == 0 ? 3 : 4) + 6, 4].Style.Border.Left.Style = ExcelBorderStyle.Thin;

                    worksheet.Cells[games.Count + (games.Count % 2 == 0 ? 3 : 4) + 4, 1, games.Count + (games.Count % 2 == 0 ? 3 : 4) + 6, 4].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[games.Count + (games.Count % 2 == 0 ? 3 : 4) + 4, 1, games.Count + (games.Count % 2 == 0 ? 3 : 4) + 6, 4].Style.Fill.BackgroundColor.SetColor(gameBackground);
                    
                    worksheet.PrinterSettings.PrintArea = worksheet.Cells[1, 1, games.Count + (games.Count % 2 == 0 ? 3 : 4) + 6, 10];
                }
                memStream = new MemoryStream(package.GetAsByteArray());
            }

            return memStream;
        }

    }
}