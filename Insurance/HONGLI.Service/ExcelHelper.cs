using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Web;
using NPOI;
using NPOI.HPSF;
using NPOI.HSSF;
using NPOI.HSSF.UserModel;
using NPOI.HSSF.Util;
using NPOI.POIFS;
using NPOI.SS.Formula.Eval;
using NPOI.SS.UserModel;
using NPOI.Util;
using NPOI.SS;
using NPOI.DDF;
using NPOI.SS.Util;
using System.Collections;
using System.Text.RegularExpressions;
using NPOI.XSSF;
using NPOI.XSSF.UserModel;
namespace HONGLI.Service
{
    public class NPOIHelper
    {
        private HSSFWorkbook hssfworkbook = new HSSFWorkbook();

        private string xlsPath = "";

        public NPOIHelper(string path)
        {
            xlsPath = path;
            using (FileStream file = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                hssfworkbook = new HSSFWorkbook(file);
            }
        }

        #region 属性

        public string XlsPath
        {
            get { return xlsPath; }
            set { xlsPath = value; }
        }

        #endregion

        #region  方法
        /// <summary>
        /// 创建一个空的Excel文档，指定sheet名
        /// </summary>
        /// <param name="xlspath">excel保存路径,默认为xls后缀名</param>
        /// <param name="sheets">sheet名称</param>
        public void CreateEmptyExcelFile(string xlspath, params string[] sheets)
        {
            InitializeWorkbook();

            if (sheets.Length > 0)
            {
                for (int i = 0; i < sheets.Length; i++)
                {
                    hssfworkbook.CreateSheet(sheets[i]);
                }
            }
            else
            {
                hssfworkbook.CreateSheet("sheet1");
            }

            ((HSSFSheet)hssfworkbook.GetSheetAt(0)).AlternativeFormula = false;
            ((HSSFSheet)hssfworkbook.GetSheetAt(0)).AlternativeExpression = false;

            FileStream file = new FileStream(xlspath + ".xls", FileMode.Create);
            hssfworkbook.Write(file);
            file.Close();
        }
        #endregion

        #region 私有方法

        /// <summary>
        /// 设置xls的信息
        /// </summary>
        private void InitializeWorkbook()
        {
            DocumentSummaryInformation dsi = PropertySetFactory.CreateDocumentSummaryInformation();
            dsi.Company = "NPOI Team";
            hssfworkbook.DocumentSummaryInformation = dsi;

            SummaryInformation si = PropertySetFactory.CreateSummaryInformation();
            si.Subject = "NPOI Example";
            hssfworkbook.SummaryInformation = si;
        }

        private void WriteToFile()
        {
            //Write the stream data of workbook to the root directory
            FileStream file = new FileStream(xlsPath, FileMode.Create);
            hssfworkbook.Write(file);
            file.Close();
        }

        #endregion

        /// <summary>
        /// 写入数据，无格式
        /// </summary>
        /// <param name="Sheetindex">sheet索引</param>
        /// <param name="value"></param>
        public void SetCellValuesInXls(int Sheetindex, int RowIndex, int CellIndex, string value)
        {
            InitializeWorkbook();

            ISheet sheet1 = hssfworkbook.GetSheetAt(Sheetindex);

            sheet1.CreateRow(RowIndex).CreateCell(CellIndex).SetCellValue(value);

            WriteToFile();
        }
        /// <summary>
        /// 写入日期格式
        /// </summary>
        /// <param name="Sheetindex"></param>
        /// <param name="RowIndex"></param>
        /// <param name="CellIndex"></param>
        /// <param name="value"></param>
        public void SetDateCellInXls(int Sheetindex, int RowIndex, int CellIndex, string date)
        {
            InitializeWorkbook();

            ISheet sheet = hssfworkbook.GetSheetAt(Sheetindex);
            // Create a row and put some cells in it. Rows are 0 based.
            IRow row = sheet.CreateRow(RowIndex);

            // Create a cell and put a date value in it.  The first cell is not styled as a date.
            ICell cell = row.CreateCell(CellIndex);
            cell.SetCellValue(date);

            ICellStyle cellStyle = hssfworkbook.CreateCellStyle();

            cellStyle.DataFormat = hssfworkbook.CreateDataFormat().GetFormat("yyyy年m月d日");
            cell.CellStyle = cellStyle;

            //ICell cell2 = row.CreateCell(1);
            //cell2.SetCellValue(new DateTime(2008, 5, 5));
            //ICellStyle cellStyle2 = hssfworkbook.CreateCellStyle();
            //IDataFormat format = hssfworkbook.CreateDataFormat();
            //cellStyle2.DataFormat = format.GetFormat("yyyy年m月d日");
            //cell2.CellStyle = cellStyle2;

            //ICell cell3 = row.CreateCell(2);
            //cell3.CellFormula = "DateValue(\"2005-11-11 11:11:11\")";
            //ICellStyle cellStyle3 = hssfworkbook.CreateCellStyle();
            //cellStyle3.DataFormat = HSSFDataFormat.GetBuiltinFormat("m/d/yy h:mm");
            //cell3.CellStyle = cellStyle3;

            WriteToFile();
        }
        /// <summary>
        /// 增加备注
        /// </summary>
        /// <param name="Sheetindex"></param>
        /// <param name="RowIndex"></param>
        /// <param name="CellIndex"></param>
        /// <param name="value"></param>
        /// <param name="commentStr">备注信息</param>
        public void SetCellCommentInXls(int Sheetindex, int RowIndex, int CellIndex, string value, string commentStr)
        {
            InitializeWorkbook();

            ISheet sheet = hssfworkbook.GetSheetAt(Sheetindex);

            IDrawing patr = (HSSFPatriarch)sheet.CreateDrawingPatriarch();

            ICell cell1 = sheet.CreateRow(RowIndex).CreateCell(CellIndex);
            cell1.SetCellValue(new HSSFRichTextString(value));

            //anchor defines size and position of the comment in worksheet
            IComment comment1 = patr.CreateCellComment(new HSSFClientAnchor(0, 0, 0, 0, 4, 2, 6, 5));

            // set text in the comment
            comment1.String = (new HSSFRichTextString(commentStr));

            // The first way to assign comment to a cell is via HSSFCell.SetCellComment method
            cell1.CellComment = (comment1);
            #region old
            ////Create another cell in row 6
            //ICell cell2 = sheet.CreateRow(6).CreateCell(1);
            //cell2.SetCellValue(value);
            //HSSFComment comment2 = (HSSFComment)patr.CreateCellComment(new HSSFClientAnchor(0, 0, 0, 0, 4, 8, 6, 11));
            ////modify background color of the comment
            //comment2.SetFillColor(204, 236, 255);
            //HSSFRichTextString str = new HSSFRichTextString("Normal body temperature");
            ////apply custom font to the text in the comment
            //IFont font = hssfworkbook.CreateFont();
            //font.FontName = ("Arial");
            //font.FontHeightInPoints = 10;
            //font.Boldweight = (short)FontBoldWeight.BOLD;
            //font.Color = HSSFColor.RED.index;
            //str.ApplyFont(font);
            //comment2.String = str;
            //comment2.Visible = true; //by default comments are hidden. This one is always visible.
            //comment2.Author = "Bill Gates";
            ///**
            // * The second way to assign comment to a cell is to implicitly specify its row and column.
            // * Note, it is possible to set row and column of a non-existing cell.
            // * It works, the commnet is visible.
            // */
            //comment2.Row = 6;
            //comment2.Column = 1;
            #endregion
            WriteToFile();
        }
        /// <summary>
        /// 给表格画边框
        /// </summary>
        /// <param name="Sheetindex"></param>
        /// <param name="RowIndex"></param>
        /// <param name="CellIndex"></param>
        public void SetBorderStyleInXls(int Sheetindex, int RowIndex, int CellIndex)
        {
            InitializeWorkbook();

            ISheet sheet = hssfworkbook.GetSheetAt(Sheetindex);

            // Create a row and put some cells in it. Rows are 0 based.
            IRow row = sheet.GetRow(RowIndex);

            // Create a cell and put a value in it.
            ICell cell = row.GetCell(CellIndex);

            // Style the cell with borders all around.
            ICellStyle style = hssfworkbook.CreateCellStyle();
            style.BorderBottom = BorderStyle.THIN;
            style.BottomBorderColor = HSSFColor.BLACK.index;
            style.BorderLeft = BorderStyle.DASH_DOT_DOT;
            style.LeftBorderColor = HSSFColor.GREEN.index;
            style.BorderRight = BorderStyle.HAIR;
            style.RightBorderColor = HSSFColor.BLUE.index;
            style.BorderTop = BorderStyle.MEDIUM_DASHED;
            style.TopBorderColor = HSSFColor.ORANGE.index;
            cell.CellStyle = style;

            WriteToFile();
        }
        /// <summary>
        /// 给单元格加公式
        /// </summary>
        /// <param name="Sheetindex"></param>
        /// <param name="RowIdex"></param>
        /// <param name="CellIndex"></param>
        /// <param name="formula">公式</param>
        public void SetFormulaOfCellInXls(int Sheetindex, int RowIdex, int CellIndex, string formula)
        {
            InitializeWorkbook();

            ISheet sheet1 = hssfworkbook.GetSheetAt(Sheetindex);

            ICellStyle blackBorder = hssfworkbook.CreateCellStyle();
            blackBorder.BorderBottom = BorderStyle.THIN;
            blackBorder.BorderLeft = BorderStyle.THIN;
            blackBorder.BorderRight = BorderStyle.THIN;
            blackBorder.BorderTop = BorderStyle.THIN;
            blackBorder.BottomBorderColor = HSSFColor.BLACK.index;
            blackBorder.LeftBorderColor = HSSFColor.BLACK.index;
            blackBorder.RightBorderColor = HSSFColor.BLACK.index;
            blackBorder.TopBorderColor = HSSFColor.BLACK.index;

            IRow row = sheet1.GetRow(RowIdex);
            ICell cell = row.CreateCell(CellIndex);
            cell.CellFormula = formula;

            WriteToFile();
        }
        /// <summary> 
        /// 设置打印区域 
        /// </summary>
        /// <param name="Sheetindex"></param>
        /// <param name="Area">"A5:G20"</param>
        public void SetPrintAreaInXls(int Sheetindex, string Area)
        {
            InitializeWorkbook();

            hssfworkbook.SetPrintArea(Sheetindex, Area);

            WriteToFile();
        }
        /// <summary>
        /// 设置打印格式，默认为A4纸
        /// </summary>
        /// <param name="Sheetindex"></param>
        /// <param name="Area"></param>
        public void SetPrintFormatInXls(int Sheetindex)
        {
            InitializeWorkbook();

            ISheet sheet1 = hssfworkbook.CreateSheet("Sheet1");
            sheet1.SetMargin(MarginType.RightMargin, (double)0.5);
            sheet1.SetMargin(MarginType.TopMargin, (double)0.6);
            sheet1.SetMargin(MarginType.LeftMargin, (double)0.4);
            sheet1.SetMargin(MarginType.BottomMargin, (double)0.3);

            sheet1.PrintSetup.Copies = 3;
            sheet1.PrintSetup.NoColor = true;
            sheet1.PrintSetup.Landscape = true;
            sheet1.PrintSetup.PaperSize = (short)PaperSize.A4;

            sheet1.FitToPage = true;
            sheet1.PrintSetup.FitHeight = 2;
            sheet1.PrintSetup.FitWidth = 3;
            sheet1.IsPrintGridlines = true;

            WriteToFile();
        }
        /// <summary>
        /// 设置表格的宽和高
        /// </summary>
        /// <param name="Sheetindex"></param>
        /// <param name="RowIdex"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public void SetWidthAndHeightInXls(int Sheetindex, int RowIdex, int CellIndex, int width, short height)
        {
            InitializeWorkbook();

            ISheet sheet1 = hssfworkbook.GetSheetAt(Sheetindex);
            //set the width of columns
            sheet1.SetColumnWidth(CellIndex, width);

            //set the width of height
            sheet1.GetRow(RowIdex).Height = height;

            sheet1.DefaultRowHeightInPoints = 50;

            WriteToFile();
        }
        /// <summary>
        /// 设置单元格对齐方式
        /// </summary>
        /// <param name="Sheetindex"></param>
        /// <param name="RowIdex"></param>
        /// <param name="CellIndex"></param>
        /// <param name="Horizont">水平对齐方式：left,center,right,justify</param>
        /// <param name="Vertical">垂直对齐方式：top,center,buttom,justify</param>
        public void SetAlignmentInXls(int Sheetindex, int RowIdex, int CellIndex, string Horizont, string Vertical)
        {
            InitializeWorkbook();

            ISheet sheet1 = hssfworkbook.GetSheetAt(Sheetindex);

            IRow row = sheet1.GetRow(RowIdex);
            ICellStyle style = hssfworkbook.CreateCellStyle();
            if (Horizont == "left")
            {
                style.Alignment = HorizontalAlignment.LEFT;
            }
            else if (Horizont == "center")
            {
                style.Alignment = HorizontalAlignment.CENTER;
            }
            else if (Horizont == "right")
            {
                style.Alignment = HorizontalAlignment.RIGHT;
            }
            else if (Horizont == "justify")
            {
                style.Alignment = HorizontalAlignment.JUSTIFY;
            }

            if (Vertical == "top")
            {
                style.VerticalAlignment = VerticalAlignment.TOP;
            }
            else if (Vertical == "center")
            {
                style.VerticalAlignment = VerticalAlignment.CENTER;
            }
            else if (Vertical == "buttom")
            {
                style.VerticalAlignment = VerticalAlignment.BOTTOM;
            }
            else if (Vertical == "justify")
            {
                style.VerticalAlignment = VerticalAlignment.JUSTIFY;
            }
            style.Indention = 3;

            row.GetCell(CellIndex).CellStyle = style;

            WriteToFile();
        }
        /// <summary>
        /// 放大缩小工作簿 根据  sub/den 进行缩放
        /// </summary>
        /// <param name="Sheetindex">要放大的sheet</param>
        /// <param name="sub">比列的分子</param>
        /// <param name="den">比列的分母</param>
        public void ZoomSheet(int Sheetindex, int sub, int den)
        {
            InitializeWorkbook();

            ISheet sheet1 = hssfworkbook.GetSheetAt(Sheetindex);
            sheet1.SetZoom(sub, den);   // 75 percent magnification

            WriteToFile();
        }
        /// <summary>
        /// 在单元格内使用多行存储数据
        /// </summary>
        /// <param name="Sheetindex"></param>
        /// <param name="RowIdex"></param>
        /// <param name="CellIndex"></param>
        /// <param name="rows">使用的行数</param>
        /// <param name="value">在换行的后面加上   \n</param>
        public void UseNewlinesInCellsInXls(int Sheetindex, int RowIdex, int CellIndex, int rows, string value)
        {
            InitializeWorkbook();

            ISheet sheet1 = hssfworkbook.GetSheetAt(Sheetindex);

            //use newlines in cell
            IRow row1 = sheet1.GetRow(RowIdex);
            ICell cell1 = row1.GetCell(CellIndex);

            //to enable newlines you need set a cell styles with wrap=true
            ICellStyle cs = hssfworkbook.CreateCellStyle();
            cs.WrapText = true;
            cell1.CellStyle = cs;

            row1.HeightInPoints = rows * sheet1.DefaultRowHeightInPoints;
            cell1.SetCellValue(value);
            WriteToFile();
        }
        /// <summary>
        /// 单元格使用基础公式
        /// </summary>
        /// <param name="Sheetindex"></param>
        /// <param name="RowIdex"></param>
        /// <param name="CellIndex"></param>
        /// <param name="Formula"></param>
        public void UseBasicFormulaInXls(int Sheetindex, int RowIdex, int CellIndex, string Formula)
        {
            InitializeWorkbook();
            ISheet s1 = hssfworkbook.GetSheetAt(Sheetindex);
            //set A4=A2+A3,set D2=SUM(A2:C2);A5=cos(5)+sin(10)
            s1.GetRow(RowIdex).GetCell(CellIndex).CellFormula = Formula;
            WriteToFile();
        }


        /// <summary>
        /// 冻结行，FreezeRow为要冻结的行
        /// </summary>
        /// <param name="Sheetindex"></param>
        public void SplitAndFreezePanes(int Sheetindex, int FreezeRow)
        {
            InitializeWorkbook();

            ISheet sheet1 = hssfworkbook.GetSheetAt(Sheetindex);

            // Freeze just one row
            sheet1.CreateFreezePane(0, FreezeRow);

            WriteToFile();
        }


        /// <summary>
        /// 缩放指定单元格字体
        /// </summary>
        /// <param name="Sheetindex"></param>
        /// <param name="RowIdex"></param>
        /// <param name="CellIndex"></param>
        public void ShrinkToFitColumnInXls(int Sheetindex, int RowIdex, int CellIndex)
        {
            InitializeWorkbook();

            ISheet sheet = hssfworkbook.GetSheetAt(Sheetindex);
            IRow row = sheet.GetRow(RowIdex);
            //create cell value
            ICell cell1 = row.GetCell(CellIndex);

            ICellStyle cellstyle1 = hssfworkbook.CreateCellStyle();
            cellstyle1.ShrinkToFit = true;
            WriteToFile();
        }


        /// <summary>
        /// 将字体旋转指定角度
        /// </summary>
        /// <param name="Sheetindex"></param>
        /// <param name="RowIdex"></param>
        /// <param name="CellIndex"></param>
        /// <param name="Angle"></param>
        public void RotateTextInXls(int Sheetindex, int RowIdex, int CellIndex, short Angle)
        {
            InitializeWorkbook();

            ISheet sheet1 = hssfworkbook.GetSheetAt(Sheetindex);

            IRow row = sheet1.GetRow(RowIdex);
            //set the style
            ICellStyle style = hssfworkbook.CreateCellStyle();
            style.Rotation = Angle;
            row.GetCell(CellIndex).CellStyle = style;

            WriteToFile();
        }

        public void RepeatingRowsAndColumns(int Sheetindex, int RowIdex, int CellIndex)
        {
            InitializeWorkbook();

            ISheet sheet1 = hssfworkbook.GetSheetAt(Sheetindex);

            IFont boldFont = hssfworkbook.CreateFont();
            boldFont.FontHeightInPoints = 22;
            boldFont.Boldweight = (short)FontBoldWeight.BOLD;

            ICellStyle boldStyle = hssfworkbook.CreateCellStyle();
            boldStyle.SetFont(boldFont);

            IRow row = sheet1.GetRow(RowIdex);
            ICell cell = row.GetCell(CellIndex);
            cell.CellStyle = (boldStyle);

            // Set the columns to repeat from column 0 to 2 on the first sheet
            hssfworkbook.SetRepeatingRowsAndColumns(Sheetindex, 0, 2, -1, -1);

            WriteToFile();
        }



        /// <summary>
        /// 向单元格中写入数字格式
        /// </summary>
        /// <param name="Sheetindex"></param>
        /// <param name="RowIdex"></param>
        /// <param name="CellIndex"></param>
        /// <param name="type">double，RMB，scentific，percent，phone，ChineseCapital，ChineseDate</param>
        public void NumberFormatInXls(int Sheetindex, int RowIdex, int CellIndex, string type)
        {
            InitializeWorkbook();

            ISheet sheet = hssfworkbook.GetSheetAt(Sheetindex);
            //increase the width of Column A
            sheet.SetColumnWidth(0, 5000);
            //create the format instance
            IDataFormat format = hssfworkbook.CreateDataFormat();

            // Create a row and put some cells in it. Rows are 0 based.
            ICell cell = sheet.GetRow(RowIdex).GetCell(CellIndex);
            ICellStyle cellStyle = hssfworkbook.CreateCellStyle();

            if (type == "double")
            {
                cellStyle.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.00");
            }
            else if (type == "RMB")
            {
                cellStyle.DataFormat = format.GetFormat("¥#,##0");
            }
            else if (type == "scentific")
            {
                cellStyle.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.00E+00");
            }
            else if (type == "percent")
            {
                cellStyle.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.00%");
            }
            else if (type == "phone")//phone number format - "021-65881234"
            {
                cellStyle.DataFormat = format.GetFormat("000-00000000");
            }
            else if (type == "ChineseCapital") //Chinese capitalized character number - 壹贰叁 元
            {
                cellStyle.DataFormat = format.GetFormat("[DbNum2][$-804]0 元");
            }
            else if (type == "ChineseDate")
            {
                cellStyle.DataFormat = format.GetFormat("yyyy年m月d日");
            }
            cell.CellStyle = cellStyle;

            WriteToFile();
        }


        /// <summary>
        /// 将一个单元格赋予两个表格的乘积
        /// </summary>
        /// <param name="Sheetindex"></param>
        /// <param name="RowIdex">要赋值的单元格行索引</param>
        /// <param name="CellIndex">要赋值的单元格列索引</param>
        /// <param name="targateRow1">第一个单元格的行</param>
        /// <param name="targateCell1">第一个单元格的列</param>
        /// <param name="targateRow2">第二个单元格的行</param>
        /// <param name="targateCell2">第二个单元格的列</param>
        public void MultplicationTableInXls(int Sheetindex, int RowIdex, int CellIndex, int targateRow1, int targateCell1,
            int targateRow2, int targateCell2)
        {
            InitializeWorkbook();

            ISheet sheet1 = hssfworkbook.GetSheetAt(Sheetindex);

            IRow row = sheet1.GetRow(RowIdex);

            string formula = GetCellPosition(targateRow1, targateCell1) + "*" + GetCellPosition(targateRow2, targateCell2);

            row.CreateCell(CellIndex).CellFormula = formula;

            WriteToFile();
        }
        public string GetCellPosition(int row, int col)
        {
            col = Convert.ToInt32('A') + col;
            row = row + 1;
            return ((char)col) + row.ToString();
        }
        /// <summary>
        /// 合并单元格
        /// </summary>
        /// <param name="Sheetindex"></param>
        /// <param name="firstRowIdex">起始单元格</param>
        /// <param name="firstCellIndex"></param>
        /// <param name="lastRowIdex">结束单元格</param>
        /// <param name="lastCellIndex"></param>
        public void MergeCellsInXls(int Sheetindex, int firstRowIdex, int firstCellIndex, int lastRowIdex, int lastCellIndex)
        {
            InitializeWorkbook();

            ISheet sheet = hssfworkbook.GetSheetAt(Sheetindex);

            sheet.AddMergedRegion(new CellRangeAddress(0, 0, 0, 5));

            CellRangeAddress region = new CellRangeAddress(firstRowIdex, lastRowIdex, firstCellIndex, lastCellIndex);

            sheet.AddMergedRegion(region);

            WriteToFile();
        }

        #region 未处理
        public void LoanCalculator(int Sheetindex, int RowIdex, int CellIndex)
        {
            InitializeWorkbook();
            Dictionary<String, ICellStyle> styles = CreateStyles(hssfworkbook);
            ISheet sheet = hssfworkbook.GetSheetAt(Sheetindex);
            sheet.IsPrintGridlines = (false);//取消打印格的显示
            sheet.DisplayGridlines = (false);

            IPrintSetup printSetup = sheet.PrintSetup;
            printSetup.Landscape = (true);
            sheet.FitToPage = (true);
            sheet.HorizontallyCenter = (true);

            sheet.SetColumnWidth(0, 3 * 256);
            sheet.SetColumnWidth(1, 3 * 256);
            sheet.SetColumnWidth(2, 11 * 256);
            sheet.SetColumnWidth(3, 14 * 256);
            sheet.SetColumnWidth(4, 14 * 256);
            sheet.SetColumnWidth(5, 14 * 256);
            sheet.SetColumnWidth(6, 14 * 256);

            CreateNames(hssfworkbook);

            IRow titleRow = sheet.CreateRow(0);
            titleRow.HeightInPoints = (35);
            for (int i = 1; i <= 7; i++)
            {
                titleRow.CreateCell(i).CellStyle = styles["title"];
            }
            ICell titleCell = titleRow.GetCell(2);
            titleCell.SetCellValue("Simple Loan Calculator");
            sheet.AddMergedRegion(CellRangeAddress.ValueOf("$C$1:$H$1"));

            IRow row = sheet.CreateRow(2);
            ICell cell = row.CreateCell(4);
            cell.SetCellValue("Enter values");
            cell.CellStyle = styles["item_right"];

            row = sheet.CreateRow(3);
            cell = row.CreateCell(2);
            cell.SetCellValue("Loan amount");
            cell.CellStyle = styles["item_left"];
            cell = row.CreateCell(4);
            cell.CellStyle = styles["input_$"];
            cell.SetAsActiveCell();

            row = sheet.CreateRow(4);
            cell = row.CreateCell(2);
            cell.SetCellValue("Annual interest rate");
            cell.CellStyle = styles["item_left"];
            cell = row.CreateCell(4);
            cell.CellStyle = styles["input_%"];

            row = sheet.CreateRow(5);
            cell = row.CreateCell(2);
            cell.SetCellValue("Loan period in years");
            cell.CellStyle = styles["item_left"];
            cell = row.CreateCell(4);
            cell.CellStyle = styles["input_i"];

            row = sheet.CreateRow(6);
            cell = row.CreateCell(2);
            cell.SetCellValue("Start date of loan");
            cell.CellStyle = styles["item_left"];
            cell = row.CreateCell(4);
            cell.CellStyle = styles["input_d"];

            row = sheet.CreateRow(8);
            cell = row.CreateCell(2);
            cell.SetCellValue("Monthly payment");
            cell.CellStyle = styles["item_left"];
            cell = row.CreateCell(4);
            cell.CellFormula = ("IF(Values_Entered,Monthly_Payment,\"\")");
            cell.CellStyle = styles["formula_$"];

            row = sheet.CreateRow(9);
            cell = row.CreateCell(2);
            cell.SetCellValue("Number of payments");
            cell.CellStyle = styles["item_left"];
            cell = row.CreateCell(4);
            cell.CellFormula = ("IF(Values_Entered,Loan_Years*12,\"\")");
            cell.CellStyle = styles["formula_i"];

            row = sheet.CreateRow(10);
            cell = row.CreateCell(2);
            cell.SetCellValue("Total interest");
            cell.CellStyle = styles["item_left"];
            cell = row.CreateCell(4);
            cell.CellFormula = ("IF(Values_Entered,Total_Cost-Loan_Amount,\"\")");
            cell.CellStyle = styles["formula_$"];

            row = sheet.CreateRow(11);
            cell = row.CreateCell(2);
            cell.SetCellValue("Total cost of loan");
            cell.CellStyle = styles["item_left"];
            cell = row.CreateCell(4);
            cell.CellFormula = ("IF(Values_Entered,Monthly_Payment*Number_of_Payments,\"\")");
            cell.CellStyle = styles["formula_$"];


            WriteToFile();
        }

        /**
         * cell styles used for formatting calendar sheets
        */
        private static Dictionary<String, ICellStyle> CreateStyles(IWorkbook wb)
        {
            Dictionary<String, ICellStyle> styles = new Dictionary<String, ICellStyle>();

            ICellStyle style = null;
            IFont titleFont = wb.CreateFont();
            titleFont.FontHeightInPoints = (short)14;
            titleFont.FontName = "Trebuchet MS";
            style = wb.CreateCellStyle();
            style.SetFont(titleFont);
            style.BorderBottom = BorderStyle.DOTTED;
            style.BottomBorderColor = IndexedColors.GREY_40_PERCENT.Index;
            styles.Add("title", style);

            IFont itemFont = wb.CreateFont();
            itemFont.FontHeightInPoints = (short)9;
            itemFont.FontName = "Trebuchet MS";
            style = wb.CreateCellStyle();
            style.Alignment = (HorizontalAlignment.LEFT);
            style.SetFont(itemFont);
            styles.Add("item_left", style);

            style = wb.CreateCellStyle();
            style.Alignment = HorizontalAlignment.RIGHT;
            style.SetFont(itemFont);
            styles.Add("item_right", style);

            style = wb.CreateCellStyle();
            style.Alignment = HorizontalAlignment.RIGHT;
            style.SetFont(itemFont);
            style.BorderRight = BorderStyle.DOTTED;
            style.RightBorderColor = IndexedColors.GREY_40_PERCENT.Index;
            style.BorderBottom = BorderStyle.DOTTED;
            style.BottomBorderColor = IndexedColors.GREY_40_PERCENT.Index;
            style.BorderLeft = BorderStyle.DOTTED;
            style.LeftBorderColor = IndexedColors.GREY_40_PERCENT.Index;
            style.BorderTop = BorderStyle.DOTTED;
            style.TopBorderColor = IndexedColors.GREY_40_PERCENT.Index;
            style.DataFormat = (wb.CreateDataFormat().GetFormat("_($* #,##0.00_);_($* (#,##0.00);_($* \"-\"??_);_(@_)"));
            styles.Add("input_$", style);

            style = wb.CreateCellStyle();
            style.Alignment = HorizontalAlignment.RIGHT;
            style.SetFont(itemFont);
            style.BorderRight = BorderStyle.DOTTED;
            style.RightBorderColor = IndexedColors.GREY_40_PERCENT.Index;
            style.BorderBottom = BorderStyle.DOTTED;
            style.BottomBorderColor = IndexedColors.GREY_40_PERCENT.Index;
            style.BorderLeft = BorderStyle.DOTTED;
            style.LeftBorderColor = IndexedColors.GREY_40_PERCENT.Index;
            style.BorderTop = BorderStyle.DOTTED;
            style.TopBorderColor = IndexedColors.GREY_40_PERCENT.Index;
            style.DataFormat = (wb.CreateDataFormat().GetFormat("0.000%"));
            styles.Add("input_%", style);

            style = wb.CreateCellStyle();
            style.Alignment = HorizontalAlignment.RIGHT;
            style.SetFont(itemFont);
            style.BorderRight = BorderStyle.DOTTED;
            style.RightBorderColor = IndexedColors.GREY_40_PERCENT.Index;
            style.BorderBottom = BorderStyle.DOTTED;
            style.BottomBorderColor = IndexedColors.GREY_40_PERCENT.Index;
            style.BorderLeft = BorderStyle.DOTTED;
            style.LeftBorderColor = IndexedColors.GREY_40_PERCENT.Index;
            style.BorderTop = BorderStyle.DOTTED;
            style.TopBorderColor = IndexedColors.GREY_40_PERCENT.Index;
            style.DataFormat = wb.CreateDataFormat().GetFormat("0");
            styles.Add("input_i", style);

            style = wb.CreateCellStyle();
            style.Alignment = (HorizontalAlignment.CENTER);
            style.SetFont(itemFont);
            style.DataFormat = wb.CreateDataFormat().GetFormat("m/d/yy");
            styles.Add("input_d", style);

            style = wb.CreateCellStyle();
            style.Alignment = HorizontalAlignment.RIGHT;
            style.SetFont(itemFont);
            style.BorderRight = BorderStyle.DOTTED;
            style.RightBorderColor = IndexedColors.GREY_40_PERCENT.Index;
            style.BorderBottom = BorderStyle.DOTTED;
            style.BottomBorderColor = IndexedColors.GREY_40_PERCENT.Index;
            style.BorderLeft = BorderStyle.DOTTED;
            style.LeftBorderColor = IndexedColors.GREY_40_PERCENT.Index;
            style.BorderTop = BorderStyle.DOTTED;
            style.TopBorderColor = IndexedColors.GREY_40_PERCENT.Index;
            style.DataFormat = wb.CreateDataFormat().GetFormat("$##,##0.00");
            style.BorderBottom = BorderStyle.DOTTED;
            style.BottomBorderColor = IndexedColors.GREY_40_PERCENT.Index;
            style.FillForegroundColor = IndexedColors.GREY_25_PERCENT.Index;
            style.FillPattern = FillPatternType.SOLID_FOREGROUND;
            styles.Add("formula_$", style);

            style = wb.CreateCellStyle();
            style.Alignment = HorizontalAlignment.RIGHT;
            style.SetFont(itemFont);
            style.BorderRight = BorderStyle.DOTTED;
            style.RightBorderColor = IndexedColors.GREY_40_PERCENT.Index;
            style.BorderBottom = BorderStyle.DOTTED;
            style.BottomBorderColor = IndexedColors.GREY_40_PERCENT.Index;
            style.BorderLeft = BorderStyle.DOTTED;
            style.LeftBorderColor = IndexedColors.GREY_40_PERCENT.Index;
            style.BorderTop = BorderStyle.DOTTED;
            style.TopBorderColor = IndexedColors.GREY_40_PERCENT.Index;
            style.DataFormat = wb.CreateDataFormat().GetFormat("0");
            style.BorderBottom = BorderStyle.DOTTED;
            style.BottomBorderColor = IndexedColors.GREY_40_PERCENT.Index;
            style.FillForegroundColor = IndexedColors.GREY_25_PERCENT.Index;
            style.FillPattern = (FillPatternType.SOLID_FOREGROUND);
            styles.Add("formula_i", style);

            return styles;
        }


        //define named ranges for the inputs and formulas
        public static void CreateNames(IWorkbook wb)
        {
            IName name;

            name = wb.CreateName();
            name.NameName = ("Interest_Rate");
            name.RefersToFormula = ("'Loan Calculator'!$E$5");

            name = wb.CreateName();
            name.NameName = ("Loan_Amount");
            name.RefersToFormula = ("'Loan Calculator'!$E$4");

            name = wb.CreateName();
            name.NameName = ("Loan_Start");
            name.RefersToFormula = ("'Loan Calculator'!$E$7");

            name = wb.CreateName();
            name.NameName = ("Loan_Years");
            name.RefersToFormula = ("'Loan Calculator'!$E$6");

            name = wb.CreateName();
            name.NameName = ("Number_of_Payments");
            name.RefersToFormula = ("'Loan Calculator'!$E$10");

            name = wb.CreateName();
            name.NameName = ("Monthly_Payment");
            name.RefersToFormula = ("-PMT(Interest_Rate/12,Number_of_Payments,Loan_Amount)");

            name = wb.CreateName();
            name.NameName = ("Total_Cost");
            name.RefersToFormula = ("'Loan Calculator'!$E$12");

            name = wb.CreateName();
            name.NameName = ("Total_Interest");
            name.RefersToFormula = ("'Loan Calculator'!$E$11");

            name = wb.CreateName();
            name.NameName = ("Values_Entered");
            name.RefersToFormula = ("IF(ISBLANK(Loan_Start),0,IF(Loan_Amount*Interest_Rate*Loan_Years>0,1,0))");
        }

        #endregion


        /// <summary>
        /// excel中插入图片,支持jpeg
        /// </summary>
        /// <param name="Sheetindex"></param>
        /// <param name="RowIdex"></param>
        /// <param name="CellIndex"></param>
        /// <param name="picpath">图片地址</param>
        /// <param name="dx1">图片坐标</param>
        /// <param name="dy1"></param>
        /// <param name="dx2"></param>
        /// <param name="dy2"></param>
        /// <param name="col1">表格</param>
        /// <param name="row1"></param>
        /// <param name="col2"></param>
        /// <param name="row2"></param>
        public void InsertPicturesInXls(int Sheetindex, int RowIdex, int CellIndex, string picpath
            , int dx1, int dy1, int dx2, int dy2, int col1, int row1, int col2, int row2)
        {
            InitializeWorkbook();

            ISheet sheet1 = hssfworkbook.GetSheetAt(Sheetindex);

            HSSFPatriarch patriarch = (HSSFPatriarch)sheet1.CreateDrawingPatriarch();
            //create the anchor
            HSSFClientAnchor anchor;
            anchor = new HSSFClientAnchor(dx1, dy1, dx2, dy2, col1, row1, col2, row2);
            anchor.AnchorType = 2;
            //load the picture and get the picture index in the workbook
            HSSFPicture picture = (HSSFPicture)patriarch.CreatePicture(anchor, LoadImage(picpath, hssfworkbook));
            //Reset the image to the original size.
            //picture.Resize();   //Note: Resize will reset client anchor you set.
            picture.LineStyle = LineStyle.DashDotGel;

            WriteToFile();
        }

        private int LoadImage(string path, HSSFWorkbook wb)
        {
            FileStream file = new FileStream(path, FileMode.Open, FileAccess.Read);
            byte[] buffer = new byte[file.Length];
            file.Read(buffer, 0, (int)file.Length);
            return wb.AddPicture(buffer, PictureType.JPEG);

        }
        /// <summary>
        /// 隐藏Excel行和列
        /// </summary>
        /// <param name="Sheetindex"></param>
        /// <param name="RowIdex"></param>
        /// <param name="CellIndex"></param>
        /// <param name="isHiddenCol"></param>
        public void HideColumnAndRowInXls(int Sheetindex, int RowIdex, int CellIndex, bool isHiddenCol)
        {
            InitializeWorkbook();

            ISheet s = hssfworkbook.GetSheetAt(Sheetindex);
            IRow r1 = s.GetRow(RowIdex);


            //hide IRow 2
            r1.ZeroHeight = true;

            //hide column C
            s.SetColumnHidden(CellIndex, isHiddenCol);

            WriteToFile();
        }
        /// <summary>
        /// 填充背景颜色
        /// </summary>
        /// <param name="Sheetindex"></param>
        /// <param name="RowIdex"></param>
        /// <param name="CellIndex"></param>
        /// <param name="fpt">填充类型</param>
        /// <param name="Forecolor">前景色：NPOI.HSSF.Util.HSSFColor.BLUE.index</param>
        /// <param name="backcolor">背景颜色：NPOI.HSSF.Util.HSSFColor.BLUE.index</param>
        public void FillBackgroundInXls(int Sheetindex, int RowIdex, int CellIndex, FillPatternType fpt, short Forecolor, short backcolor)
        {
            InitializeWorkbook();

            ISheet sheet1 = hssfworkbook.GetSheetAt(Sheetindex);
            //fill background
            ICellStyle style1 = hssfworkbook.CreateCellStyle();
            style1.FillForegroundColor = Forecolor;
            style1.FillPattern = fpt;
            style1.FillBackgroundColor = backcolor;
            sheet1.GetRow(RowIdex).GetCell(CellIndex).CellStyle = style1;
            WriteToFile();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="Sheetindex"></param>
        /// <param name="RowIdex"></param>
        /// <param name="CellIndex"></param>
        /// <param name="fontName">字体名</param>
        /// <param name="color">颜色</param>
        /// <param name="istalic">斜体</param>
        /// <param name="IsStrikeout">删除线</param>
        /// <param name="size">字体大小</param>
        public void ApplyFontInXls(int Sheetindex, int RowIdex, int CellIndex, string fontName, short color, bool istalic, bool IsStrikeout, short size)
        {
            InitializeWorkbook();

            ISheet sheet1 = hssfworkbook.GetSheetAt(Sheetindex);

            //font style1: underlined, italic, red color, fontsize=20
            IFont font1 = hssfworkbook.CreateFont();
            font1.FontName = "宋体";
            font1.Color = HSSFColor.RED.index;
            font1.IsItalic = istalic;
            font1.IsStrikeout = IsStrikeout;
            //font1.Underline = (byte)FontUnderlineType.DOUBLE;
            font1.FontHeightInPoints = size;
            //bind font with style 1

            ICell cell1 = sheet1.GetRow(RowIdex).GetCell(CellIndex);
            ICellStyle style1 = hssfworkbook.CreateCellStyle();
            style1.SetFont(font1);
            cell1.CellStyle = style1;
            WriteToFile();
        }

        /// <summary>
        /// 设置sheet的颜色
        /// </summary>
        /// <param name="Sheetindex"></param>
        /// <param name="RowIdex"></param>
        /// <param name="CellIndex"></param>
        public void ChangeSheetTabColorInXls(int Sheetindex)
        {
            InitializeWorkbook();

            ISheet sheet = hssfworkbook.GetSheetAt(Sheetindex);
            sheet.TabColorIndex = HSSFColor.AQUA.index;

            WriteToFile();
        }


        public string GetCellValue(int sheetIndex, int rowIndex, int cellIndex)
        {
            InitializeWorkbook();

            ISheet sheet1 = hssfworkbook.GetSheetAt(sheetIndex);

            ICell cell = sheet1.GetRow(rowIndex).GetCell(cellIndex);

            return cell.StringCellValue;

        }



        #region 导入导出
        /* 
         * DataTable table = new DataTable();     
         * MemoryStream ms = DataTableRenderToExcel.RenderDataTableToExcel(table) as MemoryStream;
         * Response.AddHeader("Content-Disposition", string.Format("attachment; filename=Download.xls"));
         * Response.BinaryWrite(ms.ToArray());
         * ms.Close();
         * ms.Dispose();
       
         * if (this.fuUpload.HasFile)
         * {
         *       DataTable table = DataTableRenderToExcel.RenderDataTableFromExcel(this.fuUpload.FileContent, 1, 0);
         *       this.gvExcel.DataSource = table;
         *       this.gvExcel.DataBind();
         *  }
         */
        /// <summary>
        /// Datatable导出Excel
        /// </summary>
        /// <param name="SourceTable"></param>
        /// <returns></returns>
        private static Stream RenderDataTableToExcel(DataTable SourceTable)
        {
            HSSFWorkbook workbook = new HSSFWorkbook();
            MemoryStream ms = new MemoryStream();
            ISheet sheet = workbook.CreateSheet();
            IRow headerRow = sheet.CreateRow(0);

            // handling header.
            foreach (DataColumn column in SourceTable.Columns)
                headerRow.CreateCell(column.Ordinal).SetCellValue(column.ColumnName);

            // handling value.
            int rowIndex = 1;

            foreach (DataRow row in SourceTable.Rows)
            {
                IRow dataRow = sheet.CreateRow(rowIndex);
                foreach (DataColumn column in SourceTable.Columns)
                {
                    dataRow.CreateCell(column.Ordinal).SetCellValue(row[column].ToString());
                }

                rowIndex++;
            }

            workbook.Write(ms);
            ms.Flush();
            ms.Position = 0;

            sheet = null;
            headerRow = null;
            workbook = null;

            return ms;
        }
        /// <summary>
        /// Datatable导出Excel
        /// </summary>
        /// <param name="SourceTable"></param>
        /// <param name="FileName"></param>
        public static MemoryStream RenderDataTableToExcel(DataTable SourceTable, string FileName)
        {
            MemoryStream ms = RenderDataTableToExcel(SourceTable) as MemoryStream;
            return ms;
           
        }
        /// <summary>
        /// 导出excel为Datatable
        /// </summary>
        /// <param name="ExcelFileStream"></param>
        /// <param name="SheetName"></param>
        /// <param name="HeaderRowIndex"></param>
        /// <returns></returns>
        public static DataTable RenderDataTableFromExcel(Stream ExcelFileStream, string SheetName, int HeaderRowIndex)
        {
            HSSFWorkbook workbook = new HSSFWorkbook(ExcelFileStream);
            ISheet sheet = workbook.GetSheet(SheetName);

            DataTable table = new DataTable();

            IRow headerRow = sheet.GetRow(HeaderRowIndex);
            int cellCount = headerRow.LastCellNum;

            for (int i = headerRow.FirstCellNum; i < cellCount; i++)
            {
                DataColumn column = new DataColumn(headerRow.GetCell(i).StringCellValue);
                table.Columns.Add(column);
            }

            int rowCount = sheet.LastRowNum;

            for (int i = (sheet.FirstRowNum + 1); i < sheet.LastRowNum; i++)
            {
                IRow row = sheet.GetRow(i);
                DataRow dataRow = table.NewRow();

                for (int j = row.FirstCellNum; j < cellCount; j++)
                    dataRow[j] = row.GetCell(j).ToString();
            }

            ExcelFileStream.Close();
            workbook = null;
            sheet = null;
            return table;
        }
        /// <summary>
        /// 将Excel转换为Datatable
        /// </summary>
        /// <param name="ExcelFileStream"></param>
        /// <param name="SheetIndex"></param>
        /// <param name="HeaderRowIndex"></param>
        /// <returns></returns>
        public static DataTable RenderDataTableFromExcel(Stream ExcelFileStream, int SheetIndex, int HeaderRowIndex)
        {
            HSSFWorkbook workbook = new HSSFWorkbook(ExcelFileStream);
            ISheet sheet = workbook.GetSheetAt(SheetIndex);

            DataTable table = new DataTable();

            IRow headerRow = sheet.GetRow(HeaderRowIndex);
            int cellCount = headerRow.LastCellNum;

            for (int i = headerRow.FirstCellNum; i < cellCount; i++)
            {
                DataColumn column = new DataColumn(headerRow.GetCell(i).StringCellValue);
                table.Columns.Add(column);
            }

            int rowCount = sheet.LastRowNum;

            for (int i = (sheet.FirstRowNum + 1); i < sheet.LastRowNum; i++)
            {
                IRow row = sheet.GetRow(i);
                DataRow dataRow = table.NewRow();

                for (int j = row.FirstCellNum; j < cellCount; j++)
                {
                    if (row.GetCell(j) != null)
                        dataRow[j] = row.GetCell(j).ToString();
                }

                table.Rows.Add(dataRow);
            }

            ExcelFileStream.Close();
            workbook = null;
            sheet = null;
            return table;
        }

        #endregion
    }
}
