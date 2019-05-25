using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoreLib.DataBase
{
    public class ConnectionStringModel
    {
        static ConnectionStringModel _instance;
        public string ConnectionString { get; set; }

        public static ConnectionStringModel Instance
        {
            get
            {
                if (_instance == null)
                {
                    object _lockObject = new Object();

                    lock (_lockObject)
                    {
                        if (_instance == null)
                        {
                            var path = System.IO.Path.Combine(System.Environment.CurrentDirectory, "dbconfig.json");

                            if (System.IO.File.Exists(path))
                            {
                                _instance = JsonConvert.DeserializeObject<ConnectionStringModel>(System.IO.File.ReadAllText(path));
                            }
                            else
                            {
                                System.IO.File.WriteAllText(path, JsonConvert.SerializeObject(new ConnectionStringModel()
                                {
                                    ConnectionString = "connectionstring"
                                }));

                                throw new Exception($"{path} connection string is empty");
                            }
                        }
                    }
                }

                return _instance;
            }
        }

    }
}
