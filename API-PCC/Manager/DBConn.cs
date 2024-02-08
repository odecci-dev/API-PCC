namespace API_PCC.Manager
{
    public class DBConn
    {
        private static string GetPath()
        {
            return "C:\\Files\\"; //Filepath
        }
        public static string Path
        {
            get
            {
                return GetPath();
            }
        }
    }
}
