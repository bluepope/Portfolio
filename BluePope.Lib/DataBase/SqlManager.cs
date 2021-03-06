﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml;

namespace BluePope.Lib.DataBase
{
    public class SqlManager
    {
        public static string RootPath { get; set; } = AppDomain.CurrentDomain.BaseDirectory;
        string _filePath;
        Dictionary<string, string> _sqlDictionary = new Dictionary<string, string>();
        DateTime _fileWriteTime;

        public SqlManager(string xmlPath)
        {
            _filePath = System.IO.Path.Combine(RootPath, "Sql", xmlPath);
            LoadSql();
        }

        public void LoadSql()
        {
            var xml = new XmlDocument();

            _fileWriteTime = System.IO.File.GetLastWriteTime(_filePath);
            xml.LoadXml(System.IO.File.ReadAllText(_filePath));

            _sqlDictionary.Clear();
            foreach (XmlNode node in xml["mapper"].ChildNodes)
            {
                _sqlDictionary[node.Attributes["id"].Value.ToLower()] = node.InnerText.Trim();
            }

            xml = null;
        }

        public string GetSql(string sqlId)
        {
            if (_sqlDictionary.ContainsKey(sqlId.ToLower()) == false)
                throw new Exception("Xml에 요청한 Sql Id가 존재하지 않습니다 " + sqlId.ToLower());

            if (_fileWriteTime < System.IO.File.GetLastWriteTime(_filePath))
                LoadSql();

            return _sqlDictionary[sqlId.ToLower()];
        }
    }
}
