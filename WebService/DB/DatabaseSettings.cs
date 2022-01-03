namespace WebService.DB
{
    public class DatabaseSettings
    {
        // name or network address of the instance of SQL Server to connect to
        public string DataSource { get; set; }

        // name of the database associated with the connection
        public string InitialCatalog { get; set; }

        // time (in seconds) to wait for a connection to the server before terminating the attempt and generating an error
        public int Timeout { get; set; }
    }
}
