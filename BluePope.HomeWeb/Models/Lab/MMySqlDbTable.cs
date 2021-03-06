﻿using BluePope.Lib.DataBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BluePope.HomeWeb.Models.Lab
{
    public class MMySqlDbTable
    {
        public bool isActive { get; set; } = false;

        public string TABLE_NAME { get; set; }
        public string ENGINE { get; set; }

        public string COLUMN_NAME { get; set; }
        public string DATA_TYPE { get; set; }
        public ulong? CHARACTER_MAXIMUM_LENGTH { get; set; }
        public string COLUMN_TYPE { get; set; }
        public string IS_NULLABLE { get; set; }
        public string COLUMN_COMMENT { get; set; }
        public string COLUMN_KEY { get; set; }

        public string ConvertDataType
        {
            get
            {
                if (this.DATA_TYPE.IsNull())
                    return null;

                string data_type = this.DATA_TYPE.ToLower();
                    
                if (data_type.In("smallint", "tinyint"))
                {
                    if (this.COLUMN_TYPE.IndexOf("unsigned") > -1)
                        return $"ushort{(this.IS_NULLABLE == "YES" ? "?" : "")}";
                    else
                        return $"short{(this.IS_NULLABLE == "YES" ? "?" : "")}";
                }
                if (data_type.In("bigint"))
                {
                    if (this.COLUMN_TYPE.IndexOf("unsigned") > -1)
                        return $"ulong{(this.IS_NULLABLE == "YES" ? "?" : "")}";
                    else
                        return $"long{(this.IS_NULLABLE == "YES" ? "?" : "")}";
                }
                else if (data_type.In("mediumint", "int"))
                {
                    if (this.COLUMN_TYPE.IndexOf("unsigned") > -1)
                        return $"uint{(this.IS_NULLABLE == "YES" ? "?" : "")}";
                    else
                        return $"int{(this.IS_NULLABLE == "YES" ? "?" : "")}";
                }
                else if (data_type.In("bit"))
                {
                    return $"bool{(this.IS_NULLABLE == "YES" ? "?" : "")}";
                }
                else if (data_type.In("datetime", "date", "timestamp"))
                {
                    return $"DateTime{(this.IS_NULLABLE == "YES" ? "?" : "")}";
                }
                else if (data_type.In("char", "varchar", "tinytext", "text", "mediumtext", "longtext"))
                {
                    return "string";
                }
                else
                {
                    return this.DATA_TYPE;
                }
            }
        }

        public static async Task<IEnumerable<MMySqlDbTable>> GetTableList(string schemaName)
        {
            return await MySqlDapperHelper.Instance.GetQueryAsync<MMySqlDbTable>(@"
SELECT
	TABLE_NAME
	,ENGINE
FROM
	INFORMATION_SCHEMA.tables 
WHERE
	TABLE_SCHEMA = @schemaName
ORDER BY
	TABLE_NAME
", new { schemaName = schemaName.ToLower() });
        }

        public static async Task<IEnumerable<MMySqlDbTable>> GetTableColumnsList(string schemaName, string tableName)
        {
            return await MySqlDapperHelper.Instance.GetQueryAsync<MMySqlDbTable>(@"
SELECT
	COLUMN_NAME
	,DATA_TYPE
	,CHARACTER_MAXIMUM_LENGTH
    ,IS_NULLABLE
    ,COLUMN_COMMENT
    ,COLUMN_KEY
    ,COLUMN_TYPE
FROM
	INFORMATION_SCHEMA.COLUMNS 
WHERE
	TABLE_SCHEMA = @schemaName
	AND TABLE_NAME = @tableName 
ORDER BY
	ORDINAL_POSITION
", new
            {
                schemaName = schemaName.ToLower(),
                tableName = tableName.ToLower()
            });
        }

        public static string GetClassModel(IEnumerable<MMySqlDbTable> list, string modelName)
        {
            var sb = new StringBuilder();

            sb.AppendLine($"public class {modelName}");
            sb.AppendLine("{");
            foreach (var item in list)
            {
                if (item.COLUMN_COMMENT.IsNull() == false)
                {
                    sb.AppendLine("\t/// <summary>");
                    sb.AppendLine($"\t/// {item.COLUMN_COMMENT}");
                    sb.AppendLine("\t/// </summary>");
                }
                sb.AppendLine($"\tpublic {item.ConvertDataType} {item.COLUMN_NAME} {{ get; set; }}");
            }
            sb.AppendLine("}");

            return sb.ToString();
        }

        public static string GetXmlSqlString(IEnumerable<MMySqlDbTable> list, string tableName)
        {
            tableName = tableName.Substring(0, 1).ToUpper() + tableName.Substring(1);
            var sb = new StringBuilder();
            var isFirst = true;

            //Select
            sb.AppendLine($"<select id=\"Get{tableName}\">");
            sb.AppendLine("    <![CDATA[");
            sb.AppendLine("SELECT");
            isFirst = true;
            foreach (var item in list)
            {
                if (isFirst)
                {
                    isFirst = false;
                    sb.AppendLine($"\tA.{item.COLUMN_NAME}");
                }
                else
                {
                    sb.AppendLine($"\t,A.{item.COLUMN_NAME}");
                }
            }
            sb.AppendLine("FROM");
            sb.AppendLine($"\t{tableName} A");

            sb.AppendLine("WHERE");
            isFirst = true;
            foreach (var item in list.Where(p => p.COLUMN_KEY == "PRI"))
            {
                if (isFirst)
                {
                    isFirst = false;
                    sb.AppendLine($"\tA.{item.COLUMN_NAME} = @{item.COLUMN_NAME}");
                }
                else
                {
                    sb.AppendLine($"\tAND A.{item.COLUMN_NAME} = @{item.COLUMN_NAME}");
                }
            }
            sb.AppendLine("    ]]>");
            sb.AppendLine("</select>");

            //Insert
            sb.AppendLine();
            sb.AppendLine($"<insert id=\"Insert{tableName}\">");
            sb.AppendLine("    <![CDATA[");
            sb.AppendLine($"INSERT INTO {tableName} (");
            isFirst = true;
            foreach (var item in list)
            {
                if (isFirst)
                {
                    isFirst = false;
                    sb.AppendLine($"\t{item.COLUMN_NAME}");
                }
                else
                {
                    sb.AppendLine($"\t,{item.COLUMN_NAME}");
                }
            }
            sb.AppendLine(")");
            sb.AppendLine("SELECT");
            isFirst = true;
            foreach (var item in list)
            {
                if (isFirst)
                {
                    isFirst = false;
                    sb.AppendLine($"\t@{item.COLUMN_NAME}");
                }
                else
                {
                    sb.AppendLine($"\t,@{item.COLUMN_NAME}");
                }
            }
            sb.AppendLine("    ]]>");
            sb.AppendLine("</insert>");

            //Update
            sb.AppendLine();
            sb.AppendLine($"<update id=\"Update{tableName}\">");
            sb.AppendLine("    <![CDATA[");
            sb.AppendLine($"UPDATE {tableName}");
            sb.AppendLine($"SET");
            isFirst = true;
            foreach (var item in list.Where(p => p.COLUMN_KEY != "PRI"))
            {
                if (isFirst)
                {
                    isFirst = false;
                    sb.AppendLine($"\t{item.COLUMN_NAME} = @{item.COLUMN_NAME}");
                }
                else
                {
                    sb.AppendLine($"\t,{item.COLUMN_NAME} = @{item.COLUMN_NAME}");
                }
            }

            sb.AppendLine("WHERE");
            isFirst = true;
            foreach (var item in list.Where(p => p.COLUMN_KEY == "PRI"))
            {
                if (isFirst)
                {
                    isFirst = false;
                    sb.AppendLine($"\t{item.COLUMN_NAME} = @{item.COLUMN_NAME}");
                }
                else
                {
                    sb.AppendLine($"\tAND {item.COLUMN_NAME} = @{item.COLUMN_NAME}");
                }
            }
            sb.AppendLine("    ]]>");
            sb.AppendLine("</update>");

            //Delete
            sb.AppendLine();
            sb.AppendLine($"<delete id=\"Delete{tableName}\">");
            sb.AppendLine("    <![CDATA[");
            sb.AppendLine($"DELETE FROM {tableName}");
            sb.AppendLine("WHERE");
            isFirst = true;
            foreach (var item in list.Where(p => p.COLUMN_KEY == "PRI"))
            {
                if (isFirst)
                {
                    isFirst = false;
                    sb.AppendLine($"\t{item.COLUMN_NAME} = @{item.COLUMN_NAME}");
                }
                else
                {
                    sb.AppendLine($"\tAND {item.COLUMN_NAME} = @{item.COLUMN_NAME}");
                }
            }
            sb.AppendLine("    ]]>");
            sb.AppendLine("</delete>");

            return sb.ToString();
        }
    }
}
