using Fixture02.Models;
using Microsoft.SqlServer.Server;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Fixture02.Controllers
{
    public class FileController : Controller
    {
        // GET: File
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 导入Excel
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        ///   
        
        [HttpPost]
        public JsonResult ExcelToUpload(HttpPostedFileBase file, String table)
        {
            DataTable excelTable = new DataTable();
            string msg = string.Empty;
            if (Request.Files.Count > 0)
            {
                try
                {
                    HttpPostedFileBase mypost = Request.Files[0];
                    string fileName = Request.Files[0].FileName;
                    //string table = fileModel.table;
                    string serverpath = Server.MapPath(string.Format("~/{0}",""));
                    string path = Path.Combine(serverpath, fileName);
                    mypost.SaveAs(path);
                    excelTable = ImportExcel.GetExcelDataTable(path);

                    ////注意Excel表内容格式，第一行必须为列名与数据库列名匹配
                    ////接下来为各列名对应下来的内容  
                    
                    msg = SaveExcelToDB.InsertDataToDB(excelTable,table);// 写入基础数据
                    //msg = SaveExcelToDB.InsAndDelDataToDB(excelTable, "Key", 1, ”Table“);// 写入基础数据，并删除其中的重复的项目                 
                    //msg = SaveExcelToDB.UpdateDataToDB(excelTable, "[GamesList]");// 修改对应列
                }
                catch (Exception ex)
                {
                    msg = ex.Message;
                }
            }
            else
            {
                msg = "请选择文件";
            }
            return Json(msg);
        }
    }


    static public class ImportExcel
    {
        public static DataTable GetExcelDataTable(string filePath)
        {
            IWorkbook Workbook;
            DataTable table = new DataTable();
            try
            {
                using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    //XSSFWorkbook 适用XLSX格式，HSSFWorkbook 适用XLS格式
                    string fileExt = Path.GetExtension(filePath).ToLower();
                    if (fileExt == ".xls")
                    {
                        Workbook = new HSSFWorkbook(fileStream);
                    }
                    else if (fileExt == ".xlsx")
                    {
                        Workbook = new XSSFWorkbook(fileStream);
                    }
                    else
                    {
                        Workbook = null;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            //定位在第一个sheet
            ISheet sheet = Workbook.GetSheetAt(0);
            //第一行为标题行
            IRow headerRow = sheet.GetRow(0);
            int cellCount = headerRow.LastCellNum;
            int rowCount = sheet.LastRowNum;

            //循环添加标题列
            for (int i = headerRow.FirstCellNum; i < cellCount; i++)
            {
                //IRow row = sheet.GetRow(i);
                //ICell cell = row.GetCell(i);
                DataColumn column;
                //if (cell.CellType == CellType.Numeric)
                //{
                //    //NPOI中数字和日期都是NUMERIC类型的，这里对其进行判断是否是日期类型
                //    if (HSSFDateUtil.IsCellDateFormatted(cell))//日期类型
                //    {

                //        column = new DataColumn(headerRow.GetCell(i).StringCellValue, typeof(DateTime));

                //    }
                //    else//其他数字类型
                //    {

                //        column = new DataColumn(headerRow.GetCell(i).StringCellValue, typeof(int));
                //    }
                //}
                //else
                    column = new DataColumn(headerRow.GetCell(i).StringCellValue);
                table.Columns.Add(column);
            }

            //数据
            for (int i = (sheet.FirstRowNum + 1); i <= rowCount; i++)
            {
                IRow row = sheet.GetRow(i);
                DataRow dataRow = table.NewRow();
                if (row != null)
                {
                    for (int j = row.FirstCellNum; j < cellCount; j++)
                    {
                        if (row.GetCell(j) != null)
                        {
                            ICell cell = row.GetCell(j);
                            if (cell.CellType == CellType.Numeric)
                            {
                                //NPOI中数字和日期都是NUMERIC类型的，这里对其进行判断是否是日期类型
                                if (HSSFDateUtil.IsCellDateFormatted(cell))//日期类型
                                {
                                    dataRow[j] = cell.DateCellValue;
                                    
                                }
                                else//其他数字类型
                                {
                                    dataRow[j] = cell.NumericCellValue;
                                  
                                }
                            }
                            else
                                dataRow[j] = GetCellValue(row.GetCell(j));
                        }
                    }
                }
                table.Rows.Add(dataRow);
            }
            return table;
        }

        private static string GetCellValue(ICell cell)
        {
            if (cell == null)
            {
                return string.Empty;
            }

            switch (cell.CellType)
            {
                case CellType.Blank:
                    return string.Empty;
                case CellType.Boolean:
                    return cell.BooleanCellValue.ToString();
                case CellType.Error:
                    return cell.ErrorCellValue.ToString();
         
                case CellType.Unknown:
                default:
                    return cell.ToString();
                case CellType.String:
                    return cell.StringCellValue;
                case CellType.Formula:
                    try
                    {
                        HSSFFormulaEvaluator e = new HSSFFormulaEvaluator(cell.Sheet.Workbook);
                        e.EvaluateInCell(cell);
                        return cell.ToString();
                    }
                    catch
                    {
                        return cell.NumericCellValue.ToString();
                    }
            }
        }

    }



    public static class SaveExcelToDB
    {
        
        #region 将DataTable中数据写入数据库中
        /// <summary>
        /// 将DataTable中数据写入数据库中
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="TableName">表名</param>
        /// <returns></returns>
        public static string InsertDataToDB(DataTable dt, string TableName)
        {
            int ret = 0;
            if (dt == null || dt.Rows.Count == 0)
            {
                return "Excel无内容";
            }
            //数据库表名
            string tname = TableName;
            //获取要插入列的名字（为动态，由Excel列数决定）
            string colNames = "";
            //循环获取列名
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                colNames += dt.Columns[i].ColumnName + ",";
            }
            //去除最后一位‘，’防止SQL语句错误
            colNames = colNames.TrimEnd(',');
            //定义SQL语句
            string cmd = "";
            

            //定义获取对应列的内容变量
            string colValues;
            //初始SQL语句
            string cmdmode = string.Format("insert into {0}({1}) values({{0}});", tname, colNames);
            string cmdbegin = string.Format(" set identity_insert {0} ON ", tname);
            string cmdend = string.Format(" set identity_insert {0} OFF ", tname);
            //第一个循环，遍历每一行





            for (int i = 0; i < dt.Rows.Count; i++)
            {
                colValues = "";
                //第二个循环，遍历第每一列
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    //如果为空，就跳出循环
                    if (dt.Rows[i][j].GetType() == typeof(DBNull))
                    {
                        colValues += "NULL,";
                        continue;
                    }
                    //接下来可调试寻找规律，如有不解，欢迎留言
                    if (dt.Columns[j].DataType == typeof(DateTime))
                    {
                        colValues += string.Format("cast('{0}' as datetime),", dt.Rows[i][j]);
                    }

                    else if (dt.Columns[j].DataType == typeof(int) || dt.Columns[j].DataType == typeof(float) || dt.Columns[j].DataType == typeof(double))
                    {
                        colValues += string.Format("{0},", dt.Rows[i][j] +"111");
                    }
                    else
                    if (dt.Columns[j].DataType == typeof(string))
                        colValues += string.Format("'{0}',", dt.Rows[i][j]);
                    else if (dt.Columns[j].DataType == typeof(bool))
                    {
                        colValues += string.Format("{0},", dt.Rows[i][j].ToString());
                    }
                    else
                        colValues += string.Format("'{0}',", dt.Rows[i][j]);
                }
                cmd = string.Format(cmdmode, colValues.TrimEnd(','));
                try
                {
                    SqlConnection conn = new SqlConnection(@"Data Source=(local);Initial Catalog=fixture;Integrated Security=True;MultipleActiveResultSets=True;Application Name=EntityFramework");
                    conn.Open();
                    SqlCommand cmd1 = new SqlCommand(cmd, conn);
                    SqlCommand cmdbegin1 = new SqlCommand(cmdbegin, conn);
                    SqlCommand cmdend1 = new SqlCommand(cmdend, conn);
    
                    if(tname.Equals("Jigitem"))
                    cmdbegin1.ExecuteNonQuery();
                    ret = cmd1.ExecuteNonQuery();
                    if (tname.Equals("Jigitem"))
                        cmdend1.ExecuteNonQuery();
                        //执行SQL插入语句（即cmd），获取结果（次方法有各自系统或框架决定）；
                    if (ret <= 0)
                    {
                        return "Excel导入失败，请检查匹配";
                    }
                }
                catch (Exception e)
                {
                    //写错误日志...

                    string strOuput = string.Format("向数据库中写数据失败,错误信息:{0},异常{1}\n", e.Message, e.InnerException);
                    return strOuput;
                }
            }
            return "Excel导入成功";

        }
        #endregion

        #region 写入基础数据，并删除其中的重复的项目
        /// <summary>
        /// 写入基础数据，并删除其中的重复的项目
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="KeyName">主键</param>
        /// <param name="icol">主键所在的列,起始为1</param>
        /// <param name="TableName">表名</param>
        /// <returns></returns>
        public static string InsAndDelDataToDB(DataTable dt, string KeyName, int icol, string TableName)
        {
            //删除数据库中的重复项目
            string mKeyStr = "";
            string tableName = TableName;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                mKeyStr += "'" + dt.Rows[i][icol - 1] + "',";
            }
            mKeyStr = mKeyStr.Trim(',');
            string sqlStr = "Delete from " + tableName + " where " + KeyName + " in (" + mKeyStr + ")";
            ////执行SQL删除语句（即cmd），获取结果（次方法有各自系统或框架决定）；

            //向数据库中写入新的数据
            string tname = tableName;
            string colNames = "";
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                colNames += dt.Columns[i].ColumnName + ",";
            }
            colNames = colNames.TrimEnd(',');
            //colNames = colNames + "CreateDate ";
            int ret = 0;
            string cmd = "";
            string colValues;
            string cmdmode = string.Format("insert into {0}({1}) values({{0}});", tname, colNames);
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                colValues = "";
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    if (dt.Rows[i][j].GetType() == typeof(DBNull))
                    {
                        colValues += "NULL,";
                        continue;
                    }
                    if (dt.Columns[j].DataType == typeof(string))
                    {
                        colValues += string.Format("'{0}',", dt.Rows[i][j]);
                    }
                    else if (dt.Columns[j].DataType == typeof(int) || dt.Columns[j].DataType == typeof(float) || dt.Columns[j].DataType == typeof(double))
                    {
                        colValues += string.Format("{0},", dt.Rows[i][j]);
                    }
                    else if (dt.Columns[j].DataType == typeof(DateTime))
                    {
                        colValues += string.Format("cast('{0}' as datetime),", dt.Rows[i][j]);
                    }
                    else if (dt.Columns[j].DataType == typeof(bool))
                    {
                        colValues += string.Format("{0},", dt.Rows[i][j].ToString());
                    }
                    else
                        colValues += string.Format("'{0}',", dt.Rows[i][j]);
                }
                //colValues += "getdate()";  记录更新时间
                cmd = string.Format(cmdmode, colValues.TrimEnd(','));
                try
                {
                   //// ret = 执行SQL插入语句（即cmd），获取结果（次方法有各自系统或框架决定）；
                    if (ret <= 0)
                    {
                        return "Excel导入失败，请检查匹配";
                    }
                }
                catch (Exception e)
                {
                    //写错误日志...
                    string strOuput = string.Format("向数据库中写数据失败,错误信息:{0},异常{1}\n", e.Message, e.InnerException);
                    return strOuput;
                    //将信息写入到日志输出文件
                    //DllComm.TP_WriteAppLogFileEx(DllComm.g_AppLogFileName, strOuput);

                }
            }
            return "Excel导入成功";
        }
        #endregion

        #region 将数据库相同唯一键的信息替换成DataTable对应唯一键外的信息（一个唯一键，一个修改值）
        /// <summary>
        /// 将数据库相同唯一键的信息替换成DataTable对应唯一键外的信息（一个唯一键，一个修改值）
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="TableName">表名</param>
        /// <returns></returns>
        public static string UpdateDataToDB(DataTable dt, string TableName)
        {
            if (dt == null || dt.Rows.Count == 0)
            {
                return "Excel无内容";
            }
            string tname = TableName;
            string keyOnly = dt.Columns[0].ColumnName;
            string modifyItem = dt.Columns[1].ColumnName;
            string cmd = "";
            int ret = 0;
            string keyOnlyValue, modifyItemValue;
            string cmdmode = string.Format("update {0} set {1}={{0}} where {2}={{1}};", tname, modifyItem, keyOnly);
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                keyOnlyValue = "";
                modifyItemValue = "";
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    if (dt.Rows[i][j].GetType() == typeof(DBNull))
                    {
                        keyOnlyValue = "NULL";
                        modifyItemValue = "NULL";
                        continue;
                    }
                    if (dt.Columns[j].DataType == typeof(string))
                    {
                        keyOnlyValue = string.Format("'{0}'", dt.Rows[i][0]);
                        modifyItemValue = string.Format("'{0}'", dt.Rows[i][1]);
                    }
                    else if (dt.Columns[j].DataType == typeof(int) || dt.Columns[j].DataType == typeof(float) ||
                             dt.Columns[j].DataType == typeof(double))
                    {
                        keyOnlyValue = string.Format("{0}", dt.Rows[i][0]);
                        modifyItemValue = string.Format("{0}", dt.Rows[i][1]);
                    }
                    else if (dt.Columns[j].DataType == typeof(DateTime))
                    {
                        keyOnlyValue = string.Format("cast('{0}' as datetime),", dt.Rows[i][0]);
                        modifyItemValue = string.Format("cast('{0}' as datetime),", dt.Rows[i][1]);
                    }
                    else if (dt.Columns[j].DataType == typeof(bool))
                    {
                        keyOnlyValue = string.Format("{0}", dt.Rows[i][j].ToString());
                        modifyItemValue = string.Format("{0}", dt.Rows[i][j].ToString());
                    }
                    else
                    {
                        keyOnlyValue = string.Format("'{0}'", dt.Rows[i][0]);
                        modifyItemValue = string.Format("'{0}'", dt.Rows[i][1]);
                    }

                }
                cmd = string.Format(cmdmode, modifyItemValue, keyOnlyValue);
                try
                {
                    ////ret = 执行SQL修改语句（即cmd），获取结果（次方法有各自系统或框架决定）；
                    if (ret <= 0)
                    {
                        return "Excel导入失败，请检查匹配";
                    }
                }
                catch (Exception e)
                {
                    //写错误日志...
                    string strOuput = string.Format("向数据库中写数据失败,错误信息:{0},异常{1}\n", e.Message, e.InnerException);
                    return strOuput;
                }
            }
            return "Excel导入成功";

        }
        #endregion
    }

}
