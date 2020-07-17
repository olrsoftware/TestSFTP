using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestSFTP
{
    class Program
    {
        static void Main(string[] args)
        {
            string host = args[0];
            string username = args[1];
            string password = args[2];
            string remoteDirectory = args[3];

            using (SftpClient sftp = new SftpClient(host, username, password))
            {
                try
                {
                    sftp.Connect();

                    var files = sftp.ListDirectory(remoteDirectory);
                    if (!sftp.Exists("/test"))
                    {
                        sftp.CreateDirectory("/test");
                    }

                    foreach (var file in files)
                    {
                        Console.WriteLine(file.Name);
                        var newFile = Path.Combine("test", file.Name);
                        Directory.CreateDirectory(Path.GetDirectoryName(newFile));
                        using (FileStream stream = new FileStream(newFile, FileMode.Create, FileAccess.ReadWrite, FileShare.None))
                        {
                            sftp.DownloadFile(file.FullName, stream);
                        }
                        using (FileStream stream = new FileStream(newFile, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
                        {
                            sftp.UploadFile(stream, string.Format("/test/{0}", file.Name),true);
                        }
                    }
                    sftp.Disconnect();
                }
                catch (Exception e)
                {
                    Console.WriteLine("An exception has been caught " + e.ToString());
                }
            }
        }
    }
}
